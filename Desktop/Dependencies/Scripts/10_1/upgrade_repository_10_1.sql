-- =============================================
-- Author:      Szymon Karpęcki
-- Create date: 1/12/2021
-- Description: Script upgrading repository to Dataedo 10.1
-- =============================================

-- Version
IF
  (
   SELECT COUNT(*)
     FROM [version]
     WHERE [version] = 10
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
        (10,
         1,
         0,
         0
        );
    END;
GO


-- ============================================= 
-- Author:      Michał Psyk
-- Create date: 12/10/2021
-- Description: DEV-536 Repository for Data Lineage
-- =============================================  

IF OBJECT_ID(N'data_processes', N'U') IS NULL
	BEGIN
		CREATE TABLE [data_processes](
			 [process_id] int IDENTITY(1,1) NOT NULL,
			 [name] nvarchar(250) NOT NULL,
			 [processor_type] nvarchar(100) NOT NULL,
			 [processor_id] int NOT NULL,
			 [creation_date] datetime NOT NULL,
			 [created_by] nvarchar(1024) NULL,
			 [last_modification_date] datetime NOT NULL,
			 [modified_by] nvarchar(1024) NULL,
			 [source] nvarchar(50) NOT NULL
		 CONSTRAINT [PK_data_processes] PRIMARY KEY CLUSTERED (process_id)
			WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
		)
		ON [PRIMARY];
	END;
GO
GRANT DELETE ON [data_processes] TO [users] AS [dbo];
GO
GRANT INSERT ON [data_processes] TO [users] AS [dbo];
GO
GRANT SELECT ON [data_processes] TO [users] AS [dbo];
GO
GRANT UPDATE ON [data_processes] TO [users] AS [dbo];
GO

IF OBJECT_ID(N'DF_data_processes_creation_date', N'D') IS NULL
BEGIN
    ALTER TABLE [data_processes] ADD  CONSTRAINT [DF_data_processes_creation_date]  DEFAULT (getdate()) FOR [creation_date]
END
GO

IF OBJECT_ID(N'DF_data_processes_created_by', N'D') IS NULL
BEGIN
    ALTER TABLE [data_processes] ADD  CONSTRAINT [DF_data_processes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
END
GO

IF OBJECT_ID(N'DF_data_processes_last_modification_date', N'D') IS NULL
BEGIN
    ALTER TABLE [data_processes] ADD  CONSTRAINT [DF_data_processes_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
END
GO

IF OBJECT_ID(N'DF_data_processes_modified_by', N'D') IS NULL
BEGIN
    ALTER TABLE [data_processes] ADD  CONSTRAINT [DF_data_processes_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
END
GO

IF OBJECT_ID(N'data_flows', N'U') IS NULL
	BEGIN
		CREATE TABLE [data_flows](
			 [flow_id] int IDENTITY(1,1) NOT NULL,
			 [process_id] int NOT NULL,
			 [direction] nvarchar(10) NOT NULL,
			 [object_type] nvarchar(100) NOT NULL,
			 [object_id] int NOT NULL,
			 [creation_date] datetime NOT NULL,
			 [created_by] nvarchar(1024) NULL,
			 [last_modification_date] datetime NOT NULL,
			 [modified_by] nvarchar(1024) NULL,
			 [source] nvarchar(50) NOT NULL
		 CONSTRAINT [PK_data_flows] PRIMARY KEY CLUSTERED (flow_id)
			WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
		 CONSTRAINT [FK_data_flows_data_processes] FOREIGN KEY (process_id) REFERENCES data_processes(process_id)
			ON DELETE CASCADE
		)
		ON [PRIMARY];
	END;
GO
GRANT DELETE ON [data_flows] TO [users] AS [dbo];
GO
GRANT INSERT ON [data_flows] TO [users] AS [dbo];
GO
GRANT SELECT ON [data_flows] TO [users] AS [dbo];
GO
GRANT UPDATE ON [data_flows] TO [users] AS [dbo];
GO

IF OBJECT_ID(N'DF_data_flows_creation_date', N'D') IS NULL
BEGIN
    ALTER TABLE [data_flows] ADD  CONSTRAINT [DF_data_flows_creation_date]  DEFAULT (getdate()) FOR [creation_date]
END
GO

IF OBJECT_ID(N'DF_data_flows_created_by', N'D') IS NULL
BEGIN
    ALTER TABLE [data_flows] ADD  CONSTRAINT [DF_data_flows_created_by]  DEFAULT (suser_sname()) FOR [created_by]
END
GO

IF OBJECT_ID(N'DF_data_flows_last_modification_date', N'D') IS NULL
BEGIN
    ALTER TABLE [data_flows] ADD  CONSTRAINT [DF_data_flows_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
END
GO

IF OBJECT_ID(N'DF_data_flows_modified_by', N'D') IS NULL
BEGIN
    ALTER TABLE [data_flows] ADD  CONSTRAINT [DF_data_flows_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
END
GO

IF NOT EXISTS
    (
    SELECT * 
    FROM [sys].[indexes]
    WHERE [name] = 'IX_data_flows_process_id'
        AND [object_id] = OBJECT_ID(N'data_flows')
    )
    BEGIN
        CREATE INDEX [IX_data_flows_process_id] ON [data_flows]([process_id])
    END;
GO


-- =============================================
-- Author:		Szymon Karpęcki
-- Create date: 01/12/2021
-- Description:	Updates last_modification_date columns of data lineage tables on insert or update
-- =============================================

IF NOT EXISTS
    (
    SELECT TOP 1 1
    FROM [sys].[triggers]
    WHERE [name] = N'trg_data_processes_Modify'
    )
BEGIN
    EXEC dbo.sp_executesql @statement = N'
        CREATE TRIGGER [dbo].[trg_data_processes_Modify]
           ON  [dbo].[data_processes]
           AFTER INSERT,UPDATE
        AS 
        BEGIN
             UPDATE [data_processes]
             SET  
                 [last_modification_date] = GETDATE()
             WHERE [process_id] IN (SELECT DISTINCT [process_id] FROM Inserted)
        END
    '
END

ALTER TABLE [dbo].[data_processes] ENABLE TRIGGER [trg_data_processes_Modify]
GO

IF NOT EXISTS
    (
    SELECT TOP 1 1
    FROM [sys].[triggers]
    WHERE [name] = N'trg_data_flow_Modify'
    )
BEGIN
    EXEC dbo.sp_executesql @statement = N'
        CREATE TRIGGER [dbo].[trg_data_flow_Modify]
           ON  [dbo].[data_flows]
           AFTER INSERT,UPDATE
        AS 
        BEGIN
             UPDATE [data_flows]
             SET  
                 [last_modification_date] = GETDATE()
             WHERE [flow_id] IN (SELECT DISTINCT [flow_id] FROM Inserted)
        END
    '
END

ALTER TABLE [dbo].[data_flows] ENABLE TRIGGER [trg_data_flow_Modify]
GO

-- =============================================
-- Author:      Szymon Karpęcki
-- Create date: 28/11/2021
-- Description: Delete modified_by from triggers
-- =============================================

ALTER TRIGGER [dbo].[trg_classificator_masks_Modify]
   ON  [dbo].[classificator_masks]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [classificator_masks]
     SET  
        last_modification_date = GETDATE()
     WHERE classificator_mask_id IN (SELECT DISTINCT classificator_mask_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_classificator_rules_Modify]
   ON  [dbo].[classificator_rules]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [classificator_rules]
     SET  
        last_modification_date = GETDATE()
     WHERE classificator_rule_id IN (SELECT DISTINCT classificator_rule_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_classificators_Modify]
   ON  [dbo].[classificators]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [classificators]
     SET  
        last_modification_date = GETDATE()
     WHERE classificator_id IN (SELECT DISTINCT classificator_id FROM Inserted)
END
GO

 
ALTER TRIGGER [dbo].[trg_columns_Modify]
   ON  [dbo].[columns]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [columns]
     SET  
        last_modification_date = GETDATE()
     WHERE column_id IN (SELECT DISTINCT column_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_custom_fields_Modify]
   ON  [dbo].[custom_fields]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [custom_fields]
     SET  
        last_modification_date = GETDATE()
     WHERE custom_field_id IN (SELECT DISTINCT custom_field_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_databases_Modify]
   ON  [dbo].[databases] 
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [databases]
     SET  
        last_modification_date = GETDATE()
     WHERE database_id IN (SELECT DISTINCT database_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_dependencies_Modify]
   ON  [dbo].[dependencies]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [dependencies]
     SET  
        last_modification_date = GETDATE()
     WHERE dependency_id IN (SELECT DISTINCT dependency_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_dependencies_descriptions_Modify]
   ON  [dbo].[dependencies_descriptions]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [dependencies_descriptions]
     SET  
        last_modification_date = GETDATE()
     WHERE dependency_descriptions_id IN (SELECT DISTINCT dependency_descriptions_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_documentation_custom_fields_Modify]
   ON  [dbo].[documentation_custom_fields]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [documentation_custom_fields]
     SET  
        last_modification_date = GETDATE()
     WHERE documentation_custom_field_id IN (SELECT DISTINCT documentation_custom_field_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_erd_links_descriptions_Modify]
   ON  [dbo].[erd_links]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [erd_links]
     SET  
        last_modification_date = GETDATE()
     WHERE link_id IN (SELECT DISTINCT link_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_erd_nodes_descriptions_Modify]
   ON  [dbo].[erd_nodes]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [erd_nodes]
     SET  
        last_modification_date = GETDATE()
     WHERE node_id IN (SELECT DISTINCT node_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_feedback_Modify]
   ON  [dbo].[feedback]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [feedback]
     SET 
        [last_modification_date] = GETDATE()
     WHERE [feedback_id] IN (SELECT DISTINCT [feedback_id] FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_feedback_comments_Modify]
   ON  [dbo].[feedback_comments]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [feedback_comments]
     SET 
         [last_modification_date] = GETDATE()
     WHERE [comment_id] IN (SELECT DISTINCT [comment_id] FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_feedback_links_Modify]
   ON  [dbo].[feedback_links]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [feedback_links]
     SET 
         [last_modification_date] = GETDATE()
     WHERE [link_id] IN (SELECT DISTINCT [link_id] FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_glossary_mappings_Modify] 
    ON [dbo].[glossary_mappings]
    AFTER INSERT,UPDATE
AS
    BEGIN
        UPDATE [glossary_mappings]
          SET
              [last_modification_date] = GETDATE()
          WHERE [mapping_id] IN (SELECT DISTINCT [mapping_id] FROM [Inserted])
    END;
GO


ALTER TRIGGER [dbo].[trg_glossary_term_relationship_types_Modify] 
    ON [dbo].[glossary_term_relationship_types]
    AFTER INSERT,UPDATE
AS
    BEGIN
        UPDATE [glossary_term_relationship_types]
          SET
              [last_modification_date] = GETDATE()
          WHERE [type_id] IN(SELECT DISTINCT [type_id] FROM [Inserted]);
    END;
GO


ALTER TRIGGER [dbo].[trg_glossary_term_relationships_Modify] 
    ON [dbo].[glossary_term_relationships]
    AFTER INSERT,UPDATE
AS
    BEGIN
        UPDATE [glossary_term_relationships]
          SET
              [last_modification_date] = GETDATE()
          WHERE [relationship_id] IN ( SELECT DISTINCT [relationship_id] FROM [Inserted]);
    END;
GO


ALTER TRIGGER [dbo].[trg_glossary_term_types_Modify] 
    ON [dbo].[glossary_term_types]
    AFTER INSERT,UPDATE
AS
    BEGIN
        UPDATE [glossary_term_types]
          SET
              [last_modification_date] = GETDATE()
          WHERE [term_type_id] IN (SELECT DISTINCT [term_type_id] FROM [Inserted]);
    END;
GO


ALTER TRIGGER [dbo].[trg_glossary_terms_Modify] 
    ON [dbo].[glossary_terms]
    AFTER INSERT,UPDATE
AS
    BEGIN
        UPDATE [glossary_terms]
          SET
              [last_modification_date] = GETDATE()
          WHERE [term_id] IN (SELECT DISTINCT [term_id] FROM [Inserted]);
    END;
GO


ALTER TRIGGER [dbo].[trg_ignored_objects_Modify]
   ON [dbo].[ignored_objects]
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [ignored_objects]
     SET  
         last_modification_date = GETDATE()
     WHERE ignored_object_id IN (SELECT DISTINCT ignored_object_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_licenses_Modify]
   ON [dbo].[licenses] 
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [licenses]
     SET  
         last_modification_date = GETDATE()
     WHERE license_id IN (SELECT DISTINCT license_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_modules_Modify]
   ON [dbo].[modules]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [modules]
     SET  
         last_modification_date = GETDATE()
     WHERE module_id IN (SELECT DISTINCT module_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_parameters_Modify]
   ON  [dbo].[parameters]
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [parameters]
     SET  
         last_modification_date = GETDATE()
     WHERE parameter_id IN (SELECT DISTINCT parameter_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_procedures_Modify]
   ON  [dbo].[procedures] 
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [procedures]
     SET 
        last_modification_date = GETDATE()
     WHERE procedure_id IN (SELECT DISTINCT procedure_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_procedures_modules_Modify]
   ON  [dbo].[procedures_modules]
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [procedures_modules]
     SET  
        last_modification_date = GETDATE()
     WHERE procedure_module_id IN (SELECT DISTINCT procedure_module_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_tables_Modify]
   ON  [dbo].[tables] 
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [tables]
     SET 
        last_modification_date = GETDATE()
     WHERE table_id IN (SELECT DISTINCT table_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_tables_modules_Modify]
   ON  [dbo].[tables_modules] 
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [tables_modules]
     SET 
        last_modification_date = GETDATE()
     WHERE table_module_id IN (SELECT DISTINCT table_module_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_tables_relations_Modify]
   ON  [dbo].[tables_relations] 
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [tables_relations]
     SET 
        last_modification_date = GETDATE()
     WHERE table_relation_id IN (SELECT DISTINCT table_relation_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_tables_relations_cols_Modify]
   ON  [dbo].[tables_relations_columns] 
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [tables_relations_columns]
     SET 
        last_modification_date = GETDATE()
     WHERE table_relation_column_id IN (SELECT DISTINCT table_relation_column_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_triggers_Modify]
   ON  [dbo].[triggers] 
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [triggers]
     SET 
          last_modification_date = GETDATE()
     WHERE trigger_id IN (SELECT DISTINCT trigger_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_unique_constraints_Modify]
   ON  [dbo].[unique_constraints] 
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [unique_constraints]
     SET 
        last_modification_date = GETDATE()
     WHERE unique_constraint_id IN (SELECT DISTINCT unique_constraint_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_unique_constraints_cols_Modify]
   ON  [dbo].[unique_constraints_columns] 
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [unique_constraints_columns]
     SET 
        last_modification_date = GETDATE()
     WHERE unique_constraint_column_id IN (SELECT DISTINCT unique_constraint_column_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_user_connections_Modify]
   ON  [dbo].[user_connections]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [user_connections]
     SET  
        last_modification_date = GETDATE()
     WHERE connection_id IN (SELECT DISTINCT connection_id FROM Inserted)
END
GO


ALTER TRIGGER [dbo].[trg_version_Modify]
   ON  [dbo].[version] 
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [version]
          SET  
             last_modification_date = GETDATE()
     WHERE version_entry_id IN (SELECT DISTINCT version_entry_id FROM Inserted)
END
GO





-- =============================================
-- Author:      Szymon Karpęcki
-- Create date: 29/11/2021
-- Description: Following by default
-- =============================================


IF OBJECT_ID(N'following', N'U') IS NULL
    BEGIN
        CREATE TABLE [following](
            [following_id] int IDENTITY(1,1) NOT NULL,
            [user_id] int NOT NULL,
            [object_type] nvarchar(32) NOT NULL,
            [object_id] int NOT NULL,
            [creation_date] datetime NOT NULL,
            [created_by] nvarchar(1024) NULL,
            [last_modification_date] datetime NOT NULL,
            [modified_by] nvarchar(1024) NULL,
            [source_id] int NULL
        CONSTRAINT [PK_following] PRIMARY KEY CLUSTERED (following_id)
			WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
		CONSTRAINT [FK_following_licenses] FOREIGN KEY (user_id) REFERENCES licenses(license_id)
			ON DELETE CASCADE,

        )
        ON [PRIMARY];

		GRANT DELETE ON [following] TO [users] AS [dbo];
		GRANT INSERT ON [following] TO [users] AS [dbo];
		GRANT SELECT ON [following] TO [users] AS [dbo];
		GRANT UPDATE ON [following] TO [users] AS [dbo];

        CREATE INDEX [IX_following_user_id] ON [following]([user_id]);
        CREATE INDEX [IX_following_object_type_object_id] ON [following]([object_type], [object_id]);
    END;
GO

IF OBJECT_ID(N'DF_following_creation_date', N'D') IS NULL
BEGIN
    ALTER TABLE [following] ADD  CONSTRAINT [DF_following_creation_date]  DEFAULT (getdate()) FOR [creation_date]
END
GO

IF OBJECT_ID(N'DF_following_created_by', N'D') IS NULL
BEGIN
    ALTER TABLE [following] ADD  CONSTRAINT [DF_following_created_by]  DEFAULT (suser_sname()) FOR [created_by]
END
GO

IF OBJECT_ID(N'DF_following_last_modification_date', N'D') IS NULL
BEGIN
    ALTER TABLE [following] ADD  CONSTRAINT [DF_following_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
END
GO

IF OBJECT_ID(N'DF_following_modified_by', N'D') IS NULL
BEGIN
    ALTER TABLE [following] ADD  CONSTRAINT [DF_following_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
END

IF NOT EXISTS
    (
    SELECT TOP 1 1
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[licenses]')
            AND [name] = N'email'
    )
    BEGIN
        ALTER TABLE [licenses]
        ADD 
            [email] nvarchar(100) NULL
    END;
GO


--following databases and business glossaries 
IF NOT EXISTS
    (
        SELECT TOP 1 1
        FROM [following] f
        INNER JOIN [databases] d
            ON f.[object_type] = d.[class]
     )
    BEGIN
        INSERT INTO [following] 
                (
                [object_type] 
                ,[object_id] 
                ,[user_id]
                ,[creation_date]
                ,[last_modification_date]
                ,[created_by] 
                ,[modified_by]
                ) 
        SELECT 
            [class] AS [object_type]
            ,d.[database_id] AS [object_id]
            ,u.[license_id] AS [user_id]
            ,GETDATE() AS [creation_date]
            ,GETDATE() AS [last_modification_date]
            ,N'UPGRADE' as [created_by]
            ,N'UPGRADE' as [modified_by]
        FROM [databases] d 
        INNER JOIN [licenses] u 
            ON d.[created_by] = u.[login] 
    END;
GO

--following glossary entries
IF NOT EXISTS
    (
        SELECT TOP 1 1
        FROM [following] 
        WHERE [object_type] = N'GLOSSARY_ENTRY'
    )
    BEGIN
        INSERT INTO [following] 
                (
                [object_type]
                ,[object_id]
                ,[user_id]
                ,[creation_date]
                ,[last_modification_date] 
                ,[created_by]
                ,[modified_by]
                ) 
        SELECT 
            N'GLOSSARY_ENTRY' as [object_type] 
            ,t.[term_id] AS [object_id]
            ,u.[license_id] AS [user_id]
            ,GETDATE() AS [creation_date]
            ,GETDATE() AS [last_modification_date]
            ,N'UPGRADE' as [created_by]
            ,N'UPGRADE' as [modified_by]
        FROM 
            [glossary_terms] t 
        INNER JOIN [licenses] u 
            ON t.[created_by] = u.[login] 
    END;
GO

--following modules
IF NOT EXISTS
    (
        SELECT TOP 1 1
        FROM [following]
        WHERE [object_type] = N'MODULE'
    )
    BEGIN
        INSERT INTO [following]
                (
                [object_type]
                ,[object_id]
                ,[user_id]
                ,[creation_date]
                ,[last_modification_date]
                ,[created_by] 
                ,[modified_by]
                )
        SELECT 
            N'MODULE' as [object_type]
            ,m.[module_id] as [object_id]
            ,u.[license_id] as [user_id]
            ,GETDATE() AS [creation_date]
            ,GETDATE() AS [last_modification_date]
            ,N'UPGRADE' as [created_by]
            ,N'UPGRADE' as [modified_by]
        FROM 
            [modules] m
        INNER JOIN [licenses] u
            ON m.[created_by] = u.[created_by]
    END;
GO


--following feedbacks
IF NOT EXISTS
    (
        SELECT TOP 1 1
        FROM [following] 
        WHERE [object_type] = N'FEEDBACK'
    )
    BEGIN
        INSERT INTO [following] 
                (
                [object_type]
                ,[object_id]
                ,[user_id]
                ,[creation_date]
                ,[last_modification_date] 
                ,[created_by]
                ,[modified_by]
                )
            SELECT  
                N'FEEDBACK' AS [object_type] 
                ,[feedback_id] AS [object_id]
                ,[license_id] AS [user_id] 
                ,GETDATE() AS [creation_date]
                ,GETDATE() AS [last_modification_date]
                ,N'UPGRADE' as [created_by]
                ,N'UPGRADE' as [modified_by]
            FROM( 
                SELECT 
                    f.[feedback_id] 
                    ,u.[license_id]
                FROM 
                    [feedback] f 
                INNER JOIN [licenses] u 
                    ON f.[user_id] = u.[license_id] 
                UNION
                SELECT 
                    c.[feedback_id] 
                    ,u.[license_id] 
                FROM [feedback_comments] c 
                INNER JOIN [licenses] u 
                    ON c.[user_id] = u.[license_id]
            ) t;
    END;
GO


--following tables objects

IF NOT EXISTS
    (
        SELECT TOP 1 1
        FROM [following] f
        INNER JOIN [tables] t
            ON f.[object_type] = t.[object_type]
    )
    BEGIN
        INSERT INTO [following]
                (
                    [object_type]
                    ,[object_id]
                    ,[user_id]
                    ,[creation_date]
                    ,[last_modification_date]
                    ,[created_by]
                    ,[modified_by]
                )
                SELECT 
                    DISTINCT --DISTINCT in case of few licenses for one login
                    t.[object_type]
                    ,t.[table_id] AS [object_id]
                    ,u.[license_id] AS [user_id]
                    ,GETDATE() AS [creation_date]
                    ,GETDATE() AS [last_modification_date]
                    ,N'UPGRADE' as [created_by]
                    ,N'UPGRADE' as [modified_by]
                FROM 
                    [tables] t
                INNER JOIN [licenses] u
                    ON t.[created_by] = u.[login]
                WHERE t.[source] = N'user'
    END;
GO


--following procedures/functions objects

IF NOT EXISTS
    (
        SELECT TOP 1 1
        FROM [following] f
        INNER JOIN [procedures] p
            ON f.[object_type] = p.[object_type]
    )
    BEGIN
        INSERT INTO [following]
                (
                    [object_type]
                    ,[object_id]
                    ,[user_id]
                    ,[creation_date]
                    ,[last_modification_date]
                    ,[created_by]
                    ,[modified_by]
                )
                SELECT 
                    DISTINCT 
                    p.[object_type]
                    ,p.[procedure_id] AS [object_id]
                    ,u.[license_id] AS [user_id]
                    ,GETDATE() AS [creation_date]
                    ,GETDATE() AS [last_modification_date]
                    ,N'UPGRADE' as [created_by]
                    ,N'UPGRADE' as [modified_by]
                FROM 
                    [procedures] p
                INNER JOIN [licenses] u
                    ON p.[created_by] = u.[login]
                WHERE p.[source] = N'user'         
    END;
GO


--following columns

IF NOT EXISTS
    (
        SELECT TOP 1 1
        FROM [following] f
        WHERE f.[object_type] = N'COLUMN'
    )
    BEGIN
        INSERT INTO [following]
                (
                    [object_type]
                    ,[object_id]
                    ,[user_id]
                    ,[creation_date]
                    ,[last_modification_date]
                    ,[created_by]
                    ,[modified_by]
                )
                SELECT 
                    DISTINCT
                    N'COLUMN' AS [object_type]
                    ,c.[column_id] AS [object_id]
                    ,u.[license_id] AS [user_id]
                    ,GETDATE() AS [creation_date]
                    ,GETDATE() AS [last_modification_date]
                    ,N'UPGRADE' as [created_by]
                    ,N'UPGRADE' as [modified_by]
                FROM [columns] c
                INNER JOIN [licenses] u
                    ON c.[created_by] = u.[login]
                WHERE c.[source] = N'user'
    END;
GO


--following parameters

IF NOT EXISTS
    (
        SELECT TOP 1 1 
        FROM [following] f
        WHERE f.[object_type] = N'PARAMETER'
    )
    BEGIN
        INSERT INTO [following]
                (
                    [object_type]
                    ,[object_id]
                    ,[user_id]
                    ,[creation_date]
                    ,[last_modification_date]
                    ,[created_by]
                    ,[modified_by]
                )
                SELECT 
                    DISTINCT
                    N'PARAMETER' AS [object_type]
                    ,p.[parameter_id] AS [object_id]
                    ,u.[license_id] AS [user_id]
                    ,GETDATE() AS [creation_date]
                    ,GETDATE() AS [last_modification_date]
                    ,N'UPGRADE' as [created_by]
                    ,N'UPGRADE' as [modified_by]
                FROM [parameters] p
                INNER JOIN [licenses] u
                    ON p.[created_by] = u.[login]
                WHERE p.source = N'user'
    END;
GO



-- =============================================
-- Author:		Szymon Karpęcki
-- Create date: 29/11/2021
-- Description:	Updates last_modification_date columns on insert or update
-- =============================================

IF NOT EXISTS
    (
    SELECT TOP 1 1
    FROM [sys].[triggers]
    WHERE [name] = N'trg_following_Modify'
    )
BEGIN
    EXEC dbo.sp_executesql @statement = N'
        CREATE TRIGGER [dbo].[trg_following_Modify]
           ON  [dbo].[following] 
           AFTER INSERT,UPDATE
        AS 
        BEGIN
             UPDATE [following]
             SET  
                 [last_modification_date] = GETDATE()
             WHERE [following_id] IN (SELECT DISTINCT [following_id] FROM Inserted)
        END
    '
END

ALTER TABLE [dbo].[following] ENABLE TRIGGER [trg_following_Modify]
GO




-- =============================================
-- Author:      Michał Psyk
-- Create date: 26/11/2021
-- Description: Columns for database params
-- =============================================

IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[databases]')
            AND [name] = 'param1'
    )
    BEGIN
        ALTER TABLE [databases]
        ADD [param1] nvarchar(max) NULL;
    END;
GO

IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[databases]')
            AND [name] = 'param2'
    )
    BEGIN
        ALTER TABLE [databases]
        ADD [param2] nvarchar(max) NULL;
    END;
GO

IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[databases]')
            AND [name] = 'param3'
    )
    BEGIN
        ALTER TABLE [databases]
        ADD [param3] nvarchar(max) NULL;
    END;
GO

IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[databases]')
            AND [name] = 'param4'
    )
    BEGIN
        ALTER TABLE [databases]
        ADD [param4] nvarchar(max) NULL;
    END;
GO

IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[databases]')
            AND [name] = 'param5'
    )
    BEGIN
        ALTER TABLE [databases]
        ADD [param5] nvarchar(max) NULL;
    END;
GO

IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[databases]')
            AND [name] = 'param6'
    )
    BEGIN
        ALTER TABLE [databases]
        ADD [param6] nvarchar(max) NULL;
    END;
GO

IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[databases]')
            AND [name] = 'param7'
    )
    BEGIN
        ALTER TABLE [databases]
        ADD [param7] nvarchar(max) NULL;
    END;
GO

IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[databases]')
            AND [name] = 'param8'
    )
    BEGIN
        ALTER TABLE [databases]
        ADD [param8] nvarchar(max) NULL;
    END;
