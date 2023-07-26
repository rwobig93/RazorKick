using Application.Constants.Communication;
using Application.Constants.Identity;
using Application.Database.MsSql.Identity;
using Application.Database.MsSql.Shared;
using Application.Helpers.Lifecycle;
using Application.Helpers.Runtime;
using Application.Mappers.Identity;
using Application.Models.Identity.Role;
using Application.Models.Lifecycle;
using Application.Repositories.Identity;
using Application.Repositories.Lifecycle;
using Application.Services.Database;
using Application.Services.System;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Database;
using Domain.Models.Database;

namespace Infrastructure.Repositories.MsSql.Identity;

public class AppRoleRepositoryMsSql : IAppRoleRepository
{
    private readonly ISqlDataService _database;
    private readonly ILogger _logger;
    private readonly IDateTimeService _dateTime;
    private readonly IAuditTrailsRepository _auditRepository;
    private readonly ISerializerService _serializer;

    public AppRoleRepositoryMsSql(ISqlDataService database, ILogger logger, IDateTimeService dateTime, IAuditTrailsRepository auditRepository,
        ISerializerService serializer)
    {
        _database = database;
        _logger = logger;
        _dateTime = dateTime;
        _auditRepository = auditRepository;
        _serializer = serializer;
    }

    private void UpdateAuditing(AppRoleCreate createRole, Guid modifyingUserId)
    {
        try
        {
            createRole.CreatedBy = modifyingUserId;
            createRole.CreatedOn = _dateTime.NowDatabaseTime;
        }
        catch (Exception ex)
        {
            _logger.Error("Failure occurred attempting to create auditing object: [{TableName}][{ObjectName}] :: {ErrorMessage}", 
                AppRolesMsSql.Table.TableName, createRole.Name, ex.Message);
        }
    }

