using System.Security.Claims;
using System.Security.Principal;
using Application.Constants.Web;
using Application.Models.Identity;
using Application.Services.Identity;
using Application.Services.System;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Components;
using TestBlazorServerApp.Settings;

namespace TestBlazorServerApp.Shared;

public partial class MainLayout
{
    [Inject] private IAppAccountService AccountService { get; set; } = null!;
    [Inject] private IRunningServerState ServerState { get; set; } = null!;
    public AppUserPreferenceFull _userPreferences = new();
    public ClaimsPrincipal CurrentUser { get; set; } = new();
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
        try
        {
            CurrentUser = await CurrentUserService.GetCurrentUserPrincipal() ?? new ClaimsPrincipal();
            UserFull = await CurrentUserService.GetCurrentUserFull() ?? new AppUserFull();
        }
        catch
        {
            // User has old saved token so we'll force a local storage clear and deauthenticate then redirect
            await AccountService.LogoutGuiAsync(Guid.Empty);
            NavManager.NavigateTo(AppRouteConstants.Identity.Login, true);
        }
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
            matchingTheme.Theme.Palette = new Palette()
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