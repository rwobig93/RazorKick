﻿using Application.Database.MsSql.Identity;
using Application.Database.MsSql.Shared;
using Application.Models.Identity;
using Application.Repositories.Identity;
using Application.Services.Database;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Identity;
using Domain.Models.Database;
using Domain.Models.Identity;

namespace Infrastructure.Repositories.Identity;

public class AppUserRepository : IAppUserRepository
{

    private readonly ISqlDataService _database;

    public AppUserRepository(ISqlDataService database)
    {
        _database = database;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserDb>>> GetAllAsync()
    {
        DatabaseActionResult<IEnumerable<AppUserDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.LoadData<AppUserDb, dynamic>(AppUsers.GetAll, new { });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<int>> GetCountAsync()
    {
        DatabaseActionResult<int> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<int, dynamic>(
                General.GetRowCount, new {TableName = "Users"})).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserDb>> GetByIdAsync(Guid userId)
    {
        DatabaseActionResult<AppUserDb> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<AppUserDb, dynamic>(
                AppUsers.GetById, new {Id = userId})).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }
    
    public async Task<DatabaseActionResult<AppUserFull>> GetByIdFullAsync(Guid userId)
    {
        DatabaseActionResult<AppUserFull> actionReturn = new ();
        
        try
        {
            var foundUser = (await _database.LoadData<AppUserDb, dynamic>(AppUsers.GetById, new {Id = userId})).FirstOrDefault();

            var fullUser = foundUser!.ToFullObject();
        
            // TODO: Add roles and extended attributes after both have been fully implemented
            // var foundRoles = await _database.LoadData<AppRoleDb, dynamic>(AppRoles.)
            
            actionReturn.Result = fullUser;
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserDb>> GetByUsernameAsync(string username)
    {
        DatabaseActionResult<AppUserDb> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<AppUserDb, dynamic>(
                AppUsers.GetByUsername, new {Username = username})).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserDb>> GetByNormalizedUsernameAsync(string normalizedUsername)
    {
        DatabaseActionResult<AppUserDb> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<AppUserDb, dynamic>(
                AppUsers.GetByNormalizedUsername, new {NormalizedUsername = normalizedUsername})).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserDb>> GetByEmailAsync(string email)
    {
        DatabaseActionResult<AppUserDb> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<AppUserDb, dynamic>(
                AppUsers.GetByEmail, new {Email = email})).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserDb>> GetByNormalizedEmailAsync(string normalizedEmail)
    {
        DatabaseActionResult<AppUserDb> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<AppUserDb, dynamic>(
                AppUsers.GetByNormalizedEmail, new {NormalizedEmail = normalizedEmail})).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(AppUserUpdate updateObject)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(AppUsers.Update, updateObject);
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid userId)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(AppUsers.Delete, new { Id = userId });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid>> CreateAsync(AppUserCreate createObject)
    {
        DatabaseActionResult<Guid> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.SaveDataReturnId(AppUsers.Insert, createObject);
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }
    
    public async Task<DatabaseActionResult<bool>> IsInRoleAsync(Guid userId, Guid roleId)
    {
        DatabaseActionResult<bool> actionReturn = new ();
        
        try
        {
            var foundMembership = await _database.LoadData<AppUserRoleJunctionDb, dynamic>(
                AppUserRoleJunctions.GetByUserRoleId, new { UserId = userId, RoleId = roleId });
        
            actionReturn.Result = foundMembership.FirstOrDefault() is not null;
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult> AddToRoleAsync(Guid userId, Guid roleId)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(AppUserRoleJunctions.Insert, new { UserId = userId, RoleId = roleId });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult> RemoveFromRoleAsync(Guid userId, Guid roleId)
    {
        DatabaseActionResult actionReturn = new();
        
        try
        {
            await _database.SaveData(AppUserRoleJunctions.Delete, new { UserId = userId, RoleId = roleId });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid>> AddExtendedAttributeAsync(AppUserExtendedAttributeAdd addAttribute)
    {
        DatabaseActionResult<Guid> actionReturn = new();
        
        try
        {
            actionReturn.Result = await _database.SaveDataReturnId(AppUserExtendedAttributes.Insert, addAttribute);
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateExtendedAttributeAsync(Guid attributeId, string newValue)
    {
        DatabaseActionResult actionReturn = new();
        
        try
        {
            await _database.SaveData(AppUserExtendedAttributes.Update, new { Id = attributeId, Value = newValue });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult> RemoveExtendedAttributeAsync(Guid attributeId)
    {
        DatabaseActionResult actionReturn = new();
        
        try
        {
            await _database.SaveData(AppUserExtendedAttributes.Delete, new { Id = attributeId });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<AppUserExtendedAttributeDb>> GetExtendedAttributeByIdAsync(Guid attributeId)
    {
        DatabaseActionResult<AppUserExtendedAttributeDb> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributes.GetById, new { Id = attributeId })).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetUserExtendedAttributesByTypeAsync(Guid userId, ExtendedAttributeType type)
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributes.GetAllOfTypeForOwner, new { OwnerId = userId, Type = type });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetUserExtendedAttributesByNameAsync(Guid userId, string name)
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributes.GetAllOfNameForOwner, new { OwnerId = userId, Name = name });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllUserExtendedAttributesAsync(Guid userId)
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributes.GetByOwnerId, new { OwnerId = userId });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesByTypeAsync(ExtendedAttributeType type)
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributes.GetAllOfType, new { Type = type });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesByNameAsync(string name)
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributes.GetByName, new { Name = name });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>>> GetAllExtendedAttributesAsync()
    {
        DatabaseActionResult<IEnumerable<AppUserExtendedAttributeDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.LoadData<AppUserExtendedAttributeDb, dynamic>(
                AppUserExtendedAttributes.GetAll, new { });
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.FailureOccurred = true;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }
}