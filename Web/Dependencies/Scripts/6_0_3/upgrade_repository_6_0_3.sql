IF(
    (SELECT COUNT(*)
    FROM [version]
    WHERE [version] = 6 AND [update] = 0 AND [release] = 3)
    = 0)
BEGIN
	INSERT INTO [version] ([version], [update], [release], [stable]) VALUES (6, 0, 3, 0)
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'fk_type'
)
BEGIN
    ALTER TABLE [tables_relations]
    ADD [fk_type] NVARCHAR(15) CONSTRAINT [DF_tables_relations_fk_type] DEFAULT(N'MANY') NOT NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'pk_type'
)
BEGIN
    ALTER TABLE [tables_relations]
    ADD [pk_type] NVARCHAR(15) CONSTRAINT [DF_tables_relations_pk_type] DEFAULT(N'ONE') NOT NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[databases]')
	   AND name = 'dbms_version'
)
BEGIN
    ALTER TABLE [databases]
    ADD [dbms_version] NVARCHAR(500) NULL
END
GO

UPDATE [version]
set [stable] = 1
where [version] = 6 and [update] = 0 AND [release] = 3
GO
