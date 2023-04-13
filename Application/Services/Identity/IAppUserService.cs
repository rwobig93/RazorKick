using Application.Models.Identity;
using Application.Models.Web;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Identity;
using Domain.Models.Identity;

namespace Application.Services.Identity;

public interface IAppUserService
{
    Task<IResult<IEnumerable<AppUserSlim>>> GetAllAsync();
    Task<IResult<int>> GetCountAsync();
    Task<IResult<AppUserSlim>> GetByIdAsync(Guid userId);
    Task<IResult<AppUserFull>> GetByIdFullAsync(Guid userId);
    Task<IResult<AppUserSlim>> GetByUsernameAsync(string username);
    Task<IResult<AppUserFull>> GetByUsernameFullAsync(Guid userId);
    Task<IResult<AppUserSlim>> GetByEmailAsync(string email);
    Task<IResult<AppUserFull>> GetByEmailFullAsync(Guid userId);
    Task<IResult> UpdateAsync(AppUserUpdate updateObject);
    Task<IResult> DeleteAsync(Guid userId);
    Task<IResult<IEnumerable<AppUserSlim>>> SearchAsync(string searchText);
    Task<IResult<Guid>> CreateAsync(AppUserCreate createObject);
    Task<IResult<Guid>> AddExtendedAttributeAsync(AppUserExtendedAttributeAdd addAttribute);
    Task<IResult> UpdateExtendedAttributeAsync(Guid attributeId, string newValue);
    Task<IResult> RemoveExtendedAttributeAsync(Guid attributeId);
    Task<IResult> UpdatePreferences(Guid userId, AppUserPreferenceUpdate preferenceUpdate);
    Task<IResult<AppUserPreferenceFull>> GetPreferences(Guid userId);
    Task<IResult<AppUserExtendedAttributeDb>> GetExtendedAttributeByIdAsync(Guid attributeId);
    Task<IResult<IEnumerable<AppUserExtendedAttributeDb>>> GetUserExtendedAttributesByTypeAsync(Guid userId, ExtendedAttributeType type);
    Task<IResult<IEnumerable<AppUserExtendedAttributeDb>>> GetUserExtendedAttributesByNameAsync(Guid userId, string name);
    Task<IResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllUserExtendedAttributesAsync(Guid userId);
    Task<IResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesByTypeAsync(ExtendedAttributeType type);
    Task<IResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesByNameAsync(string name);
    Task<IResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesAsync();
}