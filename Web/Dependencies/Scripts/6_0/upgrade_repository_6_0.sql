IF(
    (SELECT COUNT(*)
    FROM [version]
    WHERE [version] = 6 AND [update] = 0)
    = 0)
BEGIN
INSERT INTO [version] ([version], [update], [stable]) VALUES (6, 0, 0)
END
GO

ALTER TABLE [columns] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [columns] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [columns]
    ADD [field1]  NVARCHAR (MAX) NULL,
        [field2]  NVARCHAR (MAX) NULL,
        [field3]  NVARCHAR (MAX) NULL,
        [field4]  NVARCHAR (MAX) NULL,
        [field5]  NVARCHAR (MAX) NULL,
        [field6]  NVARCHAR (MAX) NULL,
        [field7]  NVARCHAR (MAX) NULL,
        [field8]  NVARCHAR (MAX) NULL,
        [field9]  NVARCHAR (MAX) NULL,
        [field10] NVARCHAR (MAX) NULL,
        [field11] NVARCHAR (MAX) NULL,
        [field12] NVARCHAR (MAX) NULL,
        [field13] NVARCHAR (MAX) NULL,
        [field14] NVARCHAR (MAX) NULL,
        [field15] NVARCHAR (MAX) NULL,
        [field16] NVARCHAR (MAX) NULL,
        [field17] NVARCHAR (MAX) NULL,
        [field18] NVARCHAR (MAX) NULL,
        [field19] NVARCHAR (MAX) NULL,
        [field20] NVARCHAR (MAX) NULL,
        [field21] NVARCHAR (MAX) NULL,
        [field22] NVARCHAR (MAX) NULL,
        [field23] NVARCHAR (MAX) NULL,
        [field24] NVARCHAR (MAX) NULL,
        [field25] NVARCHAR (MAX) NULL,
        [field26] NVARCHAR (MAX) NULL,
        [field27] NVARCHAR (MAX) NULL,
        [field28] NVARCHAR (MAX) NULL,
        [field29] NVARCHAR (MAX) NULL,
        [field30] NVARCHAR (MAX) NULL,
        [field31] NVARCHAR (MAX) NULL,
        [field32] NVARCHAR (MAX) NULL,
        [field33] NVARCHAR (MAX) NULL,
        [field34] NVARCHAR (MAX) NULL,
        [field35] NVARCHAR (MAX) NULL,
        [field36] NVARCHAR (MAX) NULL,
        [field37] NVARCHAR (MAX) NULL,
        [field38] NVARCHAR (MAX) NULL,
        [field39] NVARCHAR (MAX) NULL,
        [field40] NVARCHAR (MAX) NULL;
GO

ALTER TABLE [databases] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [databases] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [databases]
    ADD [field1]           NVARCHAR (MAX)   NULL,
        [field2]           NVARCHAR (MAX)   NULL,
        [field3]           NVARCHAR (MAX)   NULL,
        [field4]           NVARCHAR (MAX)   NULL,
        [field5]           NVARCHAR (MAX)   NULL,
        [field6]           NVARCHAR (MAX)   NULL,
        [field7]           NVARCHAR (MAX)   NULL,
        [field8]           NVARCHAR (MAX)   NULL,
        [field9]           NVARCHAR (MAX)   NULL,
        [field10]          NVARCHAR (MAX)   NULL,
        [field11]          NVARCHAR (MAX)   NULL,
        [field12]          NVARCHAR (MAX)   NULL,
        [field13]          NVARCHAR (MAX)   NULL,
        [field14]          NVARCHAR (MAX)   NULL,
        [field15]          NVARCHAR (MAX)   NULL,
        [field16]          NVARCHAR (MAX)   NULL,
        [field17]          NVARCHAR (MAX)   NULL,
        [field18]          NVARCHAR (MAX)   NULL,
        [field19]          NVARCHAR (MAX)   NULL,
        [field20]          NVARCHAR (MAX)   NULL,
        [field21]          NVARCHAR (MAX)   NULL,
        [field22]          NVARCHAR (MAX)   NULL,
        [field23]          NVARCHAR (MAX)   NULL,
        [field24]          NVARCHAR (MAX)   NULL,
        [field25]          NVARCHAR (MAX)   NULL,
        [field26]          NVARCHAR (MAX)   NULL,
        [field27]          NVARCHAR (MAX)   NULL,
        [field28]          NVARCHAR (MAX)   NULL,
        [field29]          NVARCHAR (MAX)   NULL,
        [field30]          NVARCHAR (MAX)   NULL,
        [field31]          NVARCHAR (MAX)   NULL,
        [field32]          NVARCHAR (MAX)   NULL,
        [field33]          NVARCHAR (MAX)   NULL,
        [field34]          NVARCHAR (MAX)   NULL,
        [field35]          NVARCHAR (MAX)   NULL,
        [field36]          NVARCHAR (MAX)   NULL,
        [field37]          NVARCHAR (MAX)   NULL,
        [field38]          NVARCHAR (MAX)   NULL,
        [field39]          NVARCHAR (MAX)   NULL,
        [field40]          NVARCHAR (MAX)   NULL,
        [guid]             UNIQUEIDENTIFIER CONSTRAINT [DF_Databases_guid] DEFAULT (newsequentialid()) NOT NULL,
        [multiple_schemas] BIT              NULL;
