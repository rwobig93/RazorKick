﻿using Application.Database.MsSql.Identity;
using Application.Database.MsSql.Shared;
using Application.Helpers.Lifecycle;
using Application.Models.Identity;
using Application.Models.Lifecycle;
using Application.Repositories.Identity;
using Application.Repositories.Lifecycle;
using Application.Services.Database;
using Application.Services.Identity;
using Application.Services.System;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Database;
using Domain.Enums.Identity;
using Domain.Models.Database;
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
            if (auditDiff.Before != new Dictionary<string, string>() && auditDiff.After != new Dictionary<string, string>())
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

    public async Task<DatabaseActionResult<IEnumerable<AppUserDb>>> GetAllAsync()
    {
        DatabaseActionResult<IEnumerable<AppUserDb>> actionReturn = new();

        try
        {
            var allUsers = await _database.LoadData<AppUserDb, dynamic>(AppUsersMsSql.GetAll, new { });
            actionReturn.Succeed(allUsers);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetAll.Path, ex.Message);
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

    public async Task<DatabaseActionResult<AppUserDb>> GetByIdAsync(Guid userId)
    {
        DatabaseActionResult<AppUserDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserDb, dynamic>(
                AppUsersMsSql.GetById, new {Id = userId})).FirstOrDefault();
            actionReturn.Succeed(foundUser!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetById.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserDb>> GetByUsernameAsync(string username)
    {
        DatabaseActionResult<AppUserDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserDb, dynamic>(
                AppUsersMsSql.GetByUsername, new {Username = username})).FirstOrDefault();
            actionReturn.Succeed(foundUser!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetByUsername.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserDb>> GetByNormalizedUsernameAsync(string normalizedUsername)
    {
        DatabaseActionResult<AppUserDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserDb, dynamic>(
                AppUsersMsSql.GetByNormalizedUsername, new {NormalizedUsername = normalizedUsername})).FirstOrDefault();
            actionReturn.Succeed(foundUser!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetByNormalizedUsername.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserDb>> GetByEmailAsync(string email)
    {
        DatabaseActionResult<AppUserDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserDb, dynamic>(
                AppUsersMsSql.GetByEmail, new {Email = email})).FirstOrDefault();
            actionReturn.Succeed(foundUser!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetByEmail.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserDb>> GetByNormalizedEmailAsync(string normalizedEmail)
    {
        DatabaseActionResult<AppUserDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserDb, dynamic>(
                AppUsersMsSql.GetByNormalizedEmail, new {NormalizedEmail = normalizedEmail})).FirstOrDefault();
            actionReturn.Succeed(foundUser!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.GetByNormalizedEmail.Path, ex.Message);
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

            await _auditRepository.CreateAsync(new AuditTrailCreate
            {
                TableName = AppUsersMsSql.Table.TableName,
                RecordId = createdId,
                ChangedBy = ((Guid)createObject.CreatedBy!),
                Action = DatabaseActionType.Create
            });
            
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

    public async Task<DatabaseActionResult> DeleteAsync(Guid userId)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            // Update user w/ a property that is modified so we get the last updated on/by for the deleting user
            var userUpdate = new AppUserUpdate() {Id = userId};
            await UpdateAsync(userUpdate);
            await _database.SaveData(AppUsersMsSql.Delete, new { userId, DeletedOn = _dateTime.NowDatabaseTime });

            await _auditRepository.CreateAsync(new AuditTrailCreate
            {
                TableName = AppUsersMsSql.Table.TableName,
                RecordId = userId,
                ChangedBy = ((Guid)userUpdate.LastModifiedBy!),
                Action = DatabaseActionType.Delete
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
            var updatedId = await _database.SaveDataReturnId(AppUsersMsSql.SetUserId, new { CurrentId = currentId, NewId = newId });
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

    public async Task<DatabaseActionResult<IEnumerable<AppUserDb>>> SearchAsync(string searchText)
    {
        DatabaseActionResult<IEnumerable<AppUserDb>> actionReturn = new();

        try
        {
            var searchResults =
                await _database.LoadData<AppUserDb, dynamic>(AppUsersMsSql.Search, new { SearchTerm = searchText });
            actionReturn.Succeed(searchResults);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsersMsSql.Search.Path, ex.Message);
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

    public async Task<DatabaseActionResult<Guid>> AddExtendedAttributeAsync(AppUserExtendedAttributeAdd addAttribute)
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

    public async Task<DatabaseActionResult> UpdateExtendedAttributeAsync(Guid attributeId, string newValue)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppUserExtendedAttributesMsSql.Update, new {Id = attributeId, Value = newValue});
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
}