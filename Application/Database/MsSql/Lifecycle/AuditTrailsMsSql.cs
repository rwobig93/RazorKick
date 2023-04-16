using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Lifecycle;

public class AuditTrailsMsSql : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(AuditTrailsMsSql).GetDbScriptsFromClass();

    public static readonly MsSqlTable Table = new()
    {
        TableName = "AuditTrails",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[AuditTrails]'))
            begin
                CREATE TABLE [dbo].[AuditTrails](
                    [Id] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
                    [TableName] NVARCHAR(100) NOT NULL,
                    [RecordId] UNIQUEIDENTIFIER NOT NULL,
                    [ChangedBy] UNIQUEIDENTIFIER NOT NULL,
                    [Timestamp] DATETIME2 NOT NULL,
                    [Action] INT NOT NULL,
                    [Before] NVARCHAR(MAX) NULL,
                    [After] NVARCHAR(MAX) NOT NULL
                )
            end"
    };

    // TODO: Move other stored procedures to string interpolation with table name injection
    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAll]
            AS
            begin
                select *
                from dbo.[{Table.TableName}]
                where IsDeleted = 0;
            end"
    };

    // TODO: Add pagination procedures to other tables
    public static readonly MsSqlStoredProcedure GetAllPaginated = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllPaginated]
                @Offset NVARCHAR(256),
                @PageSize NVARCHAR(256)
            AS
            begin
                SELECT *
                FROM dbo.[{Table.TableName}]
                ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                where IsDeleted = 0;
            end"
    };
}