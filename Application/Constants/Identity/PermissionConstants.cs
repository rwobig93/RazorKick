﻿using System.Reflection;

namespace Application.Constants.Identity;

public static class PermissionConstants
{
    public static class Users
    {
        public const string View = "Permissions.Native.Users.View";
        public const string Create = "Permissions.Native.Users.Create";
        public const string Edit = "Permissions.Native.Users.Edit";
        public const string Delete = "Permissions.Native.Users.Delete";
        public const string Export = "Permissions.Native.Users.Export";
    }

    public static class Roles
    {
        public const string View = "Permissions.Native.Roles.View";
        public const string Create = "Permissions.Native.Roles.Create";
        public const string Edit = "Permissions.Native.Roles.Edit";
        public const string Delete = "Permissions.Native.Roles.Delete";
    }

    public static class Claims
    {
        public const string View = "Permissions.Native.Claims.View";
        public const string Create = "Permissions.Native.Claims.Create";
        public const string Edit = "Permissions.Native.Claims.Edit";
        public const string Delete = "Permissions.Native.Claims.Delete";
    }
    
    public static class Preferences
    {
        public const string ChangeTheme = "Permissions.Native.Preferences.ChangeTheme";
    }

    public static class Jobs
    {
        public const string View = "Permissions.Native.Jobs.View";
        public const string JobStatus = "Permissions.Native.Jobs.Status";
    }

    public static class Developer
    {
        public const string DevMenu = "Permissions.Native.Developer.Menu";
    }

    public static class Audit
    {
        public const string View = "Permissions.Native.Audit.View";
        public const string Export = "Permissions.Native.Audit.Export";
        public const string Search = "Permissions.Native.Audit.Search";
    }
    
    /// <summary>
    /// Returns a list of permissions
    /// </summary>
    /// <returns></returns>
    public static List<string> GetRegisteredPermissions()
    {
        var permissions = new List<string>();
        foreach (var prop in typeof(PermissionConstants).GetNestedTypes().SelectMany(c => c.GetFields(
                     BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        {
            var propertyValue = prop.GetValue(null);
            if (propertyValue is not null)
                permissions.Add(propertyValue.ToString()!);
        }
        
        return permissions;
    }
}
