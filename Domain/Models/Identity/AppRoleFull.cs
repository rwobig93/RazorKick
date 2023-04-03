

using Domain.DatabaseEntities.Identity;

namespace Domain.Models.Identity;

public class AppRoleFull
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<AppUserDb> Users { get; set; } = new();
    public List<AppPermissionDb> Permissions { get; set; } = new();
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}