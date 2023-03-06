namespace Application.Constants.Communication;

public static class ErrorMessageConstants
{
    public const string GenericError = "An internal server error occurred, please contact the administrator";

    public const string InvalidValueError = "The value provided was invalid, please try again";
    
    public const string CredentialsInvalidError = "The username and password combination provided is invalid";
    public const string AccountDisabledError = "Your account is disabled, please contact the administrator";
    public const string EmailNotConfirmedError = "Your email has not been confirmed, please confirm your email";
    public const string UserNotFoundError = "Was unable to find a user with the provided information";
    public const string PasswordsNoMatchError = "Provided password and confirmation don't match, please try again";
    public const string TokenInvalidError = "The token provided is invalid";

    public const string PermissionError = "You aren't authorized to do that, please go away";
    public const string AdminSelfPowerRemovalError =
        "You can't remove admin access from yourself, another admin will have to revoke your access";
    public const string DefaultAdminPowerRemovalError = "Default admin cannot have admin access revoked";
}