using System.ComponentModel.DataAnnotations;

namespace Application.Settings.AppSettings;

public class AppConfiguration : IAppSettingsSection
{
    public const string SectionName = "General";
    
    public string ApplicationName { get; set; } = "TestBlazorServer";
    
    [Url]
    public string BaseUrl { get; init; } = "https://localhost:9500/";
}