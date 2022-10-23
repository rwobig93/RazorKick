CREATE OR ALTER PROCEDURE [dbo].[spRole_Update]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(256),
    @Description NVARCHAR(4096),
    @CreatedBy UNIQUEIDENTIFIER,
    @CreatedOn datetime2,
    @LastModifiedBy UNIQUEIDENTIFIER,
    @LastModifiedOn datetime2
AS
begin
    update dbo.[Roles]
    set Name = @Name, Description = @Description, CreatedBy = @CreatedBy, CreatedOn = @CreatedOn,
        LastModifiedBy = @LastModifiedBy, LastModifiedOn = @LastModifiedOn
    where Id = @Id;
end
