namespace Shared.Routes;

public static class ApiRoutes
{
    public static class Identity
    {
        public const string GetAll = "/api/identity/users";
        public const string GetById = "/api/identity/user/id";
        public const string GetFullById = "/api/identity/user/full";
        public const string GetByEmail = "/api/identity/user/email";
        public const string GetByUsername = "/api/identity/user/username";
        public const string Delete = "/api/identity/user";
        public const string Create = "/api/identity/user";
        public const string Register = "/api/identity/user/register";
        public const string Login = "/api/identity/user/login";
        public const string Update = "/api/identity/user";
    
        public const string ConfirmEmail = "/api/identity/confirm-email";
        public const string ResetPassword = "/api/identity/reset-password";
    }
}

public static class ApiRouteExtensions
{
    public static string ToFullUrl(this string uri, string hostOrigin) =>
        string.Concat(hostOrigin, uri);
}