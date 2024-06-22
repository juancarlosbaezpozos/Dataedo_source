DECLARE @id AS UNIQUEIDENTIFIER = '4a7b7148-613a-449a-8d10-bc31624375eb'
DECLARE @version AS INT = 10
DECLARE @update AS INT = 2
DECLARE @release AS INT = 0
DECLARE @change_no AS VARCHAR(25) = 'DEV-3317'

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
	--LICENSES 
	IF NOT EXISTS (
			SELECT *
			FROM [sys].[columns]
			WHERE object_id = OBJECT_ID(N'[licenses]')
				AND name = 'name'
			)
	BEGIN
		ALTER TABLE licenses ADD [name] NVARCHAR(100)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [sys].[columns]
			WHERE object_id = OBJECT_ID(N'[licenses]')
				AND name = 'deleted'
			)
	BEGIN
		ALTER TABLE licenses ADD [deleted] BIT DEFAULT 0
	END;

	--USER GROUPS 
	IF OBJECT_ID('user_groups', N'U') IS NULL
	BEGIN
		CREATE TABLE user_groups (
			[user_group_id] INT IDENTITY(1, 1) NOT NULL
			,[name] NVARCHAR(1024) NOT NULL
			,[default] BIT NOT NULL
			,[creation_date] DATETIME NOT NULL
			,[created_by] NVARCHAR(1024)
			,[last_modification_date] DATETIME NOT NULL
			,[modified_by] NVARCHAR(1024)
			,[source_id] INT
			,CONSTRAINT [PK_user_groups] PRIMARY KEY CLUSTERED([user_group_id])
			);
	END;

	GRANT DELETE
		ON [user_groups]
		TO [users] AS [dbo];

	GRANT INSERT
		ON [user_groups]
		TO [users] AS [dbo];

	GRANT SELECT
		ON [user_groups]
		TO [users] AS [dbo];

	GRANT UPDATE
		ON [user_groups]
		TO [users] AS [dbo];

	--USERS USER GROUPS 
	IF OBJECT_ID('users_user_groups', N'U') IS NULL
	BEGIN
		CREATE TABLE users_user_groups (
			[id] INT IDENTITY(1, 1) NOT NULL
			,[user_id] INT NOT NULL
			,[user_group_id] INT NOT NULL
			,[creation_date] DATETIME NOT NULL
			,[created_by] NVARCHAR(1024)
			,[last_modification_date] DATETIME NOT NULL
			,[modified_by] NVARCHAR(1024)
			,[source_id] INT
			,CONSTRAINT [PK_users_user_groups] PRIMARY KEY CLUSTERED([id])
			,CONSTRAINT [FK_users_user_groups_licenses] FOREIGN KEY([user_id]) REFERENCES [licenses]([license_id]) 
				ON DELETE CASCADE
			,CONSTRAINT [FK_users_user_groups_user_groups] FOREIGN KEY([user_group_id]) REFERENCES [user_groups]([user_group_id]) 
				ON DELETE CASCADE
			);
	END;

	GRANT DELETE
		ON [users_user_groups]
		TO [users] AS [dbo];

	GRANT INSERT
		ON [users_user_groups]
		TO [users] AS [dbo];

	GRANT SELECT
		ON [users_user_groups]
		TO [users] AS [dbo];

	GRANT UPDATE
		ON [users_user_groups]
		TO [users] AS [dbo];

	--ROLES 
	IF OBJECT_ID('roles', N'U') IS NULL
	BEGIN
		CREATE TABLE roles (
			[role_id] INT IDENTITY(1, 1) NOT NULL
			,[name] NVARCHAR(1024) NOT NULL
			,[description] NVARCHAR(1024) NOT NULL
			,[creation_date] DATETIME NOT NULL
			,[created_by] NVARCHAR(1024)
			,[last_modification_date] DATETIME NOT NULL
			,[modified_by] NVARCHAR(1024)
			,[source_id] INT
			,CONSTRAINT [PK_roles] PRIMARY KEY CLUSTERED ([role_id])
			);
	END;

	GRANT DELETE
		ON [roles]
		TO [users] AS [dbo];

	GRANT INSERT
		ON [roles]
		TO [users] AS [dbo];

	GRANT SELECT
		ON [roles]
		TO [users] AS [dbo];

	GRANT UPDATE
		ON [roles]
		TO [users] AS [dbo];

	--ROLE ACTIONS 
	IF OBJECT_ID('role_actions', N'U') IS NULL
	BEGIN
		CREATE TABLE role_actions (
			[role_action_id] INT IDENTITY(1, 1) NOT NULL
			,[role_id] INT NOT NULL
			,[action_code] NVARCHAR(100) NOT NULL
			,[creation_date] DATETIME NOT NULL
			,[created_by] NVARCHAR(1024)
			,[last_modification_date] DATETIME NOT NULL
			,[modified_by] NVARCHAR(1024)
			,[source_id] INT
			,CONSTRAINT PK_role_actions PRIMARY KEY CLUSTERED ([role_action_id])
			,CONSTRAINT FK_role_actions_roles FOREIGN KEY([role_id]) REFERENCES [roles]([role_id]) 
				ON DELETE CASCADE
			);
	END;

	GRANT DELETE
		ON [role_actions]
		TO [users] AS [dbo];

	GRANT INSERT
		ON [role_actions]
		TO [users] AS [dbo];

	GRANT SELECT
		ON [role_actions]
		TO [users] AS [dbo];

	GRANT UPDATE
		ON [role_actions]
		TO [users] AS [dbo];

	--PERMISSIONS 
	IF OBJECT_ID('permissions', N'U') IS NULL
	BEGIN
		CREATE TABLE permissions (
			[permission_id] INT IDENTITY(1, 1) NOT NULL
			,[user_type] NVARCHAR(15) NOT NULL
			,[user_id] INT NULL
			,[user_group_id] INT NULL
			,[object_type] NVARCHAR(15) NOT NULL
			,[database_id] INT NULL 
			,[role_id] INT NOT NULL
			,[creation_date] DATETIME NOT NULL
			,[created_by] NVARCHAR(1024)
			,[last_modification_date] DATETIME NOT NULL
			,[modified_by] NVARCHAR(1024)
			,[source_id] INT
			,CONSTRAINT PK_permissions PRIMARY KEY CLUSTERED ([permission_id])
			,CONSTRAINT FK_permissions_licenses FOREIGN KEY ([user_id]) REFERENCES [licenses]([license_id]) 
				ON DELETE CASCADE
			,CONSTRAINT FK_permissions_user_groups FOREIGN KEY ([user_group_id]) REFERENCES [user_groups]([user_group_id]) 
				ON DELETE CASCADE
			,CONSTRAINT FK_permissions_databases FOREIGN KEY ([database_id]) REFERENCES [databases]([database_id]) 
				ON DELETE CASCADE
			,CONSTRAINT FK_permissions_roles FOREIGN KEY ([role_id]) REFERENCES [roles]([role_id])
				ON DELETE CASCADE
			);
	END;

	GRANT DELETE
		ON [permissions]
		TO [users] AS [dbo];

	GRANT INSERT
		ON [permissions]
		TO [users] AS [dbo];

	GRANT SELECT
		ON [permissions]
		TO [users] AS [dbo];

	GRANT UPDATE
		ON [permissions]
		TO [users] AS [dbo];

	IF NOT EXISTS (
			SELECT *
			FROM [roles]
			WHERE [name] = 'Viewer'
			)
	BEGIN
		INSERT INTO roles (
			name
			,description
			,creation_date
			,last_modification_date
			)
		VALUES (
			'Viewer'
			,'Can browse non-sensitive documentation in read-only mode.'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [roles]
			WHERE [name] = 'Explorer'
			)
	BEGIN
		INSERT INTO roles (
			name
			,description
			,creation_date
			,last_modification_date
			)
		VALUES (
			'Explorer'
			,'Can browse whole documentation in read-only mode.'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [roles]
			WHERE [name] = 'Data Community Member'
			)
	BEGIN
		INSERT INTO roles (
			name
			,description
			,creation_date
			,last_modification_date
			)
		VALUES (
			'Data Community Member'
			,'Can explore and comment on documentation.'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [roles]
			WHERE [name] = 'Data Steward'
			)
	BEGIN
		INSERT INTO roles (
			name
			,description
			,creation_date
			,last_modification_date
			)
		VALUES (
			'Data Steward'
			,'Can edit whole documentation and community.'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [roles]
			WHERE [name] = 'ADMIN'
			)
	BEGIN
		INSERT INTO roles (
			name
			,description
			,creation_date
			,last_modification_date
			)
		VALUES (
			'Admin'
			,'Can manage users and permissions.'
			,GETDATE()
			,GETDATE()
			)
	END;

	-- ROLE ACTIONS INSERTS
	--VIEWER
	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'WEB_ACCESS'
				AND [roles].[name] = 'Viewer'
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
				WHERE [roles].[name] = 'Viewer'
				)
			,'WEB_ACCESS'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'DOCUMENTATION_VIEW'
				AND [roles].[name] = 'Viewer'
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
				WHERE [roles].[name] = 'Viewer'
				)
			,'DOCUMENTATION_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'CLASSIFICATION_VIEW'
				AND [roles].[name] = 'Viewer'
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
				WHERE [roles].[name] = 'Viewer'
				)
			,'CLASSIFICATION_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	--EXPLORER
	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'WEB_ACCESS'
				AND [roles].[name] = 'Explorer'
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
				WHERE [roles].[name] = 'Explorer'
				)
			,'WEB_ACCESS'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'DOCUMENTATION_VIEW'
				AND [roles].[name] = 'Explorer'
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
				WHERE [roles].[name] = 'Explorer'
				)
			,'DOCUMENTATION_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'SCRIPTS_VIEW'
				AND [roles].[name] = 'Explorer'
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
				WHERE [roles].[name] = 'Explorer'
				)
			,'SCRIPTS_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'PROFILING_VIEW_DISTRIBUTION'
				AND [roles].[name] = 'Explorer'
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
				WHERE [roles].[name] = 'Explorer'
				)
			,'PROFILING_VIEW_DISTRIBUTION'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'PROFILING_VIEW_DATA'
				AND [roles].[name] = 'Explorer'
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
				WHERE [roles].[name] = 'Explorer'
				)
			,'PROFILING_VIEW_DATA'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'CLASSIFICATION_VIEW'
				AND [roles].[name] = 'Explorer'
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
				WHERE [roles].[name] = 'Explorer'
				)
			,'CLASSIFICATION_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'DEPENDENCIES_VIEW'
				AND [roles].[name] = 'Explorer'
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
				WHERE [roles].[name] = 'Explorer'
				)
			,'DEPENDENCIES_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'LINEAGE_VIEW'
				AND [roles].[name] = 'Explorer'
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
				WHERE [roles].[name] = 'Explorer'
				)
			,'LINEAGE_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

		IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'SOURCE_CONNECTION_VIEW'
				AND [roles].[name] = 'Explorer'
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
				WHERE [roles].[name] = 'Explorer'
				)
			,'SOURCE_CONNECTION_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	--Data Community Member
	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'WEB_ACCESS'
				AND [roles].[name] = 'Data Community Member'
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
				WHERE [roles].[name] = 'Data Community Member'
				)
			,'WEB_ACCESS'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'DOCUMENTATION_VIEW'
				AND [roles].[name] = 'Data Community Member'
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
				WHERE [roles].[name] = 'Data Community Member'
				)
			,'DOCUMENTATION_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'SCRIPTS_VIEW'
				AND [roles].[name] = 'Data Community Member'
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
				WHERE [roles].[name] = 'Data Community Member'
				)
			,'SCRIPTS_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'PROFILING_VIEW_DISTRIBUTION'
				AND [roles].[name] = 'Data Community Member'
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
				WHERE [roles].[name] = 'Data Community Member'
				)
			,'PROFILING_VIEW_DISTRIBUTION'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'PROFILING_VIEW_DATA'
				AND [roles].[name] = 'Data Community Member'
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
				WHERE [roles].[name] = 'Data Community Member'
				)
			,'PROFILING_VIEW_DATA'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'CLASSIFICATION_VIEW'
				AND [roles].[name] = 'Data Community Member'
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
				WHERE [roles].[name] = 'Data Community Member'
				)
			,'CLASSIFICATION_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'DEPENDENCIES_VIEW'
				AND [roles].[name] = 'Data Community Member'
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
				WHERE [roles].[name] = 'Data Community Member'
				)
			,'DEPENDENCIES_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'COMMUNITY_MANAGE'
				AND [roles].[name] = 'Data Community Member'
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
				WHERE [roles].[name] = 'Data Community Member'
				)
			,'COMMUNITY_MANAGE'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'COMMUNITY_EDIT'
				AND [roles].[name] = 'Data Community Member'
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
				WHERE [roles].[name] = 'Data Community Member'
				)
			,'COMMUNITY_EDIT'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'COMMUNITY_VIEW'
				AND [roles].[name] = 'Data Community Member'
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
				WHERE [roles].[name] = 'Data Community Member'
				)
			,'COMMUNITY_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'LINEAGE_VIEW'
				AND [roles].[name] = 'Data Community Member'
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
				WHERE [roles].[name] = 'Data Community Member'
				)
			,'LINEAGE_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

		IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'SOURCE_CONNECTION_VIEW'
				AND [roles].[name] = 'Data Community Member'
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
				WHERE [roles].[name] = 'Data Community Member'
				)
			,'SOURCE_CONNECTION_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	--DATA STEWARD
	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'WEB_ACCESS'
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
			,'WEB_ACCESS'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'DOCUMENTATION_VIEW'
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
			,'DOCUMENTATION_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'DOCUMENTATION_EDIT'
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
			,'DOCUMENTATION_EDIT'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'SCRIPTS_VIEW'
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
			,'SCRIPTS_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'PROFILING_VIEW_DISTRIBUTION'
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
			,'PROFILING_VIEW_DISTRIBUTION'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'PROFILING_VIEW_DATA'
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
			,'PROFILING_VIEW_DATA'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'CLASSIFICATION_VIEW'
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
			,'CLASSIFICATION_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'DEPENDENCIES_VIEW'
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
			,'DEPENDENCIES_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'COMMUNITY_MANAGE'
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
			,'COMMUNITY_MANAGE'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'COMMUNITY_EDIT'
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
			,'COMMUNITY_EDIT'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'COMMUNITY_VIEW'
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
			,'COMMUNITY_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'CLASSIFICATION_EDIT'
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
			,'CLASSIFICATION_EDIT'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'SOURCE_CONNECTION_VIEW'
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
			,'SOURCE_CONNECTION_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'DOCUMENTATION_VIEW'
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
			,'DOCUMENTATION_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'KEYS_RELATIONS_MANAGE'
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
			,'KEYS_RELATIONS_MANAGE'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'LINEAGE_VIEW'
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
			,'LINEAGE_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	--ADMIN
	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'WEB_ACCESS'
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
			,'WEB_ACCESS'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'USERS_VIEW'
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
			,'USERS_VIEW'
			,GETDATE()
			,GETDATE()
			)
	END;

	IF NOT EXISTS (
			SELECT *
			FROM [role_actions]
			INNER JOIN [roles] ON [role_actions].[role_id] = [roles].[role_id]
			WHERE [role_actions].[action_code] = 'USERS_MANAGE'
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
			,'USERS_MANAGE'
			,GETDATE()
			,GETDATE()
			)
	END;

	--Migrated Editors group
	BEGIN
		INSERT INTO [user_groups] ([name], [default], [creation_date], [last_modification_date])
		VALUES ('Migrated Editors', 0, GETDATE(), GETDATE())
	END;

	--Migrated Editors permissions
	BEGIN
	INSERT INTO [permissions] ([user_group_id], [object_type], [role_id], [creation_date], [last_modification_date], [user_type])
	SELECT (SELECT user_group_id from user_groups where [user_groups].[name] = 'Migrated Editors'), 'REPOSITORY', [role_id], GETDATE(), GETDATE(), 'GROUP'
	FROM [roles] 
	END;

	--Migrated Editors users
	BEGIN
	INSERT INTO [users_user_groups] ([user_id], [user_group_id], [creation_date], [last_modification_date])
	SELECT [license_id], (SELECT user_group_id from user_groups where [user_groups].[name] = 'Migrated Editors'), GETDATE(), GETDATE()
	FROM [licenses]
	WHERE [licenses].[is_offline] = 0 OR ([licenses].[key] IS NOT NULL AND [licenses].[key] <> '') 
	END;

	--Migrated Viewers group
	BEGIN
	INSERT INTO [user_groups] ([name], [default], [creation_date], [last_modification_date])
	VALUES ('Migrated Viewers', 1, GETDATE(), GETDATE())
	END;

	--Migrated Viewers permissions
	BEGIN
	INSERT INTO [permissions] ([user_group_id], [object_type], [role_id], [creation_date], [last_modification_date], [user_type])
	SELECT (SELECT user_group_id from user_groups where [user_groups].[name] = 'Migrated Viewers'), 'REPOSITORY', [role_id], GETDATE(), GETDATE(), 'GROUP'
	FROM [roles]
	WHERE [name] = 'Viewer' OR [name] = 'Data Community Member'
	END;

	--Migrated Viewers users
	BEGIN
	INSERT INTO [users_user_groups] ([user_id], [user_group_id], [creation_date], [last_modification_date])
	SELECT [license_id], (SELECT user_group_id from user_groups where [user_groups].[name] = 'Migrated Viewers'), GETDATE(), GETDATE()
	FROM [licenses]
	WHERE ([licenses].[is_offline] = 1 OR [licenses].[is_offline] IS NULL) AND ([licenses].[key] IS NULL OR [licenses].[key] = '')
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
