-- =============================================
-- Author:      Szymon Karpęcki
-- Create date: 12/8/2021
-- Description: Script upgrading repository to Dataedo 10.0
-- =============================================

-- Version
IF
  (
   SELECT COUNT(*)
     FROM [version]
     WHERE [version] = 10
           AND [update] = 0
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
         0,
         0,
         0
        );
    END;
GO



-- =============================================
-- Author:      Szymon Karpęcki
-- Create date: 12/8/2021
-- Description: Basic stats
-- =============================================


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[tables]')
            AND [name] = 'stats_source'
    )
    BEGIN
        ALTER TABLE [tables]
        ADD [stats_source] varchar(10) NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[tables]')
            AND [name] = 'stats_row_count'
    )
    BEGIN
        ALTER TABLE [tables]
        ADD [stats_row_count] bigint NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[tables]')
            AND [name] = 'stats_datetime'
    )
    BEGIN
        ALTER TABLE [tables]
        ADD [stats_datetime] datetime NULL;
    END;
GO


IF OBJECT_ID(N'tables_stats', N'U') IS NULL
	BEGIN
		CREATE TABLE [tables_stats](
				[table_stat_id] int IDENTITY(1,1) NOT NULL,
				[table_id] int NOT NULL,
				[stats_source] varchar(10) NULL,
				[row_count] bigint NULL,
				[stats_datetime] datetime NULL,
				[creation_date] datetime NOT NULL,
				[created_by] nvarchar(1024) NULL,
				[last_modification_date] datetime NOT NULL,
				[modified_by] nvarchar(1024) NULL,
				[source_id] int NULL,
			CONSTRAINT [PK_tables_stats] PRIMARY KEY CLUSTERED (table_stat_id)
			WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
			CONSTRAINT [FK_tables_stats_tables] FOREIGN KEY (table_id) REFERENCES tables(table_id) 
			ON DELETE CASCADE
		)
		ON [PRIMARY];
	END;

