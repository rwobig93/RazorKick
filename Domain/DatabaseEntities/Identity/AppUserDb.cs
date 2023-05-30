using Domain.Contracts;
using Shared.Enums.Identity;
using Shared.Responses.Identity;

namespace Domain.DatabaseEntities.Identity;

public class AppUserDb : IAuditableEntity<Guid>
{
    // TODO: Add bad password attempt count, locked out state, force reauthenticate, force token regen
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public bool PhoneNumberConfirmed { get; set; }
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
    public bool IsEnabled { get; set; }
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
            IsEnabled = appUser.IsEnabled
        };
    }

    public static List<UserBasicResponse> ToBasicResponses(this IEnumerable<AppUserDb> appUsers)
    {
        return appUsers.Select(x => x.ToBasicResponse()).ToList();
    }
}