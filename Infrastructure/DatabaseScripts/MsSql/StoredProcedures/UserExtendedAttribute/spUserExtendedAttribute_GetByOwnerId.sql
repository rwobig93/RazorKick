CREATE OR ALTER PROCEDURE [dbo].[spUserExtendedAttribute_GetByOwnerId]
    @OwnerId UNIQUEIDENTIFIER
AS
begin
    select *
    from dbo.[UserExtendedAttributes]
    where OwnerId = @OwnerId;
end
