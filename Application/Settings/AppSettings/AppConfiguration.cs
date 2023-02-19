namespace Application.Settings.AppSettings;

public class AppConfiguration : IAppSettingsSection
{
    internal static string SectionName => "General";
    public string? Secret { get; set; }
    public string? BaseUrl { get; set; }
    public int PermissionValidationIntervalSeconds { get; set; } = 5;
}