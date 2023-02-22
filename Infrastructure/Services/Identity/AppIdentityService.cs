using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Constants.Identity;
using Application.Constants.Messages;
using Application.Database.MsSql.Identity;
using Application.Helpers.Identity;
using Application.Helpers.Web;
using Application.Models.Identity;
using Application.Models.Web;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Todo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Shared.ApiRoutes.Identity;
using Shared.Enums.Identity;
using Shared.Requests.Identity;
using Shared.Responses.Identity;
using Application.Settings.AppSettings;
using Domain.Enums.Identity;
using Domain.Models.Database;
using FluentEmail.Core;

namespace Infrastructure.Services.Identity;

public class AppIdentityService : IAppIdentityService
{
    private readonly IAppUserRepository _userRepository;
    private readonly AppConfiguration _appConfig;
    private readonly IFluentEmail _mailService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppRoleRepository _roleRepository;
    private readonly UserManager<AppUserDb> _userManager;
    private readonly RoleManager<AppRoleDb> _roleManager;

    public AppIdentityService(IAppUserRepository userRepository, AppConfiguration appConfig, IFluentEmail mailService,
        ICurrentUserService currentUserService, IAppRoleRepository roleRepository, UserManager<AppUserDb> userManager, RoleManager<AppRoleDb> roleManager)
    {
        _userRepository = userRepository;
        _appConfig = appConfig;
        _mailService = mailService;
        _currentUserService = currentUserService;
        _roleRepository = roleRepository;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public void Dispose()
    {
        // Using Dapper, nothing to dispose
    }

    public async Task<string> GetUserIdAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.Id.ToString());
    }

    public async Task<string> GetUserNameAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.Username);
    }

    public async Task SetUserNameAsync(AppUserDb user, string newUserName, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate()
        {
            Id = user.Id,
            Username = newUserName,
            NormalizedUserName = newUserName.NormalizeForDatabase()
        };
        
        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task<string> GetNormalizedUserNameAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.NormalizedUserName);
    }

    public async Task SetNormalizedUserNameAsync(AppUserDb user, string normalizedName, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate() { Id = user.Id, NormalizedUserName = normalizedName };
        
        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task<IdentityResult> CreateAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        try
        {
            await _userRepository.CreateAsync(user.ToCreateObject());
            return await Task.FromResult(IdentityResult.Success);
        }
        catch (Exception ex)
        {
            return await Task.FromResult(IdentityResult.Failed(new IdentityError(){Code = ex.Source, Description = ex.Message}));
        }
    }

    public async Task<IdentityResult> UpdateAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        try
        {
            var updateUser = user.ToUpdateObject();
            updateUser.LastModifiedOn = DateTime.Now;

            await _userRepository.UpdateAsync(updateUser);
            return await Task.FromResult(IdentityResult.Success);
        }
        catch (Exception ex)
        {
            return await Task.FromResult(IdentityResult.Failed(new IdentityError(){Code = ex.Source, Description = ex.Message}));
        }
    }

    public async Task<IdentityResult> DeleteAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        try
        {
            await _userRepository.DeleteAsync(user.Id);
            return await Task.FromResult(IdentityResult.Success);
        }
        catch (Exception ex)
        {
            return await Task.FromResult(IdentityResult.Failed(new IdentityError(){Code = ex.Source, Description = ex.Message}));
        }
    }

    public async Task<AppUserDb> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var id = Guid.Parse(userId);
        return (await _userRepository.GetByIdAsync(id)).Result!;
    }

    public async Task<AppUserDb> FindByNameAsync(string normalizedUsername, CancellationToken cancellationToken)
    {
        return (await _userRepository.GetByNormalizedUsernameAsync(normalizedUsername)).Result!;
    }

    public async Task SetEmailAsync(AppUserDb user, string email, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate() {Id = user.Id, Email = email};
        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task<string> GetEmailAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.Email);
    }

    public async Task<bool> GetEmailConfirmedAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.EmailConfirmed);
    }

    public async Task SetEmailConfirmedAsync(AppUserDb user, bool confirmed, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate() {Id = user.Id, EmailConfirmed = confirmed};
        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task<AppUserDb> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return (await _userRepository.GetByNormalizedEmailAsync(normalizedEmail)).Result!;
    }

    public async Task<string> GetNormalizedEmailAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.NormalizedEmail);
    }

    public async Task SetNormalizedEmailAsync(AppUserDb user, string normalizedEmail, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate() {Id = user.Id, NormalizedEmail = normalizedEmail};
        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task SetPhoneNumberAsync(AppUserDb user, string phoneNumber, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate() {Id = user.Id, PhoneNumber = phoneNumber};
        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task<string> GetPhoneNumberAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.PhoneNumber);
    }

    public async Task<bool> GetPhoneNumberConfirmedAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.PhoneNumberConfirmed);
    }

    public async Task SetPhoneNumberConfirmedAsync(AppUserDb user, bool confirmed, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate() {Id = user.Id, PhoneNumberConfirmed = confirmed};
        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task SetTwoFactorEnabledAsync(AppUserDb user, bool enabled, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate() {Id = user.Id, TwoFactorEnabled = enabled};
        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task<bool> GetTwoFactorEnabledAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.TwoFactorEnabled);
    }

    public async Task SetPasswordHashAsync(AppUserDb user, string passwordHash, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate() {Id = user.Id, PasswordHash = passwordHash};
        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task<string> GetPasswordHashAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.PasswordHash);
    }

    public async Task<bool> HasPasswordAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
    }

    public async Task SetUserPassword(Guid userId, string newPassword)
    {
        var updateObject = new AppUserUpdate() { Id = userId };
        
        AccountHelpers.GetPasswordHash(newPassword, out var hash, out var salt);
        updateObject.PasswordSalt = salt;
        updateObject.PasswordHash = hash.ToString()!;
        
        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task<DatabaseActionResult<Guid>> CreateAsync(AppUserDb user, string password, CancellationToken cancellationToken)
    {
        var createUser = user.ToCreateObject();
            
        AccountHelpers.GetPasswordHash(password, out var hash, out var salt);
        createUser.PasswordSalt = salt;
        createUser.PasswordHash = hash.ToString()!;
            
        return await _userRepository.CreateAsync(createUser);
    }

    public async Task<IResult<UserLoginResponse>> LoginAsync(UserLoginRequest loginRequest)
    {
        var user = (await _userRepository.GetByUsernameAsync(loginRequest.Username)).Result;
        if (user is null)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.CredentialsInvalidError);
        
        var passwordValid = AccountHelpers.IsPasswordCorrect(
            loginRequest.Password, Encoding.UTF8.GetBytes(user.PasswordHash), user.PasswordSalt);
        if (!passwordValid)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.CredentialsInvalidError);
        if (!user.IsActive)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.AccountDisabledError);
        if (!user.EmailConfirmed)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.EmailNotConfirmedError);

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_appConfig.TokenExpirationDays);
        await UpdateAsync(user, CancellationToken.None);

        var token = await GenerateJwtAsync(user);
        var response = new UserLoginResponse() { Token = token, RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
        return await Result<UserLoginResponse>.SuccessAsync(response);
    }

    private async Task<AppUserDb?> GetUserFromProvidedIds(UserRoleRequest roleRequest)
    {
        if (roleRequest.UserId is not null)
            return (await _userRepository.GetByIdAsync((Guid) roleRequest.UserId)).Result;
        if (roleRequest.Username is not null)
            return (await _userRepository.GetByUsernameAsync(roleRequest.Username)).Result;
        
        return roleRequest.Email is not null ? (await _userRepository.GetByEmailAsync(roleRequest.Email)).Result : null;
    }

    public async Task<IResult<List<IdentityResult>>> AddToRolesAsync(UserRoleRequest roleRequest)
    {
        var foundUser = await GetUserFromProvidedIds(roleRequest);
        if (foundUser is null)
            return await Result<List<IdentityResult>>.FailAsync(ErrorMessageConstants.UserNotFoundError);

        var currentUserId = Guid.Parse(_currentUserService.UserId);
        if (currentUserId == Guid.Empty)
            return await Result<List<IdentityResult>>.FailAsync(ErrorMessageConstants.GenericError);
        
        var currentUser = (await _userRepository.GetByIdAsync(currentUserId)).Result;
        var adminRole = (await _roleRepository.GetByNameAsync(RoleConstants.AdminRole)).Result;
        var userIsAdmin = (await _roleRepository.IsUserInRoleAsync(currentUser!.Id, adminRole!.Id)).Result;
        var requestContainsAdmin = roleRequest.RoleNames.Contains(RoleConstants.AdminRole);
        
        if (!userIsAdmin && requestContainsAdmin)
            return await Result<List<IdentityResult>>.FailAsync(ErrorMessageConstants.PermissionError);

        var identityResultList = new List<IdentityResult>();
        var failureOccurred = false;
        foreach (var role in roleRequest.RoleNames)
        {
            var foundRole = (await _roleRepository.GetByNameAsync(role)).Result;
            if (foundRole is null)
            {
                identityResultList.Add(IdentityResult.Failed(
                    new IdentityError() { Code = "RoleError", Description = ErrorMessageConstants.InvalidValueError }));
                failureOccurred = true;
                continue;
            }
            
            identityResultList.Add(IdentityResult.Success);
        }

        if (failureOccurred)
            return await Result<List<IdentityResult>>.FailAsync(identityResultList);
        
        return await Result<List<IdentityResult>>.SuccessAsync(identityResultList);
    }

    public async Task<IResult<List<IdentityResult>>> RemoveFromRolesAsync(UserRoleRequest roleRequest)
    {
        var requestedUser = await GetUserFromProvidedIds(roleRequest);
        if (requestedUser is null)
            return await Result<List<IdentityResult>>.FailAsync(ErrorMessageConstants.UserNotFoundError);

        var currentUser = (await _userRepository.GetByIdAsync(Guid.Parse(_currentUserService.UserId))).Result;
        var adminRole = (await _roleRepository.GetByNameAsync(RoleConstants.AdminRole)).Result;
        
        var currentUserIsAdmin = (await _roleRepository.IsUserInRoleAsync(currentUser!.Id, adminRole!.Id)).Result;
        var requestContainsAdmin = roleRequest.RoleNames.Contains(adminRole.Name);
        var requestIsForSelf = requestedUser.Id == currentUser.Id;
        var requestIsForDefaultAdmin = requestedUser.Username == UserConstants.DefaultUsername;
        
        if (!currentUserIsAdmin && requestContainsAdmin)
            return await Result<List<IdentityResult>>.FailAsync(ErrorMessageConstants.PermissionError);

        // Requiring another admin to remove admin access has the added benefit of there always being at least one non default admin
        if (requestIsForSelf && currentUserIsAdmin && requestContainsAdmin)
            return await Result<List<IdentityResult>>.FailAsync(ErrorMessageConstants.AdminSelfPowerRemovalError);

        if (requestIsForDefaultAdmin && requestContainsAdmin)
            return await Result<List<IdentityResult>>.FailAsync(ErrorMessageConstants.DefaultAdminPowerRemovalError);

        var resultList = new List<IdentityResult>();
        foreach (var role in roleRequest.RoleNames)
        {
            var foundRole = (await _roleRepository.GetByNameAsync(role)).Result;
            if (foundRole is null)
            {
                resultList.Add(IdentityResult.Failed(new IdentityError() 
                    {Code = "RoleError", Description = $"Role couldn't be found using the provided name: {role}"}));
                continue;
            }

            var removeResult = (await _roleRepository.RemoveUserFromRoleAsync(requestedUser.Id, foundRole.Id)).Success;
            if (!removeResult)
            {
                resultList.Add(IdentityResult.Failed(new IdentityError()
                    { Code = "RoleError", Description = $"Role already wasn't on the provided user: {role}" }));
                continue;
            }
            
            resultList.Add(IdentityResult.Success);
        }
        
        return await Result<List<IdentityResult>>.SuccessAsync(resultList);
    }

    public async Task<IResult> RegisterAsync(UserRegisterRequest registerRequest)
    {
        var matchingUserName = (await _userRepository.GetByUsernameAsync(registerRequest.Username)).Result;
        if (matchingUserName != null)
        {
            return await Result.FailAsync(string.Format($"Username {registerRequest.Username} is already in use, please try again"));
        }
        
        var newUser = new AppUserDb()
        {
            Email = registerRequest.Email,
            UserName = registerRequest.Username,
        };

        var matchingEmail = (await _userRepository.GetByEmailAsync(registerRequest.Email)).Result;
        if (matchingEmail is not null)
            return await Result.FailAsync($"The email address {registerRequest.Email} is already in use, please try again");
        
        var createUserResult = await CreateAsync(newUser, registerRequest.Password, CancellationToken.None);
        if (!createUserResult.Success)
            return await Result.FailAsync(createUserResult.ErrorMessage);

        var caveatMessage = "";
        var defaultRole = (await _roleRepository.GetByNameAsync(RoleConstants.DefaultRole)).Result;
        var addToRoleResult = await _roleRepository.AddUserToRoleAsync(createUserResult.Result, defaultRole!.Id);
        if (!addToRoleResult.Success)
            caveatMessage = $",{Environment.NewLine} Default permissions could not be added to this account, " +
                            $"please contact the administrator for assistance";
        
        var confirmationUrl = await GetEmailConfirmationUrl(newUser);
        var sendResponse = await _mailService.Subject("Registration Confirmation").To(newUser.Email)
            .UsingTemplateFromFile(UserConstants.PathEmailTemplateConfirmation,
                new EmailAction() {ActionUrl = confirmationUrl, Username = newUser.Username}
            ).SendAsync();
        
        if (!sendResponse.Successful)
            return await Result.FailAsync(
                $"Account was registered successfully but a failure occurred attempting to send an email to " +
                $"the address provided, please contact the administrator for assistance{caveatMessage}");
        
        return await Result<Guid>.SuccessAsync(newUser.Id, 
            $"User {newUser.UserName} Registered. Please check your Mailbox to confirm!{caveatMessage}");
    }

    public async Task<string> GetEmailConfirmationUrl(AppUserDb user)
    {
        // TODO: Implement AppUser ExtendedAttributes before continuing
        var previousConfirmations =
            (await _userRepository.GetUserExtendedAttributesByTypeAsync(user.Id, ExtendedAttributeType.EmailConfirmationToken)).Result;
        var previousConfirmation = previousConfirmations?.FirstOrDefault();
        
        var endpointUri = new Uri(string.Concat(_appConfig.BaseUrl, UserRoutes.ConfirmEmail));
        var confirmationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id.ToString());
        
        // Previous pending account registration exists, return current value
        if (previousConfirmation is not null)
            return QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", previousConfirmation.Value);

        // No currently pending account registration exists so we'll generate a new one, add it to the provided user
        //   and return the generated confirmation uri
        var confirmationCode = UrlHelpers.GenerateToken(32);
        var newExtendedAttribute = new AppUserExtendedAttributeAdd()
        {
            OwnerId = user.Id,
            Name = "AccountEmailConfirmation",
            Type = ExtendedAttributeType.EmailConfirmationToken,
            Value = confirmationCode
        };
        await _userRepository.AddExtendedAttributeAsync(newExtendedAttribute);
        
        return QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", confirmationCode);
    }

    public async Task<IResult> ToggleUserStatusAsync(ChangeUserActiveStateRequest activeRequest)
    {
        var requestedUser = (await _userRepository.GetByIdAsync(activeRequest.UserId)).Result;
        if (requestedUser is null)
            return await Result<List<IdentityResult>>.FailAsync("Was unable to find a user with the provided information");

        // TODO: Add permission for toggling user status and validation current user has permissions
        var adminRole = (await _roleRepository.GetByNameAsync(RoleConstants.AdminRole)).Result;
        var requestedUserIsAdmin =
            (await _roleRepository.IsUserInRoleAsync(requestedUser.Id, adminRole!.Id)).Result;
        if (requestedUserIsAdmin)
            return await Result.FailAsync("Administrators cannot be toggled, please remove admin privileges first");

        try
        {
            await _userRepository.UpdateAsync(new AppUserUpdate(){ Id = requestedUser.Id, IsActive = activeRequest.IsActive});
            return await Result.SuccessAsync($"{requestedUser.Username} active status set to: {activeRequest.IsActive}");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync($"Failed to update the active status of the user account '{requestedUser.Username}'");
        }
    }

    public async Task<IResult<List<AppRoleDb>>> GetRolesAsync(Guid userId)
    {
        var foundUser = (await _userRepository.GetByIdAsync(userId)).Result;
        if (foundUser is null)
            return await Result<List<AppRoleDb>>.FailAsync("Was unable to find a user with the provided information");

        var userRoles = (await _roleRepository.GetRolesForUser(foundUser.Id)).Result ?? new List<AppRoleDb>();

        return await Result<List<AppRoleDb>>.SuccessAsync(userRoles.ToList());
    }

    public async Task<IResult<string>> ConfirmEmailAsync(Guid userId, string confirmationCode)
    {
        var foundUser = (await _userRepository.GetByIdAsync(userId)).Result;
        if (foundUser is null)
            return await Result<string>.FailAsync(ErrorMessageConstants.UserNotFoundError);
        
        var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmationCode));

        var previousConfirmations =
            (await _userRepository.GetUserExtendedAttributesByTypeAsync(foundUser.Id, ExtendedAttributeType.EmailConfirmationToken)).Result;
        var previousConfirmation = previousConfirmations?.FirstOrDefault();

        // TODO: Add admin logging w/ attempt and failure details so an admin can troubleshoot
        if (previousConfirmation is null)
            return await Result<string>.FailAsync(
                $"An error occurred attempting to confirm account: {foundUser.Id}, please contact the administrator");
        if (previousConfirmation!.Value == confirmationCode)
            return await Result<string>.FailAsync(
                $"An error occurred attempting to confirm account: {foundUser.Id}, please contact the administrator");
        
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
        var confirmationCode = UrlHelpers.GenerateToken(32);
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
        await UpdateAsync(user, new CancellationToken());

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
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await GetRolesAsync(user.Id);
        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();
        foreach (var role in roles.Data)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role.Name));
            var thisRole = await _roleManager.FindByNameAsync(role.Name);
            var allPermissionsForThisRoles = await _roleManager.GetClaimsAsync(thisRole);
            permissionClaims.AddRange(allPermissionsForThisRoles);
        }

        var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email ?? "NA"),
                new(ClaimTypes.Name, user.FirstName ?? "NA"),
                new(ClaimTypes.Surname, user.LastName ?? "NA"),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? "NA")
            }
        .Union(userClaims)
        .Union(roleClaims)
        .Union(permissionClaims);

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