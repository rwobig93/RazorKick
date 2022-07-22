CREATE OR ALTER PROCEDURE [dbo].[spUser_GetByUsername]
@Username NVARCHAR(256)
AS
begin
    select *
    from dbo.[Users]
    where Username = @Username;
end
