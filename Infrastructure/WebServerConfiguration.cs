using Application.Api.v1.Example;
using Application.Api.v1.Identity;
using Application.Api.v1.Monitoring;
using Application.Constants.Web;
using Application.Helpers.Runtime;
using Application.Services.Database;
using Application.Services.System;
using Application.Settings.AppSettings;
using Hangfire;
using Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class WebServerConfiguration
{
    // Certificate loading via appsettings.json =>
    //   https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0
    
    public static void ConfigureWebServices(this WebApplication app)
    {
        app.ConfigureForEnvironment();
        app.ConfigureBlazorServerCommons();
        app.SetupRunningServerState();
        
        app.ValidateDatabaseStructure();
        
        app.ConfigureCoreServices();
        app.ConfigureApiServices();
        app.ConfigureIdentityServices();
        
        app.MapExampleApiEndpoints();
        app.MapApplicationApiEndpoints();
    }

    private static void ConfigureForEnvironment(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            return;
        }
        
        app.UseExceptionHandler("/Error");
        app.UseHsts();
        
        // TODO: Validate url adding via appsettings.json
        // using var scope = app.Services.CreateAsyncScope();
        // var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        // var servicesCollection = scope.ServiceProvider.GetRequiredService<IServiceCollection>();
        // var appConfig = configuration.GetApplicationSettings(servicesCollection);
        //
        // app.Urls.Add(appConfig.BaseUrl!);
    }

    private static void ConfigureBlazorServerCommons(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
        
        app.UseSerilogRequestLogging();
        app.UseForwardedHeaders(new ForwardedHeadersOptions()
        {
            ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
        });
        app.UseMiddleware<ErrorHandlerMiddleware>();
    }

    private static void SetupRunningServerState(this IHost app)
    {
        using var scope = app.Services.CreateAsyncScope();
        var appConfig = scope.ServiceProvider.GetRequiredService<IOptions<AppConfiguration>>();
        var serverState = scope.ServiceProvider.GetRequiredService<IRunningServerState>();
        
        #if DEBUG
            serverState.IsRunningInDebugMode = true;
        #else
            serverState.IsRunningInDebugMode = false;
        #endif
        
        serverState.ApplicationName = appConfig.Value.ApplicationName;
    }

    private static void ValidateDatabaseStructure(this IHost app)
    {
        using var scope = app.Services.CreateAsyncScope();
        var sqlAccess = scope.ServiceProvider.GetRequiredService<ISqlDataService>();
        sqlAccess.EnforceDatabaseStructure();
    }

    private static void ConfigureCoreServices(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard("/jobs", new DashboardOptions
        {
            DashboardTitle = "Jobs",
            // Authorization = new[] {new HangfireAuthorizationFilter()}
        });
    }

    private static void ConfigureIdentityServices(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
        ((IEndpointRouteBuilder) app).MapIdentityApiEndpoints();
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
        app.ConfigureApiVersions();
    }

    private static void ConfigureApiVersions(this IEndpointRouteBuilder app)
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
    }

    private static void MapIdentityApiEndpoints(this IEndpointRouteBuilder app)
    {
        // Map endpoints that require identity services
        app.MapEndpointsUsers();
        app.MapEndpointsRoles();
    }

    private static void MapExampleApiEndpoints(this IEndpointRouteBuilder app)
    {
        // Map all example API endpoints
        app.MapEndpointsBooks();
        app.MapEndpointsBookGenres();
        app.MapEndpointsWeather();
    }

    private static void MapApplicationApiEndpoints(this IEndpointRouteBuilder app)
    {
        // Map all other endpoints for the application (not identity and not examples)
        app.MapEndpointsHealth();
    }
}