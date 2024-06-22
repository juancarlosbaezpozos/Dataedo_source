DECLARE @id AS UNIQUEIDENTIFIER = '7c595432-c386-4ce2-81e8-4f04f1f9bbb8'
DECLARE @version as int = 10
DECLARE @update as int = 3
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-4368'

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
				FROM [sys].[columns]
				WHERE object_id = OBJECT_ID(N'[glossary_term_types]')
					AND name = 'list_name'
				)
		BEGIN
			ALTER TABLE [glossary_term_types] ADD [list_name] NVARCHAR(250) NOT NULL
			CONSTRAINT [DF_glossary_term_types_list_name] DEFAULT ''
		END;

		BEGIN
			EXEC('UPDATE [glossary_term_types]
			SET [list_name] = N''Terms''
			WHERE code = ''TERM''')
		END;
		
		BEGIN
		EXEC('UPDATE [glossary_term_types]
			SET [list_name] = N''Policies''
			WHERE code = ''POLICY''')
		END
		
		BEGIN
		EXEC('UPDATE [glossary_term_types]
			SET [list_name] = N''Categories''
			WHERE code = ''CATEGORY''')
		END;
		
		BEGIN
			EXEC('UPDATE [glossary_term_types]
			SET [list_name] = N''Rules''
			WHERE code = ''RULE''')
		END;


  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO
