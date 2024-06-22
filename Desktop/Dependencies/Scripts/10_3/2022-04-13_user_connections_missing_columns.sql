DECLARE @id AS UNIQUEIDENTIFIER = '363c2fac-c48c-4ed5-9612-26b02ead0496'
DECLARE @version as int = 10
DECLARE @update as int = 3
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-4406'

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
			WHERE object_id = OBJECT_ID(N'[user_connections]')
				AND name = 'perspective_name'
			)
	BEGIN
		ALTER TABLE [user_connections] ADD [perspective_name] [nvarchar](250) NULL
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [sys].[columns]
			WHERE object_id = OBJECT_ID(N'[user_connections]')
				AND name = 'param1'
			)
	BEGIN
		ALTER TABLE [user_connections] ADD [param1] [nvarchar](max) NULL
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [sys].[columns]
			WHERE object_id = OBJECT_ID(N'[user_connections]')
				AND name = 'param2'
			)
	BEGIN
		ALTER TABLE [user_connections] ADD [param2] [nvarchar](max) NULL
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [sys].[columns]
			WHERE object_id = OBJECT_ID(N'[user_connections]')
				AND name = 'param3'
			)
	BEGIN
		ALTER TABLE [user_connections] ADD [param3] [nvarchar](max) NULL
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [sys].[columns]
			WHERE object_id = OBJECT_ID(N'[user_connections]')
				AND name = 'param4'
			)
	BEGIN
		ALTER TABLE [user_connections] ADD [param4] [nvarchar](max) NULL
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [sys].[columns]
			WHERE object_id = OBJECT_ID(N'[user_connections]')
				AND name = 'param5'
			)
	BEGIN
		ALTER TABLE [user_connections] ADD [param5] [nvarchar](max) NULL
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [sys].[columns]
			WHERE object_id = OBJECT_ID(N'[user_connections]')
				AND name = 'param6'
			)
	BEGIN
		ALTER TABLE [user_connections] ADD [param6] [nvarchar](max) NULL
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [sys].[columns]
			WHERE object_id = OBJECT_ID(N'[user_connections]')
				AND name = 'param7'
			)
	BEGIN
		ALTER TABLE [user_connections] ADD [param7] [nvarchar](max) NULL
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [sys].[columns]
			WHERE object_id = OBJECT_ID(N'[user_connections]')
				AND name = 'param8'
			)
	BEGIN
		ALTER TABLE [user_connections] ADD [param8] [nvarchar](max) NULL
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [sys].[columns]
			WHERE object_id = OBJECT_ID(N'[user_connections]')
				AND name = 'param9'
			)
	BEGIN
		ALTER TABLE [user_connections] ADD [param9] [nvarchar](max) NULL
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [sys].[columns]
			WHERE object_id = OBJECT_ID(N'[user_connections]')
				AND name = 'param10'
			)
	BEGIN
		ALTER TABLE [user_connections] ADD [param10] [nvarchar](max) NULL
	END;

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO
