namespace Application.Models.Identity;

public class AppPermissionUpdate
{
    public Guid Id { get; set; }
    public Guid? RoleId { get; set; }
    public Guid? UserId { get; set; }
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Group { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}