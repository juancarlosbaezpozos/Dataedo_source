-- Version

IF
  (
   SELECT COUNT(*)
     FROM [version]
     WHERE [version] = 7
           AND [update] = 1
           AND [release] = 0
  ) = 0
    BEGIN
        INSERT INTO [version]
        ([version],
         [update],
         [release],
         [stable]
        )
        VALUES
        (7,
         1,
         0,
         0
        );
    END;
GO

-- Columns

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE object_id = OBJECT_ID(N'[parameters]')
                      AND [name] = 'source'
             )
    BEGIN
        ALTER TABLE [parameters]
        ADD [source] nvarchar(50) CONSTRAINT [DF_parameters_source] DEFAULT N'DBMS'
                                  NOT NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE object_id = OBJECT_ID(N'[procedures]')
                      AND [name] = 'source'
             )
    BEGIN
        ALTER TABLE [procedures]
        ADD [source] nvarchar(50) CONSTRAINT [DF_procedures_source] DEFAULT N'DBMS'
                                  NOT NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE object_id = OBJECT_ID(N'[triggers]')
                      AND [name] = 'source'
             )
    BEGIN
        ALTER TABLE [triggers]
        ADD [source] nvarchar(50) CONSTRAINT [DF_triggers_source] DEFAULT N'DBMS'
                                  NOT NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE object_id = OBJECT_ID(N'[custom_fields]')
                      AND [name] = 'type'
             )
    BEGIN
        ALTER TABLE [custom_fields]
        ADD [type] nvarchar(100) CONSTRAINT [DF_custom_fields_type] DEFAULT N'TEXT'
                                 NOT NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE object_id = OBJECT_ID(N'[custom_fields]')
                      AND [name] = 'definition'
             )
    BEGIN
        ALTER TABLE [custom_fields]
        ADD [definition] nvarchar(max) NULL;
    END;
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'temp_sync_status'
)
BEGIN
    ALTER TABLE [columns]
    ADD [temp_sync_status] [bit] DEFAULT (0) NOT NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies]')
	   AND name = 'temp_sync_status'
)
BEGIN
    ALTER TABLE [dependencies]
    ADD [temp_sync_status] [bit] DEFAULT (0) NOT NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'temp_sync_status'
)
BEGIN
    ALTER TABLE [parameters]
    ADD [temp_sync_status] [bit] DEFAULT (0) NOT NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'temp_sync_status'
)
BEGIN
    ALTER TABLE [procedures]
    ADD [temp_sync_status] [bit] DEFAULT (0) NOT NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'temp_sync_status'
)
BEGIN
    ALTER TABLE [tables]
    ADD [temp_sync_status] [bit] DEFAULT (0) NOT NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'temp_sync_status'
)
BEGIN
    ALTER TABLE [tables_relations]
    ADD [temp_sync_status] [bit] DEFAULT (0) NOT NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations_columns]')
	   AND name = 'temp_sync_status'
)
BEGIN
    ALTER TABLE [tables_relations_columns]
    ADD [temp_sync_status] [bit] DEFAULT (0) NOT NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'temp_sync_status'
)
BEGIN
    ALTER TABLE [triggers]
    ADD [temp_sync_status] [bit] DEFAULT (0) NOT NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'temp_sync_status'
)
BEGIN
    ALTER TABLE [unique_constraints]
    ADD [temp_sync_status] [bit] DEFAULT (0) NOT NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints_columns]')
	   AND name = 'temp_sync_status'
)
BEGIN
    ALTER TABLE [unique_constraints_columns]
    ADD [temp_sync_status] [bit] DEFAULT (0) NOT NULL
END
GO

