CREATE OR ALTER PROCEDURE [dbo].[spRole_GetById]
@Id UNIQUEIDENTIFIER
AS
begin
    select *
    from dbo.[Roles]
    where Id = @Id;
end
