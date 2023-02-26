using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Identity;

public class AppPermissions
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(AppPermissions).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        EnforcementOrder = 1,
        TableName = "AppPermissions",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[AppPermissions]'))
            begin
                CREATE TABLE [dbo].[AppPermissions](
                    [Id] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
                    [RoleId] UNIQUEIDENTIFIER NULL,
                    [UserId] UNIQUEIDENTIFIER NULL,
                    [Name] NVARCHAR(256) NOT NULL,
                    [ClaimType] NVARCHAR(256) NULL,
                    [ClaimValue] NVARCHAR(256) NULL,
                    [Group] NVARCHAR(256) NULL,
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
            CREATE OR ALTER PROCEDURE [dbo].[spAppPermissions_Delete]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                delete
                from dbo.[AppPermissions]
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure DeleteForUser = new()
    {
        Table = Table,
        Action = "DeleteForUser",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppPermissions_DeleteForUser]
                @UserId UNIQUEIDENTIFIER
            AS
            begin
                delete
                from dbo.[AppPermissions]
                where UserId = @UserId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure DeleteForRole = new()
    {
        Table = Table,
        Action = "DeleteForRole",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppPermissions_DeleteForRole]
                @RoleId UNIQUEIDENTIFIER
            AS
            begin
                delete
                from dbo.[AppPermissions]
                where RoleId = @RoleId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppPermissions_GetAll]
            AS
            begin
                select *
                from dbo.[AppPermissions];
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetById = new()
    {
        Table = Table,
        Action = "GetById",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppPermissions_GetById]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                select *
                from dbo.[AppPermissions]
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByName = new()
    {
        Table = Table,
        Action = "GetByName",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppPermissions_GetByName]
                @Name NVARCHAR(256)
            AS
            begin
                select *
                from dbo.[AppPermissions]
                where Name = @Name;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByGroup = new()
    {
        Table = Table,
        Action = "GetByGroup",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppPermissions_GetByGroup]
                @Group NVARCHAR(256)
            AS
            begin
                select *
                from dbo.[AppPermissions]
                where Group = @Group;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByRoleId = new()
    {
        Table = Table,
        Action = "GetByRoleId",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppPermissions_GetByRoleId]
                @RoleId UNIQUEIDENTIFIER
            AS
            begin
                select *
                from dbo.[AppPermissions]
                where RoleId = @RoleId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByUserId = new()
    {
        Table = Table,
        Action = "GetByUserId",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppPermissions_GetByUserId]
                @UserId UNIQUEIDENTIFIER
            AS
            begin
                select *
                from dbo.[AppPermissions]
                where UserId = @UserId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppPermissions_Insert]
                @RoleId UNIQUEIDENTIFIER,
                @UserId UNIQUEIDENTIFIER,
                @Name NVARCHAR(256),
                @ClaimType NVARCHAR(256),
                @ClaimValue NVARCHAR(256),
                @Group NVARCHAR(256),
                @Description NVARCHAR(4000),
                @CreatedBy UNIQUEIDENTIFIER,
                @CreatedOn datetime2,
                @LastModifiedBy UNIQUEIDENTIFIER,
                @LastModifiedOn datetime2
            AS
            begin
                insert into dbo.[AppPermissions] (RoleId, UserId, Name, ClaimType, ClaimValue, Group, Description, CreatedBy, CreatedOn,
                LastModifiedBy, LastModifiedOn)
                OUTPUT INSERTED.Id
                values (@RoleId, @UserId, @Name, @ClaimType, @ClaimValue, @Group, @Description, @CreatedBy, @CreatedOn, @LastModifiedBy,
                @LastModifiedOn);
            end"
    };
    
    public static readonly MsSqlStoredProcedure Search = new()
    {
        Table = Table,
        Action = "Search",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppPermissions_Search]
                @SearchTerm NVARCHAR(256)
            AS
            begin
                set nocount on;
                
                select *
                from dbo.[AppPermissions]
                where Name LIKE '%' + @SearchTerm + '%'
                    OR Description LIKE '%' + @SearchTerm + '%'
                    OR RoleId LIKE '%' + @SearchTerm + '%'
                    OR UserId LIKE '%' + @SearchTerm + '%'
                    OR ClaimType LIKE '%' + @SearchTerm + '%'
                    OR ClaimValue LIKE '%' + @SearchTerm + '%'
                    OR Group LIKE '%' + @SearchTerm + '%';
            end"
    };
    
    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppPermissions_Update]
                @Id UNIQUEIDENTIFIER,
                @RoleId UNIQUEIDENTIFIER,
                @UserId UNIQUEIDENTIFIER,
                @Name NVARCHAR(256),
                @ClaimType NVARCHAR(256),
                @ClaimValue NVARCHAR(256),
                @Group NVARCHAR(256),
                @Description NVARCHAR(4000),
                @CreatedBy UNIQUEIDENTIFIER,
                @CreatedOn datetime2,
                @LastModifiedBy UNIQUEIDENTIFIER,
                @LastModifiedOn datetime2
            AS
            begin
                update dbo.[AppPermissions]
                set Name = @Name, RoleId = @RoleId, UserID = @UserId, ClaimType = @ClaimType, ClaimValue = @ClaimValue,
                Group = @Group, Description = @Description, CreatedBy = @CreatedBy, CreatedOn = @CreatedOn, LastModifiedBy = @LastModifiedBy,
                LastModifiedOn = @LastModifiedOn
                where Id = @Id;
            end"
    };
}