using Application.Constants.Messages;
using Application.Helpers.Web;
using Application.Models.Identity;
using Application.Models.Web;
using Application.Repositories.Identity;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Requests.Identity;
using Shared.Responses.Identity;

namespace Application.Api.v1.Identity;

public static class PermissionEndpoints
{
    public static void MapEndpointsPermissions(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/permissions", GetAllPermissions).ApiVersionOne();
        app.MapGet("/api/identity/permission", GetPermission).ApiVersionOne();
        app.MapDelete("/api/identity/permission", DeletePermission).ApiVersionOne();
        app.MapPost("/api/identity/permission", CreatePermission).ApiVersionOne();
        app.MapPut("/api/identity/permission", UpdatePermission).ApiVersionOne();
        
        app.MapGet("/api/identity/permissions/user", GetPermissionsForUser).ApiVersionOne();
        app.MapPost("/api/identity/permission/adduser", AddUserToPermission).ApiVersionOne();
        app.MapPost("/api/identity/permission/removeuser", RemoveUserFromPermission).ApiVersionOne();
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

    private static async Task<IResult> CreatePermission(CreatePermissionRequest permissionRequest, IAppPermissionRepository repository)
    {
        try
        {
            var createRequest = permissionRequest.ToCreateObject();
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

    private static async Task<IResult> UpdatePermission(UpdatePermissionRequest permissionRequest, IAppPermissionRepository repository)
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

    private static async Task<IResult<bool>> IsUserInPermission([FromQuery]Guid userId, [FromQuery]Guid permissionId, IAppPermissionRepository repository)
    {
        try
        {
            var permissionResponse = await repository.IsUserInPermissionAsync(userId, permissionId);
            if (!permissionResponse.Success) return await Result<bool>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            return await Result<bool>.SuccessAsync(permissionResponse.Result);
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> AddUserToPermission(Guid userId, Guid permissionId, IAppPermissionRepository repository)
    {
        try
        {
            var permissionResponse = await repository.AddUserToPermissionAsync(userId, permissionId);
            if (!permissionResponse.Success) return await Result<bool>.FailAsync(permissionResponse.ErrorMessage);
            
            return await Result<bool>.SuccessAsync("Successfully added user to permission!");
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> RemoveUserFromPermission(Guid userId, Guid permissionId, IAppPermissionRepository repository)
    {
        try
        {
            var permissionResponse = await repository.RemoveUserFromPermissionAsync(userId, permissionId);
            if (!permissionResponse.Success) return await Result<bool>.FailAsync(permissionResponse.ErrorMessage);
            
            return await Result<bool>.SuccessAsync("Successfully removes user from permission!");
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
            var permissionResponse = await repository.GetPermissionsForUser(userId);
            if (!permissionResponse.Success) return await Result<List<PermissionResponse>>.FailAsync(permissionResponse.ErrorMessage);
            
            return await Result<List<PermissionResponse>>.SuccessAsync(permissionResponse.Result!.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<PermissionResponse>>.FailAsync(ex.Message);
        }
    }
}