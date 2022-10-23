CREATE OR ALTER PROCEDURE [dbo].[spJunction_UR_GetAll]
AS
begin
    select *
    from dbo.[User_Role_Junctions];
end
