using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Example;

public class BookReviews : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(BookReviews).GetDbScriptsFromClass();
    
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
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookReviews_Delete]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                update dbo.[BookReviews]
                set IsDeleted = 1
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure UnDelete = new()
    {
        Table = Table,
        Action = "UnDelete",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookReviews_UnDelete]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                update dbo.[BookReviews]
                set IsDeleted = 0
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetById = new()
    {
        Table = Table,
        Action = "GetById",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookReviews_GetById]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                select Id, BookId, Author, Content
                from dbo.[BookReviews]
                where Id = @Id AND IsDeleted = 0;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByBookId = new()
    {
        Table = Table,
        Action = "GetByBookId",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookReviews_GetByBookId]
                @BookId UNIQUEIDENTIFIER
            AS
            begin
                select Id, BookId, Author, Content
                from dbo.[BookReviews]
                where BookId = @BookId AND IsDeleted = 0;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookReviews_GetAll]
            AS
            begin
                select Id, BookId, Author, Content
                from dbo.[BookReviews]
                where IsDeleted = 0;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAllDeleted = new()
    {
        Table = Table,
        Action = "GetAllDeleted",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookReviews_GetAllDeleted]
            AS
            begin
                select Id, BookId, Author, Content
                from dbo.[BookReviews]
                where IsDeleted = 1;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAllFromAuthor = new()
    {
        Table = Table,
        Action = "GetAllFromAuthor",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookReviews_GetAllFromAuthor]
                @Author NVARCHAR(256)
            AS
            begin
                select Id, BookId, Author, Content
                from dbo.[BookReviews]
                where Author = @Author AND IsDeleted = 0;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAllFromAuthorDeleted = new()
    {
        Table = Table,
        Action = "GetAllFromAuthorDeleted",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookReviews_GetAllFromAuthorDeleted]
                @Author NVARCHAR(256)
            AS
            begin
                select Id, BookId, Author, Content
                from dbo.[BookReviews]
                where Author = @Author AND IsDeleted = 1;
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookReviews_Insert]
                @BookId UNIQUEIDENTIFIER,
                @Author NVARCHAR(256),
                @Content NVARCHAR(4000)
            AS
            begin
                insert into dbo.[BookReviews] (BookId, Author, Content)
                OUTPUT INSERTED.Id
                values (@BookId, @Author, @Content);
            end"
    };
    
    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookReviews_Update]
                @Id UNIQUEIDENTIFIER,
                @BookId UNIQUEIDENTIFIER,
                @Author NVARCHAR(256),
                @Content NVARCHAR(4000)
            AS
            begin
                update dbo.[BookReviews]
                set BookId = @BookId, Author = @Author, Content = @Content
                where Id = @Id;
            end"
    };
}