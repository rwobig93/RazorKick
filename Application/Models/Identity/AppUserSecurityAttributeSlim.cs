using Domain.Enums.Identity;

namespace Application.Models.Identity;

public class AppUserSecurityAttributeSlim
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public AuthState AuthState { get; set; }
}