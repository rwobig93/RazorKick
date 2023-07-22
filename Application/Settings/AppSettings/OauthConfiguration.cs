namespace Application.Settings.AppSettings;

public class OauthConfiguration : IAppSettingsSection
{
    public const string SectionName = "Oauth";

    public string GoogleClientId { get; set; } = "";
    public string GoogleClientSecret { get; set; } = "";

    public string DiscordClientId { get; set; } = "";
    public string DiscordClientSecret { get; set; } = "";

    public string SpotifyClientId { get; set; } = "";
    public string SpotifyClientSecret { get; set; } = "";

    public string FacebookClientId { get; set; } = "";
    public string FacebookClientSecret { get; set; } = "";
}