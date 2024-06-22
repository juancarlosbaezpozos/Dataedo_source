DECLARE @id AS UNIQUEIDENTIFIER = '8e992396-b253-4057-a3ff-d4d03b0b1f6d'
DECLARE @version as int = 10 
DECLARE @update as int = 2
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-3123'

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
  
	DELETE uccc
	FROM 
		[unique_constraints_columns_changes] uccc
	INNER JOIN [schema_updates] su
		ON uccc.[update_id] = su.[update_id]
	LEFT JOIN [databases] d
		ON su.[database_id] = d.[database_id]
	WHERE 
		d.[database_id] IS NULL;


	DELETE ucc
	FROM
		[unique_constraints_changes] ucc
	LEFT JOIN [databases] d
		ON ucc.[database_id] = d.[database_id]
	WHERE 
		ucc.[database_id] IS NULL;


	UPDATE cc
	SET 
		cc.[parent_id] = NULL
	FROM 
		[columns_changes] cc
	LEFT JOIN [databases] d
		ON cc.[database_id] = d.[database_id]
	WHERE 
		d.[database_id] IS NULL;
	
	DELETE cc
	FROM 
		[columns_changes] cc
	LEFT JOIN [databases] d
		ON cc.[database_id] = d.[database_id]
	WHERE 
		d.[database_id] IS NULL;


	DELETE trcc
	FROM
		[tables_relations_columns_changes] trcc
	INNER JOIN [schema_updates] su
		ON trcc.[update_id] = su.[update_id]
	LEFT JOIN [databases] d
		ON su.[database_id] = d.[database_id]
	WHERE 
		d.[database_id] IS NULL;


	DELETE trc
	FROM
		[tables_relations_changes] trc
	LEFT JOIN [databases] d
		ON trc.[database_id] = d.[database_id]
	WHERE d.[database_id] IS NULL;
	

	DELETE tc
	FROM 
		[tables_changes] tc
	LEFT JOIN [databases] d
		ON tc.[database_id] = d.[database_id]
	WHERE 
		d.[database_id] IS NULL;


	DELETE tc
	FROM [triggers_changes] tc
	LEFT JOIN [databases] d
		ON tc.[database_id] = d.[database_id]
	WHERE 
		d.[database_id] IS NULL;


	DELETE pc
	FROM 
		[parameters_changes] pc
	LEFT JOIN [databases] d 
		ON pc.[database_id] = d.[database_id]
	WHERE
		d.[database_id] IS NULL;


	DELETE prc
	FROM
		[procedures_changes] prc
	LEFT JOIN [databases] d
		ON prc.[database_id] = d.[database_id]
	WHERE 
		d.[database_id] IS NULL;
	

	DELETE su
	FROM 
		[schema_updates] su
	LEFT JOIN [databases] d
		ON su.[database_id] = d.[database_id]
	WHERE 
		d.[database_id] IS NULL;


  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO