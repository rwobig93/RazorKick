CREATE OR ALTER PROCEDURE [dbo].[spRole_Insert]
    @Name NVARCHAR(256),
    @Description NVARCHAR(4096),
    @CreatedBy UNIQUEIDENTIFIER,
    @CreatedOn datetime2,
    @LastModifiedBy UNIQUEIDENTIFIER,
    @LastModifiedOn datetime2
AS
begin
    insert into dbo.[Roles] (Name, Description, CreatedBy, CreatedOn, LastModifiedBy, LastModifiedOn)
    values (@Name, @Description, @CreatedBy, @CreatedOn, @LastModifiedBy, @LastModifiedOn);
end
