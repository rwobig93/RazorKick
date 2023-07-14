using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Identity;

public class AppUsersMsSql : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(AppUsersMsSql).GetDbScriptsFromClass();
    
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
                    [Email] NVARCHAR(256) NOT NULL,
                    [EmailConfirmed] BIT NOT NULL,
                    [PhoneNumber] NVARCHAR(50) NULL,
                    [PhoneNumberConfirmed] BIT NOT NULL,
                    [FirstName] NVARCHAR(256) NULL,
                    [LastName] NVARCHAR(256) NULL,
                    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
                    [ProfilePictureDataUrl] NVARCHAR(400) NULL,
                    [CreatedOn] datetime2 NOT NULL,
                    [LastModifiedBy] UNIQUEIDENTIFIER NULL,
                    [LastModifiedOn] datetime2 NULL,
                    [IsDeleted] BIT NOT NULL,
                    [DeletedOn] datetime2 NULL,
                    [AccountType] int NOT NULL
                )
                CREATE INDEX [IX_User_Id] ON [dbo].[AppUsers] ([Id])
                CREATE INDEX [IX_User_UserName] ON [dbo].[AppUsers] ([Username])
                CREATE INDEX [IX_User_Email] ON [dbo].[AppUsers] ([Email])
            end"
    };
    
    public static readonly MsSqlStoredProcedure Delete = new()
    {
        Table = Table,
        Action = "Delete",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Delete]
                @Id UNIQUEIDENTIFIER,
                @DeletedOn datetime2
            AS
            begin
                UPDATE dbo.[{Table.TableName}]
                SET IsDeleted = 1, DeletedOn = @DeletedOn
                WHERE Id = @Id;
            end"
    };

    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAll]
            AS
            begin
                SELECT u.*, s.PasswordHash, s.PasswordSalt, s.TwoFactorEnabled, s.TwoFactorKey, s.AuthState, s.AuthStateTimestamp,
                        s.RefreshToken, s.RefreshTokenExpiryTime, s.BadPasswordAttempts, s.LastBadPassword
                FROM dbo.[{Table.TableName}] u
                JOIN dbo.[{AppUserSecurityAttributesMsSql.Table.TableName}] s ON u.Id = s.OwnerId
                WHERE u.IsDeleted = 0;
            end"
    };

    public static readonly MsSqlStoredProcedure GetAllPaginated = new()
    {
        Table = Table,
        Action = "GetAllPaginated",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllPaginated]
                @Offset INT,
                @PageSize INT
            AS
            begin
                SELECT u.*, s.PasswordHash, s.PasswordSalt, s.TwoFactorEnabled, s.TwoFactorKey, s.AuthState, s.AuthStateTimestamp,
                        s.RefreshToken, s.RefreshTokenExpiryTime, s.BadPasswordAttempts, s.LastBadPassword
                FROM dbo.[{Table.TableName}] u
                JOIN dbo.[{AppUserSecurityAttributesMsSql.Table.TableName}] s ON u.Id = s.OwnerId
                WHERE u.IsDeleted = 0
                ORDER BY u.Id DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            end"
    };

    public static readonly MsSqlStoredProcedure GetAllDeleted = new()
    {
        Table = Table,
        Action = "GetAllDeleted",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllDeleted]
            AS
            begin
                SELECT u.*
                FROM dbo.[{Table.TableName}] u
                WHERE u.IsDeleted = 1;
            end"
    };

    public static readonly MsSqlStoredProcedure GetAllLockedOut = new()
    {
        Table = Table,
        Action = "GetAllLockedOut",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetAllLockedOut]
            AS
            begin
                SELECT u.*, s.PasswordHash, s.PasswordSalt, s.TwoFactorEnabled, s.TwoFactorKey, s.AuthState, s.AuthStateTimestamp,
                        s.RefreshToken, s.RefreshTokenExpiryTime, s.BadPasswordAttempts, s.LastBadPassword
                FROM dbo.[{Table.TableName}] u
                JOIN dbo.[{AppUserSecurityAttributesMsSql.Table.TableName}] s ON u.Id = s.OwnerId
                WHERE u.IsDeleted = 0 AND s.AuthState = 3;
            end"
    };

    public static readonly MsSqlStoredProcedure GetByEmail = new()
    {
        Table = Table,
        Action = "GetByEmail",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByEmail]
                @Email NVARCHAR(256)
            AS
            begin
                SELECT u.*, s.AuthState as AuthState
                FROM dbo.[{Table.TableName}] u
                JOIN dbo.[{AppUserSecurityAttributesMsSql.Table.TableName}] s ON u.Id = s.OwnerId
                WHERE u.Email = @Email AND u.IsDeleted = 0
                ORDER BY u.Id;
            end"
    };

    public static readonly MsSqlStoredProcedure GetByEmailFull = new()
    {
        Table = Table,
        Action = "GetByEmailFull",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByEmailFull]
                @Email NVARCHAR(256)
            AS
            begin
                SELECT u.*, r.*, s.AuthState as AuthState
                FROM dbo.[{Table.TableName}] u
                JOIN dbo.[{AppUserRoleJunctionsMsSql.Table.TableName}] ur ON u.Id = ur.UserId
                JOIN dbo.[{AppRolesMsSql.Table.TableName}] r ON r.Id = ur.RoleId
                JOIN dbo.[{AppUserSecurityAttributesMsSql.Table.TableName}] s ON u.Id = s.OwnerId
                WHERE u.Email = @Email AND u.IsDeleted = 0
                ORDER BY u.Id;
            end"
    };

    public static readonly MsSqlStoredProcedure GetById = new()
    {
        Table = Table,
        Action = "GetById",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetById]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                SELECT TOP 1 u.*, s.AuthState as AuthState
                FROM dbo.[{Table.TableName}] u
                JOIN dbo.[{AppUserSecurityAttributesMsSql.Table.TableName}] s ON u.Id = s.OwnerId
                WHERE u.Id = @Id AND u.IsDeleted = 0
                ORDER BY u.Id;
            end"
    };

    public static readonly MsSqlStoredProcedure GetByIdSecurity = new()
    {
        Table = Table,
        Action = "GetByIdSecurity",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByIdSecurity]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                SELECT u.*, s.PasswordHash, s.PasswordSalt, s.TwoFactorEnabled, s.TwoFactorKey, s.AuthState, s.AuthStateTimestamp,
                        s.RefreshToken, s.RefreshTokenExpiryTime, s.BadPasswordAttempts, s.LastBadPassword
                FROM dbo.[{Table.TableName}] u
                JOIN dbo.[{AppUserSecurityAttributesMsSql.Table.TableName}] s ON s.OwnerId = u.Id
                WHERE u.Id = @Id AND u.IsDeleted = 0
                ORDER BY u.Id;
            end"
    };

    public static readonly MsSqlStoredProcedure GetByIdFull = new()
    {
        Table = Table,
        Action = "GetByIdFull",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByIdFull]
                @Id UNIQUEIDENTIFIER
            AS
            begin
                SELECT u.*, r.*
                FROM dbo.[{Table.TableName}] u
                JOIN dbo.[{AppUserRoleJunctionsMsSql.Table.TableName}] ur ON u.Id = ur.UserId
                JOIN dbo.[{AppRolesMsSql.Table.TableName}] r ON r.Id = ur.RoleId
                WHERE u.Id = @Id AND u.IsDeleted = 0
                ORDER BY u.Id;
            end"
    };

    public static readonly MsSqlStoredProcedure GetByUsername = new()
    {
        Table = Table,
        Action = "GetByUsername",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByUsername]
                @Username NVARCHAR(256)
            AS
            begin
                SELECT u.*, s.AuthState as AuthState
                FROM dbo.[{Table.TableName}] u
                JOIN dbo.[{AppUserSecurityAttributesMsSql.Table.TableName}] s ON u.Id = s.OwnerId
                WHERE u.Username = @Username AND u.IsDeleted = 0
                ORDER BY u.Id;
            end"
    };

    public static readonly MsSqlStoredProcedure GetByUsernameFull = new()
    {
        Table = Table,
        Action = "GetByUsernameFull",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByUsernameFull]
                @Username NVARCHAR(256)
            AS
            begin
                SELECT u.*, r.*, s.AuthState as AuthState
                FROM dbo.[{Table.TableName}] u
                JOIN dbo.[{AppUserRoleJunctionsMsSql.Table.TableName}] ur ON u.Id = ur.UserId
                JOIN dbo.[{AppRolesMsSql.Table.TableName}] r ON r.Id = ur.RoleId
                JOIN dbo.[{AppUserSecurityAttributesMsSql.Table.TableName}] s ON u.Id = s.OwnerId
                WHERE u.Username = @Username AND u.IsDeleted = 0
                ORDER BY u.Id;
            end"
    };

    public static readonly MsSqlStoredProcedure GetByUsernameSecurity = new()
    {
        Table = Table,
        Action = "GetByUsernameSecurity",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_GetByUsernameSecurity]
                @Username NVARCHAR(256)
            AS
            begin
                SELECT u.*, s.PasswordHash, s.PasswordSalt, s.TwoFactorEnabled, s.TwoFactorKey, s.AuthState, s.AuthStateTimestamp,
                        s.RefreshToken, s.RefreshTokenExpiryTime, s.BadPasswordAttempts, s.LastBadPassword
                FROM dbo.[{Table.TableName}] u
                JOIN dbo.[{AppUserSecurityAttributesMsSql.Table.TableName}] s ON s.OwnerId = u.Id
                WHERE u.Username = @Username AND u.IsDeleted = 0
                ORDER BY u.Id;
            end"
    };

    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Insert]
                @Username NVARCHAR(256),
                @Email NVARCHAR(256),
                @EmailConfirmed BIT,
                @PhoneNumber NVARCHAR(256),
                @PhoneNumberConfirmed BIT,
                @FirstName NVARCHAR(256),
                @LastName NVARCHAR(256),
                @CreatedBy UNIQUEIDENTIFIER,
                @ProfilePictureDataUrl NVARCHAR(400),
                @CreatedOn datetime2,
                @LastModifiedBy UNIQUEIDENTIFIER,
                @LastModifiedOn datetime2,
                @IsDeleted BIT,
                @DeletedOn datetime2,
                @AccountType int
            AS
            begin
                INSERT into dbo.[{Table.TableName}] (Username, Email, EmailConfirmed, PhoneNumber, PhoneNumberConfirmed, FirstName, LastName,
                                         CreatedBy, ProfilePictureDataUrl, CreatedOn, LastModifiedBy, LastModifiedOn, IsDeleted, DeletedOn,
                                         AccountType)
                OUTPUT INSERTED.Id
                VALUES (@Username, @Email, @EmailConfirmed, @PhoneNumber, @PhoneNumberConfirmed, @FirstName, @LastName, @CreatedBy,
                        @ProfilePictureDataUrl, @CreatedOn, @LastModifiedBy, @LastModifiedOn, @IsDeleted, @DeletedOn, @AccountType);
            end"
    };

    public static readonly MsSqlStoredProcedure Search = new()
    {
        Table = Table,
        Action = "Search",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Search]
                @SearchTerm NVARCHAR(256)
            AS
            begin
                SELECT u.*, s.AuthState as AuthState
                FROM dbo.[{Table.TableName}] u
                JOIN dbo.[{AppUserSecurityAttributesMsSql.Table.TableName}] s ON s.OwnerId = u.Id
                WHERE u.FirstName LIKE '%' + @SearchTerm + '%'
                    OR u.LastName LIKE '%' + @SearchTerm + '%'
                    OR u.Email LIKE '%' + @SearchTerm + '%'
                    OR u.Id LIKE '%' + @SearchTerm + '%'
                AND u.IsDeleted = 0
                ORDER BY u.Id;
            end"
    };

    public static readonly MsSqlStoredProcedure SearchPaginated = new()
    {
        Table = Table,
        Action = "SearchPaginated",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_SearchPaginated]
                @SearchTerm NVARCHAR(256),
                @Offset INT,
                @PageSize INT
            AS
            begin
                SELECT u.*, s.AuthState as AuthState
                FROM dbo.[{Table.TableName}] u
                JOIN dbo.[{AppUserSecurityAttributesMsSql.Table.TableName}] s ON s.OwnerId = u.Id
                WHERE u.FirstName LIKE '%' + @SearchTerm + '%'
                    OR u.LastName LIKE '%' + @SearchTerm + '%'
                    OR u.Email LIKE '%' + @SearchTerm + '%'
                    OR u.Id LIKE '%' + @SearchTerm + '%'
                AND u.IsDeleted = 0
                ORDER BY u.Id DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            end"
    };

    public static readonly MsSqlStoredProcedure Update = new()
    {
        Table = Table,
        Action = "Update",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_Update]
                @Id UNIQUEIDENTIFIER,
                @Username NVARCHAR(256) = null,
                @Email NVARCHAR(256) = null,
                @EmailConfirmed BIT = null,
                @PhoneNumber NVARCHAR(256) = null,
                @PhoneNumberConfirmed BIT = null,
                @FirstName NVARCHAR(256) = null,
                @LastName NVARCHAR(256) = null,
                @ProfilePictureDataUrl NVARCHAR(400) = null,
                @LastModifiedBy UNIQUEIDENTIFIER = null,
                @LastModifiedOn datetime2 = null,
                @IsDeleted BIT = null,
                @DeletedOn datetime2 = null,
                @AccountType int = null
            AS
            begin
                UPDATE dbo.[{Table.TableName}]
                SET Username = COALESCE(@Username, Username), Email = COALESCE(@Email, Email),
                    EmailConfirmed = COALESCE(@EmailConfirmed, EmailConfirmed), PhoneNumber = COALESCE(@PhoneNumber, PhoneNumber),
                    PhoneNumberConfirmed = COALESCE(@PhoneNumberConfirmed, PhoneNumberConfirmed), FirstName = COALESCE(@FirstName, FirstName),
                    LastName = COALESCE(@LastName, LastName), ProfilePictureDataUrl = COALESCE(@ProfilePictureDataUrl, ProfilePictureDataUrl),
                    LastModifiedBy = COALESCE(@LastModifiedBy, LastModifiedBy), LastModifiedOn = COALESCE(@LastModifiedOn, LastModifiedOn),
                    IsDeleted = COALESCE(@IsDeleted, IsDeleted), DeletedOn = COALESCE(@DeletedOn, DeletedOn),
                    AccountType = COALESCE(@AccountType, AccountType)
                WHERE Id = COALESCE(@Id, Id);
            end"
    };

    public static readonly MsSqlStoredProcedure SetUserId = new()
    {
        Table = Table,
        Action = "SetUserId",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_SetUserId]
                @CurrentId UNIQUEIDENTIFIER,
                @NewId UNIQUEIDENTIFIER
            AS
            begin
                UPDATE dbo.[{Table.TableName}]
                SET Id = @NewId
                OUTPUT @NewId
                WHERE Id = @CurrentId;
            end"
    };

    public static readonly MsSqlStoredProcedure SetCreatedById = new()
    {
        Table = Table,
        Action = "SetCreatedById",
        SqlStatement = @$"
            CREATE OR ALTER PROCEDURE [dbo].[sp{Table.TableName}_SetCreatedById]
                @Id UNIQUEIDENTIFIER,
                @CreatedBy UNIQUEIDENTIFIER
            AS
            begin
                UPDATE dbo.[{Table.TableName}]
                SET CreatedBy = @CreatedBy
                WHERE Id = @Id;
            end"
    };
}