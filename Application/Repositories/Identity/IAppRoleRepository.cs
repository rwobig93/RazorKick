using Application.Models.Web;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Database;
using Shared.Requests.Identity;
using Shared.Responses.Identity;

namespace Application.Repositories.Identity;

public interface IAppRoleRepository
{
    Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> GetAllAsync();
    Task<DatabaseActionResult<int>> GetCountAsync();
    Task<DatabaseActionResult<AppRoleDb>> GetByIdAsync(Guid id);
    Task<DatabaseActionResult<AppRoleDb>> GetByNameAsync(string roleName);
    Task<DatabaseActionResult<Guid>> CreateAsync(CreateRoleRequest request);
    Task<DatabaseActionResult> UpdateAsync(UpdateRoleRequest request);
    Task<DatabaseActionResult> DeleteAsync(Guid id);
    Task<DatabaseActionResult<bool>> IsUserInRoleAsync(Guid userId, Guid roleId);
    Task<DatabaseActionResult> AddUserToRoleAsync(Guid userId, Guid roleId);
    Task<DatabaseActionResult> RemoveUserFromRoleAsync(Guid userId, Guid roleId);
    Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> GetRolesForUser(Guid userId);
}
