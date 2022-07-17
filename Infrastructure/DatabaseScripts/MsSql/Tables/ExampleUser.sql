IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[ExampleUser]'))
begin
    CREATE TABLE [dbo].[ExampleUser](
        [Id] INT NOT NULL PRIMARY KEY IDENTITY,
        [FirstName] NVARCHAR(50) NOT NULL,
        [LastName] NVARCHAR(50) NOT NULL
    )
end
