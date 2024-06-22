IF(
    (SELECT COUNT(*)
    FROM [version]
    WHERE [version] = 6 AND [update] = 0 AND [release] = 2)
    = 0)
BEGIN
	INSERT INTO [version] ([version], [update], [release], [stable]) VALUES (6, 0, 2, 0)
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [columns]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[custom_fields]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [custom_fields]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[databases]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [databases]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [dependencies]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[documentation_custom_fields]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [documentation_custom_fields]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[erd_links]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [erd_links]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[erd_nodes]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [erd_nodes]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[erd_nodes_columns]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [erd_nodes_columns]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[ignored_objects]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [ignored_objects]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [modules]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [parameters]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [procedures]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures_modules]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [procedures_modules]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [tables]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_modules]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [tables_modules]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [tables_relations]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations_columns]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [tables_relations_columns]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [triggers]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [unique_constraints]
    ADD [source_id] INT
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints_columns]')
	   AND name = 'source_id'
)
BEGIN
    ALTER TABLE [unique_constraints_columns]
    ADD [source_id] INT
END
GO

UPDATE [version]
set [stable] = 1
where [version] = 6 and [update] = 0 AND [release] = 2
GO
