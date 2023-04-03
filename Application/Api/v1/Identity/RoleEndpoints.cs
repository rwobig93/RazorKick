using Application.Constants.Communication;
using Application.Helpers.Web;
using Application.Models.Identity;
using Application.Models.Web;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Requests.Identity.Role;
using Shared.Responses.Identity;

namespace Application.Api.v1.Identity;

public static class RoleEndpoints
{
    public static void MapEndpointsRoles(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/roles", GetAllRoles).ApiVersionOne();
        app.MapGet("/api/identity/role", GetRole).ApiVersionOne();
        app.MapDelete("/api/identity/role", DeleteRole).ApiVersionOne();
        app.MapPost("/api/identity/role", CreateRole).ApiVersionOne();
        app.MapPut("/api/identity/role", UpdateRole).ApiVersionOne();
        
        app.MapGet("/api/identity/roles/user", GetRolesForUser).ApiVersionOne();
        app.MapGet("/api/identity/role/isinrole", IsUserInRole).ApiVersionOne();
        app.MapPost("/api/identity/role/adduser", AddUserToRole).ApiVersionOne();
        app.MapPost("/api/identity/role/removeuser", RemoveUserFromRole).ApiVersionOne();
    }
    
    public static async Task<IResult<List<RoleResponse>>> GetAllRoles(IAppRoleRepository repository)
    {
        try
        {
            var allRoles = await repository.GetAllAsync();
            if (!allRoles.Success)
                return await Result<List<RoleResponse>>.FailAsync(allRoles.ErrorMessage);

            return await Result<List<RoleResponse>>.SuccessAsync(allRoles.Result!.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<RoleResponse>>.FailAsync(ex.Message);
        }
    }
    
    public static async Task<IResult<RoleResponse>> GetRole(Guid roleId, IAppRoleRepository repository)
    {
        try
        {
            var role = await repository.GetByIdAsync(roleId);
            if (!role.Success)
                return await Result<RoleResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<RoleResponse>.SuccessAsync(role.Result!.ToResponse());
        }
        catch (Exception ex)
        {
            return await Result<RoleResponse>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> CreateRole(CreateRoleRequest roleRequest, IAppRoleRepository repository)
    {
        try
        {
            var createRequest = roleRequest.ToCreateObject();
            createRequest.CreatedBy = Guid.Empty;
            
            var result = await repository.CreateAsync(createRequest);
            if (!result.Success)
                return await Result.FailAsync(result.ErrorMessage);
            
            return await Result.SuccessAsync("Successfully created role!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> UpdateRole(UpdateRoleRequest roleRequest, IAppRoleRepository repository,
        ICurrentUserService currentUserService)
    {
        try
        {
            var submittingUserId = await currentUserService.GetApiCurrentUserId();
            
            var roleResponse = (await repository.GetByIdAsync(roleRequest.Id)).Result;
            if (roleResponse is null) return await Result.FailAsync(ErrorMessageConstants.UserNotFoundError);
            
            await repository.UpdateAsync(roleRequest.ToUpdate(), submittingUserId);
            return await Result.SuccessAsync("Successfully updated role!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> DeleteRole(Guid roleId, IAppRoleRepository repository, ICurrentUserService currentUserService)
    {
        try
        {
            var currentUser = await currentUserService.GetApiCurrentUserId();
            var roleResponse = (await repository.GetByIdAsync(roleId)).Result;
            if (roleResponse is null) return await Result.FailAsync(ErrorMessageConstants.UserNotFoundError);

            await repository.DeleteAsync(roleId, currentUser);
            return await Result.SuccessAsync("Successfully deleted role!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<bool>> IsUserInRole([FromQuery]Guid userId, [FromQuery]Guid roleId, IAppRoleRepository repository)
    {
        try
        {
            var roleResponse = await repository.IsUserInRoleAsync(userId, roleId);
            if (!roleResponse.Success) return await Result<bool>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            return await Result<bool>.SuccessAsync(roleResponse.Result);
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> AddUserToRole(Guid userId, Guid roleId, IAppRoleRepository repository, ICurrentUserService currentUserService)
    {
        try
        {
            var currentUserId = await currentUserService.GetApiCurrentUserId();
            var roleResponse = await repository.AddUserToRoleAsync(userId, roleId, currentUserId);
            if (!roleResponse.Success) return await Result<bool>.FailAsync(roleResponse.ErrorMessage);
            
            return await Result<bool>.SuccessAsync("Successfully added user to role!");
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> RemoveUserFromRole(Guid userId, Guid roleId, IAppRoleRepository repository, ICurrentUserService currentUserService)
    {
        try
        {
            var currentUserId = await currentUserService.GetApiCurrentUserId();
            var roleResponse = await repository.RemoveUserFromRoleAsync(userId, roleId, currentUserId);
            if (!roleResponse.Success) return await Result<bool>.FailAsync(roleResponse.ErrorMessage);
            
            return await Result<bool>.SuccessAsync("Successfully removes user from role!");
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<List<RoleResponse>>> GetRolesForUser(Guid userId, IAppRoleRepository repository)
    {
        try
        {
            var roleResponse = await repository.GetRolesForUser(userId);
            if (!roleResponse.Success) return await Result<List<RoleResponse>>.FailAsync(roleResponse.ErrorMessage);
            
            return await Result<List<RoleResponse>>.SuccessAsync(roleResponse.Result!.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<RoleResponse>>.FailAsync(ex.Message);
        }
    }
}