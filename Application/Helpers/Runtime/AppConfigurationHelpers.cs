using Application.Settings.AppSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Helpers.Runtime;

public static class AppConfigurationHelpers
{
    // Configuring a class to bind to settings allows you to call it through dependency injection
    //   doing something like this: IOptions<AppConfiguration> appConfig or AppConfiguration like a service
    public static AppConfiguration GetApplicationSettings(
        this IConfiguration configuration, IServiceCollection services)
    {
        var applicationSettingsConfiguration = configuration.GetSection(AppConfiguration.SectionName);
        services.Configure<AppConfiguration>(applicationSettingsConfiguration);
        return applicationSettingsConfiguration.Get<AppConfiguration>();
    }
    
    public static MailConfiguration GetMailSettings(
        this IConfiguration configuration, IServiceCollection services)
    {
        var mailSettingsConfiguration = configuration.GetSection(MailConfiguration.SectionName);
        services.Configure<MailConfiguration>(mailSettingsConfiguration);
        return mailSettingsConfiguration.Get<MailConfiguration>();
    }
}