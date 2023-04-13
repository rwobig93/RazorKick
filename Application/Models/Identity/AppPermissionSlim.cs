using Domain.DatabaseEntities.Identity;
using Shared.Responses.Identity;

namespace Application.Models.Identity;

public class AppPermissionSlim
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public string ClaimType { get; set; } = "";
    public string ClaimValue { get; set; } = "";
    public string Name { get; set; } = "";
    public string Group { get; set; } = "";
    public string Access { get; set; } = "";
    public string Description { get; set; } = "";
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public static class AppPermissionSlimExtensions
{
    public static AppPermissionSlim ToSlim(this AppPermissionDb appPermissionDb)
    {
        return new AppPermissionSlim
        {
            Id = appPermissionDb.Id,
            UserId = appPermissionDb.UserId,
            RoleId = appPermissionDb.RoleId,
            ClaimType = appPermissionDb.ClaimType,
            ClaimValue = appPermissionDb.ClaimValue,
            Name = appPermissionDb.Name,
            Group = appPermissionDb.Group,
            Access = appPermissionDb.Access,
            Description = appPermissionDb.Description,
            CreatedBy = appPermissionDb.CreatedBy,
            CreatedOn = appPermissionDb.CreatedOn,
            LastModifiedBy = appPermissionDb.LastModifiedBy,
            LastModifiedOn = appPermissionDb.LastModifiedOn
        };
    }

    public static IEnumerable<AppPermissionSlim> ToSlims(this IEnumerable<AppPermissionDb> appPermissionDbs)
    {
        return appPermissionDbs.Select(x => x.ToSlim());
    }
    
    public static PermissionResponse ToResponse(this AppPermissionSlim permission)
    {
        return new PermissionResponse
        {
            Id = permission.Id,
            UserId = permission.UserId,
            RoleId = permission.RoleId,
            Name = permission.Name,
            Description = permission.Description,
            Group = permission.Group
        };
    }

    public static List<PermissionResponse> ToResponses(this IEnumerable<AppPermissionSlim> permissions)
    {
        return permissions.Select(x => x.ToResponse()).ToList();
    }
}