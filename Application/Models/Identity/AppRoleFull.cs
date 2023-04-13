using Domain.DatabaseEntities.Identity;

namespace Application.Models.Identity;

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

public static class AppRoleFullExtensions
{
    public static AppRoleFull ToFull(this AppRoleDb roleDb)
    {
        return new AppRoleFull
        {
            Id = roleDb.Id,
            Name = roleDb.Name,
            Description = roleDb.Description,
            CreatedBy = roleDb.CreatedBy,
            CreatedOn = roleDb.CreatedOn,
            LastModifiedBy = roleDb.LastModifiedBy,
            LastModifiedOn = roleDb.LastModifiedOn
        };
    }

    public static AppRoleFull ToFull(this AppRoleSlim appRoleSlim)
    {
        return new AppRoleFull
        {
            Id = appRoleSlim.Id,
            Name = appRoleSlim.Name,
            Description = appRoleSlim.Description,
            CreatedBy = appRoleSlim.CreatedBy,
            CreatedOn = appRoleSlim.CreatedOn,
            LastModifiedBy = appRoleSlim.LastModifiedBy,
            LastModifiedOn = appRoleSlim.LastModifiedOn,
            Users = new List<AppUserSlim>(),
            Permissions = new List<AppPermissionSlim>()
        };
    }
}