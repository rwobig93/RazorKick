using Domain.DatabaseEntities.Identity;
using Shared.Responses.Identity;

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
    
    public static RoleResponse ToResponse(this AppRoleSlim appRole)
    {
        return new RoleResponse
        {
            Id = appRole.Id,
            Name = appRole.Name ?? "",
            Description = appRole.Description ?? ""
        };
    }
    
    public static List<RoleResponse> ToResponses(this IEnumerable<AppRoleSlim> appRoles)
    {
        return appRoles.Select(x => x.ToResponse()).ToList();
    }
    
    public static RoleFullResponse ToFullResponse(this AppRoleSlim appRole)
    {
        return new RoleFullResponse
        {
            Id = appRole.Id,
            Name = appRole.Name ?? "",
            Description = appRole.Description ?? "",
            Permissions = new List<PermissionResponse>()
        };
    }
    
    public static List<RoleFullResponse> ToFullResponses(this IEnumerable<AppRoleSlim> appRoles)
    {
        return appRoles.Select(x => x.ToFullResponse()).ToList();
    }
}