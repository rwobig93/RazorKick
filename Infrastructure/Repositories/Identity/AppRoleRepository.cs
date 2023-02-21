using Application.Repositories.Identity;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Database;
using Shared.Requests.Identity;

namespace Infrastructure.Repositories.Identity;

public class AppRoleRepository : IAppRoleRepository
{
    public async Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<DatabaseActionResult<int>> GetCountAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<DatabaseActionResult<AppRoleDb>> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<DatabaseActionResult<AppRoleDb>> GetByNameAsync(string roleName)
    {
        throw new NotImplementedException();
    }

    public async Task<DatabaseActionResult<Guid>> CreateAsync(CreateRoleRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<DatabaseActionResult> UpdateAsync(UpdateRoleRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<DatabaseActionResult> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<DatabaseActionResult> GetAllPermissionsAsync(string roleId)
    {
        throw new NotImplementedException();
    }

    public async Task<DatabaseActionResult> AddPermissionsAsync(PermissionsRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<DatabaseActionResult> RemovePermissionsAsync(PermissionsRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<DatabaseActionResult> EnforcePermissionsAsync(PermissionsRequest request)
    {
        throw new NotImplementedException();
    }
}