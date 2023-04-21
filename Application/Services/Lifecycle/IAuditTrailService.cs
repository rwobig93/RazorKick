using Application.Models.Lifecycle;
using Application.Models.Web;
using Domain.Enums.Lifecycle;

namespace Application.Services.Lifecycle;

public interface IAuditTrailService
{
    Task<IResult<IEnumerable<AuditTrailSlim>>> GetAllAsync();
    Task<IResult<IEnumerable<AuditTrailSlim>>> GetAllPaginatedAsync(int pageNumber, int pageSize);
    Task<IResult<int>> GetCountAsync();
    Task<IResult<AuditTrailSlim?>> GetByIdAsync(Guid id);
    Task<IResult<IEnumerable<AuditTrailSlim>>> GetByChangedByIdAsync(Guid id);
    Task<IResult<IEnumerable<AuditTrailSlim>>> GetByRecordIdAsync(Guid id);
    Task<IResult<Guid>> CreateAsync(AuditTrailCreate createObject, bool systemUpdate = false);
    Task<IResult<IEnumerable<AuditTrailSlim>>> SearchAsync(string searchText);
    Task<IResult<int>> DeleteOld(CleanupTimeframe olderThan);
}