using Domain.Contracts;
using Microsoft.AspNetCore.Identity;
using Shared.Enums.Identity;
using Shared.Responses.Identity;

namespace Domain.DatabaseEntities.Identity;

public class AppUserDb : IdentityUser<Guid>, IAuditableEntity<Guid>
{
    // TODO: Add bad password attempt count, locked out state, force reauthenticate, force token regen
    public override Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Guid CreatedBy { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
    // TODO: Change to IsEnabled
    public bool IsActive { get; set; }
    public string? RefreshToken { get; set; }
    // TODO: Change to FullLoginRequiredTime
    public DateTime RefreshTokenExpiryTime { get; set; }
    public AccountType AccountType { get; set; } = AccountType.User;
}

public static class AppUserDbExtensions
{
    public static UserBasicResponse ToBasicResponse(this AppUserDb appUser)
    {
        return new UserBasicResponse()
        {
            Id = appUser.Id,
            Username = appUser.Username,
            CreatedOn = appUser.CreatedOn,
            IsActive = appUser.IsActive
        };
    }

    public static List<UserBasicResponse> ToBasicResponses(this IEnumerable<AppUserDb> appUsers)
    {
        return appUsers.Select(x => x.ToBasicResponse()).ToList();
    }
}