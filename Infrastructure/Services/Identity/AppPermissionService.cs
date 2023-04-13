using Application.Models.Identity;
using Application.Models.Web;
using Application.Services.Identity;

namespace Infrastructure.Services.Identity;

public class AppPermissionService : IAppPermissionService
{
    public async Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllAsync()
    {
        try
        {

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

        }
        catch (Exception ex)
        {
            return await Result<int>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppPermissionSlim>> GetByIdAsync(Guid id)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<AppPermissionSlim>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppPermissionSlim>> GetByUserIdAndValueAsync(Guid userId, string claimValue)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<AppPermissionSlim>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppPermissionSlim>> GetByRoleIdAndValueAsync(Guid roleId, string claimValue)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<AppPermissionSlim>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllByNameAsync(string roleName)
    {
        try
        {

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

        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppPermissionSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<Guid>> CreateAsync(AppPermissionCreate createObject)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> UpdateAsync(AppPermissionUpdate updateObject)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        try
        {

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

        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }
}