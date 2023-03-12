using System.Security.Claims;
using Application.Constants.Web;
using Application.Models.Identity;
using Application.Services.Identity;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Components;
using TestBlazorServerApp.Settings;

namespace TestBlazorServerApp.Shared;

public partial class SettingsMenu
{
    [Inject] private IAppAccountService AccountService { get; set; } = null!;
    private AppUserPreferenceFull _userPreferences = new();
    private ClaimsPrincipal CurrentUser { get; set; } = new();
    private List<AppTheme> _availableThemes = AppThemes.GetAvailableThemes();
    private MudTheme _selectedTheme = AppThemes.DarkTheme.Theme;
    private AppTheme _displayTheme = AppThemes.DarkTheme;
    
    

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            UpdateDisplayTheme();
            StateHasChanged();
            await Task.CompletedTask;
        }
    }
    
    private static bool IsUserAuthenticated(ClaimsPrincipal? principal)
    {
        return principal?.Identity is not null && principal.Identity.IsAuthenticated;
    }

    private void UpdateDisplayTheme()
    {
        _displayTheme = _availableThemes.FirstOrDefault(x => x.Id == _userPreferences.ThemePreference)!;
    }

    private async Task ChangeTheme(AppTheme theme)
    {
        try
        {
            _userPreferences.ThemePreference = theme.Id;
            _selectedTheme = AppThemes.GetThemeById(theme.Id).Theme;
            UpdateDisplayTheme();
            
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

    private async Task LogoutUser()
    {
        await AccountService.LogoutGuiAsync();
        NavManager.NavigateTo(AppRouteConstants.Index, true);
    }
}