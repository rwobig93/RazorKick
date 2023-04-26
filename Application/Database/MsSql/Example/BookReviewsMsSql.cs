using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Example;

public class BookReviewsMsSql : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(BookReviewsMsSql).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        EnforcementOrder = 3,
        TableName = "BookReviews",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[BookReviews]'))
            begin
                CREATE TABLE [dbo].[BookReviews](
                    [Id] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
                    [BookId] UNIQUEIDENTIFIER NOT NULL,
                    [Author] NVARCHAR(256) NOT NULL,
                    [Content] NVARCHAR(4000) NOT NULL,
                    [IsDeleted] BIT NOT NULL
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
                UPDATE dbo.[{Table.TableName}]
                SET IsDeleted = 1
                WHERE Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure UnDelete = new()
    {
        Table = Table,
        Action = "UnDelete",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_UnDelete]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                UPDATE dbo.[{Table.TableName}]
                SET IsDeleted = 0
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
                WHERE Id = @Id AND IsDeleted = 0
                ORDER BY Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByBookId = new()
    {
        Table = Table,
        Action = "GetByBookId",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByBookId]
                @BookId UNIQUEIDENTIFIER
            AS
            begin
                SELECT b.*
                FROM dbo.[{Table.TableName}] b
                WHERE b.BookId = @BookId AND b.IsDeleted = 0;
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
                WHERE IsDeleted = 0;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAllDeleted = new()
    {
        Table = Table,
        Action = "GetAllDeleted",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllDeleted]
            AS
            begin
                SELECT *
                FROM dbo.[{Table.TableName}]
                WHERE IsDeleted = 1;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAllFromAuthor = new()
    {
        Table = Table,
        Action = "GetAllFromAuthor",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllFromAuthor]
                @Author NVARCHAR(256)
            AS
            begin
                SELECT *
                FROM dbo.[{Table.TableName}]
                WHERE Author = @Author AND IsDeleted = 0;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAllFromAuthorDeleted = new()
    {
        Table = Table,
        Action = "GetAllFromAuthorDeleted",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllFromAuthorDeleted]
                @Author NVARCHAR(256)
            AS
            begin
                SELECT *
                FROM dbo.[{Table.TableName}]
                WHERE Author = @Author AND IsDeleted = 1;
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Insert]
                @BookId UNIQUEIDENTIFIER,
                @Author NVARCHAR(256),
                @Content NVARCHAR(4000)
            AS
            begin
                INSERT into dbo.[{Table.TableName}] (BookId, Author, Content)
                OUTPUT INSERTED.Id
                values (@BookId, @Author, @Content);
            end"
    };
    
    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Update]
                @Id UNIQUEIDENTIFIER,
                @BookId UNIQUEIDENTIFIER = null,
                @Author NVARCHAR(256) = null,
                @Content NVARCHAR(4000) = null
            AS
            begin
                UPDATE dbo.[{Table.TableName}]
                SET BookId = COALESCE(@BookId, BookId), Author = COALESCE(@Author, Author), Content = COALESCE(@Content, Content)
                WHERE Id = @Id;
            end"
    };
}