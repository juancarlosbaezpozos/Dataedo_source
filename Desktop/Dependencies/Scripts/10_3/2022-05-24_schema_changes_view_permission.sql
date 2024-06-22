DECLARE @id AS UNIQUEIDENTIFIER = '60d47f42-7c78-4a52-834a-f612cc861e67'
DECLARE @version as int = 10
DECLARE @update as int = 3
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-4773'

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
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'SCHEMA_CHANGES_VIEW'
				AND [roles].[name] = 'Data Steward'
			)
	BEGIN
		INSERT INTO role_actions (
			role_id
			,action_code
			,creation_date
			,last_modification_date
			)
		VALUES (
			(
				SELECT TOP (1) role_id
				FROM [roles]
				WHERE [roles].[name] = 'Data Steward'
				)
			,'SCHEMA_CHANGES_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'SCHEMA_CHANGES_VIEW'
				AND [roles].[name] = 'Admin'
			)
	BEGIN
		INSERT INTO role_actions (
			role_id
			,action_code
			,creation_date
			,last_modification_date
			)
		VALUES (
			(
				SELECT TOP (1) role_id
				FROM [roles]
				WHERE [roles].[name] = 'Admin'
				)
			,'SCHEMA_CHANGES_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO
