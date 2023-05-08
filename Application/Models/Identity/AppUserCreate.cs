using Application.Helpers.Identity;
using Domain.DatabaseEntities.Identity;
using Shared.Enums.Identity;
using Shared.Requests.Identity.User;

namespace Application.Models.Identity;

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
    public bool IsEnabled { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public AccountType AccountType { get; set; } = AccountType.User;
}

public static class AppUserCreateExtensions
{
    public static AppUserCreate ToCreateObject(this AppUserDb appUser)
    {
        return new AppUserCreate
        {
            Username = appUser.Username,
            Email = appUser.Email,
            EmailConfirmed = appUser.EmailConfirmed,
            PasswordHash = appUser.PasswordHash,
            PasswordSalt = appUser.PasswordSalt,
            PhoneNumber = appUser.PhoneNumber,
            PhoneNumberConfirmed = appUser.PhoneNumberConfirmed,
            TwoFactorEnabled = appUser.TwoFactorEnabled,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            CreatedOn = appUser.CreatedOn,
            LastModifiedBy = appUser.LastModifiedBy,
            LastModifiedOn = appUser.LastModifiedOn,
            IsDeleted = appUser.IsDeleted,
            DeletedOn = appUser.DeletedOn,
            IsEnabled = appUser.IsEnabled,
            RefreshToken = appUser.RefreshToken,
            RefreshTokenExpiryTime = appUser.RefreshTokenExpiryTime,
            AccountType = appUser.AccountType
        };
    }
    
    public static AppUserCreate ToCreateObject(this UserCreateRequest appUser)
    {
        AccountHelpers.GenerateHashAndSalt(appUser.Password, out var salt, out var hash);
        
        return new AppUserCreate
        {
            Username = appUser.Username,
            Email = appUser.Email,
            EmailConfirmed = appUser.EmailConfirmed,
            PasswordHash = hash,
            PasswordSalt = salt,
            PhoneNumber = appUser.PhoneNumber,
            PhoneNumberConfirmed = appUser.PhoneNumberConfirmed,
            TwoFactorEnabled = false,
            TwoFactorKey = null,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            ProfilePictureDataUrl = string.Empty,
            CreatedBy = Guid.Empty,
            CreatedOn = DateTime.Now,
            LastModifiedBy = Guid.Empty,
            LastModifiedOn = DateTime.Now,
            IsDeleted = false,
            DeletedOn = null,
            IsEnabled = appUser.IsActive,
            RefreshToken = string.Empty,
            RefreshTokenExpiryTime = DateTime.Now,
            AccountType = appUser.AccountType
        };
    }
}