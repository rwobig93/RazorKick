using Application.Models.Identity;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Identity;
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
    Task<DatabaseActionResult<Guid>> AddExtendedAttributeAsync(AppUserExtendedAttributeAdd addAttribute);
    Task<DatabaseActionResult> UpdateExtendedAttributeAsync(Guid attributeId, string newValue);
    Task<DatabaseActionResult> RemoveExtendedAttributeAsync(Guid attributeId);
    Task<DatabaseActionResult<AppUserExtendedAttributeDb>> GetExtendedAttributeByIdAsync(Guid attributeId);
    Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetUserExtendedAttributesByTypeAsync(Guid userId, ExtendedAttributeType type);
    Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetUserExtendedAttributesByNameAsync(Guid userId, string name);
    Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllUserExtendedAttributesAsync(Guid userId);
    Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesByTypeAsync(ExtendedAttributeType type);
    Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesByNameAsync(string name);
    Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesAsync();
}