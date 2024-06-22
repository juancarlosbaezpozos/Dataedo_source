DECLARE @id AS UNIQUEIDENTIFIER = 'd164a04e-9bb5-4b45-8b47-e828149d508c'
DECLARE @version as int = 10
DECLARE @update as int = 3
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-4753'

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
	IF NOT EXISTS(
		SELECT TOP 1 1
		FROM [sys].[triggers]
		WHERE [name] = N'trg_columns_changes_description_modified'
	)
	BEGIN
		EXEC dbo.sp_executesql @statement = N'
			CREATE TRIGGER [trg_columns_changes_description_modified]
				ON [columns_changes]
				AFTER UPDATE
			AS 
			BEGIN
				IF UPDATE ([description]) 
				BEGIN
					UPDATE [columns_changes] 
					SET [description_date] = GETDATE()
					WHERE [id] in (SELECT DISTINCT [id] FROM Inserted)
				END
			END'

		ALTER TABLE [columns_changes] ENABLE TRIGGER [trg_columns_changes_description_modified]
	END;

	IF NOT EXISTS(
		SELECT TOP 1 1
		FROM [sys].[triggers]
		WHERE [name] = N'trg_parameters_changes_description_modified'
	)
	BEGIN
		EXEC dbo.sp_executesql @statement = N'
			CREATE TRIGGER [trg_parameters_changes_description_modified]
				ON [parameters_changes]
				AFTER UPDATE
			AS 
			BEGIN
				IF UPDATE ([description]) 
				BEGIN
					UPDATE [parameters_changes] 
					SET [description_date] = GETDATE()
					WHERE [id] in (SELECT DISTINCT [id] FROM Inserted)
				END
			END'

		ALTER TABLE [parameters_changes] ENABLE TRIGGER [trg_parameters_changes_description_modified]
	END;

	IF NOT EXISTS(
		SELECT TOP 1 1
		FROM [sys].[triggers]
		WHERE [name] = N'trg_procedures_changes_description_modified'
	)
	BEGIN
		EXEC dbo.sp_executesql @statement = N'
			CREATE TRIGGER [trg_procedures_changes_description_modified]
				ON [procedures_changes]
				AFTER UPDATE
			AS 
			BEGIN
				IF UPDATE ([description]) 
				BEGIN
					UPDATE [procedures_changes] 
					SET [description_date] = GETDATE()
					WHERE [id] in (SELECT DISTINCT [id] FROM Inserted)
				END
			END'

		ALTER TABLE [procedures_changes] ENABLE TRIGGER [trg_procedures_changes_description_modified]
	END;
	
	IF NOT EXISTS(
		SELECT TOP 1 1
		FROM [sys].[triggers]
		WHERE [name] = N'trg_tables_changes_description_modified'
	)
	BEGIN
		EXEC dbo.sp_executesql @statement = N'
			CREATE TRIGGER [trg_tables_changes_description_modified]
				ON [tables_changes]
				AFTER UPDATE
			AS 
			BEGIN
				IF UPDATE ([description]) 
				BEGIN
					UPDATE [tables_changes] 
					SET [description_date] = GETDATE()
					WHERE [id] in (SELECT DISTINCT [id] FROM Inserted)
				END
			END'

		ALTER TABLE [tables_changes] ENABLE TRIGGER [trg_tables_changes_description_modified]
	END;

	IF NOT EXISTS(
		SELECT TOP 1 1
		FROM [sys].[triggers]
		WHERE [name] = N'trg_tables_relations_changes_description_modified'
	)
	BEGIN
		EXEC dbo.sp_executesql @statement = N'
			CREATE TRIGGER [trg_tables_relations_changes_description_modified]
				ON [tables_relations_changes]
				AFTER UPDATE
			AS 
			BEGIN
				IF UPDATE ([description]) 
				BEGIN
					UPDATE [tables_relations_changes] 
					SET [description_date] = GETDATE()
					WHERE [id] in (SELECT DISTINCT [id] FROM Inserted)
				END
			END'

		ALTER TABLE [tables_relations_changes] ENABLE TRIGGER [trg_tables_relations_changes_description_modified]
	END;

	IF NOT EXISTS(
		SELECT TOP 1 1
		FROM [sys].[triggers]
		WHERE [name] = N'trg_triggers_changes_description_modified'
	)
	BEGIN
		EXEC dbo.sp_executesql @statement = N'
			CREATE TRIGGER [trg_triggers_changes_description_modified]
				ON [triggers_changes]
				AFTER UPDATE
			AS 
			BEGIN
				IF UPDATE ([description]) 
				BEGIN
					UPDATE [triggers_changes] 
					SET [description_date] = GETDATE()
					WHERE [id] in (SELECT DISTINCT [id] FROM Inserted)
				END
			END'

		ALTER TABLE [triggers_changes] ENABLE TRIGGER [trg_triggers_changes_description_modified]
	END;

	IF NOT EXISTS(
		SELECT TOP 1 1
		FROM [sys].[triggers]
		WHERE [name] = N'trg_unique_constraints_changes_description_modified'
	)
	BEGIN
		EXEC dbo.sp_executesql @statement = N'
			CREATE TRIGGER [trg_unique_constraints_changes_description_modified]
				ON [unique_constraints_changes]
				AFTER UPDATE
			AS 
			BEGIN
				IF UPDATE ([description]) 
				BEGIN
					UPDATE [unique_constraints_changes] 
					SET [description_date] = GETDATE()
					WHERE [id] in (SELECT DISTINCT [id] FROM Inserted)
				END
			END'

		ALTER TABLE [unique_constraints_changes] ENABLE TRIGGER [trg_unique_constraints_changes_description_modified]
	END;


	IF NOT EXISTS(
		SELECT TOP 1 1
		FROM [sys].[triggers]
		WHERE [name] = N'trg_schema_updates_description_modified'
	)
	BEGIN
		EXEC dbo.sp_executesql @statement = N'
			CREATE TRIGGER [trg_schema_updates_description_modified]
				ON [schema_updates]
				AFTER UPDATE
			AS 
			BEGIN
				IF UPDATE ([description]) 
				BEGIN
					UPDATE [schema_updates] 
					SET [description_date] = GETDATE()
					WHERE [update_id] in (SELECT DISTINCT [update_id] FROM Inserted)
				END
			END'

		ALTER TABLE [schema_updates] ENABLE TRIGGER [trg_schema_updates_description_modified]
	END;

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO
