CREATE OR ALTER PROCEDURE [dbo].[spUserExtendedAttribute_Insert]
    @OwnerId UNIQUEIDENTIFIER,
    @Name NVARCHAR(256),
    @Value NVARCHAR(256),
    @Type int,
    @Added datetime2,
    @Updated datetime2
AS
begin
    insert into dbo.[UserExtendedAttributes] (UserId, RoleId)
    values (@UserId, @RoleId);
end
