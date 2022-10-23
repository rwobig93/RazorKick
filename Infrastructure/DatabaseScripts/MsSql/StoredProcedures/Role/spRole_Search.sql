CREATE OR ALTER PROCEDURE [dbo].[spRole_Search]
    @SearchTerm NVARCHAR(256)
AS
begin
    set nocount on;
    
    select *
    from dbo.[Roles]
    where Name LIKE '%' + @SearchTerm + '%'
        OR Description LIKE '%' + @SearchTerm + '%';
end
