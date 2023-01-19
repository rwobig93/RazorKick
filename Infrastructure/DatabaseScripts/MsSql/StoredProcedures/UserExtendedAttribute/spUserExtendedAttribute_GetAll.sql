CREATE OR ALTER PROCEDURE [dbo].[spUserExtendedAttribute_GetAll]
AS
begin
    select *
    from dbo.[UserExtendedAttributes];
end
