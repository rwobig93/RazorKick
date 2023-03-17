using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Identity;

public class AppUsers : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(AppUsers).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        EnforcementOrder = 1,
        TableName = "AppUsers",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[AppUsers]'))
            begin
                CREATE TABLE [dbo].[AppUsers](
                    [Id] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
                    [Username] NVARCHAR(256) NOT NULL,
                    [NormalizedUserName] NVARCHAR(256) NOT NULL,
                    [Email] NVARCHAR(256) NOT NULL,
                    [NormalizedEmail] NVARCHAR(256) NOT NULL,
                    [EmailConfirmed] BIT NOT NULL,
                    [PasswordHash] NVARCHAR(256) NOT NULL,
                    [PasswordSalt] NVARCHAR(256) NOT NULL,
                    [PhoneNumber] NVARCHAR(50) NULL,
                    [PhoneNumberConfirmed] BIT NOT NULL,
                    [TwoFactorEnabled] BIT NOT NULL,
                    [FirstName] NVARCHAR(256) NULL,
                    [LastName] NVARCHAR(256) NULL,
                    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
                    [ProfilePictureDataUrl] NVARCHAR(400) NULL,
                    [CreatedOn] datetime2 NOT NULL,
                    [LastModifiedBy] UNIQUEIDENTIFIER NULL,
                    [LastModifiedOn] datetime2 NULL,
                    [IsDeleted] BIT NOT NULL,
                    [DeletedOn] datetime2 NULL,
                    [IsActive] BIT NOT NULL,
                    [RefreshToken] NVARCHAR(400) NULL,
                    [RefreshTokenExpiryTime] datetime2 NULL,
                    [AccountType] int NOT NULL
                )
                CREATE INDEX [IX_User_NormalizedUserName] ON [dbo].[AppUsers] ([NormalizedUserName])
                CREATE INDEX [IX_User_NormalizedEmail] ON [dbo].[AppUsers] ([NormalizedEmail])
            end"
    };
    
    public static readonly MsSqlStoredProcedure Delete = new()
    {
        Table = Table,
        Action = "Delete",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUsers_Delete]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                update dbo.[AppUsers]
                set IsDeleted = 1
                where Id = @Id;
            end"
    };

    public static readonly MsSqlStoredProcedure GetAllDeleted = new()
    {
        Table = Table,
        Action = "GetAllDeleted",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUsers_GetAllDeleted]
            AS
            begin
                select *
                from dbo.[AppUsers]
                where IsDeleted = 1;
            end"
    };

    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUsers_GetAll]
            AS
            begin
                select *
                from dbo.[AppUsers]
                where IsDeleted = 0;
            end"
    };

    public static readonly MsSqlStoredProcedure GetByEmail = new()
    {
        Table = Table,
        Action = "GetByEmail",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUsers_GetByEmail]
                @Email NVARCHAR(256)
            AS
            begin
                select *
                from dbo.[AppUsers]
                where Email = @Email AND IsDeleted = 0;
            end"
    };

    public static readonly MsSqlStoredProcedure GetByEmailDeleted = new()
    {
        Table = Table,
        Action = "GetByEmailDeleted",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUsers_GetByEmailDeleted]
                @Email NVARCHAR(256)
            AS
            begin
                select *
                from dbo.[AppUsers]
                where Email = @Email AND IsDeleted = 1;
            end"
    };

    public static readonly MsSqlStoredProcedure GetByNormalizedEmail = new()
    {
        Table = Table,
        Action = "GetByNormalizedEmail",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUsers_GetByNormalizedEmail]
                @NormalizedEmail NVARCHAR(256)
            AS
            begin
                select *
                from dbo.[AppUsers]
                where NormalizedEmail = @NormalizedEmail AND IsDeleted = 0;
            end"
    };

    public static readonly MsSqlStoredProcedure GetById = new()
    {
        Table = Table,
        Action = "GetById",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUsers_GetById]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                select *
                from dbo.[AppUsers]
                where Id = @Id AND IsDeleted = 0;
            end"
    };

    public static readonly MsSqlStoredProcedure GetByUsername = new()
    {
        Table = Table,
        Action = "GetByUsername",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUsers_GetByUsername]
                @Username NVARCHAR(256)
            AS
            begin
                select *
                from dbo.[AppUsers]
                where Username = @Username AND IsDeleted = 0;
            end"
    };

    public static readonly MsSqlStoredProcedure GetByUsernameDeleted = new()
    {
        Table = Table,
        Action = "GetByUsernameDeleted",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUsers_GetByUsernameDeleted]
                @Username NVARCHAR(256)
            AS
            begin
                select *
                from dbo.[AppUsers]
                where Username = @Username AND IsDeleted = 1;
            end"
    };

    public static readonly MsSqlStoredProcedure GetByNormalizedUsername = new()
    {
        Table = Table,
        Action = "GetByNormalizedUsername",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUsers_GetByNormalizedUsername]
                @NormalizedUsername NVARCHAR(256)
            AS
            begin
                select *
                from dbo.[AppUsers]
                where NormalizedUsername = @NormalizedUsername AND IsDeleted = 0;
            end"
    };

    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUsers_Insert]
                @Username NVARCHAR(256),
                @NormalizedUserName NVARCHAR(256),
                @Email NVARCHAR(256),
                @NormalizedEmail NVARCHAR(256),
                @EmailConfirmed BIT,
                @PasswordHash NVARCHAR(256),
                @PasswordSalt NVARCHAR(256),
                @PhoneNumber NVARCHAR(256),
                @PhoneNumberConfirmed BIT,
                @TwoFactorEnabled BIT,
                @FirstName NVARCHAR(256),
                @LastName NVARCHAR(256),
                @CreatedBy UNIQUEIDENTIFIER,
                @ProfilePictureDataUrl NVARCHAR(400),
                @CreatedOn datetime2,
                @LastModifiedBy UNIQUEIDENTIFIER,
                @LastModifiedOn datetime2,
                @IsDeleted BIT,
                @DeletedOn datetime2,
                @IsActive BIT,
                @RefreshToken NVARCHAR(400),
                @RefreshTokenExpiryTime datetime2,
                @AccountType int
            AS
            begin
                insert into dbo.[AppUsers] (Username, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, PasswordSalt,
                                         PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, FirstName, LastName, CreatedBy,
                                         ProfilePictureDataUrl, CreatedOn, LastModifiedBy, LastModifiedOn, IsDeleted, DeletedOn,
                                         IsActive, RefreshToken, RefreshTokenExpiryTime, AccountType)
                OUTPUT INSERTED.Id
                values (@Username, @NormalizedUserName, @Email, @NormalizedEmail, @EmailConfirmed, @PasswordHash, @PasswordSalt,
                        @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled, @FirstName, @LastName, @CreatedBy,
                        @ProfilePictureDataUrl, @CreatedOn, @LastModifiedBy, @LastModifiedOn, @IsDeleted, @DeletedOn, @IsActive,
                        @RefreshToken, @RefreshTokenExpiryTime, @AccountType);
            end"
    };

    public static readonly MsSqlStoredProcedure Search = new()
    {
        Table = Table,
        Action = "Search",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUsers_Search]
                @SearchTerm NVARCHAR(256)
            AS
            begin
                set nocount on;
                
                select *
                from dbo.[AppUsers]
                where FirstName LIKE '%' + @SearchTerm + '%'
                    OR LastName LIKE '%' + @SearchTerm + '%'
                    OR Email LIKE '%' + @SearchTerm + '%'
                AND IsDeleted = 0;
            end"
    };

    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUsers_Update]
                @Id UNIQUEIDENTIFIER,
                @Username NVARCHAR(256) = null,
                @NormalizedUserName NVARCHAR(256) = null,
                @Email NVARCHAR(256) = null,
                @NormalizedEmail NVARCHAR(256) = null,
                @EmailConfirmed BIT = null,
                @PasswordHash NVARCHAR(256) = null,
                @PasswordSalt NVARCHAR(256) = null,
                @PhoneNumber NVARCHAR(256) = null,
                @PhoneNumberConfirmed BIT = null,
                @TwoFactorEnabled BIT = null,
                @FirstName NVARCHAR(256) = null,
                @LastName NVARCHAR(256) = null,
                @ProfilePictureDataUrl NVARCHAR(400) = null,
                @LastModifiedBy UNIQUEIDENTIFIER = null,
                @LastModifiedOn datetime2 = null,
                @IsDeleted BIT = null,
                @DeletedOn datetime2 = null,
                @IsActive BIT = null,
                @RefreshToken NVARCHAR(400) = null,
                @RefreshTokenExpiryTime datetime2 = null,
                @AccountType int = null
            AS
            begin
                update dbo.[AppUsers]
                set Username = COALESCE(@Username, Username), NormalizedUserName = COALESCE(@NormalizedUserName, NormalizedUserName),
                    Email = COALESCE(@Email, Email), NormalizedEmail = COALESCE(@NormalizedEmail, NormalizedEmail),
                    EmailConfirmed = COALESCE(@EmailConfirmed, EmailConfirmed), PasswordHash = COALESCE(@PasswordHash, PasswordHash),
                    PasswordSalt = COALESCE(@PasswordSalt, PasswordSalt), PhoneNumber = COALESCE(@PhoneNumber, PhoneNumber),
                    PhoneNumberConfirmed = COALESCE(@PhoneNumberConfirmed, PhoneNumberConfirmed),
                    TwoFactorEnabled = COALESCE(@TwoFactorEnabled, TwoFactorEnabled), FirstName = COALESCE(@FirstName, FirstName),
                    LastName = COALESCE(@LastName, LastName), ProfilePictureDataUrl = COALESCE(@ProfilePictureDataUrl, ProfilePictureDataUrl),
                    LastModifiedBy = COALESCE(@LastModifiedBy, LastModifiedBy), LastModifiedOn = COALESCE(@LastModifiedOn, LastModifiedOn),
                    IsDeleted = COALESCE(@IsDeleted, IsDeleted), DeletedOn = COALESCE(@DeletedOn, DeletedOn),
                    IsActive = COALESCE(@IsActive, IsActive), RefreshToken = COALESCE(@RefreshToken, RefreshToken),
                    RefreshTokenExpiryTime = COALESCE(@RefreshTokenExpiryTime, RefreshTokenExpiryTime),
                    AccountType = COALESCE(@AccountType, AccountType)
                where Id = COALESCE(@Id, Id);
            end"
    };

    public static readonly MsSqlStoredProcedure SetUserId = new()
    {
        Table = Table,
        Action = "SetUserId",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUsers_SetUserId]
                @CurrentId UNIQUEIDENTIFIER,
                @NewId UNIQUEIDENTIFIER
            AS
            begin
                update dbo.[AppUsers]
                set Id = @NewId
                OUTPUT @NewId
                where Id = @CurrentId;
            end"
    };

    public static readonly MsSqlStoredProcedure SetCreatedById = new()
    {
        Table = Table,
        Action = "SetCreatedById",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUsers_SetCreatedById]
                @Id UNIQUEIDENTIFIER,
                @CreatedBy UNIQUEIDENTIFIER
            AS
            begin
                update dbo.[AppUsers]
                set CreatedBy = @CreatedBy
                where Id = @Id;
            end"
    };
}