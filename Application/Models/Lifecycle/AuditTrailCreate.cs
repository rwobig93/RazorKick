using Domain.DatabaseEntities.Lifecycle;
using Domain.Enums.Database;

namespace Application.Models.Lifecycle;

public class AuditTrailCreate
{
    public string TableName { get; set; } = null!;
    public Guid RecordId { get; set; }
    public Guid ChangedBy { get; set; } = Guid.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public DatabaseActionType Action { get; set; }
    public string Before { get; set; } = "";
    public string After { get; set; } = "";
}

public static class AuditTrailCreateExtensions
{
    public static AuditTrailCreate ToCreate(this AuditTrailDb auditTrailDb)
    {
        return new AuditTrailCreate
        {
            TableName = auditTrailDb.TableName,
            RecordId = auditTrailDb.RecordId,
            ChangedBy = auditTrailDb.ChangedBy,
            Action = auditTrailDb.Action,
            Timestamp = auditTrailDb.Timestamp,
            Before = auditTrailDb.Before ?? "",
            After = auditTrailDb.After
        };
    }

    public static AuditTrailDb ToDb(this AuditTrailCreate auditTrailCreate)
    {
        return new AuditTrailDb
        {
            TableName = auditTrailCreate.TableName,
            RecordId = auditTrailCreate.RecordId,
            ChangedBy = auditTrailCreate.ChangedBy,
            Action = auditTrailCreate.Action,
            Timestamp = auditTrailCreate.Timestamp,
            Before = auditTrailCreate.Before,
            After = auditTrailCreate.After
        };
    }
}