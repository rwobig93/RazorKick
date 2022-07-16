using Application.Extensibility.Extensions;
using Application.Interfaces.Example;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.ApiRequests.Example;
using Shared.ApiResponses.Example;

namespace Application.Api.v1.Example;

public static class UsersEndpoints
{
    public static void MapEndpointsUsers(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", GetAllUsers).ApiVersionOne();
        app.MapGet("/api/user", GetUser).ApiVersionOne();
        app.MapPost("/api/user", UpdateUser).ApiVersionOne();
        app.MapPut("/api/user", CreateUser).ApiVersionOne();
        app.MapDelete("/api/user", DeleteUser).ApiVersionOne();
    }

    private static async Task<IResult> GetAllUsers(IUserRepository repository)
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

    private static async Task<IResult> GetUser(int userId, IUserRepository repository, IMapper mapper)
    {
        try
        {
            var foundUser = await repository.GetUser(new GetUserRequest(){Id = userId});
            if (foundUser is null)
                return Results.NotFound("User ID provided doesn't match an existing User");

            var userResponse = mapper.Map<UserResponse>(foundUser);
            return Results.Ok(userResponse);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> CreateUser(CreateUserRequest userRequest, IUserRepository repository)
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

    private static async Task<IResult> UpdateUser(UpdateUserRequest userRequest, IUserRepository repository)
    {
        try
        {
            var userResponse = await repository.GetUser(new GetUserRequest(){ Id = userRequest.Id });
            if (userResponse is null) return Results.NotFound("User ID provided is invalid, please try again!");
            
            await repository.UpdateUser(userRequest);
            return Results.Ok("Successfully updated user!");
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> DeleteUser(int userId, IUserRepository repository)
    {
        try
        {
            var userResponse = await repository.GetUser(new GetUserRequest(){Id = userId});
            if (userResponse is null) return Results.NotFound("User ID provided is invalid, please try again!");
            
            await repository.DeleteUser(new DeleteUserRequest(){Id = userId});
            return Results.Ok("Successfully deleted user!");
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}