using System.Reflection;

namespace Application.Extensibility.Settings;

public static class Permissions
{
    public static class Users
    {
        public const string View = "Permissions.Users.View";
        public const string Create = "Permissions.Users.Create";
        public const string Edit = "Permissions.Users.Edit";
        public const string Delete = "Permissions.Users.Delete";
        public const string Export = "Permissions.Users.Export";
        public const string Search = "Permissions.Users.Search";
    }

    public static class Roles
    {
        public const string View = "Permissions.Roles.View";
        public const string Create = "Permissions.Roles.Create";
        public const string Edit = "Permissions.Roles.Edit";
        public const string Delete = "Permissions.Roles.Delete";
        public const string Search = "Permissions.Roles.Search";
    }

    public static class RoleClaims
    {
        public const string View = "Permissions.RoleClaims.View";
        public const string Create = "Permissions.RoleClaims.Create";
        public const string Edit = "Permissions.RoleClaims.Edit";
        public const string Delete = "Permissions.RoleClaims.Delete";
        public const string Search = "Permissions.RoleClaims.Search";
    }
    
    public static class Preferences
    {
        public const string ChangeTheme = "Permissions.Preferences.ChangeTheme";
    }

    public static class Jobs
    {
        public const string View = "Permissions.Jobs.View";
        public const string JobStatus = "Permissions.Jobs.Status";
    }

    public static class Developer
    {
        public const string DevMenu = "Permissions.Developer.Menu";
    }

    public static class AuditTrails
    {
        public const string View = "Permissions.AuditTrails.View";
        public const string Export = "Permissions.AuditTrails.Export";
        public const string Search = "Permissions.AuditTrails.Search";
    }
    
    /// <summary>
    /// Returns a list of Permissions.
    /// </summary>
    /// <returns></returns>
    public static List<string> GetRegisteredPermissions()
    {
        var permissions = new List<string>();
        foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(
                     BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        {
            var propertyValue = prop.GetValue(null);
            if (propertyValue is not null)
                permissions.Add(propertyValue.ToString()!);
        }
        
        return permissions;
    }
}
