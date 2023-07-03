using Application.Constants.Communication;
using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Helpers.Web;
using Application.Mappers.Identity;
using Application.Models.Web;
using Application.Requests.Identity.User;
using Application.Responses.Identity;
using Application.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Api.v1.Identity;

public static class UserEndpoints
{
    public static void MapEndpointsUsers(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRouteConstants.Identity.User.GetAll, GetAllUsers).ApiVersionOne();
        app.MapGet(ApiRouteConstants.Identity.User.GetById, GetUserById).ApiVersionOne();
        app.MapGet(ApiRouteConstants.Identity.User.GetFullById, GetFullUserById).ApiVersionOne();
        app.MapGet(ApiRouteConstants.Identity.User.GetByEmail, GetUserByEmail).ApiVersionOne();
        app.MapGet(ApiRouteConstants.Identity.User.GetFullByEmail, GetFullUserByEmail).ApiVersionOne();
        app.MapGet(ApiRouteConstants.Identity.User.GetByUsername, GetUserByUsername).ApiVersionOne();
        app.MapGet(ApiRouteConstants.Identity.User.GetFullByUsername, GetFullUserByUsername).ApiVersionOne();
        app.MapDelete(ApiRouteConstants.Identity.User.Delete, DeleteUser).ApiVersionOne();
        app.MapPost(ApiRouteConstants.Identity.User.Create, CreateUser).ApiVersionOne();
        app.MapPost(ApiRouteConstants.Identity.User.Register, Register).ApiVersionOne();
        app.MapPost(ApiRouteConstants.Identity.User.Login, Login).ApiVersionOne();
        app.MapPut(ApiRouteConstants.Identity.User.Update, UpdateUser).ApiVersionOne();
        
        // TODO: Add swagger endpoint viewer enrichment
    }

    private static async Task<IResult> Register(UserRegisterRequest registerRequest, IAppAccountService accountService)
    {
        try
        {
            var request = await accountService.RegisterAsync(registerRequest);
            if (!request.Succeeded) return await Result.FailAsync(request.Messages);
            return await Result.SuccessAsync("Successfully registered, please check the email provided for details!");
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

    [Authorize(Policy = PermissionConstants.Users.View)]
    private static async Task<IResult<List<UserBasicResponse>>> GetAllUsers(IAppUserService userService)
    {
        try
        {
            var allUsers = await userService.GetAllAsync();
            if (!allUsers.Succeeded)
                return await Result<List<UserBasicResponse>>.FailAsync(allUsers.Messages);

            return await Result<List<UserBasicResponse>>.SuccessAsync(allUsers.Data.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<UserBasicResponse>>.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Users.View)]
    private static async Task<IResult<UserBasicResponse>> GetUserById([FromQuery]Guid userId, IAppUserService userService)
    {
        try
        {
            var foundUser = await userService.GetByIdAsync(userId);
            if (!foundUser.Succeeded)
                return await Result<UserBasicResponse>.FailAsync(foundUser.Messages);

            if (foundUser.Data is null)
                return await Result<UserBasicResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<UserBasicResponse>.SuccessAsync(foundUser.Data.ToResponse());
        }
        catch (Exception ex)
        {
            return await Result<UserBasicResponse>.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Users.View)]
    private static async Task<IResult<UserFullResponse>> GetFullUserById([FromQuery]Guid userId, IAppUserService userService)
    {
        try
        {
            var foundUser = await userService.GetByIdFullAsync(userId);
            if (!foundUser.Succeeded)
                return await Result<UserFullResponse>.FailAsync(foundUser.Messages);

            if (foundUser.Data is null)
                return await Result<UserFullResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<UserFullResponse>.SuccessAsync(foundUser.Data.ToResponse());
        }
        catch (Exception ex)
        {
            return await Result<UserFullResponse>.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Users.View)]
    private static async Task<IResult<UserBasicResponse>> GetUserByEmail([FromQuery]string email, IAppUserService userService)
    {
        try
        {
            var foundUser = await userService.GetByEmailAsync(email);
            if (!foundUser.Succeeded)
                return await Result<UserBasicResponse>.FailAsync(foundUser.Messages);

            if (foundUser.Data is null)
                return await Result<UserBasicResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<UserBasicResponse>.SuccessAsync(foundUser.Data.ToResponse());
        }
        catch (Exception ex)
        {
            return await Result<UserBasicResponse>.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Users.View)]
    private static async Task<IResult<UserFullResponse>> GetFullUserByEmail([FromQuery]string email, IAppUserService userService)
    {
        try
        {
            var foundUser = await userService.GetByEmailFullAsync(email);
            if (!foundUser.Succeeded)
                return await Result<UserFullResponse>.FailAsync(foundUser.Messages);

            if (foundUser.Data is null)
                return await Result<UserFullResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<UserFullResponse>.SuccessAsync(foundUser.Data.ToResponse());
        }
        catch (Exception ex)
        {
            return await Result<UserFullResponse>.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Users.View)]
    private static async Task<IResult<UserBasicResponse>> GetUserByUsername([FromQuery]string username, IAppUserService userService)
    {
        try
        {
            var foundUser = await userService.GetByUsernameAsync(username);
            if (!foundUser.Succeeded)
                return await Result<UserBasicResponse>.FailAsync(foundUser.Messages);

            if (foundUser.Data is null)
                return await Result<UserBasicResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<UserBasicResponse>.SuccessAsync(foundUser.Data.ToResponse());
        }
        catch (Exception ex)
        {
            return await Result<UserBasicResponse>.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Users.View)]
    private static async Task<IResult<UserFullResponse>> GetFullUserByUsername([FromQuery]string username, IAppUserService userService)
    {
        try
        {
            var foundUser = await userService.GetByUsernameFullAsync(username);
            if (!foundUser.Succeeded)
                return await Result<UserFullResponse>.FailAsync(foundUser.Messages);

            if (foundUser.Data is null)
                return await Result<UserFullResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<UserFullResponse>.SuccessAsync(foundUser.Data.ToResponse());
        }
        catch (Exception ex)
        {
            return await Result<UserFullResponse>.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Users.Create)]
    private static async Task<IResult<Guid>> CreateUser(UserCreateRequest userRequest, IAppUserService userService, IAppAccountService 
    accountService, ICurrentUserService currentUserService)
    {
        try
        {
            var createRequest = userRequest.ToCreateObject();

            return await userService.CreateAsync(createRequest);
        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Users.Edit)]
    private static async Task<IResult> UpdateUser(UserUpdateRequest userRequest, IAppUserService userService, ICurrentUserService currentUserService)
    {
        try
        {
            var updateRequest = await userService.UpdateAsync(userRequest.ToUpdate());
            if (!updateRequest.Succeeded) return updateRequest;
            return await Result.SuccessAsync("Successfully updated user!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    [Authorize(Policy = PermissionConstants.Users.Delete)]
    private static async Task<IResult> DeleteUser(Guid userId, IAppUserService userService)
    {
        try
        {
            var deleteRequest = await userService.DeleteAsync(userId);
            if (!deleteRequest.Succeeded) return deleteRequest;
            return await Result.SuccessAsync("Successfully deleted user!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }
}