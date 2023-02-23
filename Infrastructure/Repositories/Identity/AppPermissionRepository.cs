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
                General.GetRowCount, new {TableName = AppPermissions.Table.TableName})).FirstOrDefault();
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

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllForUserAsync(Guid userId)
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

    public async Task<DatabaseActionResult<Guid>> CreateAsync(AppPermissionCreate createObject)
    {
        
        DatabaseActionResult<Guid> actionReturn = new ();
        
        try
        {
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
}