using Domain.Contracts;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Shared.Enums.Identity;
using Shared.Responses.Identity;

namespace Domain.DatabaseEntities.Identity;

public class AppUser : IdentityUser<Guid>, IAuditableEntity<Guid>
{
    public override Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;
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
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public AccountType AccountType { get; set; } = AccountType.User;
    public List<AppRole> Roles { get; set; } = new();
    public List<ExtendedAttribute> ExtendedAttributes { get; set; } = new();
}

public static class AppUserExtensions
{
    public static UserBasicResponse ToBasicResponse(this AppUser appUser)
    {
        return new UserBasicResponse()
        {
            Id = appUser.Id,
            Username = appUser.Username,
            CreatedOn = appUser.CreatedOn,
            IsActive = appUser.IsActive
        };
    }
}