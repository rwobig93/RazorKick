CREATE OR ALTER PROCEDURE [dbo].[spJunction_UR_GetByUserRoleId]
@UserId UNIQUEIDENTIFIER,
@RoleId UNIQUEIDENTIFIER
AS
begin
    select *
    from dbo.[User_Role_Junctions]
    where UserId = @UserId AND
          RoleId = @RoleId;
end
