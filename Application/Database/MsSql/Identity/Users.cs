using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Identity;

public class Users : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(Users).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        EnforcementOrder = 1,
        TableName = "Users",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[Users]'))
            begin
                CREATE TABLE [dbo].[Users](
                    [Id] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
                    [Username] NVARCHAR(256) NOT NULL,
                    [NormalizedUserName] NVARCHAR(256) NOT NULL,
                    [Email] NVARCHAR(256) NOT NULL,
                    [NormalizedEmail] NVARCHAR(256) NOT NULL,
                    [EmailConfirmed] BIT NOT NULL,
                    [PasswordHash] VARBINARY(MAX) NOT NULL,
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
                CREATE INDEX [IX_User_NormalizedUserName] ON [dbo].[Users] ([NormalizedUserName])
                CREATE INDEX [IX_User_NormalizedEmail] ON [dbo].[Users] ([NormalizedEmail])
            end"
    };
    
    public static readonly MsSqlStoredProcedure Delete = new()
    {
        Table = Table,
        Action = "Delete",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spUsers_Delete]
                @Id UNIQUEIDENTIFIER
            AS
            begin
            --     archive instead in production
                delete
                from dbo.[Users]
                where Id = @Id;
            end"
    };

    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spUsers_GetAll]
            AS
            begin
                select *
                from dbo.[Users];
            end"
    };

    public static readonly MsSqlStoredProcedure GetByEmail = new()
    {
        Table = Table,
        Action = "GetByEmail",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spUsers_GetByEmail]
                @Email NVARCHAR(256)
            AS
            begin
                select *
                from dbo.[Users]
                where NormalizedEmail = @Email;
            end"
    };

    public static readonly MsSqlStoredProcedure GetById = new()
    {
        Table = Table,
        Action = "GetById",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spUsers_GetById]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                select *
                from dbo.[Users]
                where Id = @Id;
            end"
    };

    public static readonly MsSqlStoredProcedure GetByUsername = new()
    {
        Table = Table,
        Action = "GetByUsername",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spUsers_GetByUsername]
                @Username NVARCHAR(256)
            AS
            begin
                select *
                from dbo.[Users]
                where Username = @Username;
            end"
    };

    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spUsers_Insert]
                @Username NVARCHAR(256),
                @NormalizedUserName NVARCHAR(256),
                @Email NVARCHAR(256),
                @NormalizedEmail NVARCHAR(256),
                @EmailConfirmed BIT,
                @PasswordHash VARBINARY(MAX),
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
                insert into dbo.[Users] (Username, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, PasswordSalt,
                                         PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, FirstName, LastName, CreatedBy,
                                         ProfilePictureDataUrl, CreatedOn, LastModifiedBy, LastModifiedOn, IsDeleted, DeletedOn,
                                         IsActive, RefreshToken, RefreshTokenExpiryTime, AccountType)
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
            CREATE OR ALTER PROCEDURE [dbo].[spUsers_Search]
                @SearchTerm NVARCHAR(256)
            AS
            begin
                set nocount on;
                
                select *
                from dbo.[Users]
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
            CREATE OR ALTER PROCEDURE [dbo].[spUsers_Update]
                @Id UNIQUEIDENTIFIER,
                @Username NVARCHAR(256),
                @NormalizedUserName NVARCHAR(256),
                @Email NVARCHAR(256),
                @NormalizedEmail NVARCHAR(256),
                @EmailConfirmed BIT,
                @PasswordHash VARBINARY(MAX),
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
                update dbo.[Users]
                set Username = @Username, NormalizedUserName = @NormalizedUserName, Email = @Email, NormalizedEmail = @NormalizedEmail,
                    EmailConfirmed = @EmailConfirmed, PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt,
                    PhoneNumber = @PhoneNumber, PhoneNumberConfirmed = @PhoneNumberConfirmed, TwoFactorEnabled = @TwoFactorEnabled,
                    FirstName = @FirstName, LastName = @LastName, CreatedBy = @CreatedBy, ProfilePictureDataUrl = @ProfilePictureDataUrl,
                    CreatedOn = @CreatedOn, LastModifiedBy = @LastModifiedBy, LastModifiedOn = @LastModifiedOn, IsDeleted = @IsDeleted,
                    DeletedOn = @DeletedOn, IsActive = @IsActive, RefreshToken = @RefreshToken, RefreshTokenExpiryTime = @RefreshTokenExpiryTime,
                    AccountType = @AccountType
                where Id = @Id;
            end"
    };
}