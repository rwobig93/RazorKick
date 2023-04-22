using Application.Constants.Identity;
using Application.Models.Identity;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Application.Services.System;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.Identity;

public class AppIdentityRoleService : IAppIdentityRoleService
{
    private readonly IAppRoleRepository _roleRepository;
    private readonly IAppUserRepository _userRepository;
    private readonly IRunningServerState _serverState;
    private readonly IDateTimeService _dateTime;

    public AppIdentityRoleService(IAppRoleRepository roleRepository, IAppUserRepository userRepository, IRunningServerState serverState,
        IDateTimeService dateTime)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _serverState = serverState;
        _dateTime = dateTime;
    }

    public void Dispose()
    {
        // Using dapper, nothing to dispose
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
        role.LastModifiedBy = _serverState.SystemUserId;
        role.LastModifiedOn = _dateTime.NowDatabaseTime;
        
        var updateRequest = await _roleRepository.UpdateAsync(role.ToObject());
        return !updateRequest.Success ? 
            IdentityResult.Failed(new IdentityError() {Code = "RoleUpdateFail", Description = updateRequest.ErrorMessage}) : 
            IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(AppRoleDb role, CancellationToken cancellationToken)
    {
        role.LastModifiedBy = _serverState.SystemUserId;
        role.LastModifiedOn = _dateTime.NowDatabaseTime;

        var systemUser = await _userRepository.GetByUsernameAsync(UserConstants.DefaultUsers.SystemUsername);
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
        var updateObject = new AppRoleUpdate
        {
            Name = role.Name,
            LastModifiedBy = _serverState.SystemUserId,
            LastModifiedOn = _dateTime.NowDatabaseTime
        };

        await _roleRepository.UpdateAsync(updateObject);
    }

    public async Task<string> GetNormalizedRoleNameAsync(AppRoleDb role, CancellationToken cancellationToken)
    {
        return await Task.FromResult(role.NormalizedName);
    }

    public async Task SetNormalizedRoleNameAsync(AppRoleDb role, string normalizedName, CancellationToken cancellationToken)
    {
        var updateObject = new AppRoleUpdate
        {
            NormalizedName = role.NormalizedName,
            LastModifiedBy = _serverState.SystemUserId,
            LastModifiedOn = _dateTime.NowDatabaseTime
        };

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