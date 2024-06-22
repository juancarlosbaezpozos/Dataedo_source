DECLARE @id AS UNIQUEIDENTIFIER = 'c155febe-4d34-484c-b82d-413c61f2f265'
DECLARE @version AS INT = 10
DECLARE @update AS INT = 3
DECLARE @release AS INT = 2
DECLARE @change_no AS VARCHAR(25) = 'DEV-5309'

IF NOT EXISTS (
		SELECT [id]
		FROM [database_update_log]
		WHERE [id] = @id
		)
BEGIN
	IF (
			SELECT COUNT(*)
			FROM [version]
			WHERE [version] = @version
				AND [update] = @update
				AND [release] = @release
			) = 0
	BEGIN
		INSERT INTO [version] (
			[version]
			,[update]
			,[release]
			,[stable]
			)
		VALUES (
			@version
			,@update
			,@release
			,0
			);
	END
	ELSE
	BEGIN
		UPDATE [version]
		SET [version] = @version
			,[update] = @update
			,[release] = @release
			,[stable] = 0
		WHERE [version] = @version
			AND [update] = @update
			AND [release] = @release;
	END

	/*-----------------------------------------------------------------------------------------------------------------------------*/
	--ADMIN
	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'MANAGE_SYSTEM_SETTINGS'
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
			,'MANAGE_SYSTEM_SETTINGS'
			,GETDATE()
			,GETDATE()
			)
	END;

	/*-----------------------------------------------------------------------------------------------------------------------------*/
	INSERT INTO [database_update_log] (
		[id]
		,[version_no]
		,[change_no]
		)
	VALUES (
		@id
		,(
			SELECT CAST(@version AS VARCHAR(2)) + '.' + CAST(@update AS VARCHAR(1)) + '.' + CAST(@release AS VARCHAR(1))
			)
		,@change_no
		);

	UPDATE [version]
	SET [version] = @version
		,[update] = @update
		,[release] = @release
		,[stable] = 1
	WHERE [version] = @version
		AND [update] = @update
		AND [release] = @release;
END
