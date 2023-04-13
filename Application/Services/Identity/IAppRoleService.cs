using Application.Models.Identity;
using Application.Models.Web;

namespace Application.Services.Identity;

public interface IAppRoleService
{
    Task<IResult<IEnumerable<AppRoleSlim>>> GetAllAsync();
    Task<IResult<int>> GetCountAsync();
    Task<IResult<AppRoleSlim>> GetByIdAsync(Guid roleId);
    Task<IResult<AppRoleFull>> GetByIdFullAsync(Guid roleId);
    Task<IResult<AppRoleSlim>> GetByNameAsync(string roleName);
    Task<IResult<AppRoleFull>> GetByNameFullAsync(Guid roleId);
    Task<IResult<IEnumerable<AppRoleSlim>>> SearchAsync(string searchText);
    Task<IResult<Guid>> CreateAsync(AppRoleCreate createObject);
    Task<IResult> UpdateAsync(AppRoleUpdate updateObject);
    Task<IResult> DeleteAsync(Guid id);
    Task<IResult> SetCreatedById(Guid roleId, Guid createdById);
    Task<IResult<bool>> IsUserInRoleAsync(Guid userId, Guid roleId);
    Task<IResult<bool>> IsUserInRoleAsync(Guid userId, string roleName);
    Task<IResult> AddUserToRoleAsync(Guid userId, Guid roleId);
    Task<IResult> RemoveUserFromRoleAsync(Guid userId, Guid roleId);
    Task<IResult<IEnumerable<AppRoleSlim>>> GetRolesForUser(Guid userId);
    Task<IResult<IEnumerable<AppUserSlim>>> GetUsersForRole(Guid roleId);
}