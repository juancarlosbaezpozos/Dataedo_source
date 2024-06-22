-- Version
IF
  (
   SELECT COUNT(*)
     FROM [version]
     WHERE [version] = 7
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
        (7,
         2,
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
                WHERE [object_id] =OBJECT_ID(N'[columns]')
                      AND [name] = 'update_id'
             )
    BEGIN
        ALTER TABLE [columns]
        ADD [update_id] int NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] =OBJECT_ID(N'[parameters]')
                      AND [name] = 'update_id'
             )
    BEGIN
        ALTER TABLE [parameters]
        ADD [update_id] int NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] =OBJECT_ID(N'[procedures]')
                      AND [name] = 'update_id'
             )
    BEGIN
        ALTER TABLE [procedures]
        ADD [update_id] int NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] =OBJECT_ID(N'[tables]')
                      AND [name] = 'update_id'
             )
    BEGIN
        ALTER TABLE [tables]
        ADD [update_id] int NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] =OBJECT_ID(N'[tables_relations]')
                      AND [name] = 'update_id'
             )
    BEGIN
        ALTER TABLE [tables_relations]
        ADD [update_id] int NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] =OBJECT_ID(N'[tables_relations_columns]')
                      AND [name] = 'update_id'
             )
    BEGIN
        ALTER TABLE [tables_relations_columns]
        ADD [update_id] int NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] =OBJECT_ID(N'[triggers]')
                      AND [name] = 'update_id'
             )
    BEGIN
        ALTER TABLE [triggers]
        ADD [update_id] int NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] =OBJECT_ID(N'[unique_constraints]')
                      AND [name] = 'update_id'
             )
    BEGIN
        ALTER TABLE [unique_constraints]
        ADD [update_id] int NULL;
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[columns]
                WHERE [object_id] =OBJECT_ID(N'[unique_constraints_columns]')
                      AND [name] = 'update_id'
             )
    BEGIN
        ALTER TABLE [unique_constraints_columns]
        ADD [update_id] int NULL;
    END;
GO

-- Setting subtypes
UPDATE [procedures]
  SET
      [subtype] = [object_type]
  WHERE [subtype] IS NULL;
GO

UPDATE [tables]
  SET
      [subtype] = [object_type]
  WHERE [subtype] IS NULL;
GO

-- Indexes
IF EXISTS
         (
          SELECT *
            FROM [sys].[indexes]
            WHERE [name] = 'ix_columns_table_id'
                  AND object_id = OBJECT_ID(N'[columns]')
         )
    BEGIN
        DROP INDEX [ix_columns_table_id] ON [columns];
    END;
GO

IF NOT EXISTS
             (
              SELECT *
                FROM [sys].[indexes]
                WHERE [name] = 'ix_columns_desc'
                      AND object_id = OBJECT_ID(N'[columns]')
             )
    BEGIN
        CREATE INDEX [ix_columns_desc] ON [columns]([table_id],[name],[title]) INCLUDE([description],[field1],[field2],[field3],[field4],[field5],[field6],[field7],[field8],[field9],[field10],[field11],[field12],[field13],[field14],[field15],[field16],[field17],[field18],[field19],[field20],[field21],[field22],[field23],[field24],[field25],[field26],[field27],[field28],[field29],[field30],[field31],[field32],[field33],[field34],[field35],[field36],[field37],[field38],[field39],[field40]);
    END;
GO

-- Foreign keys
ALTER TABLE [custom_fields_values]
ADD CONSTRAINT [FK_custom_fields_values_custom_fields] FOREIGN KEY([custom_field_id]) REFERENCES [custom_fields]([custom_field_id]) ON UPDATE NO ACTION ON DELETE CASCADE;
GO
ALTER TABLE [custom_fields_values] SET(LOCK_ESCALATION = TABLE);
GO