GO

ALTER TABLE [dependencies] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [dependencies] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO

ALTER TABLE [dependencies_descriptions] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [dependencies_descriptions] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO

ALTER TABLE [erd_links] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [erd_links] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [erd_links] ALTER COLUMN [show_join_condition] BIT NOT NULL;
GO

ALTER TABLE [erd_nodes] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [erd_nodes] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO

ALTER TABLE [ignored_objects] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [ignored_objects] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO

ALTER TABLE [licenses] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [licenses] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO

ALTER TABLE [modules] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [modules] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [modules]
    ADD [display_documentation_name_mode] NVARCHAR (25)  CONSTRAINT [DF_Modules_display_documentation_name_mode] DEFAULT (N'EXTERNAL_ENTITIES_ONLY') NULL,
        [field1]                          NVARCHAR (MAX) NULL,
        [field2]                          NVARCHAR (MAX) NULL,
        [field3]                          NVARCHAR (MAX) NULL,
        [field4]                          NVARCHAR (MAX) NULL,
        [field5]                          NVARCHAR (MAX) NULL,
        [field6]                          NVARCHAR (MAX) NULL,
        [field7]                          NVARCHAR (MAX) NULL,
        [field8]                          NVARCHAR (MAX) NULL,
        [field9]                          NVARCHAR (MAX) NULL,
        [field10]                         NVARCHAR (MAX) NULL,
        [field11]                         NVARCHAR (MAX) NULL,
        [field12]                         NVARCHAR (MAX) NULL,
        [field13]                         NVARCHAR (MAX) NULL,
        [field14]                         NVARCHAR (MAX) NULL,
        [field15]                         NVARCHAR (MAX) NULL,
        [field16]                         NVARCHAR (MAX) NULL,
        [field17]                         NVARCHAR (MAX) NULL,
        [field18]                         NVARCHAR (MAX) NULL,
        [field19]                         NVARCHAR (MAX) NULL,
        [field20]                         NVARCHAR (MAX) NULL,
        [field21]                         NVARCHAR (MAX) NULL,
        [field22]                         NVARCHAR (MAX) NULL,
        [field23]                         NVARCHAR (MAX) NULL,
        [field24]                         NVARCHAR (MAX) NULL,
        [field25]                         NVARCHAR (MAX) NULL,
        [field26]                         NVARCHAR (MAX) NULL,
        [field27]                         NVARCHAR (MAX) NULL,
        [field28]                         NVARCHAR (MAX) NULL,
        [field29]                         NVARCHAR (MAX) NULL,
        [field30]                         NVARCHAR (MAX) NULL,
        [field31]                         NVARCHAR (MAX) NULL,
        [field32]                         NVARCHAR (MAX) NULL,
        [field33]                         NVARCHAR (MAX) NULL,
        [field34]                         NVARCHAR (MAX) NULL,
        [field35]                         NVARCHAR (MAX) NULL,
        [field36]                         NVARCHAR (MAX) NULL,
        [field37]                         NVARCHAR (MAX) NULL,
        [field38]                         NVARCHAR (MAX) NULL,
        [field39]                         NVARCHAR (MAX) NULL,
        [field40]                         NVARCHAR (MAX) NULL;
GO

ALTER TABLE [parameters] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [parameters] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [parameters]
    ADD [field1]  NVARCHAR (MAX) NULL,
        [field2]  NVARCHAR (MAX) NULL,
        [field3]  NVARCHAR (MAX) NULL,
        [field4]  NVARCHAR (MAX) NULL,
        [field5]  NVARCHAR (MAX) NULL,
        [field6]  NVARCHAR (MAX) NULL,
        [field7]  NVARCHAR (MAX) NULL,
        [field8]  NVARCHAR (MAX) NULL,
        [field9]  NVARCHAR (MAX) NULL,
        [field10] NVARCHAR (MAX) NULL,
        [field11] NVARCHAR (MAX) NULL,
        [field12] NVARCHAR (MAX) NULL,
        [field13] NVARCHAR (MAX) NULL,
        [field14] NVARCHAR (MAX) NULL,
        [field15] NVARCHAR (MAX) NULL,
        [field16] NVARCHAR (MAX) NULL,
        [field17] NVARCHAR (MAX) NULL,
        [field18] NVARCHAR (MAX) NULL,
        [field19] NVARCHAR (MAX) NULL,
        [field20] NVARCHAR (MAX) NULL,
        [field21] NVARCHAR (MAX) NULL,
        [field22] NVARCHAR (MAX) NULL,
        [field23] NVARCHAR (MAX) NULL,
        [field24] NVARCHAR (MAX) NULL,
        [field25] NVARCHAR (MAX) NULL,
        [field26] NVARCHAR (MAX) NULL,
        [field27] NVARCHAR (MAX) NULL,
        [field28] NVARCHAR (MAX) NULL,
        [field29] NVARCHAR (MAX) NULL,
        [field30] NVARCHAR (MAX) NULL,
        [field31] NVARCHAR (MAX) NULL,
        [field32] NVARCHAR (MAX) NULL,
        [field33] NVARCHAR (MAX) NULL,
        [field34] NVARCHAR (MAX) NULL,
        [field35] NVARCHAR (MAX) NULL,
        [field36] NVARCHAR (MAX) NULL,
        [field37] NVARCHAR (MAX) NULL,
        [field38] NVARCHAR (MAX) NULL,
        [field39] NVARCHAR (MAX) NULL,
        [field40] NVARCHAR (MAX) NULL;
GO
ALTER TABLE [procedures] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [procedures] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [procedures]
    ADD [field1]  NVARCHAR (MAX) NULL,
        [field2]  NVARCHAR (MAX) NULL,
        [field3]  NVARCHAR (MAX) NULL,
        [field4]  NVARCHAR (MAX) NULL,
        [field5]  NVARCHAR (MAX) NULL,
        [field6]  NVARCHAR (MAX) NULL,
        [field7]  NVARCHAR (MAX) NULL,
        [field8]  NVARCHAR (MAX) NULL,
        [field9]  NVARCHAR (MAX) NULL,
        [field10] NVARCHAR (MAX) NULL,
        [field11] NVARCHAR (MAX) NULL,
        [field12] NVARCHAR (MAX) NULL,
        [field13] NVARCHAR (MAX) NULL,
        [field14] NVARCHAR (MAX) NULL,
        [field15] NVARCHAR (MAX) NULL,
        [field16] NVARCHAR (MAX) NULL,
        [field17] NVARCHAR (MAX) NULL,
        [field18] NVARCHAR (MAX) NULL,
        [field19] NVARCHAR (MAX) NULL,
        [field20] NVARCHAR (MAX) NULL,
        [field21] NVARCHAR (MAX) NULL,
        [field22] NVARCHAR (MAX) NULL,
        [field23] NVARCHAR (MAX) NULL,
        [field24] NVARCHAR (MAX) NULL,
        [field25] NVARCHAR (MAX) NULL,
        [field26] NVARCHAR (MAX) NULL,
        [field27] NVARCHAR (MAX) NULL,
        [field28] NVARCHAR (MAX) NULL,
        [field29] NVARCHAR (MAX) NULL,
        [field30] NVARCHAR (MAX) NULL,
        [field31] NVARCHAR (MAX) NULL,
        [field32] NVARCHAR (MAX) NULL,
        [field33] NVARCHAR (MAX) NULL,
        [field34] NVARCHAR (MAX) NULL,
        [field35] NVARCHAR (MAX) NULL,
        [field36] NVARCHAR (MAX) NULL,
        [field37] NVARCHAR (MAX) NULL,
        [field38] NVARCHAR (MAX) NULL,
        [field39] NVARCHAR (MAX) NULL,
        [field40] NVARCHAR (MAX) NULL;
GO

ALTER TABLE [procedures_modules] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [procedures_modules] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO

ALTER TABLE [tables] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [tables] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [tables]
    ADD [field1]  NVARCHAR (MAX) NULL,
        [field2]  NVARCHAR (MAX) NULL,
        [field3]  NVARCHAR (MAX) NULL,
        [field4]  NVARCHAR (MAX) NULL,
        [field5]  NVARCHAR (MAX) NULL,
        [field6]  NVARCHAR (MAX) NULL,
        [field7]  NVARCHAR (MAX) NULL,
        [field8]  NVARCHAR (MAX) NULL,
        [field9]  NVARCHAR (MAX) NULL,
        [field10] NVARCHAR (MAX) NULL,
        [field11] NVARCHAR (MAX) NULL,
        [field12] NVARCHAR (MAX) NULL,
        [field13] NVARCHAR (MAX) NULL,
        [field14] NVARCHAR (MAX) NULL,
        [field15] NVARCHAR (MAX) NULL,
        [field16] NVARCHAR (MAX) NULL,
        [field17] NVARCHAR (MAX) NULL,
        [field18] NVARCHAR (MAX) NULL,
        [field19] NVARCHAR (MAX) NULL,
        [field20] NVARCHAR (MAX) NULL,
        [field21] NVARCHAR (MAX) NULL,
        [field22] NVARCHAR (MAX) NULL,
        [field23] NVARCHAR (MAX) NULL,
        [field24] NVARCHAR (MAX) NULL,
        [field25] NVARCHAR (MAX) NULL,
        [field26] NVARCHAR (MAX) NULL,
        [field27] NVARCHAR (MAX) NULL,
        [field28] NVARCHAR (MAX) NULL,
        [field29] NVARCHAR (MAX) NULL,
        [field30] NVARCHAR (MAX) NULL,
        [field31] NVARCHAR (MAX) NULL,
        [field32] NVARCHAR (MAX) NULL,
        [field33] NVARCHAR (MAX) NULL,
        [field34] NVARCHAR (MAX) NULL,
        [field35] NVARCHAR (MAX) NULL,
        [field36] NVARCHAR (MAX) NULL,
        [field37] NVARCHAR (MAX) NULL,
        [field38] NVARCHAR (MAX) NULL,
        [field39] NVARCHAR (MAX) NULL,
        [field40] NVARCHAR (MAX) NULL;
GO

ALTER TABLE [tables_modules] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [tables_modules] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO

