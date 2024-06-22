insert into version (version, [update], stable) values (4, 0, 0)
GO


/****** ---------- objects_id_description_list ----------  ******/
/****** Object:  StoredProcedure [dbo].[synch_parameters]    Script Date: 2015-06-29 14:52:02 ******/
DROP PROCEDURE [dbo].[synch_parameters]
GO
/****** Object:  StoredProcedure [dbo].[module_delete]    Script Date: 2015-06-29 14:59:03 ******/
DROP PROCEDURE [dbo].[module_delete]
GO
/****** Object:  StoredProcedure [dbo].[objects_module_delete]    Script Date: 2015-06-29 15:02:17 ******/
DROP PROCEDURE [dbo].[objects_module_delete]
GO
/****** Object:  StoredProcedure [dbo].[objects_module_insert]    Script Date: 2015-06-29 15:03:09 ******/
DROP PROCEDURE [dbo].[objects_module_insert]
GO
/****** Object:  StoredProcedure [dbo].[objects_set_synchronized]    Script Date: 2015-06-29 15:03:49 ******/
DROP PROCEDURE [dbo].[objects_set_synchronized]
GO



/****** Object:  UserDefinedTableType [dbo].[objects_id_description_list]    Script Date: 2015-06-29 14:49:35 ******/
DROP TYPE [dbo].[objects_id_description_list]
GO
/****** Object:  UserDefinedTableType [dbo].[objects_id_description_list]    Script Date: 2015-06-29 14:45:30 ******/
CREATE TYPE [dbo].[objects_id_description_list] AS TABLE(
	[object_id] [int] NULL,
	[description] [nvarchar](100) NULL
)
GO
GRANT CONTROL ON TYPE::[objects_id_description_list] TO [users] AS [dbo]
GO

/****** Object:  StoredProcedure [dbo].[synch_parameters]    Script Date: 2015-06-29 14:57:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Adam Adamowicz, Joanna Grzonkowska
-- Create date: 2013-90-23
-- Description:	Synchronizing the list of parameters with the repository:
--				for each parameter updating its data or inserting it if it has not existed so far or setting its status to 'D' (deleted) if it
--				has been deleted from the database.
-- =============================================
CREATE PROCEDURE [dbo].[synch_parameters] 
	@synch_parameters_list AS synch_parameters_list READONLY
	,@procedure_name [nvarchar](100)
	,@schema NVARCHAR(250)
	,@database_id int
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	BEGIN TRY
		DECLARE @name NVARCHAR(100)
			,@position INT
			,@datatype NVARCHAR(100)
			,@description NVARCHAR(MAX)
			,@data_length NVARCHAR(20)
			,@parameter_mode NVARCHAR(10)
			,@procedure_id INT
		DECLARE synch_parameters_cursor CURSOR
		FOR
		SELECT *
		FROM @synch_parameters_list
		SET @message = ''
		SELECT @procedure_id = procedure_id
		FROM procedures
		WHERE database_id = @database_id
			AND name = @procedure_name
			AND [schema] = @schema
		OPEN synch_parameters_cursor
		FETCH NEXT
		FROM synch_parameters_cursor
		INTO @name
			,@position
			,@datatype
			,@description
			,@data_length
			,@parameter_mode;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			-- Check if parameter exists in repository (and no data change)
			IF (
					SELECT count(1)
					FROM [parameters]
					WHERE [name] = @name
						AND [datatype] = @datatype
						AND cast([description] AS NVARCHAR(max)) = cast(@description AS NVARCHAR(max))
						AND procedure_id = @procedure_id
						AND [ordinal_position] = @position
						AND [data_length] = @data_length
						AND [parameter_mode] = @parameter_mode
						AND [status] = 'A'
					) = 0
			BEGIN
				-- If parameter exists but some data changed update information
				IF (
						SELECT count(1)
						FROM [parameters]
						WHERE [name] = @name
							AND procedure_id = @procedure_id
						) > 0
				BEGIN
					UPDATE [parameters]
					SET [ordinal_position] = @position
						,[parameter_mode] = @parameter_mode
						,[name] = @name
						,[datatype] = @datatype
						,[data_length] = @data_length
						,[status] = 'A'
					WHERE [name] = @name
						AND procedure_id = @procedure_id;
					-- Update description only if it's empty
					UPDATE [parameters]
					SET [description] = @description
					WHERE [name] = @name
						AND procedure_id = @procedure_id
						AND (
							[description] IS NULL
							OR DATALENGTH([description]) = 0
							)
				END
				ELSE
				BEGIN
					-- If parameter doesn't exists insert new information to repository 
					set @message += ' inserting' + @name
					INSERT INTO [parameters] (
						[procedure_id]
						,[ordinal_position]
						,[parameter_mode]
						,[name]
						,[datatype]
						,[data_length]
						,[status]
						,[description]
						)
					VALUES (
						@procedure_id
						,@position
						,@parameter_mode
						,@name
						,@datatype
						,@data_length
						,'A'
						,@description
						)
					set @message += ' inserted' + @name
				END
			END
			FETCH NEXT
			FROM synch_parameters_cursor
			INTO @name
				,@position
				,@datatype
				,@description
				,@data_length
				,@parameter_mode;
		END
		CLOSE synch_parameters_cursor;
		DEALLOCATE synch_parameters_cursor;
		-- set status to 'D' (deleted) for all parameters which are not in the database but are still in the repository
		UPDATE parameters
		SET [parameters].[status] = 'D'
		FROM [parameters]
		INNER JOIN [procedures] ON parameters.procedure_id = procedures.procedure_id
		WHERE procedures.procedure_id = @procedure_id
			AND parameters.name NOT IN (
				SELECT name
				FROM @synch_parameters_list
				)
		COMMIT TRANSACTION
		SELECT @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message += 'synch_parameters ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END


GO
GRANT EXECUTE ON [synch_parameters]
 TO [users] AS [dbo]

GO
/****** Object:  StoredProcedure [dbo].[module_delete]    Script Date: 2015-06-29 14:59:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2013-09-23
-- Description:	Deleting the module from the Repository
-- =============================================
CREATE PROCEDURE [dbo].[module_delete]
	-- Parameters
	@module_id INT
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT
AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		DELETE
		FROM [modules]
		WHERE module_id = @module_id;
		COMMIT TRANSACTION
		SET @message = ''
		SET @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'module_delete ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO
GRANT EXECUTE ON [dbo].[module_delete] TO [users]

GO

/****** Object:  StoredProcedure [dbo].[objects_module_delete]    Script Date: 2015-06-29 15:02:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2014-08-25
-- Description:	Deleting links between a module and tables/views/functions/procedures.
-- =============================================
CREATE PROCEDURE [dbo].[objects_module_delete] 
	@module_id INT
	,@objectsList AS objects_id_description_list READONLY
	,@succeed_objects_ids NVARCHAR(MAX) OUTPUT
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		DECLARE @object_id INT
			,@object_type NVARCHAR(100)
		SET @succeed_objects_ids = ''
		DECLARE objects_cursor CURSOR
		FOR
		SELECT *
		FROM @objectsList
		OPEN objects_cursor
		FETCH NEXT
		FROM objects_cursor
		INTO @object_id
			,@object_type
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF @object_type = 'T' -- tables and view
			BEGIN
				DELETE
				FROM [tables_modules]
				WHERE module_id = @module_id
					AND table_id = @object_id
				SET @succeed_objects_ids += cast(@object_id AS NVARCHAR(MAX)) + ',';
			END
			ELSE
			BEGIN -- procedures and functions
				DELETE
				FROM [procedures_modules]
				WHERE module_id = @module_id
					AND procedure_id = @object_id
				SET @succeed_objects_ids += cast(@object_id AS NVARCHAR(MAX)) + ',';
			END
			FETCH NEXT
			FROM objects_cursor
			INTO @object_id
				,@object_type
		END
		CLOSE objects_cursor;
		DEALLOCATE objects_cursor;
		COMMIT TRANSACTION
		SET @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'objects_module_delete ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END
GO
GRANT EXECUTE ON [objects_module_delete] TO [users] AS [dbo]

GO

/****** Object:  StoredProcedure [dbo].[objects_module_insert]    Script Date: 2015-06-29 15:03:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2014-08-25
-- Description:	Adding links between a module and tables/views/functions/procedures.
-- =============================================
CREATE PROCEDURE [dbo].[objects_module_insert] @module_id INT
	,@objectsList
AS
objects_id_description_list READONLY
	,@succeed_objects_ids NVARCHAR(MAX) OUTPUT
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		DECLARE @object_id INT
			,@object_type NVARCHAR(100)
		SET @succeed_objects_ids = ''
		DECLARE objects_cursor CURSOR
		FOR
		SELECT *
		FROM @objectsList
		OPEN objects_cursor
		FETCH NEXT
		FROM objects_cursor
		INTO @object_id
			,@object_type
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF @object_type = 'T' -- tables and view
			BEGIN
				-- adding only if the link does not exists so far
				IF NOT EXISTS (
						SELECT 1
						FROM [tables_modules]
						WHERE table_id = @object_id
							AND module_id = @module_id
						)
				BEGIN
					-- Inserting a new link
					INSERT INTO [tables_modules] (
						table_id
						,module_id
						)
					VALUES (
						@object_id
						,@module_id
						)
					SET @succeed_objects_ids += cast(@object_id AS NVARCHAR(MAX)) + ',';
				END
			END
			ELSE
			BEGIN -- procedures and functions
				IF NOT EXISTS (
						SELECT 1
						FROM [procedures_modules]
						WHERE procedure_id = @object_id
							AND module_id = @module_id
						)
				BEGIN
					-- Inserting a new link
					INSERT INTO [procedures_modules] (
						procedure_id
						,module_id
						)
					VALUES (
						@object_id
						,@module_id
						)
					SET @succeed_objects_ids += cast(@object_id AS NVARCHAR(MAX)) + ',';
				END
			END
			FETCH NEXT
			FROM objects_cursor
			INTO @object_id
				,@object_type
		END
		CLOSE objects_cursor;
		DEALLOCATE objects_cursor;
		COMMIT TRANSACTION
		SET @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'objects_module_insert ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END
GO
GRANT EXECUTE ON [objects_module_insert] TO [users] AS [dbo]

GO

/****** Object:  StoredProcedure [dbo].[objects_set_synchronized]    Script Date: 2015-06-29 15:03:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2014-07-14
-- Description:	Setting the synchronization date for tables, views, procedures and functions.
-- =============================================
CREATE PROCEDURE [dbo].[objects_set_synchronized] 
	@objects_id_description_list AS objects_id_description_list READONLY
	,@server_time DATETIME
	,@synchronized_by NVARCHAR(100)
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		UPDATE [tables]
		SET synchronization_date = @server_time
				,synchronized_by = @synchronized_by
		FROM (SELECT [object_id] FROM @objects_id_description_list WHERE [description] = 'T') oidl -- tables or views
		WHERE [tables].table_id = oidl.object_id
		UPDATE [procedures]
		SET synchronization_date = @server_time
				,synchronized_by = @synchronized_by
		FROM (SELECT [object_id] FROM @objects_id_description_list WHERE [description] = 'P') oidl -- procedure or function
		WHERE [procedures].procedure_id = oidl.object_id
		COMMIT TRANSACTION
		SET @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'objects_set_synchronized ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO
GRANT EXECUTE ON [objects_set_synchronized] TO [users] AS [dbo]

GO
/****** ---------- objects_id_description_list ----------  ******/
/****** ---------- synch_columns_list ----------  ******/
/****** Object:  StoredProcedure [dbo].[synch_columns]    Script Date: 2015-06-29 15:06:29 ******/
DROP PROCEDURE [dbo].[synch_columns]
GO



/****** Object:  UserDefinedTableType [dbo].[synch_columns_list]    Script Date: 2015-06-29 14:49:35 ******/
DROP TYPE [dbo].[synch_columns_list]
GO

/****** Object:  UserDefinedTableType [dbo].[synch_columns_list]    Script Date: 2015-06-29 14:45:30 ******/
CREATE TYPE [dbo].[synch_columns_list] AS TABLE(
	[name] [nvarchar](250) NULL,
	[position] [int] NULL,
	[datatype] [nvarchar](100) NULL,
	[description] [ntext] NULL,
	[constraint_type] [nvarchar](1) NULL,
	[data_length] [nvarchar](20) NULL,
	[nullable] [bit] NULL,
	[default_def] [nvarchar](4000) NULL,
	[identity_def] [nvarchar](50) NULL
)
GO

GRANT CONTROL ON TYPE::[synch_columns_list] TO [users] AS [dbo]
GO

