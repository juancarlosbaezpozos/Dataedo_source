insert into version (version, [update], stable) values (3, 0, 0)
GO

alter table [tables] add [exists_in_DBMS] bit;
alter table [procedures] add [exists_in_DBMS] bit;

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Joanna Grzonkowska
-- Create date: 2015-01-05
-- Description:	Clear the tables' and procedures' flag which denotes if an object exists in DBMS.
-- =============================================
CREATE PROCEDURE [dbo].[objects_clear_exists_flag]
	 @result INT OUTPUT
	,@message VARCHAR(500) OUTPUT 
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
		SET @message = 'objects_clear_exists_flag ERROR: ' + cast(@@ERROR AS VARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
	END CATCH
	
END

GO
GRANT EXECUTE ON [dbo].[objects_clear_exists_flag] TO [users]

GO
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
	,@object_type VARCHAR(100)
	,@database_id INT
	,@dbms_modify_date DATETIME
	,@result CHAR OUTPUT
	,@message VARCHAR(500) OUTPUT
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
update version set stable = 1 where version = 3
GO