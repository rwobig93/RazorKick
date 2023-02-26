using System.Security.Claims;
using Domain.Contracts;
using Microsoft.AspNetCore.Identity;
using Shared.Responses.Identity;

namespace Domain.DatabaseEntities.Identity;

public class AppPermissionDb : IdentityRoleClaim<Guid>, IAuditableEntity<Guid>
{
    public new Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Group { get; set; } = "";
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public static class AppPermissionDbExtensions
{
    public static Claim ToClaim(this AppPermissionDb appPermission)
    {
        return new Claim(appPermission.ClaimType, appPermission.ClaimValue);
    }

    public static IEnumerable<Claim> ToClaims(this IEnumerable<AppPermissionDb> appPermissions)
    {
        return appPermissions.Select(x => new Claim(x.ClaimType, x.ClaimValue));
    }

    public static PermissionResponse ToResponse(this AppPermissionDb permission)
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

    public static List<PermissionResponse> ToResponses(this IEnumerable<AppPermissionDb> permissions)
    {
        return permissions.Select(x => x.ToResponse()).ToList();
    }
}