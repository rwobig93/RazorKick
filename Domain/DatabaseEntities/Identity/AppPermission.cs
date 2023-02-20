using Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Domain.DatabaseEntities.Identity;

public class AppPermission : IdentityRoleClaim<Guid>, IAuditableEntity<Guid>
{
    public Guid Id { get; set; }
    
    public string? Description { get; set; }
    
    public string? Group { get; set; }
    
    public Guid CreatedBy { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public Guid? LastModifiedBy { get; set; }
    
    public DateTime? LastModifiedOn { get; set; }

    public Guid RoleId { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }
}