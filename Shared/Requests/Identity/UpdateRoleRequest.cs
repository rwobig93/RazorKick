namespace Shared.Requests.Identity;

public class UpdateRoleRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}