GO

IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[databases]')
            AND [name] = 'param9'
    )
    BEGIN
        ALTER TABLE [databases]
        ADD [param9] nvarchar(max) NULL;
    END;
GO

IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[databases]')
            AND [name] = 'param10'
    )
    BEGIN
        ALTER TABLE [databases]
        ADD [param10] nvarchar(max) NULL;
    END;
GO


-- =============================================
-- Author:		Szymon Karpęcki
-- Create date: 01/12/2021
-- Description:	Enable SCT Triggers
-- =============================================

ALTER TABLE [columns] ENABLE TRIGGER [trg_columns_change_track_insert];
ALTER TABLE [columns] ENABLE TRIGGER [trg_columns_change_track_update];

ALTER TABLE [parameters] ENABLE TRIGGER [trg_parameters_change_track_insert];
ALTER TABLE [parameters] ENABLE TRIGGER [trg_parameters_change_track_update];

ALTER TABLE [procedures] ENABLE TRIGGER [trg_procedures_change_track_insert];
ALTER TABLE [procedures] ENABLE TRIGGER [trg_procedures_change_track_update];

ALTER TABLE [tables] ENABLE TRIGGER [trg_tables_change_track_insert];
ALTER TABLE [tables] ENABLE TRIGGER [trg_tables_change_track_update];

ALTER TABLE [triggers] ENABLE TRIGGER [trg_triggers_change_track_insert];
ALTER TABLE [triggers] ENABLE TRIGGER [trg_triggers_change_track_update];


UPDATE [version]
  SET 
      [stable] = 1
  WHERE [version] = 10
        AND [update] = 1
        AND [release] = 0;
GO