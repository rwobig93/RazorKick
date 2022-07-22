CREATE OR ALTER PROCEDURE [dbo].[spUser_Insert]
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
end
