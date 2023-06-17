using Application.Constants.Communication;
using Application.Constants.Identity;
using Application.Helpers.Web;
using Application.Models.Identity;
using Application.Models.Web;
using Application.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Requests.Identity.Role;
using Shared.Responses.Identity;
using Shared.Routes;

namespace Application.Api.v1.Identity;

public static class RoleEndpoints
{
    public static void MapEndpointsRoles(this IEndpointRouteBuilder app)
    {
        // Roles
        app.MapGet(ApiRoutes.Identity.Role.GetAll, GetAllRoles).ApiVersionOne();
        app.MapGet(ApiRoutes.Identity.Role.GetById, GetById).ApiVersionOne();
        app.MapDelete(ApiRoutes.Identity.Role.Delete, DeleteRole).ApiVersionOne();
        app.MapPost(ApiRoutes.Identity.Role.Create, CreateRole).ApiVersionOne();
        app.MapPut(ApiRoutes.Identity.Role.Update, UpdateRole).ApiVersionOne();
        
        // Users
        app.MapGet(ApiRoutes.Identity.Role.GetRolesForUser, GetRolesForUser).ApiVersionOne();
        app.MapGet(ApiRoutes.Identity.Role.IsUserInRole, IsUserInRole).ApiVersionOne();
        app.MapPost(ApiRoutes.Identity.Role.AddUserToRole, AddUserToRole).ApiVersionOne();
        app.MapPost(ApiRoutes.Identity.Role.RemoveUserFromRole, RemoveUserFromRole).ApiVersionOne();
    }

    [Authorize(Policy = PermissionConstants.Roles.View)]
    private static async Task<IResult<List<RoleResponse>>> GetAllRoles(IAppRoleService roleService)
    {
        try
        {
            var allRoles = await roleService.GetAllAsync();
            if (!allRoles.Succeeded)
                return await Result<List<RoleResponse>>.FailAsync(allRoles.Messages);

            return await Result<List<RoleResponse>>.SuccessAsync(allRoles.Data.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<RoleResponse>>.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Roles.View)]
    private static async Task<IResult<RoleResponse>> GetById([FromQuery]Guid roleId, IAppRoleService roleService)
    {
        try
        {
            var role = await roleService.GetByIdAsync(roleId);
            if (!role.Succeeded)
                return await Result<RoleResponse>.FailAsync(role.Messages);

            if (role.Data is null)
                return await Result<RoleResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<RoleResponse>.SuccessAsync(role.Data.ToResponse());
        }
        catch (Exception ex)
        {
            return await Result<RoleResponse>.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Roles.Create)]
    private static async Task<IResult<Guid>> CreateRole(CreateRoleRequest roleRequest, IAppRoleService roleService)
    {
        try
        {
            var createRequest = roleRequest.ToCreateObject();

            return await roleService.CreateAsync(createRequest);
        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Roles.Edit)]
    private static async Task<IResult> UpdateRole(UpdateRoleRequest roleRequest, IAppRoleService roleService)
    {
        try
        {
            var updateRequest = await roleService.UpdateAsync(roleRequest.ToUpdate());
            if (!updateRequest.Succeeded) return updateRequest;
            return await Result.SuccessAsync("Successfully updated role!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Roles.Delete)]
    private static async Task<IResult> DeleteRole(Guid roleId, IAppRoleService roleService)
    {
        try
        {
            var deleteRequest = await roleService.DeleteAsync(roleId);
            if (!deleteRequest.Succeeded) return deleteRequest;
            return await Result.SuccessAsync("Successfully deleted role!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Roles.View)]
    private static async Task<IResult<bool>> IsUserInRole([FromQuery]Guid userId, [FromQuery]Guid roleId, IAppRoleService roleService)
    {
        try
        {
            return await roleService.IsUserInRoleAsync(userId, roleId);
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Roles.Add)]
    private static async Task<IResult> AddUserToRole(Guid userId, Guid roleId, IAppRoleService roleService)
    {
        try
        {
            var roleResponse = await roleService.AddUserToRoleAsync(userId, roleId);
            if (!roleResponse.Succeeded) return await Result<bool>.FailAsync(roleResponse.Messages);
            
            return await Result.SuccessAsync("Successfully added user to role!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Roles.Remove)]
    private static async Task<IResult> RemoveUserFromRole(Guid userId, Guid roleId, IAppRoleService roleService)
    {
        try
        {
            var roleResponse = await roleService.RemoveUserFromRoleAsync(userId, roleId);
            if (!roleResponse.Succeeded) return await Result<bool>.FailAsync(roleResponse.Messages);
            
            return await Result<bool>.SuccessAsync("Successfully removed user from role!");
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Roles.View)]
    private static async Task<IResult<List<RoleResponse>>> GetRolesForUser([FromQuery]Guid userId, IAppRoleService roleService)
    {
        try
        {
            var roleResponse = await roleService.GetRolesForUser(userId);
            if (!roleResponse.Succeeded) return await Result<List<RoleResponse>>.FailAsync(roleResponse.Messages);
            
            return await Result<List<RoleResponse>>.SuccessAsync(roleResponse.Data.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<RoleResponse>>.FailAsync(ex.Message);
        }
    }
}