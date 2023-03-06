using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Constants.Communication;
using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Helpers.Identity;
using Application.Helpers.Runtime;
using Application.Helpers.Web;
using Application.Models.Email;
using Application.Models.Identity;
using Application.Models.Web;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Application.Settings.AppSettings;
using Blazored.LocalStorage;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Identity;
using Domain.Models.Database;
using FluentEmail.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.ApiRoutes.Identity;
using Shared.Requests.Identity.User;
using Shared.Responses.Identity;
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
    private readonly IHttpContextAccessor _contextAccessor;
    
    public AppAccountService(IConfiguration configuration, IAppPermissionRepository appPermissionRepository, IAppRoleRepository roleRepository,
        IAppUserRepository userRepository, ILocalStorageService localStorage, AuthStateProvider authProvider, HttpClient httpClient,
        IHttpContextAccessor contextAccessor, IFluentEmail mailService)
    {
        _appConfig = configuration.GetApplicationSettings();
        _appPermissionRepository = appPermissionRepository;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _localStorage = localStorage;
        _authProvider = authProvider;
        _httpClient = httpClient;
        _contextAccessor = contextAccessor;
        _mailService = mailService;
    }

    public async Task<IResult<UserLoginResponse>> LoginAsync(UserLoginRequest loginRequest)
    {
        var user = (await _userRepository.GetByUsernameAsync(loginRequest.Username)).Result;
        if (user is null)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.CredentialsInvalidError);
        
        var passwordValid = AccountHelpers.IsPasswordCorrect(
            loginRequest.Password, user.PasswordSalt, user.PasswordHash);
        if (!passwordValid)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.CredentialsInvalidError);
        if (!user.IsActive)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.AccountDisabledError);
        if (!user.EmailConfirmed)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.EmailNotConfirmedError);

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_appConfig.TokenExpirationDays);
        var update = await _userRepository.UpdateAsync(user.ToUpdateObject());
        if (!update.Success)
            return await Result<UserLoginResponse>.FailAsync(update.ErrorMessage);

        var token = await GenerateJwtAsync(user);
        var response = new UserLoginResponse() { Token = token, RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
        return await Result<UserLoginResponse>.SuccessAsync(response);
    }

    public async Task<IResult<UserLoginResponse>> LoginGuiAsync(UserLoginRequest loginRequest)
    {
        var authResponse = await LoginAsync(loginRequest);

        await _localStorage.SetItemAsync(LocalStorageConstants.AuthToken, authResponse.Data.Token);
        await _localStorage.SetItemAsync(LocalStorageConstants.AuthTokenRefresh, authResponse.Data.RefreshToken);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.Data.Token);
        
        await _authProvider.GetAuthenticationStateAsync();
        
        return authResponse;
    }

    public async Task<IResult> LogoutGuiAsync()
    {
        try
        {
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

    private async Task<DatabaseActionResult<Guid>> CreateAsync(AppUserDb user, string password)
    {
        var createUser = user.ToCreateObject();

        AccountHelpers.GenerateHashAndSalt(password, out var salt, out var hash);
        createUser.CreatedOn = DateTime.Now;
        createUser.CreatedBy = Guid.Empty;
        createUser.RefreshTokenExpiryTime = DateTime.Now;
        createUser.PasswordSalt = salt;
        createUser.PasswordHash = hash;
        createUser.NormalizedUserName = createUser.Username.NormalizeForDatabase();
        createUser.NormalizedEmail = createUser.Email.NormalizeForDatabase();

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
        var defaultRole = (await _roleRepository.GetByNameAsync(RoleConstants.DefaultRoleName)).Result;
        var addToRoleResult = await _roleRepository.AddUserToRoleAsync(createUserResult.Result, defaultRole!.Id);
        if (!addToRoleResult.Success)
            caveatMessage = $",{Environment.NewLine} Default permissions could not be added to this account, " +
                            $"please contact the administrator for assistance";
        
        // TODO: Re-add email service after validating why it was failing
        var confirmationUrl = await GetEmailConfirmationUrl(createUserResult.Result);
        if (string.IsNullOrWhiteSpace(confirmationUrl))
            return await Result.FailAsync("Failure occurred generating confirmation URL, please contact the administrator");
        
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), EmailConstants.TemplatesPath,
            EmailConstants.PathRegistrationConfirmation);
        var sendResponse = await _mailService.Subject("Registration Confirmation").To(newUser.Email)
            .UsingTemplateFromFile(templatePath, new EmailAction() {ActionUrl = confirmationUrl, Username = newUser.Username}).SendAsync();
        if (!sendResponse.Successful)
            return await Result.FailAsync(string.Join(Environment.NewLine, sendResponse.ErrorMessages));
        
        if (!sendResponse.Successful)
            return await Result.FailAsync(
                $"Account was registered successfully but a failure occurred attempting to send an email to " +
                $"the address provided, please contact the administrator for assistance{caveatMessage}");
        
        return await Result<Guid>.SuccessAsync(newUser.Id, 
            $"Account {newUser.UserName} successfully registered, please check your email to confirm!{caveatMessage}");
    }

    public async Task<string> GetEmailConfirmationUrl(Guid userId)
    {
        var previousConfirmations =
            (await _userRepository.GetUserExtendedAttributesByTypeAsync(userId, ExtendedAttributeType.EmailConfirmationToken)).Result;
        var previousConfirmation = previousConfirmations?.FirstOrDefault();
        
        var endpointUri = new Uri(string.Concat(_appConfig.BaseUrl, AppRouteConstants.Identity.ConfirmEmail));
        var confirmationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", userId.ToString());
        
        // Previous pending account registration exists, return current value
        if (previousConfirmation is not null)
            return QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", previousConfirmation.Value);

        // No currently pending account registration exists so we'll generate a new one, add it to the provided user
        //   and return the generated confirmation uri
        var confirmationCode = UrlHelpers.GenerateToken();
        var newExtendedAttribute = new AppUserExtendedAttributeAdd()
        {
            OwnerId = userId,
            Name = "AccountEmailConfirmation",
            Type = ExtendedAttributeType.EmailConfirmationToken,
            Value = confirmationCode
        };
        var addAttributeRequest = await _userRepository.AddExtendedAttributeAsync(newExtendedAttribute);
        if (!addAttributeRequest.Success)
            return "";
        
        // TODO: Split confirmation code and URI return for API endpoints to register a user programmatically
        return QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", confirmationCode);
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
        foundUser.IsActive = true;
        var userUpdate = foundUser.ToUpdateObject();
        var confirmEmail = await _userRepository.UpdateAsync(userUpdate);
        if (!confirmEmail.Success)
            return await Result<string>.FailAsync(
                $"An error occurred attempting to confirm account: {foundUser.Id}, please contact the administrator");
        await _userRepository.RemoveExtendedAttributeAsync(previousConfirmation.Id);
        
        return await Result<string>.SuccessAsync(foundUser.Id.ToString(), $"Account Confirmed for {foundUser.Username}.");
    }

    public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest forgotRequest)
    {
        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        var foundUser = (await _userRepository.GetByEmailAsync(forgotRequest.Email!)).Result;
        if (foundUser is null)
            return await Result.FailAsync(ErrorMessageConstants.GenericError);

        if (!foundUser.EmailConfirmed)
            return await Result.FailAsync(ErrorMessageConstants.EmailNotConfirmedError);

        var previousResets =
            (await _userRepository.GetUserExtendedAttributesByTypeAsync(foundUser.Id, ExtendedAttributeType.PasswordResetToken)).Result;
        var previousReset = previousResets?.FirstOrDefault();
        
        var endpointUri = new Uri(string.Concat(_appConfig.BaseUrl, UserRoutes.ConfirmEmail));
        var confirmationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", foundUser.Id.ToString());
        
        // Previous pending forgot password exists, return current value
        if (previousReset is not null)
            return await Result.SuccessAsync(QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", previousReset.Value));

        // No currently pending forgot password request exists so we'll generate a new one, add it to the provided user
        //   and return the generated reset uri
        var confirmationCode = UrlHelpers.GenerateToken();
        var newExtendedAttribute = new AppUserExtendedAttributeAdd()
        {
            OwnerId = foundUser.Id,
            Name = "ForgotPasswordReset",
            Type = ExtendedAttributeType.PasswordResetToken,
            Value = confirmationCode
        };
        await _userRepository.AddExtendedAttributeAsync(newExtendedAttribute);
        
        return await Result.SuccessAsync(QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", newExtendedAttribute.Value));
    }

    private async Task SetUserPassword(Guid userId, string newPassword)
    {
        var updateObject = new AppUserUpdate() { Id = userId };
        
        AccountHelpers.GenerateHashAndSalt(newPassword, out var salt, out var hash);
        updateObject.PasswordSalt = salt;
        updateObject.PasswordHash = hash;
        
        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest resetRequest)
    {
        var foundUser = (await _userRepository.GetByEmailAsync(resetRequest.Email)).Result;
        if (foundUser is null)
            return await Result.FailAsync(ErrorMessageConstants.GenericError);

        var previousResets =
            (await _userRepository.GetUserExtendedAttributesByTypeAsync(foundUser.Id, ExtendedAttributeType.PasswordResetToken)).Result;
        var previousReset = previousResets?.FirstOrDefault();
        
        if (previousReset is null)
            return await Result.FailAsync(ErrorMessageConstants.GenericError);
        if (resetRequest.Password != resetRequest.ConfirmPassword)
            return await Result.FailAsync(ErrorMessageConstants.PasswordsNoMatchError);
        if (resetRequest.RequestCode == previousReset.Value)
            await SetUserPassword(foundUser.Id, resetRequest.Password);

        await _userRepository.RemoveExtendedAttributeAsync(previousReset.Id);

        return await Result.SuccessAsync("Password reset was successful");
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
        var update = await _userRepository.UpdateAsync(user.ToUpdateObject());
        if (!update.Success)
            return await Result<UserLoginResponse>.FailAsync(update.ErrorMessage);

        // TODO: Auth token is failing for users that aren't admin, returning indicating token has been deleted
        var response = new UserLoginResponse { Token = token, RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
        return await Result<UserLoginResponse>.SuccessAsync(response);
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
                new(ClaimTypes.Email, user.Email ?? "NA"),
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Secret!)),
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
        var secret = Encoding.UTF8.GetBytes(_appConfig.Secret!);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
}