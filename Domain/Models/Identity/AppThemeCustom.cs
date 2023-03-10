namespace Domain.Models.Identity;

public class AppThemeCustom
{
    public string ThemeName { get; set; } = "CustomTheme";
    public string ThemeDescription { get; set; } = "This be custom yo!";
    public string ColorPrimary { get; set; } = "#BB86FC";
    public string ColorSecondary { get; set; } = "#03DAC6";
    public string ColorTertiary { get; set; } = "#f747a3";
    public string ColorBackground { get; set; } = "#32333d";
    public string ColorSuccess { get; set; } = "#007E33";
    public string ColorError { get; set; } = "#df0808";
}