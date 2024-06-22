-- Version
IF
  (
   SELECT COUNT(*)
     FROM [version]
     WHERE [version] = 8
           AND [update] = 1
           AND [release] = 0
  ) = 0
    BEGIN
        INSERT INTO [version]
        ([version],
         [update],
         [release],
         [stable]
        )
        VALUES
        (8,
         1,
         0,
         0
        );
    END;
GO

-- Columns
IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[columns]')
                      AND [name] = 'parent_id'
             )
    BEGIN
        ALTER TABLE [columns]
        ADD [parent_id] int NULL;

        ALTER TABLE [dbo].[columns]
        WITH CHECK
        ADD CONSTRAINT [FK_columns_column_parent_columns] FOREIGN KEY([parent_id]) REFERENCES [dbo].[columns]([column_id]);
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[columns]')
                      AND [name] = 'path'
             )
    BEGIN
        ALTER TABLE [columns]
        ADD [path] [nvarchar](MAX) NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[columns]')
                      AND [name] = 'level'
             )
    BEGIN
        ALTER TABLE [columns]
        ADD [level] [int] DEFAULT (1) NOT NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[columns]')
                      AND [name] = 'item_type'
             )
    BEGIN
        ALTER TABLE [columns]
        ADD [item_type] [nvarchar](100) DEFAULT('COLUMN') NOT NULL;
    END;
GO

-- Tables

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[tables]')
                      AND [name] = 'location'
             )
    BEGIN
        ALTER TABLE [tables]
        ADD [location] [nvarchar](MAX) NULL;
    END;
GO

-- Custom Field Classes

CREATE TABLE [custom_field_classes]
([custom_field_class_id]  [int] IDENTITY(1,1) NOT NULL,
 [code]                   nvarchar(100) NOT NULL,
 [name]                   nvarchar(100) NOT NULL,
 [sort]                   [int] NOT NULL,
 [creation_date]          datetime NOT NULL,
 [created_by]             nvarchar(1024) NULL,
 [last_modification_date] datetime NOT NULL,
 [modified_by]            nvarchar(1024) NULL,
 [source_id]              [int] NULL,
 CONSTRAINT [PK_custom_field_classes] PRIMARY KEY CLUSTERED([custom_field_class_id] ASC)
 WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY];
GO
ALTER TABLE [custom_field_classes]
ADD CONSTRAINT [DF_custom_field_classes_creation_date] DEFAULT GETDATE() FOR [creation_date];
GO
ALTER TABLE [custom_field_classes]
ADD CONSTRAINT [DF_custom_field_classes_created_by] DEFAULT SUSER_SNAME() FOR [created_by];
GO
ALTER TABLE [custom_field_classes]
ADD CONSTRAINT [DF_custom_field_classes_last_modification_date] DEFAULT GETDATE() FOR [last_modification_date];
GO
ALTER TABLE [custom_field_classes]
ADD CONSTRAINT [DF_custom_field_classes_modified_by] DEFAULT SUSER_SNAME() FOR [modified_by];
GO

GRANT DELETE ON [custom_field_classes] TO [users] AS [dbo];
GO
GRANT INSERT ON [custom_field_classes] TO [users] AS [dbo];
GO
GRANT SELECT ON [custom_field_classes] TO [users] AS [dbo];
GO
GRANT UPDATE ON [custom_field_classes] TO [users] AS [dbo];
GO

SET IDENTITY_INSERT [custom_field_classes] ON 
GO
INSERT [custom_field_classes] ([custom_field_class_id], [code], [name], [sort], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (1, N'DESCRIPTION', N'Description', 1, getdate(), suser_sname(), getdate(), suser_sname(), NULL)
GO
INSERT [custom_field_classes] ([custom_field_class_id], [code], [name], [sort], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (2, N'LABEL', N'Label', 2, getdate(), suser_sname(), getdate(), suser_sname(), NULL)
GO
INSERT [custom_field_classes] ([custom_field_class_id], [code], [name], [sort], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (3, N'STATUS', N'Status', 3, getdate(), suser_sname(), getdate(), suser_sname(), NULL)
GO
INSERT [custom_field_classes] ([custom_field_class_id], [code], [name], [sort], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (4, N'ROLE', N'Role', 4, getdate(), suser_sname(), getdate(), suser_sname(), NULL)
GO
INSERT [custom_field_classes] ([custom_field_class_id], [code], [name], [sort], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (5, N'CLASSIFICATION', N'Classification', 5, getdate(), suser_sname(), getdate(), suser_sname(), NULL)
GO
INSERT [custom_field_classes] ([custom_field_class_id], [code], [name], [sort], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (6, N'DOMAIN', N'Domain', 6, getdate(), suser_sname(), getdate(), suser_sname(), NULL)
GO
INSERT [custom_field_classes] ([custom_field_class_id], [code], [name], [sort], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (7, N'FORMAT', N'Format', 7, getdate(), suser_sname(), getdate(), suser_sname(), NULL)
GO
INSERT [custom_field_classes] ([custom_field_class_id], [code], [name], [sort], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (8, N'MAPPING', N'Mapping', 8, getdate(), suser_sname(), getdate(), suser_sname(), NULL)
GO
INSERT [custom_field_classes] ([custom_field_class_id], [code], [name], [sort], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (9, N'OTHER', N'Other', 9, getdate(), suser_sname(), getdate(), suser_sname(), NULL)
GO
SET IDENTITY_INSERT [custom_field_classes] OFF
GO

-- Custom Fields

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[custom_fields]')
        AND name = 'custom_field_class_id'
)
BEGIN
    ALTER TABLE [custom_fields]
    ADD [custom_field_class_id] [int] NULL
END
GO

-- Custom Fields - Default fields

UPDATE [custom_fields]
  SET
      [custom_field_class_id] =
                                (
                                 SELECT [custom_field_class_id]
                                   FROM [custom_field_classes]
                                   WHERE [custom_field_classes].[code] = 'FORMAT'
                                )
  WHERE [custom_field_class_id] IS NULL
        AND [custom_fields].[code] IN ('lookup_values', 'format', 'sample_values');
GO

UPDATE [custom_fields]
  SET
      [custom_field_class_id] =
                                (
                                 SELECT [custom_field_class_id]
                                   FROM [custom_field_classes]
                                   WHERE [custom_field_classes].[code] = 'CLASSIFICATION'
                                )
  WHERE [custom_field_class_id] IS NULL
        AND [custom_fields].[code] IN ('sensitive_data');
GO

UPDATE [custom_fields]
  SET
      [custom_field_class_id] =
                                (
                                 SELECT [custom_field_class_id]
                                   FROM [custom_field_classes]
                                   WHERE [custom_field_classes].[code] = 'STATUS'
                                )
  WHERE [custom_field_class_id] IS NULL
        AND [custom_fields].[code] IN ('status');
GO

UPDATE [custom_fields]
  SET
      [custom_field_class_id] =
                                (
                                 SELECT [custom_field_class_id]
                                   FROM [custom_field_classes]
                                   WHERE [custom_field_classes].[code] = 'ROLE'
                                )
  WHERE [custom_field_class_id] IS NULL
        AND [custom_fields].[code] IN ('owner', 'data_steward', 'data_custodian', 'author', 'expert');
GO

UPDATE [custom_fields]
  SET
      [custom_field_class_id] =
                                (
                                 SELECT [custom_field_class_id]
                                   FROM [custom_field_classes]
                                   WHERE [custom_field_classes].[code] = 'MAPPING'
                                )
  WHERE [custom_field_class_id] IS NULL
        AND [custom_fields].[code] IN ('source_system', 'source_table', 'source_column', 'transformation', 'form', 'form_field', 'class', 'class_property');
GO

UPDATE [custom_fields]
  SET
      [custom_field_class_id] =
                                (
                                 SELECT [custom_field_class_id]
                                   FROM [custom_field_classes]
                                   WHERE [custom_field_classes].[code] = 'LABEL'
                                )
  WHERE [custom_field_class_id] IS NULL
        AND [custom_fields].[code] IN ('label', 'also_known_as');
GO

UPDATE [custom_fields]
  SET
      [custom_field_class_id] =
                                (
                                 SELECT [custom_field_class_id]
                                   FROM [custom_field_classes]
                                   WHERE [custom_field_classes].[code] = 'OTHER'
                                )
  WHERE [custom_field_class_id] IS NULL
        AND [custom_fields].[code] IN ('encrypted');
GO

-- Custom Fields - Classificators

UPDATE [custom_fields]
  SET
      [custom_field_class_id] =
                                (
                                 SELECT [custom_field_class_id]
                                   FROM [custom_field_classes]
                                   WHERE [custom_field_classes].[code] = 'CLASSIFICATION'
                                )
  WHERE [custom_fields].[custom_field_id] IN
                                            (
                                             SELECT [custom_field_1_id]
                                               FROM [classificators]
                                               WHERE [custom_field_1_name] = 'PII'
                                                     OR [custom_field_1_name] = 'GDPR Classification'
                                            );
GO

UPDATE [custom_fields]
  SET
      [custom_field_class_id] =
                                (
                                 SELECT [custom_field_class_id]
                                   FROM [custom_field_classes]
                                   WHERE [custom_field_classes].[code] = 'DOMAIN'
                                )
  WHERE [custom_fields].[custom_field_id] IN
                                            (
                                             SELECT [custom_field_2_id]
                                               FROM [classificators]
                                               WHERE [custom_field_2_name] = 'PII Data Domain'
                                                     OR [custom_field_2_name] = 'GDPR Data Domain'
                                            );
GO

-- Custom Fields - Other

UPDATE [custom_fields]
  SET
      [custom_field_class_id] =
                                (
                                 SELECT [custom_field_class_id]
                                   FROM [custom_field_classes]
                                   WHERE [custom_field_classes].[code] = 'STATUS'
                                )
  WHERE [custom_field_class_id] IS NULL
        AND [custom_fields].[title] LIKE '%status%';
GO

UPDATE [custom_fields]
  SET
      [custom_field_class_id] =
                                (
                                 SELECT [custom_field_class_id]
                                   FROM [custom_field_classes]
                                   WHERE [custom_field_classes].[code] = 'ROLE'
                                )
  WHERE [custom_field_class_id] IS NULL
        AND [custom_fields].[title] LIKE '%owner%';
GO

UPDATE [custom_fields]
  SET
      [custom_field_class_id] =
                                (
                                 SELECT [custom_field_class_id]
                                   FROM [custom_field_classes]
                                   WHERE [custom_field_classes].[code] = 'MAPPING'
                                )
  WHERE [custom_field_class_id] IS NULL
        AND ([custom_fields].[title] LIKE '%source%'
             OR [custom_fields].[title] LIKE '%destination%');
GO

UPDATE [custom_fields]
  SET
      [custom_field_class_id] =
                                (
                                 SELECT [custom_field_class_id]
                                   FROM [custom_field_classes]
                                   WHERE [custom_field_classes].[code] = 'LABEL'
                                )
  WHERE [custom_field_class_id] IS NULL
        AND [custom_fields].[title] LIKE '%label%';
GO

UPDATE [custom_fields]
  SET
      [custom_field_class_id] =
                                (
                                 SELECT [custom_field_class_id]
                                   FROM [custom_field_classes]
                                   WHERE [custom_field_classes].[code] = 'CLASSIFICATION'
                                )
  WHERE [custom_field_class_id] IS NULL
        AND ([custom_fields].[title] LIKE '%classification%'
             OR [custom_fields].[title] LIKE '%sensitiv%'
             OR [custom_fields].[title] LIKE '%personal%');
GO

UPDATE [custom_fields]
  SET
      [custom_field_class_id] =
                                (
                                 SELECT [custom_field_class_id]
                                   FROM [custom_field_classes]
                                   WHERE [custom_field_classes].[code] = 'OTHER'
                                )
  WHERE [custom_field_class_id] IS NULL
        AND [custom_fields].[custom_field_class_id] IS NULL;
GO

-- Update class values of custom fields

ALTER TABLE [custom_fields] ALTER COLUMN [custom_field_class_id] [int] NOT NULL;
GO

ALTER TABLE [custom_fields]  WITH CHECK ADD  CONSTRAINT [FK_custom_fields_custom_field_classes] FOREIGN KEY([custom_field_class_id])
REFERENCES [custom_field_classes] ([custom_field_class_id])
GO

ALTER TABLE [custom_fields] CHECK CONSTRAINT [FK_custom_fields_custom_field_classes]
GO

-- Classificators

-- Classificators - columns

ALTER TABLE [classificators] ADD [custom_field_1_class_id] [int] NULL;
GO

ALTER TABLE [classificators] ADD [custom_field_2_class_id] [int] NULL;
GO

ALTER TABLE [classificators] ADD [custom_field_3_class_id] [int] NULL;
GO

ALTER TABLE [classificators] ADD [custom_field_4_class_id] [int] NULL;
GO

ALTER TABLE [classificators] ADD [custom_field_5_class_id] [int] NULL;
GO

-- Classificators - references

ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_classes_1] FOREIGN KEY([custom_field_1_class_id])
REFERENCES [custom_field_classes] ([custom_field_class_id])
GO

ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_classes_1]
GO

ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_classes_2] FOREIGN KEY([custom_field_2_class_id])
REFERENCES [custom_field_classes] ([custom_field_class_id])
GO

ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_classes_2]
GO

ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_classes_3] FOREIGN KEY([custom_field_3_class_id])
REFERENCES [custom_field_classes] ([custom_field_class_id])
GO

ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_classes_3]
GO

ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_classes_4] FOREIGN KEY([custom_field_4_class_id])
REFERENCES [custom_field_classes] ([custom_field_class_id])
GO

ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_classes_4]
GO

ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_classes_5] FOREIGN KEY([custom_field_5_class_id])
REFERENCES [custom_field_classes] ([custom_field_class_id])
GO

ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_classes_5]
GO

-- Classificators - update

UPDATE [classificators]
  SET
      [custom_field_1_class_id] =
                                  (
                                   SELECT [custom_field_class_id]
                                     FROM [custom_field_classes]
                                     WHERE [custom_field_classes].[code] = 'CLASSIFICATION'
                                  ),
      [custom_field_2_class_id] =
                                  (
                                   SELECT [custom_field_class_id]
                                     FROM [custom_field_classes]
                                     WHERE [custom_field_classes].[code] = 'DOMAIN'
                                  );
GO

-- Version
UPDATE [version]
  SET
      [stable] = 1
  WHERE [version] = 8
        AND [update] = 1
        AND [release] = 0;
GO