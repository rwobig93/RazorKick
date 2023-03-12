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
    [Parameter] public AppUserPreferenceFull UserPreferences { get; set; } = new();
    [Parameter] public ClaimsPrincipal CurrentUser { get; set; } = new();
    [Parameter] public List<AppTheme> AvailableThemes { get; set; } = AppThemes.GetAvailableThemes();
    [Parameter] public MudTheme SelectedTheme { get; set; } = AppThemes.DarkTheme.Theme;
    [Parameter] public EventCallback<AppTheme> ThemeChanged { get; set; }

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
        _displayTheme = AvailableThemes.FirstOrDefault(x => x.Id == UserPreferences.ThemePreference)!;
    }

    private string GetDisplayUsername()
    {
        if (CurrentUser.Identity?.Name is null)
            return "";
        if (CurrentUser.Identity.Name.Length <= 18)
            return CurrentUser.Identity.Name;

        return CurrentUser.Identity.Name[..18];
    }

    private async Task ChangeThemeOnLayout(AppTheme theme)
    {
        await ThemeChanged.InvokeAsync(theme);
        // TODO: Display theme isn't updating on change
        UpdateDisplayTheme();
    }

    private async Task LogoutUser()
    {
        await AccountService.LogoutGuiAsync();
        NavManager.NavigateTo(AppRouteConstants.Index, true);
    }
}