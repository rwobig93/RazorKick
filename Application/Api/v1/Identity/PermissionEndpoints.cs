using Application.Constants.Communication;
using Application.Helpers.Web;
using Application.Models.Identity;
using Application.Models.Web;
using Application.Repositories.Identity;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Requests.Identity.Permission;
using Shared.Responses.Identity;

namespace Application.Api.v1.Identity;

public static class PermissionEndpoints
{
    public static void MapEndpointsPermissions(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/permissions", GetAllPermissions).ApiVersionOne();
        app.MapGet("/api/identity/permission", GetPermission).ApiVersionOne();
        app.MapDelete("/api/identity/permission", DeletePermission).ApiVersionOne();
        app.MapPut("/api/identity/permission", UpdatePermission).ApiVersionOne();
        
        app.MapGet("/api/identity/permissions/user/direct", GetDirectPermissionsForUser).ApiVersionOne();
        app.MapGet("/api/identity/permissions/user/all", GetAllPermissionsForUser).ApiVersionOne();
        app.MapPost("/api/identity/permission/user/add", AddPermissionToUser).ApiVersionOne();
        app.MapPost("/api/identity/permission/user/remove", RemovePermissionFromUser).ApiVersionOne();
        app.MapGet("/api/identity/permission/user/has", DoesUserHavePermission).ApiVersionOne();
        
        app.MapGet("/api/identity/permission/role", GetAllPermissionsForRole).ApiVersionOne();
        app.MapPost("/api/identity/permission/role/add", AddPermissionToRole).ApiVersionOne();
        app.MapPost("/api/identity/permission/role/remove", RemovePermissionFromRole).ApiVersionOne();
        app.MapPost("/api/identity/permission/role/has", DoesRoleHavePermission).ApiVersionOne();
    }
    
