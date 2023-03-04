using Domain.DatabaseEntities.Identity;
using Shared.Enums.Identity;
using Shared.Requests.Identity.User;

namespace Application.Models.Identity;

public class AppUserUpdate
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? NormalizedUserName { get; set; }
    public string? Email { get; set; }
    public string? NormalizedEmail { get; set; }
    public bool? EmailConfirmed { get; set; }
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? PhoneNumberConfirmed { get; set; }
    public bool? TwoFactorEnabled { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
    public bool? IsActive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public AccountType AccountType { get; set; } = AccountType.User;
}

public static class AppUserUpdateExtensions
{
    public static AppUserUpdate ToUpdateObject(this AppUserDb appUser)
    {
        return new AppUserUpdate
        {
            Username = appUser.Username,
            NormalizedUserName = appUser.NormalizedUserName,
            Email = appUser.Email,
            NormalizedEmail = appUser.NormalizedEmail,
            EmailConfirmed = appUser.EmailConfirmed,
            PasswordHash = appUser.PasswordHash,
            PasswordSalt = appUser.PasswordSalt,
            PhoneNumber = appUser.PhoneNumber,
            PhoneNumberConfirmed = appUser.PhoneNumberConfirmed,
            TwoFactorEnabled = appUser.TwoFactorEnabled,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            LastModifiedBy = appUser.LastModifiedBy,
            LastModifiedOn = appUser.LastModifiedOn,
            IsDeleted = appUser.IsDeleted,
            DeletedOn = appUser.DeletedOn,
            IsActive = appUser.IsActive,
            RefreshToken = appUser.RefreshToken,
            RefreshTokenExpiryTime = appUser.RefreshTokenExpiryTime,
            AccountType = appUser.AccountType
        };
    }
    
    public static AppUserUpdate ToUpdateObject(this UserUpdateRequest appUser)
    {
        return new AppUserUpdate
        {
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            IsActive = appUser.IsActive,
        };
    }
}