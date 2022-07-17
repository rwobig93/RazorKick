CREATE OR ALTER PROCEDURE [dbo].[spUser_Delete]
@Id UNIQUEIDENTIFIER
AS
begin
--     archive instead in production
    delete
    from dbo.[Users]
    where Id = @Id;
end
