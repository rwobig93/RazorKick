namespace Domain.Models.Identity;

public class AppThemeCustom
{
    public string ThemeName { get; set; } = "CustomTheme";
    public string ThemeDescription { get; set; } = "This be custom yo!";
    public string ColorPrimary { get; set; } = "#BB86FC";
    public string ColorSecondary { get; set; } = "#03DAC6";
    public string ColorTertiary { get; set; } = "#f747a3";
    public string ColorBackground { get; set; } = "#000000";
    public string ColorTitleBar { get; set; } = "#1f1f1f";
    public string ColorNavBar { get; set; } = "#2f2f2f";
    public string ColorSuccess { get; set; } = "#007E33";
    public string ColorError { get; set; } = "#df0808";
    
    public static AppThemeCustom GetExampleCustomOne()
    {
        return new AppThemeCustom
        {
            ThemeName = "Example Custom 1",
            ThemeDescription = "I'm a bit uhm... custom ;)",
            ColorPrimary = "#C9E302",
            ColorSecondary = "#EB7003",
            ColorTertiary = "#BCB9B6",
            ColorBackground = "#000000",
            ColorTitleBar = "#1f1f1f",
            ColorNavBar = "#2f2f2f",
            ColorSuccess = "#02B802",
            ColorError = "#A30000"
        };
    }
    
    public static AppThemeCustom GetExampleCustomTwo()
    {
        return new AppThemeCustom
        {
            ThemeName = "Example Custom 2",
            ThemeDescription = "Pink Cyberpunk Homegirl!",
            ColorPrimary = "#EA00CE",
            ColorSecondary = "#AD00EA",
            ColorTertiary = "#CEEA00",
            ColorBackground = "#000000",
            ColorTitleBar = "#3E2566",
            ColorNavBar = "#5F2566",
            ColorSuccess = "#59EA00",
            ColorError = "#EA1C00"
        };
    }
    
    public static AppThemeCustom GetExampleCustomThree()
    {
        return new AppThemeCustom
        {
            ThemeName = "Example Custom 3",
            ThemeDescription = "I work part time as a Lasik machine",
            ColorPrimary = "#00FFD8",
            ColorSecondary = "#00FF00",
            ColorTertiary = "#FFFE00",
            ColorTitleBar = "#00DD74",
            ColorNavBar = "#0069DD",
            ColorBackground = "#00FF74",
            ColorSuccess = "#0500FF",
            ColorError = "#8400FF"
        };
    }
}