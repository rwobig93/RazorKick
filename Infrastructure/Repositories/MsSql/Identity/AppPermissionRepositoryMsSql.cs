using Application.Database.MsSql.Identity;
using Application.Database.MsSql.Shared;
using Application.Helpers.Runtime;
using Application.Models.Identity.Permission;
using Application.Repositories.Identity;
using Application.Services.Database;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Database;

namespace Infrastructure.Repositories.MsSql.Identity;

public class AppPermissionRepositoryMsSql : IAppPermissionRepository
{
    private readonly ISqlDataService _database;
    private readonly ILogger _logger;

    public AppPermissionRepositoryMsSql(ISqlDataService database, ILogger logger)
    {
        _database = database;
        _logger = logger;
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

            var roles = await _database.LoadData<AppRoleDb, dynamic>(
                AppUserRoleJunctionsMsSql.GetRolesOfUser, new {UserId = userId});
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
            var createdId = await _database.SaveDataReturnId(AppPermissionsMsSql.Insert, createObject);
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(AppPermissionUpdate updateObject)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppPermissionsMsSql.Update, updateObject);
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AppPermissionsMsSql.Update.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid id)
    {
        DatabaseActionResult actionReturn = new();

        try
        {
            await _database.SaveData(AppPermissionsMsSql.Delete, new {Id = id});
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