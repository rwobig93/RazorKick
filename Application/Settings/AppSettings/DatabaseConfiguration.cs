namespace Application.Settings.AppSettings;

public class DatabaseConfiguration : IAppSettingsSection
{
    internal static string SectionName => "Database";
    public string Provider { get; set; } = "MsSql";
    public string Core { get; set; } = "data source=<hostname>;initial catalog=<database>;user id=<db_username>;password=<db_password>;";
}