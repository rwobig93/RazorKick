using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Example;

public class BookGenres : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(BookGenres).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        EnforcementOrder = 1,
        TableName = "BookGenres",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[BookGenres]'))
            begin
                CREATE TABLE [dbo].[BookGenres](
                    [Id] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
                    [Name] NVARCHAR(50) NOT NULL,
                    [Value] NVARCHAR(50) NOT NULL,
                )
            end"
    };
    
    public static readonly MsSqlStoredProcedure Delete = new()
    {
        Table = Table,
        Action = "Delete",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookGenres_Delete]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                delete
                from dbo.[BookGenres]
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetById = new()
    {
        Table = Table,
        Action = "GetById",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookGenres_GetById]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                SELECT TOP 1 Id, Name, Value
                from dbo.[BookGenres]
                where Name = @Id
                ORDER BY Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByName = new()
    {
        Table = Table,
        Action = "GetByName",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookGenres_GetByName]
                @Name NVARCHAR(50)
            AS
            begin
                SELECT TOP 1 Id, Name, Value
                from dbo.[BookGenres]
                where Name = @Name
                ORDER BY Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByValue = new()
    {
        Table = Table,
        Action = "GetByValue",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookGenres_GetByValue]
                @Value NVARCHAR(50)
            AS
            begin
                select Id, Name, Value
                from dbo.[BookGenres]
                where Value = @Value;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookGenres_GetAll]
            AS
            begin
                select Id, Name, Value
                from dbo.[BookGenres];
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookGenres_Insert]
                @Name nvarchar(50),
                @Value nvarchar(50)
            AS
            begin
                insert into dbo.[BookGenres] (Name, Value)
                OUTPUT INSERTED.Id
                values (@Name, @Value);
            end"
    };
    
    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spBookGenres_Update]
                @Id UNIQUEIDENTIFIER,
                @Name nvarchar(50),
                @Value nvarchar(50)
            AS
            begin
                update dbo.[BookGenres]
                set Name = @Name, Value = @Value
                where Id = @Id;
            end"
    };
}