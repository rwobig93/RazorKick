using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Example;

public class ExampleObjects : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(ExampleObjects).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        EnforcementOrder = 3,
        TableName = "ExampleObjects",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[ExampleObjects]'))
            begin
                CREATE TABLE [dbo].[ExampleObjects](
                    [Id] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
                    [FirstName] NVARCHAR(50) NOT NULL,
                    [LastName] NVARCHAR(50) NOT NULL
                )
            end"
    };
    
    public static readonly MsSqlStoredProcedure Delete = new()
    {
        Table = Table,
        Action = "Delete",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleObjects_Delete]
                @Id UNIQUEIDENTIFIER
            AS
            begin
            --     archive instead in production
                delete
                from dbo.[ExampleObjects]
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetById = new()
    {
        Table = Table,
        Action = "GetById",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleObjects_GetById]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                select Id, FirstName, LastName
                from dbo.[ExampleObjects]
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleObjects_GetAll]
            AS
            begin
                select Id, FirstName, LastName
                from dbo.[ExampleObjects];
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleObjects_Insert]
                @FirstName nvarchar(50),
                @LastName nvarchar(50)
            AS
            begin
                insert into dbo.[ExampleObjects] (FirstName, LastName)
                values (@FirstName, @LastName)
                select Id = @@IDENTITY;
            end"
    };
    
    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleObjects_Update]
                @Id UNIQUEIDENTIFIER,
                @FirstName nvarchar(50),
                @LastName nvarchar(50)
            AS
            begin
                update dbo.[ExampleObjects]
                set FirstName = @FirstName, LastName = @LastName
                where Id = @Id;
            end"
    };
}