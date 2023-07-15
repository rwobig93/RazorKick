namespace Application.Settings.AppSettings;

public class LifecycleConfiguration : IAppSettingsSection
{
    public const string SectionName = "Lifecycle";
    
    public bool EnforceTestAccounts { get; set; }
    
    public bool AuditLoginLogout { get; set; }
}