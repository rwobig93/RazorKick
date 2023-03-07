using System.Reflection;
using Application.Services.System;
using Microsoft.AspNetCore.Components;
using TestBlazorServerApp.Settings;

namespace TestBlazorServerApp.Shared;

public partial class MainLayout
{
    [Inject] private ISerializerService Serializer { get; set; } = null!;
    private static string ApplicationName => Assembly.GetExecutingAssembly().GetName().Name ?? "TestBlazorServerApp";
    private bool _isDrawerOpen = true;
    private MudTheme _selectedTheme = AppThemes.DarkTheme.Theme;

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