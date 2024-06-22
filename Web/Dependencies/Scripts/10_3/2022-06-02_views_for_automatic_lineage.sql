DECLARE @id AS UNIQUEIDENTIFIER = 'f8250545-75d4-4488-9d64-5eb65811c2bd'
DECLARE @version as int = 10
DECLARE @update as int = 3
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-2011'

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
	IF OBJECT_ID(N'data_objects_v', N'U') IS NULL
	BEGIN 
		EXEC('CREATE VIEW [data_objects_v] AS
				SELECT
					[object_id], 
					[o].[database_id], 
					[object_schema], 
					[object_name], 
					[object_type],
					[d].[name] AS [database_name],
					[d].[host] AS [database_host]
				FROM 
					(SELECT 
						[table_id] as [object_id], 
						[database_id], 
						[schema] AS [object_schema], 
						[name] AS [object_name], 
						[object_type]
					FROM [tables]
					UNION ALL 
					SELECT 
						[procedure_id] as [object_id], 
						[database_id], 
						[schema] AS [object_schema], 
						[name] AS [object_name], 
						[object_type]
					FROM [procedures]
					UNION ALL 
					SELECT 
						[t].[trigger_id] as [object_id], 
						[tab].[database_id], 
						[tab].[schema] AS [object_schema], 
						[t].name AS [object_name], 
						''TRIGGER'' AS [object_type]
					FROM [triggers] AS t 
						INNER JOIN [tables] as [tab]
							on [t].[table_id] = [tab].[table_id]) [o]
				INNER JOIN [databases] [d]
					on [o].[database_id] = [d].[database_id]')
	END;
	GRANT SELECT ON [data_objects_v] TO [admins] AS [dbo];
	GRANT SELECT ON [data_objects_v] TO [users] AS [dbo];


	IF OBJECT_ID(N'data_processes_v', N'U') IS NULL
	BEGIN 
		EXEC('CREATE VIEW [data_processes_v] AS
				SELECT 
					[o].[object_id] AS [processor_id],
					[o].[database_id], 
					[o].[object_schema] AS [processor_schema], 
					[o].[object_name] AS [processor_name],  
					[o].[object_type] AS [processor_type], 
					[p].[process_id], 
					[p].[name] AS [process_name], 
					[p].[source]
				FROM [data_processes] AS [p] 
					INNER JOIN [data_objects_v] AS [o]
						ON [p].[processor_id] = [o].[object_id] 
						AND [p].[processor_type] = [o].[object_type]')
	END;
	GRANT SELECT ON [data_processes_v] TO [admins] AS [dbo];
	GRANT SELECT ON [data_processes_v] TO [users] AS [dbo];

	IF OBJECT_ID(N'data_flows_v', N'U') IS NULL
	BEGIN 
		EXEC('CREATE VIEW [data_flows_v] AS
				SELECT 
					[f].[flow_id],
					[p].[process_id],
					[p].[processor_id] [processor_id],
					[prc].[object_schema] AS [processor_schema],
					[prc].[object_name] AS [processor_name],
					[prc].[object_type] AS [processor_type],
					[p].[process_name],
					[f].[direction],
					[o].[object_id],
					[o].[object_schema],
					[o].[object_name],
					[o].[object_type],
					[f].[source] AS [flow_source],
					[p].[source] AS [processor_source]
				FROM [data_flows] [f]
				INNER JOIN [data_processes_v] [p]
					ON [f].[process_id] = [p].[process_id]
				INNER JOIN [data_objects_v] [o]
					ON [f].[object_id] = [o].[object_id]
					AND [f].[object_type] = [o].[object_type]
				INNER JOIN [data_objects_v] [prc]
					ON [p].[processor_id] = [prc].[object_id]
					AND [p].[processor_type] = [prc].[object_type]')
	END;
	GRANT SELECT ON [data_flows_v] TO [admins] AS [dbo];
	GRANT SELECT ON [data_flows_v] TO [users] AS [dbo];

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO
 