using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Identity;

public class AppUserExtendedAttributesMsSql : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(AppUserExtendedAttributesMsSql).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        EnforcementOrder = 3,
        TableName = "AppUserExtendedAttributes",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[AppUserExtendedAttributes]'))
            begin
                CREATE TABLE [dbo].[AppUserExtendedAttributes](
                    [Id] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
                    [OwnerId] UNIQUEIDENTIFIER NOT NULL,
                    [Name] NVARCHAR(256) NOT NULL,
                    [Value] NVARCHAR(512) NOT NULL,
                    [Description] NVARCHAR(1024) NULL,
                    [Type] int NOT NULL
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
    
    public static readonly MsSqlStoredProcedure DeleteAllForOwner = new()
    {
        Table = Table,
        Action = "DeleteAllForOwner",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_DeleteAllForOwner]
                @OwnerId UNIQUEIDENTIFIER
            AS
            begin
                DELETE
                FROM dbo.[{Table.TableName}]
                WHERE OwnerId = @OwnerId;
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
                SELECT TOP 1 e.*
                FROM dbo.[{Table.TableName}] e
                WHERE e.Id = @Id
                ORDER BY e.Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByOwnerId = new()
    {
        Table = Table,
        Action = "GetByOwnerId",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByOwnerId]
                @OwnerId UNIQUEIDENTIFIER
            AS
            begin
                SELECT e.*
                FROM dbo.[{Table.TableName}] e
                WHERE e.OwnerId = @OwnerId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByName = new()
    {
        Table = Table,
        Action = "GetByName",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByName]
                @Name NVARCHAR(256)
            AS
            begin
                SELECT e.*
                FROM dbo.[{Table.TableName}] e
                WHERE e.Name = @Name;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByTypeAndValue = new()
    {
        Table = Table,
        Action = "GetByTypeAndValue",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByTypeAndValue]
                @Type int,
                @Value NVARCHAR(256)
            AS
            begin
                SELECT e.*
                FROM dbo.[{Table.TableName}] e
                WHERE e.Value = @Value AND e.Type = @Type
                ORDER BY e.Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByTypeAndValueForOwner = new()
    {
        Table = Table,
        Action = "GetByTypeAndValueForOwner",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByTypeAndValueForOwner]
                @OwnerId UNIQUEIDENTIFIER,
                @Type int,
                @Value NVARCHAR(256)
            AS
            begin
                SELECT e.*
                FROM dbo.[{Table.TableName}] e
                WHERE e.Value = @Value AND e.Type = @Type AND e.OwnerId = @OwnerId
                ORDER BY e.Id;
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
                SELECT e.*
                FROM dbo.[{Table.TableName}] e;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAllOfType = new()
    {
        Table = Table,
        Action = "GetAllOfType",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllOfType]
                @Type int
            AS
            begin
                SELECT e.*
                FROM dbo.[{Table.TableName}] e
                WHERE e.Type = @Type;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAllOfTypeForOwner = new()
    {
        Table = Table,
        Action = "GetAllOfTypeForOwner",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllOfTypeForOwner]
                @OwnerId UNIQUEIDENTIFIER,
                @Type int
            AS
            begin
                SELECT e.*
                FROM dbo.[{Table.TableName}] e
                WHERE e.OwnerId = @OwnerId AND e.Type = @Type;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAllOfNameForOwner = new()
    {
        Table = Table,
        Action = "GetAllOfNameForOwner",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllOfNameForOwner]
                @OwnerId UNIQUEIDENTIFIER,
                @Name NVARCHAR(256)
            AS
            begin
                SELECT e.*
                FROM dbo.[{Table.TableName}] e
                WHERE e.OwnerId = @OwnerId AND e.Name = @Name;
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Insert]
                @OwnerId UNIQUEIDENTIFIER,
                @Name NVARCHAR(256),
                @Value NVARCHAR(512),
                @Description NVARCHAR(1024),
                @Type int
            AS
            begin
                INSERT into dbo.[{Table.TableName}] (OwnerId, Name, Value, Description, Type)
                OUTPUT INSERTED.Id
                values (@OwnerId, @Name, @Value, @Description, @Type);
            end"
    };
    
    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Update]
                @Id UNIQUEIDENTIFIER,
                @Value NVARCHAR(512) = null,
                @Description NVARCHAR(1024) = null
            AS
            begin
                UPDATE dbo.[{Table.TableName}]
                SET Value = COALESCE(@Value, Value), Description = COALESCE(@Description, Description)
                WHERE Id = @Id;
            end"
    };
}