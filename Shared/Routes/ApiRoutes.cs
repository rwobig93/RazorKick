namespace Shared.Routes;

public static class ApiRoutes
{
    public static class Identity
    {
        public static class User
        {
            public const string GetAll = "/api/identity/users";
            public const string GetById = "/api/identity/user/id";
            public const string GetFullById = "/api/identity/user/id/full";
            public const string GetByEmail = "/api/identity/user/email";
            public const string GetFullByEmail = "/api/identity/user/email/full";
            public const string GetByUsername = "/api/identity/user/username";
            public const string GetFullByUsername = "/api/identity/user/username/full";
            public const string Delete = "/api/identity/user";
            public const string Create = "/api/identity/user";
            public const string Register = "/api/identity/user/register";
            public const string Login = "/api/identity/user/login";
            public const string Update = "/api/identity/user";
    
            // TODO: Implement API Endpoints for these
            public const string ConfirmEmail = "/api/identity/confirm-email";
            public const string ResetPassword = "/api/identity/reset-password";
        }

        public static class Role
        {
            public const string GetAll = "/api/identity/roles";
            public const string GetById = "/api/identity/role";
            public const string Delete = "/api/identity/role";
            public const string Create = "/api/identity/role";
            public const string Update = "/api/identity/role";
        
            public const string GetRolesForUser = "/api/identity/roles/user";
            public const string IsUserInRole = "/api/identity/role/user/has";
            public const string AddUserToRole = "/api/identity/role/user/add";
            public const string RemoveUserFromRole = "/api/identity/role/user/remove";
        }

        public static class Permission
        {
            public const string GetAll = "/api/identity/permissions";
            public const string GetById = "/api/identity/permission";
            public const string Delete = "/api/identity/permission";
            public const string Update = "/api/identity/permission";
        
            public const string GetDirectPermissionsForUser = "/api/identity/permissions/user/direct";
            public const string GetAllPermissionsForUser = "/api/identity/permissions/user/all";
            public const string AddPermissionToUser = "/api/identity/permission/user/add";
            public const string RemovePermissionFromUser = "/api/identity/permission/user/remove";
            public const string DoesUserHavePermission = "/api/identity/permission/user/has";
        
            public const string GetAllPermissionsForRole = "/api/identity/permission/role";
            public const string AddPermissionToRole = "/api/identity/permission/role/add";
            public const string RemovePermissionFromRole = "/api/identity/permission/role/remove";
            public const string DoesRoleHavePermission = "/api/identity/permission/role/has";
        }
    }
}

public static class ApiRouteExtensions
{
    public static string ToFullUrl(this string uri, string hostOrigin)
    {
        if (hostOrigin.EndsWith('/'))
            hostOrigin = hostOrigin[..^1];
        return string.Concat(hostOrigin, uri);
    }
}