CREATE OR ALTER PROCEDURE [dbo].[spRole_GetAll]
AS
begin
    select *
    from dbo.[Roles];
end
