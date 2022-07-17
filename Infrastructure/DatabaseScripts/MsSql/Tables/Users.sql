IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[Users]'))
begin
    CREATE TABLE [dbo].[Users](
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY IDENTITY,
        [Username] NVARCHAR(75),
        [Email] NVARCHAR(75),
        [FirstName] NVARCHAR(75),
        [LastName] NVARCHAR(75),
        [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
        [ProfilePictureDataUrl] NVARCHAR(400),
        [CreatedOn] datetime2 NOT NULL,
        [LastModifiedBy] UNIQUEIDENTIFIER,
        [LastModifiedOn] datetime2,
        [IsDeleted] BIT NOT NULL,
        [DeletedOn] datetime2,
        [IsActive] BIT NOT NULL,
        [RefreshToken] NVARCHAR(400),
        [RefreshTokenExpiryTime] datetime2,
    )
end
