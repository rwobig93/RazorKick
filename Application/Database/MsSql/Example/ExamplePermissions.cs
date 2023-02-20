using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Example;

public class ExamplePermissions : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(ExamplePermissions).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        TableName = "ExamplePermissions",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[ExamplePermissions]'))
            begin
                CREATE TABLE [dbo].[ExamplePermissions](
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
            CREATE OR ALTER PROCEDURE [dbo].[spExamplePermissions_Delete]
                @Id UNIQUEIDENTIFIER
            AS
            begin
            --     archive instead in production
                delete
                from dbo.[ExamplePermissions]
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetById = new()
    {
        Table = Table,
        Action = "GetById",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExamplePermissions_GetById]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                select Id, Name, Value
                from dbo.[ExamplePermissions]
                where Name = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByName = new()
    {
        Table = Table,
        Action = "GetByName",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExamplePermissions_GetByName]
                @Name NVARCHAR(50)
            AS
            begin
                select Id, Name, Value
                from dbo.[ExamplePermissions]
                where Name = @Name;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByValue = new()
    {
        Table = Table,
        Action = "GetByValue",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExamplePermissions_GetByValue]
                @Value NVARCHAR(50)
            AS
            begin
                select Id, Name, Value
                from dbo.[ExamplePermissions]
                where Value = @Value;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExamplePermissions_GetAll]
            AS
            begin
                select Id, Name, Value
                from dbo.[ExamplePermissions];
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExamplePermissions_Insert]
                @Name nvarchar(50),
                @Value nvarchar(50)
            AS
            begin
                insert into dbo.[ExamplePermissions] (Name, Value)
                values (@Name, @Value)
                select @Id = @@IDENTITY;
            end"
    };
    
    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExamplePermissions_Update]
                @Id UNIQUEIDENTIFIER,
                @Name nvarchar(50),
                @Value nvarchar(50)
            AS
            begin
                update dbo.[ExamplePermissions]
                set Name = @Name, Value = @Value
                where Id = @Id;
            end"
    };
}