    private async Task UpdateAuditing(AppRoleUpdate updateRole, Guid modifyingUserId)
    {
        try
        {
            // Get current state for diff comparision
            var currentRoleState = await GetByIdAsync(updateRole.Id);
            var auditDiff = AuditHelpers.GetAuditDiff(currentRoleState.Result!.ToObject(), updateRole);
            
            updateRole.LastModifiedBy = modifyingUserId;
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

    public async Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> GetAllPaginatedAsync(int pageNumber, int pageSize)
    {
        DatabaseActionResult<IEnumerable<AppRoleDb>> actionReturn = new();

        try
        {
            var offset = MathHelpers.GetPaginatedOffset(pageNumber, pageSize);
            var allRoles = await _database.LoadData<AppRoleDb, dynamic>(
                AppRolesMsSql.GetAllPaginated, new {Offset =  offset, PageSize = pageSize});
            actionReturn.Succeed(allRoles);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRolesMsSql.GetAllPaginated.Path, ex.Message);
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

    public async Task<DatabaseActionResult<IEnumerable<AppRoleDb>>> SearchPaginatedAsync(string searchText, int pageNumber, int pageSize)
    {
        DatabaseActionResult<IEnumerable<AppRoleDb>> actionReturn = new();

        try
        {
            var offset = MathHelpers.GetPaginatedOffset(pageNumber, pageSize);
            var searchResults = await _database.LoadData<AppRoleDb, dynamic>(
                AppRolesMsSql.SearchPaginated, new { SearchTerm = searchText, Offset =  offset, PageSize = pageSize });
            actionReturn.Succeed(searchResults);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRolesMsSql.SearchPaginated.Path, ex.Message);
        }

        return actionReturn;
    }

    private async Task<DatabaseActionResult> ValidateAdministrativeRoleAction(Guid roleId, Guid modifyingUserId)
    {
        DatabaseActionResult actionReturn = new();
        
        try
        {
            var modifyingRole = await GetByIdAsync(roleId);
            if (!modifyingRole.Success || modifyingRole.Result is null)
            {
                actionReturn.Fail(ErrorMessageConstants.GenericNotFound);
                return actionReturn;
            }

            // If the modifying role isn't administrative we'll move on
            if (modifyingRole.Result.Name != RoleConstants.DefaultRoles.AdminName &&
                modifyingRole.Result.Name != RoleConstants.DefaultRoles.ModeratorName)
            {
                actionReturn.Succeed();
                return actionReturn;
            }

            // Role is administrative so we'll verify the user can take this action - only if they are a member of the role or admin
            var isAdminRequest = await IsUserInRoleAsync(modifyingUserId, RoleConstants.DefaultRoles.AdminName);
            
            // User is admin so they can continue
            if (isAdminRequest.Result)
            {
                actionReturn.Succeed();
                return actionReturn;
            }

            // User is a moderator and is modifying the moderator role
            var isModeratorRequest = await IsUserInRoleAsync(modifyingUserId, RoleConstants.DefaultRoles.ModeratorName);
            if (isModeratorRequest.Result && modifyingRole.Result.Name == RoleConstants.DefaultRoles.ModeratorName)
            {
                actionReturn.Succeed();
                return actionReturn;
            }

            // User isn't admin and is a moderator attempting to modify something they shouldn't be
            actionReturn.Fail(ErrorMessageConstants.CannotAdministrateAdminRole);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, "ValidateAdministrativeRoleAction", ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid>> CreateAsync(AppRoleCreate createObject, Guid modifyingUserId)
    {
        DatabaseActionResult<Guid> actionReturn = new();

        try
        {
            UpdateAuditing(createObject, modifyingUserId);
            var createdId = await _database.SaveDataReturnId(AppRolesMsSql.Insert, createObject);

            await _auditRepository.CreateAsync(new AuditTrailCreate
            {
                TableName = AppRolesMsSql.Table.TableName,
                RecordId = createdId,
                ChangedBy = (createObject.CreatedBy),
                Action = DatabaseActionType.Create,
                After = _serializer.Serialize(createObject)
            });
            
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRolesMsSql.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(AppRoleUpdate updateObject, Guid modifyingUserId)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await UpdateAuditing(updateObject, modifyingUserId);
            await _database.SaveData(AppRolesMsSql.Update, updateObject);
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppRolesMsSql.Update.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid id, Guid modifyingUserId)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            var foundRole = await GetByIdAsync(id);
            if (!foundRole.Success || foundRole.Result is null)
                throw new Exception(foundRole.ErrorMessage);
            var roleUpdate = foundRole.Result.ToUpdate();
            
            // Update role w/ a property that is modified so we get the last updated on/by for the deleting user
            roleUpdate.LastModifiedBy = modifyingUserId;
            await UpdateAsync(roleUpdate, modifyingUserId);
            await _database.SaveData(AppRolesMsSql.Delete, new {Id = id, DeletedOn = _dateTime.NowDatabaseTime});

            await _auditRepository.CreateAsync(new AuditTrailCreate
            {
                TableName = AppRolesMsSql.Table.TableName,
                RecordId = id,
                ChangedBy = ((Guid)roleUpdate.LastModifiedBy!),
                Action = DatabaseActionType.Delete,
                Before = _serializer.Serialize(roleUpdate)
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
            if (foundRole is null)
            {
                actionReturn.Fail(ErrorMessageConstants.GenericNotFound);
                return actionReturn;
            }
            
            var userRoleJunction = (await _database.LoadData<AppUserRoleJunctionDb, dynamic>(
                AppUserRoleJunctionsMsSql.GetByUserRoleId, new {UserId = userId, RoleId = foundRole.Id})).FirstOrDefault();
            var hasRole = userRoleJunction is not null;
            actionReturn.Succeed(hasRole);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, "IsUserInRoleAsync_RoleName", ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> AddUserToRoleAsync(Guid userId, Guid roleId, Guid modifyingUserId)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            var isValidAdminAction = await ValidateAdministrativeRoleAction(roleId, modifyingUserId);
            if (!isValidAdminAction.Success)
            {
                actionReturn.Fail(isValidAdminAction.ErrorMessage);
                return actionReturn;
            }

            await _database.SaveData(AppUserRoleJunctionsMsSql.Insert, new {UserId = userId, RoleId = roleId});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserRoleJunctionsMsSql.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> RemoveUserFromRoleAsync(Guid userId, Guid roleId, Guid modifyingUserId)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            var isValidAdminAction = await ValidateAdministrativeRoleAction(roleId, modifyingUserId);
            if (!isValidAdminAction.Success)
            {
                actionReturn.Fail(isValidAdminAction.ErrorMessage);
                return actionReturn;
            }

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
            var roles = await _database.LoadData<AppRoleDb, dynamic>(
                AppUserRoleJunctionsMsSql.GetRolesOfUser, new {UserId = userId});

            actionReturn.Succeed(roles);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserRoleJunctionsMsSql.GetRolesOfUser.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserDb>>> GetUsersForRole(Guid roleId)
    {
        DatabaseActionResult<IEnumerable<AppUserDb>> actionReturn = new();

        try
        {
            var users = await _database.LoadData<AppUserDb, dynamic>(
                AppUserRoleJunctionsMsSql.GetUsersOfRole, new {RoleId = roleId});
            
            actionReturn.Succeed(users);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserRoleJunctionsMsSql.GetUsersOfRole.Path, ex.Message);
        }

        return actionReturn;
    }
}