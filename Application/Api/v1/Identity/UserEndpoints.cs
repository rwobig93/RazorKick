using Application.Constants.Communication;
using Application.Helpers.Web;
using Application.Models.Identity;
using Application.Models.Web;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Identity;
using Shared.Requests.Identity.User;
using Shared.Responses.Identity;
using Shared.Routes;

namespace Application.Api.v1.Identity;

public static class UserEndpoints
{
    public static void MapEndpointsUsers(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Identity.GetAll, GetAllUsers).ApiVersionOne();
        app.MapGet(ApiRoutes.Identity.GetById, GetUserById).ApiVersionOne();
        app.MapGet(ApiRoutes.Identity.GetFullById, GetFullUserById).ApiVersionOne();
        app.MapGet(ApiRoutes.Identity.GetByEmail, GetUserByEmail).ApiVersionOne();
        app.MapGet(ApiRoutes.Identity.GetByUsername, GetUserByUsername).ApiVersionOne();
        app.MapDelete(ApiRoutes.Identity.Delete, DeleteUser).ApiVersionOne();
        app.MapPost(ApiRoutes.Identity.Create, CreateUser).ApiVersionOne();
        app.MapPost(ApiRoutes.Identity.Register, Register).ApiVersionOne();
        app.MapPost(ApiRoutes.Identity.Login, Login).ApiVersionOne();
        app.MapPut(ApiRoutes.Identity.Update, UpdateUser).ApiVersionOne();
        
        // TODO: Add swagger endpoint viewer enrichment
    }

    private static async Task<IResult> Register(UserRegisterRequest registerRequest, IAppAccountService accountService)
    {
        try
        {
            await accountService.RegisterAsync(registerRequest);
            return await Result.SuccessAsync("Successfully registered user!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<UserLoginResponse>> Login(UserLoginRequest loginRequest, IAppAccountService accountService)
    {
        try
        {
            return await accountService.LoginAsync(loginRequest);
        }
        catch (Exception ex)
        {
            return await Result<UserLoginResponse>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<List<UserBasicResponse>>> GetAllUsers(IAppUserRepository repository)
    {
        var usersResult = await repository.GetAllAsync();
        if (!usersResult.Success)
            return await Result<List<UserBasicResponse>>.FailAsync(usersResult.ErrorMessage);
            
        return await Result<List<UserBasicResponse>>.SuccessAsync(usersResult.Result?.ToBasicResponses() ?? new List<UserBasicResponse>());
    }

    private static async Task<IResult<UserBasicResponse>> GetUserById(string userId, IAppUserRepository repository)
    {
        try
        {
            var isValidGuid = Guid.TryParse(userId, out var convertedGuid);
            if (!isValidGuid)
                return await Result<UserBasicResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            var foundUser = (await repository.GetByIdAsync(convertedGuid)).Result;
            if (foundUser is null)
                return await Result<UserBasicResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            return await Result<UserBasicResponse>.SuccessAsync(foundUser.ToBasicResponse());
        }
        catch (Exception ex)
        {
            return await Result<UserBasicResponse>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<UserFullResponse>> GetFullUserById(string userId, IAppUserRepository repository)
    {
        try
        {
            var isValidGuid = Guid.TryParse(userId, out var convertedGuid);
            if (!isValidGuid)
                return await Result<UserFullResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            var foundUser = (await repository.GetByIdFullAsync(convertedGuid)).Result;
            if (foundUser is null)
                return await Result<UserFullResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            return await Result<UserFullResponse>.SuccessAsync(foundUser.ToFullResponse());
        }
        catch (Exception ex)
        {
            return await Result<UserFullResponse>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<UserBasicResponse>> GetUserByEmail(string email, IAppUserRepository repository)
    {
        try
        {
            var foundUser = (await repository.GetByEmailAsync(email)).Result;
            if (foundUser is null)
                return await Result<UserBasicResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            return await Result<UserBasicResponse>.SuccessAsync(foundUser.ToBasicResponse());
        }
        catch (Exception ex)
        {
            return await Result<UserBasicResponse>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<UserBasicResponse>> GetUserByUsername(string username, IAppUserRepository repository)
    {
        try
        {
            var foundUser = (await repository.GetByUsernameAsync(username)).Result;
            if (foundUser is null)
                return await Result<UserBasicResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);
            
            return await Result<UserBasicResponse>.SuccessAsync(foundUser.ToBasicResponse());
        }
        catch (Exception ex)
        {
            return await Result<UserBasicResponse>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> CreateUser(UserCreateRequest userRequest, IAppUserRepository repository, IAppAccountService 
    accountService, ICurrentUserService currentUserService)
    {
        try
        {
            var submittingUserId = await currentUserService.GetApiCurrentUserId();
            
            var passwordMeetsRequirements = accountService.PasswordMeetsRequirements(userRequest.Password);
            if (!passwordMeetsRequirements)
                return await Result.FailAsync("Password provided doesn't meet the requirements");
            
            var createRequest = userRequest.ToCreateObject();
            createRequest.CreatedBy = Guid.Empty;
            
            var result = await repository.CreateAsync(createRequest, submittingUserId);
            if (!result.Success)
                return await Result.FailAsync(result.ErrorMessage);
            
            return await Result.SuccessAsync("Successfully created user!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> UpdateUser(UserUpdateRequest userRequest, IAppUserRepository repository, ICurrentUserService currentUserService)
    {
        try
        {
            var submittingUserId = await currentUserService.GetApiCurrentUserId();

            var userResponse = (await repository.GetByIdAsync(userRequest.Id)).Result;
            if (userResponse is null) return await Result.FailAsync(ErrorMessageConstants.UserNotFoundError);
            
            await repository.UpdateAsync(userRequest.ToUpdateObject(), submittingUserId);
            return await Result.SuccessAsync("Successfully updated user!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> DeleteUser(Guid userId, IAppUserRepository repository)
    {
        try
        {
            var userResponse = (await repository.GetByIdAsync(userId)).Result;
            if (userResponse is null) return await Result.FailAsync(ErrorMessageConstants.UserNotFoundError);
            
            await repository.DeleteAsync(userId);
            return await Result.SuccessAsync("Successfully deleted user!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }
}