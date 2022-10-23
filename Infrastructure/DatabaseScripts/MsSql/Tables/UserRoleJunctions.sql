IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[User_Role_Junctions]'))
begin
    CREATE TABLE [dbo].[User_Role_Junctions](
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [RoleId] UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT User_Role_PK PRIMARY KEY (UserId, RoleId),
        CONSTRAINT FK_User
            FOREIGN KEY (UserId) REFERENCES dbo.[Users] (Id),
        CONSTRAINT FK_Role
            FOREIGN KEY (RoleId) REFERENCES dbo.[Roles] (Id)
    )
end