ALTER TABLE [tables_relations] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [tables_relations] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [tables_relations]
    ADD [field1]  NVARCHAR (MAX) NULL,
        [field2]  NVARCHAR (MAX) NULL,
        [field3]  NVARCHAR (MAX) NULL,
        [field4]  NVARCHAR (MAX) NULL,
        [field5]  NVARCHAR (MAX) NULL,
        [field6]  NVARCHAR (MAX) NULL,
        [field7]  NVARCHAR (MAX) NULL,
        [field8]  NVARCHAR (MAX) NULL,
        [field9]  NVARCHAR (MAX) NULL,
        [field10] NVARCHAR (MAX) NULL,
        [field11] NVARCHAR (MAX) NULL,
        [field12] NVARCHAR (MAX) NULL,
        [field13] NVARCHAR (MAX) NULL,
        [field14] NVARCHAR (MAX) NULL,
        [field15] NVARCHAR (MAX) NULL,
        [field16] NVARCHAR (MAX) NULL,
        [field17] NVARCHAR (MAX) NULL,
        [field18] NVARCHAR (MAX) NULL,
        [field19] NVARCHAR (MAX) NULL,
        [field20] NVARCHAR (MAX) NULL,
        [field21] NVARCHAR (MAX) NULL,
        [field22] NVARCHAR (MAX) NULL,
        [field23] NVARCHAR (MAX) NULL,
        [field24] NVARCHAR (MAX) NULL,
        [field25] NVARCHAR (MAX) NULL,
        [field26] NVARCHAR (MAX) NULL,
        [field27] NVARCHAR (MAX) NULL,
        [field28] NVARCHAR (MAX) NULL,
        [field29] NVARCHAR (MAX) NULL,
        [field30] NVARCHAR (MAX) NULL,
        [field31] NVARCHAR (MAX) NULL,
        [field32] NVARCHAR (MAX) NULL,
        [field33] NVARCHAR (MAX) NULL,
        [field34] NVARCHAR (MAX) NULL,
        [field35] NVARCHAR (MAX) NULL,
        [field36] NVARCHAR (MAX) NULL,
        [field37] NVARCHAR (MAX) NULL,
        [field38] NVARCHAR (MAX) NULL,
        [field39] NVARCHAR (MAX) NULL,
        [field40] NVARCHAR (MAX) NULL;
GO

ALTER TABLE [tables_relations_columns] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [tables_relations_columns] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO

ALTER TABLE [triggers] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [triggers] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [triggers]
    ADD [field1]  NVARCHAR (MAX) NULL,
        [field2]  NVARCHAR (MAX) NULL,
        [field3]  NVARCHAR (MAX) NULL,
        [field4]  NVARCHAR (MAX) NULL,
        [field5]  NVARCHAR (MAX) NULL,
        [field6]  NVARCHAR (MAX) NULL,
        [field7]  NVARCHAR (MAX) NULL,
        [field8]  NVARCHAR (MAX) NULL,
        [field9]  NVARCHAR (MAX) NULL,
        [field10] NVARCHAR (MAX) NULL,
        [field11] NVARCHAR (MAX) NULL,
        [field12] NVARCHAR (MAX) NULL,
        [field13] NVARCHAR (MAX) NULL,
        [field14] NVARCHAR (MAX) NULL,
        [field15] NVARCHAR (MAX) NULL,
        [field16] NVARCHAR (MAX) NULL,
        [field17] NVARCHAR (MAX) NULL,
        [field18] NVARCHAR (MAX) NULL,
        [field19] NVARCHAR (MAX) NULL,
        [field20] NVARCHAR (MAX) NULL,
        [field21] NVARCHAR (MAX) NULL,
        [field22] NVARCHAR (MAX) NULL,
        [field23] NVARCHAR (MAX) NULL,
        [field24] NVARCHAR (MAX) NULL,
        [field25] NVARCHAR (MAX) NULL,
        [field26] NVARCHAR (MAX) NULL,
        [field27] NVARCHAR (MAX) NULL,
        [field28] NVARCHAR (MAX) NULL,
        [field29] NVARCHAR (MAX) NULL,
        [field30] NVARCHAR (MAX) NULL,
        [field31] NVARCHAR (MAX) NULL,
        [field32] NVARCHAR (MAX) NULL,
        [field33] NVARCHAR (MAX) NULL,
        [field34] NVARCHAR (MAX) NULL,
        [field35] NVARCHAR (MAX) NULL,
        [field36] NVARCHAR (MAX) NULL,
        [field37] NVARCHAR (MAX) NULL,
        [field38] NVARCHAR (MAX) NULL,
        [field39] NVARCHAR (MAX) NULL,
        [field40] NVARCHAR (MAX) NULL;
GO

