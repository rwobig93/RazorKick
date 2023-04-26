using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Example;

public class BooksMsSql : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(BooksMsSql).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        EnforcementOrder = 3,
        TableName = "Books",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[Books]'))
            begin
                CREATE TABLE [dbo].[Books](
                    [Id] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
                    [Name] NVARCHAR(256) NOT NULL,
                    [Author] NVARCHAR(256) NOT NULL,
                    [Pages] INT NOT NULL
                )
            end"
    };
    
    public static readonly MsSqlStoredProcedure Delete = new()
    {
        Table = Table,
        Action = "Delete",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Delete]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                DELETE
                FROM dbo.[{Table.TableName}]
                WHERE Id = @Id;
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
    
    public static readonly MsSqlStoredProcedure GetByIdFull = new()
    {
        Table = Table,
        Action = "GetByIdFull",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByIdFull]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                SELECT b.*, g.*
                FROM dbo.[{Table.TableName}] b
                JOIN dbo.[{BookGenreJunctionsMsSql.Table.TableName}] bg ON b.Id = bg.BookId
                JOIN dbo.[{BookGenresMsSql.Table.TableName}] g ON g.Id = bg.GenreId
                WHERE b.Id = @Id;
            end"
    };
    
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
                ORDER BY Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Insert]
                @Name NVARCHAR(256),
                @Author NVARCHAR(256),
                @Pages INT
            AS
            begin
                INSERT into dbo.[{Table.TableName}] (Name, Author, Pages)
                OUTPUT INSERTED.id
                VALUES (@Name, @Author, @Pages);
            end"
    };
    
    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Update]
                @Id UNIQUEIDENTIFIER,
                @Name NVARCHAR(256) = null,
                @Author NVARCHAR(256) = null,
                @Pages INT = null
            AS
            begin
                UPDATE dbo.[{Table.TableName}]
                SET Name = COALESCE(@Name, Name), Author = COALESCE(@Author, Author), Pages = COALESCE(@Pages, Pages)
                WHERE Id = @Id;
            end"
    };
}