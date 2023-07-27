using Application.Database.MsSql.Identity;
using Application.Database.MsSql.Shared;
using Application.Helpers.Lifecycle;
using Application.Helpers.Runtime;
using Application.Mappers.Identity;
using Application.Models.Identity.User;
using Application.Models.Identity.UserExtensions;
using Application.Models.Lifecycle;
using Application.Repositories.Identity;
using Application.Repositories.Lifecycle;
using Application.Services.Database;
using Application.Services.Identity;
using Application.Services.Lifecycle;
using Application.Services.System;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Database;
using Domain.Enums.Identity;
using Domain.Models.Database;
using Domain.Models.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repositories.MsSql.Identity;

public class AppUserRepositoryMsSql : IAppUserRepository
{
    private readonly ISqlDataService _database;
    private readonly ILogger _logger;
    private readonly IDateTimeService _dateTime;
    private readonly IRunningServerState _serverState;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IAuditTrailsRepository _auditRepository;
    private readonly ISerializerService _serializer;

    public AppUserRepositoryMsSql(ISqlDataService database, ILogger logger, IDateTimeService dateTime, IRunningServerState serverState,
        IServiceScopeFactory scopeFactory, IAuditTrailsRepository auditRepository, ISerializerService serializer)
    {
        _database = database;
        _logger = logger;
        _dateTime = dateTime;
        _scopeFactory = scopeFactory;
        _auditRepository = auditRepository;
        _serializer = serializer;
        _serverState = serverState;
    }

