using Application.Models.Identity;
using Application.Models.Web;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Identity;
using Domain.Models.Identity;

namespace Infrastructure.Services.Identity;

public class AppUserService : IAppUserService
{
    private readonly IAppUserRepository _userRepository;
    private readonly IAppRoleRepository _roleRepository;
    private readonly IAppPermissionRepository _permissionRepository;

    public AppUserService(IAppUserRepository userRepository, IAppRoleRepository roleRepository, IAppPermissionRepository permissionRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<IResult<IEnumerable<AppUserSlim>>> GetAllAsync()
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<int>> GetCountAsync()
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<int>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppUserSlim>> GetByIdAsync(Guid id)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<AppUserSlim>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppUserFull>> GetByIdFullAsync(Guid userId)
    {
        try
        {
            var foundUser = await _userRepository.GetByIdAsync(userId);
            if (!foundUser.Success)
                return await Result<AppUserFull>.FailAsync(foundUser.ErrorMessage);

            var fullUser = foundUser.Result!.ToFull();

            var userRoles = await _roleRepository.GetRolesForUser(userId);
            if (!userRoles.Success)
                return await Result<AppUserFull>.FailAsync(userRoles.ErrorMessage);
            
            fullUser.Roles = (userRoles.Result?.ToSlims() ?? new List<AppRoleSlim>())
                .OrderBy(x => x.Name)
                .ToList();

            var foundAttributes = await _userRepository.GetAllUserExtendedAttributesAsync(userId);
            if (!foundAttributes.Success)
                return await Result<AppUserFull>.FailAsync(foundAttributes.ErrorMessage);

            fullUser.ExtendedAttributes = (foundAttributes.Result?.ToSlims() ?? new List<AppUserExtendedAttributeSlim>())
                .OrderBy(x => x.Type)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Value)
                .ToList();

            var foundPermissions = await _permissionRepository.GetAllDirectForUserAsync(userId);
            if (!foundPermissions.Success)
                return await Result<AppUserFull>.FailAsync(foundPermissions.ErrorMessage);
            
            fullUser.Permissions = (foundPermissions.Result?.ToSlims() ?? new List<AppPermissionSlim>())
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Access)
                .ToList();

            return await Result<AppUserFull>.SuccessAsync(fullUser);
        }
        catch (Exception ex)
        {
            return await Result<AppUserFull>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppUserSlim>> GetByUsernameAsync(string username)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<AppUserSlim>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppUserFull>> GetByUsernameFullAsync(Guid id)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<AppUserFull>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppUserSlim>> GetByEmailAsync(string email)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<AppUserSlim>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppUserFull>> GetByEmailFullAsync(Guid id)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<AppUserFull>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> UpdateAsync(AppUserUpdate updateObject)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppUserSlim>>> SearchAsync(string searchText)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<Guid>> CreateAsync(AppUserCreate createObject)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<Guid>> AddExtendedAttributeAsync(AppUserExtendedAttributeAdd addAttribute)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> UpdateExtendedAttributeAsync(Guid attributeId, string newValue)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> RemoveExtendedAttributeAsync(Guid attributeId)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> UpdatePreferences(Guid userId, AppUserPreferenceUpdate preferenceUpdate)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppUserPreferenceFull>> GetPreferences(Guid userId)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<AppUserPreferenceFull>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppUserExtendedAttributeDb>> GetExtendedAttributeByIdAsync(Guid attributeId)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<AppUserExtendedAttributeDb>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppUserExtendedAttributeDb>>> GetUserExtendedAttributesByTypeAsync(Guid userId, ExtendedAttributeType type)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserExtendedAttributeDb>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppUserExtendedAttributeDb>>> GetUserExtendedAttributesByNameAsync(Guid userId, string name)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserExtendedAttributeDb>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllUserExtendedAttributesAsync(Guid userId)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserExtendedAttributeDb>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesByTypeAsync(ExtendedAttributeType type)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserExtendedAttributeDb>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesByNameAsync(string name)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserExtendedAttributeDb>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesAsync()
    {
        try
        {

        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserExtendedAttributeDb>>.FailAsync(ex.Message);
        }
    }
}