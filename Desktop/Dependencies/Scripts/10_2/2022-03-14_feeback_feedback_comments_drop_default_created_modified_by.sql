DECLARE @id AS UNIQUEIDENTIFIER = '62e9bf70-5e95-4c57-9d25-5c8185cc5529'
DECLARE @version as int = 10 
DECLARE @update as int = 2
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-2881'

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
    IF OBJECT_ID('DF_feedback_created_by') IS NOT NULL
    ALTER TABLE [dbo].[feedback] DROP CONSTRAINT [DF_feedback_created_by]
    
    IF OBJECT_ID('DF_feedback_modified_by') IS NOT NULL
    ALTER TABLE [dbo].[feedback] DROP CONSTRAINT [DF_feedback_modified_by]
    
    IF OBJECT_ID('DF_feedback_comments_created_by') IS NOT NULL
    ALTER TABLE [dbo].[feedback_comments] DROP CONSTRAINT [DF_feedback_comments_created_by]
    
    IF OBJECT_ID('DF_feedback_comments_modified_by') IS NOT NULL
    ALTER TABLE [dbo].[feedback_comments] DROP CONSTRAINT [DF_feedback_comments_modified_by]

    DECLARE @feedback NVARCHAR(MAX);
    SET @feedback ='
    ALTER TRIGGER [trg_feedback_Modify]
       ON  [feedback]
       AFTER UPDATE
    AS 
    BEGIN
         UPDATE [feedback]
         SET 
            [last_modification_date] = GETDATE()
         WHERE [feedback_id] IN (SELECT DISTINCT [feedback_id] FROM Inserted)
    END'
    EXEC(@feedback);

    DECLARE @feedback_comments_trigger NVARCHAR(MAX);
    SET @feedback_comments_trigger = '
    ALTER TRIGGER [trg_feedback_comments_Modify]
       ON  [feedback_comments]
       AFTER UPDATE
    AS 
    BEGIN
         UPDATE [feedback_comments]
         SET 
             [last_modification_date] = GETDATE()
         WHERE [comment_id] IN (SELECT DISTINCT [comment_id] FROM Inserted)
    END'
    EXEC(@feedback_comments_trigger);

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO
