using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Example;

public class ExampleObjectPermissionJunctions : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(ExampleObjectPermissionJunctions).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        TableName = "ExampleObjectPermissionJunctions",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[ExampleObjectPermissionJunctions]'))
            begin
                CREATE TABLE [dbo].[ExampleObjectPermissionJunctions](
                    [ExampleObjectId] UNIQUEIDENTIFIER NOT NULL,
                    [ExamplePermissionId] UNIQUEIDENTIFIER NOT NULL,
                    CONSTRAINT ExampleObjectPermission_PK PRIMARY KEY (ExampleObjectId, ExamplePermissionId),
                    CONSTRAINT FK_ExampleObject
                        FOREIGN KEY (ExampleObjectId) REFERENCES dbo.[ExampleObjects] (Id),
                    CONSTRAINT FK_ExamplePermission
                        FOREIGN KEY (ExamplePermissionId) REFERENCES dbo.[ExamplePermissions] (Id)
                )
            end"
    };
    
    public static readonly MsSqlStoredProcedure Delete = new()
    {
        Table = Table,
        Action = "Delete",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleObjectPermissionJunctions_Delete]
                @ExampleObjectId UNIQUEIDENTIFIER,
                @ExamplePermissionId UNIQUEIDENTIFIER
            AS
            begin
            --     archive instead in production
                delete
                from dbo.[ExampleObjectPermissionJunctions]
                where ExampleObjectId = @ExampleObjectId AND
                      ExamplePermissionId = @ExamplePermissionId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleObjectPermissionJunctions_GetAll]
            AS
            begin
                select ExampleObjectId, ExamplePermissionId
                from dbo.[ExampleObjectPermissionJunctions];
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetPermissionsForObject = new()
    {
        Table = Table,
        Action = "GetPermissionsForObject",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleObjectPermissionJunctions_GetPermissionsForObject]
                @ExampleObjectId UNIQUEIDENTIFIER
            AS
            begin
                select ExamplePermissionId
                from dbo.[ExampleObjectPermissionJunctions]
                where ExampleObjectId = @ExampleObjectId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetObjectsWithPermission = new()
    {
        Table = Table,
        Action = "GetObjectsWithPermission",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleObjectPermissionJunctions_GetObjectsWithPermission]
                @ExamplePermissionId UNIQUEIDENTIFIER
            AS
            begin
                select ExampleObjectId
                from dbo.[ExampleObjectPermissionJunctions]
                where ExamplePermissionId = @ExamplePermissionId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleObjectPermissionJunctions_Insert]
                @ExampleObjectId UNIQUEIDENTIFIER,
                @ExamplePermissionId UNIQUEIDENTIFIER
            AS
            begin
                insert into dbo.[ExampleObjectPermissionJunctions] (ExampleObjectId, ExamplePermissionId)
                values (@ExampleObjectId, @ExamplePermissionId);
            end"
    };
    
    public static readonly MsSqlStoredProcedure Search = new()
    {
        Table = Table,
        Action = "Search",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleObjectPermissionJunctions_Search]
                @SearchTerm NVARCHAR(256)
            AS
            begin
                set nocount on;
                
                select ExampleObjectId, ExamplePermissionId
                from dbo.[ExampleObjectPermissionJunctions]
                where ExampleObjectId LIKE '%' + @SearchTerm + '%'
                    OR ExamplePermissionId LIKE '%' + @SearchTerm + '%';
            end"
    };
}