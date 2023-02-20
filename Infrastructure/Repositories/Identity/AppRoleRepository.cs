using Application.Repositories.Identity;
using Application.Services.Identity;
using AutoMapper;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories.Identity;

public class AppRoleRepository : IRoleStore<AppRoleDb>
{
    private readonly RoleManager<AppRoleDb> _roleManager;
    private readonly UserManager<AppUserDb> _userManager;
    private readonly IAppPermissionsRepository _claimService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public AppRoleRepository(
        RoleManager<AppRoleDb> roleManager,
        IMapper mapper,
        UserManager<AppUserDb> userManager,
        IAppPermissionsRepository claimService,
        ICurrentUserService currentUserService)
    {
        _roleManager = roleManager;
        _mapper = mapper;
        _userManager = userManager;
        _claimService = claimService;
        _currentUserService = currentUserService;
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

    public async Task<AppRoleDb?> GetByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<AppRoleDb?> GetByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        // Using Dapper, nothing to dispose
    }
}