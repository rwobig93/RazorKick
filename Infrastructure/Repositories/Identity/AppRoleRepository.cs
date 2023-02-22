using Application.Database.MsSql.Identity;
using Application.Database.MsSql.Shared;
using Application.Repositories.Identity;
using Application.Services.Database;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Database;
using Shared.Requests.Identity;

namespace Infrastructure.Repositories.Identity;

public class AppRoleRepository : IAppRoleRepository
{
    private readonly ISqlDataService _database;

    public AppRoleRepository(ISqlDataService database)
    {
        _database = database;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> GetAllAsync()
    {
        DatabaseActionResult<IEnumerable<AppRoleDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.LoadData<AppRoleDb, dynamic>(AppRoles.GetAll, new { });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
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
                General.GetRowCount, new {TableName = AppRoles.Table.TableName})).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppRoleDb>> GetByIdAsync(Guid id)
    {
        DatabaseActionResult<AppRoleDb> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<AppRoleDb, dynamic>(AppRoles.GetById, new {Id = id})).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppRoleDb>> GetByNameAsync(string roleName)
    {
        DatabaseActionResult<AppRoleDb> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<AppRoleDb, dynamic>(
                AppRoles.GetByName, new {Name = roleName})).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid>> CreateAsync(CreateRoleRequest request)
    {
        DatabaseActionResult<Guid> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.SaveDataReturnId(AppRoles.Insert, request);
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(UpdateRoleRequest request)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(AppRoles.Update, request);
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid id)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(AppRoles.Delete, new {Id = id});
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<bool>> IsUserInRoleAsync(Guid userId, Guid roleId)
    {
        DatabaseActionResult<bool> actionReturn = new ();
        
        try
        {
            var userRoleJunction = (await _database.LoadData<AppUserRoleJunctionDb, dynamic>(
                AppUserRoleJunctions.GetByUserRoleId, new {UserId = userId, RoleId = roleId})).FirstOrDefault();
            actionReturn.Result = userRoleJunction is not null;
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> AddUserToRoleAsync(Guid userId, Guid roleId)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(AppUserRoleJunctions.Insert, new {UserId = userId, RoleId = roleId});
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> RemoveUserFromRoleAsync(Guid userId, Guid roleId)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(AppUserRoleJunctions.Delete, new {UserId = userId, RoleId = roleId});
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> GetRolesForUser(Guid userId)
    {
        DatabaseActionResult<IEnumerable<AppRoleDb>> actionReturn = new ();
        
        try
        {
            var roleIds = await _database.LoadData<Guid, dynamic>(
                AppUserRoleJunctions.GetRolesOfUser, new { UserId = userId });

            var allRoles = (await GetAllAsync()).Result ?? new List<AppRoleDb>();
            var matchingRoles = allRoles.Where(x => roleIds.Any(r => r == x.Id));

            actionReturn.Result = matchingRoles;
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }
}