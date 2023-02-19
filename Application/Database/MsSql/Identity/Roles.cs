using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Identity;

public class Roles : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(Roles).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        EnforcementOrder = 1,
        TableName = "Roles",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[Roles]'))
            begin
                CREATE TABLE [dbo].[Roles](
                    [Id] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
                    [Name] NVARCHAR(256) NOT NULL,
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
            CREATE OR ALTER PROCEDURE [dbo].[spRole_Delete]
                @Id UNIQUEIDENTIFIER
            AS
            begin
            --     archive instead in production
                delete
                from dbo.[Roles]
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spRole_GetAll]
            AS
            begin
                select *
                from dbo.[Roles];
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetById = new()
    {
        Table = Table,
        Action = "GetById",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spRole_GetById]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                select *
                from dbo.[Roles]
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spRole_Insert]
                @Name NVARCHAR(256),
                @Description NVARCHAR(4000),
                @CreatedBy UNIQUEIDENTIFIER,
                @CreatedOn datetime2,
                @LastModifiedBy UNIQUEIDENTIFIER,
                @LastModifiedOn datetime2
            AS
            begin
                insert into dbo.[Roles] (Name, Description, CreatedBy, CreatedOn, LastModifiedBy, LastModifiedOn)
                values (@Name, @Description, @CreatedBy, @CreatedOn, @LastModifiedBy, @LastModifiedOn);
            end"
    };
    
    public static readonly MsSqlStoredProcedure Search = new()
    {
        Table = Table,
        Action = "Search",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spRole_Search]
                @SearchTerm NVARCHAR(256)
            AS
            begin
                set nocount on;
                
                select *
                from dbo.[Roles]
                where Name LIKE '%' + @SearchTerm + '%'
                    OR Description LIKE '%' + @SearchTerm + '%';
            end"
    };
    
    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spRole_Update]
                @Id UNIQUEIDENTIFIER,
                @Name NVARCHAR(256),
                @Description NVARCHAR(4000),
                @CreatedBy UNIQUEIDENTIFIER,
                @CreatedOn datetime2,
                @LastModifiedBy UNIQUEIDENTIFIER,
                @LastModifiedOn datetime2
            AS
            begin
                update dbo.[Roles]
                set Name = @Name, Description = @Description, CreatedBy = @CreatedBy, CreatedOn = @CreatedOn,
                    LastModifiedBy = @LastModifiedBy, LastModifiedOn = @LastModifiedOn
                where Id = @Id;
            end"
    };
}