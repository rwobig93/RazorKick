CREATE OR ALTER PROCEDURE [dbo].[spExampleUser_Insert]
    @FirstName nvarchar(50),
    @LastName nvarchar(50)
AS
begin
    insert into dbo.[ExampleUser] (FirstName, LastName)
    values (@FirstName, @LastName);
end
