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
                SELECT SUM(spart.rows)
                FROM sys.partitions spart
                WHERE spart.object_id = object_id(@TableName)
                AND spart.index_id < 2;
            end"
    };
    
    public static readonly MsSqlStoredProcedure VerifyConnectivity = new()
    {
        Table = new MsSqlTable() { TableName = "General" },
        Action = "VerifyConnectivity",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spGeneral_VerifyConnectivity]
            AS
            begin
                SELECT 1;
            end"
    };
}