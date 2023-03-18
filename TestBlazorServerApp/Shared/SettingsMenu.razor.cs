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
    [Parameter] public AppUserFull UserFull { get; set; } = new();
    [Parameter] public List<AppTheme> AvailableThemes { get; set; } = AppThemes.GetAvailableThemes();
    [Parameter] public MudTheme SelectedTheme { get; set; } = AppThemes.DarkTheme.Theme;
    [Parameter] public EventCallback<AppTheme> ThemeChanged { get; set; }

    private bool _testToggleOne;
    private bool _testToggleTwo;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            StateHasChanged();
            await Task.CompletedTask;
        }
    }
    
    private static bool IsUserAuthenticated(ClaimsPrincipal? principal)
    {
        return principal?.Identity is not null && principal.Identity.IsAuthenticated;
    }

    private string GetCurrentThemeName()
    {
        var currentAppTheme = AvailableThemes.FirstOrDefault(x => x.Id == UserPreferences.ThemePreference)!;
        if (currentAppTheme.FriendlyName.Length <= 12)
            return currentAppTheme.FriendlyName;
        
        return currentAppTheme.FriendlyName[..12];
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
        StateHasChanged();
    }

    private async Task LogoutUser()
    {
        await AccountService.LogoutGuiAsync();
        NavManager.NavigateTo(AppRouteConstants.Index, true);
    }
}