    public static async Task<IResult<List<PermissionResponse>>> GetAllPermissions(IAppPermissionRepository repository)
    {
        try
        {
            var allPermissions = await repository.GetAllAsync();
            if (!allPermissions.Success)
                return await Result<List<PermissionResponse>>.FailAsync(allPermissions.ErrorMessage);

            return await Result<List<PermissionResponse>>.SuccessAsync(allPermissions.Result!.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<PermissionResponse>>.FailAsync(ex.Message);
        }
    }
    
    public static async Task<IResult<PermissionResponse>> GetPermission(Guid permissionId, IAppPermissionRepository repository)
    {
        try
        {
            var permission = await repository.GetByIdAsync(permissionId);
            if (!permission.Success)
                return await Result<PermissionResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<PermissionResponse>.SuccessAsync(permission.Result!.ToResponse());
        }
        catch (Exception ex)
        {
            return await Result<PermissionResponse>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> AddPermissionToRole(PermissionCreateForRoleRequest permissionRequest, IAppPermissionRepository repository)
    {
        try
        {
            var createRequest = permissionRequest.ToCreate();
            createRequest.CreatedBy = Guid.Empty;
            
            var result = await repository.CreateAsync(createRequest);
            if (!result.Success)
                return await Result.FailAsync(result.ErrorMessage);
            
            return await Result.SuccessAsync("Successfully created permission!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> AddPermissionToUser(PermissionCreateForUserRequest permissionRequest, IAppPermissionRepository repository)
    {
        try
        {
            var createRequest = permissionRequest.ToCreate();
            createRequest.CreatedBy = Guid.Empty;
            
            var result = await repository.CreateAsync(createRequest);
            if (!result.Success)
                return await Result.FailAsync(result.ErrorMessage);
            
            return await Result.SuccessAsync("Successfully created permission!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> UpdatePermission(PermissionUpdateRequest permissionRequest, IAppPermissionRepository repository)
    {
        try
        {
            var permissionResponse = (await repository.GetByIdAsync(permissionRequest.Id)).Result;
            if (permissionResponse is null) return await Result.FailAsync(ErrorMessageConstants.UserNotFoundError);
            
            await repository.UpdateAsync(permissionRequest.ToUpdate());
            return await Result.SuccessAsync("Successfully updated permission!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> DeletePermission(Guid permissionId, IAppPermissionRepository repository)
    {
        try
        {
            var permissionResponse = (await repository.GetByIdAsync(permissionId)).Result;
            if (permissionResponse is null) return await Result.FailAsync(ErrorMessageConstants.UserNotFoundError);
            
            await repository.DeleteAsync(permissionId);
            return await Result.SuccessAsync("Successfully deleted permission!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<bool>> DoesUserHavePermission([FromQuery]Guid userId, [FromQuery]Guid permissionId, 
    IAppPermissionRepository repository)
    {
        try
        {
            var foundPermission = await repository.GetByIdAsync(permissionId);
            if (foundPermission.Success)
                return await Result<bool>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            var permissionResponse = await repository.UserIncludingRolesHasPermission(userId, foundPermission.Result!.ClaimValue);
            if (!permissionResponse.Success) return await Result<bool>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            return await Result<bool>.SuccessAsync(permissionResponse.Result);
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<bool>> DoesRoleHavePermission([FromQuery]Guid roleId, [FromQuery]Guid permissionId, 
        IAppPermissionRepository repository)
    {
        try
        {
            var foundPermission = await repository.GetByIdAsync(permissionId);
            if (foundPermission.Success)
                return await Result<bool>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            var permissionResponse = await repository.RoleHasPermission(roleId, foundPermission.Result!.ClaimValue);
            if (!permissionResponse.Success) return await Result<bool>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            return await Result<bool>.SuccessAsync(permissionResponse.Result);
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> RemovePermissionFromUser(PermissionRemoveFromUserRequest permissionRequest, IAppPermissionRepository 
    repository)
    {
        try
        {
            var foundPermission =
                await repository.GetByUserIdAndValueAsync(permissionRequest.UserId, permissionRequest.PermissionValue);
            if (foundPermission.Success)
                return await Result<bool>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            var permissionResponse = await repository.DeleteAsync(foundPermission.Result!.Id);
            if (!permissionResponse.Success) return await Result<bool>.FailAsync(permissionResponse.ErrorMessage);
            
            return await Result<bool>.SuccessAsync("Successfully removes user from permission!");
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> RemovePermissionFromRole(PermissionRemoveFromRoleRequest permissionRequest, IAppPermissionRepository 
    repository)
    {
        try
        {
            var foundPermission =
                await repository.GetByRoleIdAndValueAsync(permissionRequest.RoleId, permissionRequest.PermissionValue);
            if (foundPermission.Success)
                return await Result<bool>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            var permissionResponse = await repository.DeleteAsync(foundPermission.Result!.Id);
            if (!permissionResponse.Success) return await Result<bool>.FailAsync(permissionResponse.ErrorMessage);
            
            return await Result<bool>.SuccessAsync("Successfully removes user from permission!");
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<List<PermissionResponse>>> GetDirectPermissionsForUser(Guid userId, IAppPermissionRepository repository)
    {
        try
        {
            var permissionResponse = await repository.GetAllDirectForUserAsync(userId);
            if (!permissionResponse.Success) return await Result<List<PermissionResponse>>.FailAsync(permissionResponse.ErrorMessage);
            
            return await Result<List<PermissionResponse>>.SuccessAsync(permissionResponse.Result!.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<PermissionResponse>>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<List<PermissionResponse>>> GetAllPermissionsForUser(Guid userId, IAppPermissionRepository repository)
    {
        try
        {
            var permissionResponse = await repository.GetAllIncludingRolesForUserAsync(userId);
            if (!permissionResponse.Success) return await Result<List<PermissionResponse>>.FailAsync(permissionResponse.ErrorMessage);
            
            return await Result<List<PermissionResponse>>.SuccessAsync(permissionResponse.Result!.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<PermissionResponse>>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<List<PermissionResponse>>> GetAllPermissionsForRole(Guid roleId, IAppPermissionRepository repository)
    {
        try
        {
            var permissionResponse = await repository.GetAllForRoleAsync(roleId);
            if (!permissionResponse.Success) return await Result<List<PermissionResponse>>.FailAsync(permissionResponse.ErrorMessage);
            
            return await Result<List<PermissionResponse>>.SuccessAsync(permissionResponse.Result!.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<PermissionResponse>>.FailAsync(ex.Message);
        }
    }
}