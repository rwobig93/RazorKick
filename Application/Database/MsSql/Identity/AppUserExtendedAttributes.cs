using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Identity;

public class AppUserExtendedAttributes : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(AppUserExtendedAttributes).GetDbScriptsFromClass();
    
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
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserExtendedAttributes_Delete]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                delete
                from dbo.[AppUserExtendedAttributes]
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure DeleteAllForOwner = new()
    {
        Table = Table,
        Action = "DeleteAllForOwner",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserExtendedAttributes_DeleteAllForOwner]
                @OwnerId UNIQUEIDENTIFIER
            AS
            begin
                delete
                from dbo.[AppUserExtendedAttributes]
                where OwnerId = @OwnerId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetById = new()
    {
        Table = Table,
        Action = "GetById",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserExtendedAttributes_GetById]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                SELECT TOP 1 Id, OwnerId, Name, Value, Type
                from dbo.[AppUserExtendedAttributes]
                where Id = @Id
                ORDER BY Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByOwnerId = new()
    {
        Table = Table,
        Action = "GetByOwnerId",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserExtendedAttributes_GetByOwnerId]
                @OwnerId UNIQUEIDENTIFIER
            AS
            begin
                select Id, OwnerId, Name, Value, Type
                from dbo.[AppUserExtendedAttributes]
                where OwnerId = @OwnerId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByName = new()
    {
        Table = Table,
        Action = "GetByName",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserExtendedAttributes_GetByName]
                @Name NVARCHAR(256)
            AS
            begin
                select Id, OwnerId, Name, Value, Type
                from dbo.[AppUserExtendedAttributes]
                where Name = @Name;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserExtendedAttributes_GetAll]
            AS
            begin
                select Id, OwnerId, Name, Value, Type
                from dbo.[AppUserExtendedAttributes];
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAllOfType = new()
    {
        Table = Table,
        Action = "GetAllOfType",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserExtendedAttributes_GetAllOfType]
                @Type int
            AS
            begin
                select Id, OwnerId, Name, Value, Type
                from dbo.[AppUserExtendedAttributes]
                where Type = @Type;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAllOfTypeForOwner = new()
    {
        Table = Table,
        Action = "GetAllOfTypeForOwner",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserExtendedAttributes_GetAllOfTypeForOwner]
                @OwnerId UNIQUEIDENTIFIER,
                @Type int
            AS
            begin
                select Id, OwnerId, Name, Value, Type
                from dbo.[AppUserExtendedAttributes]
                where OwnerId = @OwnerId AND Type = @Type;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAllOfNameForOwner = new()
    {
        Table = Table,
        Action = "GetAllOfNameForOwner",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserExtendedAttributes_GetAllOfNameForOwner]
                @OwnerId UNIQUEIDENTIFIER,
                @Name NVARCHAR(256)
            AS
            begin
                select Id, OwnerId, Name, Value, Type
                from dbo.[AppUserExtendedAttributes]
                where OwnerId = @OwnerId AND Name = @Name;
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserExtendedAttributes_Insert]
                @OwnerId UNIQUEIDENTIFIER,
                @Name NVARCHAR(256),
                @Value NVARCHAR(256),
                @Type int
            AS
            begin
                insert into dbo.[AppUserExtendedAttributes] (OwnerId, Name, Value, Type)
                OUTPUT INSERTED.Id
                values (@OwnerId, @Name, @Value, @Type);
            end"
    };
    
    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserExtendedAttributes_Update]
                @Id UNIQUEIDENTIFIER,
                @Value NVARCHAR(256) = null
            AS
            begin
                update dbo.[AppUserExtendedAttributes]
                set Value = COALESCE(@Value, Value)
                where Id = @Id;
            end"
    };
}