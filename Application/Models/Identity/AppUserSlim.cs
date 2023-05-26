using Domain.DatabaseEntities.Identity;
using Shared.Enums.Identity;
using Shared.Responses.Identity;

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
    // TODO: Change to IsEnabled
    public bool IsActive { get; set; }
    public AccountType AccountType { get; set; } = AccountType.User;
}

public static class AppUserSlimExtensions
{
    public static AppUserSlim ToSlim(this AppUserFull appUserFull)
    {
        return new AppUserSlim
        {
            Id = appUserFull.Id,
            Username = appUserFull.Username,
            EmailAddress = appUserFull.EmailAddress,
            TwoFactorEnabled = appUserFull.TwoFactorEnabled,
            TwoFactorKey = appUserFull.TwoFactorKey,
            FirstName = appUserFull.FirstName,
            LastName = appUserFull.LastName,
            CreatedBy = appUserFull.CreatedBy,
            ProfilePictureDataUrl = appUserFull.ProfilePictureDataUrl,
            CreatedOn = appUserFull.CreatedOn,
            LastModifiedBy = appUserFull.LastModifiedBy,
            LastModifiedOn = appUserFull.LastModifiedOn,
            IsDeleted = appUserFull.IsDeleted,
            DeletedOn = appUserFull.DeletedOn,
            IsActive = appUserFull.IsActive,
            AccountType = appUserFull.AccountType
        };
    }

    public static AppUserSlim ToSlim(this AppUserDb appUserDb)
    {
        return new AppUserSlim
        {
            Id = appUserDb.Id,
            Username = appUserDb.Username,
            EmailAddress = appUserDb.Email,
            TwoFactorEnabled = appUserDb.TwoFactorEnabled,
            TwoFactorKey = appUserDb.TwoFactorKey,
            FirstName = appUserDb.FirstName,
            LastName = appUserDb.LastName,
            CreatedBy = appUserDb.CreatedBy,
            ProfilePictureDataUrl = appUserDb.ProfilePictureDataUrl,
            CreatedOn = appUserDb.CreatedOn,
            LastModifiedBy = appUserDb.LastModifiedBy,
            LastModifiedOn = appUserDb.LastModifiedOn,
            IsDeleted = appUserDb.IsDeleted,
            DeletedOn = appUserDb.DeletedOn,
            IsActive = appUserDb.IsEnabled,
            AccountType = appUserDb.AccountType
        };
    }

    public static IEnumerable<AppUserSlim> ToSlims(this IEnumerable<AppUserDb> appUserDbs)
    {
        return appUserDbs.Select(x => x.ToSlim());
    }
    
    public static UserBasicResponse ToResponse(this AppUserSlim appUser)
    {
        return new UserBasicResponse()
        {
            Id = appUser.Id,
            Username = appUser.Username,
            CreatedOn = appUser.CreatedOn,
            IsEnabled = appUser.IsActive
        };
    }

    public static List<UserBasicResponse> ToResponses(this IEnumerable<AppUserSlim> appUsers)
    {
        return appUsers.Select(x => x.ToResponse()).ToList();
    }
}