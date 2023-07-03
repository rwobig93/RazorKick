using Application.Helpers.Identity;
using Application.Models.Identity;
using Application.Requests.Identity.User;
using Application.Responses.Identity;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Identity;
using Domain.Models.Identity;
using Newtonsoft.Json;

namespace Application.Mappers.Identity;

public static class UserMappers
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
            AuthState = appUserFull.AuthState,
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
            AuthState = appUserDb.AuthState,
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
            AuthState = appUser.AuthState
        };
    }

    public static List<UserBasicResponse> ToResponses(this IEnumerable<AppUserSlim> appUsers)
    {
        return appUsers.Select(x => x.ToResponse()).ToList();
    }
    
    public static UserFullResponse ToFullResponse(this AppUserFull appUser)
    {
        return new UserFullResponse
        {
            Id = appUser.Id,
            Username = appUser.Username,
            CreatedOn = appUser.CreatedOn,
            AuthState = appUser.AuthState,
            AccountType = appUser.AccountType,
            ExtendedAttributes = appUser.ExtendedAttributes.ToResponses(),
            Permissions = appUser.Permissions.ToResponses()
        };
    }

    public static AppUserFull ToFull(this AppUserSlim appUser)
    {
        return new AppUserFull
        {
            Id = appUser.Id,
            Username = appUser.Username,
            EmailAddress = appUser.EmailAddress,
            TwoFactorEnabled = appUser.TwoFactorEnabled,
            TwoFactorKey = appUser.TwoFactorKey,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            CreatedBy = appUser.CreatedBy,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            CreatedOn = appUser.CreatedOn,
            LastModifiedBy = appUser.LastModifiedBy,
            LastModifiedOn = appUser.LastModifiedOn,
            IsDeleted = appUser.IsDeleted,
            DeletedOn = appUser.DeletedOn,
            AuthState = appUser.AuthState,
            AccountType = appUser.AccountType,
            Roles = new List<AppRoleSlim>(),
            ExtendedAttributes = new List<AppUserExtendedAttributeSlim>(),
            Permissions = new List<AppPermissionSlim>()
        };
    }

    public static AppUserFull ToFull(this AppUserDb appUser)
    {
        return new AppUserFull
        {
            Id = appUser.Id,
            Username = appUser.Username,
            EmailAddress = appUser.Email,
            TwoFactorEnabled = appUser.TwoFactorEnabled,
            TwoFactorKey = appUser.TwoFactorKey,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            CreatedBy = appUser.CreatedBy,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            CreatedOn = appUser.CreatedOn,
            LastModifiedBy = appUser.LastModifiedBy,
            LastModifiedOn = appUser.LastModifiedOn,
            IsDeleted = appUser.IsDeleted,
            DeletedOn = appUser.DeletedOn,
            AuthState = appUser.AuthState,
            AccountType = appUser.AccountType,
            Roles = new List<AppRoleSlim>(),
            ExtendedAttributes = new List<AppUserExtendedAttributeSlim>(),
            Permissions = new List<AppPermissionSlim>()
        };
    }

    public static AppUserFull ToFull(this AppUserFullDb appUser)
    {
        return new AppUserFull
        {
            Id = appUser.Id,
            Username = appUser.Username,
            EmailAddress = appUser.Email,
            TwoFactorEnabled = appUser.TwoFactorEnabled,
            TwoFactorKey = appUser.TwoFactorKey,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            CreatedBy = appUser.CreatedBy,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            CreatedOn = appUser.CreatedOn,
            LastModifiedBy = appUser.LastModifiedBy,
            LastModifiedOn = appUser.LastModifiedOn,
            IsDeleted = appUser.IsDeleted,
            DeletedOn = appUser.DeletedOn,
            AuthState = appUser.AuthState,
            AccountType = appUser.AccountType,
            Roles = new List<AppRoleSlim>(),
            ExtendedAttributes = new List<AppUserExtendedAttributeSlim>(),
            Permissions = new List<AppPermissionSlim>()
        };
    }
    
    public static UserFullResponse ToResponse(this AppUserFull appUser)
    {
        return new UserFullResponse
        {
            Id = appUser.Id,
            Username = appUser.Username,
            CreatedOn = appUser.CreatedOn,
            AuthState = appUser.AuthState,
            AccountType = appUser.AccountType,
            ExtendedAttributes = appUser.ExtendedAttributes.ToResponses(),
            Permissions = appUser.Permissions.ToResponses()
        };
    }

    public static List<UserFullResponse> ToResponses(this IEnumerable<AppUserFull> appUsers)
    {
        return appUsers.Select(x => x.ToResponse()).ToList();
    }
    
    public static UserBasicResponse ToBasicResponse(this AppUserDb appUser)
    {
        return new UserBasicResponse()
        {
            Id = appUser.Id,
            Username = appUser.Username,
            CreatedOn = appUser.CreatedOn,
            AuthState = appUser.AuthState
        };
    }

    public static List<UserBasicResponse> ToBasicResponses(this IEnumerable<AppUserDb> appUsers)
    {
        return appUsers.Select(x => x.ToBasicResponse()).ToList();
    }
    
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
            AuthState = appUser.AuthState,
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
            AuthState = appUser.AuthState,
            RefreshToken = string.Empty,
            RefreshTokenExpiryTime = DateTime.Now,
            AccountType = appUser.AccountType
        };
    }
    
    public static AppUserExtendedAttributeSlim ToSlim(this AppUserExtendedAttributeDb extendedAttributeDb)
    {
        return new AppUserExtendedAttributeSlim
        {
            Id = extendedAttributeDb.Id,
            OwnerId = extendedAttributeDb.OwnerId,
            Name = extendedAttributeDb.Name,
            Value = extendedAttributeDb.Value,
            Type = extendedAttributeDb.Type
        };
    }

    public static IEnumerable<AppUserExtendedAttributeSlim> ToSlims(this IEnumerable<AppUserExtendedAttributeDb> extendedAttributeDbs)
    {
        return extendedAttributeDbs.Select(x => x.ToSlim());
    }
    
    public static ExtendedAttributeResponse ToResponse(this AppUserExtendedAttributeSlim attribute)
    {
        return new ExtendedAttributeResponse
        {
            Id = attribute.Id,
            OwnerId = attribute.OwnerId,
            Name = attribute.Name,
            Value = attribute.Value,
            Type = attribute.Type
        };
    }

    public static List<ExtendedAttributeResponse> ToResponses(this IEnumerable<AppUserExtendedAttributeSlim> attributes)
    {
        return attributes.Select(x => x.ToResponse()).ToList();
    }
    public static AppUserUpdate ToUpdate(this AppUserDb appUser)
    {
        return new AppUserUpdate
        {
            Id = appUser.Id,
            Username = appUser.Username,
            Email = appUser.Email,
            EmailConfirmed = appUser.EmailConfirmed,
            PasswordHash = appUser.PasswordHash,
            PasswordSalt = appUser.PasswordSalt,
            PhoneNumber = appUser.PhoneNumber,
            PhoneNumberConfirmed = appUser.PhoneNumberConfirmed,
            TwoFactorEnabled = appUser.TwoFactorEnabled,
            TwoFactorKey = appUser.TwoFactorKey,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            LastModifiedBy = appUser.LastModifiedBy,
            LastModifiedOn = appUser.LastModifiedOn,
            AuthState = appUser.AuthState,
            RefreshToken = appUser.RefreshToken,
            RefreshTokenExpiryTime = appUser.RefreshTokenExpiryTime,
            AccountType = appUser.AccountType
        };
    }
    
    public static AppUserUpdate ToUpdate(this UserUpdateRequest appUser)
    {
        return new AppUserUpdate
        {
            Id = appUser.Id,
            Username = null,
            Email = null,
            EmailConfirmed = null,
            PasswordHash = null,
            PasswordSalt = null,
            PhoneNumber = null,
            PhoneNumberConfirmed = null,
            TwoFactorEnabled = null,
            TwoFactorKey = null,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            LastModifiedBy = null,
            LastModifiedOn = null,
            AuthState = null,
            RefreshToken = null,
            RefreshTokenExpiryTime = null,
            AccountType = AccountType.User,

        };
    }

    public static AppUserUpdate ToUpdate(this AppUserFull appUser)
    {
        return new AppUserUpdate
        {
            Id = appUser.Id,
            Username = null,
            Email = null,
            EmailConfirmed = null,
            PasswordHash = null,
            PasswordSalt = null,
            PhoneNumber = null,
            PhoneNumberConfirmed = null,
            TwoFactorEnabled = null,
            TwoFactorKey = null,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            LastModifiedBy = appUser.LastModifiedBy,
            LastModifiedOn = null,
            AuthState = appUser.AuthState,
            RefreshToken = null,
            RefreshTokenExpiryTime = null,
            AccountType = AccountType.User
        };
    }
    
    public static ExtendedAttributeResponse ToResponse(this AppUserExtendedAttributeDb attribute)
    {
        return new ExtendedAttributeResponse
        {
            Id = attribute.Id,
            OwnerId = attribute.OwnerId,
            Name = attribute.Name,
            Value = attribute.Value,
            Type = attribute.Type
        };
    }

    public static List<ExtendedAttributeResponse> ToResponses(this IEnumerable<AppUserExtendedAttributeDb> attributes)
    {
        return attributes.Select(x => x.ToResponse()).ToList();
    }
    
    public static AppUserPreferenceFull ToFull(this AppUserPreferenceDb preferenceDb)
    {
        return new AppUserPreferenceFull
        {
            Id = preferenceDb.Id,
            OwnerId = preferenceDb.OwnerId,
            ThemePreference = preferenceDb.ThemePreference,
            DrawerDefaultOpen = preferenceDb.DrawerDefaultOpen,
            CustomThemeOne = AppThemeCustom.GetExampleCustomOne(),
            CustomThemeTwo = AppThemeCustom.GetExampleCustomTwo(),
            CustomThemeThree = AppThemeCustom.GetExampleCustomThree()
        };
    }

    public static AppUserPreferenceDb ToDb(this AppUserPreferenceFull preferenceFull)
    {
        return new AppUserPreferenceDb
        {
            Id = preferenceFull.Id,
            OwnerId = preferenceFull.OwnerId,
            ThemePreference = preferenceFull.ThemePreference,
            DrawerDefaultOpen = preferenceFull.DrawerDefaultOpen,
            CustomThemeOne = string.Empty,
            CustomThemeTwo = string.Empty,
            CustomThemeThree = string.Empty
        };
    }
    
    public static AppUserPreferenceCreate ToCreate(this AppUserPreferenceDb preferenceDb)
    {
        return new AppUserPreferenceCreate
        {
            OwnerId = preferenceDb.OwnerId,
            ThemePreference = preferenceDb.ThemePreference,
            DrawerDefaultOpen = preferenceDb.DrawerDefaultOpen,
            CustomThemeOne = preferenceDb.CustomThemeOne,
            CustomThemeTwo = preferenceDb.CustomThemeTwo,
            CustomThemeThree = preferenceDb.CustomThemeThree
        };
    }

    public static AppUserPreferenceCreate ToCreate(this AppUserPreferenceUpdate preferenceUpdate)
    {
        return new AppUserPreferenceCreate
        {
            ThemePreference = preferenceUpdate.ThemePreference,
            DrawerDefaultOpen = preferenceUpdate.DrawerDefaultOpen,
            CustomThemeOne = preferenceUpdate.CustomThemeOne,
            CustomThemeTwo = preferenceUpdate.CustomThemeTwo,
            CustomThemeThree = preferenceUpdate.CustomThemeThree
        };
    }

    public static AppUserPreferenceDb ToDb(this AppUserPreferenceCreate preferenceCreate)
    {
        return new AppUserPreferenceDb
        {
            OwnerId = preferenceCreate.OwnerId,
            ThemePreference = preferenceCreate.ThemePreference,
            DrawerDefaultOpen = preferenceCreate.DrawerDefaultOpen,
            CustomThemeOne = preferenceCreate.CustomThemeOne,
            CustomThemeTwo = preferenceCreate.CustomThemeTwo,
            CustomThemeThree = preferenceCreate.CustomThemeThree
        };
    }
    
    public static AppUserPreferenceDb ToDb(this AppUserPreferenceUpdate preferenceUpdate)
    {
        return new AppUserPreferenceDb
        {
            Id = Guid.Empty,
            OwnerId = preferenceUpdate.OwnerId,
            ThemePreference = preferenceUpdate.ThemePreference,
            DrawerDefaultOpen = preferenceUpdate.DrawerDefaultOpen,
            CustomThemeOne = preferenceUpdate.CustomThemeOne,
            CustomThemeTwo = preferenceUpdate.CustomThemeTwo,
            CustomThemeThree = preferenceUpdate.CustomThemeThree
        };
    }
    
    public static AppUserPreferenceUpdate ToUpdate(this AppUserPreferenceDb preferenceDb)
    {
        return new AppUserPreferenceUpdate
        {
            Id = preferenceDb.Id,
            OwnerId = preferenceDb.OwnerId,
            ThemePreference = preferenceDb.ThemePreference,
            DrawerDefaultOpen = preferenceDb.DrawerDefaultOpen,
            CustomThemeOne = preferenceDb.CustomThemeOne,
            CustomThemeTwo = preferenceDb.CustomThemeTwo,
            CustomThemeThree = preferenceDb.CustomThemeThree
        };
    }
    
    public static AppUserPreferenceUpdate ToUpdate(this AppUserPreferenceFull preferenceDb)
    {
        return new AppUserPreferenceUpdate
        {
            Id = preferenceDb.Id,
            OwnerId = preferenceDb.OwnerId,
            ThemePreference = preferenceDb.ThemePreference,
            DrawerDefaultOpen = preferenceDb.DrawerDefaultOpen,
            CustomThemeOne = JsonConvert.SerializeObject(preferenceDb.CustomThemeOne),
            CustomThemeTwo = JsonConvert.SerializeObject(preferenceDb.CustomThemeTwo),
            CustomThemeThree = JsonConvert.SerializeObject(preferenceDb.CustomThemeThree)
        };
    }
}