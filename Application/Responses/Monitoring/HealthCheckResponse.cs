namespace Application.Responses.Monitoring;

public class HealthCheckResponse
{
    public string Message { get; set; } = "";
    public string Health { get; set; } = "Unhealthy";
    public string ApiVersion { get; set; } = "v1";
}