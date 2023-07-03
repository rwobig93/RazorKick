using Domain.Enums.Identity;

namespace Application.Models.Identity;

public class AppUserSecurityAttributeUpdate
{
    public Guid Id { get; set; }
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
    public bool? TwoFactorEnabled { get; set; }
    public string? TwoFactorKey { get; set; }
    public AuthState? AuthState { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public int? BadPasswordAttempts { get; set; }
    public DateTime? LastBadPassword { get; set; }
}