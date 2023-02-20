using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Constants.Identity;
using Application.Database.MsSql.Identity;
using Application.Database.MsSql.Shared;
using Application.Helpers.Identity;
using Application.Helpers.Web;
using Application.Models.Identity;
using Application.Models.Web;
using Application.Repositories.Identity;
using Application.Services.Database;
using Application.Services.Identity;
using Application.Settings.AppSettings;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Identity;
using Domain.Models.Todo;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Shared.ApiRoutes.Identity;
using Shared.Enums.Identity;
using Shared.Requests.Identity;
using Shared.Responses.Identity;

namespace Infrastructure.Repositories.Identity;

public class AppUserRepository : IAppUserRepository, IUserEmailStore<AppUserDb>, IUserPhoneNumberStore<AppUserDb>,
    IUserTwoFactorStore<AppUserDb>, IUserPasswordStore<AppUserDb>
{
    private const string InvalidErrorMessage = "The username and password combination provided is invalid";
    private const string GenericFailureMessage = "An internal server error occurred, please contact the administrator";

    private readonly ISqlDataService _database;
    private readonly AppConfiguration _appConfig;
    private readonly IFluentEmail _mailService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppRoleRepository _roleRepository;

    public AppUserRepository(ISqlDataService database, AppConfiguration appConfig, IFluentEmail mailService,
        ICurrentUserService currentUserService, IAppRoleRepository roleService)
    {
        _database = database;
        _appConfig = appConfig;
        _mailService = mailService;
        _currentUserService = currentUserService;
        _roleRepository = roleService;
    }

    public async Task<IEnumerable<AppUserDb>> GetAllAsync()
    {
        return await _database.LoadData<AppUserDb, dynamic>(AppUsers.GetAll, new { });
    }

    public async Task<int> GetCountAsync()
    {
        var rowCount = await _database.LoadData<int, dynamic>(General.GetRowCount, new {TableName = "Users"});
        return rowCount.FirstOrDefault();
    }

    public async Task<AppUserDb?> GetByIdAsync(Guid userId)
    {
        var foundUser = await _database.LoadData<AppUserDb, dynamic>( AppUsers.GetById, new { Id = userId });
        return foundUser.FirstOrDefault();
    }
    
    public async Task<AppUserFull?> GetByIdFullAsync(Guid userId)
    {
        var foundUser = (await _database.LoadData<AppUserDb, dynamic>(AppUsers.GetById, new {Id = userId})).FirstOrDefault();
        if (foundUser is null)
            return null;

        var fullUser = foundUser.ToFullObject();
        
        // TODO: Add roles and extended attributes after both have been fully implemented
        // var foundRoles = await _database.LoadData<AppRoleDb, dynamic>(AppRoles.)
        
        return fullUser;
    }

    public async Task<AppUserDb?> GetByUsernameAsync(string username)
    {
        var foundUser = await _database.LoadData<AppUserDb, dynamic>(
            AppUsers.GetByUsername, new { Username = username });
        
        return foundUser.FirstOrDefault();
    }

    public async Task<AppUserDb?> GetByEmailAsync(string email)
    {
        var foundUser = await _database.LoadData<AppUserDb, dynamic>(
            AppUsers.GetByEmail, new { Email = email });
        return foundUser.FirstOrDefault();
    }

    public async Task<bool> IsInRoleAsync(Guid userId, Guid roleId)
    {
        var foundMembership = await _database.LoadData<AppUserRoleJunctionDb, dynamic>(
            AppUserRoleJunctions.GetByUserRoleId, new { UserId = userId, RoleId = roleId });
        
        return foundMembership.FirstOrDefault() is not null;
    }

    public async Task<List<IResult>> AddUserToRolesAsync(UserRoleRequest roleRequest)
    {
        // TODO: Need to refactor this to not use IResult to be more in line w/ repository methods
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
                var foundRole = await _roleRepository.GetByNameAsync(roleName);
                if (foundRole is null)
                {
                    resultList.Add(await Result.FailAsync($"Role '{roleName}' wasn't found"));
                    continue;
                }

                var alreadyHasRole = await IsInRoleAsync(foundRole.Id, foundUser.Id);
                if (alreadyHasRole)
                {
                    resultList.Add(await Result.SuccessAsync($"User already has the '{roleName}' Role"));
                    continue;   
                }

                await _database.SaveData(AppUserRoleJunctions.Insert, new { RoleId = foundRole.Id, UserId = foundUser.Id });
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

    public async Task UpdateAsync(AppUserUpdate updateObject)
    {
        await _database.SaveData(AppUsers.Update, updateObject);
    }

    public async Task DeleteAsync(Guid userId)
    {
        await _database.SaveData(AppUsers.Delete, new { Id = userId });
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
        
        await UpdateAsync(updateObject);
    }

    public async Task SetUserPassword(Guid userId, string newPassword)
    {
        var updateObject = new AppUserUpdate() { Id = userId };
        
        AccountHelpers.GetPasswordHash(newPassword, out var hash, out var salt);
        updateObject.PasswordSalt = salt;
        updateObject.PasswordHash = hash.ToString()!;
        
        await UpdateAsync(updateObject);
    }

    public async Task<string> GetNormalizedUserNameAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.NormalizedUserName);
    }

    public async Task SetNormalizedUserNameAsync(AppUserDb user, string normalizedName, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate() { Id = user.Id, NormalizedUserName = normalizedName };
        
        await UpdateAsync(updateObject);
    }

    public async Task<Guid?> CreateAsync(AppUserCreate createObject)
    {
        var createdId = await _database.SaveDataReturnId(AppUsers.Insert, createObject);
        if (createdId == Guid.Empty)
            return null;

        return createdId;
    }

    public async Task<IdentityResult> CreateAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        try
        {
            await CreateAsync(user.ToCreateObject());
            return await Task.FromResult(IdentityResult.Success);
        }
        catch (Exception ex)
        {
            return await Task.FromResult(IdentityResult.Failed(new IdentityError(){Code = ex.Source, Description = ex.Message}));
        }
    }

    public async Task<IdentityResult> CreateAsync(AppUserDb user, string password, CancellationToken cancellationToken)
    {
        try
        {
            var createUser = user.ToCreateObject();
            
            AccountHelpers.GetPasswordHash(password, out var hash, out var salt);
            createUser.PasswordSalt = salt;
            createUser.PasswordHash = hash.ToString()!;
            
            await CreateAsync(createUser);
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

            await UpdateAsync(updateUser);
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
            await DeleteAsync(user.Id);
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
        return (await GetByIdAsync(id))!;
    }

    public async Task<AppUserDb> FindByNameAsync(string normalizedUsername, CancellationToken cancellationToken)
    {
        var foundUser = await _database.LoadData<AppUserDb, dynamic>(
            AppUsers.GetByNormalizedUsername, new { NormalizedUsername = normalizedUsername });
        
        return foundUser.FirstOrDefault()!;
    }

    public async Task SetEmailAsync(AppUserDb user, string email, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate() {Id = user.Id, Email = email};
        await UpdateAsync(updateObject);
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
        await UpdateAsync(updateObject);
    }

    public async Task<AppUserDb> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        var foundUser = await _database.LoadData<AppUserDb, dynamic>(
            AppUsers.GetByNormalizedEmail, new { NormalizedEmail = normalizedEmail });
        
        return foundUser.FirstOrDefault()!;
    }

    public async Task<string> GetNormalizedEmailAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.NormalizedEmail);
    }

    public async Task SetNormalizedEmailAsync(AppUserDb user, string normalizedEmail, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate() {Id = user.Id, NormalizedEmail = normalizedEmail};
        await UpdateAsync(updateObject);
    }

    public async Task SetPhoneNumberAsync(AppUserDb user, string phoneNumber, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate() {Id = user.Id, PhoneNumber = phoneNumber};
        await UpdateAsync(updateObject);
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
        await UpdateAsync(updateObject);
    }

    public async Task SetTwoFactorEnabledAsync(AppUserDb user, bool enabled, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate() {Id = user.Id, TwoFactorEnabled = enabled};
        await UpdateAsync(updateObject);
    }

    public async Task<bool> GetTwoFactorEnabledAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.TwoFactorEnabled);
    }

    public async Task SetPasswordHashAsync(AppUserDb user, string passwordHash, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate() {Id = user.Id, PasswordHash = passwordHash};
        await UpdateAsync(updateObject);
    }

    public async Task<string> GetPasswordHashAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.PasswordHash);
    }

    public async Task<bool> HasPasswordAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
    }

    public async Task<IResult<UserLoginResponse>> LoginAsync(UserLoginRequest loginRequest)
    {
        var user = await GetByUsernameAsync(loginRequest.Username);
        if (user is null)
            return await Result<UserLoginResponse>.FailAsync(InvalidErrorMessage);
        
        var passwordValid = AccountHelpers.IsPasswordCorrect(
            loginRequest.Password, Encoding.UTF8.GetBytes(user.PasswordHash), user.PasswordSalt);
        if (!passwordValid)
            return await Result<UserLoginResponse>.FailAsync(InvalidErrorMessage);
        if (!user.IsActive)
            return await Result<UserLoginResponse>.FailAsync("Your account is disabled, please contact the administrator.");
        if (!user.EmailConfirmed)
            return await Result<UserLoginResponse>.FailAsync("Your email has not been confirmed, please confirm your email.");

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
        var adminRole = await _roleRepository.GetByNameAsync(RoleConstants.AdminRole);
        var userIsAdmin = await IsInRoleAsync(currentUser!.Id, adminRole!.Id);
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
        var adminRole = await _roleRepository.GetByNameAsync(RoleConstants.AdminRole);
        
        var currentUserIsAdmin = await IsInRoleAsync(currentUser!.Id, adminRole!.Id);
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
            var foundRole = await _roleRepository.GetByNameAsync(role);
            if (foundRole is null)
            {
                resultList.Add(IdentityResult.Failed(new IdentityError() 
                    {Code = "RoleError", Description = $"Role couldn't be found using the provided name: {role}"}));
                continue;
            }
            
            var changesMade = await _database.SaveData(
                AppUserRoleJunctions.Insert, new { UserId = requestedUser.Id, RoleId = foundRole.Id });
            if (changesMade < 1)
                resultList.Add(IdentityResult.Failed(new IdentityError() 
                    { Code = "RoleError", Description = $"Role already wasn't on the provided user: {role}" }));
            else
                resultList.Add(IdentityResult.Success);
        }
        
        return await Result<List<IdentityResult>>.SuccessAsync(resultList);
    }

    public async Task<IResult<IdentityResult>> EnforceRolesAsync(UserRoleRequest roleRequest)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> RegisterAsync(UserRegisterRequest registerRequest)
    {
        var matchingUserName = await GetByUsernameAsync(registerRequest.Username);
        if (matchingUserName != null)
        {
            return await Result.FailAsync(string.Format($"Username {registerRequest.Username} is already in use, please try again"));
        }
        
        var newUser = new AppUserDb()
        {
            Email = registerRequest.Email,
            UserName = registerRequest.Username,
        };

        var matchingEmail = await GetByEmailAsync(registerRequest.Email);
        if (matchingEmail is not null)
            return await Result.FailAsync($"The email address {registerRequest.Email} is already in use, please try again");
        
        var createUserResult = await CreateAsync(newUser, registerRequest.Password, CancellationToken.None);
        if (!createUserResult.Succeeded)
            return await Result.FailAsync(createUserResult.Errors.Select(r => r.Description).ToList());

        var caveatMessage = "";
        var addToRoleResult = await AddUserToRolesAsync(
            new UserRoleRequest() { Username = newUser.Username, RoleNames = new List<string>() { RoleConstants.DefaultRole } });
        if (addToRoleResult.Select(x => !x.Succeeded).Any())
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
        var previousConfirmations = await _database.LoadData<ExtendedAttribute, dynamic>(
            UserExtendedAttributeGetByOwnerIdAndType.ToDboName(), new GetUserExtendedAttributesByOwnerIdAndType() 
                { Id = user.Id, Type = AttributeType.EmailConfirmation });
        var previousConfirmation = previousConfirmations.FirstOrDefault();
        
        var endpointUri = new Uri(string.Concat(_appConfig.BaseUrl, UserRoutes.ConfirmEmail));
        var confirmationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id.ToString());
        
        // Previous pending account registration exists, return current value
        if (previousConfirmation is not null)
            return QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", previousConfirmation.Value);

        // No currently pending account registration exists so we'll generate a new one, add it to the provided user
        //   and return the generated confirmation uri
        var confirmationCode = UrlHelpers.GenerateToken(32);
        var extendedAttribute = new ExtendedAttribute
        {
            OwnerId = user.Id,
            Name = "AccountEmailConfirmation",
            Type = AttributeType.EmailConfirmation,
            Value = confirmationCode
        };
        await _database.SaveData(UserExtendedAttributeInsert.ToDboName(), extendedAttribute);
        
        return QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", confirmationCode);
    }

    public async Task<IResult> ToggleUserStatusAsync(ChangeUserActiveStateRequest activeRequest)
    {
        var requestedUser = await GetByIdAsync(activeRequest.UserId);
        if (requestedUser is null)
            return await Result<List<IdentityResult>>.FailAsync("Was unable to find a user with the provided information");

        // TODO: Add permission for toggling user status and validation current user has permissions
        var adminRole = await _roleRepository.GetByNameAsync(RoleConstants.AdminRole);
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
    {
        var foundUser = await GetByIdAsync(userId);
        if (foundUser is null)
            return await Result<string>.FailAsync("Was unable to find a user with the provided information");
        
        var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmationCode));
        
        var previousConfirmations = await _database.LoadData<ExtendedAttribute, dynamic>(
            UserExtendedAttributeGetByOwnerIdAndType.ToDboName(), new GetUserExtendedAttributesByOwnerIdAndType() 
                { Id = foundUser.Id, Type = AttributeType.EmailConfirmation });
        var previousConfirmation = previousConfirmations.FirstOrDefault();

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
        var foundUser = await GetByEmailAsync(forgotRequest.Email!);
        if (foundUser is null)
            return await Result.FailAsync(GenericFailureMessage);

        if (!foundUser.EmailConfirmed)
            return await Result.FailAsync("Email hasn't been confirmed for this account, please contact the administrator");
        
        var previousResets = await _database.LoadData<ExtendedAttribute, dynamic>(
            UserExtendedAttributeGetByOwnerIdAndType.ToDboName(), new GetUserExtendedAttributesByOwnerIdAndType() 
                { Id = foundUser.Id, Type = AttributeType.ForgotPassword });
        var previousReset = previousResets.FirstOrDefault();
        
        var endpointUri = new Uri(string.Concat(_appConfig.BaseUrl, UserRoutes.ConfirmEmail));
        var confirmationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", foundUser.Id.ToString());
        
        // Previous pending forgot password exists, return current value
        if (previousReset is not null)
            return await Result.SuccessAsync(QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", previousReset.Value));

        // No currently pending forgot password request exists so we'll generate a new one, add it to the provided user
        //   and return the generated reset uri
        var confirmationCode = UrlTokenGenerator.GenerateToken(32);
        var extendedAttribute = new ExtendedAttribute
        {
            OwnerId = foundUser.Id,
            Name = "ForgotPasswordReset",
            Type = AttributeType.ForgotPassword,
            Value = confirmationCode
        };
        await _database.SaveData(UserExtendedAttributeInsert.ToDboName(), extendedAttribute);
        
        return await Result.SuccessAsync(QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", extendedAttribute.Value));
    }

    public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest resetRequest)
    {
        var foundUser = await GetByEmailAsync(resetRequest.Email);
        if (foundUser is null)
            return await Result.FailAsync(GenericFailureMessage);
        
        var previousResets = await _database.LoadData<ExtendedAttribute, dynamic>(
            UserExtendedAttributeGetByOwnerIdAndType.ToDboName(), new GetUserExtendedAttributesByOwnerIdAndType() 
                { Id = foundUser.Id, Type = AttributeType.ForgotPassword });
        var previousReset = previousResets.FirstOrDefault();
        
        if (previousReset is null)
            return await Result.FailAsync(GenericFailureMessage);
        if (resetRequest.Password != resetRequest.ConfirmPassword)
            return await Result.FailAsync("Password and Confirm Password provided don't match, please try again");
        if (resetRequest.RequestCode == previousReset.Value)
            await SetUserPassword(foundUser.Id, resetRequest.Password);

        return await Result.SuccessAsync("Password reset was successful");
    }

    public void Dispose()
    {
        // Using Dapper, nothing to dispose
    }

    public async Task<IResult<UserLoginResponse>> GetRefreshTokenAsync(RefreshTokenRequest? refreshRequest)
    {
        if (refreshRequest is null)
            return await Result<UserLoginResponse>.FailAsync("Invalid Client Token.");
        
        var userPrincipal = GetPrincipalFromExpiredToken(refreshRequest.Token!);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await GetByEmailAsync(userEmail);
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