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
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBooks_Delete]
                @Id UNIQUEIDENTIFIER
            AS
            begin
            --     archive instead in production
                delete
                from dbo.[Books]
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetById = new()
    {
        Table = Table,
        Action = "GetById",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBooks_GetById]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                SELECT TOP 1 Id, Name, Author
                from dbo.[Books]
                where Id = @Id
                ORDER BY Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByIdFull = new()
    {
        Table = Table,
        Action = "GetByIdFull",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBooks_GetByIdFull]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                select obj.Id, obj.Name, obj.Author from dbo.[Books] obj
                LEFT JOIN dbo.[BookReviews] attr ON obj.Id = attr.BookId
                where obj.Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBooks_GetAll]
            AS
            begin
                select Id, Name, Author, Pages
                from dbo.[Books];
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBooks_Insert]
                @Name NVARCHAR(256),
                @Author NVARCHAR(256),
                @Pages INT
            AS
            begin
                insert into dbo.[Books] (Name, Author, Pages)
                OUTPUT INSERTED.id
                values (@Name, @Author, @Pages);
            end"
    };
    
    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBooks_Update]
                @Id UNIQUEIDENTIFIER,
                @Name NVARCHAR(256),
                @Author NVARCHAR(256),
                @Pages INT
            AS
            begin
                update dbo.[Books]
                set Name = @Name, Author = @Author, Pages = @Pages
                where Id = @Id;
            end"
    };
}