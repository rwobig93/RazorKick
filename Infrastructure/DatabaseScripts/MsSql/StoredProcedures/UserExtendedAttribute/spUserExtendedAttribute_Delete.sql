CREATE OR ALTER PROCEDURE [dbo].[spUserExtendedAttribute_Delete]
    @Id UNIQUEIDENTIFIER
AS
begin
--     archive instead in production
    delete
    from dbo.[UserExtendedAttributes]
    where Id = @Id;
end
