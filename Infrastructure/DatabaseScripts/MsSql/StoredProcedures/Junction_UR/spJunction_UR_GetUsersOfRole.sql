CREATE OR ALTER PROCEDURE [dbo].[spJunction_UR_GetUsersOfRole]
@RoleId UNIQUEIDENTIFIER
AS
begin
    select UserId
    from dbo.[User_Role_Junctions]
    where RoleId = @RoleId;
end
