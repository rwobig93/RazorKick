using Shared.Models.Identity;

namespace Shared.Responses.Identity;

public class PermissionsResponse
{
    public Guid RoleId { get; set; }
    public string? RoleName { get; set; }
    public List<Permission> CurrentPermissions { get; set; } = new List<Permission>();
}