using Application.Api.v1.Example;
using Application.Api.v1.Monitoring;
using Application.Constants;
using Application.Interfaces.Database;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class WebServerConfiguration
{
    // Certificate loading via appsettings.json =>
    //   https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0
    
    public static void ConfigureWebServer(this WebApplication app)
    {
        app.ConfigureForEnvironment();
        app.ConfigureBlazorServerCommons();
        
        app.ValidateDatabaseStructure();
        
        app.ConfigureQualityOfLifeServices();
        app.ConfigureIdentityServices();
        app.ConfigureApiServices();
        app.MapApiEndpoints();
    }

    private static void ConfigureForEnvironment(this WebApplication app)
    {
        if (app.Environment.IsDevelopment()) return;
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    private static void ConfigureBlazorServerCommons(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
        
        app.UseSerilogRequestLogging();
    }

    private static void ValidateDatabaseStructure(this IHost app)
    {
        using var scope = app.Services.CreateAsyncScope();
        var sqlAccess = scope.ServiceProvider.GetRequiredService<ISqlDataAccess>();
        sqlAccess.EnsureDatabaseStructure();
    }

    private static void ConfigureQualityOfLifeServices(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard("/jobs", new DashboardOptions
        {
            DashboardTitle = "Jobs",
            // Authorization = new[] {new HangfireAuthorizationFilter()}
        });
    }

    private static void ConfigureIdentityServices(this IApplicationBuilder app)
    {
        app.UseAuthorization();
    }

    private static void ConfigureApiServices(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "TestBlazorServer v1");
            options.RoutePrefix = "api";
            options.DisplayRequestDuration();
            options.InjectStylesheet("/css/swagger-dark.css");
        });
        app.MapControllers();
    }

    private static void MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        ApiConstants.SupportsVersionOne = app.NewApiVersionSet()
            .HasApiVersion(ApiConstants.Version1)
            .ReportApiVersions()
            .Build();
        ApiConstants.SupportsOneTwo = app.NewApiVersionSet()
            .HasApiVersion(ApiConstants.Version1)
            .HasApiVersion(ApiConstants.Version2)
            .ReportApiVersions()
            .Build();
        
        // Map all active API endpoints
        app.MapEndpointsUsers();
        app.MapEndpointsHealth();
        app.MapEndpointsWeather();
    }
}