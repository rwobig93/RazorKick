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
    Task<DatabaseActionResult> DeleteAsync(string id);
    Task<DatabaseActionResult> GetAllPermissionsAsync(string roleId);
    Task<DatabaseActionResult> AddPermissionsAsync(PermissionsRequest request);
    Task<DatabaseActionResult> RemovePermissionsAsync(PermissionsRequest request);
    Task<DatabaseActionResult> EnforcePermissionsAsync(PermissionsRequest request);
}
