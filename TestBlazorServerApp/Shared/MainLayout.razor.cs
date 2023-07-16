using System.Security.Claims;
using System.Security.Principal;
using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Mappers.Identity;
using Application.Models.Identity.User;
using Application.Requests.Identity.User;
using Application.Services.Identity;
using Application.Services.System;
using Application.Settings.AppSettings;
using Blazored.LocalStorage;
using Domain.Enums.Identity;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TestBlazorServerApp.Settings;

namespace TestBlazorServerApp.Shared;

public partial class MainLayout
{
    [Inject] private IAppAccountService AccountService { get; set; } = null!;
    [Inject] private IRunningServerState ServerState { get; set; } = null!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;
    [Inject] private IAppUserService UserService { get; init; } = null!;
    [Inject] private IOptions<SecurityConfiguration> SecuritySettings { get; init; } = null!;
    [Inject] private IOptions<AppConfiguration> AppSettings { get; init; } = null!;
    [Inject] private IDateTimeService DateTimeService { get; init; } = null!;
    

    public ClaimsPrincipal CurrentUser { get; set; } = new();
    
    public AppUserPreferenceFull _userPreferences = new();
    public readonly List<AppTheme> _availableThemes = AppThemes.GetAvailableThemes();
    public MudTheme _selectedTheme = AppThemes.DarkTheme.Theme;

    private AppUserFull UserFull { get; set; } = new();
    private bool _settingsDrawerOpen;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetCurrentUser();
            await GetPreferences();
            StateHasChanged();
        }
    }

    private async Task GetCurrentUser()
    {
        // Gather current tokens if they exist for authentication flow
        var tokenRequest = await GetRefreshTokenRequest();
        
        try
        {
            CurrentUser = (await CurrentUserService.GetCurrentUserPrincipal())!;
            UserFull = (await CurrentUserService.GetCurrentUserFull())!;

            // If both tokens are present on the client local storage and there aren't any claims the token has expired
            if (!string.IsNullOrWhiteSpace(tokenRequest.Token) &&
                 !string.IsNullOrWhiteSpace(tokenRequest.RefreshToken) &&
                !CurrentUser.Claims.Any())
                throw new SecurityTokenException("Token expired");
        }
        catch (Exception)
        {
            if (string.IsNullOrWhiteSpace(tokenRequest.Token) || string.IsNullOrWhiteSpace(tokenRequest.RefreshToken))
            {
                // Using refresh token failed, user must do a fresh login
                await LogoutAndClearCache();
                return;
            }

            var response = await AccountService.ReAuthUsingRefreshTokenAsync(tokenRequest);
            if (!response.Succeeded)
            {
                // Using refresh token failed, user must do a fresh login
                await LogoutAndClearCache();
                return;
            }

            // Re-authentication using authorized token & refresh token succeeded, cache new tokens and move on
            await AccountService.CacheAuthTokens(response);
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

    private static bool IsUserAuthenticated(IPrincipal? principal)
    {
        return principal?.Identity is not null && principal.Identity.IsAuthenticated;
    }

    private async Task DrawerToggle()
    {
        _userPreferences.DrawerDefaultOpen = !_userPreferences.DrawerDefaultOpen;

        if (IsUserAuthenticated(CurrentUser))
            await AccountService.UpdatePreferences(CurrentUserService.GetIdFromPrincipal(CurrentUser), _userPreferences.ToUpdate());
    }

    private void SettingsToggle()
    {
        _settingsDrawerOpen = !_settingsDrawerOpen;
    }

    private async Task ChangeTheme(AppTheme theme)
    {
        try
        {
            _userPreferences.ThemePreference = theme.Id;
            _selectedTheme = AppThemes.GetThemeById(theme.Id).Theme;
            
            if (IsUserAuthenticated(CurrentUser))
            {
                var userId = CurrentUserService.GetIdFromPrincipal(CurrentUser);
                var result = await AccountService.UpdatePreferences(userId, _userPreferences.ToUpdate());
                if (!result.Succeeded)
                    result.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            }
        }
        catch
        {
            _selectedTheme = AppThemes.GetThemeById(theme.Id).Theme;
        }
    }

    private async Task GetPreferences()
    {
        if (IsUserAuthenticated(CurrentUser))
        {
            var userId = CurrentUserService.GetIdFromPrincipal(CurrentUser);
            var preferences = await AccountService.GetPreferences(userId);
            if (!preferences.Succeeded)
            {
                preferences.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
                return;
            }

            _userPreferences = preferences.Data;
            UpdateCustomThemes();
            _selectedTheme = AppThemes.GetThemeById(_userPreferences.ThemePreference).Theme;
        }
    }

    private void UpdateCustomThemes()
    {
        foreach (var customThemeId in AppThemes.GetCustomThemeIds())
        {
            var matchingTheme = _availableThemes.FirstOrDefault(x => x.Id == customThemeId);
            var preferenceTheme = AppThemes.GetPreferenceCustomThemeFromId(_userPreferences, customThemeId);
            
            matchingTheme!.FriendlyName = preferenceTheme.ThemeName;
            matchingTheme.Description = preferenceTheme.ThemeDescription;
            matchingTheme.Theme.Palette = new PaletteDark()
            {
                Primary = preferenceTheme.ColorPrimary,
                Secondary = preferenceTheme.ColorSecondary,
                Tertiary = preferenceTheme.ColorTertiary,
                Background = preferenceTheme.ColorBackground,
                Success = preferenceTheme.ColorSuccess,
                Error = preferenceTheme.ColorError,
                BackgroundGrey = preferenceTheme.ColorNavBar,
                TextDisabled = "rgba(255,255,255, 0.26)",
                Surface = preferenceTheme.ColorBackground,
                DrawerBackground = preferenceTheme.ColorNavBar,
                DrawerText = preferenceTheme.ColorPrimary,
                AppbarBackground = preferenceTheme.ColorTitleBar,
                AppbarText = preferenceTheme.ColorPrimary,
                TextPrimary = preferenceTheme.ColorPrimary,
                TextSecondary = preferenceTheme.ColorSecondary,
                ActionDefault = "#adadb1",
                ActionDisabled = "rgba(255,255,255, 0.26)",
                ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                DrawerIcon = preferenceTheme.ColorPrimary
            };
        }
    }
}