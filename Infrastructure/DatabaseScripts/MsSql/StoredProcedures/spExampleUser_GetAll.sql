CREATE OR ALTER PROCEDURE [dbo].[spExampleUser_GetAll]
AS
begin
    select Id, FirstName, LastName
    from dbo.[ExampleUser];
end
