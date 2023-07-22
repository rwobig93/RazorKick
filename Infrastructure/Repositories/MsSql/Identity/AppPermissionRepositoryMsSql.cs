using Application.Database.MsSql.Identity;
using Application.Database.MsSql.Shared;
using Application.Helpers.Lifecycle;
using Application.Helpers.Runtime;
using Application.Mappers.Identity;
using Application.Models.Identity.Permission;
using Application.Models.Lifecycle;
using Application.Repositories.Identity;
using Application.Repositories.Lifecycle;
using Application.Services.Database;
using Application.Services.Identity;
using Application.Services.System;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Database;
using Domain.Models.Database;
using Domain.Models.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repositories.MsSql.Identity;

public class AppPermissionRepositoryMsSql : IAppPermissionRepository
{
    private readonly ISqlDataService _database;
    private readonly ILogger _logger;
    private readonly IDateTimeService _dateTime;
    private readonly IRunningServerState _serverState;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IAuditTrailsRepository _auditRepository;
    private readonly ISerializerService _serializer;

    public AppPermissionRepositoryMsSql(ISqlDataService database, ILogger logger, IDateTimeService dateTime, IRunningServerState serverState,
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

    private async Task UpdateAuditing(AppPermissionCreate createPermission, bool systemUpdate = false)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
            var currentUserId = systemUpdate ? _serverState.SystemUserId : await currentUserService.GetCurrentUserId();
            createPermission.CreatedBy = currentUserId ?? createPermission.CreatedBy;
            createPermission.CreatedOn = _dateTime.NowDatabaseTime;
        }
        catch (Exception ex)
        {
            _logger.Error("Failure occurred attempting to create auditing object: [{TableName}][{ObjectName}] :: {ErrorMessage}", 
                AppPermissionsMsSql.Table.TableName, createPermission.Name, ex.Message);
        }
    }

    private async Task UpdateAuditing(AppPermissionUpdate updatePermission, bool systemUpdate = false)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
            var currentUserId = systemUpdate ? _serverState.SystemUserId : await currentUserService.GetCurrentUserId();

            // Get current state for diff comparision
            var currentPermissionState = await GetByIdAsync(updatePermission.Id);
            var auditDiff = AuditHelpers.GetAuditDiff(currentPermissionState.Result!.ToUpdate(), updatePermission);
            
            updatePermission.LastModifiedBy = currentUserId ?? updatePermission.LastModifiedBy ?? Guid.Empty;
            updatePermission.LastModifiedOn = _dateTime.NowDatabaseTime;

            // If no changes were detected for before and after we won't create an audit trail
            if (auditDiff.Before.Keys.Count > 0 && auditDiff.After.Count > 0)
                await _auditRepository.CreateAsync(new AuditTrailCreate
                {
                    TableName = AppPermissionsMsSql.Table.TableName,
                    RecordId = updatePermission.Id,
                    ChangedBy = ((Guid)updatePermission.LastModifiedBy!),
                    Action = DatabaseActionType.Update,
                    Before = _serializer.Serialize(auditDiff.Before),
                    After = _serializer.Serialize(auditDiff.After)
                });
        }
        catch (Exception ex)
        {
            _logger.Error("Failure occurred attempting to update auditing object: [{TableName}][{ObjectId}] :: {ErrorMessage}", 
                AppPermissionsMsSql.Table.TableName, updatePermission.Id, ex.Message);
        }
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllAsync()
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var allPermissions = await _database.LoadData<AppPermissionDb, dynamic>(AppPermissionsMsSql.GetAll, new { });
            actionReturn.Succeed(allPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.GetAll.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllPaginatedAsync(int pageNumber, int pageSize)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var offset = MathHelpers.GetPaginatedOffset(pageNumber, pageSize);
            var allPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsMsSql.GetAllPaginated, new {Offset =  offset, PageSize = pageSize});
            actionReturn.Succeed(allPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.GetAllPaginated.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> SearchAsync(string searchTerm)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var searchResults = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsMsSql.Search, new {SearchTerm = searchTerm});
            actionReturn.Succeed(searchResults);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.Search.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> SearchPaginatedAsync(string searchTerm, int pageNumber, int pageSize)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var offset = MathHelpers.GetPaginatedOffset(pageNumber, pageSize);
            var searchResults = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsMsSql.SearchPaginated, new {SearchTerm = searchTerm, Offset =  offset, PageSize = pageSize});
            actionReturn.Succeed(searchResults);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.SearchPaginated.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<int>> GetCountAsync()
    {
        DatabaseActionResult<int> actionReturn = new();

        try
        {
            var rowCount = (await _database.LoadData<int, dynamic>(
                GeneralMsSql.GetRowCount, new {AppPermissionsMsSql.Table.TableName})).FirstOrDefault();
            actionReturn.Succeed(rowCount);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, GeneralMsSql.GetRowCount.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppPermissionDb>> GetByIdAsync(Guid id)
    {
        DatabaseActionResult<AppPermissionDb> actionReturn = new();

        try
        {
            var foundPermission = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsMsSql.GetById, new {Id = id})).FirstOrDefault();
            actionReturn.Succeed(foundPermission!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.GetById.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppPermissionDb>> GetByUserIdAndValueAsync(Guid userId, string claimValue)
    {
        DatabaseActionResult<AppPermissionDb> actionReturn = new();

        try
        {
            var foundPermission = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsMsSql.GetByUserIdAndValue, new {UserId = userId, ClaimValue = claimValue})).FirstOrDefault();
            actionReturn.Succeed(foundPermission!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.GetByUserIdAndValue.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppPermissionDb>> GetByRoleIdAndValueAsync(Guid roleId, string claimValue)
    {
        DatabaseActionResult<AppPermissionDb> actionReturn = new();

        try
        {
            var foundPermission = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsMsSql.GetByRoleIdAndValue, new {RoleId = roleId, ClaimValue = claimValue})).FirstOrDefault();
            actionReturn.Succeed(foundPermission!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.GetByRoleIdAndValue.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllByNameAsync(string roleName)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var foundPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsMsSql.GetByName, new {Name = roleName});
            actionReturn.Succeed(foundPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.GetByName.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllByGroupAsync(string groupName)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var foundPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsMsSql.GetByGroup, new {Group = groupName});
            actionReturn.Succeed(foundPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.GetByGroup.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllByAccessAsync(string accessName)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var foundPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsMsSql.GetByAccess, new {Access = accessName});
            actionReturn.Succeed(foundPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.GetByAccess.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllForRoleAsync(Guid roleId)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var foundPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsMsSql.GetByRoleId, new {RoleId = roleId});
            actionReturn.Succeed(foundPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.GetByRoleId.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllDirectForUserAsync(Guid userId)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var foundPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsMsSql.GetByUserId, new {UserId = userId});
            actionReturn.Succeed(foundPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.GetByUserId.Path, ex.Message);
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
                AppPermissionsMsSql.GetByUserId, new {UserId = userId});
            allPermissions.AddRange(userPermissions);

            var roleIds = await _database.LoadData<Guid, dynamic>(
                AppUserRoleJunctionsMsSql.GetRolesOfUser, new {UserId = userId});
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

    public async Task<DatabaseActionResult<Guid>> CreateAsync(AppPermissionCreate createObject, bool systemUpdate = false)
    {
        DatabaseActionResult<Guid> actionReturn = new();

        try
        {
            if (createObject.UserId == Guid.Empty && createObject.RoleId == Guid.Empty)
                throw new Exception("UserId & RoleId cannot be empty, please provide a valid Id");
            if (createObject.UserId == GuidHelpers.GetMax() && createObject.RoleId == GuidHelpers.GetMax())
                throw new Exception("UserId & RoleId cannot be empty, please provide a valid Id");
            if (createObject.UserId != GuidHelpers.GetMax() && createObject.RoleId != GuidHelpers.GetMax())
                throw new Exception("Each permission assignment request can only be made for a User or Role, not both at the same time");

            if (createObject.UserId != GuidHelpers.GetMax())
            {
                var foundUser = (await _database.LoadData<AppUserSecurityDb, dynamic>(
                    AppUsersMsSql.GetById, new {Id = createObject.UserId})).FirstOrDefault();
                if (foundUser is null) throw new Exception("UserId provided is invalid");
            }
            
            if (createObject.RoleId != GuidHelpers.GetMax())
            {
                var foundRole = (await _database.LoadData<AppRoleDb, dynamic>(
                    AppRolesMsSql.GetById, new {Id = createObject.RoleId})).FirstOrDefault();
                if (foundRole is null) throw new Exception("RoleId provided is invalid");
            }
            
            await UpdateAuditing(createObject, systemUpdate);
            var createdId = await _database.SaveDataReturnId(AppPermissionsMsSql.Insert, createObject);

            await _auditRepository.CreateAsync(new AuditTrailCreate
            {
                TableName = AppPermissionsMsSql.Table.TableName,
                RecordId = createdId,
                ChangedBy = (createObject.CreatedBy),
                Action = DatabaseActionType.Create,
                After = _serializer.Serialize(createObject)
            });
            
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(AppPermissionUpdate updateObject, bool systemUpdate = false)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await UpdateAuditing(updateObject, systemUpdate);
            await _database.SaveData(AppPermissionsMsSql.Update, updateObject);
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.Update.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid id, Guid? modifyingUser)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            modifyingUser ??= Guid.Empty;

            var foundPermission = await GetByIdAsync(id);
            if (!foundPermission.Success || foundPermission.Result is null)
                throw new Exception(foundPermission.ErrorMessage);
            var permissionUpdate = foundPermission.Result.ToUpdate();
            
            // Update permission w/ a property that is modified so we get the last updated on/by for the deleting user
            permissionUpdate.LastModifiedBy = modifyingUser;
            await UpdateAuditing(permissionUpdate);
            await _database.SaveData(AppPermissionsMsSql.Delete, new {Id = id});

            await _auditRepository.CreateAsync(new AuditTrailCreate
            {
                TableName = AppPermissionsMsSql.Table.TableName,
                RecordId = id,
                ChangedBy = ((Guid)permissionUpdate.LastModifiedBy!),
                Action = DatabaseActionType.Delete,
                Before = _serializer.Serialize(permissionUpdate)
            });

            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.Delete.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<bool>> UserHasDirectPermission(Guid userId, string permissionValue)
    {
        DatabaseActionResult<bool> actionReturn = new();

        try
        {
            var foundPermission = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsMsSql.GetByUserIdAndValue, new {UserId = userId, ClaimValue = permissionValue})).FirstOrDefault();
            var hasPermission = foundPermission is not null;
            actionReturn.Succeed(hasPermission);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.GetByUserIdAndValue.Path, ex.Message);
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
                AppPermissionsMsSql.GetByRoleIdAndValue, new {RoleId = roleId, ClaimValue = permissionValue})).FirstOrDefault();
            var hasPermission = foundPermission is not null;
            actionReturn.Succeed(hasPermission);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.GetByRoleIdAndValue.Path, ex.Message);
        }

        return actionReturn;
    }
}