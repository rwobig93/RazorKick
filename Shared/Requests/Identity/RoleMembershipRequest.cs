namespace Shared.Requests.Identity;

public class RoleMembershipRequest
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}