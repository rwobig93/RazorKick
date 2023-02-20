namespace Domain.DatabaseEntities.Identity;

public class AppUserRoleJunction
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public virtual AppUser? User { get; set; }
    public virtual AppRole? Role { get; set; }
}