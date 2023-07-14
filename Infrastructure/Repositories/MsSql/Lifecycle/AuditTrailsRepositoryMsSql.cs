using System.Globalization;
using Application.Database.MsSql.Lifecycle;
using Application.Database.MsSql.Shared;
using Application.Helpers.Runtime;
using Application.Models.Lifecycle;
using Application.Repositories.Lifecycle;
using Application.Services.Database;
using Application.Services.Identity;
using Application.Services.System;
using Domain.DatabaseEntities.Lifecycle;
using Domain.Enums.Lifecycle;
using Domain.Models.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repositories.MsSql.Lifecycle;

public class AuditTrailsRepositoryMsSql : IAuditTrailsRepository
{
    private readonly ISqlDataService _database;
    private readonly ILogger _logger;
    private readonly IDateTimeService _dateTime;
    private readonly IRunningServerState _serverState;
    private readonly IServiceScopeFactory _scopeFactory;

    public AuditTrailsRepositoryMsSql(ISqlDataService database, ILogger logger, IDateTimeService dateTime, IRunningServerState serverState,
        IServiceScopeFactory scopeFactory)
    {
        _database = database;
        _logger = logger;
        _dateTime = dateTime;
        _serverState = serverState;
        _scopeFactory = scopeFactory;
    }
    
