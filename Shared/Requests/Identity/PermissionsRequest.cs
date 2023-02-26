using Shared.Responses.Identity;

namespace Shared.Requests.Identity;

public class PermissionsRequest
{
    public Guid RoleId { get; set; }
    private List<PermissionResponse> Permissions { get; set; } = new List<PermissionResponse>();
}