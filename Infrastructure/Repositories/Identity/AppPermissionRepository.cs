using Application.Database.MsSql.Identity;
using Application.Database.MsSql.Shared;
using Application.Models.Identity;
using Application.Repositories.Identity;
using Application.Services.Database;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Database;

namespace Infrastructure.Repositories.Identity;

public class AppPermissionRepository : IAppPermissionRepository
{
    private readonly ISqlDataService _database;

    public AppPermissionRepository(ISqlDataService database)
    {
        _database = database;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllAsync()
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.LoadData<AppPermissionDb, dynamic>(AppPermissions.GetAll, new { });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> SearchAsync(string searchTerm)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.Search, new { SearchTerm = searchTerm });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<int>> GetCountAsync()
    {
        DatabaseActionResult<int> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<int, dynamic>(
                General.GetRowCount, new {AppPermissions.Table.TableName})).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppPermissionDb>> GetByIdAsync(Guid id)
    {
        DatabaseActionResult<AppPermissionDb> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetById, new { Id = id })).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppPermissionDb>> GetByUserIdAndValueAsync(Guid userId, string claimValue)
    {
        DatabaseActionResult<AppPermissionDb> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByUserIdAndValue, new { UserId = userId, ClaimValue = claimValue })).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppPermissionDb>> GetByRoleIdAndValueAsync(Guid roleId, string claimValue)
    {
        DatabaseActionResult<AppPermissionDb> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByRoleIdAndValue, new { RoleId = roleId, ClaimValue = claimValue })).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllByNameAsync(string roleName)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByName, new { Name = roleName });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllByGroupAsync(string groupName)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByGroup, new { Group = groupName });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllByAccessAsync(string accessName)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByAccess, new { Access = accessName });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllForRoleAsync(Guid roleId)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByRoleId, new { RoleId = roleId });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllDirectForUserAsync(Guid userId)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByUserId, new { UserId = userId });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllIncludingRolesForUserAsync(Guid userId)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new ();
        
        try
        {
            List<AppPermissionDb> allPermissions = new();
            
            var userPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByUserId, new { UserId = userId });
            allPermissions.AddRange(userPermissions);
            
            var roleIds = await _database.LoadData<Guid, dynamic>(
                AppUserRoleJunctions.GetRolesOfUser, new { UserId = userId });
            foreach (var id in roleIds)
            {
                var rolePermissions = await GetAllForRoleAsync(id);
                if (rolePermissions.Success)
                    allPermissions.AddRange(rolePermissions.Result!);
            }

            actionReturn.Result = allPermissions;
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid>> CreateAsync(AppPermissionCreate createObject)
    {
        DatabaseActionResult<Guid> actionReturn = new ();
        
        try
        {
            if (createObject.UserId == Guid.Empty && createObject.RoleId == Guid.Empty)
                throw new Exception("UserId & RoleId cannot be empty, please provide a valid Id");
            
            actionReturn.Result = await _database.SaveDataReturnId(AppPermissions.Insert, createObject);
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(AppPermissionUpdate updateObject)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(AppPermissions.Update, updateObject);
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid id)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(AppPermissions.Delete, new { Id = id });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<bool>> UserHasDirectPermission(Guid userId, string permissionValue)
    {
        DatabaseActionResult<bool> actionReturn = new ();
        
        try
        {
            var foundPermission = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByUserIdAndValue, new {UserId = userId, ClaimValue = permissionValue})).FirstOrDefault();
            actionReturn.Result = foundPermission is not null;
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<bool>> UserIncludingRolesHasPermission(Guid userId, string permissionValue)
    {
        DatabaseActionResult<bool> actionReturn = new ();
        
        try
        {
            var allGlobalUserPermissions = await GetAllIncludingRolesForUserAsync(userId);
            actionReturn.Result = allGlobalUserPermissions.Result!.Any(x => x.ClaimValue == permissionValue);
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<bool>> RoleHasPermission(Guid roleId, string permissionValue)
    {
        DatabaseActionResult<bool> actionReturn = new ();
        
        try
        {
            var foundPermission = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByRoleIdAndValue, new {RoleId = roleId, ClaimValue = permissionValue})).FirstOrDefault();
            actionReturn.Result = foundPermission is not null;
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }
}