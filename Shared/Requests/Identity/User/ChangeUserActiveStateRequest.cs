namespace Shared.Requests.Identity.User;

public class ChangeUserActiveStateRequest
{
    public bool IsActive { get; set; }
    public Guid UserId { get; set; }
}