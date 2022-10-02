IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[Roles]'))
begin
    CREATE TABLE [dbo].[Roles](
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY IDENTITY,
        [Description] NVARCHAR(4096) NOT NULL,
        [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [LastModifiedBy] UNIQUEIDENTIFIER NULL,
        [LastModifiedOn] datetime2 NULL
    )
end
