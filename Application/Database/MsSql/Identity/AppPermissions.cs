using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Identity;

public class AppPermissions : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(AppPermissions).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        EnforcementOrder = 3,
        TableName = "AppPermissions",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[AppPermissions]'))
            begin
                CREATE TABLE [dbo].[AppPermissions](
                    [Id] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
                    [RoleId] UNIQUEIDENTIFIER NULL,
                    [UserId] UNIQUEIDENTIFIER NULL,
                    [ClaimType] NVARCHAR(256) NULL,
                    [ClaimValue] NVARCHAR(1024) NULL,
                    [Name] NVARCHAR(256) NOT NULL,
                    [Group] NVARCHAR(256) NOT NULL,
                    [Access] NVARCHAR(256) NOT NULL,
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
                where [Group] = @Group;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByAccess = new()
    {
        Table = Table,
        Action = "GetByAccess",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppPermissions_GetByAccess]
                @Access NVARCHAR(256)
            AS
            begin
                select *
                from dbo.[AppPermissions]
                where Access = @Access;
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
    
    public static readonly MsSqlStoredProcedure GetByRoleIdAndValue = new()
    {
        Table = Table,
        Action = "GetByRoleIdAndValue",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppPermissions_GetByRoleIdAndValue]
                @RoleId UNIQUEIDENTIFIER,
                @ClaimValue NVARCHAR(1024)
            AS
            begin
                select *
                from dbo.[AppPermissions]
                where RoleId = @RoleId AND ClaimValue = @ClaimValue;
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
    
    public static readonly MsSqlStoredProcedure GetByUserIdAndValue = new()
    {
        Table = Table,
        Action = "GetByUserIdAndValue",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppPermissions_GetByUserIdAndValue]
                @UserId UNIQUEIDENTIFIER,
                @ClaimValue NVARCHAR(1024)
            AS
            begin
                select *
                from dbo.[AppPermissions]
                where UserId = @UserId AND ClaimValue = @ClaimValue;
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
                @ClaimType NVARCHAR(256),
                @ClaimValue NVARCHAR(1024),
                @Name NVARCHAR(256),
                @Group NVARCHAR(256),
                @Access NVARCHAR(256),
                @Description NVARCHAR(4000),
                @CreatedBy UNIQUEIDENTIFIER,
                @CreatedOn datetime2,
                @LastModifiedBy UNIQUEIDENTIFIER,
                @LastModifiedOn datetime2
            AS
            begin
                insert into dbo.[AppPermissions] (RoleId, UserId, Name, [Group], Access, ClaimType, ClaimValue, Description, CreatedBy, CreatedOn,
                LastModifiedBy, LastModifiedOn)
                OUTPUT INSERTED.Id
                values (@RoleId, @UserId, @Name, @Group, @Access, @ClaimType, @ClaimValue, @Description, @CreatedBy, @CreatedOn, @LastModifiedBy,
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
                where Description LIKE '%' + @SearchTerm + '%'
                    OR RoleId LIKE '%' + @SearchTerm + '%'
                    OR UserId LIKE '%' + @SearchTerm + '%'
                    OR ClaimValue LIKE '%' + @SearchTerm + '%';
            end"
    };
    
    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppPermissions_Update]
                @Id UNIQUEIDENTIFIER,
                @RoleId UNIQUEIDENTIFIER = null,
                @UserId UNIQUEIDENTIFIER = null,
                @ClaimType NVARCHAR(256) = null,
                @ClaimValue NVARCHAR(1024) = null,
                @Name NVARCHAR(256) = null,
                @Group NVARCHAR(256) = null,
                @Access NVARCHAR(256) = null,
                @Description NVARCHAR(4000) = null,
                @CreatedBy UNIQUEIDENTIFIER = null,
                @CreatedOn datetime2 = null,
                @LastModifiedBy UNIQUEIDENTIFIER = null,
                @LastModifiedOn datetime2 = null
            AS
            begin
                update dbo.[AppPermissions]
                set RoleId = COALESCE(@RoleId, RoleId), UserID = COALESCE(@UserId, UserId), ClaimType = COALESCE(@ClaimType, ClaimType),
                    ClaimValue = COALESCE(@ClaimValue, ClaimValue), Name = COALESCE(@Name, Name), [Group] = COALESCE(@Group, [Group]),
                    Access = COALESCE(@Access, Access), Description = COALESCE(@Description, Description),
                    CreatedBy = COALESCE(@CreatedBy, CreatedBy), CreatedOn = COALESCE(@CreatedOn, CreatedOn),
                    LastModifiedBy = COALESCE(@LastModifiedBy, LastModifiedBy), LastModifiedOn = COALESCE(@LastModifiedOn, LastModifiedOn)
                where Id = COALESCE(@Id, Id);
            end"
    };
}