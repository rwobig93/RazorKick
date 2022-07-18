using Application.Interfaces.Example;
using Shared.Requests.Example;
using Shared.Responses.Example;

namespace Application.Api.v1.Example;

public static class ExampleUsersEndpoints
{
    public static void MapEndpointsExampleUsers(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/example/users", GetAllUsers).ApiVersionOne();
        app.MapGet("/api/example/user", GetUser).ApiVersionOne();
        app.MapPost("/api/example/user", UpdateUser).ApiVersionOne();
        app.MapPut("/api/example/user", CreateUser).ApiVersionOne();
        app.MapDelete("/api/example/user", DeleteUser).ApiVersionOne();
    }

    private static async Task<IResult> GetAllUsers(IExampleUserService repository)
    {
        try
        {
            return Results.Ok(await repository.GetAllUsers());
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> GetUser(int userId, IExampleUserService repository, IMapper mapper)
    {
        try
        {
            var foundUser = await repository.GetUser(new GetExampleUserRequest(){Id = userId});
            if (foundUser is null)
                return Results.NotFound("User ID provided doesn't match an existing User");

            var userResponse = mapper.Map<ExampleUserResponse>(foundUser);
            return Results.Ok(userResponse);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> CreateUser(CreateExampleUserRequest userRequest, IExampleUserService repository)
    {
        try
        {
            await repository.CreateUser(userRequest);
            return Results.Ok("Successfully created user!");
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> UpdateUser(UpdateExampleUserRequest userRequest, IExampleUserService repository)
    {
        try
        {
            var userResponse = await repository.GetUser(new GetExampleUserRequest(){ Id = userRequest.Id });
            if (userResponse is null) return Results.NotFound("User ID provided is invalid, please try again!");
            
            await repository.UpdateUser(userRequest);
            return Results.Ok("Successfully updated user!");
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> DeleteUser(int userId, IExampleUserService repository)
    {
        try
        {
            var userResponse = await repository.GetUser(new GetExampleUserRequest(){Id = userId});
            if (userResponse is null) return Results.NotFound("User ID provided is invalid, please try again!");
            
            await repository.DeleteUser(new DeleteExampleUserRequest(){Id = userId});
            return Results.Ok("Successfully deleted user!");
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}