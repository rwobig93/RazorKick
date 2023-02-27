namespace Shared.Requests.Identity.Permission;

public class PermissionRemoveFromRoleRequest
{
    public Guid RoleId { get; set; } = Guid.Empty;
    public string PermissionValue { get; set; } = null!;
}