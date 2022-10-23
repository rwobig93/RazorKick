CREATE OR ALTER PROCEDURE [dbo].[spJunction_UR_Insert]
    @UserId UNIQUEIDENTIFIER,
    @RoleId UNIQUEIDENTIFIER
AS
begin
    insert into dbo.[User_Role_Junctions] (UserId, RoleId)
    values (@UserId, @RoleId);
end
