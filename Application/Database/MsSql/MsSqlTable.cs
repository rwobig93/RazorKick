using Application.Models.Database;

namespace Application.Database.MsSql;

public class MsSqlTable : ISqlDatabaseScript
{
    public string TableName { get; set; } = "";
    public string SqlStatement { get; set; } = "";
    public string FriendlyName => TableName;
    public DbResourceType Type => DbResourceType.Table;
    public string Path => TableName;
    public int EnforcementOrder { get; init; } = 5;
}