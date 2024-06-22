DECLARE @id AS UNIQUEIDENTIFIER = '5511DF58-991D-41F6-AE26-1E4A3DB945D9'
DECLARE @version as int = 10
DECLARE @update as int = 3
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-4671'

IF NOT EXISTS (SELECT [id] FROM [database_update_log] WHERE [id] =  @id)
BEGIN

IF (SELECT COUNT(*) FROM [version] WHERE [version] = @version AND [update] = @update AND [release] = @release) = 0
     BEGIN
       INSERT INTO [version] ([version], [update], [release], [stable]) VALUES (@version, @update, @release, 0);
     END
  ELSE
     BEGIN
       UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 0 WHERE [version] = @version AND [update] = @update AND [release] = @release;
     END

  /*-----------------------------------------------------------------------------------------------------------------------------*/

DELETE
FROM [erd_nodes_columns]
WHERE [node_column_id] IN
(
    SELECT [c].[node_column_id]
    FROM [erd_nodes_columns] [c]
        INNER JOIN [erd_nodes] [n] ON [n].[node_id] = [c].[node_id]
    WHERE [n].[module_id] != [c].[module_id]
)

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO
