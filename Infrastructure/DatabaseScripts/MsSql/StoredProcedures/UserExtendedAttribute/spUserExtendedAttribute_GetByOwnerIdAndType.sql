CREATE OR ALTER PROCEDURE [dbo].[spUserExtendedAttribute_GetByOwnerIdAndType]
    @Type int,
    @OwnerId UNIQUEIDENTIFIER
AS
begin
    select *
    from dbo.[UserExtendedAttributes]
    where OwnerId = @OwnerId AND
          Type = @Type;
end
