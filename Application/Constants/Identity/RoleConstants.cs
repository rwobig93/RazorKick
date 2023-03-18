namespace Application.Constants.Identity;

public static class RoleConstants
{
    public static class DefaultRoles
    {
        public const string AdminName = "Admin";
        public const string AdminDescription = "Global administrator role with all permissions";
    
        public const string ModeratorName = "Moderator";
        public const string ModeratorDescription = "Moderator role with most administration permissions";
    
        public const string DefaultName = "Default";
        public const string DefaultDescription = "Default role with base permissions, granted to every account by default";
    }

    public static List<string> GetAdminRoleNames()
    {
        return new List<string>()
        {
            DefaultRoles.AdminName
        };
    }

    public static List<string> GetModeratorRoleNames()
    {
        return new List<string>()
        {
            DefaultRoles.ModeratorName,
            DefaultRoles.DefaultName
        };
    }

    public static List<string> GetDefaultRoleNames()
    {
        return new List<string>()
        {
            DefaultRoles.DefaultName
        };
    }
}