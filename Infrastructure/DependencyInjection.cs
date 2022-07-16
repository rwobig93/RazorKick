using System.Reflection;
using System;
using Application.Interfaces.Database;
using Application.Interfaces.Example;
using Application.Mappings;
using Application.Mappings.Example;
using Asp.Versioning;
using AutoMapper;
using Hangfire;
using Hangfire.Dashboard.Dark.Core;
using Infrastructure.Services.Database;
using Infrastructure.Services.Example;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Infrastructure;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        // Replace default logger w/ Serilog, configure via appsettings.json - uses the "Serilog" section
        builder.Host.UseSerilog((ctx, lc) => 
            lc.ReadFrom.Configuration(ctx.Configuration), preserveStaticLogger: false);
        
        builder.Services.AddBlazorServerCommon();
        builder.Services.AddQualityOfLifeServices(builder.Configuration);

        builder.Services.AddCoreSingletonServices(builder.Configuration);
        builder.Services.AddCoreTransientServices(builder.Configuration);
        builder.Services.AddCoreScopedServices(builder.Configuration);

        builder.Services.AddApiServices();
        builder.Services.AddDatabaseServices();

        return builder;
    }

    private static IServiceCollection AddBlazorServerCommon(this IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor();

        return services;
    }

    private static IServiceCollection AddCoreSingletonServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IWeatherForecast, WeatherForecast>();

        return services;
    }

    private static IServiceCollection AddCoreTransientServices(this IServiceCollection services, IConfiguration configuration)
    {

        return services;
    }

    private static IServiceCollection AddCoreScopedServices(this IServiceCollection services, IConfiguration configuration)
    {

        return services;
    }

    private static IServiceCollection AddDatabaseServices(this IServiceCollection services)
    {
        services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
        services.AddSingleton<IUserRepository, UserRepository>();

        return services;
    }

    private static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddApiVersioning(c =>
        {
            c.AssumeDefaultVersionWhenUnspecified = true;
            c.DefaultApiVersion = new ApiVersion(1);
            c.ReportApiVersions = true;
            c.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("api-version"),
                new HeaderApiVersionReader("X-Version"),
                new MediaTypeApiVersionReader("ver"),
                new UrlSegmentApiVersionReader());
        });

        return services;
    }

    private static IServiceCollection AddQualityOfLifeServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(BaseMapProfile));
        services.AddHangfire(x =>
        {
            x.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));
            x.UseDarkDashboard();
        });
        services.AddHangfireServer();

        return services;
    }
}