using Application.Models.Database;

namespace Application.Database.MsSql;

public class MsSqlStoredProcedure : ISqlDatabaseScript
{
    public MsSqlTable Table { get; set; } = new();
    public string Action { get; set; } = "";
    public DbResourceType Type => DbResourceType.StoredProcedure;
    public string SqlStatement { get; set; } = "";
    public int EnforcementOrder { get; init; } = 9;
    public string Path => $"sp{Table.TableName}_{Action}";
    public string FriendlyName => Path;
}