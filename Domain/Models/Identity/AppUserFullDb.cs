using Domain.DatabaseEntities.Identity;
using Shared.Enums.Identity;

namespace Domain.Models.Identity;

public class AppUserFullDb
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string? EmailAddress { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Guid CreatedBy { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
    public bool IsActive { get; set; }
    public AccountType AccountType { get; set; } = AccountType.User;
    public List<AppRoleDb> Roles { get; set; } = new();
    public List<AppUserExtendedAttributeDb> ExtendedAttributes { get; set; } = new();
    public List<AppPermissionDb> Permissions { get; set; } = new();
}