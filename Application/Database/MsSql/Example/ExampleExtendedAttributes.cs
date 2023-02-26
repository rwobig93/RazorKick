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
                    [AssignedTo] UNIQUEIDENTIFIER NOT NULL,
                    [Name] NVARCHAR(50) NOT NULL,
                    [Value] NVARCHAR(50) NOT NULL,
                    [Type] int NOT NULL
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
                select Id, AssignedTo, Name, Value, Type
                from dbo.[ExampleExtendedAttributes]
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByAssignedTo = new()
    {
        Table = Table,
        Action = "GetByAssignedTo",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleExtendedAttributes_GetByAssignedTo]
                @AssignedTo UNIQUEIDENTIFIER
            AS
            begin
                select Id, AssignedTo, Name, Value, Type
                from dbo.[ExampleExtendedAttributes]
                where AssignedTo = @AssignedTo;
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
                select Id, AssignedTo, Name, Value, Type
                from dbo.[ExampleExtendedAttributes];
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAllOfType = new()
    {
        Table = Table,
        Action = "GetAllOfType",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleExtendedAttributes_GetAllOfType]
                @Type int
            AS
            begin
                select Id, AssignedTo, Name, Value, Type
                from dbo.[ExampleExtendedAttributes]
                where Type = @Type;
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleExtendedAttributes_Insert]
                @AssignedTo UNIQUEIDENTIFIER,
                @Name nvarchar(50),
                @Value nvarchar(50),
                @Type int
            AS
            begin
                insert into dbo.[ExampleExtendedAttributes] (AssignedTo, Name, Value, Type)
                OUTPUT INSERTED.Id
                values (@AssignedTo, @Name, @Value, @Type);
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
                @Value nvarchar(50),
                @Type int
            AS
            begin
                update dbo.[ExampleExtendedAttributes]
                set Name = @Name, Value = @Value, Type = @Type
                where Id = @Id;
            end"
    };
}