    private async Task UpdateAuditing(AppUserCreate createUser, bool systemUpdate = false)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
            var currentUserId = systemUpdate ? _serverState.SystemUserId : await currentUserService.GetCurrentUserId();
            createUser.CreatedBy = currentUserId ?? createUser.CreatedBy;
            createUser.CreatedOn = _dateTime.NowDatabaseTime;
        }
        catch (Exception ex)
        {
            _logger.Error("Failure occurred attempting to create auditing object: [{TableName}][{ObjectName}] :: {ErrorMessage}", 
                AppUsersMsSql.Table.TableName, createUser.Username, ex.Message);
        }
    }

    private async Task UpdateAuditing(AppUserUpdate updateUser, bool systemUpdate = false)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
            var currentUserId = systemUpdate ? _serverState.SystemUserId : await currentUserService.GetCurrentUserId();

            // Get current state for diff comparision
            var currentUserState = await GetByIdAsync(updateUser.Id);
            var auditDiff = AuditHelpers.GetAuditDiff(currentUserState.Result!.ToUpdate(), updateUser);
            
            updateUser.LastModifiedBy = currentUserId ?? updateUser.LastModifiedBy;
            updateUser.LastModifiedOn = _dateTime.NowDatabaseTime;

            // If no changes were detected for before and after we won't create an audit trail
            if (auditDiff.Before.Keys.Count > 0 && auditDiff.After.Count > 0)
                await _auditRepository.CreateAsync(new AuditTrailCreate
                {
                    TableName = AppUsersMsSql.Table.TableName,
                    RecordId = updateUser.Id,
                    ChangedBy = ((Guid)updateUser.LastModifiedBy!),
                    Action = DatabaseActionType.Update,
                    Before = _serializer.Serialize(auditDiff.Before),
                    After = _serializer.Serialize(auditDiff.After)
                });
        }
        catch (Exception ex)
        {
            _logger.Error("Failure occurred attempting to update auditing object: [{TableName}][{ObjectId}] :: {ErrorMessage}", 
                AppUsersMsSql.Table.TableName, updateUser.Id, ex.Message);
        }
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserSecurityDb>>> GetAllAsync()
    {
        DatabaseActionResult<IEnumerable<AppUserSecurityDb>> actionReturn = new();

        try
        {
            var allUsers = await _database.LoadData<AppUserSecurityDb, dynamic>(AppUsersMsSql.GetAll, new { });
            actionReturn.Succeed(allUsers);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetAll.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserServicePermissionDb>>> GetAllServiceAccountsForPermissionsAsync()
    {
        DatabaseActionResult<IEnumerable<AppUserServicePermissionDb>> actionReturn = new();

        try
        {
            var allUsers = await _database.LoadData<AppUserServicePermissionDb, dynamic>(
                AppUsersMsSql.GetAllServiceAccountsForPermissions, new { });
            actionReturn.Succeed(allUsers);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetAllServiceAccountsForPermissions.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserSecurityDb>>> GetAllPaginatedAsync(int pageNumber, int pageSize)
    {
        DatabaseActionResult<IEnumerable<AppUserSecurityDb>> actionReturn = new();

        try
        {
            var offset = MathHelpers.GetPaginatedOffset(pageNumber, pageSize);
            var allUsers = await _database.LoadData<AppUserSecurityDb, dynamic>(
                AppUsersMsSql.GetAllPaginated, new {Offset =  offset, PageSize = pageSize});
            actionReturn.Succeed(allUsers);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetAllPaginated.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserSecurityDb>>> GetAllServiceAccountsPaginatedAsync(int pageNumber, int pageSize)
    {
        DatabaseActionResult<IEnumerable<AppUserSecurityDb>> actionReturn = new();

        try
        {
            var offset = MathHelpers.GetPaginatedOffset(pageNumber, pageSize);
            var allServiceAccounts = await _database.LoadData<AppUserSecurityDb, dynamic>(
                AppUsersMsSql.GetAllServiceAccountsPaginated, new {Offset =  offset, PageSize = pageSize});
            actionReturn.Succeed(allServiceAccounts);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetAllServiceAccountsPaginated.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserSecurityDb>>> GetAllDisabledPaginatedAsync(int pageNumber, int pageSize)
    {
        DatabaseActionResult<IEnumerable<AppUserSecurityDb>> actionReturn = new();

        try
        {
            var offset = MathHelpers.GetPaginatedOffset(pageNumber, pageSize);
            var allDisabledUsers = await _database.LoadData<AppUserSecurityDb, dynamic>(
                AppUsersMsSql.GetAllDisabledPaginated, new {Offset =  offset, PageSize = pageSize});
            actionReturn.Succeed(allDisabledUsers);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetAllDisabledPaginated.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserSecurityDb>>> GetAllLockedOutPaginatedAsync(int pageNumber, int pageSize)
    {
        DatabaseActionResult<IEnumerable<AppUserSecurityDb>> actionReturn = new();

        try
        {
            var offset = MathHelpers.GetPaginatedOffset(pageNumber, pageSize);
            var allLockedOutUsers = await _database.LoadData<AppUserSecurityDb, dynamic>(
                AppUsersMsSql.GetAllLockedOutPaginated, new {Offset =  offset, PageSize = pageSize});
            actionReturn.Succeed(allLockedOutUsers);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetAllLockedOutPaginated.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<int>> GetCountAsync()
    {
        DatabaseActionResult<int> actionReturn = new();

        try
        {
            var rowCount = (await _database.LoadData<int, dynamic>(
                GeneralMsSql.GetRowCount, new {AppUsersMsSql.Table.TableName})).FirstOrDefault();
            actionReturn.Succeed(rowCount);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, GeneralMsSql.GetRowCount.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserSecurityDb>> GetByIdAsync(Guid userId)
    {
        DatabaseActionResult<AppUserSecurityDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserSecurityDb, dynamic>(
                AppUsersMsSql.GetById, new {Id = userId})).FirstOrDefault();
            actionReturn.Succeed(foundUser!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetById.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserFullDb?>> GetByIdFullAsync(Guid userId)
    {
        DatabaseActionResult<AppUserFullDb?> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserFullDb, dynamic>(
                AppUsersMsSql.GetById, new {Id = userId})).FirstOrDefault();

            if (foundUser is not null)
            {
                foundUser.Roles = (await _database.LoadData<AppRoleDb, dynamic>(
                    AppUserRoleJunctionsMsSql.GetRolesOfUser, new {UserId = foundUser.Id})).ToList();

                foundUser.Permissions = (await _database.LoadData<AppPermissionDb, dynamic>(
                    AppPermissionsMsSql.GetByUserId, new {UserId = foundUser.Id})).ToList();

                foundUser.ExtendedAttributes = (await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                    AppUserExtendedAttributesMsSql.GetByOwnerId, new {OwnerId = foundUser.Id})).ToList();
            }
                
            actionReturn.Succeed(foundUser);   
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetByIdFull.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserSecurityDb>> GetByIdSecurityAsync(Guid id)
    {
        DatabaseActionResult<AppUserSecurityDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserSecurityDb, dynamic>(
                AppUsersMsSql.GetByIdSecurity, new {Id = id})).FirstOrDefault();
            
            actionReturn.Succeed(foundUser!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetByIdSecurity.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserSecurityDb>> GetByUsernameAsync(string username)
    {
        DatabaseActionResult<AppUserSecurityDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserSecurityDb, dynamic>(
                AppUsersMsSql.GetByUsername, new {Username = username})).FirstOrDefault();
            actionReturn.Succeed(foundUser!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetByUsername.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserFullDb>> GetByUsernameFullAsync(string username)
    {
        DatabaseActionResult<AppUserFullDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserFullDb, dynamic>(
                AppUsersMsSql.GetByUsername, new {Username = username})).FirstOrDefault();

            if (foundUser is not null)
            {
                foundUser.Roles = (await _database.LoadData<AppRoleDb, dynamic>(
                    AppUserRoleJunctionsMsSql.GetRolesOfUser, new {UserId = foundUser.Id})).ToList();

                foundUser.Permissions = (await _database.LoadData<AppPermissionDb, dynamic>(
                    AppPermissionsMsSql.GetByUserId, new {UserId = foundUser.Id})).ToList();

                foundUser.ExtendedAttributes = (await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                    AppUserExtendedAttributesMsSql.GetByOwnerId, new {OwnerId = foundUser.Id})).ToList();
            }
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetByUsernameFull.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserSecurityDb>> GetByUsernameSecurityAsync(string username)
    {
        DatabaseActionResult<AppUserSecurityDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserSecurityDb, dynamic>(
                AppUsersMsSql.GetByUsernameSecurity, new {Username = username})).FirstOrDefault();
            
            actionReturn.Succeed(foundUser!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetByUsernameSecurity.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserSecurityDb>> GetByEmailAsync(string email)
    {
        DatabaseActionResult<AppUserSecurityDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserSecurityDb, dynamic>(
                AppUsersMsSql.GetByEmail, new {Email = email})).FirstOrDefault();
            actionReturn.Succeed(foundUser!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetByEmail.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserFullDb>> GetByEmailFullAsync(string email)
    {
        DatabaseActionResult<AppUserFullDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserFullDb, dynamic>(
                AppUsersMsSql.GetByEmail, new {Email = email})).FirstOrDefault();

            if (foundUser is not null)
            {
                foundUser.Roles = (await _database.LoadData<AppRoleDb, dynamic>(
                    AppUserRoleJunctionsMsSql.GetRolesOfUser, new {UserId = foundUser.Id})).ToList();

                foundUser.Permissions = (await _database.LoadData<AppPermissionDb, dynamic>(
                    AppPermissionsMsSql.GetByUserId, new {UserId = foundUser.Id})).ToList();

                foundUser.ExtendedAttributes = (await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                    AppUserExtendedAttributesMsSql.GetByOwnerId, new {OwnerId = foundUser.Id})).ToList();
            }
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetByEmailFull.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid>> CreateAsync(AppUserCreate createObject, bool systemUpdate = false)
    {
        DatabaseActionResult<Guid> actionReturn = new();

        try
        {
            await UpdateAuditing(createObject, systemUpdate);
            var createdId = await _database.SaveDataReturnId(AppUsersMsSql.Insert, createObject);
            // All user get database calls also pull from the security attribute AuthState so we at least need one to exist
            await CreateSecurityAsync(new AppUserSecurityAttributeCreate
            {
                OwnerId = createdId,
                PasswordHash = "null",
                PasswordSalt = "null",
                TwoFactorEnabled = false,
                TwoFactorKey = null,
                AuthState = AuthState.Disabled,
                AuthStateTimestamp = null,
                BadPasswordAttempts = 0,
                LastBadPassword = null
            });
            
            var auditTrail = new AuditTrailCreate
            {
                TableName = AppUsersMsSql.Table.TableName,
                RecordId = createdId,
                ChangedBy = ((Guid) createObject.CreatedBy!),
                Action = DatabaseActionType.Create,
                After = _serializer.Serialize(createObject)
            };

            await _auditRepository.CreateAsync(auditTrail);
            
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(AppUserUpdate updateObject, bool systemUpdate = false)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await UpdateAuditing(updateObject, systemUpdate);
            await _database.SaveData(AppUsersMsSql.Update, updateObject);
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.Update.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid userId, Guid? modifyingUser)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            modifyingUser ??= Guid.Empty;

            var foundUser = await GetByIdAsync(userId);
            if (!foundUser.Succeeded || foundUser.Result is null)
                throw new Exception(foundUser.ErrorMessage);
            var userUpdate = foundUser.Result.ToUpdate();
            
            // Update user w/ a property that is modified so we get the last updated on/by for the deleting user
            userUpdate.LastModifiedBy = modifyingUser;
            await UpdateAsync(userUpdate);
            await _database.SaveData(AppUsersMsSql.Delete, new { userId, DeletedOn = _dateTime.NowDatabaseTime });

            await _auditRepository.CreateAsync(new AuditTrailCreate
            {
                TableName = AppUsersMsSql.Table.TableName,
                RecordId = userId,
                ChangedBy = ((Guid)userUpdate.LastModifiedBy!),
                Action = DatabaseActionType.Delete,
                Before = _serializer.Serialize(userUpdate)
            });
            
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.Delete.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid>> SetUserId(Guid currentId, Guid newId)
    {
        DatabaseActionResult<Guid> actionReturn = new();

        try
        {
            var updatedId = await _database.SaveDataReturnId(
                AppUsersMsSql.SetUserId, new { CurrentId = currentId, NewId = newId });
            var ownerId = await _database.SaveDataReturnId(
                AppUserSecurityAttributesMsSql.SetOwnerId, new { CurrentId = currentId, NewId = newId });
            if (updatedId != ownerId)
                throw new Exception("SetUserID failed, updated User ID doesn't equal security owner ID");
            
            actionReturn.Succeed(updatedId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.SetUserId.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> SetCreatedById(Guid userId, Guid createdById)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppUsersMsSql.SetCreatedById, new { Id = userId, CreatedBy = createdById });
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.SetCreatedById.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserSecurityDb>>> SearchAsync(string searchText)
    {
        DatabaseActionResult<IEnumerable<AppUserSecurityDb>> actionReturn = new();

        try
        {
            var searchResults =
                await _database.LoadData<AppUserSecurityDb, dynamic>(AppUsersMsSql.Search, new { SearchTerm = searchText });
            actionReturn.Succeed(searchResults);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.Search.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserSecurityDb>>> SearchPaginatedAsync(string searchText, int pageNumber, int pageSize)
    {
        DatabaseActionResult<IEnumerable<AppUserSecurityDb>> actionReturn = new();

        try
        {
            var offset = MathHelpers.GetPaginatedOffset(pageNumber, pageSize);
            var searchResults = await _database.LoadData<AppUserSecurityDb, dynamic>(
                AppUsersMsSql.SearchPaginated, new { SearchTerm = searchText, Offset =  offset, PageSize = pageSize });
            actionReturn.Succeed(searchResults);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.SearchPaginated.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserSecurityDb>>> SearchAsync(string searchText, int pageNumber, int pageSize)
    {
        DatabaseActionResult<IEnumerable<AppUserSecurityDb>> actionReturn = new();

        try
        {
            var offset = MathHelpers.GetPaginatedOffset(pageNumber, pageSize);
            var searchResults = await _database.LoadData<AppUserSecurityDb, dynamic>(
                AppUsersMsSql.Search, new { SearchTerm = searchText, Offset =  offset, PageSize = pageSize });
            actionReturn.Succeed(searchResults);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.SearchPaginated.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<bool>> IsInRoleAsync(Guid userId, Guid roleId)
    {
        DatabaseActionResult<bool> actionReturn = new();

        try
        {
            var foundMembership = await _database.LoadData<AppUserRoleJunctionDb, dynamic>(
                AppUserRoleJunctionsMsSql.GetByUserRoleId, new {UserId = userId, RoleId = roleId});

            var isMember = foundMembership.FirstOrDefault() is not null;
            actionReturn.Succeed(isMember);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserRoleJunctionsMsSql.GetByUserRoleId.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> AddToRoleAsync(Guid userId, Guid roleId)
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

    public async Task<DatabaseActionResult> RemoveFromRoleAsync(Guid userId, Guid roleId)
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

    public async Task<DatabaseActionResult<Guid>> AddExtendedAttributeAsync(AppUserExtendedAttributeCreate addAttribute)
    {
        DatabaseActionResult<Guid> actionReturn = new();

        try
        {
            var createdId = await _database.SaveDataReturnId(AppUserExtendedAttributesMsSql.Insert, addAttribute);
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributesMsSql.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateExtendedAttributeAsync(Guid attributeId, string? value, string? description)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppUserExtendedAttributesMsSql.Update,
                new {Id = attributeId, Value = value, Description = description});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributesMsSql.Update.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> RemoveExtendedAttributeAsync(Guid attributeId)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppUserExtendedAttributesMsSql.Delete, new {Id = attributeId});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributesMsSql.Delete.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdatePreferences(Guid userId, AppUserPreferenceUpdate preferenceUpdate)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            var existingPreference = (await _database.LoadData<AppUserPreferenceDb, dynamic>(
                AppUserPreferencesMsSql.GetByOwnerId, new {OwnerId = userId})).FirstOrDefault();
            
            if (existingPreference is null)
                await _database.SaveData(AppUserPreferencesMsSql.Insert, preferenceUpdate.ToCreate());
            else
                await _database.SaveData(AppUserPreferencesMsSql.Update, preferenceUpdate);
            
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, "UpdatePreferences", ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserPreferenceDb>> GetPreferences(Guid userId)
    {
        DatabaseActionResult<AppUserPreferenceDb> actionReturn = new();

        try
        {
            var existingPreference = (await _database.LoadData<AppUserPreferenceDb, dynamic>(
                AppUserPreferencesMsSql.GetByOwnerId, new {OwnerId = userId})).FirstOrDefault();
            
            if (existingPreference is null)
            {
                var newPreferences = new AppUserPreferenceCreate() {OwnerId = userId};
                var createdId = await _database.SaveDataReturnId(
                    AppUserPreferencesMsSql.Insert, newPreferences);
                existingPreference = (await _database.LoadData<AppUserPreferenceDb, dynamic>(
                        AppUserPreferencesMsSql.GetById, new {Id = createdId})).FirstOrDefault();
            }
            
            actionReturn.Succeed(existingPreference!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserPreferencesMsSql.GetByOwnerId.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserExtendedAttributeDb>> GetExtendedAttributeByIdAsync(Guid attributeId)
    {
        DatabaseActionResult<AppUserExtendedAttributeDb> actionReturn = new();

        try
        {
            var foundAttribute = (await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributesMsSql.GetById, new {Id = attributeId})).FirstOrDefault();
            actionReturn.Succeed(foundAttribute!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributesMsSql.GetById.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb?>>> GetExtendedAttributeByTypeAndValueAsync(
        ExtendedAttributeType type, string value)
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb?>> actionReturn = new();

        try
        {
            var foundAttribute = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributesMsSql.GetByTypeAndValue, new {Type = type, Value = value});
            actionReturn.Succeed(foundAttribute);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributesMsSql.GetByTypeAndValue.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetUserExtendedAttributesByTypeAsync(Guid userId,
        ExtendedAttributeType type)
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>> actionReturn = new();

        try
        {
            var foundAttributes = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributesMsSql.GetAllOfTypeForOwner, new {OwnerId = userId, Type = type});
            actionReturn.Succeed(foundAttributes);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributesMsSql.GetAllOfTypeForOwner.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetUserExtendedAttributesByTypeAndValueAsync(
        Guid userId, ExtendedAttributeType type, string value)
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>> actionReturn = new();

        try
        {
            var foundAttributes = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributesMsSql.GetByTypeAndValueForOwner, new {OwnerId = userId, Type = type, Value = value});
            actionReturn.Succeed(foundAttributes);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributesMsSql.GetByTypeAndValueForOwner.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetUserExtendedAttributesByNameAsync(Guid userId,
        string name)
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>> actionReturn = new();

        try
        {
            var foundAttributes = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributesMsSql.GetAllOfNameForOwner, new {OwnerId = userId, Name = name});
            actionReturn.Succeed(foundAttributes);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributesMsSql.GetAllOfNameForOwner.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllUserExtendedAttributesAsync(Guid userId)
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>> actionReturn = new();

        try
        {
            var foundAttributes = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributesMsSql.GetByOwnerId, new {OwnerId = userId});
            actionReturn.Succeed(foundAttributes);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributesMsSql.GetByOwnerId.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesByTypeAsync(
        ExtendedAttributeType type)
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>> actionReturn = new();

        try
        {
            var foundAttributes = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributesMsSql.GetAllOfType, new {Type = type});
            actionReturn.Succeed(foundAttributes);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributesMsSql.GetAllOfType.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesByNameAsync(string name)
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>> actionReturn = new();

        try
        {
            var foundAttributes = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributesMsSql.GetByName, new {Name = name});
            actionReturn.Succeed(foundAttributes);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributesMsSql.GetByName.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesAsync()
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>> actionReturn = new();

        try
        {
            var foundAttributes = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributesMsSql.GetAll, new { });
            actionReturn.Succeed(foundAttributes);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributesMsSql.GetAll.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid>> CreateSecurityAsync(AppUserSecurityAttributeCreate securityCreate)
    {
        DatabaseActionResult<Guid> actionReturn = new();

        try
        {
            Guid securityId;
            
            var existingSecurity = (await _database.LoadData<AppUserSecurityAttributeDb, dynamic>(
                AppUserSecurityAttributesMsSql.GetByOwnerId, new {securityCreate.OwnerId})).FirstOrDefault();

            if (existingSecurity is null)
                securityId = await _database.SaveDataReturnId(AppUserSecurityAttributesMsSql.Insert, securityCreate);
            else
            {
                securityId = existingSecurity.Id;
                await UpdateSecurityAsync(existingSecurity.ToUpdate());
            }
            
            actionReturn.Succeed(securityId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserSecurityAttributesMsSql.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserSecurityAttributeDb>> GetSecurityAsync(Guid userId)
    {
        DatabaseActionResult<AppUserSecurityAttributeDb> actionReturn = new();

        try
        {
            var userSecurity = (await _database.LoadData<AppUserSecurityAttributeDb, dynamic>(
                AppUserSecurityAttributesMsSql.GetByOwnerId, new {OwnerId = userId})).FirstOrDefault();
            
            actionReturn.Succeed(userSecurity!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserSecurityAttributesMsSql.GetByOwnerId.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateSecurityAsync(AppUserSecurityAttributeUpdate securityUpdate)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppUserSecurityAttributesMsSql.Update, securityUpdate);
            
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserSecurityAttributesMsSql.Update.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserSecurityDb>>> GetAllLockedOutAsync()
    {
        DatabaseActionResult<IEnumerable<AppUserSecurityDb>> actionReturn = new();

        try
        {
            var allUsers = await _database.LoadData<AppUserSecurityDb, dynamic>(
                AppUsersMsSql.GetAllLockedOut, new { });
            actionReturn.Succeed(allUsers);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetAllLockedOut.Path, ex.Message);
        }

        return actionReturn;
    }
}