CREATE OR ALTER PROCEDURE [dbo].[spUser_Get]
@Id UNIQUEIDENTIFIER
AS
begin
    select Id, FirstName, LastName, CreatedBy, ProfilePictureDataUrl, CreatedOn, LastModifiedBy, LastModifiedOn, IsDeleted,
           DeletedOn, IsActive, RefreshToken, RefreshTokenExpiryTime
    from dbo.[Users]
    where Id = @Id;
end
