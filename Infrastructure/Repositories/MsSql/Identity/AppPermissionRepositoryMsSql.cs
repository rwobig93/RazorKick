using Application.Helpers.Runtime;
using Application.Mappers.Identity;
using Application.Models.Identity.Permission;
using Application.Models.Lifecycle;
using Application.Repositories.Identity;
using Application.Repositories.Lifecycle;
using Application.Services.Database;
using Application.Services.System;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Database;
using Domain.Models.Database;
using Infrastructure.Database.MsSql.Identity;
using Infrastructure.Database.MsSql.Shared;

namespace Infrastructure.Repositories.MsSql.Identity;

public class AppPermissionRepositoryMsSql : IAppPermissionRepository
{
    private readonly ISqlDataService _database;
    private readonly ILogger _logger;
    private readonly IAuditTrailsRepository _auditRepository;
    private readonly ISerializerService _serializer;

    public AppPermissionRepositoryMsSql(ISqlDataService database, ILogger logger, IAuditTrailsRepository auditRepository, ISerializerService serializer)
    {
        _database = database;
        _logger = logger;
        _auditRepository = auditRepository;
        _serializer = serializer;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllAsync()
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var allPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsTableMsSql.GetAll, new { });
            actionReturn.Succeed(allPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.GetAll.Path, ex.Message);
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
                AppPermissionsTableMsSql.GetAllPaginated, new {Offset =  offset, PageSize = pageSize});
            actionReturn.Succeed(allPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.GetAllPaginated.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> SearchAsync(string searchTerm)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var searchResults = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsTableMsSql.Search, new {SearchTerm = searchTerm});
            actionReturn.Succeed(searchResults);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.Search.Path, ex.Message);
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
                AppPermissionsTableMsSql.SearchPaginated, new {SearchTerm = searchTerm, Offset =  offset, PageSize = pageSize});
            actionReturn.Succeed(searchResults);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.SearchPaginated.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<int>> GetCountAsync()
    {
        DatabaseActionResult<int> actionReturn = new();

        try
        {
            var rowCount = (await _database.LoadData<int, dynamic>(
                GeneralTableMsSql.GetRowCount, new {AppPermissionsTableMsSql.Table.TableName})).FirstOrDefault();
            actionReturn.Succeed(rowCount);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, GeneralTableMsSql.GetRowCount.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppPermissionDb>> GetByIdAsync(Guid id)
    {
        DatabaseActionResult<AppPermissionDb> actionReturn = new();

        try
        {
            var foundPermission = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsTableMsSql.GetById, new {Id = id})).FirstOrDefault();
            actionReturn.Succeed(foundPermission!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.GetById.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppPermissionDb>> GetByUserIdAndValueAsync(Guid userId, string claimValue)
    {
        DatabaseActionResult<AppPermissionDb> actionReturn = new();

        try
        {
            var foundPermission = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsTableMsSql.GetByUserIdAndValue, new {UserId = userId, ClaimValue = claimValue})).FirstOrDefault();
            actionReturn.Succeed(foundPermission!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.GetByUserIdAndValue.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppPermissionDb>> GetByRoleIdAndValueAsync(Guid roleId, string claimValue)
    {
        DatabaseActionResult<AppPermissionDb> actionReturn = new();

        try
        {
            var foundPermission = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsTableMsSql.GetByRoleIdAndValue, new {RoleId = roleId, ClaimValue = claimValue})).FirstOrDefault();
            actionReturn.Succeed(foundPermission!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.GetByRoleIdAndValue.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllByNameAsync(string roleName)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var foundPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsTableMsSql.GetByName, new {Name = roleName});
            actionReturn.Succeed(foundPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.GetByName.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllByGroupAsync(string groupName)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var foundPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsTableMsSql.GetByGroup, new {Group = groupName});
            actionReturn.Succeed(foundPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.GetByGroup.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllByAccessAsync(string accessName)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var foundPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsTableMsSql.GetByAccess, new {Access = accessName});
            actionReturn.Succeed(foundPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.GetByAccess.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllForRoleAsync(Guid roleId)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var foundPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsTableMsSql.GetByRoleId, new {RoleId = roleId});
            actionReturn.Succeed(foundPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.GetByRoleId.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppPermissionDb>>> GetAllDirectForUserAsync(Guid userId)
    {
        DatabaseActionResult<IEnumerable<AppPermissionDb>> actionReturn = new();

        try
        {
            var foundPermissions = await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsTableMsSql.GetByUserId, new {UserId = userId});
            actionReturn.Succeed(foundPermissions);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.GetByUserId.Path, ex.Message);
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
                AppPermissionsTableMsSql.GetByUserId, new {UserId = userId});
            allPermissions.AddRange(userPermissions);

            var roles = await _database.LoadData<AppRoleDb, dynamic>(
                AppUserRoleJunctionsTableMsSql.GetRolesOfUser, new {UserId = userId});
            foreach (var role in roles)
            {
                var rolePermissions = await GetAllForRoleAsync(role.Id);
                if (rolePermissions.Succeeded)
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
            var createdId = await _database.SaveDataReturnId(AppPermissionsTableMsSql.Insert, createObject);

            var createdPermission = await GetByIdAsync(createdId);

            await _auditRepository.CreateAsync(new AuditTrailCreate
            {
                TableName = AppPermissionsTableMsSql.Table.TableName,
                RecordId = createdId,
                ChangedBy = createObject.CreatedBy,
                Action = DatabaseActionType.Create,
                After = _serializer.Serialize(createdPermission.Result)
            });
            
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(AppPermissionUpdate updateObject)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            var foundPermission = await GetByIdAsync(updateObject.Id);
            
            await _database.SaveData(AppPermissionsTableMsSql.Update, updateObject);

            var foundPermissionAfterUpdate = await GetByIdAsync(updateObject.Id);
            
            await _auditRepository.CreateAsync(new AuditTrailCreate
            {
                TableName = AppPermissionsTableMsSql.Table.TableName,
                RecordId = updateObject.Id,
                ChangedBy = updateObject.LastModifiedBy.GetFromNullable(),
                Action = DatabaseActionType.Update,
                Before = _serializer.Serialize(foundPermission.Result!.ToSlim()),
                After = _serializer.Serialize(foundPermissionAfterUpdate.Result!.ToSlim())
            });
            
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.Update.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid id, Guid modifyingUserId)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            var foundPermission = await GetByIdAsync(id);
            
            await _database.SaveData(AppPermissionsTableMsSql.Delete, new {Id = id});
            
            await _auditRepository.CreateAsync(new AuditTrailCreate
            {
                TableName = AppPermissionsTableMsSql.Table.TableName,
                RecordId = foundPermission.Result!.Id,
                ChangedBy = modifyingUserId,
                Action = DatabaseActionType.Delete,
                Before = _serializer.Serialize(foundPermission.Result!.ToSlim())
            });
            
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.Delete.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<bool>> UserHasDirectPermission(Guid userId, string permissionValue)
    {
        DatabaseActionResult<bool> actionReturn = new();

        try
        {
            var foundPermission = (await _database.LoadData<AppPermissionDb, dynamic>(
                AppPermissionsTableMsSql.GetByUserIdAndValue, new {UserId = userId, ClaimValue = permissionValue})).FirstOrDefault();
            var hasPermission = foundPermission is not null;
            actionReturn.Succeed(hasPermission);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.GetByUserIdAndValue.Path, ex.Message);
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
                AppPermissionsTableMsSql.GetByRoleIdAndValue, new {RoleId = roleId, ClaimValue = permissionValue})).FirstOrDefault();
            var hasPermission = foundPermission is not null;
            actionReturn.Succeed(hasPermission);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsTableMsSql.GetByRoleIdAndValue.Path, ex.Message);
        }

        return actionReturn;
    }
}