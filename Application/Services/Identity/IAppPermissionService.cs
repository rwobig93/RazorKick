using Application.Models.Identity;
using Application.Models.Web;

namespace Application.Services.Identity;

public interface IAppPermissionService
{
    Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllAsync();
    Task<IResult<IEnumerable<AppPermissionSlim>>> SearchAsync(string searchTerm);
    Task<IResult<int>> GetCountAsync();
    Task<IResult<AppPermissionSlim>> GetByIdAsync(Guid id);
    Task<IResult<AppPermissionSlim>> GetByUserIdAndValueAsync(Guid userId, string claimValue);
    Task<IResult<AppPermissionSlim>> GetByRoleIdAndValueAsync(Guid roleId, string claimValue);
    Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllByNameAsync(string roleName);
    Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllByGroupAsync(string groupName);
    Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllByAccessAsync(string accessName);
    Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllForRoleAsync(Guid roleId);
    Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllDirectForUserAsync(Guid userId);
    Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllIncludingRolesForUserAsync(Guid userId);
    Task<IResult<Guid>> CreateAsync(AppPermissionCreate createObject);
    Task<IResult> UpdateAsync(AppPermissionUpdate updateObject);
    Task<IResult> DeleteAsync(Guid id);
    Task<IResult<bool>> UserHasDirectPermission(Guid userId, string permissionValue);
    Task<IResult<bool>> UserIncludingRolesHasPermission(Guid userId, string permissionValue);
    Task<IResult<bool>> RoleHasPermission(Guid roleId, string permissionValue);
}