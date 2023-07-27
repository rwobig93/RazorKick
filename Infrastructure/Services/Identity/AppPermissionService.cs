using Application.Constants.Communication;
using Application.Constants.Identity;
using Application.Database.MsSql.Identity;
using Application.Helpers.Identity;
using Application.Helpers.Lifecycle;
using Application.Helpers.Runtime;
using Application.Mappers.Identity;
using Application.Models.Identity.Permission;
using Application.Models.Lifecycle;
using Application.Models.Web;
using Application.Repositories.Identity;
using Application.Repositories.Lifecycle;
using Application.Services.Identity;
using Application.Services.Lifecycle;
using Application.Services.System;
using Domain.Enums.Database;
using Domain.Enums.Identity;

namespace Infrastructure.Services.Identity;

public class AppPermissionService : IAppPermissionService
{
    private readonly IAppPermissionRepository _permissionRepository;
    private readonly IAppUserRepository _userRepository;
    private readonly IAppRoleRepository _roleRepository;
    private readonly IAuditTrailsRepository _auditRepository;
    private readonly IRunningServerState _serverState;
    private readonly IDateTimeService _dateTimeService;
    private readonly ISerializerService _serializer;

    public AppPermissionService(IAppPermissionRepository permissionRepository, IAppUserRepository userRepository, IAuditTrailsRepository auditRepository,
        IAppRoleRepository roleRepository, IRunningServerState serverState, IDateTimeService dateTimeService, ISerializerService serializerService)
    {
        _permissionRepository = permissionRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _auditRepository = auditRepository;
        _serverState = serverState;
        _dateTimeService = dateTimeService;
        _serializer = serializerService;
    }

    private async Task<bool> IsUserAdmin(Guid userId)
    {
        var roleCheck = await _roleRepository.IsUserInRoleAsync(userId, RoleConstants.DefaultRoles.AdminName);
        return roleCheck.Result;
    }

    private async Task<bool> IsUserModerator(Guid userId)
    {
        var roleCheck = await _roleRepository.IsUserInRoleAsync(userId, RoleConstants.DefaultRoles.ModeratorName);
        return roleCheck.Result;
    }

