using System.Security.Claims;
using Domain.Contracts;
using Microsoft.AspNetCore.Identity;

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
}