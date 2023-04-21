using Domain.DatabaseEntities.Lifecycle;
using Domain.Enums.Database;
using Shared.Responses.Lifecycle;

namespace Application.Models.Lifecycle;

public class AuditTrailSlim
{
    public Guid Id { get; set; }
    public string TableName { get; set; } = null!;
    public Guid RecordId { get; set; }
    public Guid ChangedBy { get; set; }
    public string ChangedByUsername { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public DatabaseActionType Action { get; set; }
    public Dictionary<string, string>? Before { get; set; }
    public Dictionary<string, string> After { get; set; } = null!;
}

public static class AuditTrailSlimExtensions
{
    public static AuditTrailSlim ToSlim(this AuditTrailWithUserDb auditTrailDb)
    {
        return new AuditTrailSlim
        {
            Id = auditTrailDb.Id,
            TableName = auditTrailDb.TableName,
            RecordId = auditTrailDb.RecordId,
            ChangedBy = auditTrailDb.ChangedBy,
            ChangedByUsername = auditTrailDb.ChangedByUsername,
            Timestamp = auditTrailDb.Timestamp,
            Action = auditTrailDb.Action,
            Before = new Dictionary<string, string>(),
            After = new Dictionary<string, string>()
        };
    }

    public static IEnumerable<AuditTrailSlim> ToSlims(this IEnumerable<AuditTrailWithUserDb> auditTrailDbs)
    {
        return auditTrailDbs.Select(x => x.ToSlim());
    }

    public static AuditTrailResponse ToResponse(this AuditTrailSlim auditTrailSlim)
    {
        return new AuditTrailResponse
        {
            Id = auditTrailSlim.Id,
            TableName = auditTrailSlim.TableName,
            RecordId = auditTrailSlim.RecordId,
            ChangedBy = auditTrailSlim.ChangedBy,
            ChangedByUsername = auditTrailSlim.ChangedByUsername,
            Timestamp = auditTrailSlim.Timestamp,
            Action = nameof(auditTrailSlim.Action),
            Before = auditTrailSlim.Before,
            After = auditTrailSlim.After
        };
    }

    public static IEnumerable<AuditTrailResponse> ToResponses(this IEnumerable<AuditTrailSlim> auditTrailSlims)
    {
        return auditTrailSlims.Select(x => x.ToResponse());
    }
}