    public async Task<IResult<IEnumerable<AppPermissionCreate>>> GetAllAvailablePermissionsAsync()
    {
        try
        {
            var allPermissions = new List<AppPermissionCreate>();
            
            // Get all native/built-in permissions
            var allBuiltInPermissions = PermissionHelpers.GetAllBuiltInPermissions();
            allPermissions.AddRange(allBuiltInPermissions.ToAppPermissionCreates());

            // Get dynamic permissions - Service Account Admin
            var serviceAccountsRequest = await _userRepository.GetAllServiceAccountsForPermissionsAsync();
            if (!serviceAccountsRequest.Succeeded)
                return await Result<IEnumerable<AppPermissionCreate>>.FailAsync(serviceAccountsRequest.ErrorMessage);

            var allServiceAccountPermissions =
                serviceAccountsRequest.Result?.ToAppPermissionCreates(DynamicPermissionLevel.Admin) ?? new List<AppPermissionCreate>();
            allPermissions.AddRange(allServiceAccountPermissions);

            return await Result<IEnumerable<AppPermissionCreate>>.SuccessAsync(allPermissions);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppPermissionCreate>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllAssignedAsync()
    {
        try
        {
            var permissionsRequest = await _permissionRepository.GetAllAsync();
            if (!permissionsRequest.Succeeded)
                return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(permissionsRequest.ErrorMessage);

            var permissions = (permissionsRequest.Result?.ToSlims() ?? new List<AppPermissionSlim>())
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Access);

            return await Result<IEnumerable<AppPermissionSlim>>.SuccessAsync(permissions);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllAssignedPaginatedAsync(int pageNumber, int pageSize)
    {
        try
        {
            var permissionsRequest = await _permissionRepository.GetAllPaginatedAsync(pageNumber, pageSize);
            if (!permissionsRequest.Succeeded)
                return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(permissionsRequest.ErrorMessage);

            var permissions = (permissionsRequest.Result?.ToSlims() ?? new List<AppPermissionSlim>())
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Access);

            return await Result<IEnumerable<AppPermissionSlim>>.SuccessAsync(permissions);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppPermissionSlim>>> SearchAsync(string searchTerm)
    {
        try
        {
            var searchResult = await _permissionRepository.SearchAsync(searchTerm);
            if (!searchResult.Succeeded)
                return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(searchResult.ErrorMessage);

            var permissions = (searchResult.Result?.ToSlims() ?? new List<AppPermissionSlim>())
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Access);

            return await Result<IEnumerable<AppPermissionSlim>>.SuccessAsync(permissions);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppPermissionSlim>>> SearchPaginatedAsync(string searchTerm, int pageNumber, int pageSize)
    {
        try
        {
            var searchResult = await _permissionRepository.SearchPaginatedAsync(searchTerm, pageNumber, pageSize);
            if (!searchResult.Succeeded)
                return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(searchResult.ErrorMessage);

            var permissions = (searchResult.Result?.ToSlims() ?? new List<AppPermissionSlim>())
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Access);

            return await Result<IEnumerable<AppPermissionSlim>>.SuccessAsync(permissions);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<int>> GetCountAsync()
    {
        try
        {
            var countRequest = await _permissionRepository.GetCountAsync();
            if (!countRequest.Succeeded)
                return await Result<int>.FailAsync(countRequest.ErrorMessage);

            return await Result<int>.SuccessAsync(countRequest.Result);
        }
        catch (Exception ex)
        {
            return await Result<int>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppPermissionSlim?>> GetByIdAsync(Guid permissionId)
    {
        try
        {
            var foundPermission = await _permissionRepository.GetByIdAsync(permissionId);
            if (!foundPermission.Succeeded)
                return await Result<AppPermissionSlim?>.FailAsync(foundPermission.ErrorMessage);

            return await Result<AppPermissionSlim?>.SuccessAsync(foundPermission.Result?.ToSlim());
        }
        catch (Exception ex)
        {
            return await Result<AppPermissionSlim?>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppPermissionSlim?>> GetByUserIdAndValueAsync(Guid userId, string claimValue)
    {
        try
        {
            var foundPermission = await _permissionRepository.GetByUserIdAndValueAsync(userId, claimValue);
            if (!foundPermission.Succeeded)
                return await Result<AppPermissionSlim?>.FailAsync(foundPermission.ErrorMessage);

            return await Result<AppPermissionSlim?>.SuccessAsync(foundPermission.Result?.ToSlim());
        }
        catch (Exception ex)
        {
            return await Result<AppPermissionSlim?>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppPermissionSlim?>> GetByRoleIdAndValueAsync(Guid roleId, string claimValue)
    {
        try
        {
            var foundPermission = await _permissionRepository.GetByRoleIdAndValueAsync(roleId, claimValue);
            if (!foundPermission.Succeeded)
                return await Result<AppPermissionSlim?>.FailAsync(foundPermission.ErrorMessage);

            return await Result<AppPermissionSlim?>.SuccessAsync(foundPermission.Result?.ToSlim());
        }
        catch (Exception ex)
        {
            return await Result<AppPermissionSlim?>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllByRoleNameAsync(string roleName)
    {
        try
        {
            var foundPermissions = await _permissionRepository.GetAllByNameAsync(roleName);
            if (!foundPermissions.Succeeded)
                return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(foundPermissions.ErrorMessage);
            
            var permissions = (foundPermissions.Result?.ToSlims() ?? new List<AppPermissionSlim>())
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Access);

            return await Result<IEnumerable<AppPermissionSlim>>.SuccessAsync(permissions);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllByGroupAsync(string groupName)
    {
        try
        {
            var foundPermissions = await _permissionRepository.GetAllByGroupAsync(groupName);
            if (!foundPermissions.Succeeded)
                return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(foundPermissions.ErrorMessage);
            
            var permissions = (foundPermissions.Result?.ToSlims() ?? new List<AppPermissionSlim>())
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Access);

            return await Result<IEnumerable<AppPermissionSlim>>.SuccessAsync(permissions);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllByAccessAsync(string accessName)
    {
        try
        {
            var foundPermissions = await _permissionRepository.GetAllByAccessAsync(accessName);
            if (!foundPermissions.Succeeded)
                return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(foundPermissions.ErrorMessage);
            
            var permissions = (foundPermissions.Result?.ToSlims() ?? new List<AppPermissionSlim>())
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Access);

            return await Result<IEnumerable<AppPermissionSlim>>.SuccessAsync(permissions);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllForRoleAsync(Guid roleId)
    {
        try
        {
            var foundPermissions = await _permissionRepository.GetAllForRoleAsync(roleId);
            if (!foundPermissions.Succeeded)
                return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(foundPermissions.ErrorMessage);
            
            var permissions = (foundPermissions.Result?.ToSlims() ?? new List<AppPermissionSlim>())
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Access);

            return await Result<IEnumerable<AppPermissionSlim>>.SuccessAsync(permissions);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllDirectForUserAsync(Guid userId)
    {
        try
        {
            var foundPermissions = await _permissionRepository.GetAllDirectForUserAsync(userId);
            if (!foundPermissions.Succeeded)
                return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(foundPermissions.ErrorMessage);
            
            var permissions = (foundPermissions.Result?.ToSlims() ?? new List<AppPermissionSlim>())
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Access);

            return await Result<IEnumerable<AppPermissionSlim>>.SuccessAsync(permissions);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllIncludingRolesForUserAsync(Guid userId)
    {
        try
        {
            var foundPermissions = await _permissionRepository.GetAllIncludingRolesForUserAsync(userId);
            if (!foundPermissions.Succeeded)
                return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(foundPermissions.ErrorMessage);
            
            var permissions = (foundPermissions.Result?.ToSlims() ?? new List<AppPermissionSlim>())
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Access);

            return await Result<IEnumerable<AppPermissionSlim>>.SuccessAsync(permissions);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(ex.Message);
        }
    }

    private async Task<bool> CanUserDoThisAction(Guid modifyingUserId, string claimValue)
    {
        // If the user is the system user they have full reign so we let them past the permission validation
        if (modifyingUserId == _serverState.SystemUserId) return true;
        
        // If the user is an admin we let them do whatever they want
        var modifyingUserIsAdmin = await IsUserAdmin(modifyingUserId);
        if (modifyingUserIsAdmin)
            return true;

        // If the permission is a dynamic permission and the user is a moderator they can administrate the permission
        if (claimValue.StartsWith("Dynamic."))
        {
            var modifyingUserIsModerator = await IsUserModerator(modifyingUserId);
            if (modifyingUserIsModerator)
                return true;
        }
        
        // If a user has the permission and they've been given access to add/remove permissions then they are good to go,
        //    otherwise they can't add/remove a permission they themselves don't have
        var invokingUserHasRequestingPermission = await UserIncludingRolesHasPermission(modifyingUserId, claimValue);
        return invokingUserHasRequestingPermission.Data;
    }

    public async Task<IResult<Guid>> CreateAsync(AppPermissionCreate createObject, Guid modifyingUserId)
    {
        try
        {
            if (createObject.UserId == Guid.Empty && createObject.RoleId == Guid.Empty)
                return await Result<Guid>.FailAsync("UserId & RoleId cannot be empty, please provide a valid Id");
            if (createObject.UserId == GuidHelpers.GetMax() && createObject.RoleId == GuidHelpers.GetMax())
                return await Result<Guid>.FailAsync("UserId & RoleId cannot be empty, please provide a valid Id");
            if (createObject.UserId != GuidHelpers.GetMax() && createObject.RoleId != GuidHelpers.GetMax())
                return await Result<Guid>.FailAsync("Each permission assignment request can only be made for a User or Role, not both at the same time");

            if (createObject.UserId != GuidHelpers.GetMax())
            {
                var foundUser = await _userRepository.GetByIdAsync(createObject.UserId);
                if (foundUser.Result is null)
                    return await Result<Guid>.FailAsync("UserId provided is invalid");
            }
            
            if (createObject.RoleId != GuidHelpers.GetMax())
            {
                var foundRole = await _roleRepository.GetByIdAsync(createObject.RoleId);
                if (foundRole.Result is null)
                    return await Result<Guid>.FailAsync("RoleId provided is invalid");
            }

            if (!await CanUserDoThisAction(modifyingUserId, createObject.ClaimValue))
                return await Result<Guid>.FailAsync(ErrorMessageConstants.CannotAdministrateMissingPermission);

            createObject.CreatedBy = modifyingUserId;
            createObject.CreatedOn = _dateTimeService.NowDatabaseTime;

            var createRequest = await _permissionRepository.CreateAsync(createObject);
            if (!createRequest.Succeeded)
                return await Result<Guid>.FailAsync(createRequest.ErrorMessage);

            await _auditRepository.CreateAsync(new AuditTrailCreate
            {
                TableName = AppPermissionsMsSql.Table.TableName,
                RecordId = createRequest.Result,
                ChangedBy = (createObject.CreatedBy),
                Action = DatabaseActionType.Create,
                After = _serializer.Serialize(createObject)
            });
            
            return await Result<Guid>.SuccessAsync(createRequest.Result);
        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> UpdateAsync(AppPermissionUpdate updateObject, Guid modifyingUserId)
    {
        try
        {
            var foundPermission = await _permissionRepository.GetByIdAsync(updateObject.Id);
            if (!foundPermission.Succeeded || foundPermission.Result?.ClaimValue is null)
                return await Result.FailAsync(ErrorMessageConstants.GenericNotFound);

            var userCanDoAction = await CanUserDoThisAction(modifyingUserId, foundPermission.Result.ClaimValue);
            if (!userCanDoAction)
                return await Result<Guid>.FailAsync(ErrorMessageConstants.CannotAdministrateMissingPermission);

            var updateRequest = await _permissionRepository.UpdateAsync(updateObject);
            if (!updateRequest.Succeeded)
                return await Result.FailAsync(updateRequest.ErrorMessage);
            
            var permissionAfterUpdate = await _permissionRepository.GetByIdAsync(foundPermission.Result.Id);
            if (!permissionAfterUpdate.Succeeded || permissionAfterUpdate.Result is null)
            {
                await _auditRepository.CreateTroubleshootLog(_serverState, _dateTimeService, _serializer, "PermissionUpdate",
                    foundPermission.Result.Id, new Dictionary<string, string>()
                    {
                        {"PermissionValue", foundPermission.Result.ClaimValue},
                        {"ModifyingUserId", modifyingUserId.ToString()},
                        {"Details", "Was unable to retrieve updated permission"},
                        {"Error", permissionAfterUpdate.ErrorMessage}
                    });
                return await Result.FailAsync(ErrorMessageConstants.GenericErrorContactAdmin);
            }
            
            await _auditRepository.CreateAsync(new AuditTrailCreate
            {
                TableName = AppPermissionsMsSql.Table.TableName,
                RecordId = foundPermission.Result.Id,
                ChangedBy = modifyingUserId,
                Action = DatabaseActionType.Update,
                Before = _serializer.Serialize(permissionAfterUpdate.Result.ToSlim())
            });

            return await Result.SuccessAsync();
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> DeleteAsync(Guid permissionId, Guid modifyingUserId)
    {
        try
        {
            var foundPermission = await _permissionRepository.GetByIdAsync(permissionId);
            if (!foundPermission.Succeeded || foundPermission.Result?.ClaimValue is null)
                return await Result.FailAsync(ErrorMessageConstants.GenericNotFound);

            var userCanDoAction = await CanUserDoThisAction(modifyingUserId, foundPermission.Result.ClaimValue);
            if (!userCanDoAction)
                return await Result<Guid>.FailAsync(ErrorMessageConstants.CannotAdministrateMissingPermission);
            
            var deleteRequest = await _permissionRepository.DeleteAsync(foundPermission.Result.Id);
            if (!deleteRequest.Succeeded)
                return await Result.FailAsync(deleteRequest.ErrorMessage);
            
            await _auditRepository.CreateAsync(new AuditTrailCreate
            {
                TableName = AppPermissionsMsSql.Table.TableName,
                RecordId = foundPermission.Result.Id,
                ChangedBy = modifyingUserId,
                Action = DatabaseActionType.Delete,
                Before = _serializer.Serialize(foundPermission.Result.ToSlim())
            });

            return await Result.SuccessAsync();
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<bool>> UserHasDirectPermission(Guid userId, string permissionValue)
    {
        try
        {
            var checkRequest = await _permissionRepository.UserHasDirectPermission(userId, permissionValue);
            if (!checkRequest.Succeeded)
                return await Result<bool>.FailAsync(checkRequest.ErrorMessage);

            return await Result<bool>.SuccessAsync(checkRequest.Result);
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<bool>> UserIncludingRolesHasPermission(Guid userId, string permissionValue)
    {
        try
        {
            var checkRequest = await _permissionRepository.UserIncludingRolesHasPermission(userId, permissionValue);
            if (!checkRequest.Succeeded)
                return await Result<bool>.FailAsync(checkRequest.ErrorMessage);

            return await Result<bool>.SuccessAsync(checkRequest.Result);
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<bool>> RoleHasPermission(Guid roleId, string permissionValue)
    {
        try
        {
            var checkRequest = await _permissionRepository.RoleHasPermission(roleId, permissionValue);
            if (!checkRequest.Succeeded)
                return await Result<bool>.FailAsync(checkRequest.ErrorMessage);

            return await Result<bool>.SuccessAsync(checkRequest.Result);
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }
}