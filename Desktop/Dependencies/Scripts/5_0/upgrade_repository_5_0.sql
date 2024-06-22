INSERT INTO [version] ([version], [update], [stable]) VALUES (5, 0, 0)
GO

--BEGIN PROCEDURES
IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[columns_delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [columns_delete]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[columns_update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [columns_update]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[database_insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [database_insert]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[database_remove]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [database_remove]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[database_update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [database_update]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[database_update_title]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [database_update_title]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[ignored_objects_delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [ignored_objects_delete]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[ignored_objects_insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [ignored_objects_insert]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[module_delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [module_delete]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[module_insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [module_insert]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[module_update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [module_update]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[object_check_synchronization]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [object_check_synchronization]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[objects_clear_exists_flag]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [objects_clear_exists_flag]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[objects_module_delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [objects_module_delete]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[objects_module_insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [objects_module_insert]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[objects_set_synchronized]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [objects_set_synchronized]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[parameters_delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [parameters_delete]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[parameters_update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [parameters_update]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[procedure_delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [procedure_delete]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[procedure_update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [procedure_update]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[relation_insert_or_update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [relation_insert_or_update]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[relations_delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [relations_delete]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[synch_columns]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [synch_columns]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[synch_object_delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [synch_object_delete]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[synch_parameters]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [synch_parameters]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[synch_procedure]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [synch_procedure]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[synch_relations]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [synch_relations]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[synch_relations_columns]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [synch_relations_columns]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[synch_table]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [synch_table]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[synch_triggers]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [synch_triggers]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[synch_unique_constraints]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [synch_unique_constraints]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[synch_unique_constraints_columns]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [synch_unique_constraints_columns]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[table_delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [table_delete]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[table_update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [table_update]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[triggers_delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [triggers_delete]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[triggers_update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [triggers_update]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[unique_constraints_delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [unique_constraints_delete]
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[unique_constraints_insert_or_update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [unique_constraints_insert_or_update]
GO
--END PROCEDURES

--BEGIN FUNCTIONS
IF EXISTS (
    SELECT * FROM sysobjects WHERE id = object_id(N'[split_strings]') 
    AND xtype IN (N'FN', N'IF', N'TF')
)
    DROP FUNCTION [split_strings]
GO

IF EXISTS (
    SELECT * FROM sysobjects WHERE id = object_id(N'[get_constraint_columns]') 
    AND xtype IN (N'FN', N'IF', N'TF')
)
    DROP FUNCTION [get_constraint_columns]
GO

IF EXISTS (
    SELECT * FROM sysobjects WHERE id = object_id(N'[get_relation_join_condition]') 
    AND xtype IN (N'FN', N'IF', N'TF')
)
    DROP FUNCTION [get_relation_join_condition]
GO

IF EXISTS (
    SELECT * FROM sysobjects WHERE id = object_id(N'[procedures_modules_join_module_id]') 
    AND xtype IN (N'FN', N'IF', N'TF')
)
    DROP FUNCTION [procedures_modules_join_module_id]
GO

IF EXISTS (
    SELECT * FROM sysobjects WHERE id = object_id(N'[tables_modules_join_module_id]') 
    AND xtype IN (N'FN', N'IF', N'TF')
)
    DROP FUNCTION [tables_modules_join_module_id]
GO
--END FUNCTIONS

--BEGIN TYPES
IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'columns_list')
    DROP TYPE [columns_list]

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'ignored_objects_list')
    DROP TYPE [ignored_objects_list]

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'objects_id_description_list')
    DROP TYPE [objects_id_description_list]

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'objects_id_list')
    DROP TYPE [objects_id_list]

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'objects_name_schema_list')
    DROP TYPE [objects_name_schema_list]

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'objects_two_id_list')
    DROP TYPE [objects_two_id_list]

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'parameters_list')
    DROP TYPE [parameters_list]

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'synch_columns_list')
    DROP TYPE [synch_columns_list]

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'synch_parameters_list')
    DROP TYPE [synch_parameters_list]

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'synch_relations_columns_list')
    DROP TYPE [synch_relations_columns_list]

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'synch_relations_list')
    DROP TYPE [synch_relations_list]

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'synch_triggers_list')
    DROP TYPE [synch_triggers_list]

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'synch_unique_constraints_columns_list')
    DROP TYPE [synch_unique_constraints_columns_list]

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'synch_unique_constraints_list')
    DROP TYPE [synch_unique_constraints_list]

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'tables_relations_columns_list')
    DROP TYPE [tables_relations_columns_list]

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'triggers_list')
    DROP TYPE [triggers_list]

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'unique_constraints_columns_list')
    DROP TYPE  [unique_constraints_columns_list]
--END TYPES

--BEGIN TABLES
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[dependencies](
	[dependency_id] [int] IDENTITY(1,1) NOT NULL,
	[referencing_server] [nvarchar](250) NULL,
	[referencing_database] [nvarchar](250) NULL,
	[referencing_schema] [nvarchar](250) NULL,
	[referencing_name] [nvarchar](250) NOT NULL,
	[referencing_type] [nvarchar](50) NOT NULL,
	[referenced_server] [nvarchar](250) NULL,
	[referenced_database] [nvarchar](250) NULL,
	[referenced_schema] [nvarchar](250) NULL,
	[referenced_name] [nvarchar](250) NOT NULL,
	[referenced_type] [nvarchar](50) NULL,
	[is_caller_dependent] [char](1) NULL,
	[is_ambiguous] [char](1) NULL,
	[dependency_type] [nvarchar](10) NULL,
	[status] [char](1) NOT NULL,
	[source] [nvarchar](50) NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](100) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](100) NULL,
 CONSTRAINT [PK_dependencies] PRIMARY KEY CLUSTERED 
(
	[dependency_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[dependencies] ADD  CONSTRAINT [DF_dependencies_status]  DEFAULT ('A') FOR [status]
GO

ALTER TABLE [dbo].[dependencies] ADD  CONSTRAINT [DF_dependencies_source]  DEFAULT ('DBMS') FOR [source]
GO

ALTER TABLE [dbo].[dependencies] ADD  CONSTRAINT [DF_dependencies_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO

ALTER TABLE [dbo].[dependencies] ADD  CONSTRAINT [DF_dependencies_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO

ALTER TABLE [dbo].[dependencies] ADD  CONSTRAINT [DF_dependencies_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO

ALTER TABLE [dbo].[dependencies] ADD  CONSTRAINT [DF_dependencies_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO

GRANT DELETE ON [dependencies] TO [users] AS [dbo]
GO
GRANT INSERT ON [dependencies] TO [users] AS [dbo]
GO
GRANT SELECT ON [dependencies] TO [users] AS [dbo]
GO
GRANT UPDATE ON [dependencies] TO [users] AS [dbo]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[dependencies_descriptions](
	[dependency_descriptions_id] [int] IDENTITY(1,1) NOT NULL,
	[database_id] [int] NOT NULL,
	[referencing_server] [nvarchar](250) NULL,
	[referencing_database] [nvarchar](250) NULL,
	[referencing_schema] [nvarchar](250) NULL,
	[referencing_name] [nvarchar](250) NOT NULL,
	[referencing_type] [nvarchar](50) NOT NULL,
	[referenced_server] [nvarchar](250) NULL,
	[referenced_database] [nvarchar](250) NULL,
	[referenced_schema] [nvarchar](250) NULL,
	[referenced_name] [nvarchar](250) NOT NULL,
	[referenced_type] [nvarchar](50) NULL,
	[description] [ntext] NULL,
	[ordinal_position_uses] [int] NULL,
	[ordinal_position_used_by] [int] NULL,
	[source] [nvarchar](50) NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](100) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](100) NULL,
 CONSTRAINT [PK_dependencies_desrciptions] PRIMARY KEY CLUSTERED 
(
	[dependency_descriptions_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[dependencies_descriptions] ADD  CONSTRAINT [DF_dependencies_additions_creation_source]  DEFAULT ('DBMS') FOR [source]
GO

ALTER TABLE [dbo].[dependencies_descriptions] ADD  CONSTRAINT [DF_dependencies_additions_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO

ALTER TABLE [dbo].[dependencies_descriptions] ADD  CONSTRAINT [DF_dependencies_additions_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO

ALTER TABLE [dbo].[dependencies_descriptions] ADD  CONSTRAINT [DF_dependencies_additions_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO

ALTER TABLE [dbo].[dependencies_descriptions] ADD  CONSTRAINT [DF_dependencies_additions_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO

ALTER TABLE [dbo].[dependencies_descriptions]  WITH CHECK ADD  CONSTRAINT [FK_dependencies_desrciptions_databases] FOREIGN KEY([database_id])
REFERENCES [dbo].[databases] ([database_id])
GO

ALTER TABLE [dbo].[dependencies_descriptions] CHECK CONSTRAINT [FK_dependencies_desrciptions_databases]
GO

GRANT DELETE ON [dependencies_descriptions] TO [users] AS [dbo]
GO
GRANT INSERT ON [dependencies_descriptions] TO [users] AS [dbo]
GO
GRANT SELECT ON [dependencies_descriptions] TO [users] AS [dbo]
GO
GRANT UPDATE ON [dependencies_descriptions] TO [users] AS [dbo]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[erd_links](
	[link_id] [int] IDENTITY(1,1) NOT NULL,
	[module_id] [int] NULL,
	[relation_id] [int] NOT NULL,
	[label_pos_x] [int] NULL,
	[label_pos_y] [int] NULL,
	[show_label] [bit] NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](100) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](100) NULL,
 CONSTRAINT [PK_erd_links] PRIMARY KEY CLUSTERED 
(
	[link_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_erd_links_relation] UNIQUE NONCLUSTERED 
(
	[module_id] ASC,
	[relation_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[erd_links] ADD  CONSTRAINT [DF_erd_links_module_id]  DEFAULT ((0)) FOR [module_id]
GO

ALTER TABLE [dbo].[erd_links] ADD  CONSTRAINT [DF_erd_links_show_label]  DEFAULT ((0)) FOR [show_label]
GO

ALTER TABLE [dbo].[erd_links] ADD  CONSTRAINT [DF_erd_links_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO

ALTER TABLE [dbo].[erd_links] ADD  CONSTRAINT [DF_erd_links_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO

ALTER TABLE [dbo].[erd_links] ADD  CONSTRAINT [DF_erd_links_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO

ALTER TABLE [dbo].[erd_links] ADD  CONSTRAINT [DF_erd_links_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO

ALTER TABLE [dbo].[erd_links]  WITH CHECK ADD  CONSTRAINT [FK_erd_links_tables_relations] FOREIGN KEY([relation_id])
REFERENCES [dbo].[tables_relations] ([table_relation_id])
GO

ALTER TABLE [dbo].[erd_links] CHECK CONSTRAINT [FK_erd_links_tables_relations]
GO

GRANT DELETE ON [erd_links] TO [users] AS [dbo]
GO
GRANT INSERT ON [erd_links] TO [users] AS [dbo]
GO
GRANT SELECT ON [erd_links] TO [users] AS [dbo]
GO
GRANT UPDATE ON [erd_links] TO [users] AS [dbo]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[erd_nodes](
	[node_id] [int] IDENTITY(1,1) NOT NULL,
	[module_id] [int] NULL,
	[table_id] [int] NOT NULL,
	[pos_x] [int] NULL,
	[pos_y] [int] NULL,
	[color] [char](7) NULL,
	[width] [int] NULL,
	[height] [int] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](100) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](100) NULL,
 CONSTRAINT [PK_erd_nodes] PRIMARY KEY CLUSTERED 
(
	[node_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_erd_nodes_table] UNIQUE NONCLUSTERED 
(
	[module_id] ASC,
	[table_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[erd_nodes] ADD  CONSTRAINT [DF_erd_nodes_module_id]  DEFAULT ((0)) FOR [module_id]
GO

ALTER TABLE [dbo].[erd_nodes] ADD  CONSTRAINT [DF_erd_nodes_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO

ALTER TABLE [dbo].[erd_nodes] ADD  CONSTRAINT [DF_erd_nodes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO

ALTER TABLE [dbo].[erd_nodes] ADD  CONSTRAINT [DF_erd_nodes_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO

ALTER TABLE [dbo].[erd_nodes] ADD  CONSTRAINT [DF_erd_nodes_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO

ALTER TABLE [dbo].[erd_nodes]  WITH CHECK ADD  CONSTRAINT [FK_erd_nodes_tables] FOREIGN KEY([table_id])
REFERENCES [dbo].[tables] ([table_id])
GO

ALTER TABLE [dbo].[erd_nodes] CHECK CONSTRAINT [FK_erd_nodes_tables]
GO

GRANT DELETE ON [erd_nodes] TO [users] AS [dbo]
GO
GRANT INSERT ON [erd_nodes] TO [users] AS [dbo]
GO
GRANT SELECT ON [erd_nodes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [erd_nodes] TO [users] AS [dbo]
GO
--END TABLES

update licenses
set [key] = 'HNGGCOOAAJGJOBHDOEBILCJOFGEMJGMDFBPHLGPDHOINBFBCOAFDINOEDJOMNHFICFPILFMMEKFJCHIIDKEEKMEMAEENGLLOMMCJGPAOMFKCICLOGHPMKICFHIMDFCNEAFAPPNGCPINEFDCAHJMNNDECFNDMPBPFLOALBONBCECBGHMOCIOMECCJMBPHHDGJBNGBMNOAIFOIKIIGHNMDMCFJGBMLHJCBNKIFGFICHBNBAFONNDCILNKNHLAANFEP'
where [key] in (
 'AKGPLPKKCIHBNAONCLCDMCBCDHMPGKDCEMIIMCKCPDMIGEPKIDNNDPMOBOHGADEGEHKBLNMHEHFOGDIDGGGFFOOHKICAODFHEOANOAOMHNDHDEOJHCPPLCANIMJPDLIPNGEFMGJMKNENLPKINAGIDGOMFFOMEKJMNALCPKOEOELIKOPPFHLBAEIJIGILBKKFMNNNFAFPEABNDDGHIIMLLNAIFADGLHCAKFIHJHNKADHNLAAHFPIPJAGKCAMHILIK'
,'LPDEDKDFLJABJKLCOCBBONBKJCIMDBMAKCMDPPAKIPHFEGKFCOCNBABGAHMKJKAJKNGBHIPDEIHIFOGAJEKMLFHHDEEEHOGJEHLLEMJLJAAOLALLABLOMJPJPHEIDALBDBLJNPKCMMDPCBOPGNDBJJFNAAJFNOOACOOBIMCAPLEEIFMNJHJJDDDBIAENLCJHLHPDNHFKPEFKOMJNAAAEPEPBNDOLDBKGPFEFECFKAGNNPHNDNIOKPPGEHCLGCEAD'
,'JMJABKKKDHNBMHIOLPLKPJIOHDCPOHEBLPLDAPCCLAGOLEMNCDOEKCPAGLBBOBECANLBPLDKCNGJMDKMCNKHBHPDPPDGINEKCAOLAEEBFGBGJACEOIFIACLMCOGPKDGHCPHODLFAHBFJBFALNKMEBLAOPFOPPAOCDINJJKEHDIJJKAIGHOMOPHAPEGLGNPEHCFCAHPEHCIOCOOPFANCPKEJLBPADJICODHDDKGLFPDGBMAGMNPPLJLFBBOFAKJIF'
)
GO

UPDATE [version]
set [stable] = 1
where [version] = 5 and [update] = 0
GO