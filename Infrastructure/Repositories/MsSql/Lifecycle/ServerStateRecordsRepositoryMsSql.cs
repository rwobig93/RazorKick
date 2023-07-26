using Application.Database.MsSql.Lifecycle;
using Application.Database.MsSql.Shared;
using Application.Models.Lifecycle;
using Application.Repositories.Lifecycle;
using Application.Services.Database;
using Application.Services.System;
using Domain.DatabaseEntities.Lifecycle;
using Domain.Models.Database;

namespace Infrastructure.Repositories.MsSql.Lifecycle;

public class ServerStateRecordsRepositoryMsSql : IServerStateRecordsRepository
{
    private readonly ISqlDataService _database;
    private readonly ILogger _logger;
    private readonly IDateTimeService _dateTime;

    public ServerStateRecordsRepositoryMsSql(ISqlDataService database, ILogger logger, IDateTimeService dateTime)
    {
        _database = database;
        _logger = logger;
        _dateTime = dateTime;
    }

    public async Task<DatabaseActionResult<IEnumerable<ServerStateRecordDb>>> GetAllAsync()
    {
        DatabaseActionResult<IEnumerable<ServerStateRecordDb>> actionReturn = new();

        try
        {
            var allStateRecords = await _database.LoadData<ServerStateRecordDb, dynamic>(
                ServerStateRecordsMsSql.GetAll, new { });
            actionReturn.Succeed(allStateRecords);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, ServerStateRecordsMsSql.GetAll.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<ServerStateRecordDb>>> GetAllBeforeDate(DateTime olderThan)
    {
        DatabaseActionResult<IEnumerable<ServerStateRecordDb>> actionReturn = new();

        try
        {
            var foundStateRecords = await _database.LoadData<ServerStateRecordDb, dynamic>(
                ServerStateRecordsMsSql.GetAllBeforeDate, new {OlderThan = olderThan});
            actionReturn.Succeed(foundStateRecords);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, ServerStateRecordsMsSql.GetAllBeforeDate.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<ServerStateRecordDb>>> GetAllAfterDate(DateTime newerThan)
    {
        DatabaseActionResult<IEnumerable<ServerStateRecordDb>> actionReturn = new();

        try
        {
            var foundStateRecords = await _database.LoadData<ServerStateRecordDb, dynamic>(
                ServerStateRecordsMsSql.GetAllAfterDate, new {NewerThan = newerThan});
            actionReturn.Succeed(foundStateRecords);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, ServerStateRecordsMsSql.GetAllAfterDate.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<int>> GetCountAsync()
    {
        DatabaseActionResult<int> actionReturn = new();

        try
        {
            var rowCount = (await _database.LoadData<int, dynamic>(
                GeneralMsSql.GetRowCount, new {ServerStateRecordsMsSql.Table.TableName})).FirstOrDefault();
            actionReturn.Succeed(rowCount);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, GeneralMsSql.GetRowCount.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<ServerStateRecordDb>> GetByIdAsync(Guid id)
    {
        DatabaseActionResult<ServerStateRecordDb> actionReturn = new();

        try
        {
            var foundServerState = (await _database.LoadData<ServerStateRecordDb, dynamic>(
                ServerStateRecordsMsSql.GetById, new {Id = id})).FirstOrDefault();
            actionReturn.Succeed(foundServerState!);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, ServerStateRecordsMsSql.GetById.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<ServerStateRecordDb>>> GetByVersion(Version version)
    {
        DatabaseActionResult<IEnumerable<ServerStateRecordDb>> actionReturn = new();

        try
        {
            var foundStateRecords = await _database.LoadData<ServerStateRecordDb, dynamic>(
                ServerStateRecordsMsSql.GetByVersion, new {Version = version.ToString()});
            actionReturn.Succeed(foundStateRecords);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, ServerStateRecordsMsSql.GetByVersion.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid>> CreateAsync(ServerStateRecordCreate createRecord)
    {
        DatabaseActionResult<Guid> actionReturn = new();

        try
        {
            createRecord.Timestamp = _dateTime.NowDatabaseTime;
            
            var createdId = await _database.SaveDataReturnId(ServerStateRecordsMsSql.Insert, createRecord);
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, ServerStateRecordsMsSql.Insert.Path, ex.Message);
        }

        return actionReturn;
    }
}