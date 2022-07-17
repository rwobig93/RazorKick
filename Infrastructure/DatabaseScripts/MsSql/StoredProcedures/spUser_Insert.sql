CREATE OR ALTER PROCEDURE [dbo].[spUser_Insert]
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
    insert into dbo.[Users] (Username, Email, FirstName, LastName, CreatedBy, ProfilePictureDataUrl, CreatedOn, LastModifiedBy, LastModifiedOn,
                             IsDeleted, DeletedOn, IsActive, RefreshToken, RefreshTokenExpiryTime)
    values (@Username, @Email, @FirstName, @LastName, @CreatedBy, @ProfilePictureDataUrl, @CreatedOn, @LastModifiedBy, @LastModifiedOn, @IsDeleted,
            @DeletedOn, @IsActive, @RefreshToken, @RefreshTokenExpiryTime);
end
