IF(
    (SELECT COUNT(*)
    FROM [version]
    WHERE [version] = 5 AND [update] = 2)
    = 0)
BEGIN
INSERT INTO [version] ([version], [update], [stable]) VALUES (5, 2, 0)
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[databases]')
	   AND name = 'filter'
)
BEGIN
    ALTER TABLE [databases] ADD [filter] [nvarchar](max) NULL;
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'title'
)
BEGIN
    ALTER TABLE [tables_relations] ADD [title] nvarchar(250) NULL;
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[erd_links]')
	   AND name = 'show_join_condition'
)
BEGIN
    ALTER TABLE [erd_links] ADD [show_join_condition] bit DEFAULT 0 NOT NULL;
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'default_value'
)
BEGIN
    ALTER TABLE [columns] ADD [default_value] nvarchar(MAX) NULL;
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'is_identity'
)
BEGIN
    ALTER TABLE [columns] ADD [is_identity] bit NOT NULL CONSTRAINT [DF_columns_is_identity] DEFAULT 0;
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'is_computed'
)
BEGIN
    ALTER TABLE [columns] ADD [is_computed] bit NOT NULL CONSTRAINT [DF_columns_is_computed] DEFAULT 0;
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'computed_formula'
)
BEGIN
    ALTER TABLE [columns] ADD [computed_formula] nvarchar(MAX) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[databases]')
	   AND name = 'name'
)
BEGIN
    ALTER TABLE [databases]
    ALTER COLUMN [name] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[databases]')
	   AND name = 'host'
)
BEGIN
    ALTER TABLE [databases]
    ALTER COLUMN [host] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[databases]')
	   AND name = 'user'
)
BEGIN
    ALTER TABLE [databases]
    ALTER COLUMN [user] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[databases]')
	   AND name = 'password'
)
BEGIN
    ALTER TABLE [databases]
    ALTER COLUMN [password] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[licenses]')
	   AND name = 'login'
)
BEGIN
    ALTER TABLE licenses
    ALTER COLUMN [login] [nvarchar](1024) NOT NULL;
END
GO

--columns
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE columns
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE columns
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--databases
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[databases]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE databases
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[databases]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE databases
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--dependencies
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE dependencies
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE dependencies
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--dependencies_descriptions
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE dependencies_descriptions
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE dependencies_descriptions
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--erd_links
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[erd_links]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE erd_links
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[erd_links]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE erd_links
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--erd_nodes
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[erd_nodes]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE erd_nodes
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[erd_nodes]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE erd_nodes
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--ignored_objects
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[ignored_objects]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE ignored_objects
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[ignored_objects]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE ignored_objects
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--licenses
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[licenses]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE licenses
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[licenses]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE licenses
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--modules
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE modules
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE modules
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--parameters
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE parameters
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE parameters
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--procedures
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE procedures
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE procedures
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--procedures_modules
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures_modules]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE procedures_modules
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures_modules]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE procedures_modules
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--tables
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE tables
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE tables
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--tables_modules
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_modules]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE tables_modules
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_modules]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE tables_modules
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--tables_relations
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE tables_relations
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE tables_relations
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--tables_relations_columns
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations_columns]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE tables_relations_columns
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations_columns]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE tables_relations_columns
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--triggers
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE triggers
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE triggers
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--unique_constraints
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE unique_constraints
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE unique_constraints
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--unique_constraints_columns
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints_columns]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE unique_constraints_columns
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints_columns]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE unique_constraints_columns
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

--version
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[version]')
	   AND name = 'created_by'
)
BEGIN
    ALTER TABLE version
    ALTER COLUMN [created_by] [nvarchar](1024) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[version]')
	   AND name = 'modified_by'
)
BEGIN
    ALTER TABLE version
    ALTER COLUMN [modified_by] [nvarchar](1024) NULL;
END
GO

UPDATE [erd_links]
SET [show_join_condition] = [show_label];
GO

UPDATE [erd_links]
SET [show_label] = 0;
GO

UPDATE [version]
set [stable] = 1
where [version] = 5 and [update] = 2
GO