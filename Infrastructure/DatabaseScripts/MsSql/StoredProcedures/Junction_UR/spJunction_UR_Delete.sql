CREATE OR ALTER PROCEDURE [dbo].[spJunction_UR_Delete]
@User_Role_PK UNIQUEIDENTIFIER
AS
begin
--     archive instead in production
    delete
    from dbo.[User_Role_Junctions]
    where User_Role_PK = @User_Role_PK;
end
