namespace Application.Models.Lifecycle;

public class ServerStateRecordCreate
{
    public string AppVersion { get; set; } = null!;
    public DateTime Timestamp { get; set; }
}