using Application.Database.MsSql.Identity;
using Application.Database.MsSql.Shared;
using Application.Models.Identity;
using Application.Repositories.Identity;
using Application.Services.Database;
using Application.Services.Identity;
using Application.Services.System;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Database;
using Microsoft.Extensions.DependencyInjection;
using Shared.Requests.Identity.Role;

namespace Infrastructure.Repositories.Identity;

public class AppRoleRepository : IAppRoleRepository
{
    private readonly ISqlDataService _database;
    private readonly ILogger _logger;
    private readonly IDateTimeService _dateTime;
    private readonly IRunningServerState _serverState;
    private readonly IServiceScopeFactory _scopeFactory;

    public AppRoleRepository(ISqlDataService database, ILogger logger, IDateTimeService dateTime, IRunningServerState serverState,
        IServiceScopeFactory scopeFactory)
    {
        _database = database;
        _logger = logger;
        _dateTime = dateTime;
        _serverState = serverState;
        _scopeFactory = scopeFactory;
    }

    private async Task UpdateAuditing(AppRoleCreate createRole, bool systemUpdate = false)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
            var currentUserId = systemUpdate ? _serverState.SystemUserId : await currentUserService.GetCurrentUserId();
            createRole.CreatedBy = currentUserId ?? createRole.CreatedBy;
            createRole.CreatedOn = _dateTime.NowDatabaseTime;
        }
        catch (Exception ex)
        {
            _logger.Error("Failure occurred attempting to create auditing object: [{TableName}][{ObjectName}] :: {ErrorMessage}", 
                AppRoles.Table.TableName, createRole.Name, ex.Message);
        }
    }

    private async Task UpdateAuditing(AppRoleUpdate updateRole, bool systemUpdate = false)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
            var currentUserId = systemUpdate ? _serverState.SystemUserId : await currentUserService.GetCurrentUserId();
            // TODO: Add auditing trail for modified properties, don't update last modified on/by if there are no changes
            updateRole.LastModifiedBy = currentUserId ?? updateRole.LastModifiedBy;
            updateRole.LastModifiedOn = _dateTime.NowDatabaseTime;
        }
        catch (Exception ex)
        {
            _logger.Error("Failure occurred attempting to update auditing object: [{TableName}][{ObjectId}] :: {ErrorMessage}", 
                AppRoles.Table.TableName, updateRole.Id, ex.Message);
        }
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
                General.GetRowCount, new {AppRoles.Table.TableName})).FirstOrDefault();
            actionReturn.Succeed(rowCount);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, General.GetRowCount.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppRoleDb>> GetByIdAsync(Guid roleId)
    {
        DatabaseActionResult<AppRoleDb> actionReturn = new();

        try
        {
            var foundRole = (await _database.LoadData<AppRoleDb, dynamic>(AppRoles.GetById, new {Id = roleId})).FirstOrDefault();
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

    public async Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> SearchAsync(string searchText)
    {
        DatabaseActionResult<IEnumerable<AppRoleDb>> actionReturn = new();

        try
        {
            var searchResults =
                await _database.LoadData<AppRoleDb, dynamic>(AppRoles.Search, new { SearchTerm = searchText });
            actionReturn.Succeed(searchResults);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRoles.Search.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid>> CreateAsync(AppRoleCreate createObject, bool systemUpdate = false)
    {
        DatabaseActionResult<Guid> actionReturn = new();

        try
        {
            await UpdateAuditing(createObject, systemUpdate);
            var createdId = await _database.SaveDataReturnId(AppRoles.Insert, createObject);
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRoles.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(AppRoleUpdate updateObject, bool systemUpdate = false)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await UpdateAuditing(updateObject, systemUpdate);
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
            await _database.SaveData(AppRoles.Delete, new {Id = id, DeletedOn = _dateTime.NowDatabaseTime});
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

    public async Task<DatabaseActionResult<bool>> IsUserInRoleAsync(Guid userId, string roleName)
    {
        DatabaseActionResult<bool> actionReturn = new();

        try
        {
            var foundRole = (await _database.LoadData<AppRoleDb, dynamic>(
                AppRoles.GetByName, new {Name = roleName})).FirstOrDefault();
            var userRoleJunction = (await _database.LoadData<AppUserRoleJunctionDb, dynamic>(
                AppUserRoleJunctions.GetByUserRoleId, new {UserId = userId, RoleId = foundRole!.Name})).FirstOrDefault();
            var hasRole = userRoleJunction is not null;
            actionReturn.Succeed(hasRole);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, "IsUserInRoleAsync_RoleName", ex.Message);
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

    public async Task<DatabaseActionResult<IEnumerable<AppUserDb>>> GetUsersForRole(Guid roleId)
    {
        DatabaseActionResult<IEnumerable<AppUserDb>> actionReturn = new();

        try
        {
            var userIds = await _database.LoadData<Guid, dynamic>(
                AppUserRoleJunctions.GetUsersOfRole, new {RoleId = roleId});

            var allUsers = await _database.LoadData<AppUserDb, dynamic>(AppUsers.GetAll, new { });
            var matchingUsers = allUsers.Where(x => userIds.Any(u => u == x.Id));

            actionReturn.Succeed(matchingUsers);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, "GetUsersForRole", ex.Message);
        }

        return actionReturn;
    }
}