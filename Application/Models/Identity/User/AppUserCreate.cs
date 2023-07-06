using Domain.Enums.Identity;

namespace Application.Models.Identity.User;

public class AppUserCreate
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;
    public string PhoneNumber { get; set; } = "";
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public string? TwoFactorKey { get; set; }
    public string? FirstName { get; set; } = "";
    public string? LastName { get; set; } = "";
    public string? ProfilePictureDataUrl { get; set; } = "";
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
    public AuthState AuthState { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public AccountType AccountType { get; set; } = AccountType.User;
}