using Domain.DatabaseEntities.Identity;

namespace Application.Models.Identity;

public class AppRoleSlim
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public static class AppRoleSlimExtensions
{
    public static AppRoleSlim ToSlim(this AppRoleDb appRoleDb)
    {
        return new AppRoleSlim
        {
            Id = appRoleDb.Id,
            Name = appRoleDb.Name,
            Description = appRoleDb.Description,
            CreatedBy = appRoleDb.CreatedBy,
            CreatedOn = appRoleDb.CreatedOn,
            LastModifiedBy = appRoleDb.LastModifiedBy,
            LastModifiedOn = appRoleDb.LastModifiedOn
        };
    }

    public static IEnumerable<AppRoleSlim> ToSlims(this IEnumerable<AppRoleDb> appRoleDbs)
    {
        return appRoleDbs.Select(x => x.ToSlim());
    }
}