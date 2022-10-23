CREATE OR ALTER PROCEDURE [dbo].[spJunction_UR_GetRolesOfUser]
@UserId UNIQUEIDENTIFIER
AS
begin
    select RoleId
    from dbo.[User_Role_Junctions]
    where UserId = @UserId;
end
