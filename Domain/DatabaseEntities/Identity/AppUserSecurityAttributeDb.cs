﻿using Domain.Enums.Identity;

namespace Domain.DatabaseEntities.Identity;

public class AppUserSecurityAttributeDb
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;
    public bool TwoFactorEnabled { get; set; }
    public string? TwoFactorKey { get; set; }
    public AuthState AuthState { get; set; }
    public DateTime? AuthStateTimestamp { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public int BadPasswordAttempts { get; set; }
    public DateTime? LastBadPassword { get; set; }
    // TODO: Add this to the database and implement a check on this against SecurityConfiguration.ForceLoginIntervalMinutes
    public DateTime? LastFullLogin { get; set; }
}