using Application.Helpers.Web;
using Application.Models.Example;
using Application.Models.Web;
using Application.Repositories.Example;
using Shared.Requests.Example;

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

    private static async Task<IResult<List<ExampleObjectDisplay>>> GetAllObjects(IExampleObjectRepository repository)
    {
        try
        {
            return Results.Ok(await repository.GetAll());
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> GetObject(Guid ObjectId, IExampleObjectRepository repository, IMapper mapper)
    {
        try
        {
            var foundObject = await repository.Get(new GetExampleObjectRequest(){Id = ObjectId});
            if (foundObject is null)
                return Results.NotFound("Object ID provided doesn't match an existing Object");

            var ObjectResponse = mapper.Map<ExampleObjectResponse>(foundObject);
            return Results.Ok(ObjectResponse);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> CreateObject(CreateExampleObjectRequest ObjectRequest, IExampleObjectRepository repository)
    {
        try
        {
            await repository.CreateObject(ObjectRequest);
            return Results.Ok("Successfully created Object!");
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> UpdateObject(UpdateExampleObjectRequest ObjectRequest, IExampleObjectRepository repository)
    {
        try
        {
            var ObjectResponse = await repository.GetObject(new GetExampleObjectRequest(){ Id = ObjectRequest.Id });
            if (ObjectResponse is null) return Results.NotFound("Object ID provided is invalid, please try again!");
            
            await repository.UpdateObject(ObjectRequest);
            return Results.Ok("Successfully updated Object!");
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> DeleteObject(int ObjectId, IExampleObjectRepository repository)
    {
        try
        {
            var ObjectResponse = await repository.GetObject(new GetExampleObjectRequest(){Id = ObjectId});
            if (ObjectResponse is null) return Results.NotFound("Object ID provided is invalid, please try again!");
            
            await repository.DeleteObject(new DeleteExampleObjectRequest(){Id = ObjectId});
            return Results.Ok("Successfully deleted Object!");
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}