    public async Task<DatabaseActionResult<IEnumerable<AuditTrailDb>>> GetAllAsync()
    {
        DatabaseActionResult<IEnumerable<AuditTrailDb>> actionReturn = new();

        try
        {
            var allAuditTrails = await _database.LoadData<AuditTrailDb, dynamic>(AuditTrailsMsSql.GetAll, new { });
            actionReturn.Succeed(allAuditTrails);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AuditTrailsMsSql.GetAll.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AuditTrailWithUserDb>>> GetAllWithUsersAsync()
    {
        DatabaseActionResult<IEnumerable<AuditTrailWithUserDb>> actionReturn = new();

        try
        {
            var allAuditTrails = await _database.LoadData<AuditTrailWithUserDb, dynamic>(
                AuditTrailsMsSql.GetAllWithUsers, new { });
            actionReturn.Succeed(allAuditTrails);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AuditTrailsMsSql.GetAllWithUsers.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AuditTrailDb>>> GetAllPaginatedAsync(int pageNumber, int pageSize)
    {
        DatabaseActionResult<IEnumerable<AuditTrailDb>> actionReturn = new();

        try
        {
            var offset = MathHelpers.GetPaginatedOffset(pageNumber, pageSize);
            var allAuditTrails = await _database.LoadData<AuditTrailDb, dynamic>(
                AuditTrailsMsSql.GetAllPaginated, new {Offset =  offset, PageSize = pageSize});
            actionReturn.Succeed(allAuditTrails);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AuditTrailsMsSql.GetAllPaginated.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AuditTrailWithUserDb>>> GetAllPaginatedWithUsersAsync(int pageNumber, int pageSize)
    {
        DatabaseActionResult<IEnumerable<AuditTrailWithUserDb>> actionReturn = new();

        try
        {
            var offset = (pageNumber - 1) * pageSize;
            var allAuditTrails = await _database.LoadData<AuditTrailWithUserDb, dynamic>(
                AuditTrailsMsSql.GetAllPaginatedWithUsers, new {Offset =  offset, PageSize = pageSize});
            actionReturn.Succeed(allAuditTrails);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AuditTrailsMsSql.GetAllPaginatedWithUsers.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<int>> GetCountAsync()
    {
        DatabaseActionResult<int> actionReturn = new();

        try
        {
            var rowCount = (await _database.LoadData<int, dynamic>(
                GeneralMsSql.GetRowCount, new {AuditTrailsMsSql.Table.TableName})).FirstOrDefault();
            actionReturn.Succeed(rowCount);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, GeneralMsSql.GetRowCount.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AuditTrailDb>> GetByIdAsync(Guid id)
    {
        DatabaseActionResult<AuditTrailDb> actionReturn = new();

        try
        {
            var foundAuditTrail = (await _database.LoadData<AuditTrailDb, dynamic>(
                AuditTrailsMsSql.GetById, new {Id = id})).FirstOrDefault();
            actionReturn.Succeed(foundAuditTrail!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AuditTrailsMsSql.GetById.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<AuditTrailWithUserDb>> GetByIdWithUserAsync(Guid id)
    {
        DatabaseActionResult<AuditTrailWithUserDb> actionReturn = new();

        try
        {
            var foundAuditTrail = (await _database.LoadData<AuditTrailWithUserDb, dynamic>(
                AuditTrailsMsSql.GetByIdWithUser, new {Id = id})).FirstOrDefault();
            actionReturn.Succeed(foundAuditTrail!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AuditTrailsMsSql.GetByIdWithUser.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AuditTrailWithUserDb>>> GetByChangedByIdAsync(Guid id)
    {
        DatabaseActionResult<IEnumerable<AuditTrailWithUserDb>> actionReturn = new();

        try
        {
            var foundAuditTrail = (await _database.LoadData<AuditTrailWithUserDb, dynamic>(
                AuditTrailsMsSql.GetByChangedBy, new {UserId = id}));
            actionReturn.Succeed(foundAuditTrail);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AuditTrailsMsSql.GetByChangedBy.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AuditTrailWithUserDb>>> GetByRecordIdAsync(Guid id)
    {
        DatabaseActionResult<IEnumerable<AuditTrailWithUserDb>> actionReturn = new();

        try
        {
            var foundAuditTrail = (await _database.LoadData<AuditTrailWithUserDb, dynamic>(
                AuditTrailsMsSql.GetByRecordId, new {RecordId = id}));
            actionReturn.Succeed(foundAuditTrail);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AuditTrailsMsSql.GetByRecordId.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid>> CreateAsync(AuditTrailCreate createObject, bool systemUpdate = false)
    {
        DatabaseActionResult<Guid> actionReturn = new();

        try
        {
            if (createObject.ChangedBy == Guid.Empty)
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
                var currentUserId = systemUpdate ? _serverState.SystemUserId : await currentUserService.GetCurrentUserId();
                createObject.ChangedBy = currentUserId ?? Guid.Empty;
            }

            createObject.Timestamp = _dateTime.NowDatabaseTime;
            
            var createdId = await _database.SaveDataReturnId(AuditTrailsMsSql.Insert, createObject);
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AuditTrailsMsSql.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AuditTrailDb>>> SearchAsync(string searchText)
    {
        DatabaseActionResult<IEnumerable<AuditTrailDb>> actionReturn = new();

        try
        {
            var searchResults =
                await _database.LoadData<AuditTrailDb, dynamic>(AuditTrailsMsSql.Search, new { SearchTerm = searchText });
            actionReturn.Succeed(searchResults);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AuditTrailsMsSql.Search.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AuditTrailWithUserDb>>> SearchWithUserAsync(string searchText)
    {
        DatabaseActionResult<IEnumerable<AuditTrailWithUserDb>> actionReturn = new();

        try
        {
            var searchResults =
                await _database.LoadData<AuditTrailWithUserDb, dynamic>(AuditTrailsMsSql.SearchWithUser, new { SearchTerm = searchText });
            actionReturn.Succeed(searchResults);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AuditTrailsMsSql.SearchWithUser.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<int>> DeleteOld(CleanupTimeframe olderThan)
    {
        DatabaseActionResult<int> actionReturn = new();

        try
        {
            var cleanupTimestamp = olderThan switch
            {
                CleanupTimeframe.OneMonth => _dateTime.NowDatabaseTime.AddMonths(-1).ToString(CultureInfo.CurrentCulture),
                CleanupTimeframe.ThreeMonths => _dateTime.NowDatabaseTime.AddMonths(-3).ToString(CultureInfo.CurrentCulture),
                CleanupTimeframe.SixMonths => _dateTime.NowDatabaseTime.AddMonths(-6).ToString(CultureInfo.CurrentCulture),
                CleanupTimeframe.OneYear => _dateTime.NowDatabaseTime.AddYears(-1).ToString(CultureInfo.CurrentCulture),
                CleanupTimeframe.TenYears => _dateTime.NowDatabaseTime.AddYears(-10).ToString(CultureInfo.CurrentCulture),
                _ => _dateTime.NowDatabaseTime.AddMonths(-6).ToString(CultureInfo.CurrentCulture)
            };

            var rowsDeleted = await _database.SaveData(AuditTrailsMsSql.DeleteOlderThan, new {OlderThan = cleanupTimestamp});
            actionReturn.Succeed(rowsDeleted);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, AuditTrailsMsSql.DeleteOlderThan.Path, ex.Message);
        }

        return actionReturn;
    }
}