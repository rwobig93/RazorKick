CREATE OR ALTER PROCEDURE [dbo].[spUser_Update]
    @Id UNIQUEIDENTIFIER,
    @Username NVARCHAR(75),
    @Email NVARCHAR(75),
    @FirstName NVARCHAR(75),
    @LastName NVARCHAR(75),
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
    set Username = @Username, Email = @Email, FirstName = @FirstName, LastName = @LastName, CreatedBy = @CreatedBy,
        ProfilePictureDataUrl = @ProfilePictureDataUrl, CreatedOn = @CreatedOn, LastModifiedBy = @LastModifiedBy,
        LastModifiedOn = @LastModifiedOn, IsDeleted = @IsDeleted, DeletedOn = @DeletedOn, IsActive = @IsActive,
        RefreshToken = @RefreshToken, RefreshTokenExpiryTime = @RefreshTokenExpiryTime
    where Id = @Id;
end
