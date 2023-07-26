using System.Security.Claims;
using Domain.DatabaseEntities.Identity;

namespace Application.Helpers.Identity;

public static class PermissionHelpers
{
    public static string? GetClaimValueFromPermission(string? permissionGroup, string? permissionName, string? permissionAccess)
    {
        if (permissionGroup is null || permissionName is null || permissionAccess is null)
            return null;
        
        return $"Permissions.{permissionGroup}.{permissionName}.{permissionAccess}";
    }

    public static string GetGroupFromValue(string permissionValue) => permissionValue.Split('.')[1];
    public static string GetNameFromValue(string permissionValue) => permissionValue.Split('.')[2];
    public static string GetAccessFromValue(string permissionValue) => permissionValue.Split('.')[3];

    public static Claim ToClaim(this AppPermissionDb appPermission)
    {
        return new Claim(appPermission.ClaimType!, appPermission.ClaimValue!);
    }

    public static IEnumerable<Claim> ToClaims(this IEnumerable<AppPermissionDb> appPermissions)
    {
        return appPermissions.Select(x => new Claim(x.ClaimType!, x.ClaimValue!));
    }

    public static IEnumerable<Claim> ToClaims(this IEnumerable<AppRoleDb> appRoles)
    {
        return appRoles.Select(x => new Claim(ClaimTypes.Role, x.Name));
    }
}