alter table version
add stable bit
GO
alter table version
add default(0) for [update]
GO
insert into version (version, [update], stable) values (2, 0, 0)
GO
-- additional column in synch_triggers_list type: definition
IF EXISTS (select * from sys.types where name = 'synch_triggers_list')
BEGIN
 DROP PROCEDURE synch_triggers;
 DROP TYPE synch_triggers_list
END
GO
CREATE TYPE [dbo].[synch_triggers_list] AS TABLE(
 [name] [nvarchar](250) NULL,
 [before] [bit] NULL,
 [after] [bit] NULL,
 [instead_of] [bit] NULL,
 [on_insert] [bit] NULL,
 [on_update] [bit] NULL,
 [on_delete] [bit] NULL,
 [disabled] [bit] NULL,
 [definition] [ntext] NULL,
 [description] [ntext] NULL
)
GO
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
CREATE PROCEDURE [dbo].[synch_triggers] @synch_triggers_list
AS
    synch_triggers_list READONLY
 ,@table_name [nvarchar](250)
 ,@schema NVARCHAR(250)
 ,@database_id INT
 ,@result INT OUTPUT
 ,@message VARCHAR(500) OUTPUT AS

BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

 BEGIN TRANSACTION

 BEGIN TRY
  DECLARE @name VARCHAR(100)
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
  SET @message = 'synch_triggers ERROR: ' + cast(@@ERROR AS VARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
 END CATCH
END

GO
GRANT CONTROL ON TYPE::[dbo].[synch_triggers_list] TO [users]
GO
GRANT EXECUTE ON [dbo].[synch_triggers] TO [users]
GO

alter table unique_constraints add [disabled] bit;alter table procedures add [function_type] nvarchar(100);alter table tables_relations add [update_rule] nvarchar(100);
GO
alter table tables_relations add [delete_rule] nvarchar(100);
GO
IF EXISTS (select * from sys.types where name = 'synch_columns_list')
BEGIN
 DROP PROCEDURE synch_columns;
 DROP TYPE synch_columns_list
END
GO
CREATE TYPE [dbo].[synch_columns_list] AS TABLE(
 [name] [nvarchar](250) NULL,
 [position] [int] NULL,
 [datatype] [varchar](100) NULL,
 [description] [ntext] NULL,
 [constraint_type] [varchar](1) NULL,
 [data_length] [varchar](20) NULL,
 [nullable] [bit] NULL,
 [default_def] [nvarchar](4000),
 [identity_def] [nvarchar](50)
)
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
 ,@message VARCHAR(500) OUTPUT AS

BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

 BEGIN TRANSACTION

 BEGIN TRY
  DECLARE @name VARCHAR(100)
   ,@position INT
   ,@datatype VARCHAR(100)
   ,@description NVARCHAR(MAX)
   ,@constraint_type VARCHAR(1)
   ,@data_length VARCHAR(20)
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

  SELECT @table_id = table_id
  FROM tables
  WHERE database_id = @database_id
   AND name = @table_name
   AND [schema] = @schema

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
  SET @message = 'synch_columns ERROR: ' + cast(@@ERROR AS VARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
 END CATCH
END
GO

IF EXISTS (select * from sys.types where name = 'synch_unique_constraints_list')
BEGIN
 DROP PROCEDURE synch_unique_constraints;
 DROP TYPE synch_unique_constraints_list
END
GO
CREATE TYPE [dbo].[synch_unique_constraints_list] AS TABLE(
 [name] [nvarchar](250) NULL,
 [primary_key] [bit] NULL,
 [description] [ntext] NULL,
 [disabled] [bit] NULL
)
GO


-- =============================================
-- Author:  Joanna Grzonkowska
-- Create date: 2013-10-21
-- Description: Synchronizing the list of unique constraints on the given table with the repository:
--    for each unique constraint updating its data or inserting it if it has not existed so far or setting its status to 'D' (deleted) if it
--    has been deleted from the database.
-- =============================================
CREATE PROCEDURE [dbo].[synch_unique_constraints] @synch_unique_constraints_list
AS
    synch_unique_constraints_list READONLY
 ,@table_name NVARCHAR(250)
 ,@schema NVARCHAR(250)
 ,@database_id INT
 ,@result INT OUTPUT
 ,@message VARCHAR(500) OUTPUT AS

BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

 BEGIN TRANSACTION

 BEGIN TRY
  DECLARE @name VARCHAR(100)
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
  SET @message = 'synch_unique_constraints ERROR: ' + cast(@@ERROR AS VARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
 END CATCH
END
GO
IF EXISTS (select * from sys.types where name = 'synch_relations_list')
BEGIN
 DROP PROCEDURE synch_relations;
 DROP TYPE synch_relations_list
END
GO
CREATE TYPE [dbo].[synch_relations_list] AS TABLE(
 [name] [nvarchar](250) NULL,
 [fk_table_name] [nvarchar](250) NULL,
 [pk_table_name] [nvarchar](250) NULL,
 [fk_table_schema] [nvarchar](250) NULL,
 [pk_table_schema] [nvarchar](250) NULL,
 [description] [ntext] NULL,
 [update_rule] [nvarchar](100) NULL,
 [delete_rule] [nvarchar](100) NULL
)
GO


-- =============================================
-- Author:  Adam Adamowicz, Joanna Grzonkowska
-- Create date: 2013-02-22
-- Description: Synchronizing the list of relations with the repository:
--    for each relation updating its data or inserting it if it has not existed so far or setting its status to 'D' (deleted) if it
--    has been deleted from the database.
-- =============================================
CREATE PROCEDURE [dbo].[synch_relations]
@synch_relations_list AS synch_relations_list READONLY
,@synch_tables_list AS objects_name_schema_list READONLY
 ,@database_id INT
 -- if relation's synchronization succeeded then adding id of its table to this variable
 -- if number of the occurences of table's id is equal to the count of its relations then it means that the relation's synchronization succeed for this table
 ,@succeed_tables_ids VARCHAR(MAX) OUTPUT
 ,@result INT OUTPUT
 ,@message VARCHAR(500) OUTPUT AS

BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

 BEGIN TRANSACTION

 BEGIN TRY
  DECLARE @name VARCHAR(100)
   ,@fk_table_name VARCHAR(100)
   ,@pk_table_name VARCHAR(100)
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

   SET @succeed_tables_ids += cast(@table_pk_id AS VARCHAR(MAX)) + ',' + cast(@table_fk_id AS VARCHAR(MAX)) + ',';

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
  SET @message = 'synch_relation ERROR: ' + cast(@@ERROR AS VARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
 END CATCH
END

GO
-- =============================================
-- Author:  Adam Adamowicz, Joanna Grzonkowska
-- Create date: 2013-02-22
-- Description: Synchronizing a procedure or a function with the Repository.
-- =============================================
ALTER PROCEDURE [dbo].[synch_procedure] @name VARCHAR(100)
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
 ,@message VARCHAR(500) OUTPUT
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
  SET @message = 'synch_procedure ERROR: ' + cast(@@ERROR AS VARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
 END CATCH
END

GO
-- =============================================
-- Author:  Adam Adamowicz, Joanna Grzonkowska
-- Create date: 2013-02-20
-- Description: Synchronizing information about a table or a view with the Repository.
-- =============================================
ALTER PROCEDURE [dbo].[synch_table] @name VARCHAR(100)
 ,@schema NVARCHAR(250)
 ,@database_id INT
 ,@object_type NVARCHAR(100)
 ,@description NTEXT
 ,@definition NTEXT
 ,@dbms_creation_date DATETIME
 ,@dbms_last_modification_date DATETIME
 ,@table_id INT OUTPUT
 ,@result INT OUTPUT
 ,@message VARCHAR(500) OUTPUT
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
  SET @message = 'synch_table ERROR: ' + cast(@@ERROR AS VARCHAR(MAX)) + ', ERROR_MESSAGE: ' + ERROR_MESSAGE()
 END CATCH
END

GO
GRANT CONTROL ON TYPE::[dbo].[synch_relations_list] TO [users]
GO
GRANT EXECUTE ON [dbo].[synch_relations] TO [users]

GO
GRANT CONTROL ON TYPE::[dbo].[synch_unique_constraints_list] TO [users]
GO
GRANT EXECUTE ON [dbo].[synch_unique_constraints] TO [users]

GO
GRANT CONTROL ON TYPE::[dbo].[synch_columns_list] TO [users]
GO
GRANT EXECUTE ON [dbo].[synch_columns] TO [users]
GO
update version set stable = 1 where version = 2
GO
