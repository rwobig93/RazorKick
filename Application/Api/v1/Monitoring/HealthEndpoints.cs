using Application.Helpers.Web;
using Application.Models.Web;
using Microsoft.Extensions.Configuration;
using Shared.Responses.Monitoring;

namespace Application.Api.v1.Monitoring;

public static class HealthEndpoints
{
    public static void MapEndpointsHealth(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/health", GetHealth).ApiVersionOne();
    }

    private static async Task<IResult> GetHealth(IConfiguration configuration)
    {
        try
        {
            var returnObject = new HealthCheckResponse()
            {
                SettingsValue = configuration.GetSection("TestConfigVariable").Value,
                Health = "Healthy",
                ApiVersion = "v1"
            };
            return await Result<HealthCheckResponse>.SuccessAsync(returnObject);
        }
        catch (Exception ex)
        {
            var returnObject = new HealthCheckResponse()
            {
                Health = "Unhealthy",
                ApiVersion = "v1"
            };
            return await Result<HealthCheckResponse>.FailAsync(returnObject);
        }
    }
}