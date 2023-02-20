using Application.Helpers.Web;
using Application.Models.Web;
using Application.Repositories.Example;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Shared.Requests.Identity;
using Shared.Responses.Identity;

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

    private static async Task<IResult> Register(UserRegisterRequest registerRequest, IAppUserRepository repository)
    {
        try
        {
            await repository.RegisterAsync(registerRequest);
            return await Result.SuccessAsync("Successfully registered user!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<List<ExampleObjectDisplay>>> GetAllUsers(IExampleObjectRepository repository)
    {
        try
        {
            return await Result<List<ExampleObjectDisplay>>.SuccessAsync(await repository.GetAll());
        }
        catch (Exception ex)
        {
            return await Result<List<ExampleObjectDisplay>>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<UserBasicResponse>> GetUser(string userId, IExampleObjectRepository repository)
    {
        try
        {
            var isValidGuid = Guid.TryParse(userId, out var convertedGuid);
            if (!isValidGuid)
                return await Result<UserBasicResponse>.FailAsync("Userid provided is invalid");
            
            var foundUser = await repository.Get(convertedGuid);
            if (foundUser is null)
                return await Result<UserBasicResponse>.FailAsync("Userid provided is invalid");
            
            return Result<UserBasicResponse>.SuccessAsync(foundUser);
        }
        catch (Exception ex)
        {
            return Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> CreateUser(CreateExampleUserRequest userRequest, IExampleObjectRepository repository)
    {
        try
        {
            await repository.CreateUser(userRequest);
            return Result.SuccessAsync("Successfully created user!");
        }
        catch (Exception ex)
        {
            return Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> UpdateUser(UpdateExampleUserRequest userRequest, IExampleObjectRepository repository)
    {
        try
        {
            var userResponse = await repository.GetUser(new GetExampleObjectRequest(){ Id = userRequest.Id });
            if (userResponse is null) return Results.NotFound("User ID provided is invalid, please try again!");
            
            await repository.UpdateUser(userRequest);
            return Result.SuccessAsync("Successfully updated user!");
        }
        catch (Exception ex)
        {
            return Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> DeleteUser(int userId, IExampleObjectRepository repository)
    {
        try
        {
            var userResponse = await repository.GetUser(new GetExampleObjectRequest(){Id = userId});
            if (userResponse is null) return Results.NotFound("User ID provided is invalid, please try again!");
            
            await repository.DeleteUser(new DeleteExampleUserRequest(){Id = userId});
            return Result.SuccessAsync("Successfully deleted user!");
        }
        catch (Exception ex)
        {
            return Result.FailAsync(ex.Message);
        }
    }
}