using Domain.Contracts;

namespace Domain.Entities.Identity;

public class Claim : IAuditableEntity<Guid>
{
    public Guid Id { get; set; }
    
    public string? Description { get; set; }
    
    public string? Group { get; set; }
    
    public Guid CreatedBy { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public Guid? LastModifiedBy { get; set; }
    
    public DateTime? LastModifiedOn { get; set; }
    
    public virtual Role Role { get; set; } = null!;

    public Guid RoleId { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }
}