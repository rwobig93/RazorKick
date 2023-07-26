namespace Domain.DatabaseEntities.Lifecycle;

public class ServerStateRecordDb
{
    public Guid Id { get; set; }
    public string AppVersion { get; set; } = null!;
    public DateTime Timestamp { get; set; }
}