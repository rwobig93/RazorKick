using Application.Database.MsSql.Identity;
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
                SELECT *
                FROM dbo.[{Table.TableName}]
                ORDER BY Timestamp DESC;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAllWithUsers = new()
    {
        Table = Table,
        Action = "GetAllWithUsers",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllWithUsers]
            AS
            begin
                SELECT a.*, u.Id as ChangedBy, u.Username as ChangedByUsername
                FROM dbo.[{Table.TableName}] a
                JOIN {AppUsersMsSql.Table.TableName} u ON a.ChangedBy = u.Id
                ORDER BY Timestamp DESC;
            end"
    };

    // TODO: Add pagination procedures to other tables, then force pagination for GetAll() methods or large return methods
    public static readonly MsSqlStoredProcedure GetAllPaginated = new()
    {
        Table = Table,
        Action = "GetAllPaginated",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllPaginated]
                @Offset INT,
                @PageSize INT
            AS
            begin
                SELECT *
                FROM dbo.[{Table.TableName}]
                ORDER BY Timestamp DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAllPaginatedWithUsers = new()
    {
        Table = Table,
        Action = "GetAllPaginatedWithUsers",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllPaginatedWithUsers]
                @Offset INT,
                @PageSize INT
            AS
            begin
                SELECT a.*, u.Id as ChangedBy, u.Username as ChangedByUsername
                FROM dbo.[{Table.TableName}] a
                JOIN {AppUsersMsSql.Table.TableName} u ON a.ChangedBy = u.Id
                ORDER BY Timestamp DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            end"
    };

    public static readonly MsSqlStoredProcedure GetById = new()
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

    public static readonly MsSqlStoredProcedure GetByIdWithUser = new()
    {
        Table = Table,
        Action = "GetByIdWithUser",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByIdWithUser]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                SELECT TOP 1 a.*, u.Id as ChangedBy, u.Username as ChangedByUsername
                FROM dbo.[{Table.TableName}] a
                JOIN {AppUsersMsSql.Table.TableName} u ON a.ChangedBy = u.Id
                WHERE a.Id = @Id
                ORDER BY Id;
            end"
    };

    public static readonly MsSqlStoredProcedure GetByChangedBy = new()
    {
        Table = Table,
        Action = "GetByChangedBy",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByChangedBy]
                @UserId UNIQUEIDENTIFIER
            AS
            begin
                SELECT a.*, u.Id as ChangedBy, u.Username as ChangedByUsername
                FROM dbo.[{Table.TableName}] a
                JOIN {AppUsersMsSql.Table.TableName} u ON a.ChangedBy = u.Id
                WHERE a.ChangedBy = @UserId
                ORDER BY Timestamp DESC;
            end"
    };

    public static readonly MsSqlStoredProcedure GetByRecordId = new()
    {
        Table = Table,
        Action = "GetByRecordId",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByRecordId]
                @RecordId UNIQUEIDENTIFIER
            AS
            begin
                SELECT a.*, u.Id as ChangedBy, u.Username as ChangedByUsername
                FROM dbo.[{Table.TableName}] a
                JOIN {AppUsersMsSql.Table.TableName} u ON a.ChangedBy = u.Id
                WHERE a.RecordId = @RecordId
                ORDER BY Timestamp DESC;
            end"
    };

    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Insert]
                @TableName NVARCHAR(100),
                @RecordId UNIQUEIDENTIFIER,
                @ChangedBy UNIQUEIDENTIFIER,
                @Timestamp DATETIME2,
                @Action INT,
                @Before NVARCHAR(MAX),
                @After NVARCHAR(MAX)
            AS
            begin
                INSERT into dbo.[{Table.TableName}] (TableName, RecordId, ChangedBy, Timestamp, Action, Before, After)
                OUTPUT INSERTED.Id
                VALUES (@TableName, @RecordId, @ChangedBy, @Timestamp, @Action, @Before, @After);
            end"
    };

    public static readonly MsSqlStoredProcedure Search = new()
    {
        Table = Table,
        Action = "Search",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Search]
                @SearchTerm NVARCHAR(256)
            AS
            begin
                set nocount on;
                
                SELECT *
                FROM dbo.[{Table.TableName}]
                WHERE TableName LIKE '%' + @SearchTerm + '%'
                    OR RecordId LIKE '%' + @SearchTerm + '%'
                    OR Action LIKE '%' + @SearchTerm + '%'
                    OR Before LIKE '%' + @SearchTerm + '%'
                    OR After LIKE '%' + @SearchTerm + '%'
                ORDER BY Timestamp DESC;
            end"
    };

    public static readonly MsSqlStoredProcedure SearchWithUser = new()
    {
        Table = Table,
        Action = "SearchWithUser",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_SearchWithUser]
                @SearchTerm NVARCHAR(256)
            AS
            begin
                set nocount on;
                
                SELECT a.*, u.Id as ChangedBy, u.Username as ChangedByUsername
                FROM dbo.[{Table.TableName}] a
                JOIN {AppUsersMsSql.Table.TableName} u ON a.ChangedBy = u.Id
                WHERE TableName LIKE '%' + @SearchTerm + '%'
                    OR RecordId LIKE '%' + @SearchTerm + '%'
                    OR Action LIKE '%' + @SearchTerm + '%'
                    OR Before LIKE '%' + @SearchTerm + '%'
                    OR After LIKE '%' + @SearchTerm + '%'
                ORDER BY Timestamp DESC;
            end"
    };
    
    public static readonly MsSqlStoredProcedure DeleteOlderThan = new()
    {
        Table = Table,
        Action = "DeleteOlderThan",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_DeleteOlderThan]
                @OlderThan DATETIME2
            AS
            begin
                DELETE
                FROM dbo.[{Table.TableName}]
                WHERE Timestamp < @olderThan;
            end"
    };
}