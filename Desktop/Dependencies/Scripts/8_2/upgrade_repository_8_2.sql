-- Version
IF
  (
   SELECT COUNT(*)
     FROM [version]
     WHERE [version] = 8
           AND [update] = 2
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
         2,
         0,
         0
        );
    END;
GO

-- Datatypes - table
CREATE TABLE [datatypes](
     [datatype_id] [int] IDENTITY(1,1) NOT NULL,
     [datatype] [nvarchar](100) NOT NULL,
     [global_datatype] [nvarchar](100) NOT NULL,
     [creation_date] [datetime] NOT NULL,
     [created_by] [nvarchar](1024) NULL,
     [last_modification_date] [datetime] NOT NULL,
     [modified_by] [nvarchar](1024) NULL,
 CONSTRAINT [PK_datatypes] PRIMARY KEY CLUSTERED 
(
    [datatype_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_datatype] UNIQUE NONCLUSTERED 
(
    [datatype] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
) ON [PRIMARY]
GO

ALTER TABLE [datatypes] ADD  CONSTRAINT [DF_datatypes_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO

ALTER TABLE [datatypes] ADD  CONSTRAINT [DF_datatypes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO

ALTER TABLE [datatypes] ADD  CONSTRAINT [DF_datatypes_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO

ALTER TABLE [datatypes] ADD  CONSTRAINT [DF_datatypes_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO

SET IDENTITY_INSERT [datatypes] ON 
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (1, 'CHAR', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (2, 'CHARACTER SET', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (3, 'CHARSET', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (4, 'CLOB', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (5, 'ENUM', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (6, 'LONG', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (7, 'LONG RAW', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (8, 'LONGBLOB', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (9, 'LONGTEXT', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (10, 'MEDIUMBLOB', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (11, 'MEDIUMTEXT', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (12, 'NCHAR', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (13, 'NCLOB', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (14, 'NVARCHAR2', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (15, 'RAW', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (16, 'SET', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (17, 'TEXT', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (18, 'TINYTEXT', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (19, 'VARCHAR', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (20, 'VARCHAR2', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (21, 'CHARACTER', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (22, 'CHARACTER VARYING', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (23, 'CHAR VARYING', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (24, 'NATIONAL CHARACTER', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (25, 'NATIONAL CHAR', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (26, 'NATIONAL CHARACTER VARYING', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (27, 'NATIONAL CHAR VARYING', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (28, 'NCHAR VARYING', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (29, 'LONG VARCHAR', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (30, 'DBCLOB', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (31, 'STRING', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (32, 'ASCII', 'STRING', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (33, 'GEOMETRY', 'SPATIAL', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (34, 'GEOMETRYCOLLECTION', 'SPATIAL', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (35, 'LINESTRING', 'SPATIAL', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (36, 'MULTILINESTRING', 'SPATIAL', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (37, 'MULTIPOINT', 'SPATIAL', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (38, 'MULTIPOLYGON', 'SPATIAL', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (39, 'POINT', 'SPATIAL', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (40, 'POLYGON', 'SPATIAL', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (41, 'GEOGRAPHY', 'SPATIAL', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (42, 'BFILE', 'OTHER', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (43, 'JSON', 'OTHER', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (44, 'ROWID', 'OTHER', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (45, 'UROWID', 'OTHER', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (46, 'ID', 'OTHER', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (47, 'UUID', 'OTHER', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (48, 'INET', 'OTHER', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (49, 'cidr', 'OTHER', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (50, 'BIGINT', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (51, 'BIT', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (52, 'BOOL', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (53, 'BOOLEAN', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (54, 'DEC', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (55, 'DECIMAL', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (56, 'DOUBLE', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (57, 'DOUBLE PRECISION', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (58, 'FLOAT', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (59, 'INT', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (60, 'INTEGER', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (61, 'MEDIUMINT', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (62, 'NUMBER', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (63, 'NUMERIC', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (64, 'SMALLINT', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (65, 'TINYINT', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (66, 'REAL', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (67, 'DECFLOAT', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (68, 'SMALLMONEY', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (69, 'MONEY', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (70, 'COUNTER', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (71, 'VARINT', 'NUMERIC', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (72, 'DATE', 'DATE_TIME', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (73, 'DATETIME', 'DATE_TIME', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (74, 'INTERVAL DAY', 'DATE_TIME', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (75, 'INTERVAL YEAR', 'DATE_TIME', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (76, 'TIME', 'DATE_TIME', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (77, 'TIMESTAMP', 'DATE_TIME', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (78, 'TIMEUUID', 'DATE_TIME', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (79, 'YEAR', 'DATE_TIME', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (80, 'DATETIME2', 'DATE_TIME', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (81, 'SMALLDATETIME', 'DATE_TIME', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (82, 'DATETIMEOFFSET', 'DATE_TIME', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (83, 'BINARY', 'BINARY', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (84, 'BLOB', 'BINARY', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (85, 'TINYBLOB', 'BINARY', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (86, 'VARBINARY', 'BINARY', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (87, 'GRAPHIC', 'BINARY', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (88, 'VARGRAPHIC', 'BINARY', getdate(), suser_sname(), getdate(), suser_sname())
GO
INSERT [datatypes] ([datatype_id], [datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (89, 'IMAGE', 'BINARY', getdate(), suser_sname(), getdate(), suser_sname())
GO
SET IDENTITY_INSERT [datatypes] OFF
GO

GRANT DELETE ON [datatypes] TO [users] AS [dbo];
GO
GRANT INSERT ON [datatypes] TO [users] AS [dbo];
GO
GRANT SELECT ON [datatypes] TO [users] AS [dbo];
GO
GRANT UPDATE ON [datatypes] TO [users] AS [dbo];
GO

-- Term Relationship Types - columns

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[glossary_term_relationship_types]')
                      AND [name] = 'sort'
             )
    BEGIN
        ALTER TABLE [glossary_term_relationship_types]
        ADD [sort] [int] DEFAULT (99) NOT NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[glossary_term_relationship_types]')
                      AND [name] = 'sort_reverse'
             )
    BEGIN
        ALTER TABLE [glossary_term_relationship_types]
        ADD [sort_reverse] [int] DEFAULT (99) NOT NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[glossary_term_relationship_types]')
                      AND [name] = 'code'
             )
    BEGIN
        ALTER TABLE [glossary_term_relationship_types]
        ADD [code] [nvarchar](250) NULL;
    END;
GO

-- Term Relationship Types - values

UPDATE [glossary_term_relationship_types]
  SET
      [sort] = 18,
      [sort_reverse] = 18,
      [code] = 'RELATED'
  WHERE [title] = 'Related term';
GO

UPDATE [glossary_term_relationship_types]
  SET
      [sort] = 2,
      [sort_reverse] = 1,
      [code] = 'REPLACES'
  WHERE [title] = 'Replaces';
GO

UPDATE [glossary_term_relationship_types]
  SET
      [sort] = 14,
      [sort_reverse] = 15,
      [code] = 'CALCULATED'
  WHERE [title] = 'Is calculated from';
GO

UPDATE [glossary_term_relationship_types]
  SET
      [sort] = 5,
      [sort_reverse] = 5,
      [code] = 'SYNONYM'
  WHERE [title] = 'Is a synonym of';
GO

UPDATE [glossary_term_relationship_types]
  SET
      [sort] = 4,
      [sort_reverse] = 3,
      [code] = 'PREFERRED_SYNONYM'
  WHERE [title] = 'Has a preferred synonym';
GO

UPDATE [glossary_term_relationship_types]
  SET
      [sort] = 16,
      [sort_reverse] = 17,
      [code] = 'TYPE'
  WHERE [title] = 'Is a type of';
GO

UPDATE [glossary_term_relationship_types]
  SET
      [sort] = 10,
      [sort_reverse] = 11,
      [code] = 'CONTAINS'
  WHERE [title] = 'Contains';
GO

UPDATE [glossary_term_relationship_types]
  SET
      [sort] = 12,
      [sort_reverse] = 13,
      [code] = 'USED'
  WHERE [title] = 'Is used by';
GO

-- Databases - columns

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[databases]')
                      AND [name] = 'show_schema'
             )
    BEGIN
        ALTER TABLE [databases]
        ADD [show_schema] [bit] DEFAULT (1) NULL;
    END;
GO

UPDATE [databases]
  SET
      [show_schema] = [databases_show_schema].[show_schema]
  FROM
      (
       SELECT [database_ids].[database_id],
              (
               SELECT CASE
                          WHEN COUNT(*) > 1
                          THEN 1
                          ELSE 0
                      END
                 FROM
                     (
                      SELECT [schema]
                        FROM [tables]
                        WHERE [database_id] = [database_ids].[database_id]
                      UNION
                      SELECT [schema]
                        FROM [procedures]
                        WHERE [database_id] = [database_ids].[database_id]
                     ) [schemas]
              ) AS [show_schema]
         FROM
             (
              SELECT [database_id]
                FROM [databases]
             ) [database_ids]
      ) [databases_show_schema]
  WHERE [databases].[database_id] = [databases_show_schema].[database_id];

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[databases]')
                      AND [name] = 'show_schema_override'
             )
    BEGIN
        ALTER TABLE [databases]
        ADD [show_schema_override] [bit] DEFAULT (NULL) NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[databases]')
                      AND [name] = 'class'
             )
    BEGIN
        ALTER TABLE [databases]
        ADD [class] [nvarchar](50) DEFAULT('DATABASE') NULL;
    END;
GO

UPDATE [databases]
  SET
      [class] = 'DATABASE'
  WHERE [type] <> 'BUSINESS_GLOSSARY'
        OR [type] IS NULL;
GO

UPDATE [databases]
  SET
      [class] = 'GLOSSARY'
  WHERE [type] = 'BUSINESS_GLOSSARY';
GO

ALTER TABLE [databases] ALTER COLUMN [class] [nvarchar](50) NOT NULL;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[databases]')
                      AND [name] = 'multihost'
             )
    BEGIN
        ALTER TABLE [databases]
        ADD [multihost] [nvarchar](max) NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[databases]')
                      AND [name] = 'authentication_database'
             )
    BEGIN
        ALTER TABLE [databases]
        ADD [authentication_database] [nvarchar](250) NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[databases]')
                      AND [name] = 'replica_set'
             )
    BEGIN
        ALTER TABLE [databases]
        ADD [replica_set] [nvarchar](250) NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[databases]')
                      AND [name] = 'ssl_type'
             )
    BEGIN
        ALTER TABLE [databases]
        ADD [ssl_type] [nvarchar](50) NULL;
    END;
GO

-- User Connections

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[user_connections]')
                      AND [name] = 'multihost'
             )
    BEGIN
        ALTER TABLE [user_connections]
        ADD [multihost] [nvarchar](max) NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[user_connections]')
                      AND [name] = 'authentication_database'
             )
    BEGIN
        ALTER TABLE [user_connections]
        ADD [authentication_database] [nvarchar](250) NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[user_connections]')
                      AND [name] = 'replica_set'
             )
    BEGIN
        ALTER TABLE [user_connections]
        ADD [replica_set] [nvarchar](250) NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[user_connections]')
                      AND [name] = 'ssl_type'
             )
    BEGIN
        ALTER TABLE [user_connections]
        ADD [ssl_type] [nvarchar](50) NULL;
    END;
GO

-- Glossary Term Types

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[glossary_term_types]')
                      AND [name] = 'code'
             )
    BEGIN
        ALTER TABLE [glossary_term_types]
        ADD [code] [nvarchar](250) NULL;
    END;
GO

UPDATE [glossary_term_types]
  SET
      [code] = 'TERM'
  WHERE [title] = 'Term';
GO

UPDATE [glossary_term_types]
  SET
      [code] = 'POLICY'
  WHERE [title] = 'Policy';
GO

UPDATE [glossary_term_types]
  SET
      [code] = 'RULE'
  WHERE [title] = 'Rule';
GO

UPDATE [glossary_term_types]
  SET
      [code] = 'CATEGORY'
  WHERE [title] = 'Category';
GO

-- Tables - columns

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[tables]')
                      AND [name] = 'language'
             )
    BEGIN
        ALTER TABLE [tables]
        ADD [language]  [nvarchar](100) DEFAULT ('SQL') NOT NULL;
    END;
GO

-- Guid - columns

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[guid]')
                      AND [name] = 'is_web_portal_connected'
             )
    BEGIN
        ALTER TABLE [guid]
        ADD [is_web_portal_connected] [bit] DEFAULT (0);
    END;
GO

-- Guid - data

UPDATE [guid]
   SET [is_web_portal_connected] = 0
   WHERE [is_web_portal_connected] IS NULL
GO

-- Columns changes - columns

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[columns_changes]')
                      AND [name] = 'parent_id'
             )
    BEGIN
        ALTER TABLE [columns_changes]
        ADD [parent_id] [int] NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[columns_changes]')
                      AND [name] = 'path'
             )
    BEGIN
        ALTER TABLE [columns_changes]
        ADD [path] [nvarchar](max) NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[columns_changes]')
                      AND [name] = 'level'
             )
    BEGIN
        ALTER TABLE [columns_changes]
        ADD [level] [int] NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[columns_changes]')
                      AND [name] = 'item_type'
             )
    BEGIN
        ALTER TABLE [columns_changes]
        ADD [item_type] [nvarchar](100) NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[columns_changes]')
                      AND [name] = 'before_parent_id'
             )
    BEGIN
        ALTER TABLE [columns_changes]
        ADD [before_parent_id] [int] NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[columns_changes]')
                      AND [name] = 'before_path'
             )
    BEGIN
        ALTER TABLE [columns_changes]
        ADD [before_path] [nvarchar](max) NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[columns_changes]')
                      AND [name] = 'before_level'
             )
    BEGIN
        ALTER TABLE [columns_changes]
        ADD [before_level] [int] NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] = OBJECT_ID(N'[columns_changes]')
                      AND [name] = 'before_item_type'
             )
    BEGIN
        ALTER TABLE [columns_changes]
        ADD [before_item_type] [nvarchar](100) NULL;
    END;
GO

-- Columns - triggers

-- =============================================
-- Description:	Insert column's changes to schema change tracking tables
-- =============================================
ALTER TRIGGER [dbo].[trg_columns_change_track_update] ON [dbo].[columns]
FOR UPDATE
AS
     -- check if column was updated
     IF NOT (UPDATE([datatype]) OR UPDATE([data_length])  OR UPDATE([nullable]) OR UPDATE([default_value]) OR UPDATE([is_identity]) OR UPDATE([is_computed]) OR UPDATE([computed_formula])  OR UPDATE([name]) OR UPDATE([status]) 
     OR UPDATE([parent_id]) OR UPDATE([path]) OR UPDATE([level]) OR UPDATE([item_type]))
      BEGIN
          RETURN;
     END;
     -- skip manual objects created through Dataedo application
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [INSERTED]
                 WHERE [source] = 'USER'
              )
         BEGIN
             RETURN;
         END;


     -- check if object property change 
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[column_id] = [d].[column_id]
                 WHERE
                 --isnull(d.ordinal_position,0) <> i.ordinal_position OR
                 ISNULL([d].[datatype],'') <> ISNULL([i].[datatype],'')
                 OR ISNULL([d].[data_length],'') <> ISNULL([i].[data_length],'')
                 OR ISNULL([d].[nullable],'') <> ISNULL([i].[nullable],'')
                 OR ISNULL([d].[default_value],'') <> ISNULL([i].[default_value],'')
                 OR ISNULL([d].[is_identity],'') <> ISNULL([i].[is_identity],'')
                 OR ISNULL([d].[is_computed],'') <> ISNULL([i].[is_computed],'')
                 OR ISNULL([d].[computed_formula],'') <> ISNULL([i].[computed_formula],'')
                 OR ISNULL([d].[parent_id],0) <> ISNULL([i].[parent_id],0)
                 OR ISNULL([d].[path],'') <> ISNULL([i].[path],'')
                 OR ISNULL([d].[level],0) <> ISNULL([i].[level],0)
                 OR ISNULL([d].[item_type],'') <> ISNULL([i].[item_type],'')
                 OR [d].[name] <> [i].[name]
              )
         BEGIN
             UPDATE [columns_changes]
               SET
                   [valid_to] = GETDATE()
               FROM [columns_changes] [c]
                    INNER JOIN [inserted] [u] ON [c].[column_id] = [u].[column_id]
               WHERE [valid_to] IS NULL;


             -- insert changes (before and after values) for columns - when column was updated
             INSERT INTO [columns_changes]
             ([column_id],
              [table_id],
              [database_id],
              [table_schema],
              [table_name],
              [ordinal_position],
              [column_name],
              [path],
              [parent_id], 
              [level],
              [item_type],
              [datatype],
              [data_length],
              [nullable],
              [default_value],
              [is_identity],
              [is_computed],
              [computed_formula],
              [before_parent_id],
              [before_path],
              [before_level],
              [before_item_type],
              [before_ordinal_position],
              [before_column_name],
              [before_datatype],
              [before_data_length],
              [before_nullable],
              [before_default_value],
              [before_is_identity],
              [before_is_computed],
              [before_computed_formula],
              [operation],
              [valid_from],
              [update_id]
             )
                    SELECT [i].[column_id],
                           [i].[table_id],
                           [t].[database_id],
                           [t].[schema] AS [table_schema],
                           [t].[name] AS [table_name],
                           [i].[ordinal_position],
                           [i].[name] AS [column_name],
                           [i].[path] AS [path],
                           [i].[parent_id] AS [parent_id], 
                           [i].[level] AS [level],
                           [i].[item_type] AS [item_type],
                           [i].[datatype],
                           [i].[data_length],
                           [i].[nullable],
                           [i].[default_value],
                           [i].[is_identity],
                           [i].[is_computed],
                           [i].[computed_formula],
                           [d].[parent_id],
                           [d].[path],
                           [d].[level],
                           [d].[item_type],
                           [d].[ordinal_position],
                           [d].[name] AS [column_name],
                           [d].[datatype],
                           [d].[data_length],
                           [d].[nullable],
                           [d].[default_value],
                           [d].[is_identity],
                           [d].[is_computed],
                           [d].[computed_formula],
                           'UPDATED',
                           [cc].[valid_to],
                           [i].[update_id]
                      FROM [inserted] [i]
                           INNER JOIN [deleted] [d] ON [i].[column_id] = [d].[column_id]
                           OUTER APPLY
                                      (
                                       SELECT MAX([valid_to]) AS [valid_to]
                                         FROM [columns_changes] [c]
                                         WHERE [c].[column_id] = [d].[column_id]
                                               AND [c].[valid_to] IS NOT NULL
                                      ) [cc]
                           LEFT OUTER JOIN [tables] [t] ON [i].[table_id] = [t].[table_id]
                      WHERE [i].[status] = 'A'
                            AND [d].[status] = 'A'
                            AND (ISNULL([d].[ordinal_position],0) <> ISNULL([i].[ordinal_position],0)
                                 OR ISNULL([d].[datatype],'') <> ISNULL([i].[datatype],'')
                                 OR ISNULL([d].[data_length],'') <> ISNULL([i].[data_length],'')
                                 OR ISNULL([d].[nullable],'') <> ISNULL([i].[nullable],'')
                                 OR ISNULL([d].[default_value],'') <> ISNULL([i].[default_value],'')
                                 OR ISNULL([d].[is_identity],'') <> ISNULL([i].[is_identity],'')
                                 OR ISNULL([d].[is_computed],'') <> ISNULL([i].[is_computed],'')
                                 OR ISNULL([d].[computed_formula],'') <> ISNULL([i].[computed_formula],'')
                                 OR ISNULL([d].[parent_id],0) <> ISNULL([i].[parent_id],0)
                                 OR ISNULL([d].[path],'') <> ISNULL([i].[path],'')
                                 OR ISNULL([d].[level],0) <> ISNULL([i].[level],0)
                                 OR ISNULL([d].[item_type],'') <> ISNULL([i].[item_type],'')
                                 OR [d].[name] <> [i].[name]);
         END;
    BEGIN

        -- insert changes (deleted values) for columns - when column was deleted
        INSERT INTO [columns_changes]
        ([column_id],
         [table_id],
         [database_id],
         [table_schema],
         [table_name],
         [ordinal_position],
         [column_name],
         [path],
         [parent_id], 
         [level],
         [item_type],
         [datatype],
         [data_length],
         [nullable],
         [default_value],
         [is_identity],
         [is_computed],
         [computed_formula],
         [operation],
         [valid_to],
         [update_id]
        )
               SELECT [d].[column_id],
                      [d].[table_id],
                      [t].[database_id],
                      [t].[schema] AS [table_schema],
                      [t].[name] AS [table_name],
                      [d].[ordinal_position],
                      [d].[name] AS [column_name],
                      [d].[path] AS [path],
                      [d].[parent_id] AS [parent_id], 
                      [d].[level] AS [level],
                      [d].[item_type] AS [item_type],
                      [d].[datatype],
                      [d].[data_length],
                      [d].[nullable],
                      [d].[default_value],
                      [d].[is_identity],
                      [d].[is_computed],
                      [d].[computed_formula],
                      'DELETED',
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[column_id] = [d].[column_id]
                      LEFT OUTER JOIN [tables] [t] ON [i].[table_id] = [t].[table_id]
                 WHERE [i].[status] = 'D'
                       AND [d].[status] = 'A';

        -- insert changes (updated values) for columns - when column was restored
        INSERT INTO [columns_changes]
        ([column_id],
         [table_id],
         [database_id],
         [table_schema],
         [table_name],
         [ordinal_position],
         [column_name],
         [path],
         [parent_id], 
         [level],
         [item_type],
         [datatype],
         [data_length],
         [nullable],
         [default_value],
         [is_identity],
         [is_computed],
         [computed_formula],
         [operation],
         [valid_from],
         [update_id]
        )
               SELECT [i].[column_id],
                      [i].[table_id],
                      [t].[database_id],
                      [t].[schema] AS [table_schema],
                      [t].[name] AS [table_name],
                      [i].[ordinal_position],
                      [i].[name] AS [column_name],
                      [i].[path] AS [path],
                      [i].[parent_id] AS [parent_id], 
                      [i].[level] AS [level],
                      [i].[item_type] AS [item_type],
                      [i].[datatype],
                      [i].[data_length],
                      [i].[nullable],
                      [i].[default_value],
                      [i].[is_identity],
                      [i].[is_computed],
                      [i].[computed_formula],
                      'ADDED',
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[column_id] = [d].[column_id]
                      LEFT OUTER JOIN [tables] [t] ON [i].[table_id] = [t].[table_id]
                 WHERE [i].[status] = 'A'
                       AND [d].[status] = 'D';
    END;
GO

-- =============================================
-- Description:	Insert column's changes to schema change tracking tables
-- =============================================
ALTER TRIGGER [dbo].[trg_columns_change_track_insert] ON [dbo].[columns]
FOR INSERT
AS
     -- skip manual objects created through Dataedo application
     -- or first documentation import to Dataedo application
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [inserted]
                 WHERE [source] = 'USER'
              )
        OR EXISTS
                 (
                  SELECT TOP (1) 1
                    FROM [inserted] [i]
                         JOIN [schema_updates] [u] ON [u].[update_id] = [i].[update_id]
                                                            AND [u].[type] = 'IMPORT'
                 )
         BEGIN
             RETURN;
         END;
     INSERT INTO [columns_changes]
     ([column_id],
      [table_id],
      [database_id],
      [table_schema],
      [table_name],
      [ordinal_position],
      [column_name],
      [path],
      [parent_id], 
      [level],
      [item_type],
      [datatype],
      [data_length],
      [nullable],
      [default_value],
      [is_identity],
      [is_computed],
      [computed_formula],
      [operation],
      [valid_from],
      [update_id]
     )
            SELECT [i].[column_id],
                   [i].[table_id],
                   [t].[database_id],
                   [t].[schema] AS [table_schema],
                   [t].[name] AS [table_name],
                   [i].[ordinal_position],
                   [i].[name] AS [column_name],
                   [i].[path],
                   [i].[parent_id], 
                   [i].[level],
                   [i].[item_type],
                   [i].[datatype],
                   [i].[data_length],
                   [i].[nullable],
                   [i].[default_value],
                   [i].[is_identity],
                   [i].[is_computed],
                   [i].[computed_formula],
                   'ADDED',
                   GETDATE(),
                   [i].[update_id]
              FROM [inserted] [i]
                   LEFT OUTER JOIN [tables] [t] ON [i].[table_id] = [t].[table_id];
GO

-- Version
UPDATE [version]
  SET
      [stable] = 1
  WHERE [version] = 8
        AND [update] = 2
        AND [release] = 0;
GO