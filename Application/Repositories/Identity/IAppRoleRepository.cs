using Application.Models.Identity;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Database;

namespace Application.Repositories.Identity;

public interface IAppRoleRepository
{
    Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> GetAllAsync();
    Task<DatabaseActionResult<int>> GetCountAsync();
    Task<DatabaseActionResult<AppRoleDb>> GetByIdAsync(Guid id);
    Task<DatabaseActionResult<AppRoleDb>> GetByNameAsync(string roleName);
    Task<DatabaseActionResult<AppRoleDb>> GetByNormalizedNameAsync(string normalizedRoleName);
    Task<DatabaseActionResult<Guid>> CreateAsync(AppRoleCreate createObject);
    Task<DatabaseActionResult> UpdateAsync(AppRoleUpdate updateObject);
    Task<DatabaseActionResult> DeleteAsync(Guid id);
    Task<DatabaseActionResult> SetCreatedById(Guid roleId, Guid createdById);
    Task<DatabaseActionResult<bool>> IsUserInRoleAsync(Guid userId, Guid roleId);
    Task<DatabaseActionResult> AddUserToRoleAsync(Guid userId, Guid roleId);
    Task<DatabaseActionResult> RemoveUserFromRoleAsync(Guid userId, Guid roleId);
    Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> GetRolesForUser(Guid userId);
}
