using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Constants.Identity;
using Application.Extensibility.Extensions;
using Application.Extensibility.Settings;
using Application.Extensibility.Utilities;
using Application.Interfaces.Database;
using Application.Interfaces.Identity;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities.Identity;
using Domain.Exceptions;
using Domain.Models.Identity;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Shared.ApiRoutes.Identity;
using Shared.Requests.Identity;
using Shared.Responses.Identity;
using static Application.Constants.Database.MsSqlConstants.StoredProcedures;

namespace Infrastructure.Services.Identity;

public class UserService : IUserService, IUserStore<AppUser>, IUserEmailStore<AppUser>, IUserPhoneNumberStore<AppUser>,
    IUserTwoFactorStore<AppUser>, IUserPasswordStore<AppUser>
{
    private const string InvalidErrorMessage = "The username and password combination provided is invalid";

    private readonly ISqlDataService _database;
    private readonly IMapper _mapper;
    private readonly AppConfiguration _appConfig;
    private readonly IFluentEmail _mailService;
    private readonly ICurrentUserService _currentUserService;

    public UserService(ISqlDataService database, IMapper mapper, AppConfiguration appConfig, IFluentEmail mailService,
        ICurrentUserService currentUserService)
    {
        _database = database;
        _mapper = mapper;
        _appConfig = appConfig;
        _mailService = mailService;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<AppUser>> GetAllAsync()
    {
        return await _database.LoadData<AppUser, dynamic>(UserGetAll.ToDboName(), new { });
    }

    public async Task<int> GetCountAsync()
    {
        return await _database.LoadDataCount<dynamic>(UserCount.ToDboName(), new {TableName = "Users"});
    }

    public async Task<AppUser?> GetByIdAsync(GetUserByIdRequest userRequest)
    {
        var foundUser = await _database.LoadData<AppUser, dynamic>(UserGetById.ToDboName(), userRequest);
        return foundUser.FirstOrDefault();
    }

    public async Task<AppUser?> GetByUsernameAsync(GetUserByUsernameRequest userRequest)
    {
        var foundUser = await _database.LoadData<AppUser, dynamic>(UserGetByUsername.ToDboName(), userRequest);
        return foundUser.FirstOrDefault();
    }

    public async Task<AppUser?> GetByEmailAsync(GetUserByEmailRequest userRequest)
    {
        var foundUser = await _database.LoadData<AppUser, dynamic>(UserGetByEmail.ToDboName(), userRequest);
        return foundUser.FirstOrDefault();
    }

    public async Task UpdateAsync(UpdateUserRequest userRequest)
    {
        await _database.SaveData(UserUpdate.ToDboName(), userRequest);
    }

    public async Task DeleteAsync(DeleteUserRequest userRequest)
    {
        await _database.SaveData(UserDelete.ToDboName(), userRequest);
    }

    public async Task<string> GetUserIdAsync(AppUser user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.Id.ToString());
    }

    public async Task<string> GetUserNameAsync(AppUser user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.Username);
    }

    public async Task SetUserNameAsync(AppUser user, string userName, CancellationToken cancellationToken)
    {
        user.Username = userName;
        await _database.SaveData(UserUpdate.ToDboName(), user);
    }

    public async Task<string> GetNormalizedUserNameAsync(AppUser user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.NormalizedUserName);
    }

    public async Task SetNormalizedUserNameAsync(AppUser user, string normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        await _database.SaveData(UserUpdate.ToDboName(), user);
    }

    public async Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken)
    {
        try
        {
            user.CreatedOn = DateTime.Now;
            await _database.SaveData(UserCreate.ToDboName(), user);
            return await Task.FromResult(IdentityResult.Success);
        }
        catch (Exception ex)
        {
            return await Task.FromResult(IdentityResult.Failed(new IdentityError(){Code = ex.Source, Description = ex.Message}));
        }
    }

    public async Task<IdentityResult> CreateAsync(AppUser user, string password, CancellationToken cancellationToken)
    {
        try
        {
            IdentityUtilities.GetPasswordHash(password, out var hash, out var salt);
            user.PasswordSalt = salt;
            user.PasswordHash = hash.ToString();
            user.CreatedOn = DateTime.Now;
            await _database.SaveData(UserCreate.ToDboName(), user);
            return await Task.FromResult(IdentityResult.Success);
        }
        catch (Exception ex)
        {
            return await Task.FromResult(IdentityResult.Failed(new IdentityError(){Code = ex.Source, Description = ex.Message}));
        }
    }

    public async Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
    {
        try
        {
            user.LastModifiedOn = DateTime.Now;
            await _database.SaveData(UserUpdate.ToDboName(), user);
            return await Task.FromResult(IdentityResult.Success);
        }
        catch (Exception ex)
        {
            return await Task.FromResult(IdentityResult.Failed(new IdentityError(){Code = ex.Source, Description = ex.Message}));
        }
    }

    public async Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
    {
        try
        {
            user.DeletedOn = DateTime.Now;
            await _database.SaveData(UserDelete.ToDboName(), user);
            return await Task.FromResult(IdentityResult.Success);
        }
        catch (Exception ex)
        {
            return await Task.FromResult(IdentityResult.Failed(new IdentityError(){Code = ex.Source, Description = ex.Message}));
        }
    }

    public async Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var id = Guid.Parse(userId);
        return (await GetByIdAsync(new GetUserByIdRequest() {Id = id}))!;
    }

    public async Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return (await GetByUsernameAsync(new GetUserByUsernameRequest() {Username = normalizedUserName}))!;
    }

    public async Task SetEmailAsync(AppUser user, string email, CancellationToken cancellationToken)
    {
        user.Email = email;
        await _database.SaveData(UserUpdate.ToDboName(), user);
    }

    public async Task<string> GetEmailAsync(AppUser user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.Email);
    }

    public async Task<bool> GetEmailConfirmedAsync(AppUser user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.EmailConfirmed);
    }

    public async Task SetEmailConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;
        await _database.SaveData(UserUpdate.ToDboName(), user);
    }

    public async Task<AppUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        var userRequest = new GetUserByEmailRequest() {Email = normalizedEmail};
        var foundUser = await _database.LoadData<AppUser, dynamic>(UserGetByEmail.ToDboName(), userRequest);
        return foundUser.FirstOrDefault()!;
    }

    public async Task<string> GetNormalizedEmailAsync(AppUser user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.NormalizedEmail);
    }

    public async Task SetNormalizedEmailAsync(AppUser user, string normalizedEmail, CancellationToken cancellationToken)
    {
        user.Email = normalizedEmail.ToLower();
        user.NormalizedEmail = normalizedEmail;
        await _database.SaveData(UserUpdate.ToDboName(), user);
    }

    public async Task SetPhoneNumberAsync(AppUser user, string phoneNumber, CancellationToken cancellationToken)
    {
        user.PhoneNumber = phoneNumber;
        await _database.SaveData(UserUpdate.ToDboName(), user);
    }

    public async Task<string> GetPhoneNumberAsync(AppUser user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.PhoneNumber);
    }

    public async Task<bool> GetPhoneNumberConfirmedAsync(AppUser user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.PhoneNumberConfirmed);
    }

    public async Task SetPhoneNumberConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken)
    {
        user.PhoneNumberConfirmed = confirmed;
        await _database.SaveData(UserUpdate.ToDboName(), user);
    }

    public async Task SetTwoFactorEnabledAsync(AppUser user, bool enabled, CancellationToken cancellationToken)
    {
        user.TwoFactorEnabled = enabled;
        await _database.SaveData(UserUpdate.ToDboName(), user);
    }

    public async Task<bool> GetTwoFactorEnabledAsync(AppUser user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.TwoFactorEnabled);
    }

    public async Task SetPasswordHashAsync(AppUser user, string passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        await _database.SaveData(UserUpdate.ToDboName(), user);
    }

    public async Task<string> GetPasswordHashAsync(AppUser user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.PasswordHash);
    }

    public async Task<bool> HasPasswordAsync(AppUser user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(string.IsNullOrWhiteSpace(user.PasswordHash));
    }

    public async Task<IResult<UserLoginResponse>> LoginAsync(UserLoginRequest loginRequest)
    {
        var user = await GetByUsernameAsync(new GetUserByUsernameRequest(){Username = loginRequest.Username});
        if (user == null)
        {
            return await Result<UserLoginResponse>.FailAsync(InvalidErrorMessage);
        }
        if (!user.IsActive)
        {
            return await Result<UserLoginResponse>.FailAsync("Your account is disabled, please contact the administrator.");
        }
        if (!user.EmailConfirmed)
        {
            return await Result<UserLoginResponse>.FailAsync("Your email has not been confirmed, please confirm your email.");
        }
        var passwordValid = IdentityUtilities.IsPasswordCorrect(
            loginRequest.Password, System.Text.Encoding.UTF8.GetBytes(user.PasswordHash), user.PasswordSalt);
        if (!passwordValid)
        {
            return await Result<UserLoginResponse>.FailAsync(InvalidErrorMessage);
        }

        user.RefreshToken = GenerateRefreshToken();
        // TODO: Add server setting for token expiration
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        await UpdateAsync(user, CancellationToken.None);

        var token = await GenerateJwtAsync(user);
        var response = new UserLoginResponse() { Token = token, RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
        return await Result<UserLoginResponse>.SuccessAsync(response);
    }

    public async Task<IResult<List<IdentityResult>>> AddToRolesAsync(UserRoleRequest roleRequest)
    {
        AppUser? foundUser;
        if (roleRequest.UserId is not null)
            foundUser = await GetByIdAsync(new GetUserByIdRequest() {Id = (Guid)roleRequest.UserId});
        else if (roleRequest.Username is not null)
            foundUser = await GetByUsernameAsync(new GetUserByUsernameRequest() {Username = roleRequest.Username});
        else if (roleRequest.Email is not null)
            foundUser = await GetByEmailAsync(new GetUserByEmailRequest() {Email = roleRequest.Email});
        else
            return await Result<List<IdentityResult>>.FailAsync("UserId, Username & Email are all empty, at least one is required");

        if (foundUser is null)
            return await Result<List<IdentityResult>>.FailAsync("Was unable to find a user with the provided information");

        var currentUserId = Guid.Parse(_currentUserService.UserId);
        if (currentUserId == Guid.Empty)
            return await Result<List<IdentityResult>>.FailAsync("Internal Identity failure occurred, please contact the administrator");
        var currentUser = await GetByIdAsync(new GetUserByIdRequest() {Id = currentUserId});
        var userIsAdmin = await _userManager.IsInRoleAsync(currentUser, RoleConstants.AdminRole);
        var requestContainsAdmin = roleRequest.RoleNames.Contains(RoleConstants.AdminRole);
        
        if (!userIsAdmin && requestContainsAdmin)
            return await Result<List<IdentityResult>>.FailAsync("You aren't an admin so you cannot add admin to accounts");

        var resultList = new List<IdentityResult>();
        foreach (var role in roleRequest.RoleNames)
        {
            var result = await _userManager.AddToRoleAsync(foundUser, role);
            resultList.Add(result);
        }
        
        return await Result<List<IdentityResult>>.SuccessAsync(resultList);
    }

    public async Task<IResult<List<IdentityResult>>> RemoveFromRolesAsync(UserRoleRequest roleRequest)
    {
        AppUser foundUser;
        if (roleRequest.UserId is not null)
            foundUser = await _userManager.FindByIdAsync(roleRequest.UserId.ToString());
        else if (roleRequest.Username is not null)
            foundUser = await _userManager.FindByNameAsync(roleRequest.Username);
        else if (roleRequest.Email is not null)
            foundUser = await _userManager.FindByEmailAsync(roleRequest.Email);
        else
            return await Result<List<IdentityResult>>.FailAsync("UserId, Username & Email are all empty, at least one is required");

        var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
        var userIsAdmin = await _userManager.IsInRoleAsync(currentUser, RoleConstants.AdminRole);
        var requestContainsAdmin = roleRequest.RoleNames.Contains(RoleConstants.AdminRole);
        var requestIsForSelf = foundUser.Id == currentUser.Id;
        var requestIsForDefaultAdmin = foundUser.Username == UserConstants.DefaultUsername;
        
        if (!userIsAdmin && requestContainsAdmin)
            return await Result<List<IdentityResult>>.FailAsync("You aren't an admin so you cannot add or remove admin from accounts");

        // Requiring another admin to remove admin access has the added benefit of there always being at least one non default admin
        if (requestIsForSelf && userIsAdmin && !requestContainsAdmin)
            return await Result<List<IdentityResult>>.FailAsync("You can't remove admin access from yourself, another admin will have to revoke your access");

        if (requestIsForDefaultAdmin && !requestContainsAdmin)
            return await Result<List<IdentityResult>>.FailAsync("Default admin cannot have admin access revoked");

        var resultList = new List<IdentityResult>();
        foreach (var role in roleRequest.RoleNames)
        {
            var result = await _userManager.RemoveFromRoleAsync(foundUser, role);
            resultList.Add(result);
        }
        
        return await Result<List<IdentityResult>>.SuccessAsync(resultList);
    }

    public async Task<IResult<IdentityResult>> EnforceRolesAsync(UserRoleRequest roleRequest)
    {
        AppUser foundUser;
        if (roleRequest.UserId is not null)
            foundUser = await _userManager.FindByIdAsync(roleRequest.UserId.ToString());
        else if (roleRequest.Username is not null)
            foundUser = await _userManager.FindByNameAsync(roleRequest.Username);
        else if (roleRequest.Email is not null)
            foundUser = await _userManager.FindByEmailAsync(roleRequest.Email);
        else
            return await Result<IdentityResult>.FailAsync("UserId, Username & Email are all empty, at least one is required");

        var currentRoles = await _userManager.GetRolesAsync(foundUser);
        var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
        var userIsAdmin = await _userManager.IsInRoleAsync(currentUser, RoleConstants.AdminRole);
        var requestContainsAdmin = roleRequest.RoleNames.Contains(RoleConstants.AdminRole);
        var requestIsForSelf = foundUser.Id == currentUser.Id;
        var requestIsForDefaultAdmin = foundUser.Username == UserConstants.DefaultUsername;
        
        if (!userIsAdmin && requestContainsAdmin)
            return await Result<IdentityResult>.FailAsync("You aren't an admin so you cannot add or remove admin from accounts");

        // Requiring another admin to remove admin access has the added benefit of there always being at least one non default admin
        if (requestIsForSelf && userIsAdmin && !requestContainsAdmin)
            return await Result<IdentityResult>.FailAsync("You can't remove admin access from yourself, another admin will have to revoke your access");

        if (requestIsForDefaultAdmin && !requestContainsAdmin)
            return await Result<IdentityResult>.FailAsync("Default admin cannot have admin access revoked");
        
        var removeResult = await _userManager.RemoveFromRolesAsync(foundUser, currentRoles);
        if (!removeResult.Succeeded)
            return await Result<IdentityResult>.FailAsync(removeResult.Errors.ToString()!);
        
        var enforceResult = await _userManager.AddToRolesAsync(foundUser, roleRequest.RoleNames);
        return await Result<IdentityResult>.SuccessAsync(enforceResult);
    }

    public async Task<IResult> RegisterAsync(UserRegisterRequest registerRequest)
    {
        var userWithSameUserName = await _userManager.FindByNameAsync(registerRequest.Username);
        if (userWithSameUserName != null)
        {
            return await Result.FailAsync(string.Format($"Username {registerRequest.Username} is already in use, please try again"));
        }
        
        var newUser = new AppUser()
        {
            Email = registerRequest.Email,
            UserName = registerRequest.Username,
        };

        var userWithSameEmail = await _userManager.FindByEmailAsync(registerRequest.Email);
        if (userWithSameEmail is not null)
            return await Result.FailAsync($"The email address {registerRequest.Email} is already in use, please try again");
        
        var createUserResult = await _userManager.CreateAsync(newUser, registerRequest.Password);
        if (!createUserResult.Succeeded)
            return await Result.FailAsync(createUserResult.Errors.Select(r => r.Description).ToList());
        
        var addToRoleResult = await _userManager.AddToRoleAsync(newUser, RoleConstants.DefaultRole);
        if (!addToRoleResult.Succeeded)
            return await Result.FailAsync(addToRoleResult.Errors.Select(r => r.Description).ToList());
        
        var confirmationUrl = await GetEmailConfirmationUri(newUser);
        var sendResponse = await _mailService.Subject("Registration Confirmation").To(newUser.Email)
            .UsingTemplateFromFile(UserConstants.PathEmailTemplateConfirmation,
                new EmailAction() {ActionUrl = confirmationUrl, Username = newUser.Username}
            ).SendAsync();
        if (!sendResponse.Successful)
            return await Result.FailAsync(
                "Account was registered successfully but a failure occurred attempting to send an email to " +
                "the address provided, please contact the administrator for assistance");
        
        return await Result<Guid>.SuccessAsync(newUser.Id, $"User {newUser.UserName} Registered. Please check your Mailbox to confirm!");
    }

    public async Task<string> GetEmailConfirmationUri(AppUser user)
    {
        var confirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var decodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationCode));
        var endpointUri = new Uri(string.Concat(_appConfig.BaseUrl, UserRoutes.ConfirmEmail));
        var confirmationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id.ToString());
        return QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", decodedCode);
    }

    public async Task<IResult> ToggleUserStatusAsync(ChangeUserActiveStateRequest activeRequest)
    {
        var user = await GetByIdAsync(new GetUserByIdRequest(){Id = activeRequest.UserId});
        if (user is null)
            return await Result.FailAsync("User Id provided is invalid");
        
        var isAdmin = await _userManager.IsInRoleAsync(user, RoleConstants.AdminRole);
        if (isAdmin)
            return await Result.FailAsync("Administrators cannot be toggled, please remove admin privileges first");
        
        user.IsActive = activeRequest.IsActive;
        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
            return await Result.FailAsync(identityResult.Errors.Select(r => r.Description).ToList());
        
        return await Result.SuccessAsync($"{user.Username} active status set to: {user.IsActive}");
    }

    public async Task<IResult<List<RoleResponse>>> GetRolesAsync(Guid userId)
    {
        var userRoles = new List<RoleResponse>();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var roles = _roleManager.Roles.ToList();

        foreach (var role in roles)
        {
            var userRole = _mapper.Map<RoleResponse>(role);
            if (await _userManager.IsInRoleAsync(user, role.Name))
                userRole.IsMember = true;
            
            userRoles.Add(userRole);
        }
        
        return await Result<List<RoleResponse>>.SuccessAsync(userRoles);
    }

    public async Task<IResult<string>> ConfirmEmailAsync(Guid userId, string confirmationCode)
    {
        var foundUser = await _userManager.FindByIdAsync(userId.ToString());
        if (foundUser is null)
            return await Result<string>.FailAsync("User Id provided is invalid");
        
        var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmationCode));
        var result = await _userManager.ConfirmEmailAsync(foundUser, decodedCode);
        if (!result.Succeeded)
            throw new ApiException($"An error occurred attempting to confirm email: {foundUser.Email}");
        
        return await Result<string>.SuccessAsync(foundUser.Id.ToString(), $"Account Confirmed for {foundUser.Email}.");
    }

    public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest forgotRequest)
    {
        var foundUser = await _userManager.FindByEmailAsync(forgotRequest.Email);
        if (foundUser is null)
            return await Result.FailAsync("An internal server error occurred, please contact the administrator");

        if (!await _userManager.IsEmailConfirmedAsync(foundUser))
            return await Result.FailAsync("Email hasn't been confirmed for this account, please contact the administrator");
        
        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(foundUser);
        var decodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));
        var endpointUri = new Uri(string.Concat(_appConfig.BaseUrl, UserRoutes.ResetPassword));
        var resetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", decodedToken);
        // TODO: Figure out why BackgroundJob.Enqueue() isn't working here
        var sendResponse = await _mailService.Subject("Password Reset").To(foundUser.Email)
            .UsingTemplateFromFile(UserConstants.PathEmailTemplatePasswordReset,
                new EmailAction() {ActionUrl = resetUrl, Username = foundUser.Username}
            ).SendAsync();
        if (!sendResponse.Successful)
            return await Result.FailAsync(
                "A failure occurred attempting to send an email to the account email address, " +
                "please contact the administrator for assistance");
        
        return await Result.SuccessAsync("Password reset has been successfully sent to the account email");
    }

    public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest resetRequest)
    {
        var foundUser = await _userManager.FindByEmailAsync(resetRequest.Email);
        if (foundUser is null)
            return await Result.FailAsync("An internal server error occurred, please contact the administrator");

        var resetResult = await _userManager.ResetPasswordAsync(foundUser, resetRequest.Token, resetRequest.Password);
        if (!resetResult.Succeeded)
            return await Result.FailAsync("An internal server error occurred, please contact the administrator");

        return await Result.SuccessAsync("Password reset was successful");
    }

    public void Dispose()
    {
        // Using Dapper, nothing to dispose
    }

    public async Task<IResult<UserLoginResponse>> GetRefreshTokenAsync(RefreshTokenRequest? refreshRequest)
    {
        if (refreshRequest is null)
        {
            return await Result<UserLoginResponse>.FailAsync("Invalid Client Token.");
        }
        var userPrincipal = GetPrincipalFromExpiredToken(refreshRequest.Token!);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
            return await Result<UserLoginResponse>.FailAsync("User Not Found.");
        if (user.RefreshToken != refreshRequest.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            return await Result<UserLoginResponse>.FailAsync("Invalid Client Token.");
        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        user.RefreshToken = GenerateRefreshToken();
        await _userManager.UpdateAsync(user);

        // TODO: Auth token is failing for users that aren't admin, returning indicating token has been deleted
        var response = new UserLoginResponse { Token = token, RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
        return await Result<UserLoginResponse>.SuccessAsync(response);
    }

    private async Task<string> GenerateJwtAsync(AppUser user)
    {
        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        return token;
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(AppUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role));
            var thisRole = await _roleManager.FindByNameAsync(role);
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