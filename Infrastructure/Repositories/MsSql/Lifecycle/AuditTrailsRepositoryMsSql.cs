using Application.Models.Identity;
using Application.Repositories.Lifecycle;
using Domain.DatabaseEntities.Lifecycle;
using Domain.Models.Database;

namespace Infrastructure.Repositories.MsSql.Lifecycle;

public class AuditTrailsRepositoryMsSql : IAuditTrailsRepository
{
    public async Task<DatabaseActionResult<IEnumerable<AuditTrailDb>>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<DatabaseActionResult<IEnumerable<AuditTrailDb>>> GetAllPaginatedAsync(int offset, int pageSize)
    {
        throw new NotImplementedException();
    }

    public async Task<DatabaseActionResult<int>> GetCountAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<DatabaseActionResult<AuditTrailDb>> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<DatabaseActionResult<Guid>> CreateAsync(AppUserCreate createObject)
    {
        throw new NotImplementedException();
    }

    public async Task<DatabaseActionResult<IEnumerable<AuditTrailDb>>> SearchAsync(string searchText)
    {
        throw new NotImplementedException();
    }
}