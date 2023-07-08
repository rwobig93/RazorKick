using Application.Mappers.Identity;
using Application.Models.Identity.Permission;
using Application.Models.Web;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Application.Services.System;

namespace Infrastructure.Services.Identity;

public class AppPermissionService : IAppPermissionService
{
    private readonly IAppPermissionRepository _permissionRepository;
    private readonly IDateTimeService _dateTime;
    private readonly IRunningServerState _serverState;

    public AppPermissionService(IAppPermissionRepository permissionRepository, IDateTimeService dateTime, IRunningServerState serverState)
    {
        _permissionRepository = permissionRepository;
        _dateTime = dateTime;
        _serverState = serverState;
    }

    public async Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllAsync()
    {
        try
        {
            var permissionsRequest = await _permissionRepository.GetAllAsync();
            if (!permissionsRequest.Success)
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
            if (!searchResult.Success)
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
            if (!countRequest.Success)
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
            if (!foundPermission.Success)
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
            if (!foundPermission.Success)
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
            if (!foundPermission.Success)
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
            if (!foundPermissions.Success)
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
            if (!foundPermissions.Success)
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
            if (!foundPermissions.Success)
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
            if (!foundPermissions.Success)
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
            if (!foundPermissions.Success)
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
            if (!foundPermissions.Success)
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

    public async Task<IResult<Guid>> CreateAsync(AppPermissionCreate createObject, bool systemUpdate = false)
    {
        try
        {
            var createRequest = await _permissionRepository.CreateAsync(createObject);
            if (!createRequest.Success)
                return await Result<Guid>.FailAsync(createRequest.ErrorMessage);

            return await Result<Guid>.SuccessAsync(createRequest.Result);
        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> UpdateAsync(AppPermissionUpdate updateObject, bool systemUpdate = false)
    {
        try
        {
            var updateRequest = await _permissionRepository.UpdateAsync(updateObject);
            if (!updateRequest.Success)
                return await Result.FailAsync(updateRequest.ErrorMessage);

            return await Result.SuccessAsync();
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> DeleteAsync(Guid permissionId)
    {
        try
        {
            var deleteRequest = await _permissionRepository.DeleteAsync(permissionId);
            if (!deleteRequest.Success)
                return await Result.FailAsync(deleteRequest.ErrorMessage);

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
            if (!checkRequest.Success)
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
            if (!checkRequest.Success)
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
            if (!checkRequest.Success)
                return await Result<bool>.FailAsync(checkRequest.ErrorMessage);

            return await Result<bool>.SuccessAsync(checkRequest.Result);
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }
}