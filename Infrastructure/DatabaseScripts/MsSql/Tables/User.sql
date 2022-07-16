IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[User]'))
begin
    CREATE TABLE [dbo].[User](
        [Id] INT NOT NULL PRIMARY KEY IDENTITY,
        [FirstName] NVARCHAR(50) NOT NULL,
        [LastName] NVARCHAR(50) NOT NULL
    )
end
