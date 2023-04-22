using Application.Models.Identity;
using Application.Models.Web;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Application.Services.System;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Identity;
using Domain.Models.Identity;

namespace Infrastructure.Services.Identity;

public class AppUserService : IAppUserService
{
    private readonly IAppUserRepository _userRepository;
    private readonly IAppRoleRepository _roleRepository;
    private readonly IAppPermissionRepository _permissionRepository;
    private readonly ISerializerService _serializer;

    public AppUserService(IAppUserRepository userRepository, IAppRoleRepository roleRepository, IAppPermissionRepository permissionRepository,
        ISerializerService serializer)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _serializer = serializer;
    }

    private async Task<Result<AppUserFull?>> ConvertToFullAsync(AppUserDb userDb)
    {
        var fullUser = userDb.ToFull();
        
        var userRoles = await _roleRepository.GetRolesForUser(userDb.Id);
        if (!userRoles.Success)
            return await Result<AppUserFull?>.FailAsync(userRoles.ErrorMessage);

        fullUser.Roles = (userRoles.Result?.ToSlims() ?? new List<AppRoleSlim>())
            .OrderBy(x => x.Name)
            .ToList();

        var foundAttributes = await _userRepository.GetAllUserExtendedAttributesAsync(userDb.Id);
        if (!foundAttributes.Success)
            return await Result<AppUserFull?>.FailAsync(foundAttributes.ErrorMessage);

        fullUser.ExtendedAttributes = (foundAttributes.Result?.ToSlims() ?? new List<AppUserExtendedAttributeSlim>())
            .OrderBy(x => x.Type)
            .ThenBy(x => x.Name)
            .ThenBy(x => x.Value)
            .ToList();

        var foundPermissions = await _permissionRepository.GetAllDirectForUserAsync(userDb.Id);
        if (!foundPermissions.Success)
            return await Result<AppUserFull?>.FailAsync(foundPermissions.ErrorMessage);

        fullUser.Permissions = (foundPermissions.Result?.ToSlims() ?? new List<AppPermissionSlim>())
            .OrderBy(x => x.Group)
            .ThenBy(x => x.Name)
            .ThenBy(x => x.Access)
            .ToList();

        return await Result<AppUserFull?>.SuccessAsync(fullUser);
    }

    private async Task<Result<AppUserFull?>> ConvertToFullAsync(AppUserFullDb userFullDb)
    {
        var fullUser = userFullDb.ToFull();
        
        fullUser.Roles = userFullDb.Roles.ToSlims()
            .OrderBy(x => x.Name)
            .ToList();

        fullUser.ExtendedAttributes = userFullDb.ExtendedAttributes.ToSlims()
            .OrderBy(x => x.Type)
            .ThenBy(x => x.Name)
            .ThenBy(x => x.Value)
            .ToList();

        fullUser.Permissions = userFullDb.Permissions.ToSlims()
            .OrderBy(x => x.Group)
            .ThenBy(x => x.Name)
            .ThenBy(x => x.Access)
            .ToList();

        return await Result<AppUserFull?>.SuccessAsync(fullUser);
    }

    public async Task<IResult<IEnumerable<AppUserSlim>>> GetAllAsync()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            if (!users.Success)
                return await Result<IEnumerable<AppUserSlim>>.FailAsync(users.ErrorMessage);

            return await Result<IEnumerable<AppUserSlim>>.SuccessAsync(users.Result!.ToSlims());
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
            var userCount = await _userRepository.GetCountAsync();
            if (!userCount.Success)
                return await Result<int>.FailAsync(userCount.ErrorMessage);

            return await Result<int>.SuccessAsync(userCount.Result);
        }
        catch (Exception ex)
        {
            return await Result<int>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppUserSlim?>> GetByIdAsync(Guid userId)
    {
        try
        {
            var foundUser = await _userRepository.GetByIdAsync(userId);
            if (!foundUser.Success)
                return await Result<AppUserSlim?>.FailAsync(foundUser.ErrorMessage);

            return await Result<AppUserSlim?>.SuccessAsync(foundUser.Result?.ToSlim());
        }
        catch (Exception ex)
        {
            return await Result<AppUserSlim?>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppUserFull?>> GetByIdFullAsync(Guid userId)
    {
        try
        {
            // var foundUser = await _userRepository.GetByIdAsync(userId);
            // if (!foundUser.Success)
            //     return await Result<AppUserFull?>.FailAsync(foundUser.ErrorMessage);
            //
            // if (foundUser.Result is null)
            //     return await Result<AppUserFull?>.FailAsync(foundUser.Result?.ToFull());
            //
            // return await ConvertToFullAsync(foundUser.Result);
            
            // TODO: Validate functionality before removing old implementation
            var foundUser = await _userRepository.GetByIdFullAsync(userId);
            if (!foundUser.Success)
                return await Result<AppUserFull?>.FailAsync(foundUser.ErrorMessage);

            if (foundUser.Result is null)
                return await Result<AppUserFull?>.FailAsync(foundUser.Result?.ToFull());

            return await ConvertToFullAsync(foundUser.Result);
        }
        catch (Exception ex)
        {
            return await Result<AppUserFull?>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppUserSlim?>> GetByUsernameAsync(string username)
    {
        try
        {
            var foundUser = await _userRepository.GetByUsernameAsync(username);
            if (!foundUser.Success)
                return await Result<AppUserSlim?>.FailAsync(foundUser.ErrorMessage);

            return await Result<AppUserSlim?>.SuccessAsync(foundUser.Result?.ToSlim());
        }
        catch (Exception ex)
        {
            return await Result<AppUserSlim?>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppUserFull?>> GetByUsernameFullAsync(string username)
    {
        try
        {
            var foundUser = await _userRepository.GetByUsernameAsync(username);
            if (!foundUser.Success)
                return await Result<AppUserFull?>.FailAsync(foundUser.ErrorMessage);

            if (foundUser.Result is null)
                return await Result<AppUserFull?>.FailAsync(foundUser.Result?.ToFull());

            return await ConvertToFullAsync(foundUser.Result);
        }
        catch (Exception ex)
        {
            return await Result<AppUserFull?>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppUserSlim?>> GetByEmailAsync(string email)
    {
        try
        {
            var foundUser = await _userRepository.GetByEmailAsync(email);
            if (!foundUser.Success)
                return await Result<AppUserSlim?>.FailAsync(foundUser.ErrorMessage);

            return await Result<AppUserSlim?>.SuccessAsync(foundUser.Result?.ToSlim());
        }
        catch (Exception ex)
        {
            return await Result<AppUserSlim?>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppUserFull?>> GetByEmailFullAsync(string email)
    {
        try
        {
            var foundUser = await _userRepository.GetByEmailAsync(email);
            if (!foundUser.Success)
                return await Result<AppUserFull?>.FailAsync(foundUser.ErrorMessage);

            if (foundUser.Result is null)
                return await Result<AppUserFull?>.FailAsync(foundUser.Result?.ToFull());

            return await ConvertToFullAsync(foundUser.Result);
        }
        catch (Exception ex)
        {
            return await Result<AppUserFull?>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> UpdateAsync(AppUserUpdate updateObject, bool systemUpdate = false)
    {
        try
        {
            var updateUser = await _userRepository.UpdateAsync(updateObject);
            if (!updateUser.Success)
                return await Result.FailAsync(updateUser.ErrorMessage);

            return await Result.SuccessAsync();
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> DeleteAsync(Guid userId)
    {
        try
        {
            var deleteUser = await _userRepository.DeleteAsync(userId);
            if (!deleteUser.Success)
                return await Result.FailAsync(deleteUser.ErrorMessage);

            return await Result.SuccessAsync();
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
            var searchResult = await _userRepository.SearchAsync(searchText);
            if (!searchResult.Success)
                return await Result<IEnumerable<AppUserSlim>>.FailAsync(searchResult.ErrorMessage);

            var results = (searchResult.Result?.ToSlims() ?? new List<AppUserSlim>())
                .OrderBy(x => x.Username);

            return await Result<IEnumerable<AppUserSlim>>.SuccessAsync(results);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<Guid>> CreateAsync(AppUserCreate createObject, bool systemUpdate = false)
    {
        try
        {
            var createUser = await _userRepository.CreateAsync(createObject);
            if (!createUser.Success)
                return await Result<Guid>.FailAsync(createUser.ErrorMessage);

            return await Result<Guid>.SuccessAsync(createUser.Result);
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
            var addRequest = await _userRepository.AddExtendedAttributeAsync(addAttribute);
            if (!addRequest.Success)
                return await Result<Guid>.FailAsync(addRequest.ErrorMessage);

            return await Result<Guid>.SuccessAsync(addRequest.Result);
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
            var updateRequest = await _userRepository.UpdateExtendedAttributeAsync(attributeId, newValue);
            if (!updateRequest.Success)
                return await Result.FailAsync(updateRequest.ErrorMessage);

            return await Result.SuccessAsync();
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
            var removeRequest = await _userRepository.RemoveExtendedAttributeAsync(attributeId);
            if (!removeRequest.Success)
                return await Result.FailAsync(removeRequest.ErrorMessage);

            return await Result.SuccessAsync();
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
            var updateRequest = await _userRepository.UpdatePreferences(userId, preferenceUpdate);
            if (!updateRequest.Success)
                return await Result.FailAsync(updateRequest.ErrorMessage);

            return await Result.SuccessAsync();
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppUserPreferenceFull?>> GetPreferences(Guid userId)
    {
        try
        {
            var preferenceRequest = await _userRepository.GetPreferences(userId);
            if (!preferenceRequest.Success)
                return await Result<AppUserPreferenceFull?>.FailAsync(preferenceRequest.ErrorMessage);

            var preferencesFull = preferenceRequest.Result?.ToFull();
            if (preferencesFull is null)
                return await Result<AppUserPreferenceFull?>.FailAsync(preferencesFull);
            
            preferencesFull.CustomThemeOne = _serializer.Deserialize<AppThemeCustom>(preferenceRequest.Result!.CustomThemeOne!);
            preferencesFull.CustomThemeTwo = _serializer.Deserialize<AppThemeCustom>(preferenceRequest.Result!.CustomThemeTwo!);
            preferencesFull.CustomThemeThree = _serializer.Deserialize<AppThemeCustom>(preferenceRequest.Result!.CustomThemeThree!);

            return await Result<AppUserPreferenceFull?>.SuccessAsync(preferencesFull);
        }
        catch (Exception ex)
        {
            return await Result<AppUserPreferenceFull?>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AppUserExtendedAttributeSlim?>> GetExtendedAttributeByIdAsync(Guid attributeId)
    {
        try
        {
            var getRequest = await _userRepository.GetExtendedAttributeByIdAsync(attributeId);
            if (!getRequest.Success)
                return await Result<AppUserExtendedAttributeSlim?>.FailAsync(getRequest.ErrorMessage);

            return await Result<AppUserExtendedAttributeSlim?>.SuccessAsync(getRequest.Result?.ToSlim());
        }
        catch (Exception ex)
        {
            return await Result<AppUserExtendedAttributeSlim?>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppUserExtendedAttributeSlim>>> GetUserExtendedAttributesByTypeAsync(Guid userId, ExtendedAttributeType type)
    {
        try
        {
            var getRequest = await _userRepository.GetUserExtendedAttributesByTypeAsync(userId, type);
            if (!getRequest.Success)
                return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.FailAsync(getRequest.ErrorMessage);

            var attributes = getRequest.Result?.ToSlims() ?? new List<AppUserExtendedAttributeSlim>();

            return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.SuccessAsync(attributes);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppUserExtendedAttributeSlim>>> GetUserExtendedAttributesByNameAsync(Guid userId, string name)
    {
        try
        {
            var getRequest = await _userRepository.GetUserExtendedAttributesByNameAsync(userId, name);
            if (!getRequest.Success)
                return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.FailAsync(getRequest.ErrorMessage);

            var attributes = getRequest.Result?.ToSlims() ?? new List<AppUserExtendedAttributeSlim>();

            return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.SuccessAsync(attributes);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppUserExtendedAttributeSlim>>> GetAllUserExtendedAttributesAsync(Guid userId)
    {
        try
        {
            var getRequest = await _userRepository.GetAllUserExtendedAttributesAsync(userId);
            if (!getRequest.Success)
                return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.FailAsync(getRequest.ErrorMessage);

            var attributes = getRequest.Result?.ToSlims() ?? new List<AppUserExtendedAttributeSlim>();

            return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.SuccessAsync(attributes);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppUserExtendedAttributeSlim>>> GetAllExtendedAttributesByTypeAsync(ExtendedAttributeType type)
    {
        try
        {
            var getRequest = await _userRepository.GetAllExtendedAttributesByTypeAsync(type);
            if (!getRequest.Success)
                return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.FailAsync(getRequest.ErrorMessage);

            var attributes = getRequest.Result?.ToSlims() ?? new List<AppUserExtendedAttributeSlim>();

            return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.SuccessAsync(attributes);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppUserExtendedAttributeSlim>>> GetAllExtendedAttributesByNameAsync(string name)
    {
        try
        {
            var getRequest = await _userRepository.GetAllExtendedAttributesByNameAsync(name);
            if (!getRequest.Success)
                return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.FailAsync(getRequest.ErrorMessage);

            var attributes = getRequest.Result?.ToSlims() ?? new List<AppUserExtendedAttributeSlim>();

            return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.SuccessAsync(attributes);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AppUserExtendedAttributeSlim>>> GetAllExtendedAttributesAsync()
    {
        try
        {
            var getRequest = await _userRepository.GetAllExtendedAttributesAsync();
            if (!getRequest.Success)
                return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.FailAsync(getRequest.ErrorMessage);

            var attributes = getRequest.Result?.ToSlims() ?? new List<AppUserExtendedAttributeSlim>();

            return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.SuccessAsync(attributes);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AppUserExtendedAttributeSlim>>.FailAsync(ex.Message);
        }
    }
}