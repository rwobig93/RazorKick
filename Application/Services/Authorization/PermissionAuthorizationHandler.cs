using System.Security.Claims;
using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Helpers.Auth;
using Application.Models.Identity.Permission;
using Application.Requests.Identity.User;
using Application.Services.Identity;
using Application.Settings.AppSettings;
using Blazored.LocalStorage;
using Domain.Enums.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Application.Services.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly AppConfiguration _appConfig;
    private readonly IAppAccountService _accountService;
    private readonly NavigationManager _navManager;
    private readonly ILocalStorageService _localStorage;
    
    public PermissionAuthorizationHandler(IOptions<AppConfiguration> appConfig, IAppAccountService accountService, NavigationManager navManager,
        ILocalStorageService localStorage)
    {
        _accountService = accountService;
        _navManager = navManager;
        _localStorage = localStorage;
        _appConfig = appConfig.Value;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // If there are no claims the user isn't authenticated as we always have at least a NameIdentifier in our generated JWT
        if (!context.User.Claims.Any())
        {
            // throw new AuthenticationException("You are currently not authenticated, please authenticate before trying again");
            context.Fail();
            return;
        }
        
        // Validate if user is required to do a full re-authentication
        var userId = context.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).First().Value;
        var userIdParsed = Guid.Parse(userId);
        if (await _accountService.IsUserRequiredToReAuthenticate(userIdParsed))
        {
            context.Fail();
            await Task.CompletedTask;
            return;
        }
        
        // Validate or re-authenticate active session based on current state
        if (!await _accountService.IsCurrentSessionValid())
        {
            var reAuthenticationSuccess = await AttemptReAuthentication();
            if (!reAuthenticationSuccess)
            {
                context.Fail();
                await Task.CompletedTask;
                return;
            }
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
        // Gather current tokens if they exist to attempt a re-authentication
        var tokenRequest = await GetRefreshTokenRequest();
        if (string.IsNullOrWhiteSpace(tokenRequest.Token) || string.IsNullOrWhiteSpace(tokenRequest.RefreshToken))
            return false;

        var response = await _accountService.ReAuthUsingRefreshTokenAsync(tokenRequest);
        if (!response.Succeeded)
        {
            // Using refresh token failed, user must do a fresh login
            await LogoutAndClearCache();
            return false;
        }

        // Re-authentication using authorized token & refresh token succeeded, cache new tokens and move on
        await _accountService.CacheAuthTokens(response);
        return true;
    }

    private async Task LogoutAndClearCache()
    {
        await _accountService.LogoutGuiAsync(Guid.Empty);
        var loginUriFull = QueryHelpers.AddQueryString(
            AppRouteConstants.Identity.Login, LoginRedirectConstants.RedirectParameter, nameof(LoginRedirectReason.SessionExpired));
        
        _navManager.NavigateTo(loginUriFull, true);
    }

    private async Task<RefreshTokenRequest> GetRefreshTokenRequest()
    {
        var tokenRequest = new RefreshTokenRequest();
        
        try
        {
            tokenRequest.Token = await _localStorage.GetItemAsync<string>(LocalStorageConstants.AuthToken);
            tokenRequest.RefreshToken = await _localStorage.GetItemAsync<string>(LocalStorageConstants.AuthTokenRefresh);
        }
        catch
        {
            tokenRequest.Token = "";
            tokenRequest.RefreshToken = "";
        }

        return tokenRequest;
    }
}
