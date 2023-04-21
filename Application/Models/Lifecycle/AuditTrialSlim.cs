﻿using Domain.Enums.Database;

namespace Application.Models.Lifecycle;

public class AuditTrialSlim
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