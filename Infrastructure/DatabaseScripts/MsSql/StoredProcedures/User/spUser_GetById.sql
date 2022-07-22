CREATE OR ALTER PROCEDURE [dbo].[spUser_GetById]
@Id UNIQUEIDENTIFIER
AS
begin
    select *
    from dbo.[Users]
    where Id = @Id;
end
