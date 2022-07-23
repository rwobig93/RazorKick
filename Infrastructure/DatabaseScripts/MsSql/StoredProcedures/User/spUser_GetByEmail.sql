CREATE OR ALTER PROCEDURE [dbo].[spUser_GetByEmail]
@Email NVARCHAR(256)
AS
begin
    select *
    from dbo.[Users]
    where NormalizedEmail = @Email;
end
