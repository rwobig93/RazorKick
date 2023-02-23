using Application.Models.Identity;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Database;

namespace Application.Repositories.Identity;

public interface IAppPermissionRepository
{
    Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllAsync();
    Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> SearchAsync(string searchTerm);
    Task<DatabaseActionResult<int>> GetCountAsync();
    Task<DatabaseActionResult<AppPermissionDb>> GetByIdAsync(Guid id);
    Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllByNameAsync(string roleName);
    Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllByGroupAsync(string groupName);
    Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllForRoleAsync(Guid roleId);
    Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllForUserAsync(Guid userId);
    Task<DatabaseActionResult<Guid>> CreateAsync(AppPermissionCreate createObject);
    Task<DatabaseActionResult> UpdateAsync(AppPermissionUpdate updateObject);
    Task<DatabaseActionResult> DeleteAsync(Guid id);
}