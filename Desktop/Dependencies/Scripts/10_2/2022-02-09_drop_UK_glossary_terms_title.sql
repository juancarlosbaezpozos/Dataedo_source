DECLARE @id AS UNIQUEIDENTIFIER = '3b1c6455-8b99-449c-9d7e-9498117addfb'
DECLARE @version as int = 10 
DECLARE @update as int = 2
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-2020'

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
  -- SQL script here. WITOUT key word 'GO'!
  IF OBJECT_ID('dbo.[UK_glossary_terms_title]') IS NOT NULL
  ALTER TABLE [dbo].[glossary_terms] DROP CONSTRAINT [UK_glossary_terms_title]

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO
