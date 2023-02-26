namespace Shared.Responses.Identity;

public class PermissionResponse
{
    public new Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Group { get; set; } = "";
}