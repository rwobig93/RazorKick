using Application.Constants.Communication;
using Application.Constants.Identity;
using Application.Mappers.Identity;
using Application.Models.Identity;
using Application.Models.Web;
using Application.Repositories.Identity;
using Application.Requests.Identity.User;
using Application.Services.Identity;
using Application.Services.System;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.Identity;

public class AppIdentityService : IAppIdentityService
{
    private readonly IAppUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppRoleRepository _roleRepository;
    private readonly IRunningServerState _serverState;
    private readonly IDateTimeService _dateTime;

    public AppIdentityService(IAppUserRepository userRepository, ICurrentUserService currentUserService, IAppRoleRepository roleRepository,
        IRunningServerState serverState, IDateTimeService dateTime)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _roleRepository = roleRepository;
        _serverState = serverState;
        _dateTime = dateTime;
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
            // NormalizedUserName = newUserName.NormalizeForDatabase(),
            LastModifiedBy = _serverState.SystemUserId,
            LastModifiedOn = _dateTime.NowDatabaseTime
        };
        
        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task<string> GetNormalizedUserNameAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.Username);
        // return await Task.FromResult(user.NormalizedUserName);
    }

    public async Task SetNormalizedUserNameAsync(AppUserDb user, string normalizedName, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate
        {
            // Id = user.Id, NormalizedUserName = normalizedName,
            Id = user.Id, Username = normalizedName,
            LastModifiedBy = _serverState.SystemUserId,
            LastModifiedOn = _dateTime.NowDatabaseTime
        };

        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task<IdentityResult> CreateAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        try
        {
            user.LastModifiedBy = _serverState.SystemUserId;
            user.LastModifiedOn = _dateTime.NowDatabaseTime;
            
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
            var updateUser = user.ToUpdate();
            updateUser.LastModifiedOn = DateTime.Now;
            updateUser.LastModifiedBy = _serverState.SystemUserId;
            updateUser.LastModifiedOn = _dateTime.NowDatabaseTime;


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
        return (await _userRepository.GetByUsernameAsync(normalizedUsername)).Result!;
    }

    public async Task SetEmailAsync(AppUserDb user, string email, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate
        {
            Id = user.Id, Email = email,
            LastModifiedBy = _serverState.SystemUserId,
            LastModifiedOn = _dateTime.NowDatabaseTime
        };

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
        var updateObject = new AppUserUpdate
        {
            Id = user.Id, EmailConfirmed = confirmed,
            LastModifiedBy = _serverState.SystemUserId,
            LastModifiedOn = _dateTime.NowDatabaseTime
        };

        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task<AppUserDb> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return (await _userRepository.GetByEmailAsync(normalizedEmail)).Result!;
    }

    public async Task<string> GetNormalizedEmailAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.Email);
        // return await Task.FromResult(user.NormalizedEmail);
    }

    public async Task SetNormalizedEmailAsync(AppUserDb user, string email, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate
        {
            // Id = user.Id, NormalizedEmail = normalizedEmail,
            Id = user.Id, Email = email,
            LastModifiedBy = _serverState.SystemUserId,
            LastModifiedOn = _dateTime.NowDatabaseTime
        };

        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task SetPhoneNumberAsync(AppUserDb user, string phoneNumber, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate
        {
            Id = user.Id, PhoneNumber = phoneNumber,
            LastModifiedBy = _serverState.SystemUserId,
            LastModifiedOn = _dateTime.NowDatabaseTime
        };

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
        var updateObject = new AppUserUpdate
        {
            Id = user.Id, PhoneNumberConfirmed = confirmed,
            LastModifiedBy = _serverState.SystemUserId,
            LastModifiedOn = _dateTime.NowDatabaseTime
        };
        
        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task SetTwoFactorEnabledAsync(AppUserDb user, bool enabled, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate
        {
            Id = user.Id, TwoFactorEnabled = enabled,
            LastModifiedBy = _serverState.SystemUserId,
            LastModifiedOn = _dateTime.NowDatabaseTime
        };
        
        await _userRepository.UpdateAsync(updateObject);
    }

    public async Task<bool> GetTwoFactorEnabledAsync(AppUserDb user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.TwoFactorEnabled);
    }

    public async Task SetPasswordHashAsync(AppUserDb user, string passwordHash, CancellationToken cancellationToken)
    {
        var updateObject = new AppUserUpdate
        {
            Id = user.Id, PasswordHash = passwordHash,
            LastModifiedBy = _serverState.SystemUserId,
            LastModifiedOn = _dateTime.NowDatabaseTime
        };
        
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

        var currentUser = await _currentUserService.GetCurrentUserDb();
        if (currentUser is null)
            return await Result<List<IdentityResult>>.FailAsync(ErrorMessageConstants.GenericError);
        
        var adminRole = (await _roleRepository.GetByNameAsync(RoleConstants.DefaultRoles.AdminName)).Result;
        var userIsAdmin = (await _roleRepository.IsUserInRoleAsync(currentUser.Id, adminRole!.Id)).Result;
        var requestContainsAdmin = roleRequest.RoleNames.Contains(RoleConstants.DefaultRoles.AdminName);
        
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

        var currentUser = await _currentUserService.GetCurrentUserDb();
        if (currentUser is null)
            return await Result<List<IdentityResult>>.FailAsync(ErrorMessageConstants.GenericError);
        var adminRole = (await _roleRepository.GetByNameAsync(RoleConstants.DefaultRoles.AdminName)).Result;
        
        var currentUserIsAdmin = (await _roleRepository.IsUserInRoleAsync(currentUser.Id, adminRole!.Id)).Result;
        var requestContainsAdmin = roleRequest.RoleNames.Contains(adminRole.Name);
        var requestIsForSelf = requestedUser.Id == currentUser.Id;
        var requestIsForDefaultAdmin = requestedUser.Username == UserConstants.DefaultUsers.AdminUsername;
        
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

    public async Task<IResult> ToggleUserStatusAsync(ChangeUserEnabledStateRequest activeRequest)
    {
        var requestedUser = (await _userRepository.GetByIdAsync(activeRequest.UserId)).Result;
        if (requestedUser is null)
            return await Result<List<IdentityResult>>.FailAsync("Was unable to find a user with the provided information");

        // TODO: Add permission for toggling user status and validation current user has permissions
        var adminRole = (await _roleRepository.GetByNameAsync(RoleConstants.DefaultRoles.AdminName)).Result;
        var requestedUserIsAdmin =
            (await _roleRepository.IsUserInRoleAsync(requestedUser.Id, adminRole!.Id)).Result;
        if (requestedUserIsAdmin)
            return await Result.FailAsync("Administrators cannot be toggled, please remove admin privileges first");

        try
        {
            var userUpdate = new AppUserUpdate
            {
                Id = requestedUser.Id, AuthState = activeRequest.IsEnabled ? AuthState.Enabled : AuthState.Disabled,
                LastModifiedBy = _serverState.SystemUserId,
                LastModifiedOn = _dateTime.NowDatabaseTime
            };
            
            await _userRepository.UpdateAsync(userUpdate);
            return await Result.SuccessAsync($"{requestedUser.Username} active status set to: {activeRequest.IsEnabled}");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(
                $"Failed to update the active status of the user account '{requestedUser.Username}': {ex.Message}");
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
}