ALTER TABLE [tables_stats] ADD  CONSTRAINT [DF_tables_stats_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO

ALTER TABLE [tables_stats] ADD  CONSTRAINT [DF_tables_stats_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO

ALTER TABLE [tables_stats] ADD  CONSTRAINT [DF_tables_stats_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO

ALTER TABLE [tables_stats] ADD  CONSTRAINT [DF_tables_stats_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO

GO
GRANT DELETE ON [tables_stats] TO [users] AS [dbo];
GO
GRANT INSERT ON [tables_stats] TO [users] AS [dbo];
GO
GRANT SELECT ON [tables_stats] TO [users] AS [dbo];
GO
GRANT UPDATE ON [tables_stats] TO [users] AS [dbo];
GO

IF NOT EXISTS
    (
    SELECT * 
    FROM [sys].[indexes]
    WHERE [name] = 'IX_tables_stats_table_id'
        AND [object_id] = OBJECT_ID(N'tables_stats')
    )
    BEGIN
        CREATE INDEX [IX_tables_stats_table_id] ON [tables_stats]([table_id])
    END;
GO

-- =============================================
-- Author:      Szymon Karpęcki
-- Create date: 13/8/2021
-- Description: Profiling
-- =============================================


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'profiling_date'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [profiling_date] datetime NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'profiling_data_type'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [profiling_data_type] varchar(20) NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'row_count'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [row_count] bigint NULL;
    END;
GO


IF NOT EXISTS
	(
	SELECT *
	FROM [sys].[columns]
	WHERE [object_id] = OBJECT_ID(N'[columns]')
			AND [name] = 'values_null_row_count'
	)
    BEGIN
        ALTER TABLE [columns]
        ADD [values_null_row_count] bigint NULL;
    END;
GO


IF NOT EXISTS
	(
	SELECT *
	FROM [sys].[columns]
	WHERE [object_id] = OBJECT_ID(N'[columns]')
			AND [name] = 'values_empty_row_count'
	)
    BEGIN
        ALTER TABLE [columns]
        ADD [values_empty_row_count] bigint NULL;
    END;
GO


IF NOT EXISTS
	(
	SELECT *
	FROM [sys].[columns]
	WHERE [object_id] = OBJECT_ID(N'[columns]')
			AND [name] = 'values_distinct_row_count'
	)
    BEGIN
        ALTER TABLE [columns]
        ADD [values_distinct_row_count] bigint NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'values_nondistinct_row_count'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [values_nondistinct_row_count] bigint NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'values_default_row_count'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [values_default_row_count] bigint NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'values_unique_values'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [values_unique_values] bigint NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'value_min'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [value_min] float NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'value_max'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [value_max] float NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'value_avg'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [value_avg] float NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'value_stddev'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [value_stddev] float NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'value_var'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [value_var] float NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'value_min_string'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [value_min_string] varchar(250) NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'value_max_string'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [value_max_string] varchar(250) NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'value_min_date'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [value_min_date] datetime NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'value_max_date'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [value_max_date] datetime NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'string_length_min'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [string_length_min] bigint NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'string_length_max'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [string_length_max] bigint NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'string_length_avg'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [string_length_avg] float NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'string_length_stddev'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [string_length_stddev] float NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'string_length_var'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [string_length_var] float NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'values_list_mode'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [values_list_mode] char NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'values_list_rows_count'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [values_list_rows_count] bigint default 10 NULL;
    END;
GO


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[columns]')
            AND [name] = 'refresh_at_import'
    )
    BEGIN
        ALTER TABLE [columns]
        ADD [refresh_at_import] bit default 0 NOT NULL;
    END;
GO


IF OBJECT_ID(N'column_values', N'U') IS NULL
	BEGIN
		CREATE TABLE [column_values](
			 [value_id] int IDENTITY(1,1) NOT NULL,
			 [column_id] int NOT NULL,
			 [value] varchar(250) NULL,
			 [type] char NULL,
			 [row_count] bigint NULL,
			 [creation_date] datetime NOT NULL,
			 [created_by] nvarchar(1024) NULL,
			 [last_modification_date] datetime NOT NULL,
			 [modified_by] nvarchar(1024) NULL,
			 [source_id] int NULL,
		 CONSTRAINT [PK_column_values] PRIMARY KEY CLUSTERED (value_id)
			WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
		 CONSTRAINT [FK_column_values_columns] FOREIGN KEY (column_id) REFERENCES columns(column_id)
			ON DELETE CASCADE
		)
		ON [PRIMARY];
	END;

ALTER TABLE [column_values] ADD  CONSTRAINT [DF_column_values_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO

ALTER TABLE [column_values] ADD  CONSTRAINT [DF_column_values_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO

ALTER TABLE [column_values] ADD  CONSTRAINT [DF_column_values_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO

ALTER TABLE [column_values] ADD  CONSTRAINT [DF_column_values_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO

GO
GRANT DELETE ON [column_values] TO [users] AS [dbo];
GO
GRANT INSERT ON [column_values] TO [users] AS [dbo];
GO
GRANT SELECT ON [column_values] TO [users] AS [dbo];
GO
GRANT UPDATE ON [column_values] TO [users] AS [dbo];
GO

IF NOT EXISTS
    (
    SELECT * 
    FROM [sys].[indexes]
    WHERE [name] = 'IX_column_values_column_id'
        AND [object_id] = OBJECT_ID(N'column_values')
    )
    BEGIN
        CREATE INDEX [IX_column_values_column_id] ON [column_values]([column_id])
    END;
GO

IF NOT EXISTS
    (
    SELECT * 
    FROM [sys].[indexes]
    WHERE [name] = 'IX_column_values_row_count_desc'
        AND [object_id] = OBJECT_ID(N'column_values')
    )
    BEGIN
        CREATE INDEX [IX_column_values_row_count_desc] ON [column_values]([row_count] desc)
    END;
GO

-- =============================================
-- Author:      Szymon Karpęcki
-- Create date: 13/8/2021
-- Description: Killer switch
-- =============================================


IF OBJECT_ID(N'configuration', N'U') IS NULL
	BEGIN
		CREATE TABLE [configuration](
			 [key] varchar(100) NOT NULL,
			 [value] varchar(100) NULL,
			 [creation_date] datetime NOT NULL,
			 [created_by] nvarchar(1024) NULL
		 CONSTRAINT [PK_configuration] PRIMARY KEY CLUSTERED ([key])
			WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		)
        ON [PRIMARY];
	END;

ALTER TABLE [configuration] ADD  CONSTRAINT [DF_configuration_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO

ALTER TABLE [configuration] ADD  CONSTRAINT [DF_configuration_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO


GO
GRANT DELETE ON [configuration] TO [users] AS [dbo];
GO
GRANT INSERT ON [configuration] TO [users] AS [dbo];
GO
GRANT SELECT ON [configuration] TO [users] AS [dbo];
GO
GRANT UPDATE ON [configuration] TO [users] AS [dbo];
GO

IF NOT EXISTS
    (
    SELECT * FROM [configuration]
    WHERE [key] = 'DATA_PROFILING'
    )
    BEGIN
        INSERT INTO configuration ([key], 
                                   [value], 
                                   [creation_date], 
                                   [created_by])
        SELECT 
            'DATA_PROFILING', 
            'NO_SAVE',
            GETDATE(),
            NULL;
    END;
GO

-- =============================================
-- Author:      Szymon Karpęcki
-- Create date: 16/8/2021
-- Description: Users, licenses and sessions
-- =============================================


IF OBJECT_ID(N'sessions', N'U') IS NULL
	BEGIN
		CREATE TABLE [sessions](
			 [session_id] int IDENTITY(1,1) NOT NULL,
			 [login] nvarchar(1024) NULL,
			 [user_id] int NULL,
			 [datetime] datetime NOT NULL,
			 [authentication] nvarchar(50) NULL,
			 [status] nvarchar(50) NULL,
			 [registration_type] nvarchar(50) NULL,
			 [license_type] nvarchar(50) NULL,
			 [remote_license_id] nvarchar(50) NULL,
			 [package_code] nvarchar(100) NULL,
			 [package_name] nvarchar(100) NULL,
			 [license_start] date NULL,
			 [license_end] date NULL,
			 [account_id] int NULL,
			 [account_name] nvarchar(100) NULL,
			 [product] nvarchar(100) NULL,
			 [product_version] nvarchar(100) NULL,
			 [host] nvarchar(1024) NULL,
			 [ip] nvarchar(50) NULL,
			 [user_agent] nvarchar(MAX) NULL,
			 [creation_date] datetime NOT NULL,
			 [created_by] nvarchar(1024) NULL,
			 [last_modification_date] datetime NOT NULL,
			 [modified_by] nvarchar(1024) NULL,
			 [source_id] int NULL
		 CONSTRAINT [PK_sessions] PRIMARY KEY CLUSTERED (session_id)
			WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		)
		ON [PRIMARY];
	END;

ALTER TABLE [sessions] ADD  CONSTRAINT [DF_sessions_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO

ALTER TABLE [sessions] ADD  CONSTRAINT [DF_sessions_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO

ALTER TABLE [sessions] ADD  CONSTRAINT [DF_sessions_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO

ALTER TABLE [sessions] ADD  CONSTRAINT [DF_sessions_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO

GO
GRANT DELETE ON [sessions] TO [users] AS [dbo];
GO
GRANT INSERT ON [sessions] TO [users] AS [dbo];
GO
GRANT SELECT ON [sessions] TO [users] AS [dbo];
GO
GRANT UPDATE ON [sessions] TO [users] AS [dbo];
GO

IF NOT EXISTS
    (
    SELECT * 
    FROM [sys].[indexes]
    WHERE [name] = 'IX_sessions_login'
        AND [object_id] = OBJECT_ID(N'sessions')
    )
    BEGIN
        CREATE INDEX [IX_sessions_login] ON [sessions]([login])
    END;
GO

IF NOT EXISTS
    (
    SELECT * 
    FROM [sys].[indexes]
    WHERE [name] = 'IX_sessions_user_id'
        AND [object_id] = OBJECT_ID(N'sessions')
    )
    BEGIN
        CREATE INDEX [IX_sessions_user_id] ON [sessions]([user_id])
    END;
GO

-- =============================================
-- Author:      Szymon Karpęcki
-- Create date: 16/8/2021
-- Description: Community fixes
-- =============================================


/** Removing open_questions_count and questions_count **/

--columns table
IF EXISTS 
    (
	SELECT * 
	FROM [sys].[default_constraints]
	WHERE OBJECT_NAME(parent_object_id) = 'columns'
		AND [name] = 'DF_columns_open_questions_count'
    )
    BEGIN
        ALTER TABLE [columns]
        DROP CONSTRAINT [DF_columns_open_questions_count]
    END;
GO

IF EXISTS 
    (
	SELECT *
	FROM [sys].[indexes]
	WHERE OBJECT_NAME(object_id) = 'columns'
		AND [name] = 'IX_open_questions_count'
    )
    BEGIN
        DROP INDEX [columns].[IX_open_questions_count]
    END;
GO

IF EXISTS  
    (
    SELECT *
    FROM [sys].[columns]
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'open_questions_count'
    )
    BEGIN
        ALTER TABLE [columns]
        DROP COLUMN [open_questions_count]
    END;
GO


IF EXISTS 
    (
	SELECT * 
	FROM [sys].[default_constraints]
	WHERE OBJECT_NAME(parent_object_id) = 'columns'
		AND [name] = 'DF_columns_questions_count'
    )
    BEGIN
        ALTER TABLE [columns]
        DROP CONSTRAINT [DF_columns_questions_count]
    END;
GO

IF EXISTS 
    (
    SELECT *
    FROM [sys].[columns]
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'questions_count'
    )
    BEGIN
        ALTER TABLE [columns]
        DROP COLUMN [questions_count]
    END;
GO



--databases table
IF EXISTS 
    (
	SELECT * 
	FROM [sys].[default_constraints]
	WHERE OBJECT_NAME(parent_object_id) = 'databases'
		AND [name] = 'DF_databases_open_questions_count'
    )
    BEGIN
        ALTER TABLE [databases]
        DROP CONSTRAINT [DF_databases_open_questions_count]
    END;
GO

IF EXISTS 
    (
	SELECT *
	FROM [sys].[indexes]
	WHERE OBJECT_NAME(object_id) = 'databases'
		AND [name] = 'IX_open_questions_count'
    )
    BEGIN
        DROP INDEX [databases].[IX_open_questions_count]
    END;
GO

IF EXISTS 
    (
    SELECT *
    FROM [sys].[columns]
    WHERE object_id = OBJECT_ID(N'[databases]')
	   AND name = 'open_questions_count'
    )
    BEGIN
        ALTER TABLE [databases]
        DROP COLUMN [open_questions_count]
    END;
GO


IF EXISTS 
    (
	SELECT * 
	FROM [sys].[default_constraints]
	WHERE OBJECT_NAME(parent_object_id) = 'databases'
		AND [name] = 'DF_databases_questions_count'
    )
    BEGIN
        ALTER TABLE [databases]
        DROP CONSTRAINT [DF_databases_questions_count]
    END;
GO

IF EXISTS 
    (
        SELECT *
        FROM [sys].[columns]
        WHERE object_id = OBJECT_ID(N'[databases]')
        AND name = 'questions_count'
    )
    BEGIN
        ALTER TABLE [databases]
        DROP COLUMN [questions_count]
    END;
GO


--glossary_terms table
IF EXISTS 
    (
	SELECT * 
	FROM [sys].[default_constraints]
	WHERE OBJECT_NAME(parent_object_id) = 'glossary_terms'
		AND [name] = 'DF_glossary_terms_open_questions_count'
    )
    BEGIN
        ALTER TABLE [glossary_terms]
        DROP CONSTRAINT [DF_glossary_terms_open_questions_count]
    END;
GO

IF EXISTS 
    (
	SELECT *
	FROM [sys].[indexes]
	WHERE OBJECT_NAME(object_id) = 'glossary_terms'
		AND [name] = 'IX_open_questions_count'
    )
    BEGIN
        DROP INDEX [glossary_terms].[IX_open_questions_count]
    END;
GO

IF EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE object_id = OBJECT_ID(N'[glossary_terms]')
	   AND name = 'open_questions_count'
    )
    BEGIN
        ALTER TABLE [glossary_terms]
        DROP COLUMN [open_questions_count]
    END;
GO


IF EXISTS 
    (
	SELECT * 
	FROM [sys].[default_constraints]
	WHERE OBJECT_NAME(parent_object_id) = 'glossary_terms'
		AND [name] = 'DF_glossary_terms_questions_count'
    )
    BEGIN
        ALTER TABLE [glossary_terms]
        DROP CONSTRAINT [DF_glossary_terms_questions_count]
    END;
GO

IF EXISTS 
    (
    SELECT *
    FROM [sys].[columns]
    WHERE object_id = OBJECT_ID(N'[glossary_terms]')
	   AND name = 'questions_count'
    )
    BEGIN
        ALTER TABLE [glossary_terms]
        DROP COLUMN [questions_count]
    END;
GO


--modules table
IF EXISTS 
    (
	SELECT * 
	FROM [sys].[default_constraints]
	WHERE OBJECT_NAME(parent_object_id) = 'modules'
		AND [name] = 'DF_modules_open_questions_count'
    )
    BEGIN
        ALTER TABLE [modules]
        DROP CONSTRAINT [DF_modules_open_questions_count]
    END;
GO

IF EXISTS 
    (
	SELECT *
	FROM [sys].[indexes]
	WHERE OBJECT_NAME(object_id) = 'modules'
		AND [name] = 'IX_open_questions_count'
    )
    BEGIN
        DROP INDEX [modules].[IX_open_questions_count]
    END;
GO

IF EXISTS 
    (
    SELECT *
    FROM [sys].[columns]
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'open_questions_count'
    )
    BEGIN
        ALTER TABLE [modules]
        DROP COLUMN [open_questions_count]
    END;
GO


IF EXISTS 
    (
	SELECT * 
	FROM [sys].[default_constraints]
	WHERE OBJECT_NAME(parent_object_id) = 'modules'
		AND [name] = 'DF_modules_questions_count'
    )
    BEGIN
        ALTER TABLE [modules]
        DROP CONSTRAINT [DF_modules_questions_count]
    END;
GO

IF EXISTS 
    (
    SELECT *
    FROM [sys].[columns]
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'questions_count'
    )
    BEGIN
        ALTER TABLE [modules]
        DROP COLUMN [questions_count]
    END;
GO


--parameters table
IF EXISTS 
    (
	SELECT * 
	FROM [sys].[default_constraints]
	WHERE OBJECT_NAME(parent_object_id) = 'parameters'
		AND [name] = 'DF_parameters_open_questions_count'
    )
    BEGIN
        ALTER TABLE [parameters]
        DROP CONSTRAINT [DF_parameters_open_questions_count]
    END;
GO

IF EXISTS 
    (
	SELECT *
	FROM [sys].[indexes]
	WHERE OBJECT_NAME(object_id) = 'parameters'
		AND [name] = 'IX_open_questions_count'
    )
    BEGIN
        DROP INDEX [parameters].[IX_open_questions_count]
    END;
GO

IF EXISTS 
    (
    SELECT *
    FROM [sys].[columns]
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'open_questions_count'
    )
    BEGIN
        ALTER TABLE [parameters]
        DROP COLUMN [open_questions_count]
    END;
GO


IF EXISTS 
    (
	SELECT * 
	FROM [sys].[default_constraints]
	WHERE OBJECT_NAME(parent_object_id) = 'parameters'
		AND [name] = 'DF_parameters_questions_count'
    )
    BEGIN
        ALTER TABLE [parameters]
        DROP CONSTRAINT [DF_parameters_questions_count]
    END;
GO

IF EXISTS 
    (
    SELECT *
    FROM [sys].[columns]
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'questions_count'
    )
    BEGIN
        ALTER TABLE [parameters]
        DROP COLUMN [questions_count]
    END;
GO



--procedures table
IF EXISTS 
    (
	SELECT * 
	FROM [sys].[default_constraints]
	WHERE OBJECT_NAME(parent_object_id) = 'procedures'
		AND [name] = 'DF_procedures_open_questions_count'
    )
    BEGIN
        ALTER TABLE [procedures]
        DROP CONSTRAINT [DF_procedures_open_questions_count]
    END;
GO

IF EXISTS 
    (
	SELECT *
	FROM [sys].[indexes]
	WHERE OBJECT_NAME(object_id) = 'procedures'
		AND [name] = 'IX_open_questions_count'
    )
    BEGIN
        DROP INDEX [procedures].[IX_open_questions_count]
    END;
GO

IF EXISTS 
    (
    SELECT *
    FROM [sys].[columns]
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'open_questions_count'
    )
    BEGIN
        ALTER TABLE [procedures]
        DROP COLUMN [open_questions_count]
    END;
GO


IF EXISTS 
    (
	SELECT * 
	FROM [sys].[default_constraints]
	WHERE OBJECT_NAME(parent_object_id) = 'procedures'
		AND [name] = 'DF_procedures_questions_count'
    )
    BEGIN
        ALTER TABLE [procedures]
        DROP CONSTRAINT [DF_procedures_questions_count]
    END;
GO

IF EXISTS 
    (
    SELECT *
    FROM [sys].[columns]
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'questions_count'
    )
    BEGIN
        ALTER TABLE [procedures]
        DROP COLUMN [questions_count]
    END;
GO


--tables table
IF EXISTS 
    (
	SELECT * 
	FROM [sys].[default_constraints]
	WHERE OBJECT_NAME(parent_object_id) = 'tables'
		AND [name] = 'DF_tables_open_questions_count'
    )
    BEGIN
        ALTER TABLE [tables]
        DROP CONSTRAINT [DF_tables_open_questions_count]
    END;
GO

IF EXISTS 
    (
	SELECT *
	FROM [sys].[indexes]
	WHERE OBJECT_NAME(object_id) = 'tables'
		AND [name] = 'IX_open_questions_count'
    )
    BEGIN
        DROP INDEX [tables].[IX_open_questions_count]
    END;
GO

IF EXISTS 
    (
    SELECT *
    FROM [sys].[columns]
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'open_questions_count'
    )
    BEGIN
        ALTER TABLE [tables]
        DROP COLUMN [open_questions_count]
    END;
GO


IF EXISTS 
    (
	SELECT * 
	FROM [sys].[default_constraints]
	WHERE OBJECT_NAME(parent_object_id) = 'tables'
		AND [name] = 'DF_tables_questions_count'
    )
    BEGIN
        ALTER TABLE [tables]
        DROP CONSTRAINT [DF_tables_questions_count]
    END;
GO

IF EXISTS 
    (
    SELECT *
    FROM [sys].[columns]
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'questions_count'
    )
    BEGIN
        ALTER TABLE [tables]
        DROP COLUMN [questions_count]
    END;
GO


--triggers table
IF EXISTS 
    (
	SELECT * 
	FROM [sys].[default_constraints]
	WHERE OBJECT_NAME(parent_object_id) = 'triggers'
		AND [name] = 'DF_triggers_open_questions_count'
    )
    BEGIN
        ALTER TABLE [triggers]
        DROP CONSTRAINT [DF_triggers_open_questions_count]
    END;
GO

IF EXISTS 
    (
	SELECT *
	FROM [sys].[indexes]
	WHERE OBJECT_NAME(object_id) = 'triggers'
		AND [name] = 'IX_open_questions_count'
    )
    BEGIN
        DROP INDEX [triggers].[IX_open_questions_count]
    END;
GO

IF EXISTS 
    (
    SELECT *
    FROM [sys].[columns]
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'open_questions_count'
    )
    BEGIN
        ALTER TABLE [triggers]
        DROP COLUMN [open_questions_count]
    END;
GO


IF EXISTS 
    (
	SELECT * 
	FROM [sys].[default_constraints]
	WHERE OBJECT_NAME(parent_object_id) = 'triggers'
		AND [name] = 'DF_triggers_questions_count'
    )
    BEGIN
        ALTER TABLE [triggers]
        DROP CONSTRAINT [DF_triggers_questions_count]
    END;
GO

IF EXISTS 
    (
    SELECT *
    FROM [sys].[columns]
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'questions_count'
    )
    BEGIN
        ALTER TABLE [triggers]
        DROP COLUMN [questions_count]
    END;
GO

/** Updating thread types **/
UPDATE [feedback]
SET [type] = 'COMMENT'
WHERE [type] IN ('QUESTION', 'TODO')


-- =============================================
-- Author:      Szymon Karpęcki
-- Create date: 2/9/2021
-- Description: SCT Triggers enabled by deafult 
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

GO


-- =============================================
-- Author:      Szymon Karpęcki
-- Create date: 13/9/2021
-- Description: Column for SSAS perspective 
-- =============================================


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[databases]')
            AND [name] = 'perspective_name'
    )
    BEGIN
        ALTER TABLE [databases]
        ADD [perspective_name] nvarchar(250) NULL;
    END;
GO

-- =============================================
-- Author:      Szymon Karpęcki
-- Create date: 13/9/2021
-- Description: Column for SSAS perspective 
-- =============================================


IF NOT EXISTS
    (
    SELECT *
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[databases]')
            AND [name] = 'perspective_name'
    )
    BEGIN
        ALTER TABLE [databases]
        ADD [perspective_name] nvarchar(250) NULL;
    END;
GO


-- =============================================
-- Author:      Szymon Karpęcki
-- Create date: 23/9/2021
-- Description: Updates for licenses fix. 
-- =============================================

IF NOT EXISTS
    (
    SELECT 1
    FROM [sys].[columns]
    WHERE [object_id] = OBJECT_ID(N'[licenses]')
            AND [name] = 'is_offline'
    )
    BEGIN
        ALTER TABLE [licenses]
        ADD [is_offline] BIT NULL
    END;
GO

IF (SELECT is_nullable 
    FROM [sys].[columns] 
    WHERE [object_id] = object_id(N'licenses') 
	AND [name] = N'key') 
    = 
    0
    BEGIN
        ALTER TABLE [licenses]
        ALTER COLUMN [key]
        varchar(256) NULL
    END;

UPDATE [version]
  SET 
      [stable] = 1
  WHERE [version] = 10
        AND [update] = 0
        AND [release] = 0;
GO