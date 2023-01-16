CREATE OR ALTER PROCEDURE [dbo].[spJunction_UR_GetRolesOfUser]
@UserId UNIQUEIDENTIFIER
AS
begin
    select FK_Role
    from dbo.[User_Role_Junctions]
    where UserId = @UserId;
end
