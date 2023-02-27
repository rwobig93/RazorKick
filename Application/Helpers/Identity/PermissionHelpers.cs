namespace Application.Helpers.Identity;

public static class PermissionHelpers
{
    public static string? GetClaimValueFromPermission(string? permissionGroup, string? permissionName, string? permissionAccess)
    {
        if (permissionGroup is null || permissionName is null || permissionAccess is null)
            return null;
        
        return $"Permissions.{permissionGroup}.{permissionName}.{permissionAccess}";
    }
}