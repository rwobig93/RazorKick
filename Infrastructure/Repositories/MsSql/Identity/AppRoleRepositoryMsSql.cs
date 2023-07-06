using Application.Database.MsSql.Identity;
using Application.Database.MsSql.Shared;
using Application.Helpers.Lifecycle;
using Application.Mappers.Identity;
using Application.Models.Identity;
using Application.Models.Identity.Role;
using Application.Models.Lifecycle;
using Application.Repositories.Identity;
using Application.Repositories.Lifecycle;
using Application.Services.Database;
using Application.Services.Identity;
using Application.Services.System;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Database;
using Domain.Models.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repositories.MsSql.Identity;

public class AppRoleRepositoryMsSql : IAppRoleRepository
{
    private readonly ISqlDataService _database;
    private readonly ILogger _logger;
    private readonly IDateTimeService _dateTime;
    private readonly IRunningServerState _serverState;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IAuditTrailsRepository _auditRepository;
    private readonly ISerializerService _serializer;

    public AppRoleRepositoryMsSql(ISqlDataService database, ILogger logger, IDateTimeService dateTime, IRunningServerState serverState,
        IServiceScopeFactory scopeFactory, IAuditTrailsRepository auditRepository, ISerializerService serializer)
    {
        _database = database;
        _logger = logger;
        _dateTime = dateTime;
        _serverState = serverState;
        _scopeFactory = scopeFactory;
        _auditRepository = auditRepository;
        _serializer = serializer;
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
                AppRolesMsSql.Table.TableName, createRole.Name, ex.Message);
        }
    }

    private async Task UpdateAuditing(AppRoleUpdate updateRole, bool systemUpdate = false)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
            var currentUserId = systemUpdate ? _serverState.SystemUserId : await currentUserService.GetCurrentUserId();

            // Get current state for diff comparision
            var currentRoleState = await GetByIdAsync(updateRole.Id);
            var auditDiff = AuditHelpers.GetAuditDiff(currentRoleState.Result!.ToObject(), updateRole);
            
            updateRole.LastModifiedBy = currentUserId ?? updateRole.LastModifiedBy;
            updateRole.LastModifiedOn = _dateTime.NowDatabaseTime;

            // If no changes were detected for before and after we won't create an audit trail
            if (auditDiff.Before.Keys.Count > 0 && auditDiff.After.Count > 0)
                await _auditRepository.CreateAsync(new AuditTrailCreate
                {
                    TableName = AppRolesMsSql.Table.TableName,
                    RecordId = updateRole.Id,
                    ChangedBy = ((Guid)updateRole.LastModifiedBy!),
                    Action = DatabaseActionType.Update,
                    Before = _serializer.Serialize(auditDiff.Before),
                    After = _serializer.Serialize(auditDiff.After)
                });
        }
        catch (Exception ex)
        {
            _logger.Error("Failure occurred attempting to update auditing object: [{TableName}][{ObjectId}] :: {ErrorMessage}", 
                AppRolesMsSql.Table.TableName, updateRole.Id, ex.Message);
        }
    }

    public async Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> GetAllAsync()
    {
        DatabaseActionResult<IEnumerable<AppRoleDb>> actionReturn = new();

        try
        {
            var allRoles = await _database.LoadData<AppRoleDb, dynamic>(AppRolesMsSql.GetAll, new { });
            actionReturn.Succeed(allRoles);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRolesMsSql.GetAll.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<int>> GetCountAsync()
    {
        DatabaseActionResult<int> actionReturn = new();

        try
        {
            var rowCount = (await _database.LoadData<int, dynamic>(
                GeneralMsSql.GetRowCount, new {AppRolesMsSql.Table.TableName})).FirstOrDefault();
            actionReturn.Succeed(rowCount);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, GeneralMsSql.GetRowCount.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppRoleDb>> GetByIdAsync(Guid roleId)
    {
        DatabaseActionResult<AppRoleDb> actionReturn = new();

        try
        {
            var foundRole = (await _database.LoadData<AppRoleDb, dynamic>(AppRolesMsSql.GetById, new {Id = roleId})).FirstOrDefault();
            actionReturn.Succeed(foundRole!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRolesMsSql.GetById.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppRoleDb>> GetByNameAsync(string roleName)
    {
        DatabaseActionResult<AppRoleDb> actionReturn = new();

        try
        {
            var foundRole = (await _database.LoadData<AppRoleDb, dynamic>(
                AppRolesMsSql.GetByName, new {Name = roleName})).FirstOrDefault();
            actionReturn.Succeed(foundRole!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRolesMsSql.GetByName.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppRoleDb>> GetByNormalizedNameAsync(string normalizedRoleName)
    {
        DatabaseActionResult<AppRoleDb> actionReturn = new();

        try
        {
            var foundRole = (await _database.LoadData<AppRoleDb, dynamic>(
                AppRolesMsSql.GetByNormalizedName, new {NormalizedName = normalizedRoleName})).FirstOrDefault();
            actionReturn.Succeed(foundRole!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRolesMsSql.GetByNormalizedName.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> SearchAsync(string searchText)
    {
        DatabaseActionResult<IEnumerable<AppRoleDb>> actionReturn = new();

        try
        {
            var searchResults =
                await _database.LoadData<AppRoleDb, dynamic>(AppRolesMsSql.Search, new { SearchTerm = searchText });
            actionReturn.Succeed(searchResults);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRolesMsSql.Search.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid>> CreateAsync(AppRoleCreate createObject, bool systemUpdate = false)
    {
        DatabaseActionResult<Guid> actionReturn = new();

        try
        {
            await UpdateAuditing(createObject, systemUpdate);
            var createdId = await _database.SaveDataReturnId(AppRolesMsSql.Insert, createObject);

            await _auditRepository.CreateAsync(new AuditTrailCreate
            {
                TableName = AppRolesMsSql.Table.TableName,
                RecordId = createdId,
                ChangedBy = (createObject.CreatedBy),
                Action = DatabaseActionType.Create
            });
            
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRolesMsSql.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(AppRoleUpdate updateObject, bool systemUpdate = false)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await UpdateAuditing(updateObject, systemUpdate);
            await _database.SaveData(AppRolesMsSql.Update, updateObject);
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRolesMsSql.Update.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid id)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            // Update role w/ a property that is modified so we get the last updated on/by for the deleting user
            var roleUpdate = new AppRoleUpdate() {Id = id};
            await UpdateAsync(roleUpdate);
            await _database.SaveData(AppRolesMsSql.Delete, new {Id = id, DeletedOn = _dateTime.NowDatabaseTime});

            await _auditRepository.CreateAsync(new AuditTrailCreate
            {
                TableName = AppRolesMsSql.Table.TableName,
                RecordId = id,
                ChangedBy = ((Guid)roleUpdate.LastModifiedBy!),
                Action = DatabaseActionType.Delete
            });
            
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRolesMsSql.Delete.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> SetCreatedById(Guid roleId, Guid createdById)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppRolesMsSql.SetCreatedById, new { Id = roleId, CreatedBy = createdById });
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRolesMsSql.SetCreatedById.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<bool>> IsUserInRoleAsync(Guid userId, Guid roleId)
    {
        DatabaseActionResult<bool> actionReturn = new();

        try
        {
            var userRoleJunction = (await _database.LoadData<AppUserRoleJunctionDb, dynamic>(
                AppUserRoleJunctionsMsSql.GetByUserRoleId, new {UserId = userId, RoleId = roleId})).FirstOrDefault();
            var hasRole = userRoleJunction is not null;
            actionReturn.Succeed(hasRole);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserRoleJunctionsMsSql.GetByUserRoleId.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<bool>> IsUserInRoleAsync(Guid userId, string roleName)
    {
        DatabaseActionResult<bool> actionReturn = new();

        try
        {
            var foundRole = (await _database.LoadData<AppRoleDb, dynamic>(
                AppRolesMsSql.GetByName, new {Name = roleName})).FirstOrDefault();
            var userRoleJunction = (await _database.LoadData<AppUserRoleJunctionDb, dynamic>(
                AppUserRoleJunctionsMsSql.GetByUserRoleId, new {UserId = userId, RoleId = foundRole!.Name})).FirstOrDefault();
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
            await _database.SaveData(AppUserRoleJunctionsMsSql.Insert, new {UserId = userId, RoleId = roleId});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserRoleJunctionsMsSql.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> RemoveUserFromRoleAsync(Guid userId, Guid roleId)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppUserRoleJunctionsMsSql.Delete, new {UserId = userId, RoleId = roleId});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserRoleJunctionsMsSql.Delete.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> GetRolesForUser(Guid userId)
    {
        DatabaseActionResult<IEnumerable<AppRoleDb>> actionReturn = new();

        try
        {
            var roleIds = await _database.LoadData<Guid, dynamic>(
                AppUserRoleJunctionsMsSql.GetRolesOfUser, new {UserId = userId});

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
                AppUserRoleJunctionsMsSql.GetUsersOfRole, new {RoleId = roleId});

            var allUsers = await _database.LoadData<AppUserDb, dynamic>(AppUsersMsSql.GetAll, new { });
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