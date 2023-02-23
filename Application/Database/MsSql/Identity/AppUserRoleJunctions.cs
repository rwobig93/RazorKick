using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Identity;

public class AppUserRoleJunctions : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(AppUserRoleJunctions).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        EnforcementOrder = 3,
        TableName = "AppUserRoleJunctions",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[AppUserRoleJunctions]'))
            begin
                CREATE TABLE [dbo].[AppUserRoleJunctions](
                    [UserId] UNIQUEIDENTIFIER NOT NULL,
                    [RoleId] UNIQUEIDENTIFIER NOT NULL,
                    CONSTRAINT User_Role_PK PRIMARY KEY (UserId, RoleId),
                    CONSTRAINT FK_User
                        FOREIGN KEY (UserId) REFERENCES dbo.[AppUsers] (Id),
                    CONSTRAINT FK_Role
                        FOREIGN KEY (RoleId) REFERENCES dbo.[AppRoles] (Id)
                )
            end"
    };
    
    public static readonly MsSqlStoredProcedure Delete = new()
    {
        Table = Table,
        Action = "Delete",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserRoleJunctions_Delete]
                @UserId UNIQUEIDENTIFIER,
                @RoleId UNIQUEIDENTIFIER
            AS
            begin
            --     archive instead in production
                delete
                from dbo.[User_Role_Junctions]
                where UserId = @UserId AND
                      RoleId = @RoleId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserRoleJunctions_GetAll]
            AS
            begin
                select *
                from dbo.[User_Role_Junctions];
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByUserRoleId = new()
    {
        Table = Table,
        Action = "GetByUserRoleId",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserRoleJunctions_GetByUserRoleId]
                @UserId UNIQUEIDENTIFIER,
                @RoleId UNIQUEIDENTIFIER
            AS
            begin
                select *
                from dbo.[User_Role_Junctions]
                where UserId = @UserId AND
                      RoleId = @RoleId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetRolesOfUser = new()
    {
        Table = Table,
        Action = "GetRolesOfUser",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserRoleJunctions_GetRolesOfUser]
                @UserId UNIQUEIDENTIFIER
            AS
            begin
                select RoleId
                from dbo.[User_Role_Junctions]
                where UserId = @UserId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetUsersOfRole = new()
    {
        Table = Table,
        Action = "GetUsersOfRole",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserRoleJunctions_GetUsersOfRole]
                @RoleId UNIQUEIDENTIFIER
            AS
            begin
                select UserId
                from dbo.[User_Role_Junctions]
                where RoleId = @RoleId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserRoleJunctions_Insert]
                @UserId UNIQUEIDENTIFIER,
                @RoleId UNIQUEIDENTIFIER
            AS
            begin
                insert into dbo.[User_Role_Junctions] (UserId, RoleId)
                values (@UserId, @RoleId)
                select Id = @@IDENTITY;
            end"
    };
    
    public static readonly MsSqlStoredProcedure Search = new()
    {
        Table = Table,
        Action = "Search",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserRoleJunctions_Search]
                @SearchTerm NVARCHAR(256)
            AS
            begin
                set nocount on;
                
                select *
                from dbo.[User_Role_Junctions]
                where UserId LIKE '%' + @SearchTerm + '%'
                    OR RoleId LIKE '%' + @SearchTerm + '%';
            end"
    };
}