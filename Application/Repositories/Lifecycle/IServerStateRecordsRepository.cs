using Application.Models.Lifecycle;
using Domain.DatabaseEntities.Lifecycle;
using Domain.Models.Database;

namespace Application.Repositories.Lifecycle;

public interface IServerStateRecordsRepository
{
    Task<DatabaseActionResult<IEnumerable<ServerStateRecordDb>>> GetAllAsync();
    Task<DatabaseActionResult<IEnumerable<ServerStateRecordDb>>> GetAllBeforeDate(DateTime olderThan);
    Task<DatabaseActionResult<IEnumerable<ServerStateRecordDb>>> GetAllAfterDate(DateTime newerThan);
    Task<DatabaseActionResult<int>> GetCountAsync();
    Task<DatabaseActionResult<ServerStateRecordDb>> GetByIdAsync(Guid id);
    Task<DatabaseActionResult<IEnumerable<ServerStateRecordDb>>> GetByVersion(Version version);
    Task<DatabaseActionResult<Guid>> CreateAsync(ServerStateRecordCreate createRecord);
}