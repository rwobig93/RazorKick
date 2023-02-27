using Application.Constants.Messages;
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
        
        app.MapGet("/api/identity/permissions/user", GetPermissionsForUser).ApiVersionOne();
        app.MapPost("/api/identity/permission/user/add", AddUserToPermission).ApiVersionOne();
        app.MapPost("/api/identity/permission/user/remove", RemoveUserFromPermission).ApiVersionOne();
        app.MapPost("/api/identity/permission/user/has", UserHasPermission).ApiVersionOne();
        
        app.MapGet("/api/identity/permissions/role", GetPermissionsForRole).ApiVersionOne();
        app.MapPost("/api/identity/permission/role/add", AddRoleToPermission).ApiVersionOne();
        app.MapPost("/api/identity/permission/role/remove", RemoveRoleFromPermission).ApiVersionOne();
        app.MapPost("/api/identity/permission/role/has", RoleHasPermission).ApiVersionOne();
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

    private static async Task<IResult<bool>> UserHasPermission([FromQuery]Guid userId, [FromQuery]string permissionValue, IAppPermissionRepository 
    repository)
    {
        try
        {
            var permissionResponse = await repository.UserHasDirectPermission(userId, permissionValue);
            if (!permissionResponse.Success) return await Result<bool>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            return await Result<bool>.SuccessAsync(permissionResponse.Result);
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<bool>> RoleHasPermission([FromQuery]Guid roleId, [FromQuery]string permissionValue, IAppPermissionRepository 
        repository)
    {
        try
        {
            var permissionResponse = await repository.RoleHasPermission(roleId, permissionValue);
            if (!permissionResponse.Success) return await Result<bool>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            return await Result<bool>.SuccessAsync(permissionResponse.Result);
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> AddUserToPermission(PermissionCreateForUserRequest newPermission, IAppPermissionRepository repository)
    {
        try
        {
            var convertedRequest = newPermission.ToCreate();
            var permissionExists = (await repository.UserHasDirectPermission(convertedRequest.UserId, convertedRequest.ClaimValue)).Result;
            if (permissionExists)
                return await Result<bool>.FailAsync("Permission already exists for the provided user");
            
            var permissionResponse = await repository.CreateAsync(convertedRequest);
            if (!permissionResponse.Success) return await Result<bool>.FailAsync(permissionResponse.ErrorMessage);
            
            return await Result<bool>.SuccessAsync("Successfully added user to permission!");
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> RemoveUserFromPermission(PermissionRemoveFromUserRequest removeRequest, IAppPermissionRepository 
    repository)
    {
        try
        {
            var foundPermission = await repository.GetByUserIdAndValueAsync(
                removeRequest.UserId, removeRequest.PermissionValue);
            if (!foundPermission.Success)
                return await Result<bool>.FailAsync("Permission doesn't exist for the provided user");
            
            var permissionResponse = await repository.DeleteAsync(foundPermission.Result!.Id);
            if (!permissionResponse.Success) return await Result<bool>.FailAsync(permissionResponse.ErrorMessage);
            
            return await Result<bool>.SuccessAsync("Successfully removed user from permission!");
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<List<PermissionResponse>>> GetPermissionsForUser(Guid userId, IAppPermissionRepository repository)
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

    private static async Task<IResult> AddRoleToPermission(PermissionCreateForRoleRequest newPermission, IAppPermissionRepository repository)
    {
        try
        {
            var convertedRequest = newPermission.ToCreate();
            var permissionExists = (await repository.RoleHasPermission(convertedRequest.RoleId, convertedRequest.ClaimValue)).Result;
            if (permissionExists)
                return await Result<bool>.FailAsync("Permission already exists for the provided role");
            
            var permissionResponse = await repository.CreateAsync(convertedRequest);
            if (!permissionResponse.Success) return await Result<bool>.FailAsync(permissionResponse.ErrorMessage);
            
            return await Result<bool>.SuccessAsync("Successfully added role to permission!");
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> RemoveRoleFromPermission(PermissionRemoveFromRoleRequest removeRequest, IAppPermissionRepository 
        repository)
    {
        try
        {
            var foundPermission = await repository.GetByRoleIdAndValueAsync(
                removeRequest.RoleId, removeRequest.PermissionValue);
            if (!foundPermission.Success)
                return await Result<bool>.FailAsync("Permission doesn't exist for the provided role");
            
            var permissionResponse = await repository.DeleteAsync(foundPermission.Result!.Id);
            if (!permissionResponse.Success) return await Result<bool>.FailAsync(permissionResponse.ErrorMessage);
            
            return await Result<bool>.SuccessAsync("Successfully removed role from permission!");
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<List<PermissionResponse>>> GetPermissionsForRole(Guid roleId, IAppPermissionRepository repository)
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