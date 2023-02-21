using Application.Services.Identity;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.Identity;

public class AppIdentityRoleService : IAppIdentityRoleService
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityResult> CreateAsync(AppRoleDb role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityResult> UpdateAsync(AppRoleDb role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityResult> DeleteAsync(AppRoleDb role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetRoleIdAsync(AppRoleDb role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetRoleNameAsync(AppRoleDb role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task SetRoleNameAsync(AppRoleDb role, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetNormalizedRoleNameAsync(AppRoleDb role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task SetNormalizedRoleNameAsync(AppRoleDb role, string normalizedName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<AppRoleDb> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<AppRoleDb> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}