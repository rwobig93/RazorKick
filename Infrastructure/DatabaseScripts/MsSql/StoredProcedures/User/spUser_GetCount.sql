CREATE OR ALTER PROCEDURE [dbo].[spUser_GetCount]
    @TableName NVARCHAR(50)
AS
begin
    SELECT OBJECT_NAME(object_id), SUM(row_count) AS rows
    FROM sys.dm_db_partition_stats
    WHERE object_id = OBJECT_ID(@TableName)
      AND index_id < 2
    GROUP BY OBJECT_NAME(object_id);
end
