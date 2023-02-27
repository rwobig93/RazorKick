using System.Net;
using System.Security.Claims;
using System.Text;
using Application.Helpers.Runtime;
using Application.Models.Web;
using Application.Repositories.Example;
using Application.Repositories.Identity;
using Application.Services.Database;
using Application.Services.Example;
using Application.Services.Identity;
using Application.Settings.AppSettings;
using Application.Settings.Identity;
using Asp.Versioning;
using Domain.DatabaseEntities.Identity;
using Hangfire;
using Hangfire.Dashboard.Dark.Core;
using Infrastructure.Repositories.Example;
using Infrastructure.Repositories.Identity;
using Infrastructure.Services.Database;
using Infrastructure.Services.Example;
using Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        builder.Services.AddSettingsConfiguration(builder.Configuration);
        builder.Services.AddCoreServices(builder.Configuration);

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

    private static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(x =>
        {
            x.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));
            x.UseDarkDashboard();
        });
        services.AddHangfireServer();
        services.AddMudServices();

        var mailConfig = configuration.GetMailSettings();

        services.AddFluentEmail(mailConfig.From, mailConfig.DisplayName)
            .AddRazorRenderer().AddSmtpSender(mailConfig.Host, mailConfig.Port, mailConfig.UserName, mailConfig.Password);
    }

    private static void AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        var appSettings = configuration.ConfigureApplicationSettings(services);

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IAppAccountService, AppAccountService>();
        
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
            .AddIdentity<AppUserDb, AppRoleDb>(options =>
            {
                options.Password.RequiredLength = 12;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddUserStore<AppIdentityService>()
            .AddRoleStore<AppIdentityRoleService>()
            .AddUserManager<UserManagerService>()
            .AddDefaultTokenProviders();
        
        // services.AddJwtAuthentication(appSettings);
        services.AddAuthorization(options =>
        {
            // Enumerate permissions and create claim policies for them
            foreach (var permission in Permissions.GetRegisteredPermissions())
            {
                options.AddPolicy(permission, policy => policy.RequireClaim(
                    ApplicationClaimTypes.Permission, permission));
            }
        });
        // services.Configure<SecurityStampValidatorOptions>(options =>
        // {
        //     options.ValidationInterval = TimeSpan.FromSeconds(appSettings.PermissionValidationIntervalSeconds);
        // });
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