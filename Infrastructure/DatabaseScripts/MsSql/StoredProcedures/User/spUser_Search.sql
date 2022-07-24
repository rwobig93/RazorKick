CREATE OR ALTER PROCEDURE [dbo].[spUser_Search]
    @SearchTerm NVARCHAR(256)
AS
begin
    set nocount on;
    
    select *
    from dbo.[Users]
    where FirstName LIKE '%' + @SearchTerm + '%'
        OR LastName LIKE '%' + @SearchTerm + '%'
        OR Email LIKE '%' + @SearchTerm + '%';
end
