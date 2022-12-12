CREATE OR ALTER PROCEDURE [dbo].[spJunction_UR_Delete]
    @UserId UNIQUEIDENTIFIER,
    @RoleId UNIQUEIDENTIFIER
AS
begin
--     archive instead in production
    delete
    from dbo.[User_Role_Junctions]
    where UserId = @UserId AND
          RoleId = @RoleId;
end
