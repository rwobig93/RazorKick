using Application.Helpers.Identity;
using Application.Models.Identity.Permission;
using Application.Models.Identity.Role;
using Application.Models.Identity.User;
using Application.Models.Identity.UserExtensions;
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
            FirstName = appUserFull.FirstName,
            LastName = appUserFull.LastName,
            CreatedBy = appUserFull.CreatedBy,
            ProfilePictureDataUrl = appUserFull.ProfilePictureDataUrl,
            CreatedOn = appUserFull.CreatedOn,
            LastModifiedBy = appUserFull.LastModifiedBy,
            LastModifiedOn = appUserFull.LastModifiedOn,
            IsDeleted = appUserFull.IsDeleted,
            DeletedOn = appUserFull.DeletedOn,
            AccountType = appUserFull.AccountType,
            AuthState = appUserFull.AuthState
        };
    }

    public static AppUserSlim ToSlim(this AppUserDb appUserDb)
    {
        return new AppUserSlim
        {
            Id = appUserDb.Id,
            Username = appUserDb.Username,
            EmailAddress = appUserDb.Email,
            FirstName = appUserDb.FirstName,
            LastName = appUserDb.LastName,
            CreatedBy = appUserDb.CreatedBy,
            ProfilePictureDataUrl = appUserDb.ProfilePictureDataUrl,
            CreatedOn = appUserDb.CreatedOn,
            LastModifiedBy = appUserDb.LastModifiedBy,
            LastModifiedOn = appUserDb.LastModifiedOn,
            IsDeleted = appUserDb.IsDeleted,
            DeletedOn = appUserDb.DeletedOn,
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
            CreatedOn = appUser.CreatedOn
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
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            CreatedBy = appUser.CreatedBy,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            CreatedOn = appUser.CreatedOn,
            LastModifiedBy = appUser.LastModifiedBy,
            LastModifiedOn = appUser.LastModifiedOn,
            IsDeleted = appUser.IsDeleted,
            DeletedOn = appUser.DeletedOn,
            AccountType = appUser.AccountType,
            Roles = new List<AppRoleSlim>(),
            ExtendedAttributes = new List<AppUserExtendedAttributeSlim>(),
            Permissions = new List<AppPermissionSlim>(),
            AuthState = appUser.AuthState
        };
    }

    public static AppUserFull ToFull(this AppUserFullDb appUser)
    {
        return new AppUserFull
        {
            Id = appUser.Id,
            Username = appUser.Username,
            EmailAddress = appUser.Email,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            CreatedBy = appUser.CreatedBy,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            CreatedOn = appUser.CreatedOn,
            LastModifiedBy = appUser.LastModifiedBy,
            LastModifiedOn = appUser.LastModifiedOn,
            IsDeleted = appUser.IsDeleted,
            DeletedOn = appUser.DeletedOn,
            AccountType = appUser.AccountType,
            Roles = new List<AppRoleSlim>(),
            ExtendedAttributes = new List<AppUserExtendedAttributeSlim>(),
            Permissions = new List<AppPermissionSlim>(),
            AuthState = appUser.AuthState
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
            CreatedOn = appUser.CreatedOn
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
            PhoneNumber = appUser.PhoneNumber,
            PhoneNumberConfirmed = appUser.PhoneNumberConfirmed,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            CreatedOn = appUser.CreatedOn,
            LastModifiedBy = appUser.LastModifiedBy,
            LastModifiedOn = appUser.LastModifiedOn,
            IsDeleted = appUser.IsDeleted,
            DeletedOn = appUser.DeletedOn,
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
            PhoneNumber = appUser.PhoneNumber,
            PhoneNumberConfirmed = appUser.PhoneNumberConfirmed,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            ProfilePictureDataUrl = string.Empty,
            CreatedBy = Guid.Empty,
            CreatedOn = DateTime.Now,
            LastModifiedBy = Guid.Empty,
            LastModifiedOn = DateTime.Now,
            IsDeleted = false,
            DeletedOn = null,
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
            PhoneNumber = appUser.PhoneNumber,
            PhoneNumberConfirmed = appUser.PhoneNumberConfirmed,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            LastModifiedBy = appUser.LastModifiedBy,
            LastModifiedOn = appUser.LastModifiedOn,
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
            PhoneNumber = null,
            PhoneNumberConfirmed = null,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            LastModifiedBy = null,
            LastModifiedOn = null,
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
            PhoneNumber = null,
            PhoneNumberConfirmed = null,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            LastModifiedBy = appUser.LastModifiedBy,
            LastModifiedOn = null,
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

    public static AppUserSecurityAttributeInfo ToInfo(this AppUserSecurityAttributeDb securityDb)
    {
        return new AppUserSecurityAttributeInfo
        {
            Id = securityDb.Id,
            OwnerId = securityDb.OwnerId,
            TwoFactorEnabled = securityDb.TwoFactorEnabled,
            AuthState = securityDb.AuthState,
            BadPasswordAttempts = securityDb.BadPasswordAttempts,
            LastBadPassword = securityDb.LastBadPassword
        };
    }

    public static AppUserSecurityAttributeUpdate ToUpdate(this AppUserSecurityAttributeDb securityDb)
    {
        return new AppUserSecurityAttributeUpdate
        {
            OwnerId = securityDb.OwnerId,
            PasswordHash = securityDb.PasswordHash,
            PasswordSalt = securityDb.PasswordSalt,
            TwoFactorEnabled = securityDb.TwoFactorEnabled,
            TwoFactorKey = securityDb.TwoFactorKey,
            AuthState = securityDb.AuthState,
            RefreshToken = securityDb.RefreshToken,
            RefreshTokenExpiryTime = securityDb.RefreshTokenExpiryTime,
            BadPasswordAttempts = securityDb.BadPasswordAttempts,
            LastBadPassword = securityDb.LastBadPassword
        };
    }

    public static AppUserSecurityFull ToUserSecurityFull(this AppUserSecurityDb securityDb)
    {
        return new AppUserSecurityFull
        {
            Id = securityDb.Id,
            Username = securityDb.Username,
            Email = securityDb.Email,
            EmailConfirmed = securityDb.EmailConfirmed,
            PhoneNumber = securityDb.PhoneNumber,
            PhoneNumberConfirmed = securityDb.PhoneNumberConfirmed,
            FirstName = securityDb.FirstName,
            LastName = securityDb.LastName,
            CreatedBy = securityDb.CreatedBy,
            ProfilePictureDataUrl = securityDb.ProfilePictureDataUrl,
            CreatedOn = securityDb.CreatedOn,
            LastModifiedBy = securityDb.LastModifiedBy,
            LastModifiedOn = securityDb.LastModifiedOn,
            IsDeleted = securityDb.IsDeleted,
            DeletedOn = securityDb.DeletedOn,
            AccountType = securityDb.AccountType,
            PasswordHash = securityDb.PasswordHash,
            PasswordSalt = securityDb.PasswordSalt,
            AuthState = securityDb.AuthState,
            AuthStateTimestamp = securityDb.AuthStateTimestamp,
            RefreshToken = securityDb.RefreshToken,
            RefreshTokenExpiryTime = securityDb.RefreshTokenExpiryTime,
            BadPasswordAttempts = securityDb.BadPasswordAttempts,
            LastBadPassword = securityDb.LastBadPassword,
            TwoFactorEnabled = securityDb.TwoFactorEnabled,
            TwoFactorKey = securityDb.TwoFactorKey
        };
    }
    
    public static AppUserUpdate ToUserUpdate(this AppUserSecurityDb appUser)
    {
        return new AppUserUpdate
        {
            Id = appUser.Id,
            Username = appUser.Username,
            Email = appUser.Email,
            EmailConfirmed = appUser.EmailConfirmed,
            PhoneNumber = appUser.PhoneNumber,
            PhoneNumberConfirmed = appUser.PhoneNumberConfirmed,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            LastModifiedBy = appUser.LastModifiedBy,
            LastModifiedOn = appUser.LastModifiedOn,
            AccountType = appUser.AccountType
        };
    }
    
    public static AppUserSecurityAttributeUpdate ToSecurityUpdate(this AppUserSecurityDb appUser)
    {
        return new AppUserSecurityAttributeUpdate
        {
            OwnerId = appUser.Id,
            PasswordHash = appUser.PasswordHash,
            PasswordSalt = appUser.PasswordSalt,
            TwoFactorEnabled = appUser.TwoFactorEnabled,
            TwoFactorKey = appUser.TwoFactorKey,
            AuthState = appUser.AuthState,
            RefreshToken = appUser.RefreshToken,
            RefreshTokenExpiryTime = appUser.RefreshTokenExpiryTime,
            BadPasswordAttempts = appUser.BadPasswordAttempts,
            LastBadPassword = appUser.LastBadPassword
        };
    }
    
    public static AppUserUpdate ToUserUpdate(this AppUserSecurityFull appUser)
    {
        return new AppUserUpdate
        {
            Id = appUser.Id,
            Username = appUser.Username,
            Email = appUser.Email,
            EmailConfirmed = appUser.EmailConfirmed,
            PhoneNumber = appUser.PhoneNumber,
            PhoneNumberConfirmed = appUser.PhoneNumberConfirmed,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            LastModifiedBy = appUser.LastModifiedBy,
            LastModifiedOn = appUser.LastModifiedOn,
            AccountType = appUser.AccountType
        };
    }
    
    public static AppUserSecurityAttributeUpdate ToSecurityUpdate(this AppUserSecurityFull appUser)
    {
        return new AppUserSecurityAttributeUpdate
        {
            OwnerId = appUser.Id,
            PasswordHash = appUser.PasswordHash,
            PasswordSalt = appUser.PasswordSalt,
            TwoFactorEnabled = appUser.TwoFactorEnabled,
            TwoFactorKey = appUser.TwoFactorKey,
            AuthState = appUser.AuthState,
            RefreshToken = appUser.RefreshToken,
            RefreshTokenExpiryTime = appUser.RefreshTokenExpiryTime,
            BadPasswordAttempts = appUser.BadPasswordAttempts,
            LastBadPassword = appUser.LastBadPassword
        };
    }

    public static AppUserDb ToUserDb(this AppUserSecurityDb securityDb)
    {
        return new AppUserDb
        {
            Id = securityDb.Id,
            Username = securityDb.Username,
            Email = securityDb.Email,
            EmailConfirmed = securityDb.EmailConfirmed,
            PhoneNumber = securityDb.PhoneNumber,
            PhoneNumberConfirmed = securityDb.PhoneNumberConfirmed,
            FirstName = securityDb.FirstName,
            LastName = securityDb.LastName,
            CreatedBy = securityDb.CreatedBy,
            ProfilePictureDataUrl = securityDb.ProfilePictureDataUrl,
            CreatedOn = securityDb.CreatedOn,
            LastModifiedBy = securityDb.LastModifiedBy,
            LastModifiedOn = securityDb.LastModifiedOn,
            IsDeleted = securityDb.IsDeleted,
            DeletedOn = securityDb.DeletedOn,
            AccountType = securityDb.AccountType
        };
    }

    public static AppUserUpdate ToUpdate(this AppUserSecurityDb userDb)
    {
        return new AppUserUpdate
        {
            Id = userDb.Id,
            Username = userDb.Username,
            Email = userDb.Email,
            EmailConfirmed = userDb.EmailConfirmed,
            PhoneNumber = userDb.PhoneNumber,
            PhoneNumberConfirmed = userDb.PhoneNumberConfirmed,
            FirstName = userDb.FirstName,
            LastName = userDb.LastName,
            ProfilePictureDataUrl = userDb.ProfilePictureDataUrl,
            LastModifiedBy = userDb.LastModifiedBy,
            LastModifiedOn = userDb.LastModifiedOn,
            AccountType = userDb.AccountType
        };
    }

    public static AppUserSlim ToSlim(this AppUserSecurityDb userDb)
    {
        return new AppUserSlim
        {
            Id = userDb.Id,
            Username = userDb.Username,
            EmailAddress = userDb.Email,
            FirstName = userDb.FirstName,
            LastName = userDb.FirstName,
            CreatedBy = userDb.CreatedBy,
            ProfilePictureDataUrl = userDb.ProfilePictureDataUrl,
            CreatedOn = userDb.CreatedOn,
            LastModifiedBy = userDb.LastModifiedBy,
            LastModifiedOn = userDb.LastModifiedOn,
            IsDeleted = userDb.IsDeleted,
            DeletedOn = userDb.DeletedOn,
            AccountType = userDb.AccountType,
            AuthState = userDb.AuthState
        };
    }

    public static IEnumerable<AppUserSlim> ToSlims(this IEnumerable<AppUserSecurityDb> userDbs)
    {
        return userDbs.Select(x => x.ToSlim()).ToList();
    }

    public static UserBasicResponse ToBasicResponse(this AppUserSecurityDb userDb)
    {
        return new UserBasicResponse
        {
            Id = userDb.Id,
            Username = userDb.Username,
            CreatedOn = userDb.CreatedOn,
            AuthState = userDb.AuthState,
            AccountType = userDb.AccountType
        };
    }

    public static AppUserFull ToUserFull(this AppUserSecurityDb userDb)
    {
        return new AppUserFull
        {
            Id = userDb.Id,
            Username = userDb.Username,
            EmailAddress = userDb.Email,
            FirstName = userDb.FirstName,
            LastName = userDb.LastName,
            CreatedBy = userDb.CreatedBy,
            ProfilePictureDataUrl = userDb.ProfilePictureDataUrl,
            CreatedOn = userDb.CreatedOn,
            LastModifiedBy = userDb.LastModifiedBy,
            LastModifiedOn = userDb.LastModifiedOn,
            IsDeleted = userDb.IsDeleted,
            DeletedOn = userDb.DeletedOn,
            AccountType = userDb.AccountType,
            Roles = new List<AppRoleSlim>(),
            ExtendedAttributes = new List<AppUserExtendedAttributeSlim>(),
            Permissions = new List<AppPermissionSlim>(),
            AuthState = userDb.AuthState
        };
    }
}