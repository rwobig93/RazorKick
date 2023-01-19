IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[UserExtendedAttributes]'))
begin
    CREATE TABLE [dbo].[UserExtendedAttributes](
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY IDENTITY,
        [OwnerId] UNIQUEIDENTIFIER NOT NULL,
        [Name] NVARCHAR(256) NOT NULL,
        [Value] NVARCHAR(256) NOT NULL,
        [Completed] BIT NOT NULL,
        [Type] int NOT NULL,
        [Added] datetime2 NOT NULL,
        [Updated] datetime2 NOT NULL,
        [PreviousValues] NVARCHAR(2048) NOT NULL
    )
    CREATE INDEX [IX_UserExtendedAttribute_OwnerId] ON [dbo].[UserExtendedAttributes] ([OwnerId])
    CREATE INDEX [IX_UserExtendedAttribute_Type] ON [dbo].[UserExtendedAttributes] ([Type])
end
