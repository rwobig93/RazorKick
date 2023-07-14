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
                    [Value] NVARCHAR(256) NOT NULL,
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
                FROM dbo.[{Table.TableName}] e
                WHERE e.Id = @Id;
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
                FROM dbo.[{Table.TableName}] e
                WHERE e.OwnerId = @OwnerId;
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

    public static readonly MsSqlStoredProcedure GetAllPaginated = new()
    {
        Table = Table,
        Action = "GetAllPaginated",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllPaginated]
                @Offset INT,
                @PageSize INT
            AS
            begin
                SELECT e.*
                FROM dbo.[{Table.TableName}]
                ORDER BY Timestamp DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
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

    public static readonly MsSqlStoredProcedure GetAllOfTypePaginated = new()
    {
        Table = Table,
        Action = "GetAllOfTypePaginated",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllOfTypePaginated]
                @Offset INT,
                @PageSize INT
            AS
            begin
                SELECT e.*
                FROM dbo.[{Table.TableName}] e
                WHERE e.Type = @Type
                ORDER BY e.Id DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
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
                @Value NVARCHAR(256),
                @Type int
            AS
            begin
                INSERT into dbo.[{Table.TableName}] (OwnerId, Name, Value, Type)
                OUTPUT INSERTED.Id
                values (@OwnerId, @Name, @Value, @Type);
            end"
    };
    
    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Update]
                @Id UNIQUEIDENTIFIER,
                @Value NVARCHAR(256) = null
            AS
            begin
                UPDATE dbo.[{Table.TableName}]
                SET Value = COALESCE(@Value, Value)
                WHERE Id = @Id;
            end"
    };
}