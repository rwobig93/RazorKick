using Application.Helpers.Identity;
using Application.Settings.Identity;
using Shared.Requests.Identity.Permission;

namespace Application.Models.Identity;

public class AppPermissionUpdate
{
    public Guid Id { get; set; }
    public Guid? RoleId { get; set; }
    public Guid? UserId { get; set; }
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }
    public string? Name { get; set; }
    public string? Group { get; set; }
    public string? Access { get; set; }
    public string? Description { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public static class AppPermissionUpdateExtensions
{
    public static AppPermissionUpdate ToUpdate(this PermissionUpdateRequest permissionUpdate)
    {
        return new AppPermissionUpdate
        {
            Id = permissionUpdate.Id,
            ClaimType = ApplicationClaimTypes.Permission,
            ClaimValue = PermissionHelpers.GetClaimValueFromPermission(permissionUpdate.Group, permissionUpdate.Name, permissionUpdate.Access),
            Name = permissionUpdate.Name,
            Description = permissionUpdate.Description,
            Group = permissionUpdate.Group,
            Access = permissionUpdate.Access
        };
    }
}