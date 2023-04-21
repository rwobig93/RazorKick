using Domain.DatabaseEntities.Lifecycle;
using Domain.Enums.Database;

namespace Application.Models.Lifecycle;

public class AuditTrailCreate
{
    public Guid Id { get; set; }
    public string TableName { get; set; } = null!;
    public Guid RecordId { get; set; }
    public Guid ChangedBy { get; set; } = Guid.Empty;
    public DatabaseActionType Action { get; set; }
    public Dictionary<string, string>? Before { get; set; }
    public Dictionary<string, string> After { get; set; } = null!;
}

public static class AuditTrailCreateExtensions
{
    public static AuditTrailCreate ToCreate(this AuditTrailDb auditTrailDb)
    {
        return new AuditTrailCreate
        {
            Id = auditTrailDb.Id,
            TableName = auditTrailDb.TableName,
            RecordId = auditTrailDb.RecordId,
            ChangedBy = auditTrailDb.ChangedBy,
            Action = auditTrailDb.Action,
            Before = new Dictionary<string, string>(),
            After = new Dictionary<string, string>()
        };
    }

    public static AuditTrailDb ToDb(this AuditTrailCreate auditTrailCreate)
    {
        return new AuditTrailDb
        {
            Id = auditTrailCreate.Id,
            TableName = auditTrailCreate.TableName,
            RecordId = auditTrailCreate.RecordId,
            ChangedBy = auditTrailCreate.ChangedBy,
            Action = auditTrailCreate.Action,
            Before = "",
            After = ""
        };
    }
}