using Application.Database.MsSql.Identity;
using Application.Database.MsSql.Shared;
using Application.Models.Identity;
using Application.Repositories.Identity;
using Application.Services.Database;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Database;

namespace Infrastructure.Repositories.Identity;

public class AppRoleRepository : IAppRoleRepository
{
    private readonly ISqlDataService _database;
    private readonly ILogger _logger;

    public AppRoleRepository(ISqlDataService database, ILogger logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> GetAllAsync()
    {
        DatabaseActionResult<IEnumerable<AppRoleDb>> actionReturn = new();

        try
        {
            var allRoles = await _database.LoadData<AppRoleDb, dynamic>(AppRoles.GetAll, new { });
            actionReturn.Succeed(allRoles);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRoles.GetAll.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<int>> GetCountAsync()
    {
        DatabaseActionResult<int> actionReturn = new();

        try
        {
            var rowCount = (await _database.LoadData<int, dynamic>(
                General.GetRowCount, new {TableName = AppRoles.Table.TableName})).FirstOrDefault();
            actionReturn.Succeed(rowCount);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, General.GetRowCount.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppRoleDb>> GetByIdAsync(Guid id)
    {
        DatabaseActionResult<AppRoleDb> actionReturn = new();

        try
        {
            var foundRole = (await _database.LoadData<AppRoleDb, dynamic>(AppRoles.GetById, new {Id = id})).FirstOrDefault();
            actionReturn.Succeed(foundRole!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRoles.GetById.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppRoleDb>> GetByNameAsync(string roleName)
    {
        DatabaseActionResult<AppRoleDb> actionReturn = new();

        try
        {
            var foundRole = (await _database.LoadData<AppRoleDb, dynamic>(
                AppRoles.GetByName, new {Name = roleName})).FirstOrDefault();
            actionReturn.Succeed(foundRole!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRoles.GetByName.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppRoleDb>> GetByNormalizedNameAsync(string normalizedRoleName)
    {
        DatabaseActionResult<AppRoleDb> actionReturn = new();

        try
        {
            var foundRole = (await _database.LoadData<AppRoleDb, dynamic>(
                AppRoles.GetByNormalizedName, new {NormalizedName = normalizedRoleName})).FirstOrDefault();
            actionReturn.Succeed(foundRole!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRoles.GetByNormalizedName.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid>> CreateAsync(AppRoleCreate createObject)
    {
        DatabaseActionResult<Guid> actionReturn = new();

        try
        {
            var createdId = await _database.SaveDataReturnId(AppRoles.Insert, createObject);
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRoles.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(AppRoleUpdate updateObject)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppRoles.Update, updateObject);
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRoles.Update.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid id)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppRoles.Delete, new {Id = id});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRoles.Delete.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> SetCreatedById(Guid roleId, Guid createdById)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppRoles.SetCreatedById, new { Id = roleId, CreatedBy = createdById });
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRoles.SetCreatedById.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<bool>> IsUserInRoleAsync(Guid userId, Guid roleId)
    {
        DatabaseActionResult<bool> actionReturn = new();

        try
        {
            var userRoleJunction = (await _database.LoadData<AppUserRoleJunctionDb, dynamic>(
                AppUserRoleJunctions.GetByUserRoleId, new {UserId = userId, RoleId = roleId})).FirstOrDefault();
            var hasRole = userRoleJunction is not null;
            actionReturn.Succeed(hasRole);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserRoleJunctions.GetByUserRoleId.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> AddUserToRoleAsync(Guid userId, Guid roleId)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppUserRoleJunctions.Insert, new {UserId = userId, RoleId = roleId});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserRoleJunctions.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> RemoveUserFromRoleAsync(Guid userId, Guid roleId)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppUserRoleJunctions.Delete, new {UserId = userId, RoleId = roleId});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserRoleJunctions.Delete.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> GetRolesForUser(Guid userId)
    {
        DatabaseActionResult<IEnumerable<AppRoleDb>> actionReturn = new();

        try
        {
            var roleIds = await _database.LoadData<Guid, dynamic>(
                AppUserRoleJunctions.GetRolesOfUser, new {UserId = userId});

            var allRoles = (await GetAllAsync()).Result ?? new List<AppRoleDb>();
            var matchingRoles = allRoles.Where(x => roleIds.Any(r => r == x.Id));

            actionReturn.Succeed(matchingRoles);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, "GetRolesForUser", ex.Message);
        }

        return actionReturn;
    }
}