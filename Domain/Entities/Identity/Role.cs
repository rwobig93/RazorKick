using Domain.Contracts;

namespace Domain.Entities.Identity;

public class Role : IAuditableEntity<Guid>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? NormalizedName { get; set; }

    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    
    public string? Description { get; set; }
    
    public Guid CreatedBy { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public Guid? LastModifiedBy { get; set; }
    
    public DateTime? LastModifiedOn { get; set; }

    public ICollection<Claim> Claims { get; set; } = new List<Claim>();

    public override string ToString()
    {
        return Name;
    }
}