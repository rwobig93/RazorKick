using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Lifecycle;

public class AuditTrails : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(AuditTrails).GetDbScriptsFromClass();

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
                    [Action] NVARCHAR(10) NOT NULL,
                    [Before] NVARCHAR(MAX) NULL,
                    [After] NVARCHAR(MAX) NULL
                )
            end"
    };
}