ALTER TABLE [unique_constraints] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [unique_constraints] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [unique_constraints]
    ADD [field1]  NVARCHAR (MAX) NULL,
        [field2]  NVARCHAR (MAX) NULL,
        [field3]  NVARCHAR (MAX) NULL,
        [field4]  NVARCHAR (MAX) NULL,
        [field5]  NVARCHAR (MAX) NULL,
        [field6]  NVARCHAR (MAX) NULL,
        [field7]  NVARCHAR (MAX) NULL,
        [field8]  NVARCHAR (MAX) NULL,
        [field9]  NVARCHAR (MAX) NULL,
        [field10] NVARCHAR (MAX) NULL,
        [field11] NVARCHAR (MAX) NULL,
        [field12] NVARCHAR (MAX) NULL,
        [field13] NVARCHAR (MAX) NULL,
        [field14] NVARCHAR (MAX) NULL,
        [field15] NVARCHAR (MAX) NULL,
        [field16] NVARCHAR (MAX) NULL,
        [field17] NVARCHAR (MAX) NULL,
        [field18] NVARCHAR (MAX) NULL,
        [field19] NVARCHAR (MAX) NULL,
        [field20] NVARCHAR (MAX) NULL,
        [field21] NVARCHAR (MAX) NULL,
        [field22] NVARCHAR (MAX) NULL,
        [field23] NVARCHAR (MAX) NULL,
        [field24] NVARCHAR (MAX) NULL,
        [field25] NVARCHAR (MAX) NULL,
        [field26] NVARCHAR (MAX) NULL,
        [field27] NVARCHAR (MAX) NULL,
        [field28] NVARCHAR (MAX) NULL,
        [field29] NVARCHAR (MAX) NULL,
        [field30] NVARCHAR (MAX) NULL,
        [field31] NVARCHAR (MAX) NULL,
        [field32] NVARCHAR (MAX) NULL,
        [field33] NVARCHAR (MAX) NULL,
        [field34] NVARCHAR (MAX) NULL,
        [field35] NVARCHAR (MAX) NULL,
        [field36] NVARCHAR (MAX) NULL,
        [field37] NVARCHAR (MAX) NULL,
        [field38] NVARCHAR (MAX) NULL,
        [field39] NVARCHAR (MAX) NULL,
        [field40] NVARCHAR (MAX) NULL;
GO

ALTER TABLE [unique_constraints_columns] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [unique_constraints_columns] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO

ALTER TABLE [version] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;
GO
ALTER TABLE [version] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;
GO

CREATE TABLE [custom_fields] (
    [custom_field_id]          INT             IDENTITY (1, 1) NOT NULL,
    [ordinal_position]         INT             NOT NULL,
    [field_name]               NVARCHAR (50)   NULL,
    [title]                    NVARCHAR (250)  NOT NULL,
    [code]                     NVARCHAR (255)  NOT NULL,
    [description]              NVARCHAR (MAX)  NULL,
    [table_visibility]         BIT             NOT NULL,
    [procedure_visibility]     BIT             NOT NULL,
    [column_visibility]        BIT             NOT NULL,
    [relation_visibility]      BIT             NOT NULL,
    [key_visibility]           BIT             NOT NULL,
    [trigger_visibility]       BIT             NOT NULL,
    [parameter_visibility]     BIT             NOT NULL,
    [module_visibility]        BIT             NOT NULL,
    [documentation_visibility] BIT             NOT NULL,
    [creation_date]            DATETIME        NOT NULL,
    [created_by]               NVARCHAR (1024) NULL,
    [last_modification_date]   DATETIME        NOT NULL,
    [modified_by]              NVARCHAR (1024) NULL,
    CONSTRAINT [PK_custom_fields] PRIMARY KEY ([custom_field_id]),
    CONSTRAINT [UK_custom_fields_title] UNIQUE ([title])
);
GO
GRANT DELETE ON [custom_fields] TO [users] AS [dbo]
GO
GRANT INSERT ON [custom_fields] TO [users] AS [dbo]
GO
GRANT SELECT ON [custom_fields] TO [users] AS [dbo]
GO
GRANT UPDATE ON [custom_fields] TO [users] AS [dbo]
GO

