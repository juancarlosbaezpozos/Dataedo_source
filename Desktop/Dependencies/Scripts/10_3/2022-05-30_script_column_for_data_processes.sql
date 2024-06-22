DECLARE @id AS UNIQUEIDENTIFIER = '7f25bdb8-abcc-4b96-bb7e-b55cfe73b9be'
DECLARE @version as int = 10
DECLARE @update as int = 3
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-4765'

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
	IF NOT EXISTS (
        SELECT *
        FROM sys.columns
        WHERE object_id = OBJECT_ID(N'[data_processes]')
            AND name = 'script')
    BEGIN
        ALTER TABLE [data_processes]
        ADD [script] NVARCHAR(MAX) NULL
    END

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO
