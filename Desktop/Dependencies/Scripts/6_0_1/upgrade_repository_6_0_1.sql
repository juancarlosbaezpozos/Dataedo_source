IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[version]')
	   AND name = 'release'
)
BEGIN
    ALTER TABLE [version]
    ADD [release] int NULL
END
GO

IF (SELECT TOP 1 1
       FROM sys.objects
       WHERE type_desc LIKE '%CONSTRAINT'
           AND OBJECT_NAME(OBJECT_ID) = 'DF__version__release'
       ) is null
BEGIN
   ALTER TABLE [version] ADD CONSTRAINT [DF__version__release] DEFAULT(0)
   FOR [release];
END;

IF((SELECT COUNT(*)
    FROM sys.indexes
    WHERE name = 'UK_version' AND object_id = OBJECT_ID('version')) > 0)
BEGIN
DROP INDEX [UK_version] ON [version];
END
GO

CREATE UNIQUE NONCLUSTERED INDEX [UK_version] ON [version]
(
	[version] ASC,
	[update] ASC,
	[release] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


IF (SELECT TOP 1 1
       FROM sys.objects
       WHERE type_desc LIKE '%CONSTRAINT'
           AND OBJECT_NAME(OBJECT_ID) = 'UK_custom_fields_code'
       ) is null
BEGIN
	ALTER TABLE [dbo].[custom_fields] ADD CONSTRAINT [UK_custom_fields_code] UNIQUE NONCLUSTERED 
		([code] ASC)
		WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END;
GO

IF (SELECT TOP 1 1
       FROM sys.objects
       WHERE type_desc LIKE '%CONSTRAINT'
           AND OBJECT_NAME(OBJECT_ID) = 'UK_custom_fields_field_name'
       ) is null
BEGIN
	ALTER TABLE [dbo].[custom_fields] ADD CONSTRAINT [UK_custom_fields_field_name] UNIQUE NONCLUSTERED 
		([field_name] ASC)
		WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END;
GO

IF(
    (SELECT COUNT(*)
    FROM [version]
    WHERE [version] = 6 AND [update] = 0 AND [release] = 1)
    = 0)
BEGIN
	INSERT INTO [version] ([version], [update], [release], [stable]) VALUES (6, 0, 1, 0)
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[databases]')
	   AND name = 'description_plain'
)
BEGIN
    ALTER TABLE [databases]
    ADD [description_plain] nvarchar(max) NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description_plain'
)
BEGIN
    ALTER TABLE [modules]
    ADD [description_plain] nvarchar(max) NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description_plain'
)
BEGIN
    ALTER TABLE [procedures]
    ADD [description_plain] nvarchar(max) NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description_plain'
)
BEGIN
    ALTER TABLE [tables]
    ADD [description_plain] nvarchar(max) NULL
END
GO

UPDATE [version]
set [stable] = 1
where [version] = 6 and [update] = 0 AND [release] = 1
GO
