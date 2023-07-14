using Application.Constants.Web;
using Application.Helpers.Web;
using Application.Models.Web;
using Application.Requests.Api;
using Application.Responses.Api;
using Application.Services.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Application.Api.v1.Api;

public static class ApiEndpoints
{
    public static void MapEndpointsApi(this IEndpointRouteBuilder app)
    {
        // TODO: See https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0&source=recommendations
        app.MapPost(ApiRouteConstants.Api.GetToken, GetToken).ApiVersionOne();
    }

    /// <summary>
    /// Gets a service account Json Web Token (JWT) for use in API calls
    /// </summary>
    /// <param name="tokenRequest">Credentials to authenticate</param>
    /// <returns>JWT with an expiration datetime in GMT/UTC</returns>
    [AllowAnonymous]
    private static async Task<IResult<ApiTokenResponse>> GetToken(ApiGetTokenRequest tokenRequest, IAppAccountService accountService)
    {
        try
        {
            return await accountService.GetApiToken(tokenRequest);
        }
        catch (Exception ex)
        {
            return await Result<ApiTokenResponse>.FailAsync(ex.Message);
        }
    }
}