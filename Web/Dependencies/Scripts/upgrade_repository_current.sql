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

ALTER TABLE [data_processes] ADD  CONSTRAINT [DF_data_processes_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO

ALTER TABLE [data_processes] ADD  CONSTRAINT [DF_data_processes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO

ALTER TABLE [data_processes] ADD  CONSTRAINT [DF_data_processes_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO

ALTER TABLE [data_processes] ADD  CONSTRAINT [DF_data_processes_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
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

ALTER TABLE [data_flows] ADD  CONSTRAINT [DF_data_flows_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO

ALTER TABLE [data_flows] ADD  CONSTRAINT [DF_data_flows_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO

ALTER TABLE [data_flows] ADD  CONSTRAINT [DF_data_flows_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO

ALTER TABLE [data_flows] ADD  CONSTRAINT [DF_data_flows_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
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