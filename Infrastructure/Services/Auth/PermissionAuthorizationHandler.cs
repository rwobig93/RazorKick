using System.Security.Claims;
using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Helpers.Auth;
using Application.Helpers.Identity;
using Application.Models.Identity.Permission;
using Application.Services.Identity;
using Application.Settings.AppSettings;
using Domain.Enums.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Auth;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly AppConfiguration _appConfig;
    private readonly IAppAccountService _accountService;
    private readonly NavigationManager _navManager;
    
    public PermissionAuthorizationHandler(IOptions<AppConfiguration> appConfig, IAppAccountService accountService, NavigationManager navManager)
    {
        _accountService = accountService;
        _navManager = navManager;
        _appConfig = appConfig.Value;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // If there are no claims the user isn't authenticated as we always have at least a NameIdentifier in our generated JWT
        if (!context.User.Claims.Any() || context.User == UserConstants.UnauthenticatedPrincipal)
        {
            context.Fail();
            return;
        }

        // User has an expired session, let's try re-authenticating them using the refresh token
        if (context.User == UserConstants.ExpiredPrincipal)
        {
            var reAuthenticationSuccess = await AttemptReAuthentication();
            if (!reAuthenticationSuccess)
            {
                context.Fail();
                await Task.CompletedTask;
                return;
            }
            
            _navManager.NavigateTo(_navManager.Uri, true);
            context.Fail();
            return;
        }
        
        // Validate if user is required to do a full re-authentication
        var userId = context.User.Claims.GetId();
        if ((await _accountService.IsUserRequiredToReAuthenticate(userId)).Data)
        {
            context.Fail();
            await Task.CompletedTask;
            return;
        }
        
        // Validate or re-authenticate active session based on token expiration, this can happen if the token hasn't been validated recently
        if (!(await _accountService.IsCurrentSessionValid()).Data)
        {
            var reAuthenticationSuccess = await AttemptReAuthentication();
            if (!reAuthenticationSuccess)
            {
                context.Fail();
                await Task.CompletedTask;
                return;
            }
            
            _navManager.NavigateTo(_navManager.Uri, true);
            context.Fail();
            return;
        }
        
        // If active session is valid and not expired validate permissions via claims
        var permissions = context.User.Claims.Where(x => x.Type == ApplicationClaimTypes.Permission &&
                                                         x.Value == requirement.Permission &&
                                                         x.Issuer == JwtHelpers.GetJwtIssuer(_appConfig));
        if (permissions.Any())
        {
            context.Succeed(requirement);
            await Task.CompletedTask;
            return;
        }
        
        // Default explicit permission validation fail
        context.Fail();
    }

    private async Task<bool> AttemptReAuthentication()
    {
        var response = await _accountService.ReAuthUsingRefreshTokenAsync();
        if (!response.Succeeded)
        {
            // Using refresh token failed, user must do a fresh login
            await LogoutAndClearCache();
            return false;
        }

        // Re-authentication using authorized token & refresh token succeeded, cache new tokens and move on
        await _accountService.CacheTokensAndAuthAsync(response.Data);
        return true;
    }

    private async Task LogoutAndClearCache()
    {
        await _accountService.LogoutGuiAsync(Guid.Empty);
        var loginUriFull = QueryHelpers.AddQueryString(
            AppRouteConstants.Identity.Login, LoginRedirectConstants.RedirectParameter, nameof(LoginRedirectReason.SessionExpired));
        
        _navManager.NavigateTo(loginUriFull, true);
    }
}
