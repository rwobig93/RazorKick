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
                    [PasswordSalt] VARBINARY(MAX) NOT NULL,
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
            --     archive instead in production
                delete
                from dbo.[AppUsers]
                where Id = @Id;
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
                from dbo.[AppUsers];
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
                where Email = @Email;
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
                where NormalizedEmail = @NormalizedEmail;
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
                where Id = @Id;
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
                where Username = @Username;
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
                where NormalizedUsername = @NormalizedUsername;
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
                @PasswordSalt VARBINARY(MAX),
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
                values (@Username, @NormalizedUserName, @Email, @NormalizedEmail, @EmailConfirmed, @PasswordHash, @PasswordSalt,
                        @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled, @FirstName, @LastName, @CreatedBy,
                        @ProfilePictureDataUrl, @CreatedOn, @LastModifiedBy, @LastModifiedOn, @IsDeleted, @DeletedOn, @IsActive,
                        @RefreshToken, @RefreshTokenExpiryTime, @AccountType)
                select Id = @@IDENTITY;
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
                    OR Email LIKE '%' + @SearchTerm + '%';
            end"
    };

    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spAppUsers_Update]
                @Id UNIQUEIDENTIFIER,
                @Username NVARCHAR(256),
                @NormalizedUserName NVARCHAR(256),
                @Email NVARCHAR(256),
                @NormalizedEmail NVARCHAR(256),
                @EmailConfirmed BIT,
                @PasswordHash NVARCHAR(256),
                @PasswordSalt VARBINARY(MAX),
                @PhoneNumber NVARCHAR(256),
                @PhoneNumberConfirmed BIT,
                @TwoFactorEnabled BIT,
                @FirstName NVARCHAR(256),
                @LastName NVARCHAR(256),
                @ProfilePictureDataUrl NVARCHAR(400),
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
                update dbo.[AppUsers]
                set Username = @Username, NormalizedUserName = @NormalizedUserName, Email = @Email, NormalizedEmail = @NormalizedEmail,
                    EmailConfirmed = @EmailConfirmed, PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt,
                    PhoneNumber = @PhoneNumber, PhoneNumberConfirmed = @PhoneNumberConfirmed, TwoFactorEnabled = @TwoFactorEnabled,
                    FirstName = @FirstName, LastName = @LastName, ProfilePictureDataUrl = @ProfilePictureDataUrl,
                    LastModifiedBy = @LastModifiedBy, LastModifiedOn = @LastModifiedOn, IsDeleted = @IsDeleted,
                    DeletedOn = @DeletedOn, IsActive = @IsActive, RefreshToken = @RefreshToken, RefreshTokenExpiryTime = @RefreshTokenExpiryTime,
                    AccountType = @AccountType
                where Id = @Id;
            end"
    };
}