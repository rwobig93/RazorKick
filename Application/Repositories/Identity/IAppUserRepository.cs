using Application.Models.Identity;
using Application.Models.Identity.User;
using Application.Models.Identity.UserExtensions;
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
    Task<DatabaseActionResult<AppUserFullDb>> GetByIdFullAsync(Guid id);
    Task<DatabaseActionResult<AppUserDb>> GetByUsernameAsync(string username);
    Task<DatabaseActionResult<AppUserFullDb>> GetByUsernameFullAsync(string username);
    Task<DatabaseActionResult<AppUserDb>> GetByEmailAsync(string email);
    Task<DatabaseActionResult<AppUserFullDb>> GetByEmailFullAsync(string email);
    Task<DatabaseActionResult<Guid>> CreateAsync(AppUserCreate createObject, bool systemUpdate = false);
    Task<DatabaseActionResult> UpdateAsync(AppUserUpdate updateObject, bool systemUpdate = false);
    Task<DatabaseActionResult> DeleteAsync(Guid id);
    Task<DatabaseActionResult<Guid>> SetUserId(Guid currentId, Guid newId);
    Task<DatabaseActionResult> SetCreatedById(Guid userId, Guid createdById);
    Task<DatabaseActionResult<IEnumerable<AppUserDb>>> SearchAsync(string searchText);
    Task<DatabaseActionResult<Guid>> AddExtendedAttributeAsync(AppUserExtendedAttributeCreate addAttribute);
    Task<DatabaseActionResult> UpdateExtendedAttributeAsync(Guid attributeId, string newValue);
    Task<DatabaseActionResult> RemoveExtendedAttributeAsync(Guid attributeId);
    Task<DatabaseActionResult> UpdatePreferences(Guid userId, AppUserPreferenceUpdate preferenceUpdate);
    Task<DatabaseActionResult<AppUserPreferenceDb>> GetPreferences(Guid userId);
    Task<DatabaseActionResult<AppUserExtendedAttributeDb>> GetExtendedAttributeByIdAsync(Guid attributeId);
    Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetUserExtendedAttributesByTypeAsync(Guid userId, ExtendedAttributeType type);
    Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetUserExtendedAttributesByNameAsync(Guid userId, string name);
    Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllUserExtendedAttributesAsync(Guid userId);
    Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesByTypeAsync(ExtendedAttributeType type);
    Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesByNameAsync(string name);
    Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesAsync();
    Task<DatabaseActionResult<Guid>> CreateSecurityAsync(AppUserSecurityAttributeCreate securityCreate);
    Task<DatabaseActionResult<AppUserSecurityAttributeDb>> GetSecurityAsync(Guid userId);
    Task<DatabaseActionResult> UpdateSecurityAsync(Guid userId, AppUserSecurityAttributeUpdate securityUpdate);
}