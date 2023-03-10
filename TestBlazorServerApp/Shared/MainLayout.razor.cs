using System.Reflection;
using System.Security.Claims;
using Application.Models.Identity;
using Application.Services.Identity;
using Domain.Enums.Identity;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Components;
using TestBlazorServerApp.Settings;

namespace TestBlazorServerApp.Shared;

public partial class MainLayout
{
    [Inject] private IAppAccountService AccountService { get; set; } = null!;
    private static string ApplicationName => Assembly.GetExecutingAssembly().GetName().Name ?? "TestBlazorServerApp";
    private AppUserPreferenceFull _userPreferences = new();
    private ClaimsPrincipal _currentUser = new();
    private List<AppTheme> _availableThemes = AppThemes.GetAvailableThemes();
    private MudTheme _selectedTheme = AppThemes.DarkTheme.Theme;

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
        _currentUser = await CurrentUserService.GetCurrentUserPrincipal() ?? new ClaimsPrincipal();
    }

    private static bool IsUserAuthenticated(ClaimsPrincipal? principal)
    {
        return principal?.Identity is not null && principal.Identity.IsAuthenticated;
    }

    private async Task DrawerToggle()
    {
        _userPreferences.DrawerDefaultOpen = !_userPreferences.DrawerDefaultOpen;

        if (IsUserAuthenticated(_currentUser))
            await AccountService.UpdatePreferences(CurrentUserService.GetIdFromPrincipal(_currentUser), _userPreferences.ToUpdate());
    }

    private async Task ChangeTheme(AppTheme theme)
    {
        try
        {
            _userPreferences.ThemePreference = theme.Id;
            _selectedTheme = AppThemes.GetThemeById(theme.Id).Theme;
            
            if (IsUserAuthenticated(_currentUser))
            {
                var userId = CurrentUserService.GetIdFromPrincipal(_currentUser);
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
        // TODO: Theme isn't updating for unauthorized pages - user not having permission
        if (IsUserAuthenticated(_currentUser))
        {
            var userId = CurrentUserService.GetIdFromPrincipal(_currentUser);
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
        if (_userPreferences.CustomThemeOne is not null)
        {
            var firstTheme = _availableThemes.FirstOrDefault(x => x.Id == AppThemeId.CustomOne);
            firstTheme!.FriendlyName = _userPreferences.CustomThemeOne.ThemeName;
            firstTheme.Description = _userPreferences.CustomThemeOne.ThemeDescription;
            firstTheme.Theme.Palette = new Palette()
            {
                Primary = _userPreferences.CustomThemeOne.ColorPrimary,
                Secondary = _userPreferences.CustomThemeOne.ColorSecondary,
                Tertiary = _userPreferences.CustomThemeOne.ColorTertiary,
                Background = _userPreferences.CustomThemeOne.ColorBackground,
                Success = _userPreferences.CustomThemeOne.ColorSuccess,
                Error = _userPreferences.CustomThemeOne.ColorError
            };
        }
        
        if (_userPreferences.CustomThemeTwo is not null)
        {
            var secondTheme = _availableThemes.FirstOrDefault(x => x.Id == AppThemeId.CustomTwo);
            secondTheme!.FriendlyName = _userPreferences.CustomThemeTwo.ThemeName;
            secondTheme.Description = _userPreferences.CustomThemeTwo.ThemeDescription;
            secondTheme.Theme.Palette = new Palette()
            {
                Primary = _userPreferences.CustomThemeTwo.ColorPrimary,
                Secondary = _userPreferences.CustomThemeTwo.ColorSecondary,
                Tertiary = _userPreferences.CustomThemeTwo.ColorTertiary,
                Background = _userPreferences.CustomThemeTwo.ColorBackground,
                Success = _userPreferences.CustomThemeTwo.ColorSuccess,
                Error = _userPreferences.CustomThemeTwo.ColorError
            };
        }
        
        if (_userPreferences.CustomThemeThree is not null)
        {
            var thirdtheme = _availableThemes.FirstOrDefault(x => x.Id == AppThemeId.CustomThree);
            thirdtheme!.FriendlyName = _userPreferences.CustomThemeThree.ThemeName;
            thirdtheme.Description = _userPreferences.CustomThemeThree.ThemeDescription;
            thirdtheme.Theme.Palette = new Palette()
            {
                Primary = _userPreferences.CustomThemeThree.ColorPrimary,
                Secondary = _userPreferences.CustomThemeThree.ColorSecondary,
                Tertiary = _userPreferences.CustomThemeThree.ColorTertiary,
                Background = _userPreferences.CustomThemeThree.ColorBackground,
                Success = _userPreferences.CustomThemeThree.ColorSuccess,
                Error = _userPreferences.CustomThemeThree.ColorError
            };
        }
    }
}