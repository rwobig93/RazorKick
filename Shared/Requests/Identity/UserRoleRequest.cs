namespace Shared.Requests.Identity;

public class UserRoleRequest
{
    public Guid UserId { get; set; }
    public List<string> RoleNames { get; set; } = new List<string>();
}