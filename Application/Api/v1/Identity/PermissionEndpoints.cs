using Application.Constants.Communication;
using Application.Constants.Web;
using Application.Helpers.Web;
using Application.Mappers.Identity;
using Application.Models.Web;
using Application.Requests.Identity.Permission;
using Application.Responses.Identity;
using Application.Services.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Application.Api.v1.Identity;

public static class PermissionEndpoints
{
    public static void MapEndpointsPermissions(this IEndpointRouteBuilder app)
    {
        // Permissions
        app.MapGet(ApiRouteConstants.Identity.Permission.GetAll, GetAllPermissions).ApiVersionOne();
        app.MapGet(ApiRouteConstants.Identity.Permission.GetById, GetPermission).ApiVersionOne();
        
        // Users
        app.MapGet(ApiRouteConstants.Identity.Permission.GetDirectPermissionsForUser, GetDirectPermissionsForUser).ApiVersionOne();
        app.MapGet(ApiRouteConstants.Identity.Permission.GetAllPermissionsForUser, GetAllPermissionsForUser).ApiVersionOne();
        app.MapPost(ApiRouteConstants.Identity.Permission.AddPermissionToUser, AddPermissionToUser).ApiVersionOne();
        app.MapPost(ApiRouteConstants.Identity.Permission.RemovePermissionFromUser, RemovePermissionFromUser).ApiVersionOne();
        app.MapGet(ApiRouteConstants.Identity.Permission.DoesUserHavePermission, DoesUserHavePermission).ApiVersionOne();
        
        // Roles
        app.MapGet(ApiRouteConstants.Identity.Permission.GetAllPermissionsForRole, GetAllPermissionsForRole).ApiVersionOne();
        app.MapPost(ApiRouteConstants.Identity.Permission.AddPermissionToRole, AddPermissionToRole).ApiVersionOne();
        app.MapPost(ApiRouteConstants.Identity.Permission.RemovePermissionFromRole, RemovePermissionFromRole).ApiVersionOne();
        app.MapPost(ApiRouteConstants.Identity.Permission.DoesRoleHavePermission, DoesRoleHavePermission).ApiVersionOne();
    }

    private static async Task<IResult<List<PermissionResponse>>> GetAllPermissions(IAppPermissionService permissionService)
    {
        try
        {
            var allPermissions = await permissionService.GetAllAsync();
            if (!allPermissions.Succeeded)
                return await Result<List<PermissionResponse>>.FailAsync(allPermissions.Messages);

            return await Result<List<PermissionResponse>>.SuccessAsync(allPermissions.Data.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<PermissionResponse>>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<PermissionResponse>> GetPermission([FromQuery]Guid permissionId, IAppPermissionService permissionService)
    {
        try
        {
            var foundPermission = await permissionService.GetByIdAsync(permissionId);
            if (!foundPermission.Succeeded)
                return await Result<PermissionResponse>.FailAsync(foundPermission.Messages);

            if (foundPermission.Data is null)
                return await Result<PermissionResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<PermissionResponse>.SuccessAsync(foundPermission.Data.ToResponse());
        }
        catch (Exception ex)
        {
            return await Result<PermissionResponse>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> AddPermissionToRole(PermissionCreateForRoleRequest permissionRequest, IAppPermissionService permissionService)
    {
        try
        {
            var addRequest = permissionRequest.ToCreate();
            
            return await permissionService.CreateAsync(addRequest);
        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<Guid>> AddPermissionToUser(PermissionCreateForUserRequest permissionRequest, IAppPermissionService 
    permissionService)
    {
        try
        {
            var addRequest = permissionRequest.ToCreate();
            
            return await permissionService.CreateAsync(addRequest);
        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<bool>> DoesUserHavePermission([FromQuery]Guid userId, [FromQuery]Guid permissionId, 
    IAppPermissionService permissionService)
    {
        try
        {
            var foundPermission = await permissionService.GetByIdAsync(permissionId);
            if (!foundPermission.Succeeded) return await Result<bool>.FailAsync(foundPermission.Messages);
            if (foundPermission.Data is null) return await Result<bool>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            return await permissionService.UserIncludingRolesHasPermission(userId, foundPermission.Data!.ClaimValue);
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<bool>> DoesRoleHavePermission([FromQuery]Guid roleId, [FromQuery]Guid permissionId, 
        IAppPermissionService permissionService)
    {
        try
        {
            var foundPermission = await permissionService.GetByIdAsync(permissionId);
            if (!foundPermission.Succeeded) return await Result<bool>.FailAsync(foundPermission.Messages);
            if (foundPermission.Data is null) return await Result<bool>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            return await permissionService.RoleHasPermission(roleId, foundPermission.Data!.ClaimValue);
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> RemovePermissionFromUser(PermissionRemoveFromUserRequest permissionRequest,
        IAppPermissionService permissionService)
    {
        try
        {
            var foundPermission =
                await permissionService.GetByUserIdAndValueAsync(permissionRequest.UserId, permissionRequest.PermissionValue);
            if (!foundPermission.Succeeded) return await Result.FailAsync(foundPermission.Messages);
            if (foundPermission.Data is null) return await Result.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            var permissionResponse = await permissionService.DeleteAsync(foundPermission.Data.Id);
            if (!permissionResponse.Succeeded) return permissionResponse;
            
            return await Result.SuccessAsync("Successfully removed permission from user!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> RemovePermissionFromRole(PermissionRemoveFromRoleRequest permissionRequest,
        IAppPermissionService permissionService)
    {
        try
        {
            var foundPermission =
                await permissionService.GetByUserIdAndValueAsync(permissionRequest.RoleId, permissionRequest.PermissionValue);
            if (!foundPermission.Succeeded) return await Result.FailAsync(foundPermission.Messages);
            if (foundPermission.Data is null) return await Result.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            var permissionResponse = await permissionService.DeleteAsync(foundPermission.Data.Id);
            if (!permissionResponse.Succeeded) return permissionResponse;

            return await Result<bool>.SuccessAsync("Successfully removed permission from role!");
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<List<PermissionResponse>>> GetDirectPermissionsForUser([FromQuery]Guid userId,
        IAppPermissionService permissionService)
    {
        try
        {
            var foundPermissions = await permissionService.GetAllDirectForUserAsync(userId);
            if (!foundPermissions.Succeeded)
                return await Result<List<PermissionResponse>>.FailAsync(foundPermissions.Messages);

            return await Result<List<PermissionResponse>>.SuccessAsync(foundPermissions.Data.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<PermissionResponse>>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<List<PermissionResponse>>> GetAllPermissionsForUser([FromQuery]Guid userId,
        IAppPermissionService permissionService)
    {
        try
        {
            var foundPermissions = await permissionService.GetAllIncludingRolesForUserAsync(userId);
            if (!foundPermissions.Succeeded)
                return await Result<List<PermissionResponse>>.FailAsync(foundPermissions.Messages);

            return await Result<List<PermissionResponse>>.SuccessAsync(foundPermissions.Data.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<PermissionResponse>>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<List<PermissionResponse>>> GetAllPermissionsForRole([FromQuery]Guid roleId,
        IAppPermissionService permissionService)
    {
        try
        {
            var foundPermissions = await permissionService.GetAllForRoleAsync(roleId);
            if (!foundPermissions.Succeeded)
                return await Result<List<PermissionResponse>>.FailAsync(foundPermissions.Messages);

            return await Result<List<PermissionResponse>>.SuccessAsync(foundPermissions.Data.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<PermissionResponse>>.FailAsync(ex.Message);
        }
    }
}