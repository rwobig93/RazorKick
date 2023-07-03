using Domain.Enums.Identity;

namespace Application.Models.Identity;

public class AppUserSlim
{
    // TODO: Add bad password attempt count, locked out state, force reauthenticate, force token regen
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string? EmailAddress { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public string? TwoFactorKey { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Guid CreatedBy { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
    public AuthState AuthState { get; set; }
    public AccountType AccountType { get; set; } = AccountType.User;
}