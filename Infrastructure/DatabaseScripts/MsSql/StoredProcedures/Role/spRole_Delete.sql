CREATE OR ALTER PROCEDURE [dbo].[spRole_Delete]
@Id UNIQUEIDENTIFIER
AS
begin
--     archive instead in production
    delete
    from dbo.[Roles]
    where Id = @Id;
end
