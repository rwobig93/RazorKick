using Application.Models.Identity;
using Application.Models.Identity.Role;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Database;

namespace Application.Repositories.Identity;

public interface IAppRoleRepository
{
    Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> GetAllAsync();
    Task<DatabaseActionResult<int>> GetCountAsync();
    Task<DatabaseActionResult<AppRoleDb>> GetByIdAsync(Guid roleId);
    Task<DatabaseActionResult<AppRoleDb>> GetByNameAsync(string roleName);
    Task<DatabaseActionResult<AppRoleDb>> GetByNormalizedNameAsync(string normalizedRoleName);
    Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> SearchAsync(string searchText);
    Task<DatabaseActionResult<Guid>> CreateAsync(AppRoleCreate createObject, bool systemUpdate = false);
    Task<DatabaseActionResult> UpdateAsync(AppRoleUpdate updateObject, bool systemUpdate = false);
    Task<DatabaseActionResult> DeleteAsync(Guid id);
    Task<DatabaseActionResult> SetCreatedById(Guid roleId, Guid createdById);
    Task<DatabaseActionResult<bool>> IsUserInRoleAsync(Guid userId, Guid roleId);
    Task<DatabaseActionResult<bool>> IsUserInRoleAsync(Guid userId, string roleName);
    Task<DatabaseActionResult> AddUserToRoleAsync(Guid userId, Guid roleId);
    Task<DatabaseActionResult> RemoveUserFromRoleAsync(Guid userId, Guid roleId);
    Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> GetRolesForUser(Guid userId);
    Task<DatabaseActionResult<IEnumerable<AppUserDb>>> GetUsersForRole(Guid roleId);
}
