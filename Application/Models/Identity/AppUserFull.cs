using Domain.DatabaseEntities.Identity;
using Shared.Enums.Identity;
using Shared.Responses.Identity;

namespace Application.Models.Identity;

public class AppUserFull
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
    public List<AppRoleSlim> Roles { get; set; } = new();
    public List<AppUserExtendedAttributeSlim> ExtendedAttributes { get; set; } = new();
    public List<AppPermissionSlim> Permissions { get; set; } = new();
}

public static class AppUserFullExtensions
{
    public static UserFullResponse ToFullResponse(this AppUserFull appUser)
    {
        return new UserFullResponse
        {
            Id = appUser.Id,
            Username = appUser.Username,
            CreatedOn = appUser.CreatedOn,
            IsActive = appUser.IsActive,
            AccountType = appUser.AccountType,
            ExtendedAttributes = appUser.ExtendedAttributes.ToResponses(),
            Permissions = appUser.Permissions.ToResponses()
        };
    }

    // TODO: Move all extension methods for all classes to consistent standard: All extensions go to the object they are on
    //       Example: Only ToFull() methods should be here, not ToSlim() or ToDB()
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
            IsActive = appUser.IsActive,
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
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            CreatedBy = appUser.CreatedBy,
            ProfilePictureDataUrl = appUser.ProfilePictureDataUrl,
            CreatedOn = appUser.CreatedOn,
            LastModifiedBy = appUser.LastModifiedBy,
            LastModifiedOn = appUser.LastModifiedOn,
            IsDeleted = appUser.IsDeleted,
            DeletedOn = appUser.DeletedOn,
            IsActive = appUser.IsActive,
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
            IsActive = appUser.IsActive,
            AccountType = appUser.AccountType,
            ExtendedAttributes = appUser.ExtendedAttributes.ToResponses(),
            Permissions = appUser.Permissions.ToResponses()
        };
    }

    public static List<UserFullResponse> ToResponses(this IEnumerable<AppUserFull> appUsers)
    {
        return appUsers.Select(x => x.ToResponse()).ToList();
    }
}