CREATE OR ALTER PROCEDURE [dbo].[spUser_Delete]
@Id int
AS
begin
--     archive instead in production
    delete
    from dbo.[User]
    where Id = @Id;
end