/****** Object:  StoredProcedure [dbo].[synch_columns]    Script Date: 2015-06-29 15:06:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:  Adam Adamowicz, Joanna Grzonkowska
-- Create date: 2013-02-20
-- Description: Synchronizing the list of columns with the repository:
--    for each column updating its data or inserting it if it has not existed so far or setting its status to 'D' (deleted) if it
--    has been deleted from the database.
-- =============================================
CREATE PROCEDURE [dbo].[synch_columns] @synch_columns_list
AS
    synch_columns_list READONLY
 ,@table_name [nvarchar](250)
 ,@schema NVARCHAR(250)
 ,@database_id INT
 ,@result INT OUTPUT
 ,@message NVARCHAR(500) OUTPUT AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;
 BEGIN TRANSACTION
 BEGIN TRY
  DECLARE @name NVARCHAR(100)
   ,@position INT
   ,@datatype NVARCHAR(100)
   ,@description NVARCHAR(MAX)
   ,@constraint_type NVARCHAR(1)
   ,@data_length NVARCHAR(20)
   ,@nullable BIT
   ,@table_id INT
   ,@primary_key CHAR(1)
   ,@default_def NVARCHAR(4000)
   ,@identity_def NVARCHAR(50)
  DECLARE synch_columns_cursor CURSOR
  FOR
  SELECT *
  FROM @synch_columns_list
  SET @message = ''
  
  set @table_id = null
  SELECT @table_id = table_id
  FROM tables
  WHERE database_id = @database_id
   AND name = @table_name
   AND [schema] = @schema
  
  --23.06.2015 Ł.Gil
  --special message when unable to find table with specified name
  if @table_id is null begin
  ROLLBACK TRANSACTION
  SET @result = - 2;
  return
  end

  OPEN synch_columns_cursor
  FETCH NEXT
  FROM synch_columns_cursor
  INTO @name
   ,@position
   ,@datatype
   ,@description
   ,@constraint_type
   ,@data_length
   ,@nullable
   ,@default_def
   ,@identity_def;
  WHILE @@FETCH_STATUS = 0
  BEGIN
   SELECT @primary_key = CASE
     WHEN @constraint_type = 'P'
      THEN 1
     ELSE 0
     END
   -- Check if column exists in repository (and no data change)
   IF (
     SELECT count(1)
     FROM [columns]
     WHERE [name] = @name
      AND [datatype] = @datatype
      AND cast([description] AS NVARCHAR(max)) = cast(@description AS NVARCHAR(max))
      AND table_id = @table_id
      AND [ordinal_position] = @position
      AND [status] = 'A'
      AND [data_length] = @data_length
      AND [primary_key] = @primary_key
      AND [nullable] = @nullable
      AND [default_def] = @default_def
      AND [identity_def] = @identity_def
     ) = 0
   BEGIN
    -- If column exists but some data changed update information
    IF (
      SELECT count(1)
      FROM [columns]
      WHERE [name] = @name
       AND table_id = @table_id
      ) > 0
    BEGIN
     UPDATE [columns]
     SET [datatype] = @datatype
      ,[ordinal_position] = @position
      ,[status] = 'A'
      ,[data_length] = @data_length
      ,[primary_key] = @primary_key
      ,[nullable] = @nullable
      ,[default_def] = @default_def
      ,[identity_def] = @identity_def
     WHERE [name] = @name
      AND table_id = @table_id;
     -- Override column's comments from DBMS only if it's empty in repository
     UPDATE [columns]
     SET [description] = @description
     WHERE [name] = @name
      AND table_id = @table_id
      AND (
       [description] IS NULL
       OR DATALENGTH([description]) = 0
       )
    END
    ELSE
    BEGIN
     -- If column doesn't exists insert new information to repository
     INSERT INTO [columns] (
      [table_id]
      ,[name]
      ,[ordinal_position]
      ,[description]
      ,[datatype]
      ,[status]
      ,[primary_key]
      ,[data_length]
      ,[nullable]
      ,[default_def]
      ,[identity_def]
      )
     VALUES (
      @table_id
      ,@name
      ,@position
      ,@description
      ,@datatype
      ,'A'
      ,@primary_key
      ,@data_length
      ,@nullable
      ,@default_def
      ,@identity_def
      )
    END
   END
   FETCH NEXT
   FROM synch_columns_cursor
   INTO @name
    ,@position
    ,@datatype
    ,@description
    ,@constraint_type
    ,@data_length
    ,@nullable
    ,@default_def
    ,@identity_def;
  END
  CLOSE synch_columns_cursor;
  DEALLOCATE synch_columns_cursor;
  -- set status to 'D' (deleted) for all columns which are not in the database but are still in the repository
  UPDATE c
  SET c.[status] = 'D'
  FROM [columns] c
  INNER JOIN [tables] t ON c.table_id = t.table_id
  WHERE t.table_id = @table_id
   AND c.name NOT IN (
    SELECT name
    FROM @synch_columns_list
    )
  COMMIT TRANSACTION
  SELECT @result = 0
 END TRY
 BEGIN CATCH
  ROLLBACK TRANSACTION
  SET @result = - 1;
  SET @message = 'synch_columns ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
 END CATCH
END
GO
GRANT EXECUTE ON [synch_columns] TO [users] AS [dbo]

GO
/****** ---------- synch_columns_list ----------  ******/
/****** ---------- synch_parameters_list ----------  ******/
/****** Object:  StoredProcedure [dbo].[synch_parameters]    Script Date: 2015-06-29 15:09:00 ******/
DROP PROCEDURE [dbo].[synch_parameters]
GO



/****** Object:  UserDefinedTableType [dbo].[synch_parameters_list]    Script Date: 2015-06-29 14:49:35 ******/
DROP TYPE [dbo].[synch_parameters_list]
GO

/****** Object:  UserDefinedTableType [dbo].[synch_parameters_list]    Script Date: 2015-06-29 14:45:30 ******/
CREATE TYPE [dbo].[synch_parameters_list] AS TABLE(
	[name] [nvarchar](250) NULL,
	[position] [int] NULL,
	[datatype] [nvarchar](250) NULL,
	[description] [ntext] NULL,
	[data_length] [nvarchar](20) NULL,
	[parameter_mode] [nvarchar](10) NULL
)
GO

GRANT CONTROL ON TYPE::[synch_parameters_list] TO [users] AS [dbo]

GO
-- =============================================
-- Author:		Adam Adamowicz, Joanna Grzonkowska
-- Create date: 2013-90-23
-- Description:	Synchronizing the list of parameters with the repository:
--				for each parameter updating its data or inserting it if it has not existed so far or setting its status to 'D' (deleted) if it
--				has been deleted from the database.
-- =============================================
CREATE PROCEDURE [dbo].[synch_parameters] 
	@synch_parameters_list AS synch_parameters_list READONLY
	,@procedure_name [nvarchar](100)
	,@schema NVARCHAR(250)
	,@database_id int
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	BEGIN TRY
		DECLARE @name NVARCHAR(100)
			,@position INT
			,@datatype NVARCHAR(100)
			,@description NVARCHAR(MAX)
			,@data_length NVARCHAR(20)
			,@parameter_mode NVARCHAR(10)
			,@procedure_id INT
		DECLARE synch_parameters_cursor CURSOR
		FOR
		SELECT *
		FROM @synch_parameters_list
		SET @message = ''
		SELECT @procedure_id = procedure_id
		FROM procedures
		WHERE database_id = @database_id
			AND name = @procedure_name
			AND [schema] = @schema
		OPEN synch_parameters_cursor
		FETCH NEXT
		FROM synch_parameters_cursor
		INTO @name
			,@position
			,@datatype
			,@description
			,@data_length
			,@parameter_mode;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			-- Check if parameter exists in repository (and no data change)
			IF (
					SELECT count(1)
					FROM [parameters]
					WHERE [name] = @name
						AND [datatype] = @datatype
						AND cast([description] AS NVARCHAR(max)) = cast(@description AS NVARCHAR(max))
						AND procedure_id = @procedure_id
						AND [ordinal_position] = @position
						AND [data_length] = @data_length
						AND [parameter_mode] = @parameter_mode
						AND [status] = 'A'
					) = 0
			BEGIN
				-- If parameter exists but some data changed update information
				IF (
						SELECT count(1)
						FROM [parameters]
						WHERE [name] = @name
							AND procedure_id = @procedure_id
						) > 0
				BEGIN
					UPDATE [parameters]
					SET [ordinal_position] = @position
						,[parameter_mode] = @parameter_mode
						,[name] = @name
						,[datatype] = @datatype
						,[data_length] = @data_length
						,[status] = 'A'
					WHERE [name] = @name
						AND procedure_id = @procedure_id;
					-- Update description only if it's empty
					UPDATE [parameters]
					SET [description] = @description
					WHERE [name] = @name
						AND procedure_id = @procedure_id
						AND (
							[description] IS NULL
							OR DATALENGTH([description]) = 0
							)
				END
				ELSE
				BEGIN
					-- If parameter doesn't exists insert new information to repository 
					set @message += ' inserting' + @name
					INSERT INTO [parameters] (
						[procedure_id]
						,[ordinal_position]
						,[parameter_mode]
						,[name]
						,[datatype]
						,[data_length]
						,[status]
						,[description]
						)
					VALUES (
						@procedure_id
						,@position
						,@parameter_mode
						,@name
						,@datatype
						,@data_length
						,'A'
						,@description
						)
					set @message += ' inserted' + @name
				END
			END
			FETCH NEXT
			FROM synch_parameters_cursor
			INTO @name
				,@position
				,@datatype
				,@description
				,@data_length
				,@parameter_mode;
		END
		CLOSE synch_parameters_cursor;
		DEALLOCATE synch_parameters_cursor;
		-- set status to 'D' (deleted) for all parameters which are not in the database but are still in the repository
		UPDATE parameters
		SET [parameters].[status] = 'D'
		FROM [parameters]
		INNER JOIN [procedures] ON parameters.procedure_id = procedures.procedure_id
		WHERE procedures.procedure_id = @procedure_id
			AND parameters.name NOT IN (
				SELECT name
				FROM @synch_parameters_list
				)
		COMMIT TRANSACTION
		SELECT @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message += 'synch_parameters ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO
GRANT EXECUTE ON [synch_parameters]
 TO [users] AS [dbo]

GO
/****** ---------- synch_parameters_list ----------  ******/


/****** ---------- TABLES ----------  ******/
ALTER TABLE [dbo].[tables_relations]
DROP CONSTRAINT [DF_tables_relations_source]
GO
ALTER TABLE [dbo].[tables_relations]
ALTER COLUMN [source] [nvarchar](50) NOT NULL
GO
ALTER TABLE [dbo].[tables_relations]
ADD  CONSTRAINT [DF_tables_relations_source] DEFAULT ('USER') FOR [source]
GO


ALTER TABLE [dbo].[unique_constraints]
DROP CONSTRAINT [DF_unique_constraints_source]
GO
ALTER TABLE [dbo].[unique_constraints]
ALTER COLUMN [source] [nvarchar](50) NOT NULL
GO
ALTER TABLE [dbo].[unique_constraints]
ADD CONSTRAINT [DF_unique_constraints_source] DEFAULT ('USER') FOR [source]

ALTER TABLE [dbo].[version]
DROP CONSTRAINT [DF_version_installed_by]
GO
ALTER TABLE [dbo].[version]
ALTER COLUMN [installed_by] [nvarchar](100) NULL
GO
ALTER TABLE [dbo].[version]
ADD CONSTRAINT [DF_version_installed_by] DEFAULT (suser_sname()) FOR [installed_by]
GO

/****** ---------- STORED PROCEDURES ----------  ******/

