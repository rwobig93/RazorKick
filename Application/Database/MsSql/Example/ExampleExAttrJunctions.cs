using Application.Helpers.Runtime;

namespace Application.Database.MsSql.Example;

public class ExampleExAttrJunctions : ISqlEnforcedEntityMsSql
{
    public IEnumerable<ISqlDatabaseScript> GetDbScripts() => typeof(ExampleExAttrJunctions).GetDbScriptsFromClass();
    
    public static readonly MsSqlTable Table = new()
    {
        TableName = "ExampleExAttrJunctions",
        SqlStatement = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('[dbo].[ExampleExAttrJunctions]'))
            begin
                CREATE TABLE [dbo].[ExampleExAttrJunctions](
                    [ExampleObjectId] UNIQUEIDENTIFIER NOT NULL,
                    [ExampleExtendedAttributeId] UNIQUEIDENTIFIER NOT NULL,
                    CONSTRAINT Example_Obj_ExAttr_PK PRIMARY KEY (ExampleObjectId, ExampleExtendedAttributeId),
                    CONSTRAINT FK_ExampleObject
                        FOREIGN KEY (ExampleObjectId) REFERENCES dbo.[ExampleObjects] (Id),
                    CONSTRAINT FK_ExampleExtendedAttribute
                        FOREIGN KEY (ExampleExtendedAttributeId) REFERENCES dbo.[ExampleExtendedAttributes] (Id)
                )
            end"
    };
    
    public static readonly MsSqlStoredProcedure Delete = new()
    {
        Table = Table,
        Action = "Delete",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleExAttrJunctions_Delete]
                @ExampleObjectId UNIQUEIDENTIFIER,
                @ExampleExtendedAttributeId UNIQUEIDENTIFIER
            AS
            begin
            --     archive instead in production
                delete
                from dbo.[ExampleExAttrJunctions]
                where ExampleObjectId = @ExampleObjectId AND
                      ExampleExtendedAttributeId = @ExampleExtendedAttributeId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAll = new()
    {
        Table = Table,
        Action = "GetAll",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleExAttrJunctions_GetAll]
            AS
            begin
                select ExampleObjectId, ExampleExtendedAttributeId
                from dbo.[ExampleExAttrJunctions];
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetAttributesForObject = new()
    {
        Table = Table,
        Action = "GetAttributesForObject",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleExAttrJunctions_GetAttributesForObject]
                @ExampleObjectId UNIQUEIDENTIFIER
            AS
            begin
                select ExampleExtendedAttributeId
                from dbo.[ExampleExAttrJunctions]
                where ExampleObjectId = @ExampleObjectId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure GetObjectsForAttribute = new()
    {
        Table = Table,
        Action = "GetObjectsForAttribute",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleExAttrJunctions_GetObjectsForAttribute]
                @ExampleExtendedAttributeId UNIQUEIDENTIFIER
            AS
            begin
                select ExampleObjectId
                from dbo.[ExampleExAttrJunctions]
                where ExampleExtendedAttributeId = @ExampleExtendedAttributeId;
            end"
    };
    
    public static readonly MsSqlStoredProcedure Insert = new()
    {
        Table = Table,
        Action = "Insert",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleExAttrJunctions_Insert]
                @ExampleObjectId UNIQUEIDENTIFIER,
                @ExampleExtendedAttributeId UNIQUEIDENTIFIER
            AS
            begin
                insert into dbo.[ExampleExAttrJunctions] (ExampleObjectId, ExampleExtendedAttributeId)
                values (@ExampleObjectId, @ExampleExtendedAttributeId);
            end"
    };
    
    public static readonly MsSqlStoredProcedure Search = new()
    {
        Table = Table,
        Action = "Search",
        SqlStatement = @"
            CREATE OR ALTER PROCEDURE [dbo].[spExampleExAttrJunctions_Search]
                @SearchTerm NVARCHAR(256)
            AS
            begin
                set nocount on;
                
                select ExampleObjectId, ExampleExtendedAttributeId
                from dbo.[ExampleExAttrJunctions]
                where ExampleObjectId LIKE '%' + @SearchTerm + '%'
                    OR ExampleExtendedAttributeId LIKE '%' + @SearchTerm + '%';
            end"
    };
}