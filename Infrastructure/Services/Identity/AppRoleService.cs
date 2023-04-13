using Application.Models.Identity;
using Application.Models.Web;
using Application.Repositories.Identity;
using Application.Services.Identity;

namespace Infrastructure.Services.Identity;

public class AppRoleService : IAppRoleService
{
    private readonly IAppRoleRepository _roleRepository;
    private readonly IAppPermissionRepository _permissionRepository;

    public AppRoleService(IAppRoleRepository roleRepository, IAppPermissionRepository permissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<IResult<IEnumerable<AppRoleSlim>>> GetAllAsync()
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppRoleSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<int>> GetCountAsync()
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<int>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppRoleSlim>> GetByIdAsync(Guid roleId)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<AppRoleSlim>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppRoleFull>> GetByIdFullAsync(Guid roleId)
    {
        try
        {
            var foundRole = await _roleRepository.GetByIdAsync(roleId);
            if (!foundRole.Success)
                return await Result<AppRoleFull>.FailAsync(foundRole.ErrorMessage);
            
            var fullRole = foundRole.Result!.ToFull();

            var foundUsers = await _roleRepository.GetUsersForRole(roleId);
            if (!foundUsers.Success)
                return await Result<AppRoleFull>.FailAsync(foundUsers.ErrorMessage);
            
            fullRole.Users = (foundUsers.Result?.ToSlims() ?? new List<AppUserSlim>())
                .OrderBy(x => x.Username)
                .ToList();

            var foundPermissions = await _permissionRepository.GetAllForRoleAsync(roleId);
            if (!foundPermissions.Success)
                return await Result<AppRoleFull>.FailAsync(foundPermissions.ErrorMessage);
            
            fullRole.Permissions = (foundPermissions.Result?.ToSlims() ?? new List<AppPermissionSlim>())
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Access)
                .ToList();

            return await Result<AppRoleFull>.SuccessAsync(fullRole);
        }
        catch (Exception ex)
        {
            return await Result<AppRoleFull>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppRoleSlim>> GetByNameAsync(string roleName)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<AppRoleSlim>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppRoleFull>> GetByNameFullAsync(Guid roleId)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<AppRoleFull>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppRoleSlim>>> SearchAsync(string searchText)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppRoleSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<Guid>> CreateAsync(AppRoleCreate createObject)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> UpdateAsync(AppRoleUpdate updateObject)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> SetCreatedById(Guid roleId, Guid createdById)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<bool>> IsUserInRoleAsync(Guid userId, Guid roleId)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<bool>> IsUserInRoleAsync(Guid userId, string roleName)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> AddUserToRoleAsync(Guid userId, Guid roleId)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> RemoveUserFromRoleAsync(Guid userId, Guid roleId)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppRoleSlim>>> GetRolesForUser(Guid userId)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppRoleSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppUserSlim>>> GetUsersForRole(Guid roleId)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserSlim>>.FailAsync(ex.Message);
        }
    }
}