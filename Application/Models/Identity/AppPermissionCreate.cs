using Application.Helpers.Identity;
using Shared.Requests.Identity.Permission;

namespace Application.Models.Identity;

public class AppPermissionCreate
{
    public Guid RoleId { get; set; }
    public Guid UserId { get; set; }
    public string ClaimType { get; set; } = "";
    public string ClaimValue { get; set; } = "";
    public string Name { get; set; } = "";
    public string Group { get; set; } = "";
    public string Access { get; set; } = "";
    public string Description { get; set; } = "";
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public static class AppPermissionCreateExtensions
{
    public static AppPermissionCreate ToCreate(this PermissionCreateForRoleRequest permissionCreate)
    {
        return new AppPermissionCreate
        {
            RoleId = permissionCreate.RoleId,
            ClaimType = ApplicationClaimTypes.Permission,
            ClaimValue = PermissionHelpers.GetClaimValueFromPermission(permissionCreate.Group, permissionCreate.Name, permissionCreate.Access) ?? "",
            Name = permissionCreate.Name,
            Description = permissionCreate.Description,
            Group = permissionCreate.Group,
            Access = permissionCreate.Access
        };
    }
    
    public static AppPermissionCreate ToCreate(this PermissionCreateForUserRequest permissionCreate)
    {
        return new AppPermissionCreate
        {
            UserId = permissionCreate.UserId,
            ClaimType = ApplicationClaimTypes.Permission,
            ClaimValue = PermissionHelpers.GetClaimValueFromPermission(permissionCreate.Group, permissionCreate.Name, permissionCreate.Access) ?? "",
            Name = permissionCreate.Name,
            Description = permissionCreate.Description,
            Group = permissionCreate.Group,
            Access = permissionCreate.Access
        };
    }

    public static AppPermissionCreate ToAppPermissionCreate(this string permissionValue)
    {
        if (permissionValue.Split('.').Length != 4)
            throw new Exception("Permission value provided doesn't match the correct syntax of Permissions.Group.Name.Access");

        var permissionName = PermissionHelpers.GetNameFromValue(permissionValue);
        var permissionGroup = PermissionHelpers.GetGroupFromValue(permissionValue);
        var permissionAccess = PermissionHelpers.GetAccessFromValue(permissionValue);
        
        return new AppPermissionCreate
        {
            RoleId = default,
            UserId = default,
            ClaimType = ApplicationClaimTypes.Permission,
            ClaimValue = permissionValue,
            Name = permissionName,
            Group = permissionGroup,
            Access = permissionAccess,
            Description = $"{permissionAccess} access to {permissionName} {permissionGroup}",
            CreatedBy = Guid.Empty,
            CreatedOn = DateTime.Now,
            LastModifiedBy = null,
            LastModifiedOn = null
        };
    }
}