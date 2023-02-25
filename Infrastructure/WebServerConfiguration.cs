﻿using Application.Api.v1.Example;
using Application.Api.v1.Identity;
using Application.Api.v1.Monitoring;
using Application.Constants.Web;
using Application.Services.Database;
using Hangfire;
using Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class WebServerConfiguration
{
    // Certificate loading via appsettings.json =>
    //   https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0
    
    public static void ConfigureWebServices(this WebApplication app)
    {
        app.ConfigureForEnvironment();
        app.ConfigureBlazorServerCommons();
        
        app.ValidateDatabaseStructure();
        
        app.ConfigureCoreServices();
        app.ConfigureApiServices();
        app.ConfigureIdentityServices();
        app.MapApiEndpoints();
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
        app.UseAuthorization();
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
    }

    private static void MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        // Map all service API endpoints
        app.MapEndpointsExampleObjects();
        app.MapEndpointsHealth();
        app.MapEndpointsWeather();
    }
}