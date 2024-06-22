DECLARE @id AS UNIQUEIDENTIFIER = '95c3a7ab-6a52-4e02-ac50-f036736077c5'
DECLARE @version as int = 10 
DECLARE @update as int = 2
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-2334'

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

  ALTER TABLE [columns]
  ALTER COLUMN [value_min_date] [datetime2] NULL;

  ALTER TABLE [columns]
  ALTER COLUMN [value_max_date] [datetime2] NULL;

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO