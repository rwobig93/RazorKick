using Application.Interfaces.Identity;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Shared.Requests.Identity;
using Shared.Responses.Identity;

namespace Infrastructure.Services.Identity;

public class RoleService : IRoleService
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IClaimService _caimService;
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
        _caimService = claimService;
        _currentUserService = currentUserService;
    }

    public async Task<IResult<List<RoleResponse>>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<int>> GetCountAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<RoleResponse>> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<RoleResponse>> GetByNameAsync(string roleName)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<string>> CreateAsync(CreateRoleRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<string>> UpdateAsync(UpdateRoleRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<string>> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<PermissionsResponse>> GetAllPermissionsAsync(string roleId)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<string>> AddPermissionsAsync(PermissionsRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<string>> RemovePermissionsAsync(PermissionsRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<string>> EnforcePermissionsAsync(PermissionsRequest request)
    {
        throw new NotImplementedException();
    }
}