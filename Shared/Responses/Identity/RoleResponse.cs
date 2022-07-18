namespace Shared.Responses.Identity;

public class RoleResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsMember { get; set; }
}