-- Schema Change Tracking
CREATE TABLE [columns_changes]
([id]                      int IDENTITY(1,1) NOT NULL,
 [update_id]               int NULL,
 [column_id]               int NOT NULL,
 [table_id]                int NOT NULL,
 [database_id]             int NULL,
 [table_schema]            nvarchar(250) NULL,
 [table_name]              nvarchar(250) NULL,
 [column_name]             nvarchar(250) NOT NULL,
 [ordinal_position]        int NULL,
 [datatype]                nvarchar(250) NULL,
 [data_length]             nvarchar(50) NULL,
 [nullable]                bit NOT NULL,
 [default_value]           nvarchar(max) NULL,
 [is_identity]             bit NOT NULL,
 [is_computed]             bit NOT NULL,
 [computed_formula]        nvarchar(max) NULL,
 [before_column_name]      nvarchar(250) NULL,
 [before_ordinal_position] int NULL,
 [before_datatype]         nvarchar(250) NULL,
 [before_data_length]      nvarchar(50) NULL,
 [before_nullable]         bit NULL,
 [before_default_value]    nvarchar(max) NULL,
 [before_is_identity]      bit NULL,
 [before_is_computed]      bit NULL,
 [before_computed_formula] nvarchar(max) NULL,
 [operation]               nvarchar(50) NOT NULL,
 [valid_from]              datetime NULL,
 [valid_to]                datetime NULL,
 [creation_date]           datetime NOT NULL,
 [created_by]              nvarchar(1024) NULL,
 [description]             nvarchar(max) NULL,
 [description_date]        datetime NULL,
 [description_by]          nvarchar(1024) NULL,
 CONSTRAINT [PK_columns_changes] PRIMARY KEY CLUSTERED([id] ASC)
 WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO
GRANT DELETE ON [columns_changes] TO [users] AS [dbo];
GO
GRANT INSERT ON [columns_changes] TO [users] AS [dbo];
GO
GRANT SELECT ON [columns_changes] TO [users] AS [dbo];
GO
GRANT UPDATE ON [columns_changes] TO [users] AS [dbo];
GO

CREATE TABLE [parameters_changes]
([id]                      int IDENTITY(1,1) NOT NULL,
 [update_id]               int NULL,
 [database_id]             int NOT NULL,
 [parameter_id]            int NOT NULL,
 [procedure_id]            int NOT NULL,
 [ordinal_position]        int NOT NULL,
 [parameter_mode]          nvarchar(10) NOT NULL,
 [name]                    nvarchar(250) NOT NULL,
 [datatype]                nvarchar(250) NULL,
 [data_length]             nvarchar(20) NULL,
 [before_ordinal_position] int NULL,
 [before_parameter_mode]   nvarchar(10) NULL,
 [before_datatype]         nvarchar(250) NULL,
 [before_data_length]      nvarchar(20) NULL,
 [operation]               nvarchar(50) NOT NULL,
 [valid_from]              datetime NULL,
 [valid_to]                datetime NULL,
 [creation_date]           datetime NOT NULL,
 [created_by]              nvarchar(1024) NULL,
 [description]             nvarchar(max) NULL,
 [description_date]        datetime NULL,
 [description_by]          nvarchar(1024) NULL,
 CONSTRAINT [PK_parameters_changes] PRIMARY KEY CLUSTERED([id] ASC)
 WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO
GRANT DELETE ON [parameters_changes] TO [users] AS [dbo];
GO
GRANT INSERT ON [parameters_changes] TO [users] AS [dbo];
GO
GRANT SELECT ON [parameters_changes] TO [users] AS [dbo];
GO
GRANT UPDATE ON [parameters_changes] TO [users] AS [dbo];
GO

CREATE TABLE [procedures_changes]
([id]                          int IDENTITY(1,1) NOT NULL,
 [update_id]                   int NULL,
 [procedure_id]                int NOT NULL,
 [database_id]                 int NOT NULL,
 [schema]                      nvarchar(250) NULL,
 [name]                        nvarchar(250) NOT NULL,
 [object_type]                 nvarchar(100) NOT NULL,
 [subtype]                     nvarchar(100) NULL,
 [function_type]               nvarchar(100) NULL,
 [definition]                  nvarchar(max) NULL,
 [before_object_type]          nvarchar(100) NULL,
 [before_definition]           nvarchar(max) NULL,
 [before_function_type]        nvarchar(100) NULL,
 [before_subtype]              nvarchar(100) NULL,
 [operation]                   nvarchar(50) NOT NULL,
 [dbms_last_modification_date] datetime NULL,
 [valid_from]                  datetime NULL,
 [valid_to]                    datetime NULL,
 [creation_date]               datetime NOT NULL,
 [created_by]                  nvarchar(1024) NULL,
 [description]                 nvarchar(max) NULL,
 [description_date]            datetime NULL,
 [description_by]              nvarchar(1024) NULL,
 CONSTRAINT [PK_procedures_changes] PRIMARY KEY CLUSTERED([id] ASC)
 WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO
GRANT DELETE ON [procedures_changes] TO [users] AS [dbo];
GO
GRANT INSERT ON [procedures_changes] TO [users] AS [dbo];
GO
GRANT SELECT ON [procedures_changes] TO [users] AS [dbo];
GO
GRANT UPDATE ON [procedures_changes] TO [users] AS [dbo];
GO

CREATE TABLE [schema_updates]
([update_id]                int IDENTITY(1,1) NOT NULL,
 [type]                     nvarchar(100) NOT NULL,
 [datetime]                 datetime NULL,
 [repository_login]         nvarchar(1024) NULL,
 [database_id]              int NULL,
 [connection_database_type] nvarchar(100) NULL,
 [connection_host]          nvarchar(1024) NULL,
 [connection_user]          nvarchar(1024) NULL,
 [connection_port]          int NULL,
 [connection_service_name]  nvarchar(100) NULL,
 [connection_database_name] nvarchar(1024) NULL,
 [connection_dbms_version]  nvarchar(500) NULL,
 [creation_date]            datetime NOT NULL,
 [created_by]               nvarchar(1024) NULL,
 [last_modification_date]   datetime NOT NULL,
 [modified_by]              nvarchar(1024) NULL,
 CONSTRAINT [PK_schema_updates] PRIMARY KEY CLUSTERED([update_id] ASC)
 WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY];
GO
GRANT DELETE ON [schema_updates] TO [users] AS [dbo];
GO
GRANT INSERT ON [schema_updates] TO [users] AS [dbo];
GO
GRANT SELECT ON [schema_updates] TO [users] AS [dbo];
GO
GRANT UPDATE ON [schema_updates] TO [users] AS [dbo];
GO

CREATE TABLE [tables_changes]
([id]                          int IDENTITY(1,1) NOT NULL,
 [update_id]                   int NULL,
 [table_id]                    int NOT NULL,
 [database_id]                 int NOT NULL,
 [schema]                      nvarchar(250) NULL,
 [name]                        nvarchar(250) NOT NULL,
 [object_type]                 nvarchar(100) NOT NULL,
 [subtype]                     nvarchar(100) NULL,
 [definition]                  nvarchar(max) NULL,
 [before_schema]               nvarchar(250) NULL,
 [before_name]                 nvarchar(250) NULL,
 [before_object_type]          nvarchar(100) NULL,
 [before_subtype]              nvarchar(100) NULL,
 [before_definition]           nvarchar(max) NULL,
 [operation]                   nvarchar(50) NOT NULL,
 [dbms_last_modification_date] datetime NULL,
 [valid_from]                  datetime NULL,
 [valid_to]                    datetime NULL,
 [creation_date]               datetime NOT NULL,
 [created_by]                  nvarchar(1024) NULL,
 [description]                 nvarchar(max) NULL,
 [description_date]            datetime NULL,
 [description_by]              nvarchar(1024) NULL,
 CONSTRAINT [PK_tables_changes] PRIMARY KEY CLUSTERED([id] ASC)
 WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO
GRANT DELETE ON [tables_changes] TO [users] AS [dbo];
GO
GRANT INSERT ON [tables_changes] TO [users] AS [dbo];
GO
GRANT SELECT ON [tables_changes] TO [users] AS [dbo];
GO
GRANT UPDATE ON [tables_changes] TO [users] AS [dbo];
GO

CREATE TABLE [tables_relations_changes]
([id]                     int IDENTITY(1,1) NOT NULL,
 [update_id]              int NULL,
 [table_relation_id]      int NOT NULL,
 [database_id]            int NOT NULL,
 [pk_table_id]            int NOT NULL,
 [fk_table_id]            int NOT NULL,
 [pk_table_schema]        nvarchar(250) NULL,
 [pk_table_name]          nvarchar(250) NULL,
 [fk_table_schema]        nvarchar(250) NULL,
 [fk_table_name]          nvarchar(250) NULL,
 [name]                   nvarchar(250) NOT NULL,
 [update_rule]            nvarchar(100) NULL,
 [delete_rule]            nvarchar(100) NULL,
 [disabled]               bit NULL,
 [fk_type]                nvarchar(15) NOT NULL,
 [pk_type]                nvarchar(15) NOT NULL,
 [before_pk_table_schema] nvarchar(250) NULL,
 [before_fk_table_schema] nvarchar(250) NULL,
 [before_pk_table_name]   nvarchar(250) NULL,
 [before_fk_table_name]   nvarchar(250) NULL,
 [before_name]            nvarchar(250) NULL,
 [before_update_rule]     nvarchar(100) NULL,
 [before_delete_rule]     nvarchar(100) NULL,
 [before_disabled]        bit NULL,
 [before_fk_type]         nvarchar(15) NULL,
 [before_pk_type]         nvarchar(15) NULL,
 [operation]              nvarchar(50) NOT NULL,
 [valid_from]             datetime NULL,
 [valid_to]               datetime NULL,
 [creation_date]          datetime NOT NULL,
 [created_by]             nvarchar(1024) NULL,
 [description]            nvarchar(max) NULL,
 [description_date]       datetime NULL,
 [description_by]         nvarchar(1024) NULL,
 CONSTRAINT [PK_tables_relations_changes] PRIMARY KEY CLUSTERED([id] ASC)
 WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO
GRANT DELETE ON [tables_relations_changes] TO [users] AS [dbo];
GO
GRANT INSERT ON [tables_relations_changes] TO [users] AS [dbo];
GO
GRANT SELECT ON [tables_relations_changes] TO [users] AS [dbo];
GO
GRANT UPDATE ON [tables_relations_changes] TO [users] AS [dbo];
GO

CREATE TABLE [tables_relations_columns_changes]
([id]                       int IDENTITY(1,1) NOT NULL,
 [update_id]                int NULL,
 [table_relation_column_id] int NOT NULL,
 [table_relation_id]        int NOT NULL,
 [column_fk_id]             int NOT NULL,
 [column_pk_id]             int NOT NULL,
 [column_fk_name]           nvarchar(250) NULL,
 [column_pk_name]           nvarchar(250) NULL,
 [ordinal_position]         int NOT NULL,
 [before_column_fk_id]      int NULL,
 [before_column_pk_id]      int NULL,
 [before_column_fk_name]    nvarchar(250) NULL,
 [before_column_pk_name]    nvarchar(250) NULL,
 [before_ordinal_position]  int NULL,
 [operation]                nvarchar(50) NOT NULL,
 [valid_from]               datetime NULL,
 [valid_to]                 datetime NULL,
 [creation_date]            datetime NOT NULL,
 [created_by]               nvarchar(1024) NULL,
 CONSTRAINT [PK_tables_relations_columns_changes] PRIMARY KEY CLUSTERED([id] ASC)
 WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY];
GO
GRANT DELETE ON [tables_relations_columns_changes] TO [users] AS [dbo];
GO
GRANT INSERT ON [tables_relations_columns_changes] TO [users] AS [dbo];
GO
GRANT SELECT ON [tables_relations_columns_changes] TO [users] AS [dbo];
GO
GRANT UPDATE ON [tables_relations_columns_changes] TO [users] AS [dbo];
GO

CREATE TABLE [triggers_changes]
([id]                int IDENTITY(1,1) NOT NULL,
 [update_id]         int NULL,
 [trigger_id]        int NOT NULL,
 [database_id]       int NOT NULL,
 [table_id]          int NOT NULL,
 [name]              nvarchar(250) NOT NULL,
 [before]            bit NOT NULL,
 [after]             bit NOT NULL,
 [instead_of]        bit NOT NULL,
 [on_insert]         bit NOT NULL,
 [on_update]         bit NOT NULL,
 [on_delete]         bit NOT NULL,
 [disabled]          bit NOT NULL,
 [definition]        nvarchar(max) NULL,
 [before_before]     bit NULL,
 [before_after]      bit NULL,
 [before_instead_of] bit NULL,
 [before_on_insert]  bit NULL,
 [before_on_update]  bit NULL,
 [before_on_delete]  bit NULL,
 [before_disabled]   bit NULL,
 [before_definition] nvarchar(max) NULL,
 [type]              nvarchar(100) NOT NULL,
 [operation]         nvarchar(50) NOT NULL,
 [valid_from]        datetime NULL,
 [valid_to]          datetime NULL,
 [creation_date]     datetime NOT NULL,
 [created_by]        nvarchar(1024) NULL,
 [description]       nvarchar(max) NULL,
 [description_date]  datetime NULL,
 [description_by]    nvarchar(1024) NULL,
 CONSTRAINT [PK_triggers_changes2] PRIMARY KEY CLUSTERED([id] ASC)
 WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO
GRANT DELETE ON [triggers_changes] TO [users] AS [dbo];
GO
GRANT INSERT ON [triggers_changes] TO [users] AS [dbo];
GO
GRANT SELECT ON [triggers_changes] TO [users] AS [dbo];
GO
GRANT UPDATE ON [triggers_changes] TO [users] AS [dbo];
GO

CREATE TABLE [unique_constraints_changes]
([id]                   int IDENTITY(1,1) NOT NULL,
 [update_id]            int NULL,
 [unique_constraint_id] int NOT NULL,
 [database_id]          int NOT NULL,
 [table_id]             int NOT NULL,
 [name]                 nvarchar(250) NOT NULL,
 [primary_key]          bit NOT NULL,
 [disabled]             bit NULL,
 [before_name]          nvarchar(250) NULL,
 [before_primary_key]   bit NULL,
 [before_disabled]      bit NULL,
 [operation]            nvarchar(50) NOT NULL,
 [valid_from]           datetime NULL,
 [valid_to]             datetime NULL,
 [creation_date]        datetime NOT NULL,
 [created_by]           nvarchar(1024) NULL,
 [description]          nvarchar(max) NULL,
 [description_date]     datetime NULL,
 [description_by]       nvarchar(1024) NULL,
 CONSTRAINT [PK_unique_constraints_changes] PRIMARY KEY CLUSTERED([id] ASC)
 WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO
GRANT DELETE ON [unique_constraints_changes] TO [users] AS [dbo];
GO
GRANT INSERT ON [unique_constraints_changes] TO [users] AS [dbo];
GO
GRANT SELECT ON [unique_constraints_changes] TO [users] AS [dbo];
GO
GRANT UPDATE ON [unique_constraints_changes] TO [users] AS [dbo];
GO

CREATE TABLE [unique_constraints_columns_changes]
([id]                          int IDENTITY(1,1) NOT NULL,
 [update_id]                   int NULL,
 [unique_constraint_column_id] int NOT NULL,
 [unique_constraint_id]        int NOT NULL,
 [column_id]                   int NOT NULL,
 [column_name]                 nvarchar(250) NULL,
 [before_column_name]          nvarchar(250) NULL,
 [ordinal_position]            int NOT NULL,
 [before_column_id]            int NULL,
 [before_ordinal_position]     int NULL,
 [operation]                   nvarchar(50) NOT NULL,
 [valid_from]                  datetime NULL,
 [valid_to]                    datetime NULL,
 [creation_date]               datetime NOT NULL,
 [created_by]                  nvarchar(1024) NULL,
 CONSTRAINT [PK_unique_constraints_columns_changes] PRIMARY KEY CLUSTERED([id] ASC)
 WITH(PAD_INDEX = OFF,STATISTICS_NORECOMPUTE = OFF,IGNORE_DUP_KEY = OFF,ALLOW_ROW_LOCKS = ON,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY];
GO
GRANT DELETE ON [unique_constraints_columns_changes] TO [users] AS [dbo];
GO
GRANT INSERT ON [unique_constraints_columns_changes] TO [users] AS [dbo];
GO
GRANT SELECT ON [unique_constraints_columns_changes] TO [users] AS [dbo];
GO
GRANT UPDATE ON [unique_constraints_columns_changes] TO [users] AS [dbo];
GO

ALTER TABLE [columns_changes]
ADD CONSTRAINT [DF_columns_changes_creation_date] DEFAULT GETDATE() FOR [creation_date];
GO
ALTER TABLE [columns_changes]
ADD CONSTRAINT [DF_columns_changes_created_by] DEFAULT SUSER_SNAME() FOR [created_by];
GO
ALTER TABLE [parameters_changes]
ADD CONSTRAINT [DF_parameters_changes_creation_date] DEFAULT GETDATE() FOR [creation_date];
GO
ALTER TABLE [parameters_changes]
ADD CONSTRAINT [DF_parameters_changes_created_by] DEFAULT SUSER_SNAME() FOR [created_by];
GO
ALTER TABLE [procedures_changes]
ADD CONSTRAINT [DF_procedures_changes_date] DEFAULT GETDATE() FOR [creation_date];
GO
ALTER TABLE [procedures_changes]
ADD CONSTRAINT [DF_procedures_changes_created_by] DEFAULT SUSER_SNAME() FOR [created_by];
GO
ALTER TABLE [schema_updates]
ADD CONSTRAINT [DF_schema_updates_datetime] DEFAULT GETDATE() FOR datetime;
GO
ALTER TABLE [schema_updates]
ADD CONSTRAINT [DF_schema_updates_repository_login] DEFAULT SUSER_SNAME() FOR [repository_login];
GO
ALTER TABLE [schema_updates]
ADD CONSTRAINT [DF_schema_updates_creation_date] DEFAULT GETDATE() FOR [creation_date];
GO
ALTER TABLE [schema_updates]
ADD CONSTRAINT [DF_schema_updates_created_by] DEFAULT SUSER_SNAME() FOR [created_by];
GO
ALTER TABLE [schema_updates]
ADD CONSTRAINT [DF_schema_updates_last_modification_date] DEFAULT GETDATE() FOR [last_modification_date];
GO
ALTER TABLE [schema_updates]
ADD CONSTRAINT [DF_schema_updates_modified_by] DEFAULT SUSER_SNAME() FOR [modified_by];
GO
ALTER TABLE [tables_changes]
ADD CONSTRAINT [DF_tables_changes_creation_date] DEFAULT GETDATE() FOR [creation_date];
GO
ALTER TABLE [tables_changes]
ADD CONSTRAINT [DF_tables_changes_created_by] DEFAULT SUSER_SNAME() FOR [created_by];
GO
ALTER TABLE [tables_relations_changes]
ADD CONSTRAINT [DF_tables_relations_changes_creation_date] DEFAULT GETDATE() FOR [creation_date];
GO
ALTER TABLE [tables_relations_changes]
ADD CONSTRAINT [DF_tables_relations_changes_created_by] DEFAULT SUSER_SNAME() FOR [created_by];
GO
ALTER TABLE [tables_relations_columns_changes]
ADD CONSTRAINT [DF_tables_relations_columns_changes_creation_date] DEFAULT GETDATE() FOR [creation_date];
GO
ALTER TABLE [tables_relations_columns_changes]
ADD CONSTRAINT [DF_tables_relations_columns_changes_created_by] DEFAULT SUSER_SNAME() FOR [created_by];
GO
ALTER TABLE [triggers_changes]
ADD CONSTRAINT [DF_triggers_changes_creation_date] DEFAULT GETDATE() FOR [creation_date];
GO
ALTER TABLE [triggers_changes]
ADD CONSTRAINT [DF_triggers_changes_created_by] DEFAULT SUSER_SNAME() FOR [created_by];
GO
ALTER TABLE [unique_constraints_changes]
ADD CONSTRAINT [DF_unique_constraints_changes_creation_date] DEFAULT GETDATE() FOR [creation_date];
GO
ALTER TABLE [unique_constraints_changes]
ADD CONSTRAINT [DF_unique_constraints_changes_created_by] DEFAULT SUSER_SNAME() FOR [created_by];
GO
ALTER TABLE [unique_constraints_columns_changes]
ADD CONSTRAINT [DF_unique_constraints_columns_changes_creation_date] DEFAULT GETDATE() FOR [creation_date];
GO
ALTER TABLE [unique_constraints_columns_changes]
ADD CONSTRAINT [DF_unique_constraints_columns_changes_created_by] DEFAULT SUSER_SNAME() FOR [created_by];
GO

-- Schema Change Tracking - Triggers

-- Schema Change Tracking - Triggers - columns
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert column's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_columns_change_track_insert] ON [columns]
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

ALTER TABLE [columns] DISABLE TRIGGER [trg_columns_change_track_insert];

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert column's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_columns_change_track_update] ON [columns]
FOR UPDATE
AS

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
                      LEFT OUTER JOIN [tables] [t] ON [i].[table_id] = [t].[table_id]
                 WHERE
                 --isnull(d.ordinal_position,0) <> i.ordinal_position OR
                 ISNULL([d].[datatype],'') <> ISNULL([i].[datatype],'')
                 OR ISNULL([d].[data_length],'') <> ISNULL([i].[data_length],'')
                 OR ISNULL([d].[nullable],'') <> ISNULL([i].[nullable],'')
                 OR ISNULL([d].[default_value],'') <> ISNULL([i].[default_value],'')
                 OR ISNULL([d].[is_identity],'') <> ISNULL([i].[is_identity],'')
                 OR ISNULL([d].[is_computed],'') <> ISNULL([i].[is_computed],'')
                 OR ISNULL([d].[computed_formula],'') <> ISNULL([i].[computed_formula],'')
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
              [datatype],
              [data_length],
              [nullable],
              [default_value],
              [is_identity],
              [is_computed],
              [computed_formula],
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
                           [i].[datatype],
                           [i].[data_length],
                           [i].[nullable],
                           [i].[default_value],
                           [i].[is_identity],
                           [i].[is_computed],
                           [i].[computed_formula],
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

ALTER TABLE [columns] DISABLE TRIGGER [trg_columns_change_track_update];

-- Schema Change Tracking - Triggers - parameters
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert parameter's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_parameters_change_track_insert] ON [parameters]
FOR INSERT
AS
     INSERT INTO [parameters_changes]
     ([database_id],
      [parameter_id],
      [procedure_id],
      [ordinal_position],
      [parameter_mode],
      [name],
      [datatype],
      [data_length],
      [operation],
      [valid_from],
      [update_id]
     )
            SELECT [p].[database_id],
                   [i].[parameter_id],
                   [i].[procedure_id],
                   [i].[ordinal_position],
                   [i].[parameter_mode],
                   [i].[name],
                   [i].[datatype],
                   [i].[data_length],
                   'ADDED',
                   GETDATE(),
                   [i].[update_id]
              FROM [inserted] [i]
                   JOIN [procedures] [p] ON [i].[procedure_id] = [p].[procedure_id]
              WHERE NOT EXISTS
                              (
                               SELECT 1
                                 FROM [schema_updates] [u]
                                 WHERE [u].[update_id] = [i].[update_id]
                                       AND [u].[type] = 'IMPORT'
                              );
GO

ALTER TABLE [parameters] DISABLE TRIGGER [trg_parameters_change_track_insert];

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert parameter's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_parameters_change_track_update] ON [parameters]
FOR UPDATE
AS

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
                      INNER JOIN [deleted] [d] ON [i].[parameter_id] = [d].[parameter_id]
                 WHERE
                 --isnull(d.[name],'') <> i.[name] or
                 ISNULL([d].[ordinal_position],'') <> ISNULL([i].[ordinal_position],'')
                 OR ISNULL([d].[parameter_mode],'') <> ISNULL([i].[parameter_mode],'')
                 OR ISNULL([d].[datatype],'') <> ISNULL([i].[datatype],'')
                 OR ISNULL([d].[data_length],'') <> ISNULL([i].[data_length],'')
              )

         BEGIN
             UPDATE [parameters_changes]
               SET
                   [valid_to] = GETDATE()
               FROM [parameters_changes] [c]
                    INNER JOIN [inserted] [u] ON [c].[procedure_id] = [u].[procedure_id]
               WHERE [valid_to] IS NULL;

             -- insert changes (before and after values) for parameter - when parameter was updated
             INSERT INTO [parameters_changes]
             ([database_id],
              [parameter_id],
              [procedure_id],
              [ordinal_position],
              [parameter_mode],
              [name],
              [datatype],
              [data_length],
              [before_ordinal_position],
              [before_parameter_mode],
              [before_datatype],
              [before_data_length],
              [operation],
              [valid_from],
              [update_id]
             )
                    SELECT [p].[database_id],
                           [i].[parameter_id],
                           [i].[procedure_id],
                           [i].[ordinal_position],
                           [i].[parameter_mode],
                           [i].[name],
                           [i].[datatype],
                           [i].[data_length],
                           [d].[ordinal_position],
                           [d].[parameter_mode],
                           [d].[datatype],
                           [d].[data_length],
                           'UPDATED',
                           [cc].[valid_to],
                           [i].[update_id]
                      FROM [inserted] [i]
                           INNER JOIN [deleted] [d] ON [i].[parameter_id] = [d].[parameter_id]
                           JOIN [procedures] [p] ON [d].[procedure_id] = [p].[procedure_id]
                           OUTER APPLY
                                      (
                                       SELECT MAX([valid_to]) AS [valid_to]
                                         FROM [procedures_changes] [c]
                                         WHERE [c].[procedure_id] = [d].[procedure_id]
                                               AND [c].[valid_to] IS NOT NULL
                                      ) [cc]
                      WHERE [i].[status] = 'A'
                            AND [d].[status] = 'A'
                            AND (ISNULL([d].[ordinal_position],'') <> ISNULL([i].[ordinal_position],'')
                                 OR ISNULL([d].[parameter_mode],'') <> ISNULL([i].[parameter_mode],'')
                                 OR ISNULL([d].[datatype],'') <> ISNULL([i].[datatype],'')
                                 OR ISNULL([d].[data_length],'') <> ISNULL([i].[data_length],''));

         END;

    BEGIN
        -- insert changes (deleted values) for parameter - when parameter was deleted
        INSERT INTO [parameters_changes]
        ([database_id],
         [parameter_id],
         [procedure_id],
         [ordinal_position],
         [parameter_mode],
         [name],
         [datatype],
         [data_length],
         [operation],
         [valid_to],
         [update_id]
        )
               SELECT [p].[database_id],
                      [d].[parameter_id],
                      [d].[procedure_id],
                      [d].[ordinal_position],
                      [d].[parameter_mode],
                      [d].[name],
                      [d].[datatype],
                      [d].[data_length],
                      'DELETED',
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[parameter_id] = [d].[parameter_id]
                      JOIN [procedures] [p] ON [d].[procedure_id] = [p].[procedure_id]
                 WHERE [i].[status] = 'D'
                       AND [d].[status] = 'A';

        -- insert changes (updated values) for parameter - when parameter was restored
        INSERT INTO [parameters_changes]
        ([database_id],
         [parameter_id],
         [procedure_id],
         [ordinal_position],
         [parameter_mode],
         [name],
         [datatype],
         [data_length],
         [operation],
         [valid_from],
         [update_id]
        )
               SELECT [p].[database_id],
                      [i].[parameter_id],
                      [i].[procedure_id],
                      [i].[ordinal_position],
                      [i].[parameter_mode],
                      [i].[name],
                      [i].[datatype],
                      [i].[data_length],
                      'ADDED',
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[parameter_id] = [d].[parameter_id]
                      JOIN [procedures] [p] ON [d].[procedure_id] = [p].[procedure_id]
                 WHERE [i].[status] = 'A'
                       AND [d].[status] = 'D';

    END;

GO

ALTER TABLE [parameters] DISABLE TRIGGER [trg_parameters_change_track_update];

-- Schema Change Tracking - Triggers - procedures
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert procedure's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_procedures_change_track_insert] ON [procedures]
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

     INSERT INTO [procedures_changes]
     ([database_id],
      [procedure_id],
      [schema],
      [name],
      [object_type],
      [definition],
      [function_type],
      [subtype],
      [operation],
      [dbms_last_modification_date],
      [valid_from],
      [update_id]
     )
            SELECT [i].[database_id],
                   [i].[procedure_id],
                   [i].[schema],
                   [i].[name],
                   [i].[object_type],
                   [i].[definition],
                   [i].[function_type],
                   [i].[subtype],
                   'ADDED',
                   GETDATE(),
                   [i].[dbms_last_modification_date],
                   [i].[update_id]
              FROM [inserted] [i];
GO

ALTER TABLE [procedures] DISABLE TRIGGER [trg_procedures_change_track_insert];

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert procedure's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_procedures_change_track_update] ON [procedures]
FOR UPDATE
AS

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
                      INNER JOIN [deleted] [d] ON [i].[procedure_id] = [d].[procedure_id]
                 WHERE ISNULL([d].[schema],'') <> ISNULL([i].[schema],'')
                       OR ISNULL([d].[name],'') <> ISNULL([i].[name],'')
                       OR ISNULL([d].[object_type],'') <> ISNULL([i].[object_type],'')
                       OR ISNULL([d].[definition],'') <> ISNULL([i].[definition],'')
                       OR ISNULL([d].[function_type],'') <> ISNULL([i].[function_type],'')
                       OR ISNULL([d].[subtype],'') <> ISNULL([i].[subtype],'')
                       OR ISNULL([d].[dbms_last_modification_date],'') <> ISNULL([i].[dbms_last_modification_date],'')
              )

         BEGIN

             UPDATE [procedures_changes]
               SET
                   [valid_to] = GETDATE()
               FROM [procedures_changes] [c]
                    INNER JOIN [inserted] [u] ON [c].[procedure_id] = [u].[procedure_id]
               WHERE [valid_to] IS NULL;

             -- insert changes (before and after values) for procedure - when procedure was updated
             INSERT INTO [procedures_changes]
             ([database_id],
              [procedure_id],
              [schema],
              [name],
              [object_type],
              [definition],
              [function_type],
              [subtype],
              [before_object_type],
              [before_definition],
              [before_function_type],
              [before_subtype],
              [operation],
              [dbms_last_modification_date],
              [valid_from],
              [update_id]
             )
                    SELECT [i].[database_id],
                           [i].[procedure_id],
                           [i].[schema],
                           [i].[name],
                           [i].[object_type],
                           [i].[definition],
                           [i].[function_type],
                           [i].[subtype],
                           [d].[object_type],
                           [d].[definition],
                           [d].[function_type],
                           [d].[subtype],
                           'UPDATED',
                           [i].[dbms_last_modification_date],
                           [cc].[valid_to],
                           [i].[update_id]
                      FROM [inserted] [i]
                           INNER JOIN [deleted] [d] ON [i].[procedure_id] = [d].[procedure_id]
                           OUTER APPLY
                                      (
                                       SELECT MAX([valid_to]) AS [valid_to]
                                         FROM [procedures_changes] [c]
                                         WHERE [c].[procedure_id] = [d].[procedure_id]
                                               AND [c].[valid_to] IS NOT NULL
                                      ) [cc]
                      WHERE [i].[status] = 'A'
                            AND [d].[status] = 'A'
                            AND (ISNULL([d].[schema],'') <> ISNULL([i].[schema],'')
                                 OR ISNULL([d].[name],'') <> ISNULL([i].[name],'')
                                 OR ISNULL([d].[object_type],'') <> ISNULL([i].[object_type],'')
                                 OR ISNULL([d].[definition],'') <> ISNULL([i].[definition],'')
                                 OR ISNULL([d].[function_type],'') <> ISNULL([i].[function_type],'')
                                 OR ISNULL([d].[subtype],'') <> ISNULL([i].[subtype],''));

         END;

    BEGIN
        -- insert changes (deleted values) for procedure - when procedure was deleted
        INSERT INTO [procedures_changes]
        ([database_id],
         [procedure_id],
         [schema],
         [name],
         [object_type],
         [definition],
         [function_type],
         [subtype],
         [operation],
         [dbms_last_modification_date],
         [valid_to],
         [update_id]
        )
               SELECT [d].[database_id],
                      [d].[procedure_id],
                      [d].[schema],
                      [d].[name],
                      [d].[object_type],
                      [d].[definition],
                      [d].[function_type],
                      [d].[subtype],
                      'DELETED',
                      [i].[dbms_last_modification_date],
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[procedure_id] = [d].[procedure_id]
                 WHERE [i].[status] = 'D'
                       AND [d].[status] = 'A';

        --insert changes (updated values) for procedure - when procedure was restored
        INSERT INTO [procedures_changes]
        ([database_id],
         [procedure_id],
         [schema],
         [name],
         [object_type],
         [definition],
         [function_type],
         [subtype],
         [operation],
         [dbms_last_modification_date],
         [valid_from],
         [update_id]
        )
               SELECT [i].[database_id],
                      [i].[procedure_id],
                      [i].[schema],
                      [i].[name],
                      [i].[object_type],
                      [i].[definition],
                      [i].[function_type],
                      [i].[subtype],
                      'ADDED',
                      [i].[dbms_last_modification_date],
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[procedure_id] = [d].[procedure_id]
                 WHERE [i].[status] = 'A'
                       AND [d].[status] = 'D';

    END;

GO

ALTER TABLE [procedures] DISABLE TRIGGER [trg_procedures_change_track_update];

-- Schema Change Tracking - Triggers - tables
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert table's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_tables_change_track_insert] ON [tables]
FOR INSERT
AS

     -- skip manual objects created through Dataedo application
     -- or first documentation import to Dataedo application
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [INSERTED]
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

     INSERT INTO [tables_changes]
     ([table_id],
      [database_id],
      [schema],
      [name],
      [object_type],
      [subtype],
      [definition],
      [dbms_last_modification_date],
      [operation],
      [valid_from],
      [update_id]
     )
            SELECT [i].[table_id],
                   [i].[database_id],
                   [i].[schema],
                   [i].[name],
                   [i].[object_type],
                   [i].[subtype],
                   [i].[definition],
                   [i].[dbms_last_modification_date],
                   'ADDED',
                   GETDATE(),
                   [i].[update_id]
              FROM [inserted] [i];

GO

ALTER TABLE [tables] DISABLE TRIGGER [trg_tables_change_track_insert];

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert table's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_tables_change_track_update] ON [tables]
FOR UPDATE
AS

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
                      INNER JOIN [deleted] [d] ON [i].[table_id] = [d].[table_id]
                 WHERE ISNULL([d].[schema],'') <> ISNULL([i].[schema],'')
                       OR ISNULL([d].[name],'') <> ISNULL([i].[name],'')
                       OR ISNULL([d].[object_type],'') <> ISNULL([i].[object_type],'')
                       OR ISNULL([d].[subtype],'') <> ISNULL([i].[subtype],'')
                       OR ISNULL([d].[definition],'') <> ISNULL([i].[definition],'')
                       OR ISNULL([d].[dbms_last_modification_date],'') <> ISNULL([i].[dbms_last_modification_date],'')
              )

         BEGIN

             UPDATE [tables_changes]
               SET
                   [valid_to] = GETDATE()
               FROM [tables_changes] [c]
                    INNER JOIN [inserted] [u] ON [c].[table_id] = [u].[table_id]
               WHERE [valid_to] IS NULL;

             -- insert changes (before and after values) for table - when table was updated
             INSERT INTO [tables_changes]
             ([table_id],
              [database_id],
              [schema],
              [name],
              [object_type],
              [subtype],
              [definition],
              [before_schema],
              [before_name],
              [before_object_type],
              [before_subtype],
              [before_definition],
              [dbms_last_modification_date],
              [operation],
              [valid_from],
              [update_id]
             )
                    SELECT [i].[table_id],
                           [i].[database_id],
                           [i].[schema],
                           [i].[name],
                           [i].[object_type],
                           [i].[subtype],
                           [i].[definition],
                           [d].[schema],
                           [d].[name],
                           [d].[object_type],
                           [d].[subtype],
                           [d].[definition],
                           [i].[dbms_last_modification_date],
                           'UPDATED',
                           [cc].[valid_to],
                           [i].[update_id]
                      FROM [inserted] [i]
                           INNER JOIN [deleted] [d] ON [i].[table_id] = [d].[table_id]
                           OUTER APPLY
                                      (
                                       SELECT MAX([valid_to]) AS [valid_to]
                                         FROM [tables_changes] [c]
                                         WHERE [c].[table_id] = [d].[table_id]
                                               AND [c].[valid_to] IS NOT NULL
                                      ) [cc]
                      WHERE [i].[status] = 'A'
                            AND [d].[status] = 'A'
                            -- and isnull(d.last_modification_date,'') <> i.last_modification_date
                            AND (ISNULL([d].[schema],'') <> ISNULL([i].[schema],'')
                                 OR ISNULL([d].[name],'') <> ISNULL([i].[name],'')
                                 OR ISNULL([d].[object_type],'') <> ISNULL([i].[object_type],'')
                                 OR ISNULL([d].[subtype],'') <> ISNULL([i].[subtype],'')
                                 OR ISNULL([d].[definition],'') <> ISNULL([i].[definition],'')
                                 OR ISNULL([d].[dbms_last_modification_date],'') <> ISNULL([i].[dbms_last_modification_date],''));

         END;

    BEGIN

        -- insert changes (deleted values) for table - when table was deleted
        INSERT INTO [tables_changes]
        ([table_id],
         [database_id],
         [schema],
         [name],
         [object_type],
         [subtype],
         [definition],
         [dbms_last_modification_date],
         [operation],
         [valid_to],
         [update_id]
        )
               SELECT [d].[table_id],
                      [d].[database_id],
                      [d].[schema],
                      [d].[name],
                      [d].[object_type],
                      [d].[subtype],
                      [d].[definition],
                      [i].[dbms_last_modification_date],
                      'DELETED',
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[table_id] = [d].[table_id]
                 WHERE [i].[status] = 'D'
                       AND [d].[status] = 'A';

        -- insert changes (updated values) for table - when table was restored
        INSERT INTO [tables_changes]
        ([table_id],
         [database_id],
         [schema],
         [name],
         [object_type],
         [subtype],
         [definition],
         [dbms_last_modification_date],
         [operation],
         [valid_from],
         [update_id]
        )
               SELECT [i].[table_id],
                      [i].[database_id],
                      [i].[schema],
                      [i].[name],
                      [i].[object_type],
                      [i].[subtype],
                      [i].[definition],
                      [i].[dbms_last_modification_date],
                      'ADDED',
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[table_id] = [d].[table_id]
                 WHERE [i].[status] = 'A'
                       AND [d].[status] = 'D';

    END;

GO

ALTER TABLE [tables] DISABLE TRIGGER [trg_tables_change_track_update];

-- Schema Change Tracking - Triggers - triggers
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert trigger's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_triggers_change_track_insert] ON [triggers]
FOR INSERT
AS

     -- skip manual objects created through Dataedo application
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [inserted]
                 WHERE [source] = 'USER'
              )
         BEGIN
             RETURN;
         END;

     INSERT INTO [triggers_changes]
     ([database_id],
      [trigger_id],
      [table_id],
      [name],
      [before],
      [after],
      [instead_of],
      [on_insert],
      [on_update],
      [on_delete],
      [disabled],
      [definition],
      [type],
      [operation],
      [valid_from],
      [update_id]
     )
            SELECT [t].[database_id],
                   [i].[trigger_id],
                   [i].[table_id],
                   [i].[name],
                   [i].[before],
                   [i].[after],
                   [i].[instead_of],
                   [i].[on_insert],
                   [i].[on_update],
                   [i].[on_delete],
                   [i].[disabled],
                   [i].[definition],
                   [i].[type],
                   'ADDED',
                   GETDATE(),
                   [i].[update_id]
              FROM [inserted] [i]
                   LEFT OUTER JOIN [tables] [t] ON [i].[table_id] = [t].[table_id]
              WHERE NOT EXISTS
                              (
                               SELECT 1
                                 FROM [schema_updates] [u]
                                 WHERE [u].[update_id] = [i].[update_id]
                                       AND [u].[type] = 'IMPORT'
                              );
GO

ALTER TABLE [triggers] DISABLE TRIGGER [trg_triggers_change_track_insert];

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert trigger's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_triggers_change_track_update] ON [triggers]
FOR UPDATE
AS

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
                      INNER JOIN [deleted] [d] ON [i].[trigger_id] = [d].[trigger_id]
                 WHERE ISNULL([d].[table_id],'') <> ISNULL([i].[table_id],'')
                       OR ISNULL([d].[name],'') <> ISNULL([i].[name],'')
                       OR ISNULL([d].[before],'') <> ISNULL([i].[before],'')
                       OR ISNULL([d].[after],'') <> ISNULL([i].[after],'')
                       OR ISNULL([d].[instead_of],'') <> ISNULL([i].[instead_of],'')
                       OR ISNULL([d].[on_insert],'') <> ISNULL([i].[on_insert],'')
                       OR ISNULL([d].[on_update],'') <> ISNULL([i].[on_update],'')
                       OR ISNULL([d].[on_delete],'') <> ISNULL([i].[on_delete],'')
                       OR ISNULL([d].[disabled],'') <> ISNULL([i].[disabled],'')
                       OR ISNULL([d].[definition],'') <> ISNULL([i].[definition],'')
                       OR ISNULL([d].[type],'') <> ISNULL([i].[type],'')
              )

         BEGIN

             UPDATE [triggers_changes]
               SET
                   [valid_to] = GETDATE()
               FROM [triggers_changes] [c]
                    INNER JOIN [inserted] [u] ON [c].[table_id] = [u].[table_id]
               WHERE [valid_to] IS NULL;

             -- insert changes (before and after values) for trigger - when trigger was updated
             INSERT INTO [triggers_changes]
             ([database_id],
              [trigger_id],
              [table_id],
              [name],
              [before],
              [after],
              [instead_of],
              [on_insert],
              [on_update],
              [on_delete],
              [disabled],
              [definition],
              [before_before],
              [before_after],
              [before_instead_of],
              [before_on_insert],
              [before_on_update],
              [before_on_delete],
              [before_disabled],
              [before_definition],
              [type],
              [operation],
              [valid_from],
              [update_id]
             )
                    SELECT [t].[database_id],
                           [i].[trigger_id],
                           [i].[table_id],
                           [i].[name],
                           [i].[before],
                           [i].[after],
                           [i].[instead_of],
                           [i].[on_insert],
                           [i].[on_update],
                           [i].[on_delete],
                           [i].[disabled],
                           [i].[definition],
                           [d].[before],
                           [d].[after],
                           [d].[instead_of],
                           [d].[on_insert],
                           [d].[on_update],
                           [d].[on_delete],
                           [d].[disabled],
                           [d].[definition],
                           [d].[type],
                           'UPDATED',
                           [cc].[valid_to],
                           [i].[update_id]
                      FROM [inserted] [i]
                           INNER JOIN [deleted] [d] ON [i].[trigger_id] = [d].[trigger_id]
                           LEFT OUTER JOIN [tables] [t] ON [d].[table_id] = [t].[table_id]
                           OUTER APPLY
                                      (
                                       SELECT MAX([valid_to]) AS [valid_to]
                                         FROM [triggers_changes] [c]
                                         WHERE [c].[trigger_id] = [d].[trigger_id]
                                               AND [c].[valid_to] IS NOT NULL
                                      ) [cc]
                      WHERE [i].[status] = 'A'
                            AND [d].[status] = 'A'
                            AND (ISNULL([d].[table_id],'') <> ISNULL([i].[table_id],'')
                                 OR ISNULL([d].[name],'') <> ISNULL([i].[name],'')
                                 OR ISNULL([d].[before],'') <> ISNULL([i].[before],'')
                                 OR ISNULL([d].[after],'') <> ISNULL([i].[after],'')
                                 OR ISNULL([d].[instead_of],'') <> ISNULL([i].[instead_of],'')
                                 OR ISNULL([d].[on_insert],'') <> ISNULL([i].[on_insert],'')
                                 OR ISNULL([d].[on_update],'') <> ISNULL([i].[on_update],'')
                                 OR ISNULL([d].[on_delete],'') <> ISNULL([i].[on_delete],'')
                                 OR ISNULL([d].[disabled],'') <> ISNULL([i].[disabled],'')
                                 OR ISNULL([d].[definition],'') <> ISNULL([i].[definition],'')
                                 OR ISNULL([d].[type],'') <> ISNULL([i].[type],''));

         END;

    BEGIN
        -- insert changes (deleted values) for trigger - when trigger was deleted
        INSERT INTO [triggers_changes]
        ([database_id],
         [trigger_id],
         [table_id],
         [name],
         [before],
         [after],
         [instead_of],
         [on_insert],
         [on_update],
         [on_delete],
         [disabled],
         [definition],
         [type],
         [operation],
         [valid_to],
         [update_id]
        )
               SELECT [t].[database_id],
                      [d].[trigger_id],
                      [d].[table_id],
                      [d].[name],
                      [d].[before],
                      [d].[after],
                      [d].[instead_of],
                      [d].[on_insert],
                      [d].[on_update],
                      [d].[on_delete],
                      [d].[disabled],
                      [d].[definition],
                      [d].[type],
                      'DELETED',
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[trigger_id] = [d].[trigger_id]
                      LEFT OUTER JOIN [tables] [t] ON [d].[table_id] = [t].[table_id]
                 WHERE [i].[status] = 'D'
                       AND [d].[status] = 'A';

        -- insert changes (updated values) for trigger - when trigger was restored
        INSERT INTO [triggers_changes]
        ([database_id],
         [trigger_id],
         [table_id],
         [name],
         [before],
         [after],
         [instead_of],
         [on_insert],
         [on_update],
         [on_delete],
         [disabled],
         [definition],
         [type],
         [operation],
         [valid_from],
         [update_id]
        )
               SELECT [t].[database_id],
                      [i].[trigger_id],
                      [i].[table_id],
                      [i].[name],
                      [i].[before],
                      [i].[after],
                      [i].[instead_of],
                      [i].[on_insert],
                      [i].[on_update],
                      [i].[on_delete],
                      [i].[disabled],
                      [i].[definition],
                      [i].[type],
                      'ADDED',
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[trigger_id] = [d].[trigger_id]
                      LEFT OUTER JOIN [tables] [t] ON [d].[table_id] = [t].[table_id]
                 WHERE [i].[status] = 'A'
                       AND [d].[status] = 'D';

    END;

GO

ALTER TABLE [triggers] DISABLE TRIGGER [trg_triggers_change_track_update];

-- Version
UPDATE [version]
  SET
      [stable] = 1
  WHERE [version] = 7
        AND [update] = 2
        AND [release] = 0;
GO