CREATE TABLE [documentation_custom_fields] (
    [documentation_custom_field_id] INT IDENTITY (1, 1) NOT NULL, 
    [custom_field_id]   INT            NOT NULL,
    [database_id]       INT            NOT NULL,
    [extended_property] NVARCHAR (128) NOT NULL,
    [creation_date]          DATETIME        CONSTRAINT [DF_documentation_custom_fields_creation_date] DEFAULT (getdate()) NOT NULL,
    [created_by]             NVARCHAR (1024) CONSTRAINT [DF_documentation_custom_fields_created_by] DEFAULT (suser_sname()) NULL,
    [last_modification_date] DATETIME        CONSTRAINT [DF_documentation_custom_fields_last_modification_date] DEFAULT (getdate()) NOT NULL,
    [modified_by]            NVARCHAR (1024) CONSTRAINT [DF_documentation_custom_fields_modified_by] DEFAULT (suser_sname()) NULL,
    CONSTRAINT [PK_documentation_custom_fields] PRIMARY KEY ([documentation_custom_field_id]), 
    CONSTRAINT [FK_documentation_custom_fields_databases] FOREIGN KEY (database_id) REFERENCES [databases]([database_id]), 
    CONSTRAINT [FK_documentation_custom_fields_custom_fields] FOREIGN KEY (custom_field_id) REFERENCES [custom_fields](custom_field_id), 
    CONSTRAINT [UK_documentation_custom_fields] UNIQUE NONCLUSTERED ([custom_field_id] ASC, [database_id] ASC)
);
GO
GRANT DELETE ON [documentation_custom_fields] TO [users] AS [dbo]
GO
GRANT INSERT ON [documentation_custom_fields] TO [users] AS [dbo]
GO
GRANT SELECT ON [documentation_custom_fields] TO [users] AS [dbo]
GO
GRANT UPDATE ON [documentation_custom_fields] TO [users] AS [dbo]
GO

CREATE TABLE [guid] (
    [guid] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_guid] PRIMARY KEY ([guid])
);
GO
GRANT DELETE ON [guid] TO [users] AS [dbo]
GO
GRANT INSERT ON [guid] TO [users] AS [dbo]
GO
GRANT SELECT ON [guid] TO [users] AS [dbo]
GO
GRANT UPDATE ON [guid] TO [users] AS [dbo]
GO

ALTER TABLE [custom_fields]
    ADD CONSTRAINT [DF_custom_fields_table_visibility] DEFAULT ((1)) FOR [table_visibility];
GO
ALTER TABLE [custom_fields]
    ADD CONSTRAINT [DF_custom_fields_procedure_visibility] DEFAULT ((1)) FOR [procedure_visibility];
GO
ALTER TABLE [custom_fields]
    ADD CONSTRAINT [DF_custom_fields_column_visibility] DEFAULT ((1)) FOR [column_visibility];
GO
ALTER TABLE [custom_fields]
    ADD CONSTRAINT [DF_custom_fields_relation_visibility] DEFAULT ((1)) FOR [relation_visibility];
GO
ALTER TABLE [custom_fields]
    ADD CONSTRAINT [DF_custom_fields_key_visibility] DEFAULT ((1)) FOR [key_visibility];
GO
ALTER TABLE [custom_fields]
    ADD CONSTRAINT [DF_custom_fields_trigger_visibility] DEFAULT ((1)) FOR [trigger_visibility];
GO
ALTER TABLE [custom_fields]
    ADD CONSTRAINT [DF_custom_fields_parameter_visibility] DEFAULT ((1)) FOR [parameter_visibility];
GO
ALTER TABLE [custom_fields]
    ADD CONSTRAINT [DF_custom_fields_module_visibility] DEFAULT ((1)) FOR [module_visibility];
GO
ALTER TABLE [custom_fields]
    ADD CONSTRAINT [DF_custom_fields_documentation_visibility] DEFAULT ((1)) FOR [documentation_visibility];
GO
ALTER TABLE [custom_fields]
    ADD CONSTRAINT [DF_custom_fields_creation_date] DEFAULT (getdate()) FOR [creation_date];
GO
ALTER TABLE [custom_fields]
    ADD CONSTRAINT [DF_custom_fields_created_by] DEFAULT (suser_sname()) FOR [created_by];
GO
ALTER TABLE [custom_fields]
    ADD CONSTRAINT [DF_custom_fields_last_modification_date] DEFAULT (getdate()) FOR [last_modification_date];
