CREATE OR ALTER PROCEDURE [dbo].[spExampleUser_Get]
@Id int
AS
begin
    select Id, FirstName, LastName
    from dbo.[ExampleUser]
    where Id = @Id;
end
