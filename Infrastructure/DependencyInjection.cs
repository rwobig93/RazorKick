using System.Net;
using System.Security.Claims;
using System.Text;
using Application.Extensibility.Extensions;
using Application.Extensibility.Settings;
using Application.Interfaces.Database;
using Application.Interfaces.Example;
using Application.Mappings;
using Application.Wrappers;
using Asp.Versioning;
using Hangfire;
using Hangfire.Dashboard.Dark.Core;
using Infrastructure.Features.Database;
using Infrastructure.Features.Example;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using Newtonsoft.Json;

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
        builder.Services.AddAuthServices(builder.Configuration);

        builder.Services.AddCoreSingletonServices();
        builder.Services.AddCoreTransientServices();
        builder.Services.AddCoreScopedServices();

        builder.Services.AddApiServices();
        builder.Services.AddDatabaseServices();

        return builder;
    }

    private static void AddBlazorServerCommon(this IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor();
    }

    private static void AddQualityOfLifeServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(BaseMapProfile));
        services.AddHangfire(x =>
        {
            x.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));
            x.UseDarkDashboard();
        });
        services.AddHangfireServer();
        services.AddMudServices();
    }

    private static void AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJwtAuthentication(configuration.GetApplicationSettings(services));
    }

    private static void AddCoreSingletonServices(this IServiceCollection services)
    {
        services.AddSingleton<IWeatherForecast, WeatherForecast>();
    }

    // ReSharper disable once UnusedParameter.Local
    private static void AddCoreTransientServices(this IServiceCollection services)
    {
        
    }

    // ReSharper disable once UnusedParameter.Local
    private static void AddCoreScopedServices(this IServiceCollection services)
    {
        
    }

    private static void AddApiServices(this IServiceCollection services)
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
    }

    private static void AddDatabaseServices(this IServiceCollection services)
    {
        services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
        services.AddSingleton<IExampleUserRepository, ExampleUserRepository>();
    }
    
    private static void AddJwtAuthentication(this IServiceCollection services, AppConfiguration config)
    {
        var key = Encoding.ASCII.GetBytes(config.Secret);
        services
            .AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearer =>
            {
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                bearer.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = c =>
                    {
                        if (c.Exception is SecurityTokenExpiredException)
                        {
                            c.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                            c.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Result.Fail("The Token is expired."));
                            return c.Response.WriteAsync(result);
                        }
                        else
                        {
                            c.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                            c.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Result.Fail("An unhandled error has occurred."));
                            return c.Response.WriteAsync(result);
                        }
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Result.Fail("You are not Authorized."));
                            return context.Response.WriteAsync(result);
                        }

                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(
                            Result.Fail("You are not authorized to access this resource."));
                        return context.Response.WriteAsync(result);
                    },
                };
            });
        services.AddAuthorization(options =>
        {
            // Enumerate permissions and create claim policies for them
            foreach (var permission in Permissions.GetRegisteredPermissions())
            {
                options.AddPolicy(permission, policy => policy.RequireClaim(
                    ApplicationClaimTypes.Permission, permission));
            }
        });
    }
}