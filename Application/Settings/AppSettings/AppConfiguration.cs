﻿namespace Application.Settings.AppSettings;

public class AppConfiguration : IAppSettingsSection
{
    internal static string SectionName => "General";
    public string? Secret { get; set; }
    public string? BaseUrl { get; set; }
    public int PermissionValidationIntervalSeconds { get; set; } = 5;
    public int TokenExpirationDays { get; set; } = 7;
    public int SessionIdleTimeoutMinutes { get; set; } = 240;
}