-- Indexes

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_dependencies_referencing_server'
                      AND [object_id] = OBJECT_ID(N'[dbo].[dependencies]')
             )
    BEGIN
        CREATE INDEX [ix_dependencies_referencing_server] ON [dbo].[dependencies]([referencing_server]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_dependencies_referenced_server'
                      AND [object_id] = OBJECT_ID(N'[dbo].[dependencies]')
             )
    BEGIN
        CREATE INDEX [ix_dependencies_referenced_server] ON [dbo].[dependencies]([referenced_server]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_dependencies_referencing_type'
                      AND [object_id] = OBJECT_ID(N'[dbo].[dependencies]')
             )
    BEGIN
        CREATE INDEX [ix_dependencies_referencing_type] ON [dbo].[dependencies]([referencing_type]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_dependencies_referenced_type'
                      AND [object_id] = OBJECT_ID(N'[dbo].[dependencies]')
             )
    BEGIN
        CREATE INDEX [ix_dependencies_referenced_type] ON [dbo].[dependencies]([referenced_type]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_dependencies_referencing_database'
                      AND [object_id] = OBJECT_ID(N'[dbo].[dependencies]')
             )
    BEGIN
        CREATE INDEX [ix_dependencies_referencing_database] ON [dbo].[dependencies]([referencing_database]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_dependencies_referenced_database'
                      AND [object_id] = OBJECT_ID(N'[dbo].[dependencies]')
             )
    BEGIN
        CREATE INDEX [ix_dependencies_referenced_database] ON [dbo].[dependencies]([referenced_database]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_dependencies_referencing_schema_name'
                      AND [object_id] = OBJECT_ID(N'[dbo].[dependencies]')
             )
    BEGIN
        CREATE INDEX [ix_dependencies_referencing_schema_name] ON [dbo].[dependencies]([referencing_schema],[referencing_name]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_dependencies_referenced_schema_name'
                      AND [object_id] = OBJECT_ID(N'[dbo].[dependencies]')
             )
    BEGIN
        CREATE INDEX [ix_dependencies_referenced_schema_name] ON [dbo].[dependencies]([referenced_schema],[referenced_name]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_modules_database_id'
                      AND [object_id] = OBJECT_ID(N'[dbo].[modules]')
             )
    BEGIN
        CREATE INDEX [ix_modules_database_id] ON [dbo].[modules]([database_id]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_tables_database_type_status'
                      AND [object_id] = OBJECT_ID(N'[dbo].[tables]')
             )
    BEGIN
       CREATE INDEX [ix_tables_database_type_status] ON [dbo].[tables]([database_id],[object_type],[status]) INCLUDE([schema],[name],[title],[dbms_creation_date],[dbms_last_modification_date],[synchronization_date]); 
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_procedures_database_type_status'
                      AND [object_id] = OBJECT_ID(N'[dbo].[procedures]')
             )
    BEGIN
        CREATE INDEX [ix_procedures_database_type_status] ON [dbo].[procedures]([database_id],[object_type],[status]) INCLUDE([schema],[name],[title],[dbms_creation_date],[dbms_last_modification_date],[synchronization_date]);     
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_columns_table_id'
                      AND [object_id] = OBJECT_ID(N'[dbo].[columns]')
             )
    BEGIN
        CREATE INDEX [ix_columns_table_id] ON [dbo].[columns]([table_id]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_parameters_procedure_id'
                      AND [object_id] = OBJECT_ID(N'[dbo].[parameters]')
             )
    BEGIN
        CREATE INDEX [ix_parameters_procedure_id] ON [dbo].[parameters]([procedure_id]);        
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_triggers_table_id'
                      AND [object_id] = OBJECT_ID(N'[dbo].[triggers]')
             )
    BEGIN
        CREATE INDEX [ix_triggers_table_id] ON [dbo].[triggers]([table_id]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_tables_relations_pk_table_id'
                      AND [object_id] = OBJECT_ID(N'[dbo].[tables_relations]')
             )
    BEGIN
        CREATE INDEX [ix_tables_relations_pk_table_id] ON [dbo].[tables_relations]([pk_table_id]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_tables_relations_fk_table_id]'
                      AND [object_id] = OBJECT_ID(N'[dbo].[tables_relations]')
             )
    BEGIN
       CREATE INDEX [ix_tables_relations_fk_table_id] ON [dbo].[tables_relations]([fk_table_id]); 
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_tables_relations_columns_column_fk_id'
                      AND [object_id] = OBJECT_ID(N'[dbo].[tables_relations_columns]')
             )
    BEGIN
        CREATE INDEX [ix_tables_relations_columns_column_fk_id] ON [dbo].[tables_relations_columns]([column_fk_id]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_tables_relations_columns_column_pk_id'
                      AND [object_id] = OBJECT_ID(N'[dbo].[tables_relations_columns]')
             )
    BEGIN
        CREATE INDEX [ix_tables_relations_columns_column_pk_id] ON [dbo].[tables_relations_columns]([column_pk_id]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_tables_relations_columns_table_relation_id'
                      AND [object_id] = OBJECT_ID(N'[dbo].[tables_relations_columns]')
             )
    BEGIN
        CREATE INDEX [ix_tables_relations_columns_table_relation_id] ON [dbo].[tables_relations_columns]([table_relation_id]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_unique_constraints_table_id'
                      AND [object_id] = OBJECT_ID(N'[dbo].[unique_constraints]')
             )
    BEGIN
        CREATE INDEX [ix_unique_constraints_table_id] ON [dbo].[unique_constraints]([table_id],[status]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_unique_constraints_columns_uc_id'
                      AND [object_id] = OBJECT_ID(N'[dbo].[unique_constraints_columns]')
             )
    BEGIN
        CREATE INDEX [ix_unique_constraints_columns_uc_id] ON [dbo].[unique_constraints_columns]([unique_constraint_id]);
    END;
GO
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_unique_constraints_columns_column_id'
                      AND [object_id] = OBJECT_ID(N'[dbo].[unique_constraints_columns]')
             )
    BEGIN
        CREATE INDEX [ix_unique_constraints_columns_column_id] ON [dbo].[unique_constraints_columns]([column_id]);
    END;
GO

-- Custom fields' values

CREATE TABLE [custom_fields_values]
([id]              int IDENTITY(1,1)
                       NOT NULL,
 [custom_field_id] int NOT NULL,
 [object_type]     nvarchar(100) NOT NULL,
 object_id         int NOT NULL,
 [value]           nvarchar(100) NOT NULL,
 CONSTRAINT [PK_custom_fields_values] PRIMARY KEY CLUSTERED([id] ASC)
 WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY];
GO
GRANT DELETE ON [custom_fields_values] TO [users] AS [dbo];
GO
GRANT INSERT ON [custom_fields_values] TO [users] AS [dbo];
GO
GRANT SELECT ON [custom_fields_values] TO [users] AS [dbo];
GO
GRANT UPDATE ON [custom_fields_values] TO [users] AS [dbo];
GO
CREATE NONCLUSTERED INDEX [IX_custom_fields_values] ON [custom_fields_values]([custom_field_id] ASC,[object_type] ASC,object_id ASC) WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,SORT_IN_TEMPDB = OFF,DROP_EXISTING = OFF,ONLINE = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
GO
CREATE NONCLUSTERED INDEX [IX_custom_fields_values_distinct] ON [custom_fields_values]([custom_field_id] ASC,[value] ASC) WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,SORT_IN_TEMPDB = OFF,DROP_EXISTING = OFF,ONLINE = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
GO

-- Version

UPDATE [version]
  SET
      [stable] = 1
  WHERE [version] = 7
        AND [update] = 1
        AND [release] = 0;
GO
