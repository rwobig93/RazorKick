using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Constants.Communication;
using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Helpers.Communication;
using Application.Helpers.Identity;
using Application.Helpers.Web;
using Application.Mappers.Identity;
using Application.Models.Identity;
using Application.Models.Lifecycle;
using Application.Models.Web;
using Application.Repositories.Identity;
using Application.Requests.Identity.User;
using Application.Responses.Identity;
using Application.Services.Identity;
using Application.Services.Lifecycle;
using Application.Services.System;
using Application.Settings.AppSettings;
using Blazored.LocalStorage;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Database;
using Domain.Enums.Identity;
using Domain.Models.Database;
using Domain.Models.Identity;
using FluentEmail.Core;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using IResult = Application.Models.Web.IResult;

namespace Infrastructure.Services.Identity;

public class AppAccountService : IAppAccountService 
{
    private readonly IAppUserRepository _userRepository;
    private readonly IFluentEmail _mailService;
    private readonly IAppRoleRepository _roleRepository;
    private readonly IAppPermissionRepository _appPermissionRepository;
    private readonly AppConfiguration _appConfig;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthStateProvider _authProvider;
    private readonly HttpClient _httpClient;
    private readonly IDateTimeService _dateTime;
    private readonly IRunningServerState _serverState;
    private readonly ISerializerService _serializer;
    private readonly IAuditTrailService _auditService;

    public AppAccountService(IOptions<AppConfiguration> appConfig, IAppPermissionRepository appPermissionRepository, IAppRoleRepository 
    roleRepository,
        IAppUserRepository userRepository, ILocalStorageService localStorage, AuthStateProvider authProvider, IHttpClientFactory httpClientFactory,
        IFluentEmail mailService, IDateTimeService dateTime, IRunningServerState serverState, ISerializerService serializer,
        IAuditTrailService auditService)
    {
        _appConfig = appConfig.Value;
        _appPermissionRepository = appPermissionRepository;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _localStorage = localStorage;
        _authProvider = authProvider;
        _httpClient = httpClientFactory.CreateClient("Default");
        _mailService = mailService;
        _dateTime = dateTime;
        _serverState = serverState;
        _serializer = serializer;
        _auditService = auditService;
    }

    public async Task<IResult<UserLoginResponse>> LoginAsync(UserLoginRequest loginRequest)
    {
        // TODO: Add bad password lockout on configurable attempt count, reset sets to 0, successful login sets to 0
        var user = (await _userRepository.GetByUsernameAsync(loginRequest.Username)).Result;
        if (user is null)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.CredentialsInvalidError);
        
