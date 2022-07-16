using Application.Extensibility.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;

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
            var returnObject = new
            {
                Testconfigvariable = configuration.GetSection("TestConfigVariable"),
                Health = "Healthy",
                ApiVersion = "v1"
            };
            await Task.CompletedTask;
            return Results.Ok(returnObject);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}