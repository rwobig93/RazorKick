namespace Shared.Responses.Monitoring;

public class HealthCheckResponse
{
    public string SettingsValue { get; set; } = "";
    public string Health { get; set; } = "Unhealthy";
    public string ApiVersion { get; set; } = "v1";
}