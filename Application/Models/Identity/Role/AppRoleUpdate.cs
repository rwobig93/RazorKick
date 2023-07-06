namespace Application.Models.Identity.Role;

public class AppRoleUpdate
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? NormalizedName { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public string? Description { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}