using Application.Constants.Identity;
using Application.Mappers.Identity;
using Application.Models.Identity.Permission;
using Application.Models.Identity.Role;
using Application.Models.Identity.User;
using Application.Models.Web;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Domain.DatabaseEntities.Identity;

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

    private async Task<IResult<AppRoleFull?>> ConvertToFullAsync(AppRoleDb roleDb)
    {
        var fullRole = roleDb.ToFull();

        var foundUsers = await _roleRepository.GetUsersForRole(roleDb.Id);
        if (!foundUsers.Success)
            return await Result<AppRoleFull?>.FailAsync(foundUsers.ErrorMessage);
            
        fullRole.Users = (foundUsers.Result?.ToSlims() ?? new List<AppUserSlim>())
            .OrderBy(x => x.Username)
            .ToList();

        var foundPermissions = await _permissionRepository.GetAllForRoleAsync(roleDb.Id);
        if (!foundPermissions.Success)
            return await Result<AppRoleFull?>.FailAsync(foundPermissions.ErrorMessage);
            
        fullRole.Permissions = (foundPermissions.Result?.ToSlims() ?? new List<AppPermissionSlim>())
            .OrderBy(x => x.Group)
            .ThenBy(x => x.Name)
            .ThenBy(x => x.Access)
            .ToList();

        return await Result<AppRoleFull?>.SuccessAsync(fullRole);
    }

    public async Task<IResult<IEnumerable<AppRoleSlim>>> GetAllAsync()
    {
        try
        {
            var roles = await _roleRepository.GetAllAsync();
            if (!roles.Success)
                return await Result<IEnumerable<AppRoleSlim>>.FailAsync(roles.ErrorMessage);

            return await Result<IEnumerable<AppRoleSlim>>.SuccessAsync(roles.Result?.ToSlims() ?? new List<AppRoleSlim>());
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppRoleSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppRoleSlim>>> GetAllPaginatedAsync(int pageNumber, int pageSize)
    {
        try
        {
            var roles = await _roleRepository.GetAllPaginatedAsync(pageNumber, pageSize);
            if (!roles.Success)
                return await Result<IEnumerable<AppRoleSlim>>.FailAsync(roles.ErrorMessage);

            return await Result<IEnumerable<AppRoleSlim>>.SuccessAsync(roles.Result?.ToSlims() ?? new List<AppRoleSlim>());
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
            var roleCount = await _roleRepository.GetCountAsync();
            if (!roleCount.Success)
                return await Result<int>.FailAsync(roleCount.ErrorMessage);

            return await Result<int>.SuccessAsync(roleCount.Result);
        }
        catch (Exception ex)
        {
            return await Result<int>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppRoleSlim?>> GetByIdAsync(Guid roleId)
    {
        try
        {
            var foundRole = await _roleRepository.GetByIdAsync(roleId);
            if (!foundRole.Success)
                return await Result<AppRoleSlim?>.FailAsync(foundRole.ErrorMessage);

            return await Result<AppRoleSlim?>.SuccessAsync(foundRole.Result?.ToSlim());
        }
        catch (Exception ex)
        {
            return await Result<AppRoleSlim?>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppRoleFull?>> GetByIdFullAsync(Guid roleId)
    {
        try
        {
            var foundRole = await _roleRepository.GetByIdAsync(roleId);
            if (!foundRole.Success)
                return await Result<AppRoleFull?>.FailAsync(foundRole.ErrorMessage);

            if (foundRole.Result is null)
                return await Result<AppRoleFull?>.FailAsync(foundRole.Result?.ToFull());

            return await ConvertToFullAsync(foundRole.Result);
        }
        catch (Exception ex)
        {
            return await Result<AppRoleFull?>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppRoleSlim?>> GetByNameAsync(string roleName)
    {
        try
        {
            var foundRole = await _roleRepository.GetByNameAsync(roleName);
            if (!foundRole.Success)
                return await Result<AppRoleSlim?>.FailAsync(foundRole.ErrorMessage);

            if (foundRole.Result is null)
                return await Result<AppRoleSlim?>.FailAsync(foundRole.Result?.ToSlim());

            return await Result<AppRoleSlim?>.SuccessAsync(foundRole.Result.ToSlim());
        }
        catch (Exception ex)
        {
            return await Result<AppRoleSlim?>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppRoleFull?>> GetByNameFullAsync(string roleName)
    {
        try
        {
            var foundRole = await _roleRepository.GetByNameAsync(roleName);
            if (!foundRole.Success)
                return await Result<AppRoleFull?>.FailAsync(foundRole.ErrorMessage);
            
            if (foundRole.Result is null)
                return await Result<AppRoleFull?>.FailAsync(foundRole.Result?.ToFull());

            return await ConvertToFullAsync(foundRole.Result);
        }
        catch (Exception ex)
        {
            return await Result<AppRoleFull?>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppRoleSlim>>> SearchAsync(string searchText)
    {
        try
        {
            var searchResult = await _roleRepository.SearchAsync(searchText);
            if (!searchResult.Success)
                return await Result<IEnumerable<AppRoleSlim>>.FailAsync(searchResult.ErrorMessage);

            var results = (searchResult.Result?.ToSlims() ?? new List<AppRoleSlim>())
                .OrderBy(x => x.Name);

            return await Result<IEnumerable<AppRoleSlim>>.SuccessAsync(results);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppRoleSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppRoleSlim>>> SearchPaginatedAsync(string searchText, int pageNumber, int pageSize)
    {
        try
        {
            var searchResult = await _roleRepository.SearchPaginatedAsync(searchText, pageNumber, pageSize);
            if (!searchResult.Success)
                return await Result<IEnumerable<AppRoleSlim>>.FailAsync(searchResult.ErrorMessage);

            var results = (searchResult.Result?.ToSlims() ?? new List<AppRoleSlim>())
                .OrderBy(x => x.Name);

            return await Result<IEnumerable<AppRoleSlim>>.SuccessAsync(results);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppRoleSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<Guid>> CreateAsync(AppRoleCreate createObject, bool systemUpdate = false)
    {
        try
        {
            var createRequest = await _roleRepository.CreateAsync(createObject);
            if (!createRequest.Success)
                return await Result<Guid>.FailAsync(createRequest.ErrorMessage);

            return await Result<Guid>.SuccessAsync(createRequest.Result);
        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> UpdateAsync(AppRoleUpdate updateObject, bool systemUpdate = false)
    {
        try
        {
            var roleToChange = await GetByIdAsync(updateObject.Id);
            if (RoleConstants.GetUnchangeableRoleNames().Contains(roleToChange.Data!.Name!) && roleToChange.Data!.Name! != updateObject.Name)
                return await Result.FailAsync("The role you are attempting to modify cannot have it's name changed");

            var updateRequest = await _roleRepository.UpdateAsync(updateObject);
            if (!updateRequest.Success)
                return await Result.FailAsync(updateRequest.ErrorMessage);

            return await Result.SuccessAsync();
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
            var deleteRequest = await _roleRepository.DeleteAsync(id, Guid.Empty);
            if (!deleteRequest.Success)
                return await Result.FailAsync(deleteRequest.ErrorMessage);

            return await Result.SuccessAsync();
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
            var roleCheck = await _roleRepository.IsUserInRoleAsync(userId, roleId);
            if (!roleCheck.Success)
                return await Result<bool>.FailAsync(roleCheck.ErrorMessage);

            return await Result<bool>.SuccessAsync(roleCheck.Result);
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
            var roleCheck = await _roleRepository.IsUserInRoleAsync(userId, roleName);
            if (!roleCheck.Success)
                return await Result<bool>.FailAsync(roleCheck.ErrorMessage);

            return await Result<bool>.SuccessAsync(roleCheck.Result);
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
            var userAdd = await _roleRepository.AddUserToRoleAsync(userId, roleId);
            if (!userAdd.Success)
                return await Result.FailAsync(userAdd.ErrorMessage);

            return await Result.SuccessAsync();
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
            var userRemove = await _roleRepository.RemoveUserFromRoleAsync(userId, roleId);
            if (!userRemove.Success)
                return await Result.FailAsync(userRemove.ErrorMessage);

            return await Result.SuccessAsync();
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
            var rolesRequest = await _roleRepository.GetRolesForUser(userId);
            if (!rolesRequest.Success)
                return await Result<IEnumerable<AppRoleSlim>>.FailAsync(rolesRequest.ErrorMessage);

            var roles = (rolesRequest.Result?.ToSlims() ?? new List<AppRoleSlim>())
                .OrderBy(x => x.Name);

            return await Result<IEnumerable<AppRoleSlim>>.SuccessAsync(roles);
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
            var rolesRequest = await _roleRepository.GetUsersForRole(roleId);
            if (!rolesRequest.Success)
                return await Result<IEnumerable<AppUserSlim>>.FailAsync(rolesRequest.ErrorMessage);

            var users = (rolesRequest.Result?.ToSlims() ?? new List<AppUserSlim>())
                .OrderBy(x => x.Username);

            return await Result<IEnumerable<AppUserSlim>>.SuccessAsync(users);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserSlim>>.FailAsync(ex.Message);
        }
    }
}