using Application.Models.Identity;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Database;
using Domain.Models.Identity;

namespace Application.Repositories.Identity;

public interface IAppUserRepository
{
    Task<DatabaseActionResult<IEnumerable<AppUserDb>>> GetAllAsync();
    Task<DatabaseActionResult<int>> GetCountAsync();
    Task<DatabaseActionResult<AppUserDb>> GetByIdAsync(Guid id);
    Task<DatabaseActionResult<AppUserFull>> GetByIdFullAsync(Guid id);
    Task<DatabaseActionResult<AppUserDb>> GetByUsernameAsync(string username);
    Task<DatabaseActionResult<AppUserDb>> GetByNormalizedUsernameAsync(string normalizedUsername);
    Task<DatabaseActionResult<AppUserDb>> GetByEmailAsync(string email);
    Task<DatabaseActionResult<AppUserDb>> GetByNormalizedEmailAsync(string normalizedEmail);
    Task<DatabaseActionResult> UpdateAsync(AppUserUpdate updateObject);
    Task<DatabaseActionResult> DeleteAsync(Guid id);
    Task<DatabaseActionResult<Guid>> CreateAsync(AppUserCreate createObject);
    Task<DatabaseActionResult<bool>> IsInRoleAsync(Guid userId, Guid roleId);
    Task<DatabaseActionResult> AddToRoleAsync(Guid userId, Guid roleId);
    Task<DatabaseActionResult> RemoveFromRoleAsync(Guid userId, Guid roleId);
}