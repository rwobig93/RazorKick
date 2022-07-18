namespace Shared.Requests.Identity;

public class ChangeUserActiveStateRequest
{
    public bool IsActive { get; set; }
    public Guid UserId { get; set; }
}