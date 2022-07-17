CREATE OR ALTER PROCEDURE [dbo].[spUser_GetByUsername]
@Username NVARCHAR(256)
AS
begin
    select Id, Username, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, PasswordSalt,
           PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, FirstName, LastName, CreatedBy,
           ProfilePictureDataUrl, CreatedOn, LastModifiedBy, LastModifiedOn, IsDeleted, DeletedOn,
           IsActive, RefreshToken, RefreshTokenExpiryTime
    from dbo.[Users]
    where Username = @Username;
end
