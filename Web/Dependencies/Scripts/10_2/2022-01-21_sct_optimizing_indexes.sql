DECLARE @id AS UNIQUEIDENTIFIER = 'd71fdd1d-22bb-4aef-99f4-c1d997d3e9de'
DECLARE @version as int = 10 
DECLARE @update as int = 2
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-2340'

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
  
    IF NOT EXISTS
        (
        SELECT TOP 1 1
        FROM [sys].[indexes]
        WHERE [name] = 'IX_columns_changes_update_id_table_id'
            AND [object_id] = OBJECT_ID(N'columns_changes')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_columns_changes_update_id_table_id
            ON [columns_changes] ([update_id],[table_id])
        END;

    IF NOT EXISTS
        (
        SELECT TOP 1 1
        FROM [sys].[indexes]
        WHERE [name] = 'IX_triggers_changes_update_id_table_id'
            AND [object_id] = OBJECT_ID(N'triggers_changes')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_triggers_changes_update_id_table_id
            ON [triggers_changes] ([update_id],[table_id])
        END;

    IF NOT EXISTS
        (
        SELECT TOP 1 1
        FROM [sys].[indexes]
        WHERE [name] = 'IX_unique_constraints_changes_update_id_table_id'
            AND [object_id] = OBJECT_ID(N'unique_constraints_changes')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_unique_constraints_changes_update_id_table_id
            ON [unique_constraints_changes] ([update_id],[table_id])
        END;

    IF NOT EXISTS
        (
        SELECT TOP 1 1
        FROM [sys].[indexes]
        WHERE [name] = 'IX_tables_relations_changes_update_id_table_id'
            AND [object_id] = OBJECT_ID(N'tables_relations_changes')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_tables_relations_changes_update_id_table_id
            ON [dbo].[tables_relations_changes] ([update_id],[fk_table_id])
        END;

    IF NOT EXISTS 
        (
            SELECT TOP 1 1
            FROM [sys].[indexes]
            WHERE [name] = 'IX_parameters_changes_update_id_procedure_id'
                AND [object_id] = OBJECT_ID(N'parameters_changes')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_parameters_changes_update_id_procedure_id
            ON [dbo].[parameters_changes] ([update_id], [procedure_id])
        END;

    IF NOT EXISTS
        (
            SELECT TOP 1 1
            FROM [sys].[indexes]
            WHERE [name] = 'IX_tables_changes_table_id'
                AND [object_id] = OBJECT_ID(N'tables_changes')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_tables_changes_table_id
            ON [dbo].[tables_changes] ([table_id])
        END;

    IF NOT EXISTS
        (
            SELECT TOP 1 1 
            FROM [sys].[indexes]
            WHERE [name] = 'IX_procedure_changes_procedure_id'
                AND [object_id] = OBJECT_ID(N'procedures_changes')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_procedure_changes_procedure_id
            ON [dbo].[procedures_changes] ([procedure_id])
        END;

  /*-----------------------------------------------------------------------------------------------------------------------------*/

  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO