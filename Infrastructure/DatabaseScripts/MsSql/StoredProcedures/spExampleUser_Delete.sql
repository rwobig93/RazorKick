CREATE OR ALTER PROCEDURE [dbo].[spExampleUser_Delete]
@Id int
AS
begin
--     archive instead in production
    delete
    from dbo.[ExampleUser]
    where Id = @Id;
end
