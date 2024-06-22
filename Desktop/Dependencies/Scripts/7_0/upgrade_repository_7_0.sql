IF(
    (SELECT COUNT(*)
    FROM [version]
    WHERE [version] = 7 AND [update] = 0 AND [release] = 0)
    = 0)
BEGIN
	INSERT INTO [version] ([version], [update], [release], [stable]) VALUES (7, 0, 0, 0)
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'source'
)
BEGIN
    ALTER TABLE [columns]
    ADD [source] NVARCHAR(50) CONSTRAINT [DF_columns_source] DEFAULT (N'DBMS') NOT NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'sort'
)
BEGIN
    ALTER TABLE [columns]
    ADD [sort] INT CONSTRAINT [DF_columns_sort] DEFAULT (99999) NOT NULL
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'ordinal_position'
)
BEGIN
    ALTER TABLE [columns]
    ALTER COLUMN [ordinal_position] INT NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'source'
)
BEGIN
    ALTER TABLE [tables]
    ADD [source] NVARCHAR(50) CONSTRAINT [DF_tables_source] DEFAULT (N'DBMS') NOT NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'subtype'
)
BEGIN
    ALTER TABLE [tables]
    ADD [subtype] [nvarchar](100) NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'subtype'
)
BEGIN
    ALTER TABLE [procedures]
    ADD [subtype] [nvarchar](100) NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'language'
)
BEGIN
    ALTER TABLE [procedures]
    ADD [language] [nvarchar](100) NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'type'
)
BEGIN
    ALTER TABLE [triggers]
    ADD [type] [nvarchar](100) CONSTRAINT [DF_triggers_type] DEFAULT (N'TRIGGER') NOT NULL
END
GO

UPDATE [version]
set [stable] = 1
where [version] = 7 and [update] = 0 AND [release] = 0
GO
