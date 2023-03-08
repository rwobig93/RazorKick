using System.Reflection;
using System.Security.Claims;
using Application.Services.Identity;
using Application.Services.System;
using Microsoft.AspNetCore.Components;
using TestBlazorServerApp.Settings;

namespace TestBlazorServerApp.Shared;

public partial class MainLayout
{
    [Inject] private ISerializerService Serializer { get; set; } = null!;
    [Inject] private IAppAccountService AccountService { get; set; } = null!;
    private static string ApplicationName => Assembly.GetExecutingAssembly().GetName().Name ?? "TestBlazorServerApp";
    private bool _isDrawerOpen = true;
    private MudTheme _selectedTheme = AppThemes.DarkTheme.Theme;

    protected override async Task OnInitializedAsync()
    {
        await SetThemeFromPreference();
    }

    private async Task SetThemeFromPreference()
    {
        try
        {
            var currentUser = await CurrentUserService.GetCurrentUserPrincipal();
            if (IsUserAuthenticated(currentUser))
            {
                var userId = CurrentUserService.GetIdFromPrincipal(currentUser!);
                var themeId = await AccountService.GetThemePreference(userId);
                var desiredTheme = AppThemes.GetThemeById(themeId.Data);
                _selectedTheme = desiredTheme.Theme;
            }
        }
        catch
        {
            _selectedTheme = AppThemes.DarkTheme.Theme;
        }
    }

    private static bool IsUserAuthenticated(ClaimsPrincipal? principal)
    {
        return principal?.Identity is not null && principal.Identity.IsAuthenticated;
    }

    private void DrawerToggle()
    {
        _isDrawerOpen = !_isDrawerOpen;
    }

    private void ChangeTheme(MudTheme theme)
    {
        _selectedTheme = theme;
    }
    
    private static List<AppTheme> GetAvailableThemes()
    {
        var fields = typeof(AppThemes)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        return (from fi in fields select fi.GetValue(null)
            into propertyValue
            where propertyValue is not null select (AppTheme)propertyValue).ToList()!;
    }
}