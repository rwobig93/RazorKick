﻿using System.ComponentModel.DataAnnotations;

namespace Application.Settings.AppSettings;

public class AppConfiguration : IAppSettingsSection
{
    public const string SectionName = "General";
    
    public string ApplicationName { get; set; } = "TestBlazorServer";

    [Required]
    [MinLength(32)]
    [MaxLength(128)]
    public string Secret { get; init; } = null!;
    
    [Url]
    public string BaseUrl { get; init; } = "https://localhost:9500/";
    
    [Range(0, 2_592_000)]
    public int PermissionValidationIntervalSeconds { get; init; } = 5;
    
    [Range(1, 5_184_000)]
    public int TokenExpirationMinutes { get; init; } = 15;
    
    [Range(0, 86_400)]
    public int SessionIdleTimeoutMinutes { get; init; } = 240;
    
    [Range(1, 100)]
    public int MaxBadPasswordAttempts { get; init; } = 3;
    
    public bool TrustAllCertificates { get; set; }
    
    public bool EnforceNonSystemAndAdminAccounts { get; set; }
    
    public bool AuditLoginLogout { get; set; }
}