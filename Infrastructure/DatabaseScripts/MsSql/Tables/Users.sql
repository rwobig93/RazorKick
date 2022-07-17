IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[Users]'))
begin
    CREATE TABLE [dbo].[Users](
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY IDENTITY,
        [Username] NVARCHAR(256) NOT NULL,
        [NormalizedUserName] NVARCHAR(256) NOT NULL,
        [Email] NVARCHAR(256) NOT NULL,
        [NormalizedEmail] NVARCHAR(256) NOT NULL,
        [EmailConfirmed] BIT NOT NULL,
        [PasswordHash] VARBINARY(MAX) NOT NULL,
        [PasswordSalt] VARBINARY(MAX) NOT NULL,
        [PhoneNumber] NVARCHAR(50) NULL,
        [PhoneNumberConfirmed] BIT NOT NULL,
        [TwoFactorEnabled] BIT NOT NULL,
        [FirstName] NVARCHAR(256) NULL,
        [LastName] NVARCHAR(256) NULL,
        [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
        [ProfilePictureDataUrl] NVARCHAR(400) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [LastModifiedBy] UNIQUEIDENTIFIER NULL,
        [LastModifiedOn] datetime2 NULL,
        [IsDeleted] BIT NOT NULL,
        [DeletedOn] datetime2 NULL,
        [IsActive] BIT NOT NULL,
        [RefreshToken] NVARCHAR(400) NULL,
        [RefreshTokenExpiryTime] datetime2 NULL
    )
    CREATE INDEX [IX_User_NormalizedUserName] ON [dbo].[Users] ([NormalizedUserName])
    CREATE INDEX [IX_User_NormalizedEmail] ON [dbo].[Users] ([NormalizedEmail])
end
