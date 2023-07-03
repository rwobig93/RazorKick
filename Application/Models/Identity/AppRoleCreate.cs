namespace Application.Models.Identity;

public class AppRoleCreate
{
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
    public string? ConcurrencyStamp { get; set; }
    public string? Description { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}