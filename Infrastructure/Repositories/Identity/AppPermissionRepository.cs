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
    private readonly ILogger _logger;

    public AppPermissionRepository(ISqlDataService database, ILogger logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllAsync()
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var allPermissions = await _database.LoadData<AppPermissionDb, dynamic>(AppPermissions.GetAll, new { });
            actionReturn.Succeed(allPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissions.GetAll.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> SearchAsync(string searchTerm)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var searchResults = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.Search, new {SearchTerm = searchTerm});
            actionReturn.Succeed(searchResults);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissions.Search.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<int>> GetCountAsync()
    {
        DatabaseActionResult<int> actionReturn = new();

        try
        {
            var rowCount = (await _database.LoadData<int, dynamic>(
                General.GetRowCount, new {AppPermissions.Table.TableName})).FirstOrDefault();
            actionReturn.Succeed(rowCount);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, General.GetRowCount.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppPermissionDb>> GetByIdAsync(Guid id)
    {
        DatabaseActionResult<AppPermissionDb> actionReturn = new();

        try
        {
            var foundPermission = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetById, new {Id = id})).FirstOrDefault();
            actionReturn.Succeed(foundPermission!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissions.GetById.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppPermissionDb>> GetByUserIdAndValueAsync(Guid userId, string claimValue)
    {
        DatabaseActionResult<AppPermissionDb> actionReturn = new();

        try
        {
            var foundPermission = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByUserIdAndValue, new {UserId = userId, ClaimValue = claimValue})).FirstOrDefault();
            actionReturn.Succeed(foundPermission!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissions.GetByUserIdAndValue.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppPermissionDb>> GetByRoleIdAndValueAsync(Guid roleId, string claimValue)
    {
        DatabaseActionResult<AppPermissionDb> actionReturn = new();

        try
        {
            var foundPermission = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByRoleIdAndValue, new {RoleId = roleId, ClaimValue = claimValue})).FirstOrDefault();
            actionReturn.Succeed(foundPermission!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissions.GetByRoleIdAndValue.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllByNameAsync(string roleName)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var foundPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByName, new {Name = roleName});
            actionReturn.Succeed(foundPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissions.GetByName.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllByGroupAsync(string groupName)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var foundPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByGroup, new {Group = groupName});
            actionReturn.Succeed(foundPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissions.GetByGroup.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllByAccessAsync(string accessName)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var foundPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByAccess, new {Access = accessName});
            actionReturn.Succeed(foundPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissions.GetByAccess.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllForRoleAsync(Guid roleId)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var foundPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByRoleId, new {RoleId = roleId});
            actionReturn.Succeed(foundPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissions.GetByRoleId.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllDirectForUserAsync(Guid userId)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var foundPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByUserId, new {UserId = userId});
            actionReturn.Succeed(foundPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissions.GetByUserId.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllIncludingRolesForUserAsync(Guid userId)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            List<AppPermissionDb> allPermissions = new();

            var userPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByUserId, new {UserId = userId});
            allPermissions.AddRange(userPermissions);

            var roleIds = await _database.LoadData<Guid, dynamic>(
                AppUserRoleJunctions.GetRolesOfUser, new {UserId = userId});
            foreach (var id in roleIds)
            {
                var rolePermissions = await GetAllForRoleAsync(id);
                if (rolePermissions.Success)
                    allPermissions.AddRange(rolePermissions.Result!);
            }

            actionReturn.Succeed(allPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, "GetAllIncludingRolesForUserAsync", ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid>> CreateAsync(AppPermissionCreate createObject)
    {
        DatabaseActionResult<Guid> actionReturn = new();

        try
        {
            if (createObject.UserId == Guid.Empty && createObject.RoleId == Guid.Empty)
                throw new Exception("UserId & RoleId cannot be empty, please provide a valid Id");

            var createdId = await _database.SaveDataReturnId(AppPermissions.Insert, createObject);
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissions.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(AppPermissionUpdate updateObject)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppPermissions.Update, updateObject);
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissions.Update.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid id)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppPermissions.Delete, new {Id = id});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissions.Delete.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<bool>> UserHasDirectPermission(Guid userId, string permissionValue)
    {
        DatabaseActionResult<bool> actionReturn = new();

        try
        {
            var foundPermission = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByUserIdAndValue, new {UserId = userId, ClaimValue = permissionValue})).FirstOrDefault();
            var hasPermission = foundPermission is not null;
            actionReturn.Succeed(hasPermission);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissions.GetByUserIdAndValue.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<bool>> UserIncludingRolesHasPermission(Guid userId, string permissionValue)
    {
        DatabaseActionResult<bool> actionReturn = new();

        try
        {
            var allGlobalUserPermissions = await GetAllIncludingRolesForUserAsync(userId);
            var hasPermission = allGlobalUserPermissions.Result!.Any(x => x.ClaimValue == permissionValue);
            actionReturn.Succeed(hasPermission);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, "UserIncludingRolesHasPermission", ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<bool>> RoleHasPermission(Guid roleId, string permissionValue)
    {
        DatabaseActionResult<bool> actionReturn = new();

        try
        {
            var foundPermission = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissions.GetByRoleIdAndValue, new {RoleId = roleId, ClaimValue = permissionValue})).FirstOrDefault();
            var hasPermission = foundPermission is not null;
            actionReturn.Succeed(hasPermission);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissions.GetByRoleIdAndValue.Path, ex.Message);
        }

        return actionReturn;
    }
}