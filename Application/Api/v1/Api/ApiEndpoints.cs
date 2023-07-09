using Application.Constants.Web;
using Application.Helpers.Web;
using Application.Models.Web;
using Application.Requests.Api;
using Application.Responses.Api;
using Application.Services.Identity;

namespace Application.Api.v1.Api;

public static class ApiEndpoints
{
    public static void MapEndpointsApi(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRouteConstants.Api.GetToken, GetToken).ApiVersionOne();
    }

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