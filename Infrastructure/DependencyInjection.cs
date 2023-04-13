﻿using System.Net;
using System.Security.Claims;
using System.Text;
using Application.Constants.Identity;
using Application.Helpers.Runtime;
using Application.Models.Identity;
using Application.Models.Web;
using Application.Repositories.Example;
using Application.Repositories.Identity;
using Application.Services.Database;
using Application.Services.Example;
using Application.Services.Identity;
using Application.Services.System;
using Application.Settings.AppSettings;
using Asp.Versioning;
using Blazored.LocalStorage;
using Domain.DatabaseEntities.Identity;
using Hangfire;
using Hangfire.Dashboard.Dark.Core;
using Infrastructure.Repositories.Example;
using Infrastructure.Repositories.Identity;
using Infrastructure.Services.Database;
using Infrastructure.Services.Example;
using Infrastructure.Services.Identity;
using Infrastructure.Services.System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
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
        builder.Services.AddSettingsConfiguration(builder.Configuration);
        builder.Services.AddSystemServices(builder.Configuration);

        builder.Services.AddRepositories();
        builder.Services.AddApplicationServices();

        builder.Services.AddApiServices();
        builder.Services.AddAuthServices(builder.Configuration);
 
        builder.Services.AddDatabaseServices();

        return builder;
    }

    private static void AddBlazorServerCommon(this IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor();
    }

    private static void AddSettingsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        configuration.ConfigureMailSettings(services);
        configuration.ConfigureApplicationSettings(services);
    }

    private static void AddSystemServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(x =>
        {
            x.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));
            x.UseDarkDashboard();
        });
        services.AddHangfireServer();
        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
            config.SnackbarConfiguration.PreventDuplicates = true;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 6000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
        });
        
        services.AddBlazoredLocalStorage();
        services.AddHttpClient("Default", options =>
        {
            options.BaseAddress = new Uri(configuration.GetApplicationSettings().BaseUrl);
        }).ConfigureCertificateHandling(configuration);

        var mailConfig = configuration.GetMailSettings();

        services.AddFluentEmail(mailConfig.From, mailConfig.DisplayName)
            .AddRazorRenderer().AddSmtpSender(mailConfig.Host, mailConfig.Port, mailConfig.UserName, mailConfig.Password);
        
        services.AddSingleton<IRunningServerState, RunningServerState>();
        services.AddSingleton<ISerializerService, JsonSerializerService>();
        services.AddSingleton<IDateTimeService, DateTimeService>();
    }

    private static IHttpClientBuilder ConfigureCertificateHandling(this IHttpClientBuilder httpClientBuilder, IConfiguration configuration)
    {
        if (!configuration.GetApplicationSettings().TrustAllCertificates)
            return httpClientBuilder;
        
        return httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
        });
    }

    private static void AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        var appSettings = configuration.GetApplicationSettings();

        services.AddHttpContextAccessor();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(appSettings.SessionIdleTimeoutMinutes);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        services.AddScoped<AuthStateProvider>();
        services.AddTransient<AuthenticationStateProvider, AuthStateProvider>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAppAccountService, AppAccountService>();
        
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
            .AddIdentity<AppUserDb, AppRoleDb>(options =>
            {
                options.Password = UserConstants.PasswordRequirements;
                options.User = UserConstants.UserRequirements;
            })
            .AddUserStore<AppIdentityService>()
            .AddRoleStore<AppIdentityRoleService>()
            .AddUserManager<UserManagerService>()
            .AddDefaultTokenProviders();
        
        services.AddJwtAuthentication(appSettings);
        services.AddAuthorization(options =>
        {
            // Enumerate permissions and create claim policies for them
            foreach (var permission in PermissionConstants.GetAllPermissions())
            {
                // TODO: Use IAuthorizationPolicyProvider to add policies during runtime
                options.AddPolicy(permission, policy => policy.RequireClaim(
                    ApplicationClaimTypes.Permission, permission));
            }
        });
        services.Configure<SecurityStampValidatorOptions>(options =>
        {
            options.ValidationInterval = TimeSpan.FromSeconds(appSettings.PermissionValidationIntervalSeconds);
        });
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IBookRepository, BookRepository>();
        services.AddSingleton<IBookGenreRepository, BookGenreRepository>();
        services.AddSingleton<IBookReviewRepository, BookReviewRepository>();
        services.AddSingleton<IAppUserRepository, AppUserRepository>();
        services.AddSingleton<IAppRoleRepository, AppRoleRepository>();
        services.AddSingleton<IAppPermissionRepository, AppPermissionRepository>();
    }

    private static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IWeatherService, WeatherForecastService>();
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
        // TODO: Add more sql support, currently we only support MsSql
        services.AddSingleton<ISqlDataService, SqlDataServiceMsSql>();

        // Seeds the targeted database using the indicated provider on startup
        services.AddHostedService<SqlDatabaseSeederService>();
    }
    
    private static void AddJwtAuthentication(this IServiceCollection services, AppConfiguration config)
    {
        var key = Encoding.ASCII.GetBytes(config.Secret!);
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
                    NameClaimType = ClaimTypes.Name,
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
                        if (context.Response.HasStarted) return Task.CompletedTask;
                        
                        context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(Result.Fail("You are not Authorized."));
                        return context.Response.WriteAsync(result);

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
    }
}