GO
ALTER TABLE [custom_fields]
    ADD CONSTRAINT [DF_custom_fields_modified_by] DEFAULT (suser_sname()) FOR [modified_by];
GO
ALTER TABLE [guid]
    ADD CONSTRAINT [DF_guid_guid] DEFAULT (newsequentialid()) FOR [guid];
GO

-- =============================================
-- Author:		£ukasz Gil
-- Create date: 2017-09-22
-- Description:	Updates last_modification_date, modified_by custom_fields on insert or update
-- =============================================
CREATE TRIGGER [trg_custom_fields_Modify]
   ON  [custom_fields]
   AFTER INSERT,UPDATE
AS 
BEGIN
	UPDATE [custom_fields]
	SET  last_modification_date = GETDATE(),
		 modified_by = suser_sname()
	WHERE custom_field_id IN (SELECT DISTINCT custom_field_id FROM Inserted)
END
GO

-- =============================================
-- Author:		£ukasz Gil
-- Create date: 2017-09-22
-- Description:	Updates last_modification_date, modified_by custom_fields on insert or update
-- =============================================
CREATE TRIGGER [trg_documentation_custom_fields_Modify]
   ON  [documentation_custom_fields]
   AFTER INSERT,UPDATE
AS 
BEGIN
	UPDATE [documentation_custom_fields]
	SET  last_modification_date = GETDATE(),
		 modified_by = suser_sname()
	WHERE documentation_custom_field_id IN (SELECT DISTINCT documentation_custom_field_id FROM Inserted)
END
GO

-- =============================================
-- Author:		£ukasz Gil
-- Create date: 2017-09-22
-- Description:	Updates last_modification_date, modified_by dependencies on insert or update
-- =============================================
CREATE TRIGGER [trg_dependencies_Modify]
   ON  [dependencies]
   AFTER INSERT,UPDATE
AS 
BEGIN
	UPDATE [dependencies]
	SET  last_modification_date = GETDATE(),
		 modified_by = suser_sname()
	WHERE dependency_id IN (SELECT DISTINCT dependency_id FROM Inserted)
END
GO

-- =============================================
-- Author:		£ukasz Gil
-- Create date: 2017-09-22
-- Description:	Updates last_modification_date, modified_by dependencies_descriptions on insert or update
-- =============================================
CREATE TRIGGER [trg_dependencies_descriptions_Modify]
   ON  [dependencies_descriptions]
   AFTER INSERT,UPDATE
AS 
BEGIN
	UPDATE [dependencies_descriptions]
	SET  last_modification_date = GETDATE(),
		 modified_by = suser_sname()
	WHERE dependency_descriptions_id IN (SELECT DISTINCT dependency_descriptions_id FROM Inserted)
END
GO

-- =============================================
-- Author:		£ukasz Gil
-- Create date: 2017-09-22
-- Description:	Updates last_modification_date, modified_by erd_links on insert or update
-- =============================================
CREATE TRIGGER [trg_erd_links_descriptions_Modify]
   ON  [erd_links]
   AFTER INSERT,UPDATE
AS 
BEGIN
	UPDATE [erd_links]
	SET  last_modification_date = GETDATE(),
		 modified_by = suser_sname()
	WHERE link_id IN (SELECT DISTINCT link_id FROM Inserted)
END
GO

-- =============================================
-- Author:		£ukasz Gil
-- Create date: 2017-09-22
-- Description:	Updates last_modification_date, modified_by erd_nodes on insert or update
-- =============================================
CREATE TRIGGER [trg_erd_nodes_descriptions_Modify]
   ON  [erd_nodes]
   AFTER INSERT,UPDATE
AS 
BEGIN
	UPDATE [erd_nodes]
	SET  last_modification_date = GETDATE(),
		 modified_by = suser_sname()
	WHERE node_id IN (SELECT DISTINCT node_id FROM Inserted)
END
GO

UPDATE [version]
set [stable] = 1
where [version] = 6 and [update] = 0
GO

IF((select count(*) from [guid]) = 0)
BEGIN
insert into [guid] default values;
END
GO
