CREATE OR ALTER PROCEDURE [dbo].[spUser_GetAll]
AS
begin
    select Id, FirstName, LastName, CreatedBy, ProfilePictureDataUrl, CreatedOn, LastModifiedBy, LastModifiedOn, IsDeleted,
           DeletedOn, IsActive, RefreshToken, RefreshTokenExpiryTime
    from dbo.[Users];
end
