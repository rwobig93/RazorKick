using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Constants.Identity;
using Application.Extensibility.Extensions;
using Application.Extensibility.Identity;
using Application.Extensibility.Settings;
using Application.Extensibility.Web;
using Application.Interfaces.Database;
using Application.Interfaces.Identity;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities.Identity;
using Domain.Enums.Identity;
using Domain.Exceptions;
using Domain.Models.Identity;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Shared.ApiRoutes.Identity;
using Shared.Enums.Identity;
using Shared.Requests.Identity;
using Shared.Responses.Identity;
using static Application.Constants.Database.MsSqlConstants.StoredProcedures;

namespace Infrastructure.Services.Identity;

public class UserService : IUserService, IUserEmailStore<AppUser>, IUserPhoneNumberStore<AppUser>,
    IUserTwoFactorStore<AppUser>, IUserPasswordStore<AppUser>
{
    private const string InvalidErrorMessage = "The username and password combination provided is invalid";

    private readonly ISqlDataService _database;
    private readonly IMapper _mapper;
    private readonly AppConfiguration _appConfig;
    private readonly IFluentEmail _mailService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRoleService _roleService;

    public UserService(ISqlDataService database, IMapper mapper, AppConfiguration appConfig, IFluentEmail mailService,
        ICurrentUserService currentUserService, IRoleService roleService)
    {
        _database = database;
        _mapper = mapper;
        _appConfig = appConfig;
        _mailService = mailService;
        _currentUserService = currentUserService;
        _roleService = roleService;
    }

    public async Task<IEnumerable<AppUser>> GetAllAsync()
    {
        return await _database.LoadData<AppUser, dynamic>(UserGetAll.ToDboName(), new { });
    }

    public async Task<int> GetCountAsync()
    {
        return await _database.LoadDataCount<dynamic>(GeneralGetTableRowCount.ToDboName(), new {TableName = "Users"});
    }

    public async Task<AppUser?> GetByIdAsync(Guid userId)
    {
        var foundUser = await _database.LoadData<AppUser, dynamic>(
            UserGetById.ToDboName(), 
            new GetUserByIdRequest { Id = userId });
        return foundUser.FirstOrDefault();
    }

    public async Task<AppUser?> GetByUsernameAsync(string username)
    {
        var foundUser = await _database.LoadData<AppUser, dynamic>(
            UserGetByUsername.ToDboName(), 
            new GetUserByUsernameRequest() { Username = username});
        return foundUser.FirstOrDefault();
    }

    public async Task<AppUser?> GetByEmailAsync(string emailAddress)
    {
        var foundUser = await _database.LoadData<AppUser, dynamic>(
            UserGetByEmail.ToDboName(),
            new GetUserByEmailRequest() { Email = emailAddress });
        return foundUser.FirstOrDefault();
    }

    public async Task<bool> IsInRoleAsync(RoleMembershipRequest membershipValidationRequest)
    {
        var foundMembership = await _database.LoadData<AppUserRoleJunction, dynamic>(
            JunctionUserRoleGetByUserRoleId.ToDboName(), membershipValidationRequest);
        return foundMembership.FirstOrDefault() is not null;
    }

    public async Task<List<IResult>> AddUserToRolesAsync(UserRoleRequest roleRequest)
    {
        var resultList = new List<IResult>();
        
        var foundUser = await GetUserFromProvidedIds(roleRequest);
        if (foundUser is null)
        {
            resultList.Add(await Result.FailAsync("A user account couldn't be found with the provided data, please try again"));
            return resultList;   
        }

        foreach (var roleName in roleRequest.RoleNames)
        {
            try
            {
                var foundRole = await _roleService.GetByNameAsync(roleName);
                if (foundRole is null)
                {
                    resultList.Add(await Result.FailAsync($"Role '{roleName}' wasn't found"));
                    continue;
                }

                var alreadyHasRole = await IsInRoleAsync(
                    new RoleMembershipRequest() {RoleId = foundRole.Id, UserId = foundUser.Id});
                if (alreadyHasRole)
                {
                    resultList.Add(await Result.SuccessAsync($"User already has the '{roleName}' Role"));
                    continue;   
                }

                await _database.SaveData(JunctionUserRoleCreate.ToDboName(),
                    new RoleMembershipRequest() {RoleId = foundRole.Id, UserId = foundUser.Id});
                resultList.Add(await Result.SuccessAsync($"Successfully added role '{roleName}' to user '{foundUser.Username}'"));
            }
            catch (Exception ex)
            {
                resultList.Add(await Result.
                    FailAsync($"Failure occurred attempting to add user '{foundUser.Username}' to role '{roleName}: {ex.Message}"));
            }
        }

        return resultList;
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
            AccountValidation.GetPasswordHash(password, out var hash, out var salt);
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
        return (await GetByIdAsync(id))!;
    }

    public async Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return (await GetByUsernameAsync(normalizedUserName))!;
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
        var user = await GetByUsernameAsync(loginRequest.Username);
        if (user is null)
            return await Result<UserLoginResponse>.FailAsync(InvalidErrorMessage);
        
        var passwordValid = AccountValidation.IsPasswordCorrect(
            loginRequest.Password, Encoding.UTF8.GetBytes(user.PasswordHash), user.PasswordSalt);
        if (!passwordValid)
            return await Result<UserLoginResponse>.FailAsync(InvalidErrorMessage);
        if (!user.IsActive)
            return await Result<UserLoginResponse>.FailAsync("Your account is disabled, please contact the administrator.");
        if (!user.EmailConfirmed)
            return await Result<UserLoginResponse>.FailAsync("Your email has not been confirmed, please confirm your email.");

        user.RefreshToken = GenerateRefreshToken();
        // TODO: Add server setting for token expiration
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        await UpdateAsync(user, CancellationToken.None);

        var token = await GenerateJwtAsync(user);
        var response = new UserLoginResponse() { Token = token, RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
        return await Result<UserLoginResponse>.SuccessAsync(response);
    }

    private async Task<AppUser?> GetUserFromProvidedIds(UserRoleRequest roleRequest)
    {
        if (roleRequest.UserId is not null)
            return await GetByIdAsync((Guid) roleRequest.UserId);
        if (roleRequest.Username is not null)
            return await GetByUsernameAsync(roleRequest.Username);
        if (roleRequest.Email is not null)
            return await GetByEmailAsync(roleRequest.Email);

        return null;
    }

    public async Task<IResult<List<IdentityResult>>> AddToRolesAsync(UserRoleRequest roleRequest)
    {
        var foundUser = await GetUserFromProvidedIds(roleRequest);
        if (foundUser is null)
            return await Result<List<IdentityResult>>.FailAsync("Was unable to find a user with the provided information");

        var currentUserId = Guid.Parse(_currentUserService.UserId);
        if (currentUserId == Guid.Empty)
            return await Result<List<IdentityResult>>.FailAsync("Internal Identity failure occurred, please contact the administrator");
        
        var currentUser = await GetByIdAsync(currentUserId);
        var adminRole = await _roleService.GetByNameAsync(RoleConstants.AdminRole);
        var userIsAdmin = await IsInRoleAsync(new RoleMembershipRequest() { UserId = currentUser!.Id, RoleId = adminRole!.Id});
        var requestContainsAdmin = roleRequest.RoleNames.Contains(RoleConstants.AdminRole);
        
        if (!userIsAdmin && requestContainsAdmin)
            return await Result<List<IdentityResult>>.FailAsync("You aren't an admin so you cannot add admin to accounts");

        var roleResults = await AddUserToRolesAsync(roleRequest);
        var identityResultList = new List<IdentityResult>();
        var failureOccurred = false;
        foreach (var roleResult in roleResults)
        {
            if (roleResult.Succeeded)
            {
                identityResultList.Add(IdentityResult.Success);
                continue;
            }
            
            identityResultList.Add(IdentityResult.Failed(roleResult.Messages.Select(x => 
                new IdentityError() { Code = "RoleError", Description = x }).ToArray()));
            failureOccurred = true;
        }

        if (failureOccurred)
            return await Result<List<IdentityResult>>.FailAsync(identityResultList);
        
        return await Result<List<IdentityResult>>.SuccessAsync(identityResultList);
    }

    public async Task<IResult<List<IdentityResult>>> RemoveFromRolesAsync(UserRoleRequest roleRequest)
    {
        var requestedUser = await GetUserFromProvidedIds(roleRequest);
        if (requestedUser is null)
            return await Result<List<IdentityResult>>.FailAsync("Was unable to find a user with the provided information");

        var currentUser = await GetByIdAsync(Guid.Parse(_currentUserService.UserId));
        var adminRole = await _roleService.GetByNameAsync(RoleConstants.AdminRole);
        
        var currentUserIsAdmin = await IsInRoleAsync(new RoleMembershipRequest() { UserId = currentUser!.Id, RoleId = adminRole!.Id });
        var requestContainsAdmin = roleRequest.RoleNames.Contains(adminRole.Name);
        var requestIsForSelf = requestedUser.Id == currentUser.Id;
        var requestIsForDefaultAdmin = requestedUser.Username == UserConstants.DefaultUsername;
        
        if (!currentUserIsAdmin && requestContainsAdmin)
            return await Result<List<IdentityResult>>.FailAsync("You aren't an admin so you cannot add or remove admin from accounts");

        // Requiring another admin to remove admin access has the added benefit of there always being at least one non default admin
        if (requestIsForSelf && currentUserIsAdmin && requestContainsAdmin)
            return await Result<List<IdentityResult>>.
                FailAsync("You can't remove admin access from yourself, another admin will have to revoke your access");

        if (requestIsForDefaultAdmin && requestContainsAdmin)
            return await Result<List<IdentityResult>>.FailAsync("Default admin cannot have admin access revoked");

        var resultList = new List<IdentityResult>();
        foreach (var role in roleRequest.RoleNames)
        {
            var foundRole = await _roleService.GetByNameAsync(role);
            if (foundRole is null)
            {
                resultList.Add(IdentityResult.Failed(new IdentityError() 
                    {Code = "RoleError", Description = $"Role couldn't be found using the provided name: {role}"}));
                continue;
            }
            
            var changesMade = await _database.SaveData(JunctionUserRoleDelete.ToDboName(),
                new RoleMembershipRequest() { RoleId = foundRole.Id, UserId = requestedUser.Id });
            if (changesMade < 1)
                resultList.Add(IdentityResult.Failed(new IdentityError() 
                    { Code = "RoleError", Description = $"Role already wasn't on the provided user: {role}" }));
            else
                resultList.Add(IdentityResult.Success);
        }
        
        return await Result<List<IdentityResult>>.SuccessAsync(resultList);
    }

    public async Task<IResult> RegisterAsync(UserRegisterRequest registerRequest, CancellationToken cancellationToken)
    {
        var matchingUserName = await GetByUsernameAsync(registerRequest.Username);
        if (matchingUserName != null)
        {
            return await Result.FailAsync(string.Format($"Username {registerRequest.Username} is already in use, please try again"));
        }
        
        var newUser = new AppUser()
        {
            Email = registerRequest.Email,
            UserName = registerRequest.Username,
        };

        var matchingEmail = await GetByEmailAsync(registerRequest.Email);
        if (matchingEmail is not null)
            return await Result.FailAsync($"The email address {registerRequest.Email} is already in use, please try again");
        
        var createUserResult = await CreateAsync(newUser, registerRequest.Password, cancellationToken);
        if (!createUserResult.Succeeded)
            return await Result.FailAsync(createUserResult.Errors.Select(r => r.Description).ToList());

        var caveatMessage = "";
        var addToRoleResult = await AddUserToRolesAsync(
            new UserRoleRequest() { Username = newUser.Username, RoleNames = new List<string>() { RoleConstants.DefaultRole } });
        if (addToRoleResult.Select(x => !x.Succeeded).Any())
            caveatMessage = $",{Environment.NewLine} Default permissions could not be added to this account, " +
                            $"please contact the administrator for assistance";
        
        var confirmationUrl = await GetEmailConfirmationUri(newUser);
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

    public async Task<string> GetEmailConfirmationUri(AppUser user)
    {
        var confirmationCode = UrlTokenGenerator.GenerateToken(32);
        // TODO: Add confirmation code to AppUser in DB, AppUser will have list of Auxiliary Attributes
        var extendedAttributes = new ExtendedAttribute
        {
            OwnerId = user.Id,
            Name = "AccountEmailConfirmation",
            Type = AttributeType.EmailConfirmation
        };
        var previousConfirmations = await _database.LoadData<ExtendedAttribute, dynamic>(
            UserExtendedAttributeGetByOwnerIdAndType.ToDboName(), new GetUserExtendedAttributesByOwnerIdAndType() 
                { Id = user.Id, Type = AttributeType.EmailConfirmation });
        var previousConfirmation = previousConfirmations.FirstOrDefault();
        
        var endpointUri = new Uri(string.Concat(_appConfig.BaseUrl, UserRoutes.ConfirmEmail));
        var confirmationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id.ToString());
        
        if (previousConfirmation is not null)
        {
            if (previousConfirmation.Completed)
                return QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", previousConfirmation.Value);
            
            extendedAttributes.Id = previousConfirmation.Id;
            extendedAttributes.Update(confirmationCode);
            // TODO: Convert previous values on extended attributes to string since we can't do a raw list
            //   Do toDB() and fromDB() and convert to a DB entity
            await _database.SaveData(UserExtendedAttributeUpdate.ToDboName(), extendedAttributes);
        }
        else
            await _database.SaveData(UserExtendedAttributeInsert.ToDboName(), extendedAttributes);
        
        return QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", confirmationCode);
    }

    public async Task<IResult> ToggleUserStatusAsync(ChangeUserActiveStateRequest activeRequest)
    {
        var requestedUser = await GetByIdAsync(activeRequest.UserId);
        if (requestedUser is null)
            return await Result<List<IdentityResult>>.FailAsync("Was unable to find a user with the provided information");

        // TODO: Add permission for toggling user status and validation current user has permissions
        var adminRole = await _roleService.GetByNameAsync(RoleConstants.AdminRole);
        var requestedUserIsAdmin = await IsInRoleAsync(new RoleMembershipRequest() { UserId = requestedUser!.Id, RoleId = adminRole!.Id });
        if (requestedUserIsAdmin)
            return await Result.FailAsync("Administrators cannot be toggled, please remove admin privileges first");

        try
        {
            await UpdateAsync(new UpdateUserRequest() { IsActive = activeRequest.IsActive});
            return await Result.SuccessAsync($"{requestedUser.Username} active status set to: {activeRequest.IsActive}");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync($"Failed to update the active status of the user account '{requestedUser.Username}'");
        }
    }

    public async Task<IResult<List<RoleResponse>>> GetRolesAsync(Guid userId)
    {
        var foundUser = await GetByIdAsync(userId);
        if (foundUser is null)
            return await Result<List<RoleResponse>>.FailAsync("Was unable to find a user with the provided information");

        var userRoles = await _database
            .LoadData<RoleResponse, dynamic>(JunctionUserRoleGetRolesOfUser.ToDboName(), new {UserId = userId});

        var roleResponses = userRoles.ToList();
        
        return await Result<List<RoleResponse>>.SuccessAsync(roleResponses);
    }

    public async Task<IResult<string>> ConfirmEmailAsync(Guid userId, string confirmationCode)
    {\
        var foundUser = await GetByIdAsync(userId);
        if (foundUser is null)
            return await Result<string>.FailAsync("Was unable to find a user with the provided information");
        
        var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmationCode));
        // TODO: Integrate user manager email confirmation after finishing confirmation token generation
        var result = await _userManager.ConfirmEmailAsync(foundUser, decodedCode);
        if (!result.Succeeded)
            throw new ApiException($"An error occurred attempting to confirm email: {foundUser.Email}");
        
        return await Result<string>.SuccessAsync(foundUser.Id.ToString(), $"Account Confirmed for {foundUser.Email}.");
    }

    public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest forgotRequest)
    {
        var foundUser = await GetByEmailAsync(forgotRequest.Email!);
        if (foundUser is null)
            return await Result.FailAsync("An internal server error occurred, please contact the administrator");

        if (!foundUser.EmailConfirmed)
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
        var foundUser = await GetByEmailAsync(resetRequest.Email);
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