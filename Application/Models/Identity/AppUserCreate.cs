using Application.Helpers.Identity;
using Domain.DatabaseEntities.Identity;
using Shared.Enums.Identity;
using Shared.Requests.Identity.User;

namespace Application.Models.Identity;

public class AppUserCreate
{
    public string Username { get; set; } = null!;
    public string NormalizedUserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string NormalizedEmail { get; set; } = null!;
    public bool EmailConfirmed { get; set; } = false;
    public string PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;
    public string PhoneNumber { get; set; } = "";
    public bool PhoneNumberConfirmed { get; set; } = false;
    public bool TwoFactorEnabled { get; set; } = false;
    public string? FirstName { get; set; } = "";
    public string? LastName { get; set; } = "";
    public string? ProfilePictureDataUrl { get; set; } = "";
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedOn { get; set; }
    public bool IsActive { get; set; }
    public string? RefreshToken { get; set; } = null!;
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
            CreatedOn = appUser.CreatedOn,
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
    
    public static AppUserCreate ToCreateObject(this UserCreateRequest appUser)
    {
        AccountHelpers.GetPasswordHash(appUser.Password, out var salt, out var hash);
        
        return new AppUserCreate
        {
            Username = appUser.Username,
            NormalizedUserName = appUser.Username.NormalizeForDatabase(),
            Email = appUser.Email,
            NormalizedEmail = appUser.Email.NormalizeForDatabase(),
            EmailConfirmed = appUser.EmailConfirmed,
            PasswordHash = hash.ToString()!,
            PasswordSalt = salt,
            PhoneNumber = appUser.PhoneNumber,
            PhoneNumberConfirmed = appUser.PhoneNumberConfirmed,
            TwoFactorEnabled = false,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            ProfilePictureDataUrl = string.Empty,
            CreatedBy = Guid.Empty,
            CreatedOn = DateTime.Now,
            LastModifiedBy = Guid.Empty,
            LastModifiedOn = DateTime.Now,
            IsDeleted = false,
            DeletedOn = null,
            IsActive = appUser.IsActive,
            RefreshToken = string.Empty,
            RefreshTokenExpiryTime = DateTime.Now,
            AccountType = appUser.AccountType
        };
    }
}