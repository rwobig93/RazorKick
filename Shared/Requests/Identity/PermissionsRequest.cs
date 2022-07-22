using Shared.Models.Identity;

namespace Shared.Requests.Identity;

public class PermissionsRequest
{
    public Guid RoleId { get; set; }
    private List<Permission> Permissions { get; set; } = new List<Permission>();
}