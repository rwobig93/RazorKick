using Application.Settings;
using Application.Settings.AppSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Helpers.Runtime;

public static class AppConfigurationHelpers
{
    // Configuring a class to bind to settings allows you to call it through dependency injection
    //   doing something like this: IOptions<AppConfiguration> appConfig or AppConfiguration like a service
    public static AppConfiguration ConfigureApplicationSettings(
        this IConfiguration configuration, IServiceCollection services)
    {
        var applicationSettingsConfiguration = configuration.GetSection(AppConfiguration.SectionName);
        services.Configure<AppConfiguration>(applicationSettingsConfiguration);
        return applicationSettingsConfiguration.Get<AppConfiguration>();
    }

    public static AppConfiguration GetApplicationSettings(this IConfiguration configuration)
    {
        return configuration.GetSection(AppConfiguration.SectionName).Get<AppConfiguration>();
    }
    
    public static MailConfiguration ConfigureMailSettings(
        this IConfiguration configuration, IServiceCollection services)
    {
        var mailSettingsConfiguration = configuration.GetSection(MailConfiguration.SectionName);
        services.Configure<MailConfiguration>(mailSettingsConfiguration);
        return mailSettingsConfiguration.Get<MailConfiguration>();
    }

    public static MailConfiguration GetMailSettings(this IConfiguration configuration)
    {
        return configuration.GetSection(MailConfiguration.SectionName).Get<MailConfiguration>();
    }
    
    public static DatabaseConfiguration ConfigureDatabaseSettings(
        this IConfiguration configuration, IServiceCollection services)
    {
        var dbSettingsConfiguration = configuration.GetSection(DatabaseConfiguration.SectionName);
        services.Configure<MailConfiguration>(dbSettingsConfiguration);
        return dbSettingsConfiguration.Get<DatabaseConfiguration>();
    }

    public static DatabaseConfiguration GetDatabaseSettings(this IConfiguration configuration)
    {
        return configuration.GetSection(DatabaseConfiguration.SectionName).Get<DatabaseConfiguration>();
    }
}