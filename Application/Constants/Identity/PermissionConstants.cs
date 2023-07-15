using System.Reflection;

namespace Application.Constants.Identity;

public static class PermissionConstants
{
    // NOTE: Use IAuthorizationPolicyProvider to add policies during runtime, each permission must have a policy
    //  The below permissions are registered at runtime but any dynamic permissions will require policies to be created dynamically
    public static class Users
    {
        public const string View = "Permissions.Identity.Users.View";
        public const string Create = "Permissions.Identity.Users.Create";
        public const string Edit = "Permissions.Identity.Users.Edit";
        public const string Delete = "Permissions.Identity.Users.Delete";
        public const string Export = "Permissions.Identity.Users.Export";
        public const string Enable = "Permissions.Identity.Users.Enable";
        public const string Disable = "Permissions.Identity.Users.Disable";
        public const string ResetPassword = "Permissions.Identity.Users.ResetPassword";
        public const string ViewExtAttrs = "Permissions.Identity.Users.ExtendedAttrView";
    }

    public static class Roles
    {
        public const string View = "Permissions.Identity.Roles.View";
        public const string Create = "Permissions.Identity.Roles.Create";
        public const string Edit = "Permissions.Identity.Roles.Edit";
        public const string Delete = "Permissions.Identity.Roles.Delete";
        public const string Add = "Permissions.Identity.Roles.Add";
        public const string Remove = "Permissions.Identity.Roles.Remove";
    }

    public static class Permissions
    {
        public const string View = "Permissions.Identity.Permissions.View";
        public const string Add = "Permissions.Identity.Permissions.Add";
        public const string Remove = "Permissions.Identity.Permissions.Remove";
    }
    
    public static class Preferences
    {
        public const string ChangeTheme = "Permissions.Identity.Preferences.ChangeTheme";
    }

    public static class Jobs
    {
        public const string View = "Permissions.System.Jobs.View";
        public const string Status = "Permissions.System.Jobs.Status";
    }

    public static class Api
    {
        public const string View = "Permissions.System.Api.View";
    }

    public static class Example
    {
        public const string Counter = "Permissions.Native.Example.Counter";
        public const string Weather = "Permissions.Native.Example.Weather";
    }

    public static class Audit
    {
        public const string View = "Permissions.System.Audit.View";
        public const string Export = "Permissions.System.Audit.Export";
        public const string Search = "Permissions.System.Audit.Search";
        public const string DeleteOld = "Permissions.System.Audit.DeleteOld";
    }
    
    /// <summary>
    /// Returns a list of all native permissions values
    /// </summary>
    /// <returns></returns>
    public static List<string> GetAllPermissions()
    {
        return (from prop in typeof(PermissionConstants).GetNestedTypes().SelectMany(
            c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)) 
            select prop.GetValue(null) into propertyValue where propertyValue is not null select propertyValue.ToString()!).ToList();
    }

    public static List<string> GetModeratorRolePermissions()
    {
        return new List<string>()
        {
            Jobs.View,
            Permissions.View,
            Permissions.Add,
            Permissions.Remove,
            Roles.View,
            Roles.Edit,
            Roles.Create,
            Roles.Delete,
            Users.View,
            Users.Edit,
            Users.Create,
            Users.Delete,
            Users.Disable,
            Users.Enable,
            Users.ResetPassword,
            Audit.View,
            Audit.Search,
            Audit.Export
        };
    }

    public static List<string> GetDefaultRolePermissions()
    {
        return new List<string>()
        {
            Preferences.ChangeTheme
        };
    }
}
