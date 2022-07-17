CREATE OR ALTER PROCEDURE [dbo].[spExampleUser_Update]
    @Id int,
    @FirstName nvarchar(50),
    @LastName nvarchar(50)
AS
begin
    update dbo.[ExampleUser]
    set FirstName = @FirstName, LastName = @LastName
    where Id = @Id;
end
