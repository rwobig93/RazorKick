using Application.Database.MsSql.Identity;
using Application.Database.MsSql.Shared;
using Application.Models.Identity;
using Application.Repositories.Identity;
using Application.Services.Database;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Identity;
using Domain.Models.Database;
using Domain.Models.Identity;

namespace Infrastructure.Repositories.Identity;

public class AppUserRepository : IAppUserRepository
{
    private readonly ISqlDataService _database;
    private readonly IAppRoleRepository _roleRepository;
    private readonly ILogger _logger;

    public AppUserRepository(ISqlDataService database, IAppRoleRepository roleRepository, ILogger logger)
    {
        _database = database;
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserDb>>> GetAllAsync()
    {
        DatabaseActionResult<IEnumerable<AppUserDb>> actionReturn = new();

        try
        {
            var allUsers = await _database.LoadData<AppUserDb, dynamic>(AppUsers.GetAll, new { });
            actionReturn.Succeed(allUsers);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsers.GetAll.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<int>> GetCountAsync()
    {
        DatabaseActionResult<int> actionReturn = new();

        try
        {
            var rowCount = (await _database.LoadData<int, dynamic>(
                General.GetRowCount, new {TableName = AppUsers.Table.TableName})).FirstOrDefault();
            actionReturn.Succeed(rowCount);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, General.GetRowCount.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserDb>> GetByIdAsync(Guid userId)
    {
        DatabaseActionResult<AppUserDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserDb, dynamic>(
                AppUsers.GetById, new {Id = userId})).FirstOrDefault();
            actionReturn.Succeed(foundUser!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsers.GetById.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserFull>> GetByIdFullAsync(Guid userId)
    {
        DatabaseActionResult<AppUserFull> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserDb, dynamic>(AppUsers.GetById, new {Id = userId})).FirstOrDefault();

            var fullUser = foundUser!.ToFullObject();

            var foundRoles = await _roleRepository.GetRolesForUser(foundUser!.Id);
            fullUser.Roles = foundRoles.Result?.ToList() ?? new List<AppRoleDb>();

            var foundAttributes = await GetAllUserExtendedAttributesAsync(foundUser.Id);
            fullUser.ExtendedAttributes = foundAttributes.Result?.ToList() ?? new List<AppUserExtendedAttributeDb>();

            actionReturn.Succeed(fullUser);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, "GetByIdFullAsync", ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserDb>> GetByUsernameAsync(string username)
    {
        DatabaseActionResult<AppUserDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserDb, dynamic>(
                AppUsers.GetByUsername, new {Username = username})).FirstOrDefault();
            actionReturn.Succeed(foundUser!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsers.GetByUsername.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserDb>> GetByNormalizedUsernameAsync(string normalizedUsername)
    {
        DatabaseActionResult<AppUserDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserDb, dynamic>(
                AppUsers.GetByNormalizedUsername, new {NormalizedUsername = normalizedUsername})).FirstOrDefault();
            actionReturn.Succeed(foundUser!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsers.GetByNormalizedUsername.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserDb>> GetByEmailAsync(string email)
    {
        DatabaseActionResult<AppUserDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserDb, dynamic>(
                AppUsers.GetByEmail, new {Email = email})).FirstOrDefault();
            actionReturn.Succeed(foundUser!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsers.GetByEmail.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserDb>> GetByNormalizedEmailAsync(string normalizedEmail)
    {
        DatabaseActionResult<AppUserDb> actionReturn = new();

        try
        {
            var foundUser = (await _database.LoadData<AppUserDb, dynamic>(
                AppUsers.GetByNormalizedEmail, new {NormalizedEmail = normalizedEmail})).FirstOrDefault();
            actionReturn.Succeed(foundUser!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsers.GetByNormalizedEmail.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(AppUserUpdate updateObject)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppUsers.Update, updateObject);
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsers.Update.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid userId)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppUsers.Delete, new {Id = userId});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsers.Delete.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid>> SetUserId(Guid currentId, Guid newId)
    {
        DatabaseActionResult<Guid> actionReturn = new();

        try
        {
            var updatedId = await _database.SaveDataReturnId(AppUsers.SetUserId, new { CurrentId = currentId, NewId = newId });
            actionReturn.Succeed(updatedId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsers.SetUserId.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> SetCreatedById(Guid userId, Guid createdById)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppUsers.SetCreatedById, new { Id = userId, CreatedBy = createdById });
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsers.SetCreatedById.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserDb>>> SearchAsync(string searchText)
    {
        DatabaseActionResult<IEnumerable<AppUserDb>> actionReturn = new();

        try
        {
            var searchResults =
                await _database.LoadData<AppUserDb, dynamic>(AppUsers.Search, new { SearchTerm = searchText });
            actionReturn.Succeed(searchResults);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsers.Search.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid>> CreateAsync(AppUserCreate createObject)
    {
        DatabaseActionResult<Guid> actionReturn = new();

        try
        {
            var createdId = await _database.SaveDataReturnId(AppUsers.Insert, createObject);
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUsers.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<bool>> IsInRoleAsync(Guid userId, Guid roleId)
    {
        DatabaseActionResult<bool> actionReturn = new();

        try
        {
            var foundMembership = await _database.LoadData<AppUserRoleJunctionDb, dynamic>(
                AppUserRoleJunctions.GetByUserRoleId, new {UserId = userId, RoleId = roleId});

            var isMember = foundMembership.FirstOrDefault() is not null;
            actionReturn.Succeed(isMember);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserRoleJunctions.GetByUserRoleId.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> AddToRoleAsync(Guid userId, Guid roleId)
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

    public async Task<DatabaseActionResult> RemoveFromRoleAsync(Guid userId, Guid roleId)
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

    public async Task<DatabaseActionResult<Guid>> AddExtendedAttributeAsync(AppUserExtendedAttributeAdd addAttribute)
    {
        DatabaseActionResult<Guid> actionReturn = new();

        try
        {
            var createdId = await _database.SaveDataReturnId(AppUserExtendedAttributes.Insert, addAttribute);
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributes.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateExtendedAttributeAsync(Guid attributeId, string newValue)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppUserExtendedAttributes.Update, new {Id = attributeId, Value = newValue});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributes.Update.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> RemoveExtendedAttributeAsync(Guid attributeId)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppUserExtendedAttributes.Delete, new {Id = attributeId});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributes.Delete.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdatePreferences(Guid userId, AppUserPreferenceUpdate preferenceUpdate)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            var existingPreference = (await _database.LoadData<AppUserPreferenceDb, dynamic>(
                AppUserPreferences.GetByOwnerId, new {OwnerId = userId})).FirstOrDefault();
            
            if (existingPreference is null)
                await _database.SaveData(AppUserPreferences.Insert, preferenceUpdate.ToCreate());
            else
                await _database.SaveData(AppUserPreferences.Update, preferenceUpdate);
            
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, "UpdatePreferences", ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserPreferenceFull>> GetPreferences(Guid userId)
    {
        DatabaseActionResult<AppUserPreferenceFull> actionReturn = new();

        try
        {
            var existingPreference = (await _database.LoadData<AppUserPreferenceDb, dynamic>(
                AppUserPreferences.GetByOwnerId, new {OwnerId = userId})).FirstOrDefault();
            
            if (existingPreference is null)
            {
                var newPreferences = new AppUserPreferenceCreate() {OwnerId = userId};
                var createdId = await _database.SaveDataReturnId(
                    AppUserPreferences.Insert, newPreferences);
                existingPreference = (await _database.LoadData<AppUserPreferenceDb, dynamic>(
                        AppUserPreferences.GetById, new {Id = createdId})).FirstOrDefault();
            }
            
            actionReturn.Succeed(existingPreference!.ToFull());
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserPreferences.GetByOwnerId.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserExtendedAttributeDb>> GetExtendedAttributeByIdAsync(Guid attributeId)
    {
        DatabaseActionResult<AppUserExtendedAttributeDb> actionReturn = new();

        try
        {
            var foundAttribute = (await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributes.GetById, new {Id = attributeId})).FirstOrDefault();
            actionReturn.Succeed(foundAttribute!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributes.GetById.Path, ex.Message);
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
                AppUserExtendedAttributes.GetAllOfTypeForOwner, new {OwnerId = userId, Type = type});
            actionReturn.Succeed(foundAttributes);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributes.GetAllOfTypeForOwner.Path, ex.Message);
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
                AppUserExtendedAttributes.GetAllOfNameForOwner, new {OwnerId = userId, Name = name});
            actionReturn.Succeed(foundAttributes);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributes.GetAllOfNameForOwner.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllUserExtendedAttributesAsync(Guid userId)
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>> actionReturn = new();

        try
        {
            var foundAttributes = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributes.GetByOwnerId, new {OwnerId = userId});
            actionReturn.Succeed(foundAttributes);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributes.GetByOwnerId.Path, ex.Message);
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
                AppUserExtendedAttributes.GetAllOfType, new {Type = type});
            actionReturn.Succeed(foundAttributes);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributes.GetAllOfType.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesByNameAsync(string name)
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>> actionReturn = new();

        try
        {
            var foundAttributes = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributes.GetByName, new {Name = name});
            actionReturn.Succeed(foundAttributes);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributes.GetByName.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesAsync()
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>> actionReturn = new();

        try
        {
            var foundAttributes = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributes.GetAll, new { });
            actionReturn.Succeed(foundAttributes);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppUserExtendedAttributes.GetAll.Path, ex.Message);
        }

        return actionReturn;
    }
}