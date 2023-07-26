using Application.Models.Identity.Permission;
using Application.Models.Web;

namespace Application.Services.Identity;

public interface IAppPermissionService
{
    Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllAsync();
    Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllPaginatedAsync(int pageNumber, int pageSize);
    Task<IResult<IEnumerable<AppPermissionSlim>>> SearchAsync(string searchTerm);
    Task<IResult<IEnumerable<AppPermissionSlim>>> SearchPaginatedAsync(string searchTerm, int pageNumber, int pageSize);
    Task<IResult<int>> GetCountAsync();
    Task<IResult<AppPermissionSlim?>> GetByIdAsync(Guid permissionId);
    Task<IResult<AppPermissionSlim?>> GetByUserIdAndValueAsync(Guid userId, string claimValue);
    Task<IResult<AppPermissionSlim?>> GetByRoleIdAndValueAsync(Guid roleId, string claimValue);
    Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllByRoleNameAsync(string roleName);
    Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllByGroupAsync(string groupName);
    Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllByAccessAsync(string accessName);
    Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllForRoleAsync(Guid roleId);
    Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllDirectForUserAsync(Guid userId);
    Task<IResult<IEnumerable<AppPermissionSlim>>> GetAllIncludingRolesForUserAsync(Guid userId);
    Task<IResult<Guid>> CreateAsync(AppPermissionCreate createObject, Guid modifyingUserId);
    Task<IResult> UpdateAsync(AppPermissionUpdate updateObject, Guid modifyingUserId);
    Task<IResult> DeleteAsync(Guid permissionId, Guid modifyingUserId);
    Task<IResult<bool>> UserHasDirectPermission(Guid userId, string permissionValue);
    Task<IResult<bool>> UserIncludingRolesHasPermission(Guid userId, string permissionValue);
    Task<IResult<bool>> RoleHasPermission(Guid roleId, string permissionValue);
}