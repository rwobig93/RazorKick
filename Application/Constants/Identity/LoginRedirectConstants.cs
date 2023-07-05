﻿namespace Application.Constants.Identity;

public static class LoginRedirectConstants
{
    public const string RedirectParameter = "redirectReason";
    
    public const string SessionExpired = "Your session has expired and you need to re-login";
    public const string ReAuthenticationForce = "Your session was forcefully logged out by an Administrator and you are required to re-login";
    public const string Unknown = "An error occurred with your session, please re-login";
}