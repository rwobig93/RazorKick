using Application.Extensibility.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensibility.Extensions;

public static class ConfigurationExtensions
{
    // Configuring a class to bind to settings allows you to call it through dependency injection
    //   doing something like this: IOptions<AppConfiguration> appConfig or AppConfiguration like a service
    public static AppConfiguration GetApplicationSettings(
        this IConfiguration configuration, IServiceCollection services)
    {
        var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
        services.Configure<AppConfiguration>(applicationSettingsConfiguration);
        return applicationSettingsConfiguration.Get<AppConfiguration>();
    }
    
    public static MailConfiguration GetMailSettings(
        this IConfiguration configuration, IServiceCollection services)
    {
        var mailSettingsConfiguration = configuration.GetSection(nameof(MailConfiguration));
        services.Configure<MailConfiguration>(mailSettingsConfiguration);
        return mailSettingsConfiguration.Get<MailConfiguration>();
    }
}