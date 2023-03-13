namespace Application.Constants.Web;

public static class AppRouteConstants
{
    public const string Index = "/";
    
    public static class Example
    {
        public const string Counter = "/example/counter";
        public const string WeatherData = "/example/weather-data";
        public const string Books = "/example/books";
    }
    
    public static class Identity
    {
        public const string Login = "/identity/login";
        public const string Register = "/identity/register";
        public const string ConfirmEmail = "/identity/confirm-email";
        public const string ForgotPassword = "/identity/reset-password";
    }

    public static class Account
    {
        public const string AccountSettings = "/account/settings";
        public const string Themes = "/account/themes";
        public const string Security = "/account/security";
    }

    public static class Admin
    {
        public const string UserAdmin = "/admin/users";
        public const string RoleAdmin = "/admin/roles";
        public const string PermissionAdmin = "/admin/permissions";
    }

    public static class Api
    {
        public const string Root = "/api";
    }

    public static class Jobs
    {
        public const string Root = "/jobs";
    }
}