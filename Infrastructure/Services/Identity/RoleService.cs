using Application.Services.Identity;
using AutoMapper;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.Identity;

public class RoleService : IRoleStore<AppRole>
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IClaimService _claimService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public RoleService(
        RoleManager<AppRole> roleManager,
        IMapper mapper,
        UserManager<AppUser> userManager,
        IClaimService claimService,
        ICurrentUserService currentUserService)
    {
        _roleManager = roleManager;
        _mapper = mapper;
        _userManager = userManager;
        _claimService = claimService;
        _currentUserService = currentUserService;
    }

    public async Task<IdentityResult> CreateAsync(AppRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityResult> UpdateAsync(AppRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityResult> DeleteAsync(AppRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetRoleIdAsync(AppRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetRoleNameAsync(AppRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task SetRoleNameAsync(AppRole role, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetNormalizedRoleNameAsync(AppRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task SetNormalizedRoleNameAsync(AppRole role, string normalizedName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<AppRole?> GetByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<AppRole?> GetByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        // Using Dapper, nothing to dispose
    }
}