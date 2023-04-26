using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Example;

public class BookGenreJunctionsMsSql : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(BookGenreJunctionsMsSql).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        TableName = "BookGenreJunctions",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[BookGenreJunctions]'))
            begin
                CREATE TABLE [dbo].[BookGenreJunctions](
                    [BookId] UNIQUEIDENTIFIER NOT NULL,
                    [GenreId] UNIQUEIDENTIFIER NOT NULL,
                    CONSTRAINT BookGenre_PK PRIMARY KEY (BookId, GenreId),
                    CONSTRAINT FK_Book
                        FOREIGN KEY (BookId) REFERENCES dbo.[Books] (Id),
                    CONSTRAINT FK_Genre
                        FOREIGN KEY (GenreId) REFERENCES dbo.[BookGenres] (Id)
                )
            end"
    };
    
    public static readonly MsSqlStoredProcedure Delete = new()
    {
        Table = Table,
        Action = "Delete",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Delete]
                @BookId UNIQUEIDENTIFIER,
                @GenreId UNIQUEIDENTIFIER
            AS
            begin
                DELETE
                FROM dbo.[{Table.TableName}]
                WHERE BookId = @BookId AND
                      GenreId = @GenreId;
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
                SELECT BookId, GenreId
                FROM dbo.[{Table.TableName}];
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetGenresForBook = new()
    {
        Table = Table,
        Action = "GetGenresForBook",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetGenresForBook]
                @BookId UNIQUEIDENTIFIER
            AS
            begin
                SELECT GenreId
                FROM dbo.[{Table.TableName}]
                WHERE BookId = @BookId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetBooksForGenre = new()
    {
        Table = Table,
        Action = "GetBooksForGenre",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetBooksForGenre]
                @GenreId UNIQUEIDENTIFIER
            AS
            begin
                SELECT BookId
                FROM dbo.[{Table.TableName}]
                WHERE GenreId = @GenreId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Insert]
                @BookId UNIQUEIDENTIFIER,
                @GenreId UNIQUEIDENTIFIER
            AS
            begin
                insert into dbo.[{Table.TableName}] (BookId, GenreId)
                VALUES (@BookId, @GenreId);
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
                SET nocount on;
                
                SELECT *
                FROM dbo.[{Table.TableName}]
                WHERE BookId LIKE '%' + @SearchTerm + '%'
                    OR GenreId LIKE '%' + @SearchTerm + '%';
            end"
    };
}