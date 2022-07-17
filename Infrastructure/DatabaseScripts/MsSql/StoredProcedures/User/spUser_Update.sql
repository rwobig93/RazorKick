CREATE OR ALTER PROCEDURE [dbo].[spUser_Update]
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
    @RefreshTokenExpiryTime datetime2
AS
begin
    update dbo.[Users]
    set Username = @Username, NormalizedUserName = @NormalizedUserName, Email = @Email, NormalizedEmail = @NormalizedEmail,
        EmailConfirmed = @EmailConfirmed, PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt,
        PhoneNumber = @PhoneNumber, PhoneNumberConfirmed = @PhoneNumberConfirmed, TwoFactorEnabled = @TwoFactorEnabled,
        FirstName = @FirstName, LastName = @LastName, CreatedBy = @CreatedBy, ProfilePictureDataUrl = @ProfilePictureDataUrl,
        CreatedOn = @CreatedOn, LastModifiedBy = @LastModifiedBy, LastModifiedOn = @LastModifiedOn, IsDeleted = @IsDeleted,
        DeletedOn = @DeletedOn, IsActive = @IsActive, RefreshToken = @RefreshToken, RefreshTokenExpiryTime = @RefreshTokenExpiryTime
    where Id = @Id;
end
