using Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity;

public class AppRole : IdentityRole<Guid>, IAuditableEntity<Guid>
{
    public override Guid Id { get; set; }
    public string? Description { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public ICollection<AppRoleClaim> Claims { get; set; } = new List<AppRoleClaim>();

    public override string ToString()
    {
        return Name;
    }
}