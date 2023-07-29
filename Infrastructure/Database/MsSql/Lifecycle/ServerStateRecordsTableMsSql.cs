using Application.Database;
using Application.Database.Tables.Lifecycle;
using Application.Helpers.Runtime;

namespace Infrastructure.Database.MsSql.Lifecycle;

public class ServerStateRecordsTableMsSql : IServerStateRecordsTable
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(ServerStateRecordsTableMsSql).GetDbScriptsFromClass();
    
    public static readonly SqlTable Table = new()
    {
        TableName = "ServerStateRecords",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[ServerStateRecords]'))
            begin
                CREATE TABLE [dbo].[ServerStateRecords](
                    [Id] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
                    [AppVersion] NVARCHAR(128) NOT NULL,
                    [Timestamp] DATETIME2 NOT NULL
                )
            end"
    };

    public static readonly SqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAll]
            AS
            begin
                SELECT *
                FROM dbo.[{Table.TableName}]
                ORDER BY Timestamp DESC;
            end"
    };

    public static readonly SqlStoredProcedure GetAllBeforeDate = new()
    {
        Table = Table,
        Action = "GetAllBeforeDate",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllBeforeDate]
                @OlderThan DATETIME2
            AS
            begin
                SELECT *
                FROM dbo.[{Table.TableName}]
                WHERE Timestamp < @OlderThan
                ORDER BY Timestamp DESC;
            end"
    };

    public static readonly SqlStoredProcedure GetAllAfterDate = new()
    {
        Table = Table,
        Action = "GetAllAfterDate",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllAfterDate]
                @NewerThan DATETIME2
            AS
            begin
                SELECT *
                FROM dbo.[{Table.TableName}]
                WHERE Timestamp > @NewerThan
                ORDER BY Timestamp DESC;
            end"
    };

    public static readonly SqlStoredProcedure GetById = new()
    {
        Table = Table,
        Action = "GetById",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetById]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                SELECT TOP 1 *
                FROM dbo.[{Table.TableName}]
                WHERE Id = @Id
                ORDER BY Id;
            end"
    };

    public static readonly SqlStoredProcedure GetByVersion = new()
    {
        Table = Table,
        Action = "GetByVersion",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByVersion]
                @Version NVARCHAR(128)
            AS
            begin
                SELECT *
                FROM dbo.[{Table.TableName}]
                WHERE AppVersion = @Version
                ORDER BY Id;
            end"
    };

    public static readonly SqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Insert]
                @AppVersion NVARCHAR(128),
                @Timestamp DATETIME2
            AS
            begin
                INSERT into dbo.[{Table.TableName}] (AppVersion, Timestamp)
                OUTPUT INSERTED.Id
                VALUES (@AppVersion, @Timestamp);
            end"
    };
}