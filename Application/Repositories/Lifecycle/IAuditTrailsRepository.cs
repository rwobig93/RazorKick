using Application.Models.Identity;
using Domain.DatabaseEntities.Lifecycle;
using Domain.Models.Database;

namespace Application.Repositories.Lifecycle;

public interface IAuditTrailsRepository
{
    Task<DatabaseActionResult<IEnumerable<AuditTrailDb>>> GetAllAsync();
    Task<DatabaseActionResult<IEnumerable<AuditTrailDb>>> GetAllPaginatedAsync(int offset, int pageSize);
    Task<DatabaseActionResult<int>> GetCountAsync();
    Task<DatabaseActionResult<AuditTrailDb>> GetByIdAsync(Guid id);
    Task<DatabaseActionResult<Guid>> CreateAsync(AppUserCreate createObject);
    Task<DatabaseActionResult<IEnumerable<AuditTrailDb>>> SearchAsync(string searchText);
}