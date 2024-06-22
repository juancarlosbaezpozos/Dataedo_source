insert into version (version, [update], stable) values (4, 1, 0)
GO

/****** Object:  StoredProcedure [dbo].[object_check_synchronization]    Script Date: 2015-11-03 14:28:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2013-09-06
-- Description:	Check if an object (table/view/procedure/function) is ignored/new/unsynchronized/synchronized.
-- =============================================
-- 31.08.15 Ł.Gil
-- Adding object_type column in where clauses
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
			  AND object_type = @object_type
			  AND database_id = @database_id
	END
	ELSE
	BEGIN
		UPDATE [procedures]
		SET [exists_in_DBMS] = 1
		WHERE name = @name
			  AND [schema] = @schema
			  AND object_type = @object_type
			  AND database_id = @database_id
	END
	IF EXISTS (
			SELECT 1
			FROM [ignored_objects]
			WHERE name = @name
				AND [schema] = @schema
				AND object_type = @object_type
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
					AND object_type = @object_type
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
					AND object_type = @object_type
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
					AND object_type = @object_type
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
					AND object_type = @object_type
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

/****** Object:  StoredProcedure [dbo].[synch_procedure]    Script Date: 2015-11-03 14:10:58 ******/
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
	AND object_type = @object_type
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

/****** Object:  StoredProcedure [dbo].[synch_table]    Script Date: 2015-11-03 14:07:53 ******/
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
	AND object_type = @object_type
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

update version set stable = 1 where version = 4 and [update] = 1
GO