        var passwordValid = AccountHelpers.IsPasswordCorrect(
            loginRequest.Password, user.PasswordSalt, user.PasswordHash);
        if (!passwordValid)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.CredentialsInvalidError);
        if (!user.EmailConfirmed)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.EmailNotConfirmedError);
        if (user.AuthState == AuthState.Disabled)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.AccountDisabledError);
        if (user.AuthState == AuthState.LockedOut)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.AccountLockedOutError);

        user.LastModifiedBy = _serverState.SystemUserId;
        user.LastModifiedOn = _dateTime.NowDatabaseTime;
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_appConfig.TokenExpirationDays);
        
        var update = await _userRepository.UpdateAsync(user.ToUpdate());
        if (!update.Success)
            return await Result<UserLoginResponse>.FailAsync(update.ErrorMessage);

        var token = await GenerateJwtAsync(user);
        var response = new UserLoginResponse() { Token = token, RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };

        if (_serverState.AuditLoginLogout)
        {
            await _auditService.CreateAsync(new AuditTrailCreate
            {
                TableName = "AuthState",
                RecordId = user.Id,
                ChangedBy = _serverState.SystemUserId,
                Action = DatabaseActionType.Login,
                Before = _serializer.Serialize(new Dictionary<string, string>() {{"Username", ""}, {"Email", ""}}),
                After = _serializer.Serialize(new Dictionary<string, string>() {{"Username", user.Username}, {"Email", user.Email}})
            });
        }
        
        return await Result<UserLoginResponse>.SuccessAsync(response);
    }

    public async Task<IResult<UserLoginResponse>> LoginGuiAsync(UserLoginRequest loginRequest)
    {
        try
        {
            var loginResponse = await LoginAsync(loginRequest);
            if (!loginResponse.Succeeded)
                return loginResponse;
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Data.Token);

            await _localStorage.SetItemAsync(LocalStorageConstants.AuthToken, loginResponse.Data.Token);
            await _localStorage.SetItemAsync(LocalStorageConstants.AuthTokenRefresh, loginResponse.Data.RefreshToken);

            await _authProvider.GetAuthenticationStateAsync(loginResponse.Data.Token);

            return await Result<UserLoginResponse>.SuccessAsync(loginResponse.Data);
        }
        catch (Exception ex)
        {
            return await Result<UserLoginResponse>.FailAsync($"Failure occurred attempting to login: {ex.Message}");
        }
    }

    private static Guid GetIdFromPrincipal(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
        var isGuid = Guid.TryParse(userIdClaim?.Value, out var userId);
        
        return !isGuid ? Guid.Empty : userId;
    }

    public async Task<IResult> LogoutGuiAsync(Guid userId)
    {
        try
        {
            if (_serverState.AuditLoginLogout)
            {
                if (userId == Guid.Empty)
                    userId = GetIdFromPrincipal(_authProvider.AuthenticationStateUser);
                var user = (await _userRepository.GetByIdAsync(userId)).Result ??
                           new AppUserDb() {Username = "Unknown", Email = "User@Unknown.void"};

                await _auditService.CreateAsync(new AuditTrailCreate
                {
                    TableName = "AuthState",
                    RecordId = userId,
                    ChangedBy = _serverState.SystemUserId,
                    Action = DatabaseActionType.Logout,
                    Before = _serializer.Serialize(new Dictionary<string, string>() {{"Username", user.Username}, {"Email", user.Email}}),
                    After = _serializer.Serialize(new Dictionary<string, string>() {{"Username", ""}, {"Email", ""}})
                });
            }
            
            await _localStorage.RemoveItemAsync(LocalStorageConstants.AuthToken);
            await _localStorage.RemoveItemAsync(LocalStorageConstants.AuthTokenRefresh);
            
            _authProvider.DeauthenticateUser();
            _httpClient.DefaultRequestHeaders.Authorization = null;

            return await Result.SuccessAsync();
        }
        catch
        {
            return await Result.FailAsync();
        }
    }

    public async Task<IResult<bool>> PasswordMeetsRequirements(string password)
    {
        try
        {
            return await Result<bool>.SuccessAsync(AccountHelpers.DoesPasswordMeetRequirements(password));
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private async Task<DatabaseActionResult<Guid>> CreateAsync(AppUserDb user, string password)
    {
        var createUser = user.ToCreateObject();
        var passwordMeetsRequirements = (await PasswordMeetsRequirements(password)).Data;
        if (!passwordMeetsRequirements)
        {
            var passwordFailResult = new DatabaseActionResult<Guid>();
            passwordFailResult.Fail("Provided password doesn't meet the requirements");
            return passwordFailResult;
        }

        AccountHelpers.GenerateHashAndSalt(password, out var salt, out var hash);
        createUser.CreatedOn = _dateTime.NowDatabaseTime;
        createUser.CreatedBy = _serverState.SystemUserId;
        createUser.RefreshTokenExpiryTime = DateTime.Now;
        createUser.PasswordSalt = salt;
        createUser.PasswordHash = hash;

        return await _userRepository.CreateAsync(createUser);
    }

    public async Task<IResult> RegisterAsync(UserRegisterRequest registerRequest)
    {
        // TODO: Add handling for a deleted account that still exists in the database, reactivate account or reach out to admin for enablement?
        if (!AccountHelpers.IsValidEmailAddress(registerRequest.Email))
            return await Result.FailAsync($"The email address {registerRequest.Email} provided isn't a valid email, please try again");
        
        var matchingEmail = (await _userRepository.GetByEmailAsync(registerRequest.Email)).Result;
        if (matchingEmail is not null)
            return await Result.FailAsync($"The email address {registerRequest.Email} is already in use, please try again");
        
        var matchingUserName = (await _userRepository.GetByUsernameAsync(registerRequest.Username)).Result;
        if (matchingUserName != null)
        {
            return await Result.FailAsync(string.Format($"Username {registerRequest.Username} is already in use, please try again"));
        }
        
        var newUser = new AppUserDb()
        {
            Email = registerRequest.Email,
            Username = registerRequest.Username,
        };
        
        var createUserResult = await CreateAsync(newUser, registerRequest.Password);
        if (!createUserResult.Success)
            return await Result.FailAsync(createUserResult.ErrorMessage);

        var caveatMessage = "";
        await _userRepository.GetByUsernameAsync(UserConstants.DefaultUsers.SystemUsername);
        var defaultRole = (await _roleRepository.GetByNameAsync(RoleConstants.DefaultRoles.DefaultName)).Result;
        var addToRoleResult = await _roleRepository.AddUserToRoleAsync(createUserResult.Result, defaultRole!.Id);
        if (!addToRoleResult.Success)
            caveatMessage = $",{Environment.NewLine} Default permissions could not be added to this account, " +
                            $"please contact the administrator for assistance";
        
        var confirmationUrl = (await GetEmailConfirmationUrl(createUserResult.Result)).Data;
        if (string.IsNullOrWhiteSpace(confirmationUrl))
            return await Result.FailAsync("Failure occurred generating confirmation URL, please contact the administrator");
        
        // TODO: Look into why calling this as a BackgroundJob.Enqueue(() => send) fails
        var response = await _mailService.SendRegistrationEmail(newUser.Email, newUser.Username, confirmationUrl);

        if (!response.Successful)
            return await Result.FailAsync(
                $"Account was registered successfully but a failure occurred attempting to send an email to " +
                $"the address provided, please contact the administrator for assistance{caveatMessage}");
        
        return await Result<Guid>.SuccessAsync(newUser.Id, 
            $"Account {newUser.Username} successfully registered, please check your email to confirm!{caveatMessage}");
    }

    public async Task<IResult<string>> GetEmailConfirmationUrl(Guid userId)
    {
        var previousConfirmations =
            (await _userRepository.GetUserExtendedAttributesByTypeAsync(userId, ExtendedAttributeType.EmailConfirmationToken)).Result;
        var previousConfirmation = previousConfirmations?.FirstOrDefault();
        
        var endpointUri = new Uri(string.Concat(_appConfig.BaseUrl, AppRouteConstants.Identity.ConfirmEmail));
        var confirmationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", userId.ToString());
        
        // Previous pending account registration exists, return current value
        if (previousConfirmation is not null)
            return await Result<string>.SuccessAsync(QueryHelpers.AddQueryString(
                confirmationUri, "confirmationCode", previousConfirmation.Value));

        // No currently pending account registration exists so we'll generate a new one, add it to the provided user
        //   and return the generated confirmation uri
        var confirmationCode = UrlHelpers.GenerateToken();
        var newExtendedAttribute = new AppUserExtendedAttributeCreate()
        {
            OwnerId = userId,
            Name = "AccountEmailConfirmation",
            Type = ExtendedAttributeType.EmailConfirmationToken,
            Value = confirmationCode
        };
        var addAttributeRequest = await _userRepository.AddExtendedAttributeAsync(newExtendedAttribute);
        if (!addAttributeRequest.Success)
            return await Result<string>.FailAsync(addAttributeRequest.ErrorMessage);
        
        // TODO: Split confirmation code and URI return for API endpoints to register a user programmatically
        return await Result<string>.SuccessAsync(QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", confirmationCode));
    }

    public async Task<IResult<string>> ConfirmEmailAsync(Guid userId, string confirmationCode)
    {
        var foundUser = (await _userRepository.GetByIdAsync(userId)).Result;
        if (foundUser is null)
            return await Result<string>.FailAsync(ErrorMessageConstants.UserNotFoundError);

        var previousConfirmations =
            (await _userRepository.GetUserExtendedAttributesByTypeAsync(foundUser.Id, ExtendedAttributeType.EmailConfirmationToken)).Result;
        var previousConfirmation = previousConfirmations?.FirstOrDefault();

        // TODO: Add admin logging w/ attempt and failure details so an admin can troubleshoot
        if (previousConfirmation is null)
            return await Result<string>.FailAsync(
                $"An error occurred attempting to confirm account: {foundUser.Id}, please contact the administrator");
        if (previousConfirmation.Value != confirmationCode)
            return await Result<string>.FailAsync(
                $"An error occurred attempting to confirm account: {foundUser.Id}, please contact the administrator");
        
        foundUser.EmailConfirmed = true;
        foundUser.AuthState = AuthState.Enabled;
        
        var userUpdate = foundUser.ToUpdate();
        userUpdate.LastModifiedBy = _serverState.SystemUserId;
        userUpdate.LastModifiedOn = _dateTime.NowDatabaseTime;
        
        var confirmEmail = await _userRepository.UpdateAsync(userUpdate);
        if (!confirmEmail.Success)
            return await Result<string>.FailAsync(
                $"An error occurred attempting to confirm account: {foundUser.Id}, please contact the administrator");
        await _userRepository.RemoveExtendedAttributeAsync(previousConfirmation.Id);
        
        return await Result<string>.SuccessAsync(foundUser.Id.ToString(), $"Account Confirmed for {foundUser.Username}.");
    }

    public async Task<IResult> SetUserPassword(Guid userId, string newPassword)
    {
        try
        {
            var updateObject = new AppUserUpdate() { Id = userId };
        
            AccountHelpers.GenerateHashAndSalt(newPassword, out var salt, out var hash);
            updateObject.PasswordSalt = salt;
            updateObject.PasswordHash = hash;
            updateObject.LastModifiedBy = _serverState.SystemUserId;
            updateObject.LastModifiedOn = _dateTime.NowDatabaseTime;
        
            var result = await _userRepository.UpdateAsync(updateObject);
            if (!result.Success)
                return await Result.FailAsync(result.ErrorMessage);
            
            return await Result.SuccessAsync();
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<bool>> IsPasswordCorrect(Guid userId, string password)
    {
        try
        {
            var matchingUser = (await _userRepository.GetByIdAsync(userId)).Result;
            var passwordCorrect = AccountHelpers.IsPasswordCorrect(password, matchingUser!.PasswordSalt, matchingUser.PasswordHash);
            return await Result<bool>.SuccessAsync(passwordCorrect);
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest forgotRequest)
    {
        var foundUser = (await _userRepository.GetByEmailAsync(forgotRequest.Email!)).Result;
        if (foundUser is null)
            return await Result.FailAsync(ErrorMessageConstants.GenericError);

        if (!foundUser.EmailConfirmed)
            return await Result.FailAsync(ErrorMessageConstants.EmailNotConfirmedError);

        var previousResets =
            (await _userRepository.GetUserExtendedAttributesByTypeAsync(foundUser.Id, ExtendedAttributeType.PasswordResetToken)).Result;
        var previousReset = previousResets?.FirstOrDefault();
        
        var endpointUri = new Uri(string.Concat(_appConfig.BaseUrl, AppRouteConstants.Identity.ForgotPassword));
        var confirmationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", foundUser.Id.ToString());
        
        // Previous pending forgot password exists, return current value
        if (previousReset is not null)
        {
            confirmationUri = QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", previousReset.Value);
        }
        else
        {
            // No currently pending forgot password request exists so we'll generate a new one, add it to the provided user
            //   and return the generated reset uri
            var confirmationCode = UrlHelpers.GenerateToken();
            var newExtendedAttribute = new AppUserExtendedAttributeCreate()
            {
                OwnerId = foundUser.Id,
                Name = "ForgotPasswordReset",
                Type = ExtendedAttributeType.PasswordResetToken,
                Value = confirmationCode
            };
            await _userRepository.AddExtendedAttributeAsync(newExtendedAttribute);
            confirmationUri = QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", newExtendedAttribute.Value);
        }

        // TODO: Look into why calling this as a BackgroundJob.Enqueue(() => send) fails
        var response = await _mailService.SendPasswordResetEmail(foundUser.Email, foundUser.Username, confirmationUri);
        if (!response.Successful)
            return await Result.FailAsync(
                "Failure occurred attempting to send the password reset email, please reach out to the administrator");

        return await Result.SuccessAsync("Successfully sent password reset to the email provided!");
    }

    public async Task<IResult> ForgotPasswordConfirmationAsync(Guid userId, string confirmationCode, string password, string confirmPassword)
    {
        var foundUser = (await _userRepository.GetByIdAsync(userId)).Result;
        if (foundUser is null)
            return await Result.FailAsync(ErrorMessageConstants.GenericError);

        var previousResets =
            (await _userRepository.GetUserExtendedAttributesByTypeAsync(foundUser.Id, ExtendedAttributeType.PasswordResetToken)).Result;
        var previousReset = previousResets?.FirstOrDefault();
        
        if (previousReset is null)
            return await Result.FailAsync(ErrorMessageConstants.GenericError);
        if (password != confirmPassword)
            return await Result.FailAsync(ErrorMessageConstants.PasswordsNoMatchError);
        if (confirmationCode != previousReset.Value)
            return await Result.FailAsync(ErrorMessageConstants.TokenInvalidError);

        var passwordMeetsRequirements = (await PasswordMeetsRequirements(password)).Data;
        if (!passwordMeetsRequirements)
            return await Result.FailAsync("Password provided doesn't meet the minimum requirements, please try again");
        
        await SetUserPassword(foundUser.Id, password);

        await _userRepository.RemoveExtendedAttributeAsync(previousReset.Id);

        return await Result.SuccessAsync("Password reset was successful, please log back in with your fresh new password!");
    }

    public async Task<IResult<UserLoginResponse>> GetRefreshTokenAsync(RefreshTokenRequest? refreshRequest)
    {
        if (refreshRequest is null)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.TokenInvalidError);
        
        var userPrincipal = GetPrincipalFromExpiredToken(refreshRequest.Token!);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        
        var user = (await _userRepository.GetByEmailAsync(userEmail)).Result;
        if (user == null)
            return await Result<UserLoginResponse>.FailAsync("User Not Found.");
        
        if (user.RefreshToken != refreshRequest.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            return await Result<UserLoginResponse>.FailAsync("Invalid Client Token.");
        
        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        user.RefreshToken = GenerateRefreshToken();
        user.LastModifiedBy = _serverState.SystemUserId;
        user.LastModifiedOn = _dateTime.NowDatabaseTime;
        
        var update = await _userRepository.UpdateAsync(user.ToUpdate());
        if (!update.Success)
            return await Result<UserLoginResponse>.FailAsync(update.ErrorMessage);

        // TODO: Auth token is failing for users that aren't admin, returning indicating token has been deleted
        var response = new UserLoginResponse { Token = token, RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
        return await Result<UserLoginResponse>.SuccessAsync(response);
    }

    public async Task<IResult> UpdatePreferences(Guid userId, AppUserPreferenceUpdate preferenceUpdate)
    {
        var updateRequest = await _userRepository.UpdatePreferences(userId, preferenceUpdate);
        if (!updateRequest.Success)
            return await Result.FailAsync($"Failure occurred attempting to update preferences: {updateRequest.ErrorMessage}");

        return await Result.SuccessAsync("Preferences updated successfully");
    }

    public async Task<IResult<AppUserPreferenceFull>> GetPreferences(Guid userId)
    {
        var preferences = await _userRepository.GetPreferences(userId);
        if (!preferences.Success)
            return await Result<AppUserPreferenceFull>.FailAsync($"Failure occurred getting preferences: {preferences.ErrorMessage}");

        if (preferences.Result is null)
            return await Result<AppUserPreferenceFull>.FailAsync("Preferences couldn't be found for the UserId provided");

        var preferencesFull = preferences.Result.ToFull();
            
        preferencesFull.CustomThemeOne = _serializer.Deserialize<AppThemeCustom>(preferences.Result.CustomThemeOne!);
        preferencesFull.CustomThemeTwo = _serializer.Deserialize<AppThemeCustom>(preferences.Result.CustomThemeTwo!);
        preferencesFull.CustomThemeThree = _serializer.Deserialize<AppThemeCustom>(preferences.Result.CustomThemeThree!);

        return await Result<AppUserPreferenceFull>.SuccessAsync(preferencesFull);
    }

    public async Task<IResult> ChangeUserEnabledState(Guid userId, bool enabled)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (!user.Success)
            return await Result.FailAsync(user.ErrorMessage);
        
        user.Result!.AuthState = AuthState.Enabled;
        user.Result!.LastModifiedBy = _serverState.SystemUserId;
        user.Result!.LastModifiedOn = _dateTime.NowDatabaseTime;

        var updateRequest = await _userRepository.UpdateAsync(user.Result.ToUpdate());
        if (!updateRequest.Success)
            return await Result.FailAsync(updateRequest.ErrorMessage);

        var action = enabled ? "Enabled" : "Disabled";
        return await Result.SuccessAsync($"User account successfully {action}");
    }

    public async Task<IResult> ForceUserPasswordReset(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (!user.Success)
            return await Result.FailAsync(user.ErrorMessage);

        // TODO: Invalidate any currently active logins for the account / log them out
        await SetUserPassword(userId, UrlHelpers.GenerateToken());
        return await ForgotPasswordAsync(new ForgotPasswordRequest() { Email = user.Result!.Email });
    }

    public async Task<IResult> SetTwoFactorEnabled(Guid userId, bool enabled)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (!user.Success || user.Result is null)
            return await Result.FailAsync(user.ErrorMessage);

        var updateUser = user.Result.ToUpdate();
        updateUser.TwoFactorEnabled = enabled;

        var updateRequest = await _userRepository.UpdateAsync(updateUser);
        if (!updateRequest.Success)
            return await Result.FailAsync(updateRequest.ErrorMessage);

        return await Result.SuccessAsync();
    }

    public async Task<IResult> SetTwoFactorKey(Guid userId, string key)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (!user.Success || user.Result is null)
            return await Result.FailAsync(user.ErrorMessage);

        var updateUser = user.Result.ToUpdate();
        updateUser.TwoFactorKey = key;

        var updateRequest = await _userRepository.UpdateAsync(updateUser);
        if (!updateRequest.Success)
            return await Result.FailAsync(updateRequest.ErrorMessage);

        return await Result.SuccessAsync();
    }

    private async Task<string> GenerateJwtAsync(AppUserDb user)
    {
        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        return token;
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(AppUserDb user)
    {
        var allUserAndRolePermissions = 
            (await _appPermissionRepository.GetAllIncludingRolesForUserAsync(user.Id)).Result?.ToClaims() ?? new List<Claim>();
        var allRoles = 
            (await _roleRepository.GetRolesForUser(user.Id)).Result?.ToClaims() ?? new List<Claim>();

        var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.Username)
            }
        .Union(allUserAndRolePermissions)
        .Union(allRoles);

        return claims;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
           claims: claims,
           expires: DateTime.UtcNow.AddDays(2),
           signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);
        return encryptedToken;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_appConfig.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
}