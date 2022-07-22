CREATE OR ALTER PROCEDURE [dbo].[spUser_GetAll]
AS
begin
    select *
    from dbo.[Users];
end
