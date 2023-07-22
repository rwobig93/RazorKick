using System.Security.Claims;
using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Models.Identity.User;
using Application.Requests.Identity.User;
using Application.Services.Identity;
using Application.Services.System;
using Application.Settings.AppSettings;
using Blazored.LocalStorage;
using Domain.Enums.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace TestBlazorServerApp.Shared;

public partial class NotAuthorized
{
    [Inject] private IAppAccountService AccountService { get; set; } = null!;
    [Inject] private IRunningServerState ServerState { get; set; } = null!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;
    [Inject] private IAppUserService UserService { get; init; } = null!;
    [Inject] private IOptions<SecurityConfiguration> SecuritySettings { get; init; } = null!;
    [Inject] private IOptions<AppConfiguration> AppSettings { get; init; } = null!;
    [Inject] private IDateTimeService DateTimeService { get; init; } = null!;
    
    
    public ClaimsPrincipal CurrentUser { get; set; } = new();
    private AppUserFull UserFull { get; set; } = new();
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetCurrentUser();
            StateHasChanged();
        }
    }

    private async Task GetCurrentUser()
    {
        try
        {
            CurrentUser = (await CurrentUserService.GetCurrentUserPrincipal())!;
            UserFull = (await CurrentUserService.GetCurrentUserFull())!;

            // If we've made it to the NotAuthorized page
            var tokenRequest = await GetRefreshTokenRequest();
            if (string.IsNullOrWhiteSpace(tokenRequest.Token) || string.IsNullOrWhiteSpace(tokenRequest.RefreshToken))
            {
                // A token is missing so something happened, we'll just start over
                await LogoutAndClearCache();
                return;
            }

            var authRequired = await AccountService.IsUserRequiredToReAuthenticate(UserFull.Id);
            if (authRequired.Data)
            {
                // Re-authentication is required, force back to login page after logging out
                await LogoutAndClearCache();
                return;
            }

            var refreshResponse = await AccountService.ReAuthUsingRefreshTokenAsync(tokenRequest);
            if (!refreshResponse.Succeeded)
            {
                // Using refresh token failed, user must do a fresh login
                await LogoutAndClearCache();
                return;
            }

            // Re-authentication using authorized token & refresh token succeeded, cache new tokens and move on
            await AccountService.CacheAuthTokens(refreshResponse);
        }
        catch (Exception)
        {
            // Failure occurred, start fresh
            await LogoutAndClearCache();
        }
    }

    private async Task LogoutAndClearCache()
    {
        var loginRedirectReason = LoginRedirectReason.SessionExpired;

        try
        {
            // Validate if re-login is forced to give feedback to the user, items are ordered for overwrite precedence
            if (UserFull.Id != Guid.Empty)
            {
                var userSecurity = (await UserService.GetSecurityInfoAsync(UserFull.Id)).Data;
                
                // Force re-login was set on the account
                if (userSecurity!.AuthState == AuthState.LoginRequired)
                    loginRedirectReason = LoginRedirectReason.ReAuthenticationForce;
                // Last full login is older than the configured timeout
                if (userSecurity.LastFullLogin!.Value.AddMinutes(SecuritySettings.Value.ForceLoginIntervalMinutes) <
                    DateTimeService.NowDatabaseTime)
                    loginRedirectReason = LoginRedirectReason.FullLoginTimeout;
            }
        }
        catch
        {
            // Ignore any exceptions since we'll just be logging out anyway
        }

        await AccountService.LogoutGuiAsync(Guid.Empty);
        var loginUriBase = new Uri(string.Concat(AppSettings.Value.BaseUrl, AppRouteConstants.Identity.Login));
        var loginUriFull = QueryHelpers.AddQueryString(
            loginUriBase.ToString(), LoginRedirectConstants.RedirectParameter, loginRedirectReason.ToString());
        
        NavManager.NavigateTo(loginUriFull, true);
    }

    private async Task<RefreshTokenRequest> GetRefreshTokenRequest()
    {
        var tokenRequest = new RefreshTokenRequest();
        
        try
        {
            tokenRequest.ClientId = await LocalStorage.GetItemAsync<string>(LocalStorageConstants.ClientId);
            tokenRequest.Token = await LocalStorage.GetItemAsync<string>(LocalStorageConstants.AuthToken);
            tokenRequest.RefreshToken = await LocalStorage.GetItemAsync<string>(LocalStorageConstants.AuthTokenRefresh);
        }
        catch
        {
            tokenRequest.Token = "";
            tokenRequest.RefreshToken = "";
        }

        return tokenRequest;
    }
}