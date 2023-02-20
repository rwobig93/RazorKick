using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Example;

public class ExampleExtendedAttributes : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(ExampleExtendedAttributes).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        EnforcementOrder = 3,
        TableName = "ExampleExtendedAttributes",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[ExampleExtendedAttributes]'))
            begin
                CREATE TABLE [dbo].[ExampleExtendedAttributes](
                    [Id] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
                    [Name] NVARCHAR(50) NOT NULL,
                    [Value] NVARCHAR(50) NOT NULL
                )
            end"
    };
    
    public static readonly MsSqlStoredProcedure Delete = new()
    {
        Table = Table,
        Action = "Delete",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleExtendedAttributes_Delete]
                @Id UNIQUEIDENTIFIER
            AS
            begin
            --     archive instead in production
                delete
                from dbo.[ExampleExtendedAttributes]
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetById = new()
    {
        Table = Table,
        Action = "GetById",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleExtendedAttributes_GetById]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                select Id, Name, Value
                from dbo.[ExampleExtendedAttributes]
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleExtendedAttributes_GetAll]
            AS
            begin
                select Id, Name, Value
                from dbo.[ExampleExtendedAttributes];
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleExtendedAttributes_Insert]
                @Name nvarchar(50),
                @Value nvarchar(50)
            AS
            begin
                insert into dbo.[ExampleExtendedAttributes] (Name, Value)
                values (@Name, @Value);
            end"
    };
    
    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleExtendedAttributes_Update]
                @Id UNIQUEIDENTIFIER,
                @Name nvarchar(50),
                @Value nvarchar(50)
            AS
            begin
                update dbo.[ExampleExtendedAttributes]
                set Name = @Name, Value = @Value
                where Id = @Id;
            end"
    };
}