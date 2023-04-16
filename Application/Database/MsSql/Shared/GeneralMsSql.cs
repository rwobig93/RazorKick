using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Shared;

public class GeneralMsSql : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(GeneralMsSql).GetDbScriptsFromClass();
    
    public static readonly MsSqlStoredProcedure GetRowCount = new()
    {
        Table = new MsSqlTable() { TableName = "General" },
        Action = "GetRowCount",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spGeneral_GetRowCount]
                @TableName NVARCHAR(50)
            AS
            begin
                SELECT OBJECT_NAME(object_id), SUM(row_count) AS rows
                FROM sys.dm_db_partition_stats
                WHERE object_id = OBJECT_ID(@TableName)
                  AND index_id < 2
                GROUP BY OBJECT_NAME(object_id);
            end"
    };
}