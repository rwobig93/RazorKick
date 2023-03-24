using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Identity;

public class AppRoles : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(AppRoles).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        EnforcementOrder = 1,
        TableName = "AppRoles",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[AppRoles]'))
            begin
                CREATE TABLE [dbo].[AppRoles](
                    [Id] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
                    [Name] NVARCHAR(256) NOT NULL,
                    [NormalizedName] NVARCHAR(256) NOT NULL,
                    [ConcurrencyStamp] NVARCHAR(256) NULL,
                    [Description] NVARCHAR(4000) NOT NULL,
                    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
                    [CreatedOn] datetime2 NOT NULL,
                    [LastModifiedBy] UNIQUEIDENTIFIER NULL,
                    [LastModifiedOn] datetime2 NULL
                )
            end"
    };
    
    public static readonly MsSqlStoredProcedure Delete = new()
    {
        Table = Table,
        Action = "Delete",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppRoles_Delete]
                @Id UNIQUEIDENTIFIER
            AS
            begin
            --     archive instead in production
                delete
                from dbo.[AppRoles]
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppRoles_GetAll]
            AS
            begin
                select *
                from dbo.[AppRoles];
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetById = new()
    {
        Table = Table,
        Action = "GetById",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppRoles_GetById]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                select *
                from dbo.[AppRoles]
                where Id = @Id
                LIMIT 1;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByName = new()
    {
        Table = Table,
        Action = "GetByName",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppRoles_GetByName]
                @Name NVARCHAR(256)
            AS
            begin
                select *
                from dbo.[AppRoles]
                where Name = @Name
                LIMIT 1;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByNormalizedName = new()
    {
        Table = Table,
        Action = "GetByNormalizedName",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppRoles_GetByNormalizedName]
                @NormalizedName NVARCHAR(256)
            AS
            begin
                select *
                from dbo.[AppRoles]
                where NormalizedName = @NormalizedName
                LIMIT 1;
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppRoles_Insert]
                @Name NVARCHAR(256),
                @NormalizedName NVARCHAR(256),
                @ConcurrencyStamp NVARCHAR(256),
                @Description NVARCHAR(4000),
                @CreatedBy UNIQUEIDENTIFIER,
                @CreatedOn datetime2,
                @LastModifiedBy UNIQUEIDENTIFIER,
                @LastModifiedOn datetime2
            AS
            begin
                insert into dbo.[AppRoles] (Name, NormalizedName, ConcurrencyStamp, Description, CreatedBy, CreatedOn, LastModifiedBy, LastModifiedOn)
                OUTPUT INSERTED.Id
                values (@Name, @NormalizedName, @ConcurrencyStamp, @Description, @CreatedBy, @CreatedOn, @LastModifiedBy, @LastModifiedOn);
            end"
    };
    
    public static readonly MsSqlStoredProcedure Search = new()
    {
        Table = Table,
        Action = "Search",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppRoles_Search]
                @SearchTerm NVARCHAR(256)
            AS
            begin
                set nocount on;
                
                select *
                from dbo.[AppRoles]
                where Name LIKE '%' + @SearchTerm + '%'
                    OR Description LIKE '%' + @SearchTerm + '%';
            end"
    };
    
    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppRoles_Update]
                @Id UNIQUEIDENTIFIER,
                @Name NVARCHAR(256) = null,
                @NormalizedName NVARCHAR(256) = null,
                @ConcurrencyStamp NVARCHAR(256) = null,
                @Description NVARCHAR(4000) = null,
                @CreatedBy UNIQUEIDENTIFIER = null,
                @CreatedOn datetime2 = null,
                @LastModifiedBy UNIQUEIDENTIFIER = null,
                @LastModifiedOn datetime2 = null
            AS
            begin
                update dbo.[AppRoles]
                set Name = COALESCE(@Name, Name), NormalizedName = COALESCE(@NormalizedName, NormalizedName),
                    ConcurrencyStamp = COALESCE(@ConcurrencyStamp, ConcurrencyStamp), Description = COALESCE(@Description, Description),
                    CreatedBy = COALESCE(@CreatedBy, CreatedBy), CreatedOn = COALESCE(@CreatedOn, CreatedOn),
                    LastModifiedBy = COALESCE(@LastModifiedBy, LastModifiedBy), LastModifiedOn = COALESCE(@LastModifiedOn, LastModifiedOn)
                where Id = COALESCE(@Id, Id);
            end"
    };
    
    public static readonly MsSqlStoredProcedure SetCreatedById = new()
    {
        Table = Table,
        Action = "SetCreatedById",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppRoles_SetCreatedById]
                @Id UNIQUEIDENTIFIER,
                @CreatedBy UNIQUEIDENTIFIER
            AS
            begin
                update dbo.[AppRoles]
                set CreatedBy = @CreatedBy
                where Id = @Id;
            end"
    };
}