DECLARE @id AS UNIQUEIDENTIFIER = '5ee9d46e-b942-4929-8949-6effd37cf623'
DECLARE @version as int = 10 
DECLARE @update as int = 3
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-3737'

IF NOT EXISTS (SELECT [id] FROM [database_update_log] WHERE [id] = @id)
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
  -- SQL script here. WITOUT key word 'GO'!
  /****** Object: Table [import_tables] ******/
	IF OBJECT_ID(N'data_columns_flows', N'U') IS NULL
	BEGIN
		CREATE TABLE [data_columns_flows](
		[data_columns_flow_id] [int] IDENTITY(1,1) NOT NULL,
		[inflow_id] [int] NOT NULL,
		[inflow_column_id] [int] NOT NULL,
		[outflow_id] [int] NOT NULL,
		[outflow_column_id] [int] NOT NULL,
		[source] [nvarchar](50) NULL,
		[created_by] [nvarchar](1024) NULL,
		[creation_date] [datetime] NOT NULL,
		[modified_by] [nvarchar](1024) NULL,
		[last_modification_date] [datetime] NOT NULL
		CONSTRAINT [PK_data_columns_flow_id] PRIMARY KEY CLUSTERED 
		(
			[data_columns_flow_id] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
		CONSTRAINT [FK_data_columns_flows_data_flows_inflow] FOREIGN KEY (inflow_id) REFERENCES data_flows(flow_id)
			ON DELETE NO ACTION ON UPDATE NO ACTION,
		CONSTRAINT [FK_data_columns_flows_data_flows_outflow] FOREIGN KEY (outflow_id) REFERENCES data_flows(flow_id)
			ON DELETE NO ACTION ON UPDATE NO ACTION,
		) ON [PRIMARY]
	END

	IF OBJECT_ID('dbo.[DF_data_columns_flows_creation_date]') IS NULL
		ALTER TABLE [data_columns_flows] ADD  CONSTRAINT [DF_data_columns_flows_creation_date] DEFAULT (getdate()) FOR [creation_date]
	IF OBJECT_ID('dbo.[DF_data_columns_flows_created_by]') IS NULL
		ALTER TABLE [data_columns_flows] ADD  CONSTRAINT [DF_data_columns_flows_created_by] DEFAULT (suser_sname()) FOR [created_by]
	IF OBJECT_ID('dbo.[DF_data_columns_flows_modified_by]') IS NULL
		ALTER TABLE [data_columns_flows] ADD  CONSTRAINT [DF_data_columns_flows_modified_by] DEFAULT (suser_sname()) FOR [modified_by]
	IF OBJECT_ID('dbo.[DF_data_columns_flows_last_modification_date]') IS NULL
		ALTER TABLE [data_columns_flows] ADD  CONSTRAINT [DF_data_columns_flows_last_modification_date] DEFAULT (getdate()) FOR [last_modification_date]

	ALTER TABLE [data_columns_flows] NOCHECK CONSTRAINT [FK_data_columns_flows_data_flows_inflow]
	ALTER TABLE [data_columns_flows] NOCHECK CONSTRAINT [FK_data_columns_flows_data_flows_outflow]

	GRANT DELETE ON [data_columns_flows] TO [users] AS [dbo]
	GRANT INSERT ON [data_columns_flows] TO [users] AS [dbo]
	GRANT SELECT ON [data_columns_flows] TO [users] AS [dbo]
	GRANT UPDATE ON [data_columns_flows] TO [users] AS [dbo]

	IF OBJECT_ID(N'trg_data_flows_delete_flow') IS NULL
	BEGIN
		EXEC('CREATE TRIGGER [trg_data_flows_delete_flow]
		   ON [data_flows]
		   FOR DELETE
		AS
		BEGIN
		 SET NOCOUNT ON
		 DELETE FROM [data_columns_flows] WHERE [inflow_id] IN (SELECT flow_id FROM DELETED)
		 DELETE FROM [data_columns_flows] WHERE [outflow_id] IN (SELECT flow_id FROM DELETED)
		END;')
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_data_columns_flows_inflow_id' 
		AND object_id = OBJECT_ID('dbo.[data_columns_flows]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_data_columns_flows_inflow_id] ON [data_columns_flows] ([inflow_id])
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_data_columns_flows_inflow_column_id' 
		AND object_id = OBJECT_ID('dbo.[data_columns_flows]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_data_columns_flows_inflow_column_id] ON [data_columns_flows] ([inflow_column_id])
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_data_columns_flows_outflow_id' 
		AND object_id = OBJECT_ID('dbo.[data_columns_flows]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_data_columns_flows_outflow_id] ON [data_columns_flows] ([outflow_id])
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_data_columns_flows_outflow_column_id' 
		AND object_id = OBJECT_ID('dbo.[data_columns_flows]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_data_columns_flows_outflow_column_id] ON [data_columns_flows] ([outflow_column_id])
	END

	IF NOT EXISTS(
		SELECT TOP 1 1
		FROM [sys].[triggers]
		WHERE [name] = N'trg_data_columns_flows_Modify'
    )
	BEGIN
		EXEC dbo.sp_executesql @statement = N'
		   CREATE TRIGGER [trg_data_columns_flows_Modify]
			   ON  [data_columns_flows]
			   AFTER INSERT,UPDATE
			AS 
			BEGIN
				 UPDATE [data_columns_flows]
				 SET  
					 [last_modification_date] = GETDATE()
				 WHERE [data_columns_flow_id] IN (SELECT DISTINCT [data_columns_flow_id] FROM Inserted)
			END'
	
		ALTER TABLE [dbo].[data_columns_flows] ENABLE TRIGGER [trg_data_columns_flows_Modify]
	END

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO
