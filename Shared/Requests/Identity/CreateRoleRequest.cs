namespace Shared.Requests.Identity;

public class CreateRoleRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}