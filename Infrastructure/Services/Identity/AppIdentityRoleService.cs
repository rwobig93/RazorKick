using Application.Models.Identity;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.Identity;

public class AppIdentityRoleService : IAppIdentityRoleService
{
    private readonly IAppRoleRepository _roleRepository;

    public AppIdentityRoleService(IAppRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityResult> CreateAsync(AppRoleDb role, CancellationToken cancellationToken)
    {
        var creationRequest = await _roleRepository.CreateAsync(role.ToCreateObject());
        return !creationRequest.Success ? 
            IdentityResult.Failed(new IdentityError() {Code = "RoleCreateFail", Description = creationRequest.ErrorMessage}) : 
            IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(AppRoleDb role, CancellationToken cancellationToken)
    {
        var updateRequest = await _roleRepository.UpdateAsync(role.ToUpdateObject());
        return !updateRequest.Success ? 
            IdentityResult.Failed(new IdentityError() {Code = "RoleUpdateFail", Description = updateRequest.ErrorMessage}) : 
            IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(AppRoleDb role, CancellationToken cancellationToken)
    {
        var deleteRequest = await _roleRepository.DeleteAsync(role.Id);
        return !deleteRequest.Success ? 
            IdentityResult.Failed(new IdentityError() {Code = "RoleDeleteFail", Description = deleteRequest.ErrorMessage}) : 
            IdentityResult.Success;
    }

    public async Task<string> GetRoleIdAsync(AppRoleDb role, CancellationToken cancellationToken)
    {
        return await Task.FromResult(role.Id.ToString());
    }

    public async Task<string> GetRoleNameAsync(AppRoleDb role, CancellationToken cancellationToken)
    {
        return await Task.FromResult(role.Name);
    }

    public async Task SetRoleNameAsync(AppRoleDb role, string roleName, CancellationToken cancellationToken)
    {
        var updateObject = new AppRoleUpdate() {Name = role.Name};
        await _roleRepository.UpdateAsync(updateObject);
    }

    public async Task<string> GetNormalizedRoleNameAsync(AppRoleDb role, CancellationToken cancellationToken)
    {
        return await Task.FromResult(role.NormalizedName);
    }

    public async Task SetNormalizedRoleNameAsync(AppRoleDb role, string normalizedName, CancellationToken cancellationToken)
    {
        var updateObject = new AppRoleUpdate() {NormalizedName = role.NormalizedName};
        await _roleRepository.UpdateAsync(updateObject);
    }

    public async Task<AppRoleDb> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        return (await _roleRepository.GetByIdAsync(Guid.Parse(roleId))).Result!;
    }

    public async Task<AppRoleDb> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        return (await _roleRepository.GetByNormalizedNameAsync(normalizedRoleName)).Result!;
    }
}