using Application.Constants.Messages;
using Application.Helpers.Web;
using Application.Models.Example;
using Application.Models.Web;
using Application.Repositories.Example;
using Domain.DatabaseEntities.Example;
using Shared.Requests.Example;
using Shared.Responses.Example;

namespace Application.Api.v1.Example;

public static class ExampleObjectEndpoints
{
    public static void MapEndpointsExampleObjects(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/example/objects", GetAllObjects).ApiVersionOne();
        app.MapGet("/api/example/object", GetObject).ApiVersionOne();
        app.MapPost("/api/example/object", UpdateObject).ApiVersionOne();
        app.MapPut("/api/example/object", CreateObject).ApiVersionOne();
        app.MapDelete("/api/example/object", DeleteObject).ApiVersionOne();
    }

    // TODO: Add authorization/permissions to these endpoints
    private static async Task<IResult<List<ExampleObjectResponse>>> GetAllObjects(IExampleObjectRepository repository)
    {
        try
        {
            var allExampleObjects = await repository.GetAll();
            
            return await Result<List<ExampleObjectResponse>>.SuccessAsync(allExampleObjects.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<ExampleObjectResponse>>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<ExampleObjectResponse>> GetObject(Guid objectId, IExampleObjectRepository repository)
    {
        try
        {
            var foundObject = await repository.Get(objectId);
            
            if (foundObject is null)
                return await Result<ExampleObjectResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<ExampleObjectResponse>.SuccessAsync(foundObject.ToResponse());
        }
        catch (Exception ex)
        {
            return await Result<ExampleObjectResponse>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<Guid>> CreateObject(ExampleObjectCreateRequest objectRequest, IExampleObjectRepository repository)
    {
        try
        {
            var createdId = await repository.Create(objectRequest.ToObjectCreate());
            if (createdId is null)
                return await Result<Guid>.FailAsync(ErrorMessageConstants.GenericError);

            return await Result<Guid>.SuccessAsync((Guid)createdId);
        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> UpdateObject(ExampleObjectUpdateRequest updateRequest, IExampleObjectRepository repository)
    {
        try
        {
            await repository.Update(updateRequest.ToObjectUpdate());
            return await Result.SuccessAsync("Object successfully updated!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> DeleteObject(Guid objectId, IExampleObjectRepository repository)
    {
        try
        {
            await repository.Delete(objectId);
            return await Result.SuccessAsync("Object successfully deleted!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }
}