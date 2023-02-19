namespace Application.Settings.AppSettings;

public class MailConfiguration : IAppSettingsSection
{
    internal static string SectionName => "Mail";
    public string? From { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? DisplayName { get; set; }
}