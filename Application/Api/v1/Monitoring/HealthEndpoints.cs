using Application.Helpers.Web;
using Application.Models.Web;
using Application.Responses.Monitoring;
using Microsoft.Extensions.Configuration;

namespace Application.Api.v1.Monitoring;

public static class HealthEndpoints
{
    public static void MapEndpointsHealth(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/monitoring/health", GetHealth).ApiVersionOne();
    }

    private static async Task<IResult> GetHealth(IConfiguration configuration)
    {
        var returnObject = new HealthCheckResponse() { ApiVersion = "v1" };
        
        try
        {
            returnObject.Message = configuration.GetSection("TestConfigVariable").Value;
            returnObject.Health = "Healthy";
            return await Result<HealthCheckResponse>.SuccessAsync(returnObject);
        }
        catch (Exception ex)
        {
            returnObject.Message = ex.Message;
            returnObject.Health = "Unhealthy";
            return await Result<HealthCheckResponse>.FailAsync(returnObject);
        }
    }
}