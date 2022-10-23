CREATE OR ALTER PROCEDURE [dbo].[spJunction_UR_Search]
    @SearchTerm NVARCHAR(256)
AS
begin
    set nocount on;
    
    select *
    from dbo.[User_Role_Junctions]
    where UserId LIKE '%' + @SearchTerm + '%'
        OR RoleId LIKE '%' + @SearchTerm + '%';
end