/****** Object:  StoredProcedure [dbo].[columns_delete]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2014-06-23
-- Description:	Deleting a table's columns which has id in the given @objectsList
-- =============================================
ALTER PROCEDURE [dbo].[columns_delete] 
	@objectsList AS objects_id_list READONLY
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		--CASCADE SIMULATION
		DELETE FROM [unique_constraints_columns]
			WHERE [column_id] in (SELECT [object_id] FROM @objectsList)
		DELETE FROM [tables_relations_columns]
			WHERE [column_fk_id] in (SELECT [object_id] FROM @objectsList)
			OR [column_pk_id] in (SELECT [object_id] FROM @objectsList)
		--ACTUAL DELETE
		DELETE
		FROM [columns]
		WHERE [column_id] in (SELECT [object_id] FROM @objectsList)
	COMMIT TRANSACTION
	SET @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'columns_delete ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[columns_update]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2014-04-18
-- Description:	Updating information about all modified columns
-- =============================================
ALTER PROCEDURE [dbo].[columns_update]
    @columns_list AS columns_list READONLY,
  	@result int OUTPUT,
	@message NVARCHAR(500) OUTPUT
AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		UPDATE [columns]
		SET 
			[title] = cl.[title],
  			[description] = cl.[description]
		FROM 
			(SELECT [column_id], [title], [description] FROM @columns_list) cl
		WHERE 
			[columns].[column_id] = cl.[column_id]
	COMMIT TRANSACTION
	SET @result = 0;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'columns_update ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[database_insert]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2013-12-31
-- Description:	Inserting information about a new database
-- =============================================
ALTER PROCEDURE [dbo].[database_insert]
	-- Parameters
	@name NVARCHAR(250)
	,@title NVARCHAR(250)
	,@host NVARCHAR(100)
	,@user NVARCHAR(100)
	,@password NVARCHAR(100)
	,@type NVARCHAR(100)
	,@port INT
	,@service_name NVARCHAR(100)
	,@description NTEXT
	,@windows_authentication BIT
	,@different_schema BIT
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT
AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		INSERT INTO [databases] (
			[name]
			,[title]
			,[host]
			,[user]
			,[password]
			,[type]
			,[port]
			,[service_name]
			,[description]
			,[windows_authentication]
			,[different_schema]
			)
		VALUES (
			@name
			,@title
			,@host
			,@user
			,@password
			,@type
			,@port
			,@service_name
			,@description
			,@windows_authentication
			,@different_schema
			)
		COMMIT TRANSACTION
		-- Set returns values
		SET @message = ''
		SET @result = @@IDENTITY
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'database_insert ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[database_remove]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2013-12-31
-- Description:	Deleting the database from the repository
-- =============================================
ALTER PROCEDURE [dbo].[database_remove]
	-- Parameters
	@database_id INT
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT
AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		SELECT [table_id]
		INTO #deleted_tables
		FROM [tables]
		WHERE database_id = @database_id;
		SELECT [column_id]
		INTO #deleted_columns
		FROM [columns]
		WHERE [table_id] IN (SELECT [table_id] FROM #deleted_tables);
		SELECT procedure_id
		INTO #deleted_procedures
		FROM [procedures]
		WHERE database_id = @database_id;
		--CASCADE COLUMNS SIMULATION
		DELETE
		FROM [unique_constraints_columns]
		WHERE column_id IN (SELECT column_id FROM #deleted_columns);
		DELETE
		FROM [tables_relations_columns]
		WHERE [column_fk_id] IN (SELECT column_id FROM #deleted_columns)
			OR [column_pk_id] IN (SELECT column_id FROM #deleted_columns);
		--CASCADE PROCEDURES SIMULATION
		DELETE
		FROM [procedures_modules]
		WHERE procedure_id IN (SELECT procedure_id FROM #deleted_procedures);
		--CASCADE TABLES DELETE
		DELETE
		FROM [tables_modules]
		WHERE table_id IN (SELECT table_id FROM #deleted_tables);
		DELETE
		FROM [tables_relations]
		WHERE [pk_table_id] IN (SELECT table_id FROM #deleted_tables)
			OR [fk_table_id] IN (SELECT table_id FROM #deleted_tables);
		DELETE
		FROM [databases]
		WHERE database_id = @database_id
		COMMIT TRANSACTION
		-- Set returns values
		SET @message = ''
		SET @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'database_remove ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[database_update]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Adam Adamowicz, Joanna Grzonkowska
-- Create date: 2013-02-20
-- Description:	Updating information about the database.
--				When title or description are '' then these fields are not updated.
-- =============================================
ALTER PROCEDURE [dbo].[database_update]
	-- Parameters
	@database_id INT
	,@name NVARCHAR(250)
	,@title NVARCHAR(250)
	,@host NVARCHAR(100)
	,@user NVARCHAR(100)
	,@password NVARCHAR(100)
	,@port INT
	,@service_name NVARCHAR(100)
	,@description NTEXT
	,@windows_authentication BIT
	,@different_schema BIT
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT
AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		-- Update database information
		UPDATE databases
		SET [name] = @name
			,[title] = CASE 
				WHEN isnull(cast(@title AS NVARCHAR(max)), 'empty') = ''
					THEN [title]
				ELSE @title
				END
			,[host] = @host
			,[user] = @user
			,[password] = @password
			,[port] = @port
			,[service_name] = @service_name
			,[description] = CASE 
				WHEN isnull(cast(@description AS NVARCHAR(max)), 'empty') = ''
					THEN [description]
				ELSE @description
				END
			,[windows_authentication] = @windows_authentication
			,[different_schema] = @different_schema
		WHERE database_id = @database_id
		COMMIT TRANSACTION
		SET @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'database_update ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[database_update_title]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2014-04-17
-- Description:	Updating only title of a given database
-- =============================================
ALTER PROCEDURE [dbo].[database_update_title]
	-- Parameters
	@database_id INT
	,@title NVARCHAR(250)
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT
AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		-- Update database information
		UPDATE [databases]
		SET [title] = @title
		WHERE database_id = @database_id
		COMMIT TRANSACTION
		SET @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'database_update_title ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[ignored_objects_delete]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2013-09-06
-- Description:	Deleting objects from the ignored objects table.
-- Those objects (tables/views/procedures/functions) will be synchronized.
-- =============================================
ALTER PROCEDURE [dbo].[ignored_objects_delete] 
	@ignored_objects_list AS ignored_objects_list READONLY
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		DELETE
		FROM [ignored_objects]
		FROM [ignored_objects] io INNER JOIN (select * FROM @ignored_objects_list) l ON 
		    io.[database_id] = l.[database_id]
			AND io.[schema] = l.[schema]
			AND io.[name] = l.[name]
			AND io.[object_type] = l.[object_type]
		COMMIT TRANSACTION
		SET @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'ignored_objects_delete ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[ignored_objects_insert]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2013-09-06
-- Description:	Inserting ignored objects - those objects (tables/views/procedures/functions) won't be synchronized.
-- =============================================
ALTER PROCEDURE [dbo].[ignored_objects_insert] 
	@ignored_objects_list AS ignored_objects_list READONLY
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		INSERT INTO [ignored_objects] (
			[database_id]
			,[schema]
			,[name]
			,[object_type]
			)
		SELECT iol.[database_id]
			,iol.[schema]
			,iol.[name]
			,iol.[OBJECT_TYPE]
		FROM @ignored_objects_list iol LEFT JOIN ignored_objects io
		 ON iol.[database_id] = io.[database_id]
			AND iol.[schema] = io.[schema]
			AND iol.[name] = io.[name]
			AND iol.[object_type] = io.[object_type]
		AND io.database_id IS NULL  -- to avoid duplicates
		COMMIT TRANSACTION
		SET @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = @message + 'ignored_objects_insert ERROR: ' + CAST(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[module_insert]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2013-09-04
-- Description:	Inserting a new module.
-- =============================================
ALTER PROCEDURE [dbo].[module_insert]
	-- Parameters
	@database_id INT
	,@title NVARCHAR(250)
	,@description NTEXT
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT
AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		INSERT INTO [modules] (
			[title]
			,[description]
			,[database_id]
			)
		VALUES (
			@title
			,@description
			,@database_id
			)
		COMMIT TRANSACTION
		SET @message = ''
		SET @result = @@IDENTITY
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'module_insert ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[module_update]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2013-09-04
-- Description:	Updating information about a module.
--				When @description is equal to '' then this field is not updated
-- =============================================
ALTER PROCEDURE [dbo].[module_update]
	-- Parameters
	@module_id INT
	,@title NVARCHAR(250)
	,@description NTEXT
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT
AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		-- Updating domain information
		UPDATE [modules]
		SET [title] = @title
			,[description] = CASE 
				WHEN isnull(cast(@description AS NVARCHAR(max)), 'empty') = ''
					THEN [description]
				ELSE @description
				END
		WHERE module_id = @module_id;
		COMMIT TRANSACTION
		SET @message = ''
		SET @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'module_update ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[object_check_synchronization]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2013-09-06
-- Description:	Check if an object (table/view/procedure/function) is ignored/new/unsynchronized/synchronized.
-- =============================================
ALTER PROCEDURE [dbo].[object_check_synchronization]
	-- Parameters
	@name NVARCHAR(250)
	,@schema NVARCHAR(250)
	,@object_type NVARCHAR(100)
	,@database_id INT
	,@dbms_modify_date DATETIME
	,@result CHAR OUTPUT
	,@message NVARCHAR(500) OUTPUT
AS
BEGIN
	IF @object_type = 'TABLE'
		OR @object_type = 'VIEW'
	BEGIN
		UPDATE [tables]
		SET [exists_in_DBMS] = 1
		WHERE name = @name
			  AND [schema] = @schema
			  AND database_id = @database_id
	END
	ELSE
	BEGIN
		UPDATE [procedures]
		SET [exists_in_DBMS] = 1
		WHERE name = @name
			  AND [schema] = @schema
			  AND database_id = @database_id
	END
	IF EXISTS (
			SELECT 1
			FROM [ignored_objects]
			WHERE name = @name
				AND [schema] = @schema
				AND database_id = @database_id
			)
	BEGIN
		SET @message = 'Object is ignored'
		SET @result = 'I'
		RETURN 0
	END
	IF @object_type = 'TABLE'
		OR @object_type = 'VIEW'
	BEGIN
		IF NOT EXISTS (
				SELECT 1
				FROM [tables]
				WHERE name = @name
					AND [schema] = @schema
					AND database_id = @database_id
				)
		BEGIN
			SET @message = 'Table is new'
			SET @result = 'N'
			RETURN 0
		END
		IF EXISTS (
				SELECT 1
				FROM [tables]
				WHERE name = @name
					AND [schema] = @schema
					AND database_id = @database_id
					AND synchronization_date > convert(DATETIME, @dbms_modify_date, 120)
					AND [status] = 'A'
				)
		BEGIN
			SET @message = 'Table is synchronized'
			SET @result = 'S'
			RETURN 0
		END
	END
	ELSE
	BEGIN
		IF NOT EXISTS (
				SELECT 1
				FROM [procedures]
				WHERE name = @name
					AND [schema] = @schema
					AND database_id = @database_id
				)
		BEGIN
			SET @message = 'Procedure is new'
			SET @result = 'N'
			RETURN 0
		END
		IF EXISTS (
				SELECT 1
				FROM [procedures]
				WHERE name = @name
					AND [schema] = @schema
					AND database_id = @database_id
					AND synchronization_date > convert(DATETIME, @dbms_modify_date, 120)
					AND [status] = 'A'
				)
		BEGIN
			SET @message = 'Procedure is synchronized'
			SET @result = 'S'
			RETURN 0
		END
	END
	SELECT @message = 'Object is unsynchronized'
	SELECT @result = 'U'
	RETURN 0
END

GO

/****** Object:  StoredProcedure [dbo].[objects_clear_exists_flag]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2015-01-05
-- Description:	Clear the tables' and procedures' flag which denotes if an object exists in DBMS.
-- =============================================
ALTER PROCEDURE [dbo].[objects_clear_exists_flag]
	 @result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT 
AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		UPDATE [tables]
		set [exists_in_DBMS] = 0;
		UPDATE [procedures]
		set [exists_in_DBMS] = 0;
		COMMIT TRANSACTION
		SET @result = 0;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'objects_clear_exists_flag ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[parameters_delete]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2014-06-27
-- Description:	Deleting a procedures's paramters which has id in the given @objectsList
-- =============================================
ALTER PROCEDURE [dbo].[parameters_delete] 
	@objectsList AS objects_id_list READONLY
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		DELETE
		FROM [parameters]
		WHERE parameter_id IN (SELECT [object_id] FROM @objectsList)
		COMMIT TRANSACTION
		SET @result = 0;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'parameters_delete ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[parameters_update]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2014-04-22
-- Description:	Updating information about all modified parameters
-- =============================================
ALTER PROCEDURE [dbo].[parameters_update] 
	@parameters_list AS parameters_list READONLY
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		UPDATE [parameters]
		SET [parameters].[description] = pl.description
		FROM (SELECT parameter_id, [description] FROM @parameters_list) pl
		WHERE [parameters].parameter_id = pl.parameter_id;
		COMMIT TRANSACTION
		SET @result = 0;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'parameters_update ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[procedure_delete]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2013-09-23
-- Description:	Deleting the procedure/function from the Repository
-- =============================================
ALTER PROCEDURE [dbo].[procedure_delete]
	@procedure_id INT
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT
AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		--CASCADE SIMULATION
		DELETE FROM [procedures_modules]
			WHERE procedure_id = @procedure_id;
		--ACTUAL DELETE
		DELETE
		FROM [procedures]
		WHERE procedure_id = @procedure_id;
		COMMIT TRANSACTION
		SELECT @message = ''
		SELECT @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'procedure_delete ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[procedure_update]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Adam Adamowicz, Joanna Grzonkowska
-- Create date: 2013-06-20
-- Description:	Updating information about a procedure
--				When @description or @modules_id are equal to '' then these fields are not updated
-- =============================================
ALTER PROCEDURE [dbo].[procedure_update]
	-- Parameters
	@procedure_id INT
	,@title NVARCHAR(250)
	,@description NTEXT
	,@modules_id NVARCHAR(MAX)
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT
AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		SELECT @message = ''
		IF @modules_id is not null
		BEGIN
			SET @modules_id = replace(@modules_id, ' ', '');
			SELECT *
			INTO #new_modules_id
			FROM [split_strings](@modules_id, ',');
			DELETE  -- delete unchecked modules 
			FROM procedures_modules
			WHERE procedure_id = @procedure_id
			AND module_id NOT IN (SELECT value FROM #new_modules_id)
			INSERT INTO procedures_modules (  -- insert new checked modules
				procedure_id,
				module_id
		 	)
			(SELECT @procedure_id, value FROM #new_modules_id nmi 
				WHERE value NOT IN (SELECT module_id FROM procedures_modules WHERE procedure_id = @procedure_id) )
		END
		-- Update information
		UPDATE [procedures]
		SET [title] = @title
			,[description] = CASE 
				WHEN isnull(cast(@description AS NVARCHAR(max)), 'empty') = ''
					THEN [description]
				ELSE @description
				END
		WHERE procedure_id = @procedure_id;
		COMMIT TRANSACTION
		SELECT @message = ''
		SELECT @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'procedure_update ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[relation_insert_or_update]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2014-05-23
-- Description:	Inserting or updating information about a given constraint
-- =============================================
ALTER PROCEDURE [dbo].[relation_insert_or_update]
	-- Parameters
	@table_relation_id INT
	,@pk_table_id INT
	,@fk_table_id INT
	,@name NVARCHAR(250)
	,@description NTEXT
	,@tables_relations_columnsList AS tables_relations_columns_list READONLY
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	BEGIN TRY
		DECLARE @table_relation_column_id INT
		DECLARE @column_fk_id INT
		DECLARE @column_pk_id INT
		DECLARE @ordinal_position INT
		SET @message = '';
		-- inserting or updating relation
		SELECT TOP 1 1
		FROM tables_relations
		WHERE table_relation_id = @table_relation_id;
		IF (@@ROWCOUNT > 0)
		BEGIN
			UPDATE tables_relations
			SET [name] = @name
				,[description] = @description
				,[pk_table_id] = @pk_table_id
				,[fk_table_id] = @fk_table_id
			WHERE table_relation_id = @table_relation_id;
		END
		ELSE
		BEGIN
			-- There are 2 types of relations: 
			-- from database ('DBMS') and created by users in the Metadata Editor program (USER).
			-- Here we pass only relations that were created in the Metadata Editor.
			-- Inserting new references
			INSERT INTO tables_relations (
				[pk_table_id]
				,[fk_table_id]
				,[name]
				,[description]
				,[status]
				,[source]
				)
			VALUES (
				@pk_table_id
				,@fk_table_id
				,@name
				,@description
				,'A'
				,'USER'
				)
			SET @table_relation_id = @@IDENTITY
		END
		-- inserting, updating or deleting relation's columns
		SELECT table_relation_column_id
		INTO #modified_relation_columns
		FROM @tables_relations_columnsList;
		SELECT table_relation_column_id
		INTO #current_relation_columns
		FROM tables_relations_columns
		WHERE table_relation_id = @table_relation_id;
		-- relation's columns to delete
		DECLARE links_delete_cursor CURSOR
		FOR
		SELECT *
		FROM #current_relation_columns
		EXCEPT
		SELECT *
		FROM #modified_relation_columns
		OPEN links_delete_cursor
		FETCH NEXT
		FROM links_delete_cursor
		INTO @table_relation_column_id;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			DELETE
			FROM tables_relations_columns
			WHERE table_relation_column_id = @table_relation_column_id
			FETCH NEXT
			FROM links_delete_cursor
			INTO @table_relation_column_id;
		END
		CLOSE links_delete_cursor;
		DEALLOCATE links_delete_cursor;
		-- relation's columns to insert
		DECLARE links_insert_cursor CURSOR
		FOR
		SELECT *
		FROM #modified_relation_columns
		EXCEPT
		SELECT *
		FROM #current_relation_columns
		OPEN links_insert_cursor
		FETCH NEXT
		FROM links_insert_cursor
		INTO @table_relation_column_id;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SELECT @column_fk_id = column_fk_id
				,@column_pk_id = column_pk_id
				,@ordinal_position = ordinal_position
			FROM @tables_relations_columnsList
			WHERE table_relation_column_id = @table_relation_column_id;
			-- adding the given columns for this relation only if there is no such data already
			SELECT TOP 1 1
			FROM tables_relations_columns
			WHERE table_relation_id = @table_relation_id
				AND column_fk_id = @column_fk_id
				AND column_pk_id = @column_pk_id;
			IF (@@ROWCOUNT = 0)
			BEGIN
				-- Inserting new references
				INSERT INTO tables_relations_columns (
					[table_relation_id]
					,[column_fk_id]
					,[column_pk_id]
					,[ordinal_position]
					,[status]
					)
				VALUES (
					@table_relation_id
					,@column_fk_id
					,@column_pk_id
					,@ordinal_position
					,'A'
					)
			END
			FETCH NEXT
			FROM links_insert_cursor
			INTO @table_relation_column_id;
		END
		CLOSE links_insert_cursor;
		DEALLOCATE links_insert_cursor;
		-- relation columns to update
		DECLARE links_update_cursor CURSOR
		FOR
		SELECT *
		FROM #modified_relation_columns
		INTERSECT
		SELECT *
		FROM #current_relation_columns
		OPEN links_update_cursor
		FETCH NEXT
		FROM links_update_cursor
		INTO @table_relation_column_id;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SELECT @column_fk_id = column_fk_id
				,@column_pk_id = column_pk_id
				,@ordinal_position = ordinal_position
			FROM @tables_relations_columnsList
			WHERE table_relation_column_id = @table_relation_column_id;
			UPDATE tables_relations_columns
			SET [column_fk_id] = @column_fk_id
				,[column_pk_id] = @column_pk_id
				,[ordinal_position] = @ordinal_position
			WHERE [table_relation_column_id] = @table_relation_column_id
			FETCH NEXT
			FROM links_update_cursor
			INTO @table_relation_column_id;
		END
		CLOSE links_update_cursor;
		DEALLOCATE links_update_cursor;
		COMMIT TRANSACTION
		SET @result = 0;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = @message + 'relation_insert_or_update ERROR: ' + CAST(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[relations_delete]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2013-10-08
-- Description:	Deleting a relation from the repository
-- =============================================
ALTER PROCEDURE [dbo].[relations_delete] 
	@objectsList AS objects_id_list READONLY
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	BEGIN TRY
		DELETE
		FROM tables_relations
		WHERE table_relation_id IN (SELECT [object_id] FROM @objectsList)
		COMMIT TRANSACTION
		SET @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'relations_delete ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO


/****** Object:  StoredProcedure [dbo].[synch_object_delete]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Adam Adamowicz, Joanna Grzonkowska
-- Create date: 2013-02-20
-- Description:	This procedure sets status as 'D' (Deleted) for tables, views, procedures and functions and theirs objects
-- =============================================
ALTER PROCEDURE [dbo].[synch_object_delete]
	-- Parameters
	@object_id INT
	,@server_time DATETIME
	,@object_type NVARCHAR(100)
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	BEGIN TRY
		IF @object_type = 'TABLE'
			OR @object_type = 'VIEW'
		BEGIN
			-- Set status of table/view to 'D'
			UPDATE [tables]
			SET synchronization_date = @server_time
				,[status] = 'D'
			WHERE table_id = @object_id
			-- Set status of table's/view's objects (columns, triggers, unique keys, relations) to 'D'
			UPDATE [columns]
			SET [status] = 'D'
			WHERE table_id = @object_id
			UPDATE [triggers]
			SET [status] = 'D'
			WHERE table_id = @object_id
			UPDATE [tables_relations]
			SET [status] = 'D'
			WHERE pk_table_id = @object_id
				OR fk_table_id = @object_id
			-- deleting relation's columns
			DELETE
			FROM tables_relations_columns
			WHERE table_relation_column_id IN (
					SELECT table_relation_column_id
					FROM tables_relations tr
					INNER JOIN tables_relations_columns trc ON trc.table_relation_id = tr.table_relation_id
					INNER JOIN tables t ON (
							t.table_id = tr.fk_table_id
							OR t.table_id = tr.pk_table_id
							)
					WHERE t.table_id = @object_id
					)
			UPDATE [unique_constraints]
			SET [status] = 'D'
			WHERE table_id = @object_id
			-- deleting unique constraints's columns
			DELETE
			FROM unique_constraints_columns
			WHERE unique_constraint_column_id IN (
					SELECT unique_constraint_column_id
					FROM unique_constraints_columns c
					JOIN unique_constraints u ON u.unique_constraint_id = c.unique_constraint_id
					WHERE u.table_id = @object_id
					)
		END
		ELSE
		BEGIN
			-- Set status of procedure/function to 'D'
			UPDATE [procedures]
			SET synchronization_date = @server_time
				,[status] = 'D'
			WHERE procedure_id = @object_id
			UPDATE [parameters]
			SET [status] = 'D'
			WHERE procedure_id = @object_id
		END
		COMMIT TRANSACTION
		SET @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'synch_object_delete ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[synch_procedure]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:  Adam Adamowicz, Joanna Grzonkowska
-- Create date: 2013-02-22
-- Description: Synchronizing a procedure or a function with the Repository.
-- =============================================
ALTER PROCEDURE [dbo].[synch_procedure] @name NVARCHAR(100)
 ,@database_id INT
 ,@schema NVARCHAR(250)
 ,@object_type NVARCHAR(100)
 ,@description NTEXT
 ,@definition NTEXT
 ,@function_type NVARCHAR(100)
 ,@dbms_creation_date DATETIME
 ,@dbms_last_modification_date DATETIME
 ,@procedure_id INT OUTPUT
 ,@result INT OUTPUT
 ,@message NVARCHAR(500) OUTPUT
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;
 BEGIN TRANSACTION
 BEGIN TRY
  -- Set procedure id (if procedure doesn't exists return null)
  SET @procedure_id = (
    SELECT procedure_id
    FROM [procedures]
    WHERE name = @name
     AND [schema] = @schema
     AND database_id = @database_id
    );
  -- Check if procedures' information exists in repository
  IF @procedure_id IS NOT NULL
  BEGIN
   -- Update definition and last modyfication time about the procedure
   UPDATE [procedures]
   SET [definition] = @definition
    ,dbms_last_modification_date = @dbms_last_modification_date
    ,[status] = 'A'
    ,[function_type] = @function_type
   WHERE [procedure_id] = @procedure_id
   -- Update table description if value in repository is null
   UPDATE [procedures]
   SET [description] = @description
   WHERE [procedure_id] = @procedure_id
    AND (
     [description] IS NULL
     OR DATALENGTH([description]) = 0
     )
  END
  ELSE
  BEGIN
   -- If procedure doesn't exists insert new information to repository
   INSERT INTO [procedures] (
    [name]
    ,[schema]
    ,database_id
    ,[description]
    ,[definition]
    ,[object_type]
    ,[status]
    ,[function_type]
    ,[dbms_creation_date]
    ,[dbms_last_modification_date]
    )
   VALUES (
    @name
    ,@schema
    ,@database_id
    ,@description
    ,@definition
    ,@object_type
    ,'A'
    ,@function_type
    ,@dbms_creation_date
    ,@dbms_last_modification_date
    )
   -- Set procedure id
   SET @procedure_id = @@IDENTITY;
  END
  COMMIT TRANSACTION
  SELECT @message = ''
  SELECT @result = 0
 END TRY
 BEGIN CATCH
  ROLLBACK TRANSACTION
  SET @result = - 1;
  SET @message = 'synch_procedure ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
 END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[synch_relations]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:  Adam Adamowicz, Joanna Grzonkowska
-- Create date: 2013-02-22
-- Description: Synchronizing the list of relations with the repository:
--    for each relation updating its data or inserting it if it has not existed so far or setting its status to 'D' (deleted) if it
--    has been deleted from the database.
-- =============================================
ALTER PROCEDURE [dbo].[synch_relations]
@synch_relations_list AS synch_relations_list READONLY
,@synch_tables_list AS objects_name_schema_list READONLY
 ,@database_id INT
 -- if relation's synchronization succeeded then adding id of its table to this variable
 -- if number of the occurences of table's id is equal to the count of its relations then it means that the relation's synchronization succeed for this table
 ,@succeed_tables_ids NVARCHAR(MAX) OUTPUT
 ,@result INT OUTPUT
 ,@message NVARCHAR(500) OUTPUT AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;
 BEGIN TRANSACTION
 BEGIN TRY
  DECLARE @name NVARCHAR(100)
   ,@fk_table_name NVARCHAR(100)
   ,@pk_table_name NVARCHAR(100)
   ,@fk_table_schema NVARCHAR(250)
   ,@pk_table_schema NVARCHAR(250)
   ,@table_pk_id INT
   ,@table_fk_id INT
   ,@relation_id INT
   ,@description NVARCHAR(MAX)
   ,@update_rule NVARCHAR(100)
   ,@delete_rule NVARCHAR(100)
  DECLARE synch_relations_cursor CURSOR
  FOR
  SELECT *
  FROM @synch_relations_list
  SET @message = ''
  SET @succeed_tables_ids = ''
  OPEN synch_relations_cursor
  FETCH NEXT
  FROM synch_relations_cursor
  INTO @name
   ,@fk_table_name
   ,@pk_table_name
   ,@fk_table_schema
   ,@pk_table_schema
   ,@description
   ,@update_rule
   ,@delete_rule
  WHILE @@FETCH_STATUS = 0
  BEGIN
   -- Get relation_id
   SELECT @relation_id = - 1
    ,@table_pk_id = - 1
    ,@table_fk_id = - 1;
   -- Get ids of linked tables
   SELECT @table_pk_id = table_id
   FROM [tables]
   WHERE database_id = @database_id
    AND name = @pk_table_name
    AND [schema] = @pk_table_schema
   SELECT @table_fk_id = table_id
   FROM [tables]
   WHERE database_id = @database_id
    AND name = @fk_table_name
    AND [schema] = @fk_table_schema
   IF @table_pk_id <> - 1
    AND @table_fk_id <> - 1
   BEGIN
    SELECT @relation_id = tr.table_relation_id
    FROM tables_relations tr
    WHERE tr.name = @name
     AND tr.source = 'DBMS'
     AND tr.pk_table_id = @table_pk_id
     AND tr.fk_table_id = @table_fk_id
    -- Insert the relation if not exists
    IF @relation_id = - 1
    BEGIN
     -- There are 2 types of relations:
     -- from database ('DBMS') and created by users in the Metadata Editor program (USER).
     -- Here we pass only relations that we are in the database.
     INSERT INTO tables_relations (
      [pk_table_id]
      ,[fk_table_id]
      ,[name]
      ,[status]
      ,[source]
      ,[description]
      ,[update_rule]
      ,[delete_rule]
      )
     VALUES (
      @table_pk_id
      ,@table_fk_id
      ,@name
      ,'A'
      ,'DBMS'
      ,@description
      ,@update_rule
      ,@delete_rule
      )
    END
    ELSE
    BEGIN
     UPDATE tables_relations
     SET [status] = 'A',
      [update_rule] = @update_rule,
      [delete_rule] = @delete_rule
     WHERE table_relation_id = @relation_id;
     -- Override relation's comments from DBMS only if it's empty in repository
     UPDATE tables_relations
     SET [description] = @description
     WHERE table_relation_id = @relation_id
      AND (
       [description] IS NULL
       OR DATALENGTH([description]) = 0
       )
    END
   END
   SET @succeed_tables_ids += cast(@table_pk_id AS NVARCHAR(MAX)) + ',' + cast(@table_fk_id AS NVARCHAR(MAX)) + ',';
   FETCH NEXT
   FROM synch_relations_cursor
   INTO @name
    ,@fk_table_name
    ,@pk_table_name
    ,@fk_table_schema
    ,@pk_table_schema
    ,@description
    ,@update_rule
    ,@delete_rule
  END
  CLOSE synch_relations_cursor;
  DEALLOCATE synch_relations_cursor;
  -- set status to 'D' for those relations which not exists in the database but are still in the repository
  UPDATE tr
  SET tr.[status] = 'D'
  FROM tables_relations tr
  INNER JOIN tables t_fk ON t_fk.table_id = tr.fk_table_id
  INNER JOIN tables t_pk ON t_pk.table_id = tr.pk_table_id
  INNER JOIN @synch_tables_list pri ON
     (pri.name = t_fk.name
     AND pri.[schema] = t_fk.[schema]
     ) OR ( pri.name = t_pk.name
     AND pri.[schema] = t_pk.[schema])    
  LEFT JOIN @synch_relations_list pr ON
     pr.fk_table_name = t_fk.name
     AND pr.fk_table_schema = t_fk.[schema]
     AND pr.pk_table_name = t_pk.name
     AND pr.pk_table_schema = t_pk.[schema]
     AND pr.name = tr.name
  WHERE t_fk.database_id = @database_id
   AND t_pk.database_id = @database_id
   AND tr.source = 'DBMS'
   AND pr.name IS NULL
  -- delete columns of those relations deleted from the database
  DELETE
  FROM tables_relations_columns
  WHERE table_relation_id IN (
    SELECT table_relation_id
    FROM tables_relations tr
    INNER JOIN tables t_fk ON t_fk.table_id = tr.fk_table_id
    INNER JOIN tables t_pk ON t_pk.table_id = tr.pk_table_id
    INNER JOIN @synch_tables_list pri ON
       (pri.name = t_fk.name
       AND pri.[schema] = t_fk.[schema]
       ) OR ( pri.name = t_pk.name
       AND pri.[schema] = t_pk.[schema])    
    LEFT JOIN @synch_relations_list pr ON
       pr.fk_table_name = t_fk.name
       AND pr.fk_table_schema = t_fk.[schema]
       AND pr.pk_table_name = t_pk.name
       AND pr.pk_table_schema = t_pk.[schema]
       AND pr.name = tr.name
    WHERE t_fk.database_id = @database_id
     AND t_pk.database_id = @database_id
     AND tr.source = 'DBMS'
     AND pr.name IS NULL
    )
  COMMIT TRANSACTION
  SELECT @result = 0
 END TRY
 BEGIN CATCH
  ROLLBACK TRANSACTION
  SET @result = - 1;
  SET @message = 'synch_relation ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
 END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[synch_relations_columns]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Adam Adamowicz, Joanna Grzonkowska
-- Create date: 2013-02-22
-- Description:	Synchronizing the list of relations with the repository:
--				for each relation updating its data or inserting it if it has not existed so far or setting its status to 'D' (deleted) if it
--				has been deleted from the database.
-- =============================================
ALTER PROCEDURE [dbo].[synch_relations_columns] @synch_relations_columns_list
AS
synch_relations_columns_list READONLY
	,@database_id INT
	-- if column's synchronization succeeded then adding id of its table to this variable 
	-- if number of the occurences of table's id is equal to the count of its relations' column then it means that the relation's synchronization succeed for this table 
	,@succeed_tables_ids NVARCHAR(MAX) OUTPUT
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	BEGIN TRY
		DECLARE @relation_name NVARCHAR(100)
			,@fk_table_name NVARCHAR(100)
			,@fk_table_schema NVARCHAR(250)
			,@fk_column_name NVARCHAR(100)
			,@pk_table_name NVARCHAR(100)
			,@pk_table_schema NVARCHAR(250)
			,@pk_column_name NVARCHAR(100)
			,@ordinal_position INT
			,@column_pk_id INT
			,@column_fk_id INT
			,@table_pk_id INT
			,@table_fk_id INT
			,@relation_id INT
			,@table_relation_column_id INT
		DECLARE synch_relations_columns_cursor CURSOR
		FOR
		SELECT *
		FROM @synch_relations_columns_list
		SET @message = ''
		SET @succeed_tables_ids = ''
		OPEN synch_relations_columns_cursor
		FETCH NEXT
		FROM synch_relations_columns_cursor
		INTO @relation_name
			,@fk_table_name
			,@fk_table_schema
			,@fk_column_name
			,@pk_table_name
			,@pk_table_schema
			,@pk_column_name
			,@ordinal_position
		WHILE @@FETCH_STATUS = 0
		BEGIN
			-- Get relation_id
			SELECT @relation_id = - 1
				,@column_pk_id = - 1
				,@column_fk_id = - 1
				,@table_pk_id = - 1
				,@table_fk_id = - 1;
			-- Get ids of linked tables
			SELECT @table_pk_id = table_id
			FROM [tables]
			WHERE database_id = @database_id
				AND name = @pk_table_name
				AND [schema] = @pk_table_schema
			SELECT @table_fk_id = table_id
			FROM [tables]
			WHERE database_id = @database_id
				AND name = @fk_table_name
				AND [schema] = @fk_table_schema
			IF @table_pk_id <> - 1
				AND @table_fk_id <> - 1
			BEGIN
				SELECT @relation_id = tr.table_relation_id
				FROM tables_relations tr
				WHERE tr.name = @relation_name
					AND tr.source = 'DBMS'
					AND tr.pk_table_id = @table_pk_id
					AND tr.fk_table_id = @table_fk_id
				IF @relation_id <> - 1
				BEGIN
					-- Get ids of linked columns	
					SELECT @column_pk_id = column_id
					FROM [columns]
					WHERE name = @pk_column_name
						AND table_id = @table_pk_id;
					SELECT @column_fk_id = column_id
					FROM [columns]
					WHERE name = @fk_column_name
						AND table_id = @table_fk_id;
					--Checking if the columns from relations exist
					IF (
							@column_fk_id <> - 1
							AND @column_pk_id <> - 1
							)
					BEGIN
						IF (
								SELECT count(1)
								FROM tables_relations_columns
								WHERE [table_relation_id] = @relation_id
									AND [column_fk_id] = @column_fk_id
									AND [column_pk_id] = @column_pk_id
									AND [ordinal_position] = @ordinal_position
									AND [status] = 'A'
								) = 0
						BEGIN
							SELECT @table_relation_column_id = - 1
							SELECT @table_relation_column_id = table_relation_column_id
							FROM tables_relations_columns
							WHERE [table_relation_id] = @relation_id
								AND [column_fk_id] = @column_fk_id
								AND [column_pk_id] = @column_pk_id
							-- Insert or update information about link to repository
							IF @table_relation_column_id <> - 1
							BEGIN
								UPDATE tables_relations_columns
								SET ordinal_position = @ordinal_position
									,status = 'A'
								WHERE table_relation_column_id = @table_relation_column_id
							END
							ELSE
							BEGIN
								INSERT INTO tables_relations_columns (
									[table_relation_id]
									,[column_fk_id]
									,[column_pk_id]
									,[ordinal_position]
									,[status]
									)
								VALUES (
									@relation_id
									,@column_fk_id
									,@column_pk_id
									,@ordinal_position
									,'A'
									)
							END
						END
					END
				END
			END
			SET @succeed_tables_ids += cast(@table_pk_id AS NVARCHAR(MAX)) + ',' + cast(@table_fk_id AS NVARCHAR(MAX)) + ',';
			FETCH NEXT
			FROM synch_relations_columns_cursor
			INTO @relation_name
				,@fk_table_name
				,@fk_table_schema
				,@fk_column_name
				,@pk_table_name
				,@pk_table_schema
				,@pk_column_name
				,@ordinal_position
		END
		CLOSE synch_relations_columns_cursor;
		DEALLOCATE synch_relations_columns_cursor;
		-- delete those relations' columns which are not in the database but are still in the repository
		DELETE
		FROM tables_relations_columns
		WHERE table_relation_column_id IN (
				SELECT trc.table_relation_column_id
				FROM tables_relations_columns trc
				INNER JOIN tables_relations tr ON trc.table_relation_id = tr.table_relation_id
				INNER JOIN tables t_fk ON t_fk.table_id = tr.fk_table_id
				INNER JOIN tables t_pk ON t_pk.table_id = tr.pk_table_id
				INNER JOIN columns c_fk ON c_fk.column_id = trc.column_fk_id
				INNER JOIN columns c_pk ON c_pk.column_id = trc.column_pk_id
				INNER JOIN @synch_relations_columns_list pri ON pri.fk_table_name = t_fk.name
					AND pri.fk_table_schema = t_fk.[schema]
					AND pri.pk_table_name = t_pk.name
					AND pri.pk_table_schema = t_pk.[schema]
				LEFT OUTER JOIN @synch_relations_columns_list pr ON pr.fk_table_name = t_fk.name
					AND pr.fk_table_schema = t_fk.[schema]
					AND pr.pk_table_name = t_pk.name
					AND pr.pk_table_schema = t_pk.[schema]
					AND pr.relation_name = tr.name
					AND pr.fk_column_name = c_fk.name
					AND pr.pk_column_name = c_pk.name
				WHERE t_fk.database_id = @database_id
					AND t_pk.database_id = @database_id
					AND tr.source = 'DBMS'
					AND pr.relation_name IS NULL
				)
		COMMIT TRANSACTION
		SELECT @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'synch_relations_columns ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[synch_table]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:  Adam Adamowicz, Joanna Grzonkowska
-- Create date: 2013-02-20
-- Description: Synchronizing information about a table or a view with the Repository.
-- =============================================
ALTER PROCEDURE [dbo].[synch_table] @name NVARCHAR(100)
 ,@schema NVARCHAR(250)
 ,@database_id INT
 ,@object_type NVARCHAR(100)
 ,@description NTEXT
 ,@definition NTEXT
 ,@dbms_creation_date DATETIME
 ,@dbms_last_modification_date DATETIME
 ,@table_id INT OUTPUT
 ,@result INT OUTPUT
 ,@message NVARCHAR(500) OUTPUT
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;
 BEGIN TRANSACTION
 BEGIN TRY
  DECLARE @id INT
  -- Check if table information exists in repository
  SET @table_id = (
    SELECT table_id
    FROM [tables]
    WHERE [name] = @name
     AND [schema] = @schema
     AND database_id = @database_id
    )
  -- Check if tables' information exists in repository
  IF @table_id IS NOT NULL
  BEGIN
   -- Update information time about table
   UPDATE [tables]
   SET [definition] = @definition
    ,dbms_last_modification_date = @dbms_last_modification_date
    ,[status] = 'A'
   WHERE table_id = @table_id
   -- Update table description if value in repository is null
   UPDATE [tables]
   SET [description] = @description
   WHERE table_id = @table_id
    AND (
     [description] IS NULL
     OR DATALENGTH([description]) = 0
     )
  END
  ELSE
  BEGIN
   -- insert new information to repository
   INSERT INTO tables (
    name
    ,[schema]
    ,database_id
    ,[description]
    ,[definition]
    ,[object_type]
    ,[dbms_creation_date]
    ,dbms_last_modification_date
    ,[status]
    )
   VALUES (
    @name
    ,@schema
    ,@database_id
    ,@description
    ,@definition
    ,@object_type
    ,@dbms_creation_date
    ,@dbms_last_modification_date
    ,'A'
    )
   SET @table_id = @@IDENTITY
  END
  COMMIT TRANSACTION
  SELECT @message = ''
  SELECT @result = 0
 END TRY
 BEGIN CATCH
  ROLLBACK TRANSACTION
  SET @result = - 1;
  SET @message = 'synch_table ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
 END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[synch_triggers]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:  Joanna Grzonkowska
-- Create date: 2013-10-08
-- Description: Synchronizing the list of triggers with the Repository:
--    for each trigger updating its data or inserting it if it has not existed so far or setting its status to 'D' (deleted) if it
--    has been deleted from the database.
-- =============================================
ALTER PROCEDURE [dbo].[synch_triggers] @synch_triggers_list
AS
    synch_triggers_list READONLY
 ,@table_name [nvarchar](250)
 ,@schema NVARCHAR(250)
 ,@database_id INT
 ,@result INT OUTPUT
 ,@message NVARCHAR(500) OUTPUT AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;
 BEGIN TRANSACTION
 BEGIN TRY
  DECLARE @name NVARCHAR(100)
   ,@before BIT
   ,@after BIT
   ,@instead_of BIT
   ,@on_insert BIT
   ,@on_update BIT
   ,@on_delete BIT
   ,@disabled BIT
   ,@definition NVARCHAR(MAX)
   ,@table_id INT
   ,@description NVARCHAR(MAX)
  DECLARE synch_triggers_cursor CURSOR
  FOR
  SELECT *
  FROM @synch_triggers_list
  SET @message = ''
  SELECT @table_id = table_id
  FROM tables
  WHERE database_id = @database_id
   AND name = @table_name
   AND [schema] = @schema
  OPEN synch_triggers_cursor
  FETCH NEXT
  FROM synch_triggers_cursor
  INTO @name
   ,@before
   ,@after
   ,@instead_of
   ,@on_insert
   ,@on_update
   ,@on_delete
   ,@disabled
   ,@definition
   ,@description;
  WHILE @@FETCH_STATUS = 0
  BEGIN
   -- Check if the trigger already exists in repository (and no data change)
   IF (
     SELECT count(1)
     FROM [triggers]
     WHERE name = @name
      AND table_id = @table_id
      AND before = @before
      AND [after] = @after
      AND instead_of = @instead_of
      AND on_insert = @on_insert
      AND on_update = @on_update
      AND on_delete = @on_delete
      AND [disabled] = @disabled
      AND [status] = 'A'
      AND cast([definition] AS NVARCHAR(max)) = cast(@definition AS NVARCHAR(max))
      AND cast([description] AS NVARCHAR(max)) = cast(@description AS NVARCHAR(max))
     ) = 0
   BEGIN
    -- If the trigger exists but some data changed update information
    IF (
      SELECT count(1)
      FROM [triggers]
      WHERE [name] = @name
       AND table_id = @table_id
      ) > 0
    BEGIN
     UPDATE [triggers]
     SET [before] = @before
      ,[after] = @after
      ,instead_of = @instead_of
      ,on_insert = @on_insert
      ,on_update = @on_update
      ,on_delete = @on_delete
      ,[disabled] = @disabled
      ,[status] = 'A'
      ,[definition] = @definition
     WHERE [name] = @name
      AND table_id = @table_id;
     -- Override trigger's comments from DBMS only if it's empty in repository
     UPDATE [triggers]
     SET [description] = @description
     WHERE [name] = @name
      AND table_id = @table_id
      AND (
       [description] IS NULL
       OR DATALENGTH([description]) = 0
       )
    END
    ELSE
    BEGIN
     -- If the trigger doesn't exists insert new information to the Repository
     INSERT INTO [triggers] (
      [name]
      ,[table_id]
      ,[before]
      ,[after]
      ,instead_of
      ,on_insert
      ,on_update
      ,on_delete
      ,[disabled]
      ,[status]
      ,[definition]
      ,[description]
      )
     VALUES (
      @name
      ,@table_id
      ,@before
      ,@after
      ,@instead_of
      ,@on_insert
      ,@on_update
      ,@on_delete
      ,@disabled
      ,'A'
      ,@definition
      ,@description
      )
    END
   END
   FETCH NEXT
   FROM synch_triggers_cursor
   INTO @name
    ,@before
    ,@after
    ,@instead_of
    ,@on_insert
    ,@on_update
    ,@on_delete
    ,@disabled
    ,@definition
    ,@description;
  END
  CLOSE synch_triggers_cursor;
  DEALLOCATE synch_triggers_cursor;
  -- set status to 'D' (deleted) for all columns which are not in the database but are still in the repository
  UPDATE triggers
  SET triggers.[status] = 'D'
  FROM [triggers] tr
  INNER JOIN [tables] tb ON tr.table_id = tb.table_id
  WHERE tb.database_id = @database_id
   AND tb.table_id = @table_id
   AND tr.name NOT IN (
    SELECT name
    FROM @synch_triggers_list
    )
  COMMIT TRANSACTION
  SELECT @result = 0
 END TRY
 BEGIN CATCH
  ROLLBACK TRANSACTION
  SET @result = - 1;
  SET @message = 'synch_triggers ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
 END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[synch_unique_constraints]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:  Joanna Grzonkowska
-- Create date: 2013-10-21
-- Description: Synchronizing the list of unique constraints on the given table with the repository:
--    for each unique constraint updating its data or inserting it if it has not existed so far or setting its status to 'D' (deleted) if it
--    has been deleted from the database.
-- =============================================
ALTER PROCEDURE [dbo].[synch_unique_constraints] @synch_unique_constraints_list
AS
    synch_unique_constraints_list READONLY
 ,@table_name NVARCHAR(250)
 ,@schema NVARCHAR(250)
 ,@database_id INT
 ,@result INT OUTPUT
 ,@message NVARCHAR(500) OUTPUT AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;
 BEGIN TRANSACTION
 BEGIN TRY
  DECLARE @name NVARCHAR(100)
   ,@primary_key BIT
   ,@table_id INT
   ,@unique_constraint_id INT
   ,@description NVARCHAR(MAX)
   ,@disabled BIT
  DECLARE synch_unique_constraints_cursor CURSOR
  FOR
  SELECT *
  FROM @synch_unique_constraints_list
  SET @message = ''
  SELECT @table_id = table_id
  FROM tables
  WHERE database_id = @database_id
   AND name = @table_name
   AND [schema] = @schema
  OPEN synch_unique_constraints_cursor
  FETCH NEXT
  FROM synch_unique_constraints_cursor
  INTO @name
   ,@primary_key
   ,@description
   ,@disabled
  WHILE @@FETCH_STATUS = 0
  BEGIN
   -- Check if constraint already exists in repository (and no data change)
   IF (
     SELECT COUNT(1)
     FROM unique_constraints
     WHERE name = @name
      AND table_id = @table_id
      AND primary_key = @primary_key
      AND [source] = 'DBMS'
      AND [status] = 'A'
      AND [disabled] = @disabled
      AND cast([description] AS NVARCHAR(max)) = cast(@description AS NVARCHAR(max))
     ) = 0
   BEGIN
    -- If unique constraint exists but some data changed update information
    IF (
      SELECT COUNT(1)
      FROM unique_constraints
      WHERE [name] = @name
       AND table_id = @table_id
      ) > 0
    BEGIN
     UPDATE unique_constraints
     SET primary_key = @primary_key
      ,[source] = 'DBMS'
      ,[status] = 'A'
      ,[disabled] = @disabled
     WHERE [name] = @name
      AND table_id = @table_id;
     -- Override index's comments from DBMS only if it's empty in repository
     UPDATE unique_constraints
     SET [description] = @description
     WHERE [name] = @name
      AND table_id = @table_id
      AND (
       [description] IS NULL
       OR DATALENGTH([description]) = 0
       )
    END
    ELSE
    BEGIN
     -- If constraint doesn't exists insert new information to repository
     INSERT INTO unique_constraints (
      [name]
      ,[table_id]
      ,[source]
      ,[primary_key]
      ,[status]
      ,[description]
      ,[disabled]
      )
     VALUES (
      @name
      ,@table_id
      ,'DBMS'
      ,@primary_key
      ,'A'
      ,@description
      ,@disabled
      )
    END
   END
   FETCH NEXT
   FROM synch_unique_constraints_cursor
   INTO @name
    ,@primary_key
    ,@description
    ,@disabled
  END
  CLOSE synch_unique_constraints_cursor;
  DEALLOCATE synch_unique_constraints_cursor;
  -- set status to 'D' for those unique constraints which not exists in the database but are still in the repository
  UPDATE u
  SET u.[status] = 'D'
  FROM unique_constraints u
  JOIN tables t ON u.table_id = t.table_id
  JOIN databases d ON t.database_id = d.database_id
  WHERE t.table_id = @table_id
   AND d.database_id = @database_id
   AND u.source = 'DBMS'
   AND u.name NOT IN (
    SELECT name
    FROM @synch_unique_constraints_list
    )
  -- delete columns of those constraints deleted from the database
  DELETE
  FROM unique_constraints_columns
  WHERE unique_constraint_id IN (
    SELECT unique_constraint_id
    FROM unique_constraints u
    JOIN tables t ON u.table_id = t.table_id
    WHERE t.table_id = @table_id
     AND u.source = 'DBMS'
     AND u.name NOT IN (
      SELECT name
      FROM @synch_unique_constraints_list
      )
    )
  COMMIT TRANSACTION
  SELECT @result = 0
 END TRY
 BEGIN CATCH
  ROLLBACK TRANSACTION
  SET @result = - 1;
  SET @message = 'synch_unique_constraints ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
 END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[synch_unique_constraints_columns]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2013-10-21
-- Description:	Synchronizing given unique constraints' columns with the repository
-- =============================================
ALTER PROCEDURE [dbo].[synch_unique_constraints_columns] @synch_unique_constraints_columns_list
AS
    synch_unique_constraints_columns_list READONLY
	,@table_name NVARCHAR(250)
	,@schema NVARCHAR(250)
	,@database_id INT
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	BEGIN TRY
		DECLARE @column_id INT
			,@column_name NVARCHAR(100)
			,@ordinal_position INT
			,@constraint_name NVARCHAR(100)
			,@unique_constraint_id INT
			,@table_id INT
		DECLARE synch_unique_constraints_columns_cursor CURSOR
		FOR
		SELECT *
		FROM @synch_unique_constraints_columns_list
		SET @message = ''
		SELECT @table_id = table_id
		FROM tables
		WHERE database_id = @database_id
			AND name = @table_name
			AND [schema] = @schema
		OPEN synch_unique_constraints_columns_cursor
		FETCH NEXT
		FROM synch_unique_constraints_columns_cursor
		INTO @column_name
			,@ordinal_position
			,@constraint_name
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SELECT @column_id = - 1
			SELECT @column_id = column_id
			FROM [columns] c
			INNER JOIN [tables] t ON c.table_id = t.table_id
			WHERE t.table_id = @table_id
				AND c.name = @column_name
			IF @column_id <> - 1
			BEGIN
				-- Check if already exists in repository (and no data change)
				IF (
						SELECT count(1)
						FROM unique_constraints_columns c
						INNER JOIN unique_constraints u ON c.unique_constraint_id = u.unique_constraint_id
						INNER JOIN tables t ON t.table_id = u.table_id
						WHERE t.table_id = @table_id
							AND u.name = @constraint_name
							AND c.column_id = @column_id
							AND c.ordinal_position = @ordinal_position
						) = 0
				BEGIN
					DECLARE @unique_constraint_column_id INT;
					-- If exists but some data changed update information
					SELECT @unique_constraint_column_id = - 1
					SELECT @unique_constraint_column_id = c.unique_constraint_column_id
					FROM unique_constraints_columns c
					INNER JOIN unique_constraints u ON c.unique_constraint_id = u.unique_constraint_id
					INNER JOIN tables t ON t.table_id = u.table_id
					WHERE t.table_id = @table_id
						AND u.name = @constraint_name
						AND c.column_id = @column_id;
					IF @unique_constraint_column_id <> - 1
					BEGIN
						UPDATE unique_constraints_columns
						SET ordinal_position = @ordinal_position
						WHERE unique_constraint_column_id = @unique_constraint_column_id;
					END
					ELSE
					BEGIN
						-- If doesn't exists insert new information to repository 
						SELECT @unique_constraint_id = - 1
						SELECT @unique_constraint_id = u.unique_constraint_id
						FROM unique_constraints u
						INNER JOIN tables t ON t.table_id = u.table_id
						WHERE t.table_id = @table_id
							AND u.name = @constraint_name;
						IF @unique_constraint_id <> - 1
						BEGIN
							INSERT INTO unique_constraints_columns (
								[unique_constraint_id]
								,[column_id]
								,[ordinal_position]
								)
							VALUES (
								@unique_constraint_id
								,@column_id
								,@ordinal_position
								)
						END
					END
				END
			END
			FETCH NEXT
			FROM synch_unique_constraints_columns_cursor
			INTO @column_name
				,@ordinal_position
				,@constraint_name
		END
		CLOSE synch_unique_constraints_columns_cursor;
		DEALLOCATE synch_unique_constraints_columns_cursor;
		-- delete those constraints' columns which are not in the database but are still in the repository
		DELETE
		FROM unique_constraints_columns
		WHERE unique_constraint_column_id IN (
				SELECT c.unique_constraint_column_id
				FROM unique_constraints_columns c
				JOIN columns col ON c.column_id = col.column_id
				JOIN unique_constraints u ON c.unique_constraint_id = u.unique_constraint_id
				JOIN tables t ON u.table_id = t.table_id
				JOIN @synch_unique_constraints_columns_list pc ON pc.constraint_name = u.name
				WHERE t.table_id = @table_id
					AND u.source = 'DBMS'
					AND NOT EXISTS (
						SELECT 1
						FROM @synch_unique_constraints_columns_list pc
						WHERE pc.constraint_name = u.name
							AND pc.column_name = col.name
						)
				)
		COMMIT TRANSACTION
		SELECT @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'synch_unique_constraints_columns ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[table_delete]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2013-09-23
-- Description:	Deleting the table/view from the Repository
-- =============================================
ALTER PROCEDURE [dbo].[table_delete] @table_id INT
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	BEGIN TRY
		SELECT column_id
		INTO #deleted_columns
		FROM columns
		WHERE table_id = @table_id
		--CASCADE COLUMNS SIMULATION
		DELETE
		FROM [unique_constraints_columns]
		WHERE column_id IN (SELECT column_id FROM #deleted_columns);
		DELETE
		FROM [tables_relations_columns]
		WHERE [column_fk_id] IN (SELECT column_id FROM #deleted_columns)
			OR [column_pk_id] IN (SELECT column_id FROM #deleted_columns);
		--CASCADE SIMULATION
		DELETE
		FROM [tables_modules]
		WHERE table_id = @table_id;
		DELETE
		FROM [tables_relations]
		WHERE [pk_table_id] = @table_id
			OR [fk_table_id] = @table_id;
		--ACTUAL DELETE
		DELETE
		FROM tables
		WHERE table_id = @table_id;
		COMMIT TRANSACTION
		SELECT @message = ''
		SELECT @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'table_delete ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[table_update]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Adam Adamowicz, Joanna Grzonkowska
-- Create date: 2013-02-20
-- Description:	Updating information about a table
--				When @description or @modules_id are equal to '' then these fields are not updated
-- =============================================
ALTER PROCEDURE [dbo].[table_update]
	-- Parameters
	@table_id INT
	,@title NVARCHAR(250)
	,@description NTEXT
	,@modules_id NVARCHAR(MAX)
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	BEGIN TRY
		SELECT @message = ''
		IF @modules_id is not null
		BEGIN
			SET @modules_id = replace(@modules_id, ' ', '');
			SELECT *
			INTO #new_modules_id
			FROM [split_strings](@modules_id, ',');
			DELETE  -- delete unchecked modules 
			FROM tables_modules
			WHERE table_id = @table_id
			AND module_id NOT IN (SELECT value FROM #new_modules_id)
			INSERT INTO tables_modules (  -- insert new checked modules
				table_id,
				module_id
		 	)
			(SELECT @table_id, value FROM #new_modules_id nmi 
				WHERE value NOT IN (SELECT module_id FROM tables_modules WHERE table_id = @table_id) )
		END
		-- Update information about Table
		UPDATE [tables]
		SET [title] = @title
			,[description] = CASE 
				WHEN isnull(cast(@description AS NVARCHAR(max)), 'empty') = ''
					THEN [description]
				ELSE @description
				END
		WHERE table_id = @table_id
		COMMIT TRANSACTION
		SELECT @message = ''
		SELECT @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'table_update ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[triggers_delete]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2014-06-23
-- Description:	Deleting a table's triggers which have id in the given @objectsList
-- =============================================
ALTER PROCEDURE [dbo].[triggers_delete]
	@objectsList AS objects_id_list READONLY,
	@result int OUTPUT,
	@message NVARCHAR(500) OUTPUT
AS
BEGIN
  SET NOCOUNT ON;
  	BEGIN TRANSACTION
	BEGIN TRY
		DELETE FROM	
			[triggers]
		WHERE 
			trigger_id IN (SELECT [object_id] FROM @objectsList)
		COMMIT TRANSACTION
		SET @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'triggers_delete ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH	
END

GO

/****** Object:  StoredProcedure [dbo].[triggers_update]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2013-10-18
-- Description:	Updating information about the triggers.
-- =============================================
ALTER PROCEDURE [dbo].[triggers_update] 
	 @triggers_list AS triggers_list READONLY
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		UPDATE [triggers]
			SET [description] = tl.[description]
			FROM @triggers_list tl
			WHERE [triggers].trigger_id = tl.trigger_id
		COMMIT TRANSACTION
		SET @result = 0;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'triggers_update ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[unique_constraints_delete]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2014-05-09
-- Description:	Deleting information about unique constraints and theirs columns
-- =============================================
ALTER PROCEDURE [dbo].[unique_constraints_delete] 
	@objectsList AS objects_id_list READONLY
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	BEGIN TRY
		DELETE
		FROM unique_constraints
		WHERE unique_constraint_id IN (SELECT [object_id] FROM @objectsList)
		COMMIT TRANSACTION
		SET @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = 'unique_constraints_delete ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO

/****** Object:  StoredProcedure [dbo].[unique_constraints_insert_or_update]    Script Date: 2015-06-29 15:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2014-05-23
-- Description:	Inserting or updating information about a given constraint
-- =============================================
ALTER PROCEDURE [dbo].[unique_constraints_insert_or_update]
	-- Parameters
	@unique_constraint_id INT
	,@table_id INT
	,@source NVARCHAR(50)
	,@name NVARCHAR(250)
	,@description NTEXT
	,@primary_key BIT
	,@unique_constraints_columnsList AS unique_constraints_columns_list READONLY
	,@result INT OUTPUT
	,@message NVARCHAR(500) OUTPUT AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		SET @message = '';
		SELECT TOP 1 1
		FROM unique_constraints
		WHERE unique_constraint_id = @unique_constraint_id;
		IF (@@ROWCOUNT > 0)
		BEGIN
			UPDATE [unique_constraints]
			SET [source] = @source
				,[name] = @name
				,[description] = @description
				,[primary_key] = @primary_key
				,[last_modification_date] = GETDATE()
				,[modified_by] = suser_sname()
			WHERE unique_constraint_id = @unique_constraint_id
		END
		ELSE
		BEGIN
			INSERT INTO [unique_constraints] (
				[table_id]
				,[source]
				,[name]
				,[description]
				,[primary_key]
				,[status]
				,[creation_date]
				,[created_by]
				,[last_modification_date]
				,[modified_by]
				)
			VALUES (
				@table_id
				,@source
				,@name
				,@description
				,@primary_key
				,'A'
				,GETDATE()
				,suser_sname()
				,GETDATE()
				,suser_sname()
				);
			SET @unique_constraint_id = @@IDENTITY
		END
		-- inserting or deleting constraint's columns
		SELECT unique_constraint_column_id
			,column_id
			,ordinal_position
		INTO #new_constraints_columns
		FROM @unique_constraints_columnsList;
		SELECT unique_constraint_column_id
			,column_id
			,ordinal_position
		INTO #current_constraints_columns
		FROM unique_constraints_columns
		WHERE unique_constraint_id = @unique_constraint_id;
		DECLARE @unique_constraint_column_id INT
		DECLARE @column_id INT
		DECLARE @ordinal_position INT
		-- constraints columns to delete
		DECLARE links_delete_cursor CURSOR
		FOR
		SELECT *
		FROM #current_constraints_columns
		EXCEPT
		SELECT *
		FROM #new_constraints_columns
		OPEN links_delete_cursor
		FETCH NEXT
		FROM links_delete_cursor
		INTO @unique_constraint_column_id
			,@column_id
			,@ordinal_position;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			DELETE
			FROM [unique_constraints_columns]
			WHERE unique_constraint_column_id = @unique_constraint_column_id
			FETCH NEXT
			FROM links_delete_cursor
			INTO @unique_constraint_column_id
				,@column_id
				,@ordinal_position;
		END
		CLOSE links_delete_cursor;
		DEALLOCATE links_delete_cursor;
		-- constraints columns to insert
		DECLARE links_insert_cursor CURSOR
		FOR
		SELECT *
		FROM #new_constraints_columns
		EXCEPT
		SELECT *
		FROM #current_constraints_columns
		OPEN links_insert_cursor
		FETCH NEXT
		FROM links_insert_cursor
		INTO @unique_constraint_column_id
			,@column_id
			,@ordinal_position;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			-- adding given column for this constraint only if there is no such column already
			SELECT TOP 1 1
			FROM unique_constraints_columns
			WHERE unique_constraint_id = @unique_constraint_id
				AND column_id = @column_id;
			IF (@@ROWCOUNT = 0)
			BEGIN
				INSERT INTO [unique_constraints_columns] (
					[unique_constraint_id]
					,[column_id]
					,[ordinal_position]
					,[status]
					)
				VALUES (
					@unique_constraint_id
					,@column_id
					,@ordinal_position
					,'A'
					)
			END
			FETCH NEXT
			FROM links_insert_cursor
			INTO @unique_constraint_column_id
				,@column_id
				,@ordinal_position;
		END
		CLOSE links_insert_cursor;
		DEALLOCATE links_insert_cursor;
		COMMIT TRANSACTION
		SET @result = 0
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @result = - 1;
		SET @message = @message + 'unique_constraints_insert_or_update ERROR: ' + CAST(@@ERROR AS NVARCHAR(8)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
END

GO


/****** ---------- FUNCTIONS ----------  ******/
/****** Object:  UserDefinedFunction [dbo].[split_strings]    Script Date: 2015-06-29 15:47:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION [dbo].[split_strings]
(
@input_string nvarchar(MAX)
,@separator nvarchar(MAX)
)
RETURNS @value_table TABLE (value nvarchar(MAX))
AS
BEGIN
    DECLARE @separator_index INT, @total_length INT, @start_index INT, @value nvarchar(MAX)
    SET @total_length=LEN(@input_string)
    SET @start_index = 1
    IF @separator IS NULL RETURN
    WHILE @start_index <= @total_length
    BEGIN
        SET @separator_index = CHARINDEX(@separator, @input_string, @start_index)
        IF @separator_index > 0
        BEGIN
            SET @value = SUBSTRING(@input_string, @start_index, @separator_index-@start_index)
            SET @start_index = @separator_index + 1
        END
        ELSE
        BEGIN
            Set @value = SUBSTRING(@input_string, @start_index, @total_length-@start_index+1)
            SET @start_index = @total_length+1
        END
        INSERT INTO @value_table
        (value)
        VALUES
        (@value)
    END
    RETURN
END

GO

/****** Object:  UserDefinedFunction [dbo].[get_constraint_columns]    Script Date: 2015-06-29 15:47:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Grzonkowska	Joanna
-- Create date: 2014-05-22
-- Description:	Joining columns of the given constraint into one string
-- =============================================
ALTER FUNCTION [dbo].[get_constraint_columns]
(
	@unique_constraint_id int
)
RETURNS nvarchar(500)
AS
BEGIN
	DECLARE @column_name nvarchar(100),
			@all_joined_columns nvarchar(500)
	set @all_joined_columns = ''
	DECLARE cur CURSOR FOR 
	  select c.name
	  from unique_constraints_columns cc join columns c on cc.column_id = c.column_id
	  where cc.unique_constraint_id = @unique_constraint_id
	  order by cc.ordinal_position;
	OPEN cur
	FETCH NEXT FROM cur 
	INTO @column_name
	WHILE @@FETCH_STATUS = 0
	BEGIN
		if @all_joined_columns = ''
		begin
			set @all_joined_columns = @column_name;
		end
		else
		begin
			set @all_joined_columns = @all_joined_columns + ', ' + @column_name;
		end
		FETCH NEXT FROM cur INTO @column_name
    END
	CLOSE cur;
	DEALLOCATE cur;
	RETURN @all_joined_columns
END

GO

/****** Object:  UserDefinedFunction [dbo].[get_relation_join_condition]    Script Date: 2015-06-29 15:47:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Grzonkowska	Joanna
-- Create date: 2014-05-22
-- Description:	Joining conditions of the given relation into one string
-- =============================================
ALTER FUNCTION [dbo].[get_relation_join_condition]
(
	@table_relation_id int
)
RETURNS nvarchar(500)
AS
BEGIN
	DECLARE @all_join_conditions nvarchar(500)
	-- String concatenation
	select @all_join_conditions = STUFF((
      select CHAR(13)+CHAR(10) + join_condition
      from (select t2.name + '.' + c2.name + ' = ' + c1.name as join_condition 
			FROM            
			tables_relations AS tr INNER JOIN
			tables AS t2 ON tr.fk_table_id = t2.table_id INNER JOIN
			tables_relations_columns trc on tr.table_relation_id = trc.table_relation_id
			INNER JOIN columns as c1 on trc.column_pk_id = c1.column_id
			INNER JOIN columns as c2 on trc.column_fk_id = c2.column_id
			where trc.table_relation_id = @table_relation_id
	) t1
        for xml path(''), type ).value('.', 'varchar(max)'), 1, 2, '') 
	RETURN @all_join_conditions
END

GO

/****** Object:  UserDefinedFunction [dbo].[procedures_modules_join_module_id]    Script Date: 2015-06-29 15:47:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Grzonkowska Joanna
-- Create date: 2014-04-27
-- Description:	Joins id of all modules to which a given procedure belongs.
-- =============================================
ALTER FUNCTION [dbo].[procedures_modules_join_module_id]
(
	@procedure_id int
)
RETURNS nvarchar(max)
AS
BEGIN
	declare @result nvarchar(max)
	select @result = COALESCE(@result + ', ', '') + cast(module_id as nvarchar(8))
	from procedures_modules where procedure_id = @procedure_id
	RETURN @result
END

GO

/****** Object:  UserDefinedFunction [dbo].[tables_modules_join_module_id]    Script Date: 2015-06-29 15:47:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Grzonkowska Joanna
-- Create date: 2014-04-27
-- Description:	Joins id of all modules to which a given table belongs.
-- =============================================
ALTER FUNCTION [dbo].[tables_modules_join_module_id]
(
	@table_id int
)
RETURNS nvarchar(max)
AS
BEGIN
	declare @result nvarchar(max)
	select @result = COALESCE(@result + ', ', '') + cast(module_id as nvarchar(8))
	from tables_modules where table_id = @table_id
	RETURN @result
END

GO

--!!!!!!!!!!!!!!!POCZĄTEK ZMIAN ŁUKASZA!!!!!!!!!!!!!!!!!!!!

--dodanie kolumny
--https://support.logicsystems.com.pl/pm/issue.action?issueId=10088
alter table dbo.tables_relations
add [disabled] [bit] NULL
GO

--ZMIANA W PROCEDURZE ZWRACAJACEJ WARUNEK JOIN
--https://support.logicsystems.com.pl/pm/issue.action?issueId=10122
/****** Object:  UserDefinedFunction [dbo].[get_relation_join_condition]    Script Date: 2015-06-18 12:24:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Grzonkowska	Joanna
-- Create date: 2014-05-22
-- Description:	Joining conditions of the given relation into one string
-- =============================================
ALTER FUNCTION [dbo].[get_relation_join_condition]
(
	@table_relation_id int
)
RETURNS NVARCHAR(500)
AS
BEGIN
	DECLARE @all_join_conditions NVARCHAR(500)
	-- String concatenation
	--22.06.2015 Ł.Gil
	--Modifications of join conditions(table name with schema)
	select @all_join_conditions = STUFF((
      select CHAR(13)+CHAR(10) + join_condition + ' and'
      from (select 
			c1.name + ' = ' + case (select [type] from databases where database_id = t2.database_id) when 'SQLSERVER' then
			t2.[schema] + '.'
			else '' 
			end
			+ t2.name + '.' + c2.name as join_condition 
			FROM            
			tables_relations AS tr INNER JOIN
			tables AS t2 ON tr.fk_table_id = t2.table_id INNER JOIN
			tables_relations_columns trc on tr.table_relation_id = trc.table_relation_id
			INNER JOIN columns as c1 on trc.column_pk_id = c1.column_id
			INNER JOIN columns as c2 on trc.column_fk_id = c2.column_id
			where trc.table_relation_id = @table_relation_id
	) t1
        for xml path(''), type ).value('.', 'varchar(max)'), 1, 2, '')
		--substring for remove last ' and' string
	RETURN SUBSTRING(@all_join_conditions, 1, LEN(@all_join_conditions) - 4)
END
GO

/****** Object:  StoredProcedure [dbo].[synch_columns]    Script Date: 2015-06-23 11:40:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:  Adam Adamowicz, Joanna Grzonkowska
-- Create date: 2013-02-20
-- Description: Synchronizing the list of columns with the repository:
--    for each column updating its data or inserting it if it has not existed so far or setting its status to 'D' (deleted) if it
--    has been deleted from the database.
-- =============================================
ALTER PROCEDURE [dbo].[synch_columns] @synch_columns_list
AS
    synch_columns_list READONLY
 ,@table_name [nvarchar](250)
 ,@schema NVARCHAR(250)
 ,@database_id INT
 ,@result INT OUTPUT
 ,@message NVARCHAR(500) OUTPUT AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;
 BEGIN TRANSACTION
 BEGIN TRY
  DECLARE @name NVARCHAR(100)
   ,@position INT
   ,@datatype NVARCHAR(100)
   ,@description NVARCHAR(MAX)
   ,@constraint_type NVARCHAR(1)
   ,@data_length NVARCHAR(20)
   ,@nullable BIT
   ,@table_id INT
   ,@primary_key CHAR(1)
   ,@default_def NVARCHAR(4000)
   ,@identity_def NVARCHAR(50)
  DECLARE synch_columns_cursor CURSOR
  FOR
  SELECT *
  FROM @synch_columns_list
  SET @message = ''
  
  set @table_id = null
  SELECT @table_id = table_id
  FROM tables
  WHERE database_id = @database_id
   AND name = @table_name
   AND [schema] = @schema
  
  --23.06.2015 Ł.Gil
  --special message when unable to find table with specified name
  if @table_id is null begin
  ROLLBACK TRANSACTION
  SET @result = - 2;
  return
  end

  OPEN synch_columns_cursor
  FETCH NEXT
  FROM synch_columns_cursor
  INTO @name
   ,@position
   ,@datatype
   ,@description
   ,@constraint_type
   ,@data_length
   ,@nullable
   ,@default_def
   ,@identity_def;
  WHILE @@FETCH_STATUS = 0
  BEGIN
   SELECT @primary_key = CASE
     WHEN @constraint_type = 'P'
      THEN 1
     ELSE 0
     END
   -- Check if column exists in repository (and no data change)
   IF (
     SELECT count(1)
     FROM [columns]
     WHERE [name] = @name
      AND [datatype] = @datatype
      AND cast([description] AS NVARCHAR(max)) = cast(@description AS NVARCHAR(max))
      AND table_id = @table_id
      AND [ordinal_position] = @position
      AND [status] = 'A'
      AND [data_length] = @data_length
      AND [primary_key] = @primary_key
      AND [nullable] = @nullable
      AND [default_def] = @default_def
      AND [identity_def] = @identity_def
     ) = 0
   BEGIN
    -- If column exists but some data changed update information
    IF (
      SELECT count(1)
      FROM [columns]
      WHERE [name] = @name
       AND table_id = @table_id
      ) > 0
    BEGIN
     UPDATE [columns]
     SET [datatype] = @datatype
      ,[ordinal_position] = @position
      ,[status] = 'A'
      ,[data_length] = @data_length
      ,[primary_key] = @primary_key
      ,[nullable] = @nullable
      ,[default_def] = @default_def
      ,[identity_def] = @identity_def
     WHERE [name] = @name
      AND table_id = @table_id;
     -- Override column's comments from DBMS only if it's empty in repository
     UPDATE [columns]
     SET [description] = @description
     WHERE [name] = @name
      AND table_id = @table_id
      AND (
       [description] IS NULL
       OR DATALENGTH([description]) = 0
       )
    END
    ELSE
    BEGIN
     -- If column doesn't exists insert new information to repository
     INSERT INTO [columns] (
      [table_id]
      ,[name]
      ,[ordinal_position]
      ,[description]
      ,[datatype]
      ,[status]
      ,[primary_key]
      ,[data_length]
      ,[nullable]
      ,[default_def]
      ,[identity_def]
      )
     VALUES (
      @table_id
      ,@name
      ,@position
      ,@description
      ,@datatype
      ,'A'
      ,@primary_key
      ,@data_length
      ,@nullable
      ,@default_def
      ,@identity_def
      )
    END
   END
   FETCH NEXT
   FROM synch_columns_cursor
   INTO @name
    ,@position
    ,@datatype
    ,@description
    ,@constraint_type
    ,@data_length
    ,@nullable
    ,@default_def
    ,@identity_def;
  END
  CLOSE synch_columns_cursor;
  DEALLOCATE synch_columns_cursor;
  -- set status to 'D' (deleted) for all columns which are not in the database but are still in the repository
  UPDATE c
  SET c.[status] = 'D'
  FROM [columns] c
  INNER JOIN [tables] t ON c.table_id = t.table_id
  WHERE t.table_id = @table_id
   AND c.name NOT IN (
    SELECT name
    FROM @synch_columns_list
    )
  COMMIT TRANSACTION
  SELECT @result = 0
 END TRY
 BEGIN CATCH
  ROLLBACK TRANSACTION
  SET @result = - 1;
  SET @message = 'synch_columns ERROR: ' + cast(@@ERROR AS NVARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
 END CATCH
END

GO

update licenses
set [key] = 'JMJABKKKDHNBMHIOLPLKPJIOHDCPOHEBLPLDAPCCLAGOLEMNCDOEKCPAGLBBOBECANLBPLDKCNGJMDKMCNKHBHPDPPDGINEKCAOLAEEBFGBGJACEOIFIACLMCOGPKDGHCPHODLFAHBFJBFALNKMEBLAOPFOPPAOCDINJJKEHDIJJKAIGHOMOPHAPEGLGNPEHCFCAHPEHCIOCOOPFANCPKEJLBPADJICODHDDKGLFPDGBMAGMNPPLJLFBBOFAKJIF'
where [key] = 'AKGPLPKKCIHBNAONCLCDMCBCDHMPGKDCEMIIMCKCPDMIGEPKIDNNDPMOBOHGADEGEHKBLNMHEHFOGDIDGGGFFOOHKICAODFHEOANOAOMHNDHDEOJHCPPLCANIMJPDLIPNGEFMGJMKNENLPKINAGIDGOMFFOMEKJMNALCPKOEOELIKOPPFHLBAEIJIGILBKKFMNNNFAFPEABNDDGHIIMLLNAIFADGLHCAKFIHJHNKADHNLAAHFPIPJAGKCAMHILIK'
or [key] = 'LPDEDKDFLJABJKLCOCBBONBKJCIMDBMAKCMDPPAKIPHFEGKFCOCNBABGAHMKJKAJKNGBHIPDEIHIFOGAJEKMLFHHDEEEHOGJEHLLEMJLJAAOLALLABLOMJPJPHEIDALBDBLJNPKCMMDPCBOPGNDBJJFNAAJFNOOACOOBIMCAPLEEIFMNJHJJDDDBIAENLCJHLHPDNHFKPEFKOMJNAAAEPEPBNDOLDBKGPFEFECFKAGNNPHNDNIOKPPGEHCLGCEAD'

update version set stable = 1 where version = 4
GO