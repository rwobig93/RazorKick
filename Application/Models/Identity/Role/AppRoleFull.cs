using Application.Models.Identity.Permission;
using Application.Models.Identity.User;

namespace Application.Models.Identity.Role;

public class AppRoleFull
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public List<AppUserSlim> Users { get; set; } = new();
    public List<AppPermissionSlim> Permissions { get; set; } = new();
}