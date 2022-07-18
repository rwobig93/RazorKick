using Application.Interfaces.Example;
using Application.Interfaces.Identity;
using Shared.Requests.Example;
using Shared.Requests.Identity;
using Shared.Responses.Example;

namespace Application.Api.v1.Identity;

public static class UserEndpoints
{
    public static void MapEndpointsUsers(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", GetAllUsers).ApiVersionOne();
        app.MapGet("/api/user", GetUser).ApiVersionOne();
        app.MapDelete("/api/user", DeleteUser).ApiVersionOne();
        app.MapPost("/api/user/register", Register).ApiVersionOne();
        app.MapPost("/api/user/update", UpdateUser).ApiVersionOne();
        
        // TODO: Add swagger endpoint viewer enrichment
    }

    private static async Task<IResult> Register(UserRegisterRequest registerRequest, IUserService repository)
    {
        try
        {
            await repository.RegisterAsync(registerRequest);
            return Results.Ok("Successfully registered user!");
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
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