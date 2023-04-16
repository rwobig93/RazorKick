using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Identity;

public class AppUserPreferencesMsSql : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(AppUserPreferencesMsSql).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        EnforcementOrder = 3,
        TableName = "AppUserPreferences",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[AppUserPreferences]'))
            begin
                CREATE TABLE [dbo].[AppUserPreferences](
                    [Id] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
                    [OwnerId] UNIQUEIDENTIFIER NULL,
                    [ThemePreference] INT NULL,
                    [DrawerDefaultOpen] BIT NULL,
                    [CustomThemeOne] NVARCHAR(1024) NULL,
                    [CustomThemeTwo] NVARCHAR(1024) NULL,
                    [CustomThemeThree] NVARCHAR(1024) NULL
                )
            end"
    };
    
    public static readonly MsSqlStoredProcedure Delete = new()
    {
        Table = Table,
        Action = "Delete",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserPreferences_Delete]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                delete
                from dbo.[AppUserPreferences]
                where Id = @Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure DeleteForUser = new()
    {
        Table = Table,
        Action = "DeleteForUser",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserPreferences_DeleteForUser]
                @OwnerId UNIQUEIDENTIFIER
            AS
            begin
                delete
                from dbo.[AppUserPreferences]
                where OwnerId = @OwnerId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserPreferences_GetAll]
            AS
            begin
                select *
                from dbo.[AppUserPreferences];
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetById = new()
    {
        Table = Table,
        Action = "GetById",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserPreferences_GetById]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                SELECT TOP 1 *
                from dbo.[AppUserPreferences]
                where Id = @Id
                ORDER BY Id;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetByOwnerId = new()
    {
        Table = Table,
        Action = "GetByOwnerId",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserPreferences_GetByOwnerId]
                @OwnerId UNIQUEIDENTIFIER
            AS
            begin
                select *
                from dbo.[AppUserPreferences]
                where OwnerId = @OwnerId;
            end"
    };

    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserPreferences_Insert]
                @OwnerId UNIQUEIDENTIFIER,
                @ThemePreference INT,
                @DrawerDefaultOpen BIT,
                @CustomThemeOne NVARCHAR(1024),
                @CustomThemeTwo NVARCHAR(1024),
                @CustomThemeThree NVARCHAR(1024)
            AS
            begin
                insert into dbo.[AppUserPreferences] (OwnerId, ThemePreference, DrawerDefaultOpen, CustomThemeOne, CustomThemeTwo, CustomThemeThree)
                OUTPUT INSERTED.Id
                values (@OwnerId, @ThemePreference, @DrawerDefaultOpen, @CustomThemeOne, @CustomThemeTwo, @CustomThemeThree);
            end"
    };

    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUserPreferences_Update]
                @Id UNIQUEIDENTIFIER,
                @OwnerId UNIQUEIDENTIFIER = null,
                @ThemePreference INT = null,
                @DrawerDefaultOpen BIT = null,
                @CustomThemeOne NVARCHAR(1024) = null,
                @CustomThemeTwo NVARCHAR(1024) = null,
                @CustomThemeThree NVARCHAR(1024) = null
            AS
            begin
                update dbo.[AppUserPreferences]
                set OwnerId = COALESCE(@OwnerId, OwnerId), ThemePreference = COALESCE(@ThemePreference, ThemePreference),
                    DrawerDefaultOpen = COALESCE(@DrawerDefaultOpen, DrawerDefaultOpen), CustomThemeOne = COALESCE(@CustomThemeOne, CustomThemeOne),
                    CustomThemeTwo = COALESCE(@CustomThemeTwo, CustomThemeTwo), CustomThemeThree = COALESCE(@CustomThemeThree, CustomThemeThree)
                where Id = COALESCE(@Id, Id);
            end"
    };
}