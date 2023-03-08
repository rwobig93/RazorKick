using Domain.Enums.Identity;

namespace TestBlazorServerApp.Settings;

public static class AppThemes
{
    public static AppTheme GetThemeById(AppThemeId themeId)
    {
        return themeId switch
        {
            AppThemeId.Dark => DarkTheme,
            AppThemeId.Darker => DarkerTheme,
            AppThemeId.Hacker => HackerTheme,
            AppThemeId.Bright => BrightTheme,
            _ => DarkTheme
        };
    }
    
    public static readonly AppTheme DarkTheme = new()
    {
        Id = AppThemeId.Dark,
        FriendlyName = "Dark",
        Description = "Easy on the eyes, and awesome, don't forget awesome",
        Theme = new MudTheme()
        {
            Palette = new Palette()
            {
                Primary = "#BB86FC",
                Secondary = "#03DAC6",
                Success = "#007E33",
                Black = "#27272f",
                Background = "#32333d",
                BackgroundGrey = "#27272f",
                TextDisabled = "rgba(255,255,255, 0.26)",
                Surface = "#202020",
                DrawerBackground = "#27272f",
                DrawerText = "rgba(255,255,255, 0.50)",
                AppbarBackground = "#373740",
                AppbarText = "rgba(255,255,255, 0.70)",
                TextPrimary = "rgba(255,255,255, 0.70)",
                TextSecondary = "rgba(255,255,255, 0.50)",
                ActionDefault = "#adadb1",
                ActionDisabled = "rgba(255,255,255, 0.26)",
                ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                DrawerIcon = "rgba(255,255,255, 0.50)"
            },
            Typography = AppTypographies.DefaultTypography,
            LayoutProperties = AppLayouts.DefaultLayoutProperties
        }
    };
    
    public static readonly AppTheme DarkerTheme = new()
    {
        Id = AppThemeId.Darker,
        FriendlyName = "Darker",
        Description = "I get it, you wanna go darker, I don't blame you",
        Theme = new MudTheme()
        {
            Palette = new Palette()
            {
                Primary = "#BB86FC",
                Secondary = "#03DAC6",
                Success = "#007E33",
                Black = "#202324",
                Background = "#2A2D2F",
                BackgroundGrey = "#202324",
                TextDisabled = "rgba(255,255,255, 0.26)",
                Surface = "#202020",
                DrawerBackground = "#181A1B",
                DrawerText = "rgba(255,255,255, 0.50)",
                AppbarBackground = "#181A1B",
                AppbarText = "rgba(255,255,255, 0.70)",
                TextPrimary = "rgba(255,255,255, 0.70)",
                TextSecondary = "rgba(255,255,255, 0.50)",
                ActionDefault = "#adadb1",
                ActionDisabled = "rgba(255,255,255, 0.26)",
                ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                DrawerIcon = "rgba(255,255,255, 0.50)"
            },
            Typography = AppTypographies.DefaultTypography,
            LayoutProperties = AppLayouts.DefaultLayoutProperties
        }
    };
    
    public static readonly AppTheme HackerTheme = new()
    {
        Id = AppThemeId.Hacker,
        FriendlyName = "Hackerman",
        Description = "I'm not normal and I never will be, no matter what world I'm in",
        Theme = new MudTheme()
        {
            Palette = new Palette()
            {
                Primary = "#00bf00",
                Secondary = "#800080",
                Success = "#800080",
                Black = "#191919",
                Background = "#000000",
                BackgroundGrey = "#000000",
                TextDisabled = "rgba(255,255,255, 0.26)",
                Surface = "#000000",
                DrawerBackground = "#000000",
                DrawerText = "#00bf00",
                AppbarBackground = "#000000",
                AppbarText = "#00bf00",
                TextPrimary = "#00bf00",
                TextSecondary = "#800080",
                ActionDefault = "#800080",
                ActionDisabled = "rgba(255,255,255, 0.26)",
                ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                DrawerIcon = "#191919"
            },
            Typography = AppTypographies.DefaultTypography,
            LayoutProperties = AppLayouts.DefaultLayoutProperties
        }
    };
    
    public static readonly AppTheme BrightTheme = new()
    {
        Id = AppThemeId.Bright,
        FriendlyName = "Bright",
        Description = "I mean if you're into that sort of thing, we still gave you that option",
        Theme = new MudTheme()
        {
            Palette = new Palette()
            {
                Primary = "#1E88E5",
                AppbarBackground = "#1E88E5",
                Background = Colors.Grey.Lighten5,
                DrawerBackground = "#FFF",
                DrawerText = "rgba(0,0,0, 0.7)",
                Success = "#007E33"
            },
            Typography = AppTypographies.DefaultTypography,
            LayoutProperties = AppLayouts.DefaultLayoutProperties
        }
    };
}