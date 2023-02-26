using Application.Constants.Messages;
using Application.Helpers.Web;
using Application.Models.Example;
using Application.Models.Web;
using Application.Repositories.Example;
using Domain.DatabaseEntities.Example;
using Shared.Requests.Example;
using Shared.Responses.Example;

namespace Application.Api.v1.Example;

public static class ExamplePermissionEndpoints
{
    public static void MapEndpointsExamplePermissions(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/example/permissions", GetAllPermissions).ApiVersionOne();
        app.MapGet("/api/example/permission", GetPermission).ApiVersionOne();
        app.MapPost("/api/example/permission", CreatePermission).ApiVersionOne();
        app.MapPut("/api/example/permission", UpdatePermission).ApiVersionOne();
        app.MapDelete("/api/example/permission", DeletePermission).ApiVersionOne();
        app.MapPost("/api/example/permission/add-to", AddPermissionToObject).ApiVersionOne();
        app.MapPost("/api/example/permission/remove-from", RemovePermissionToObject).ApiVersionOne();
    }

    // TODO: Add authorization/permissions to these endpoints
    private static async Task<IResult<List<ExamplePermissionResponse>>> GetAllPermissions(IExamplePermissionRepository repository)
    {
        try
        {
            var allExamplePermissions = await repository.GetAll();
            
            return await Result<List<ExamplePermissionResponse>>.SuccessAsync(allExamplePermissions.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<ExamplePermissionResponse>>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<ExamplePermissionResponse>> GetPermission(Guid permissionId, IExamplePermissionRepository repository)
    {
        try
        {
            var foundPermission = await repository.GetById(permissionId);
            
            if (foundPermission is null)
                return await Result<ExamplePermissionResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<ExamplePermissionResponse>.SuccessAsync(foundPermission.ToResponse());
        }
        catch (Exception ex)
        {
            return await Result<ExamplePermissionResponse>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<Guid>> CreatePermission(ExamplePermissionCreateRequest permissionRequest, IExamplePermissionRepository repository)
    {
        try
        {
            var createdId = await repository.Create(permissionRequest.ToCreate());
            if (createdId is null)
                return await Result<Guid>.FailAsync(ErrorMessageConstants.GenericError);

            return await Result<Guid>.SuccessAsync((Guid)createdId);
        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> UpdatePermission(ExamplePermissionUpdateRequest updateRequest, IExamplePermissionRepository repository)
    {
        try
        {
            await repository.Update(updateRequest.ToUpdate());
            return await Result.SuccessAsync("Permission successfully updated!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> DeletePermission(Guid permissionId, IExamplePermissionRepository repository)
    {
        try
        {
            await repository.Delete(permissionId);
            return await Result.SuccessAsync("Permission successfully deleted!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<List<ExamplePermissionResponse>>> GetPermissionsForObject(Guid objectId, IExamplePermissionRepository repository)
    {
        try
        {
            var permissions = await repository.GetPermissionsForObject(objectId);
            return await Result<List<ExamplePermissionResponse>>.SuccessAsync(permissions.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<ExamplePermissionResponse>>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> AddPermissionToObject(Guid objectId, Guid permissionId, IExamplePermissionRepository repository)
    {
        try
        {
            await repository.AddToObject(objectId, permissionId);
            return await Result.SuccessAsync("Permission successfully added!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> RemovePermissionToObject(Guid objectId, Guid permissionId, IExamplePermissionRepository repository)
    {
        try
        {
            await repository.RemoveFromObject(objectId, permissionId);
            return await Result.SuccessAsync("Permission successfully removed!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }
}