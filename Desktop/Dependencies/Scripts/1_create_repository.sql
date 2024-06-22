/****** Object:  DatabaseRole [users]    Script Date: 7/5/2022 9:26:43 PM ******/
CREATE ROLE [users]
GO
/****** Object:  DatabaseRole [admins]    Script Date: 7/5/2022 9:26:43 PM ******/
CREATE ROLE [admins]
GO
GRANT VIEW DEFINITION TO [admins] AS [dbo]
GO
/****** Object:  Table [changes_history]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [changes_history](
	[changes_history_id] [int] IDENTITY(1,1) NOT NULL,
	[database_id] [int] NOT NULL,
	[table] [nvarchar](100) NOT NULL,
	[column] [nvarchar](100) NOT NULL,
	[row_id] [int] NOT NULL,
	[value] [nvarchar](max) NULL,
	[value_plain] [nvarchar](max) NULL,
	[datetime] [datetime] NOT NULL,
	[user_id] [int] NULL,
	[product] [nvarchar](100) NULL,
	[client_version] [nvarchar](100) NULL,
	[source_id] [int] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
 CONSTRAINT [PK_changes_history] PRIMARY KEY CLUSTERED 
(
	[changes_history_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [changes_history] TO [admins] AS [dbo]
GO
GRANT INSERT ON [changes_history] TO [admins] AS [dbo]
GO
GRANT SELECT ON [changes_history] TO [admins] AS [dbo]
GO
GRANT UPDATE ON [changes_history] TO [admins] AS [dbo]
GO
GRANT INSERT ON [changes_history] TO [users] AS [dbo]
GO
GRANT SELECT ON [changes_history] TO [users] AS [dbo]
GO
GRANT UPDATE ON [changes_history] TO [users] AS [dbo]
GO
/****** Object:  Table [classificator_masks]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [classificator_masks](
	[classificator_mask_id] [int] IDENTITY(1,1) NOT NULL,
	[built_in_id] [int] NULL,
	[mask_name] [nvarchar](250) NOT NULL,
	[mask] [nvarchar](250) NOT NULL,
	[data_types] [nvarchar](250) NULL,
	[sort] [int] NOT NULL,
	[is_column] [bit] NOT NULL,
	[is_title] [bit] NOT NULL,
	[is_description] [bit] NOT NULL,
	[comments] [nvarchar](max) NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_classificator_masks] PRIMARY KEY CLUSTERED 
(
	[classificator_mask_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [classificator_masks] TO [users] AS [dbo]
GO
GRANT INSERT ON [classificator_masks] TO [users] AS [dbo]
GO
GRANT SELECT ON [classificator_masks] TO [users] AS [dbo]
GO
GRANT UPDATE ON [classificator_masks] TO [users] AS [dbo]
GO
/****** Object:  Table [classificator_rules]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [classificator_rules](
	[classificator_rule_id] [int] IDENTITY(1,1) NOT NULL,
	[built_in_id] [int] NULL,
	[classificator_id] [int] NOT NULL,
	[mask_name] [nvarchar](250) NOT NULL,
	[custom_field_1_value] [nvarchar](250) NULL,
	[custom_field_2_value] [nvarchar](250) NULL,
	[custom_field_3_value] [nvarchar](250) NULL,
	[custom_field_4_value] [nvarchar](250) NULL,
	[custom_field_5_value] [nvarchar](250) NULL,
	[comments] [nvarchar](max) NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_classificator_rules] PRIMARY KEY CLUSTERED 
(
	[classificator_rule_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [classificator_rules] TO [users] AS [dbo]
GO
GRANT INSERT ON [classificator_rules] TO [users] AS [dbo]
GO
GRANT SELECT ON [classificator_rules] TO [users] AS [dbo]
GO
GRANT UPDATE ON [classificator_rules] TO [users] AS [dbo]
GO
/****** Object:  Table [classificators]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [classificators](
	[classificator_id] [int] IDENTITY(1,1) NOT NULL,
	[built_in_id] [int] NULL,
	[title] [nvarchar](250) NOT NULL,
	[custom_field_1_name] [nvarchar](250) NULL,
	[custom_field_2_name] [nvarchar](250) NULL,
	[custom_field_3_name] [nvarchar](250) NULL,
	[custom_field_4_name] [nvarchar](250) NULL,
	[custom_field_5_name] [nvarchar](250) NULL,
	[custom_field_1_id] [int] NULL,
	[custom_field_2_id] [int] NULL,
	[custom_field_3_id] [int] NULL,
	[custom_field_4_id] [int] NULL,
	[custom_field_5_id] [int] NULL,
	[custom_field_1_definition] [nvarchar](max) NULL,
	[custom_field_2_definition] [nvarchar](max) NULL,
	[custom_field_3_definition] [nvarchar](max) NULL,
	[custom_field_4_definition] [nvarchar](max) NULL,
	[custom_field_5_definition] [nvarchar](max) NULL,
	[description] [nvarchar](max) NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
	[custom_field_1_class_id] [int] NULL,
	[custom_field_2_class_id] [int] NULL,
	[custom_field_3_class_id] [int] NULL,
	[custom_field_4_class_id] [int] NULL,
	[custom_field_5_class_id] [int] NULL,
 CONSTRAINT [PK_classificators] PRIMARY KEY CLUSTERED 
(
	[classificator_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [classificators] TO [users] AS [dbo]
GO
GRANT INSERT ON [classificators] TO [users] AS [dbo]
GO
GRANT SELECT ON [classificators] TO [users] AS [dbo]
GO
GRANT UPDATE ON [classificators] TO [users] AS [dbo]
GO
/****** Object:  Table [column_values]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [column_values](
	[value_id] [int] IDENTITY(1,1) NOT NULL,
	[column_id] [int] NOT NULL,
	[value] [varchar](250) NULL,
	[type] [char](1) NULL,
	[row_count] [bigint] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_column_values] PRIMARY KEY CLUSTERED 
(
	[value_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [column_values] TO [users] AS [dbo]
GO
GRANT INSERT ON [column_values] TO [users] AS [dbo]
GO
GRANT SELECT ON [column_values] TO [users] AS [dbo]
GO
GRANT UPDATE ON [column_values] TO [users] AS [dbo]
GO
/****** Object:  Table [columns]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [columns](
	[column_id] [int] IDENTITY(1,1) NOT NULL,
	[table_id] [int] NOT NULL,
	[ordinal_position] [int] NULL,
	[name] [nvarchar](250) NOT NULL,
	[title] [nvarchar](250) NULL,
	[description] [nvarchar](max) NULL,
	[datatype] [nvarchar](250) NULL,
	[data_length] [nvarchar](50) NULL,
	[primary_key] [char](1) NOT NULL,
	[nullable] [bit] NOT NULL,
	[default_def] [nvarchar](4000) NULL,
	[identity_def] [nvarchar](50) NULL,
	[status] [char](1) NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[default_value] [nvarchar](max) NULL,
	[is_identity] [bit] NOT NULL,
	[is_computed] [bit] NOT NULL,
	[computed_formula] [nvarchar](max) NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[source_id] [int] NULL,
	[source] [nvarchar](50) NOT NULL,
	[sort] [int] NOT NULL,
	[temp_sync_status] [bit] NOT NULL,
	[update_id] [int] NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
	[parent_id] [int] NULL,
	[path] [nvarchar](max) NULL,
	[level] [int] NOT NULL,
	[item_type] [nvarchar](100) NOT NULL,
	[comments_count] [int] NOT NULL,
	[rating] [decimal](3, 2) NOT NULL,
	[rating_count] [int] NOT NULL,
	[open_warnings_count] [int] NOT NULL,
	[warnings_count] [int] NOT NULL,
	[open_todos_count] [int] NOT NULL,
	[todos_count] [int] NOT NULL,
	[profiling_date] [datetime] NULL,
	[profiling_data_type] [varchar](20) NULL,
	[row_count] [bigint] NULL,
	[values_null_row_count] [bigint] NULL,
	[values_empty_row_count] [bigint] NULL,
	[values_distinct_row_count] [bigint] NULL,
	[values_nondistinct_row_count] [bigint] NULL,
	[values_default_row_count] [bigint] NULL,
	[values_unique_values] [bigint] NULL,
	[value_min] [float] NULL,
	[value_max] [float] NULL,
	[value_avg] [float] NULL,
	[value_stddev] [float] NULL,
	[value_var] [float] NULL,
	[value_min_string] [varchar](250) NULL,
	[value_max_string] [varchar](250) NULL,
	[value_min_date] [datetime2](7) NULL,
	[value_max_date] [datetime2](7) NULL,
	[string_length_min] [bigint] NULL,
	[string_length_max] [bigint] NULL,
	[string_length_avg] [float] NULL,
	[string_length_stddev] [float] NULL,
	[string_length_var] [float] NULL,
	[values_list_mode] [char](1) NULL,
	[values_list_rows_count] [bigint] NULL,
	[refresh_at_import] [bit] NOT NULL,
 CONSTRAINT [PK_columns] PRIMARY KEY CLUSTERED 
(
	[column_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [columns] TO [users] AS [dbo]
GO
GRANT INSERT ON [columns] TO [users] AS [dbo]
GO
GRANT SELECT ON [columns] TO [users] AS [dbo]
GO
GRANT UPDATE ON [columns] TO [users] AS [dbo]
GO
/****** Object:  Table [columns_changes]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [columns_changes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[update_id] [int] NULL,
	[column_id] [int] NOT NULL,
	[table_id] [int] NOT NULL,
	[database_id] [int] NULL,
	[table_schema] [nvarchar](250) NULL,
	[table_name] [nvarchar](250) NULL,
	[column_name] [nvarchar](250) NOT NULL,
	[ordinal_position] [int] NULL,
	[datatype] [nvarchar](250) NULL,
	[data_length] [nvarchar](50) NULL,
	[nullable] [bit] NOT NULL,
	[default_value] [nvarchar](max) NULL,
	[is_identity] [bit] NOT NULL,
	[is_computed] [bit] NOT NULL,
	[computed_formula] [nvarchar](max) NULL,
	[before_column_name] [nvarchar](250) NULL,
	[before_ordinal_position] [int] NULL,
	[before_datatype] [nvarchar](250) NULL,
	[before_data_length] [nvarchar](50) NULL,
	[before_nullable] [bit] NULL,
	[before_default_value] [nvarchar](max) NULL,
	[before_is_identity] [bit] NULL,
	[before_is_computed] [bit] NULL,
	[before_computed_formula] [nvarchar](max) NULL,
	[operation] [nvarchar](50) NOT NULL,
	[valid_from] [datetime] NULL,
	[valid_to] [datetime] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[description] [nvarchar](max) NULL,
	[description_date] [datetime] NULL,
	[description_by] [nvarchar](1024) NULL,
	[parent_id] [int] NULL,
	[path] [nvarchar](max) NULL,
	[level] [int] NULL,
	[item_type] [nvarchar](100) NULL,
	[before_parent_id] [int] NULL,
	[before_path] [nvarchar](max) NULL,
	[before_level] [int] NULL,
	[before_item_type] [nvarchar](100) NULL,
 CONSTRAINT [PK_columns_changes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [columns_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [columns_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [columns_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [columns_changes] TO [users] AS [dbo]
GO
/****** Object:  Table [configuration]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [configuration](
	[key] [varchar](100) NOT NULL,
	[value] [varchar](100) NULL,
	[creation_date] [datetime] NULL,
	[created_by] [nvarchar](1024) NULL,
 CONSTRAINT [PK_configuration] PRIMARY KEY CLUSTERED 
(
	[key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [configuration] TO [users] AS [dbo]
GO
GRANT INSERT ON [configuration] TO [users] AS [dbo]
GO
GRANT SELECT ON [configuration] TO [users] AS [dbo]
GO
GRANT UPDATE ON [configuration] TO [users] AS [dbo]
GO
/****** Object:  Table [custom_field_classes]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_field_classes](
	[custom_field_class_id] [int] IDENTITY(1,1) NOT NULL,
	[code] [nvarchar](100) NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[sort] [int] NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_custom_field_classes] PRIMARY KEY CLUSTERED 
(
	[custom_field_class_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [custom_field_classes] TO [users] AS [dbo]
GO
GRANT INSERT ON [custom_field_classes] TO [users] AS [dbo]
GO
GRANT SELECT ON [custom_field_classes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [custom_field_classes] TO [users] AS [dbo]
GO
/****** Object:  Table [custom_fields]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_fields](
	[custom_field_id] [int] IDENTITY(1,1) NOT NULL,
	[ordinal_position] [int] NOT NULL,
	[field_name] [nvarchar](50) NULL,
	[title] [nvarchar](250) NOT NULL,
	[code] [nvarchar](255) NOT NULL,
	[description] [nvarchar](max) NULL,
	[table_visibility] [bit] NOT NULL,
	[procedure_visibility] [bit] NOT NULL,
	[column_visibility] [bit] NOT NULL,
	[relation_visibility] [bit] NOT NULL,
	[key_visibility] [bit] NOT NULL,
	[trigger_visibility] [bit] NOT NULL,
	[parameter_visibility] [bit] NOT NULL,
	[module_visibility] [bit] NOT NULL,
	[documentation_visibility] [bit] NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
	[type] [nvarchar](100) NOT NULL,
	[definition] [nvarchar](max) NULL,
	[term_visibility] [bit] NOT NULL,
	[custom_field_class_id] [int] NOT NULL,
 CONSTRAINT [PK_custom_fields] PRIMARY KEY CLUSTERED 
(
	[custom_field_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_custom_fields_code] UNIQUE NONCLUSTERED 
(
	[code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_custom_fields_field_name] UNIQUE NONCLUSTERED 
(
	[field_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_custom_fields_title] UNIQUE NONCLUSTERED 
(
	[title] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [custom_fields] TO [users] AS [dbo]
GO
GRANT INSERT ON [custom_fields] TO [users] AS [dbo]
GO
GRANT SELECT ON [custom_fields] TO [users] AS [dbo]
GO
GRANT UPDATE ON [custom_fields] TO [users] AS [dbo]
GO
/****** Object:  Table [custom_fields_values]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_fields_values](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[custom_field_id] [int] NOT NULL,
	[object_type] [nvarchar](100) NOT NULL,
	[object_id] [int] NOT NULL,
	[value] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_custom_fields_values] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [custom_fields_values] TO [users] AS [dbo]
GO
GRANT INSERT ON [custom_fields_values] TO [users] AS [dbo]
GO
GRANT SELECT ON [custom_fields_values] TO [users] AS [dbo]
GO
GRANT UPDATE ON [custom_fields_values] TO [users] AS [dbo]
GO
/****** Object:  Table [data_columns_flows]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [data_columns_flows](
	[data_columns_flow_id] [int] IDENTITY(1,1) NOT NULL,
	[inflow_id] [int] NOT NULL,
	[inflow_column_id] [int] NOT NULL,
	[outflow_id] [int] NOT NULL,
	[outflow_column_id] [int] NOT NULL,
	[source] [nvarchar](50) NULL,
	[created_by] [nvarchar](1024) NULL,
	[creation_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
 CONSTRAINT [PK_data_columns_flow_id] PRIMARY KEY CLUSTERED 
(
	[data_columns_flow_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [data_columns_flows] TO [users] AS [dbo]
GO
GRANT INSERT ON [data_columns_flows] TO [users] AS [dbo]
GO
GRANT SELECT ON [data_columns_flows] TO [users] AS [dbo]
GO
GRANT UPDATE ON [data_columns_flows] TO [users] AS [dbo]
GO
/****** Object:  Table [data_flows]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [data_flows](
	[flow_id] [int] IDENTITY(1,1) NOT NULL,
	[process_id] [int] NOT NULL,
	[direction] [nvarchar](10) NOT NULL,
	[object_type] [nvarchar](100) NOT NULL,
	[object_id] [int] NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_data_flows] PRIMARY KEY CLUSTERED 
(
	[flow_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [data_flows] TO [users] AS [dbo]
GO
GRANT INSERT ON [data_flows] TO [users] AS [dbo]
GO
GRANT SELECT ON [data_flows] TO [users] AS [dbo]
GO
GRANT UPDATE ON [data_flows] TO [users] AS [dbo]
GO
/****** Object:  Table [data_processes]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [data_processes](
	[process_id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](250) NOT NULL,
	[processor_type] [nvarchar](100) NOT NULL,
	[processor_id] [int] NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source] [nvarchar](50) NOT NULL,
	[script] [nvarchar](max) NULL,
 CONSTRAINT [PK_data_processes] PRIMARY KEY CLUSTERED 
(
	[process_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [data_processes] TO [users] AS [dbo]
GO
GRANT INSERT ON [data_processes] TO [users] AS [dbo]
GO
GRANT SELECT ON [data_processes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [data_processes] TO [users] AS [dbo]
GO
/****** Object:  Table [database_update_log]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [database_update_log](
	[id] [uniqueidentifier] NOT NULL,
	[version_no] [varchar](10) NULL,
	[change_no] [varchar](25) NULL,
	[executed] [datetime] NULL,
 CONSTRAINT [PK_database_update_log] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [database_update_log] TO [admins] AS [dbo]
GO
GRANT INSERT ON [database_update_log] TO [admins] AS [dbo]
GO
GRANT SELECT ON [database_update_log] TO [admins] AS [dbo]
GO
GRANT UPDATE ON [database_update_log] TO [admins] AS [dbo]
GO
GRANT INSERT ON [database_update_log] TO [users] AS [dbo]
GO
GRANT SELECT ON [database_update_log] TO [users] AS [dbo]
GO
GRANT UPDATE ON [database_update_log] TO [users] AS [dbo]
GO
/****** Object:  Table [databases]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [databases](
	[database_id] [int] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](250) NOT NULL,
	[type] [nvarchar](100) NULL,
	[name] [nvarchar](max) NULL,
	[user] [nvarchar](1024) NULL,
	[password] [nvarchar](max) NULL,
	[windows_authentication] [bit] NULL,
	[different_schema] [bit] NULL,
	[connection_type] [nvarchar](100) NULL,
	[host] [nvarchar](1024) NULL,
	[port] [int] NULL,
	[service_name] [nvarchar](100) NULL,
	[network_alias] [nvarchar](100) NULL,
	[description] [nvarchar](max) NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[description_search] [nvarchar](max) NULL,
	[filter] [nvarchar](max) NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[guid] [uniqueidentifier] NOT NULL,
	[multiple_schemas] [bit] NULL,
	[description_plain] [nvarchar](max) NULL,
	[source_id] [int] NULL,
	[dbms_version] [nvarchar](500) NULL,
	[ssl_key_path] [nvarchar](500) NULL,
	[ssl_cert_path] [nvarchar](500) NULL,
	[ssl_ca_path] [nvarchar](500) NULL,
	[ssl_cipher] [nvarchar](max) NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
	[instance_identifier] [nvarchar](50) NULL,
	[oracle_sid] [nvarchar](50) NULL,
	[show_schema] [bit] NULL,
	[show_schema_override] [bit] NULL,
	[class] [nvarchar](50) NOT NULL,
	[multihost] [nvarchar](max) NULL,
	[authentication_database] [nvarchar](250) NULL,
	[replica_set] [nvarchar](250) NULL,
	[ssl_type] [nvarchar](50) NULL,
	[comments_count] [int] NOT NULL,
	[rating] [decimal](3, 2) NOT NULL,
	[rating_count] [int] NOT NULL,
	[open_warnings_count] [int] NOT NULL,
	[warnings_count] [int] NOT NULL,
	[open_todos_count] [int] NOT NULL,
	[todos_count] [int] NOT NULL,
	[connection_role] [nvarchar](100) NULL,
	[perspective_name] [nvarchar](250) NULL,
	[param1] [nvarchar](max) NULL,
	[param2] [nvarchar](max) NULL,
	[param3] [nvarchar](max) NULL,
	[param4] [nvarchar](max) NULL,
	[param5] [nvarchar](max) NULL,
	[param6] [nvarchar](max) NULL,
	[param7] [nvarchar](max) NULL,
	[param8] [nvarchar](max) NULL,
	[param9] [nvarchar](max) NULL,
	[param10] [nvarchar](max) NULL,
 CONSTRAINT [PK_databases] PRIMARY KEY NONCLUSTERED 
(
	[database_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [databases] TO [users] AS [dbo]
GO
GRANT INSERT ON [databases] TO [users] AS [dbo]
GO
GRANT SELECT ON [databases] TO [users] AS [dbo]
GO
GRANT UPDATE ON [databases] TO [users] AS [dbo]
GO
/****** Object:  Table [datatypes]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [datatypes] TO [users] AS [dbo]
GO
GRANT INSERT ON [datatypes] TO [users] AS [dbo]
GO
GRANT SELECT ON [datatypes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [datatypes] TO [users] AS [dbo]
GO
/****** Object:  Table [dependencies]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dependencies](
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
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
	[temp_sync_status] [bit] NOT NULL,
 CONSTRAINT [PK_dependencies] PRIMARY KEY CLUSTERED 
(
	[dependency_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [dependencies] TO [users] AS [dbo]
GO
GRANT INSERT ON [dependencies] TO [users] AS [dbo]
GO
GRANT SELECT ON [dependencies] TO [users] AS [dbo]
GO
GRANT UPDATE ON [dependencies] TO [users] AS [dbo]
GO
/****** Object:  Table [dependencies_descriptions]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dependencies_descriptions](
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
	[description] [nvarchar](max) NULL,
	[ordinal_position_uses] [int] NULL,
	[ordinal_position_used_by] [int] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_dependencies_desrciptions] PRIMARY KEY CLUSTERED 
(
	[dependency_descriptions_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [dependencies_descriptions] TO [users] AS [dbo]
GO
GRANT INSERT ON [dependencies_descriptions] TO [users] AS [dbo]
GO
GRANT SELECT ON [dependencies_descriptions] TO [users] AS [dbo]
GO
GRANT UPDATE ON [dependencies_descriptions] TO [users] AS [dbo]
GO
/****** Object:  Table [documentation_custom_fields]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [documentation_custom_fields](
	[documentation_custom_field_id] [int] IDENTITY(1,1) NOT NULL,
	[custom_field_id] [int] NOT NULL,
	[database_id] [int] NOT NULL,
	[extended_property] [nvarchar](128) NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_documentation_custom_fields] PRIMARY KEY CLUSTERED 
(
	[documentation_custom_field_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_documentation_custom_fields] UNIQUE NONCLUSTERED 
(
	[custom_field_id] ASC,
	[database_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [documentation_custom_fields] TO [users] AS [dbo]
GO
GRANT INSERT ON [documentation_custom_fields] TO [users] AS [dbo]
GO
GRANT SELECT ON [documentation_custom_fields] TO [users] AS [dbo]
GO
GRANT UPDATE ON [documentation_custom_fields] TO [users] AS [dbo]
GO
/****** Object:  Table [erd_links]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [erd_links](
	[link_id] [int] IDENTITY(1,1) NOT NULL,
	[module_id] [int] NOT NULL,
	[relation_id] [int] NOT NULL,
	[label_pos_x] [int] NULL,
	[label_pos_y] [int] NULL,
	[show_label] [bit] NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[hidden] [bit] NOT NULL,
	[link_style] [nvarchar](25) NOT NULL,
	[show_join_condition] [bit] NOT NULL,
	[source_id] [int] NULL,
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
GRANT DELETE ON [erd_links] TO [users] AS [dbo]
GO
GRANT INSERT ON [erd_links] TO [users] AS [dbo]
GO
GRANT SELECT ON [erd_links] TO [users] AS [dbo]
GO
GRANT UPDATE ON [erd_links] TO [users] AS [dbo]
GO
/****** Object:  Table [erd_nodes]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [erd_nodes](
	[node_id] [int] IDENTITY(1,1) NOT NULL,
	[module_id] [int] NOT NULL,
	[table_id] [int] NOT NULL,
	[pos_x] [int] NULL,
	[pos_y] [int] NULL,
	[color] [char](7) NULL,
	[width] [int] NULL,
	[height] [int] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
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
GRANT DELETE ON [erd_nodes] TO [users] AS [dbo]
GO
GRANT INSERT ON [erd_nodes] TO [users] AS [dbo]
GO
GRANT SELECT ON [erd_nodes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [erd_nodes] TO [users] AS [dbo]
GO
/****** Object:  Table [erd_nodes_columns]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [erd_nodes_columns](
	[node_column_id] [int] IDENTITY(1,1) NOT NULL,
	[module_id] [int] NOT NULL,
	[node_id] [int] NOT NULL,
	[column_id] [int] NOT NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_erd_nodes_columns] PRIMARY KEY CLUSTERED 
(
	[node_column_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [erd_nodes_columns] TO [users] AS [dbo]
GO
GRANT INSERT ON [erd_nodes_columns] TO [users] AS [dbo]
GO
GRANT SELECT ON [erd_nodes_columns] TO [users] AS [dbo]
GO
GRANT UPDATE ON [erd_nodes_columns] TO [users] AS [dbo]
GO
/****** Object:  Table [erd_post_its]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [erd_post_its](
	[post_it_id] [int] IDENTITY(1,1) NOT NULL,
	[module_id] [int] NOT NULL,
	[pos_x] [int] NULL,
	[pos_y] [int] NULL,
	[pos_z] [int] NULL,
	[color] [char](7) NULL,
	[width] [int] NULL,
	[height] [int] NULL,
	[text] [nvarchar](1000) NULL,
	[text_position] [nvarchar](20) NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
 CONSTRAINT [PK_erd_post_its] PRIMARY KEY CLUSTERED 
(
	[post_it_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [erd_post_its] TO [users] AS [dbo]
GO
GRANT INSERT ON [erd_post_its] TO [users] AS [dbo]
GO
GRANT SELECT ON [erd_post_its] TO [users] AS [dbo]
GO
GRANT UPDATE ON [erd_post_its] TO [users] AS [dbo]
GO
/****** Object:  Table [feedback]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [feedback](
	[feedback_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[type] [nvarchar](100) NOT NULL,
	[comment] [nvarchar](max) NULL,
	[rating] [tinyint] NULL,
	[resolved] [bit] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_feedback] PRIMARY KEY CLUSTERED 
(
	[feedback_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [feedback] TO [users] AS [dbo]
GO
GRANT INSERT ON [feedback] TO [users] AS [dbo]
GO
GRANT SELECT ON [feedback] TO [users] AS [dbo]
GO
GRANT UPDATE ON [feedback] TO [users] AS [dbo]
GO
/****** Object:  Table [feedback_comments]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [feedback_comments](
	[comment_id] [int] IDENTITY(1,1) NOT NULL,
	[feedback_id] [int] NOT NULL,
	[user_id] [int] NULL,
	[action] [nvarchar](100) NOT NULL,
	[comment] [nvarchar](max) NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_feedback_comments] PRIMARY KEY CLUSTERED 
(
	[comment_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [feedback_comments] TO [users] AS [dbo]
GO
GRANT INSERT ON [feedback_comments] TO [users] AS [dbo]
GO
GRANT SELECT ON [feedback_comments] TO [users] AS [dbo]
GO
GRANT UPDATE ON [feedback_comments] TO [users] AS [dbo]
GO
/****** Object:  Table [feedback_links]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [feedback_links](
	[link_id] [int] IDENTITY(1,1) NOT NULL,
	[feedback_id] [int] NOT NULL,
	[object_type] [nvarchar](100) NOT NULL,
	[object_id] [int] NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_feedback_links] PRIMARY KEY CLUSTERED 
(
	[link_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [feedback_links] TO [users] AS [dbo]
GO
GRANT INSERT ON [feedback_links] TO [users] AS [dbo]
GO
GRANT SELECT ON [feedback_links] TO [users] AS [dbo]
GO
GRANT UPDATE ON [feedback_links] TO [users] AS [dbo]
GO
/****** Object:  Table [following]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [following](
	[following_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[object_type] [nvarchar](32) NOT NULL,
	[object_id] [int] NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_following] PRIMARY KEY CLUSTERED 
(
	[following_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [following] TO [users] AS [dbo]
GO
GRANT INSERT ON [following] TO [users] AS [dbo]
GO
GRANT SELECT ON [following] TO [users] AS [dbo]
GO
GRANT UPDATE ON [following] TO [users] AS [dbo]
GO
/****** Object:  Table [glossary_mappings]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [glossary_mappings](
	[mapping_id] [int] IDENTITY(1,1) NOT NULL,
	[term_id] [int] NOT NULL,
	[object_type] [nvarchar](100) NULL,
	[object_id] [int] NOT NULL,
	[element_id] [int] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_glossary_mappings] PRIMARY KEY CLUSTERED 
(
	[mapping_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_glossary_mappings] UNIQUE NONCLUSTERED 
(
	[term_id] ASC,
	[object_type] ASC,
	[object_id] ASC,
	[element_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [glossary_mappings] TO [users] AS [dbo]
GO
GRANT INSERT ON [glossary_mappings] TO [users] AS [dbo]
GO
GRANT SELECT ON [glossary_mappings] TO [users] AS [dbo]
GO
GRANT UPDATE ON [glossary_mappings] TO [users] AS [dbo]
GO
/****** Object:  Table [glossary_term_relationship_types]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [glossary_term_relationship_types](
	[type_id] [int] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](250) NOT NULL,
	[title_reverse] [nvarchar](250) NOT NULL,
	[is_symmetrical] [bit] NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
	[sort] [int] NOT NULL,
	[sort_reverse] [int] NOT NULL,
	[code] [nvarchar](250) NULL,
 CONSTRAINT [PK_glossary_term_relationship_types] PRIMARY KEY CLUSTERED 
(
	[type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [glossary_term_relationship_types] TO [users] AS [dbo]
GO
GRANT INSERT ON [glossary_term_relationship_types] TO [users] AS [dbo]
GO
GRANT SELECT ON [glossary_term_relationship_types] TO [users] AS [dbo]
GO
GRANT UPDATE ON [glossary_term_relationship_types] TO [users] AS [dbo]
GO
/****** Object:  Table [glossary_term_relationships]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [glossary_term_relationships](
	[relationship_id] [int] IDENTITY(1,1) NOT NULL,
	[source_term_id] [int] NOT NULL,
	[destination_term_id] [int] NOT NULL,
	[type_id] [int] NOT NULL,
	[description] [nvarchar](max) NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_glossary_term_relationships] PRIMARY KEY CLUSTERED 
(
	[relationship_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT ALTER ON [glossary_term_relationships] TO [users] AS [dbo]
GO
GRANT DELETE ON [glossary_term_relationships] TO [users] AS [dbo]
GO
GRANT INSERT ON [glossary_term_relationships] TO [users] AS [dbo]
GO
GRANT SELECT ON [glossary_term_relationships] TO [users] AS [dbo]
GO
GRANT UPDATE ON [glossary_term_relationships] TO [users] AS [dbo]
GO
/****** Object:  Table [glossary_term_types]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [glossary_term_types](
	[term_type_id] [int] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](250) NOT NULL,
	[icon_id] [int] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
	[code] [nvarchar](250) NULL,
	[list_name] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_glossary_term_types] PRIMARY KEY CLUSTERED 
(
	[term_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [glossary_term_types] TO [users] AS [dbo]
GO
GRANT INSERT ON [glossary_term_types] TO [users] AS [dbo]
GO
GRANT SELECT ON [glossary_term_types] TO [users] AS [dbo]
GO
GRANT UPDATE ON [glossary_term_types] TO [users] AS [dbo]
GO
/****** Object:  Table [glossary_terms]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [glossary_terms](
	[term_id] [int] IDENTITY(1,1) NOT NULL,
	[database_id] [int] NOT NULL,
	[type_id] [int] NOT NULL,
	[parent_id] [int] NULL,
	[title] [nvarchar](250) NOT NULL,
	[description] [nvarchar](max) NULL,
	[description_search] [nvarchar](max) NULL,
	[description_plain] [nvarchar](max) NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
	[comments_count] [int] NOT NULL,
	[rating] [decimal](3, 2) NOT NULL,
	[rating_count] [int] NOT NULL,
	[open_warnings_count] [int] NOT NULL,
	[warnings_count] [int] NOT NULL,
	[open_todos_count] [int] NOT NULL,
	[todos_count] [int] NOT NULL,
 CONSTRAINT [PK_glossary_terms] PRIMARY KEY CLUSTERED 
(
	[term_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [glossary_terms] TO [users] AS [dbo]
GO
GRANT INSERT ON [glossary_terms] TO [users] AS [dbo]
GO
GRANT SELECT ON [glossary_terms] TO [users] AS [dbo]
GO
GRANT UPDATE ON [glossary_terms] TO [users] AS [dbo]
GO
/****** Object:  Table [guid]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [guid](
	[guid] [uniqueidentifier] NOT NULL,
	[is_web_portal_connected] [bit] NULL,
 CONSTRAINT [PK_guid] PRIMARY KEY CLUSTERED 
(
	[guid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [guid] TO [users] AS [dbo]
GO
GRANT INSERT ON [guid] TO [users] AS [dbo]
GO
GRANT SELECT ON [guid] TO [users] AS [dbo]
GO
GRANT UPDATE ON [guid] TO [users] AS [dbo]
GO
/****** Object:  Table [ignored_objects]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ignored_objects](
	[ignored_object_id] [int] IDENTITY(1,1) NOT NULL,
	[database_id] [int] NOT NULL,
	[schema] [nvarchar](250) NULL,
	[name] [nvarchar](250) NOT NULL,
	[object_type] [nvarchar](100) NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_ignored_objects] PRIMARY KEY CLUSTERED 
(
	[ignored_object_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [ignored_objects] TO [users] AS [dbo]
GO
GRANT INSERT ON [ignored_objects] TO [users] AS [dbo]
GO
GRANT SELECT ON [ignored_objects] TO [users] AS [dbo]
GO
GRANT UPDATE ON [ignored_objects] TO [users] AS [dbo]
GO
/****** Object:  Table [import_columns]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [import_columns](
	[import_column_id] [int] IDENTITY(1,1) NOT NULL,
	[database_name] [nvarchar](250) NOT NULL,
	[table_schema] [nvarchar](250) NULL,
	[table_name] [nvarchar](250) NOT NULL,
	[table_object_type] [nvarchar](100) NOT NULL,
	[column_name] [nvarchar](250) NOT NULL,
	[column_path] [nvarchar](250) NULL,
	[column_level] [int] NOT NULL,
	[ordinal_position] [int] NULL,
	[item_type] [nvarchar](100) NOT NULL,
	[datatype] [nvarchar](250) NULL,
	[data_length] [nvarchar](50) NULL,
	[nullable] [bit] NOT NULL,
	[default_value] [nvarchar](max) NULL,
	[is_identity] [bit] NOT NULL,
	[is_computed] [bit] NOT NULL,
	[computed_formula] [nvarchar](max) NULL,
	[description] [nvarchar](max) NULL,
	[creation_date] [datetime] NOT NULL,
	[error_message] [nvarchar](max) NULL,
	[error_failed] [bit] NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
 CONSTRAINT [PK_import_columns] PRIMARY KEY CLUSTERED 
(
	[import_column_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_import_columns] UNIQUE NONCLUSTERED 
(
	[database_name] ASC,
	[table_schema] ASC,
	[table_name] ASC,
	[table_object_type] ASC,
	[column_name] ASC,
	[column_path] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [import_columns] TO [users] AS [dbo]
GO
GRANT INSERT ON [import_columns] TO [users] AS [dbo]
GO
GRANT SELECT ON [import_columns] TO [users] AS [dbo]
GO
GRANT UPDATE ON [import_columns] TO [users] AS [dbo]
GO
/****** Object:  Table [import_errors]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [import_errors](
	[import_error_id] [int] IDENTITY(1,1) NOT NULL,
	[import_table_name] [nvarchar](max) NOT NULL,
	[row_id] [int] NULL,
	[error_message] [nvarchar](max) NULL,
	[error_failed] [bit] NULL,
	[creation_date] [datetime] NOT NULL,
 CONSTRAINT [PK_import_errors] PRIMARY KEY CLUSTERED 
(
	[import_error_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [import_errors] TO [users] AS [dbo]
GO
GRANT INSERT ON [import_errors] TO [users] AS [dbo]
GO
GRANT SELECT ON [import_errors] TO [users] AS [dbo]
GO
GRANT UPDATE ON [import_errors] TO [users] AS [dbo]
GO
/****** Object:  Table [import_procedures]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [import_procedures](
	[import_procedure_id] [int] IDENTITY(1,1) NOT NULL,
	[database_name] [nvarchar](250) NOT NULL,
	[procedure_schema] [nvarchar](250) NULL,
	[procedure_name] [nvarchar](250) NOT NULL,
	[object_type] [nvarchar](100) NOT NULL,
	[object_subtype] [nvarchar](100) NULL,
	[function_type] [nvarchar](100) NULL,
	[language] [nvarchar](100) NULL,
	[definition] [nvarchar](max) NULL,
	[description] [nvarchar](max) NULL,
	[dbms_created] [datetime] NULL,
	[dbms_last_modified] [datetime] NULL,
	[creation_date] [datetime] NOT NULL,
	[error_message] [nvarchar](max) NULL,
	[error_failed] [bit] NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
 CONSTRAINT [PK_import_procedures] PRIMARY KEY CLUSTERED 
(
	[import_procedure_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_import_procedures] UNIQUE NONCLUSTERED 
(
	[database_name] ASC,
	[procedure_schema] ASC,
	[procedure_name] ASC,
	[object_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [import_procedures] TO [users] AS [dbo]
GO
GRANT INSERT ON [import_procedures] TO [users] AS [dbo]
GO
GRANT SELECT ON [import_procedures] TO [users] AS [dbo]
GO
GRANT UPDATE ON [import_procedures] TO [users] AS [dbo]
GO
/****** Object:  Table [import_procedures_parameters]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [import_procedures_parameters](
	[import_procedures_parameter_id] [int] IDENTITY(1,1) NOT NULL,
	[database_name] [nvarchar](250) NOT NULL,
	[procedure_schema] [nvarchar](250) NULL,
	[procedure_name] [nvarchar](250) NOT NULL,
	[procedure_object_type] [nvarchar](100) NOT NULL,
	[parameter_name] [nvarchar](250) NOT NULL,
	[ordinal_position] [int] NULL,
	[parameter_mode] [nvarchar](10) NOT NULL,
	[datatype] [nvarchar](250) NULL,
	[data_length] [nvarchar](20) NULL,
	[description] [nvarchar](max) NULL,
	[creation_date] [datetime] NOT NULL,
	[error_message] [nvarchar](max) NULL,
	[error_failed] [bit] NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
 CONSTRAINT [PK_import_procedures_parameters] PRIMARY KEY CLUSTERED 
(
	[import_procedures_parameter_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_import_procedures_parameters] UNIQUE NONCLUSTERED 
(
	[database_name] ASC,
	[procedure_schema] ASC,
	[procedure_name] ASC,
	[procedure_object_type] ASC,
	[parameter_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [import_procedures_parameters] TO [users] AS [dbo]
GO
GRANT INSERT ON [import_procedures_parameters] TO [users] AS [dbo]
GO
GRANT SELECT ON [import_procedures_parameters] TO [users] AS [dbo]
GO
GRANT UPDATE ON [import_procedures_parameters] TO [users] AS [dbo]
GO
/****** Object:  Table [import_tables]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [import_tables](
	[import_table_id] [int] IDENTITY(1,1) NOT NULL,
	[database_name] [nvarchar](250) NOT NULL,
	[table_schema] [nvarchar](250) NULL,
	[table_name] [nvarchar](250) NOT NULL,
	[object_type] [nvarchar](100) NOT NULL,
	[object_subtype] [nvarchar](100) NULL,
	[description] [nvarchar](max) NULL,
	[definition] [nvarchar](max) NULL,
	[location] [nvarchar](max) NULL,
	[language] [nvarchar](100) NULL,
	[dbms_created] [datetime] NULL,
	[dbms_last_modified] [datetime] NULL,
	[creation_date] [datetime] NOT NULL,
	[error_message] [nvarchar](max) NULL,
	[error_failed] [bit] NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
 CONSTRAINT [PK_import_tables] PRIMARY KEY CLUSTERED 
(
	[import_table_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_import_tables] UNIQUE NONCLUSTERED 
(
	[database_name] ASC,
	[table_schema] ASC,
	[table_name] ASC,
	[object_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [import_tables] TO [users] AS [dbo]
GO
GRANT INSERT ON [import_tables] TO [users] AS [dbo]
GO
GRANT SELECT ON [import_tables] TO [users] AS [dbo]
GO
GRANT UPDATE ON [import_tables] TO [users] AS [dbo]
GO
/****** Object:  Table [import_tables_foreign_keys_columns]    Script Date: 7/5/2022 9:26:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [import_tables_foreign_keys_columns](
	[import_tables_foreign_keys_column_id] [int] IDENTITY(1,1) NOT NULL,
	[database_name] [nvarchar](250) NOT NULL,
	[foreign_table_schema] [nvarchar](250) NULL,
	[foreign_table_name] [nvarchar](250) NOT NULL,
	[foreign_table_object_type] [nvarchar](100) NOT NULL,
	[primary_table_schema] [nvarchar](250) NULL,
	[primary_table_name] [nvarchar](250) NOT NULL,
	[primary_table_object_type] [nvarchar](100) NOT NULL,
	[foreign_column_name] [nvarchar](250) NOT NULL,
	[foreign_column_path] [nvarchar](250) NULL,
	[primary_column_name] [nvarchar](250) NOT NULL,
	[primary_column_path] [nvarchar](250) NULL,
	[column_pair_order] [int] NULL,
	[key_name] [nvarchar](250) NULL,
	[description] [nvarchar](max) NULL,
	[creation_date] [datetime] NOT NULL,
	[error_message] [nvarchar](max) NULL,
	[error_failed] [bit] NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
 CONSTRAINT [PK_import_tables_foreign_keys_columns] PRIMARY KEY CLUSTERED 
(
	[import_tables_foreign_keys_column_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_import_tables_foreign_keys_columns] UNIQUE NONCLUSTERED 
(
	[database_name] ASC,
	[foreign_table_schema] ASC,
	[foreign_table_name] ASC,
	[foreign_table_object_type] ASC,
	[primary_table_schema] ASC,
	[primary_table_name] ASC,
	[primary_table_object_type] ASC,
	[foreign_column_name] ASC,
	[foreign_column_path] ASC,
	[primary_column_name] ASC,
	[primary_column_path] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [import_tables_foreign_keys_columns] TO [users] AS [dbo]
GO
GRANT INSERT ON [import_tables_foreign_keys_columns] TO [users] AS [dbo]
GO
GRANT SELECT ON [import_tables_foreign_keys_columns] TO [users] AS [dbo]
GO
GRANT UPDATE ON [import_tables_foreign_keys_columns] TO [users] AS [dbo]
GO
/****** Object:  Table [import_tables_keys_columns]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [import_tables_keys_columns](
	[import_tables_keys_column_id] [int] IDENTITY(1,1) NOT NULL,
	[database_name] [nvarchar](250) NOT NULL,
	[table_schema] [nvarchar](250) NULL,
	[table_name] [nvarchar](250) NOT NULL,
	[table_object_type] [nvarchar](100) NOT NULL,
	[key_name] [nvarchar](250) NULL,
	[key_type] [nvarchar](10) NOT NULL,
	[description] [nvarchar](max) NULL,
	[disabled] [bit] NOT NULL,
	[column_name] [nvarchar](250) NOT NULL,
	[column_path] [nvarchar](250) NULL,
	[column_order] [int] NULL,
	[creation_date] [datetime] NOT NULL,
	[error_message] [nvarchar](max) NULL,
	[error_failed] [bit] NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
 CONSTRAINT [PK_import_tables_keys_columns] PRIMARY KEY CLUSTERED 
(
	[import_tables_keys_column_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_import_tables_keys_columns] UNIQUE NONCLUSTERED 
(
	[database_name] ASC,
	[table_schema] ASC,
	[table_name] ASC,
	[table_object_type] ASC,
	[key_name] ASC,
	[key_type] ASC,
	[column_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [import_tables_keys_columns] TO [users] AS [dbo]
GO
GRANT INSERT ON [import_tables_keys_columns] TO [users] AS [dbo]
GO
GRANT SELECT ON [import_tables_keys_columns] TO [users] AS [dbo]
GO
GRANT UPDATE ON [import_tables_keys_columns] TO [users] AS [dbo]
GO
/****** Object:  Table [import_triggers]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [import_triggers](
	[import_trigger_id] [int] IDENTITY(1,1) NOT NULL,
	[database_name] [nvarchar](250) NOT NULL,
	[table_schema] [nvarchar](250) NULL,
	[table_name] [nvarchar](250) NOT NULL,
	[table_object_type] [nvarchar](100) NOT NULL,
	[trigger_name] [nvarchar](250) NOT NULL,
	[trigger_type] [nvarchar](100) NOT NULL,
	[definition] [nvarchar](max) NULL,
	[before] [bit] NOT NULL,
	[after] [bit] NOT NULL,
	[instead_of] [bit] NOT NULL,
	[on_insert] [bit] NOT NULL,
	[on_update] [bit] NOT NULL,
	[on_delete] [bit] NOT NULL,
	[disabled] [bit] NOT NULL,
	[description] [nvarchar](max) NULL,
	[creation_date] [datetime] NOT NULL,
	[error_message] [nvarchar](max) NULL,
	[error_failed] [bit] NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
 CONSTRAINT [PK_import_triggers] PRIMARY KEY CLUSTERED 
(
	[import_trigger_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_import_triggers] UNIQUE NONCLUSTERED 
(
	[database_name] ASC,
	[table_schema] ASC,
	[table_name] ASC,
	[table_object_type] ASC,
	[trigger_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [import_triggers] TO [users] AS [dbo]
GO
GRANT INSERT ON [import_triggers] TO [users] AS [dbo]
GO
GRANT SELECT ON [import_triggers] TO [users] AS [dbo]
GO
GRANT UPDATE ON [import_triggers] TO [users] AS [dbo]
GO
/****** Object:  Table [licenses]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [licenses](
	[license_id] [int] IDENTITY(1,1) NOT NULL,
	[login] [nvarchar](1024) NOT NULL,
	[key] [varchar](256) NULL,
	[host] [nvarchar](50) NULL,
	[host2] [nvarchar](50) NULL,
	[host1_last_login] [datetime] NULL,
	[host2_last_login] [datetime] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[is_offline] [bit] NULL,
	[email] [nvarchar](100) NULL,
	[name] [nvarchar](100) NULL,
	[deleted] [bit] NULL,
 CONSTRAINT [PK_licenses] PRIMARY KEY CLUSTERED 
(
	[license_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_licenses_login] UNIQUE NONCLUSTERED 
(
	[login] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [licenses] TO [admins] AS [dbo]
GO
GRANT INSERT ON [licenses] TO [admins] AS [dbo]
GO
GRANT SELECT ON [licenses] TO [admins] AS [dbo]
GO
GRANT UPDATE ON [licenses] TO [admins] AS [dbo]
GO
GRANT INSERT ON [licenses] TO [users] AS [dbo]
GO
GRANT SELECT ON [licenses] TO [users] AS [dbo]
GO
GRANT UPDATE ON [licenses] TO [users] AS [dbo]
GO
/****** Object:  Table [modules]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [modules](
	[module_id] [int] IDENTITY(1,1) NOT NULL,
	[database_id] [int] NOT NULL,
	[ordinal_position] [int] NULL,
	[title] [nvarchar](250) NOT NULL,
	[description] [nvarchar](max) NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[description_search] [nvarchar](max) NULL,
	[erd_link_style] [nvarchar](25) NULL,
	[erd_show_types] [bit] NULL,
	[display_documentation_name_mode] [nvarchar](25) NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[description_plain] [nvarchar](max) NULL,
	[source_id] [int] NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
	[comments_count] [int] NOT NULL,
	[rating] [decimal](3, 2) NOT NULL,
	[rating_count] [int] NOT NULL,
	[open_warnings_count] [int] NOT NULL,
	[warnings_count] [int] NOT NULL,
	[open_todos_count] [int] NOT NULL,
	[todos_count] [int] NOT NULL,
	[erd_show_nullable] [bit] NOT NULL,
 CONSTRAINT [PK_modules] PRIMARY KEY CLUSTERED 
(
	[module_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [modules] TO [users] AS [dbo]
GO
GRANT INSERT ON [modules] TO [users] AS [dbo]
GO
GRANT SELECT ON [modules] TO [users] AS [dbo]
GO
GRANT UPDATE ON [modules] TO [users] AS [dbo]
GO
/****** Object:  Table [parameters]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [parameters](
	[parameter_id] [int] IDENTITY(1,1) NOT NULL,
	[procedure_id] [int] NOT NULL,
	[ordinal_position] [int] NOT NULL,
	[parameter_mode] [nvarchar](10) NOT NULL,
	[name] [nvarchar](250) NOT NULL,
	[description] [nvarchar](max) NULL,
	[datatype] [nvarchar](250) NULL,
	[data_length] [nvarchar](20) NULL,
	[status] [char](1) NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[source_id] [int] NULL,
	[source] [nvarchar](50) NOT NULL,
	[temp_sync_status] [bit] NOT NULL,
	[update_id] [int] NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
	[comments_count] [int] NOT NULL,
	[rating] [decimal](3, 2) NOT NULL,
	[rating_count] [int] NOT NULL,
	[open_warnings_count] [int] NOT NULL,
	[warnings_count] [int] NOT NULL,
	[open_todos_count] [int] NOT NULL,
	[todos_count] [int] NOT NULL,
 CONSTRAINT [PK_parameters] PRIMARY KEY CLUSTERED 
(
	[parameter_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [parameters] TO [users] AS [dbo]
GO
GRANT INSERT ON [parameters] TO [users] AS [dbo]
GO
GRANT SELECT ON [parameters] TO [users] AS [dbo]
GO
GRANT UPDATE ON [parameters] TO [users] AS [dbo]
GO
/****** Object:  Table [parameters_changes]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [parameters_changes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[update_id] [int] NULL,
	[database_id] [int] NOT NULL,
	[parameter_id] [int] NOT NULL,
	[procedure_id] [int] NOT NULL,
	[ordinal_position] [int] NOT NULL,
	[parameter_mode] [nvarchar](10) NOT NULL,
	[name] [nvarchar](250) NOT NULL,
	[datatype] [nvarchar](250) NULL,
	[data_length] [nvarchar](20) NULL,
	[before_ordinal_position] [int] NULL,
	[before_parameter_mode] [nvarchar](10) NULL,
	[before_datatype] [nvarchar](250) NULL,
	[before_data_length] [nvarchar](20) NULL,
	[operation] [nvarchar](50) NOT NULL,
	[valid_from] [datetime] NULL,
	[valid_to] [datetime] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[description] [nvarchar](max) NULL,
	[description_date] [datetime] NULL,
	[description_by] [nvarchar](1024) NULL,
 CONSTRAINT [PK_parameters_changes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [parameters_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [parameters_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [parameters_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [parameters_changes] TO [users] AS [dbo]
GO
/****** Object:  Table [permissions]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [permissions](
	[permission_id] [int] IDENTITY(1,1) NOT NULL,
	[user_type] [nvarchar](15) NOT NULL,
	[user_id] [int] NULL,
	[user_group_id] [int] NULL,
	[object_type] [nvarchar](15) NOT NULL,
	[database_id] [int] NULL,
	[role_id] [int] NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_permissions] PRIMARY KEY CLUSTERED 
(
	[permission_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [permissions] TO [users] AS [dbo]
GO
GRANT INSERT ON [permissions] TO [users] AS [dbo]
GO
GRANT SELECT ON [permissions] TO [users] AS [dbo]
GO
GRANT UPDATE ON [permissions] TO [users] AS [dbo]
GO
/****** Object:  Table [procedures]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [procedures](
	[procedure_id] [int] IDENTITY(1,1) NOT NULL,
	[database_id] [int] NOT NULL,
	[schema] [nvarchar](250) NULL,
	[name] [nvarchar](250) NOT NULL,
	[title] [nvarchar](250) NULL,
	[description] [nvarchar](max) NULL,
	[object_type] [nvarchar](100) NOT NULL,
	[definition] [nvarchar](max) NULL,
	[status] [char](1) NOT NULL,
	[dbms_creation_date] [datetime] NULL,
	[dbms_last_modification_date] [datetime] NULL,
	[synchronization_date] [datetime] NULL,
	[synchronized_by] [nvarchar](100) NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[function_type] [nvarchar](100) NULL,
	[exists_in_DBMS] [bit] NULL,
	[description_search] [nvarchar](max) NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[description_plain] [nvarchar](max) NULL,
	[source_id] [int] NULL,
	[subtype] [nvarchar](100) NULL,
	[language] [nvarchar](100) NULL,
	[source] [nvarchar](50) NOT NULL,
	[temp_sync_status] [bit] NOT NULL,
	[update_id] [int] NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
	[comments_count] [int] NOT NULL,
	[rating] [decimal](3, 2) NOT NULL,
	[rating_count] [int] NOT NULL,
	[open_warnings_count] [int] NOT NULL,
	[warnings_count] [int] NOT NULL,
	[open_todos_count] [int] NOT NULL,
	[todos_count] [int] NOT NULL,
 CONSTRAINT [PK_procedures] PRIMARY KEY CLUSTERED 
(
	[procedure_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [procedures] TO [users] AS [dbo]
GO
GRANT INSERT ON [procedures] TO [users] AS [dbo]
GO
GRANT SELECT ON [procedures] TO [users] AS [dbo]
GO
GRANT UPDATE ON [procedures] TO [users] AS [dbo]
GO
/****** Object:  Table [procedures_changes]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [procedures_changes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[update_id] [int] NULL,
	[procedure_id] [int] NOT NULL,
	[database_id] [int] NOT NULL,
	[schema] [nvarchar](250) NULL,
	[name] [nvarchar](250) NOT NULL,
	[object_type] [nvarchar](100) NOT NULL,
	[subtype] [nvarchar](100) NULL,
	[function_type] [nvarchar](100) NULL,
	[definition] [nvarchar](max) NULL,
	[before_object_type] [nvarchar](100) NULL,
	[before_definition] [nvarchar](max) NULL,
	[before_function_type] [nvarchar](100) NULL,
	[before_subtype] [nvarchar](100) NULL,
	[operation] [nvarchar](50) NOT NULL,
	[dbms_last_modification_date] [datetime] NULL,
	[valid_from] [datetime] NULL,
	[valid_to] [datetime] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[description] [nvarchar](max) NULL,
	[description_date] [datetime] NULL,
	[description_by] [nvarchar](1024) NULL,
 CONSTRAINT [PK_procedures_changes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [procedures_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [procedures_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [procedures_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [procedures_changes] TO [users] AS [dbo]
GO
/****** Object:  Table [procedures_modules]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [procedures_modules](
	[procedure_module_id] [int] IDENTITY(1,1) NOT NULL,
	[procedure_id] [int] NOT NULL,
	[module_id] [int] NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_procedures_modules] PRIMARY KEY CLUSTERED 
(
	[procedure_module_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [procedures_modules] TO [users] AS [dbo]
GO
GRANT INSERT ON [procedures_modules] TO [users] AS [dbo]
GO
GRANT SELECT ON [procedures_modules] TO [users] AS [dbo]
GO
GRANT UPDATE ON [procedures_modules] TO [users] AS [dbo]
GO
/****** Object:  Table [role_actions]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [role_actions](
	[role_action_id] [int] IDENTITY(1,1) NOT NULL,
	[role_id] [int] NOT NULL,
	[action_code] [nvarchar](100) NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_role_actions] PRIMARY KEY CLUSTERED 
(
	[role_action_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [role_actions] TO [users] AS [dbo]
GO
GRANT INSERT ON [role_actions] TO [users] AS [dbo]
GO
GRANT SELECT ON [role_actions] TO [users] AS [dbo]
GO
GRANT UPDATE ON [role_actions] TO [users] AS [dbo]
GO
/****** Object:  Table [roles]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [roles](
	[role_id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](1024) NOT NULL,
	[description] [nvarchar](1024) NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_roles] PRIMARY KEY CLUSTERED 
(
	[role_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [roles] TO [users] AS [dbo]
GO
GRANT INSERT ON [roles] TO [users] AS [dbo]
GO
GRANT SELECT ON [roles] TO [users] AS [dbo]
GO
GRANT UPDATE ON [roles] TO [users] AS [dbo]
GO
/****** Object:  Table [schema_updates]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schema_updates](
	[update_id] [int] IDENTITY(1,1) NOT NULL,
	[type] [nvarchar](100) NOT NULL,
	[datetime] [datetime] NULL,
	[repository_login] [nvarchar](1024) NULL,
	[database_id] [int] NULL,
	[connection_database_type] [nvarchar](100) NULL,
	[connection_host] [nvarchar](1024) NULL,
	[connection_user] [nvarchar](1024) NULL,
	[connection_port] [int] NULL,
	[connection_service_name] [nvarchar](100) NULL,
	[connection_database_name] [nvarchar](max) NULL,
	[connection_dbms_version] [nvarchar](500) NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[description] [nvarchar](max) NULL,
	[description_date] [datetime] NULL,
	[description_by] [nvarchar](1024) NULL,
 CONSTRAINT [PK_schema_updates] PRIMARY KEY CLUSTERED 
(
	[update_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [schema_updates] TO [users] AS [dbo]
GO
GRANT INSERT ON [schema_updates] TO [users] AS [dbo]
GO
GRANT SELECT ON [schema_updates] TO [users] AS [dbo]
GO
GRANT UPDATE ON [schema_updates] TO [users] AS [dbo]
GO
/****** Object:  Table [sessions]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [sessions](
	[session_id] [int] IDENTITY(1,1) NOT NULL,
	[login] [nvarchar](1024) NULL,
	[user_id] [int] NULL,
	[datetime] [datetime] NOT NULL,
	[authentication] [nvarchar](50) NULL,
	[status] [nvarchar](50) NULL,
	[registration_type] [nvarchar](50) NULL,
	[license_type] [nvarchar](50) NULL,
	[remote_license_id] [nvarchar](50) NULL,
	[package_code] [nvarchar](100) NULL,
	[package_name] [nvarchar](100) NULL,
	[license_start] [date] NULL,
	[license_end] [date] NULL,
	[account_id] [int] NULL,
	[account_name] [nvarchar](100) NULL,
	[product] [nvarchar](100) NULL,
	[product_version] [nvarchar](100) NULL,
	[host] [nvarchar](1024) NULL,
	[ip] [nvarchar](50) NULL,
	[user_agent] [nvarchar](max) NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_sessions] PRIMARY KEY CLUSTERED 
(
	[session_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [sessions] TO [users] AS [dbo]
GO
GRANT INSERT ON [sessions] TO [users] AS [dbo]
GO
GRANT SELECT ON [sessions] TO [users] AS [dbo]
GO
GRANT UPDATE ON [sessions] TO [users] AS [dbo]
GO
/****** Object:  Table [tables]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [tables](
	[table_id] [int] IDENTITY(1,1) NOT NULL,
	[database_id] [int] NOT NULL,
	[schema] [nvarchar](250) NULL,
	[name] [nvarchar](250) NOT NULL,
	[title] [nvarchar](250) NULL,
	[description] [nvarchar](max) NULL,
	[object_type] [nvarchar](100) NOT NULL,
	[definition] [nvarchar](max) NULL,
	[status] [char](1) NOT NULL,
	[dbms_creation_date] [datetime] NULL,
	[dbms_last_modification_date] [datetime] NULL,
	[synchronization_date] [datetime] NULL,
	[synchronized_by] [nvarchar](100) NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[exists_in_DBMS] [bit] NULL,
	[description_search] [nvarchar](max) NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[description_plain] [nvarchar](max) NULL,
	[source_id] [int] NULL,
	[source] [nvarchar](50) NOT NULL,
	[subtype] [nvarchar](100) NULL,
	[temp_sync_status] [bit] NOT NULL,
	[update_id] [int] NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
	[location] [nvarchar](max) NULL,
	[language] [nvarchar](100) NOT NULL,
	[comments_count] [int] NOT NULL,
	[rating] [decimal](3, 2) NOT NULL,
	[rating_count] [int] NOT NULL,
	[open_warnings_count] [int] NOT NULL,
	[warnings_count] [int] NOT NULL,
	[open_todos_count] [int] NOT NULL,
	[todos_count] [int] NOT NULL,
	[stats_source] [varchar](10) NULL,
	[stats_row_count] [bigint] NULL,
	[stats_datetime] [datetime] NULL,
 CONSTRAINT [PK_tables] PRIMARY KEY CLUSTERED 
(
	[table_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [tables] TO [users] AS [dbo]
GO
GRANT INSERT ON [tables] TO [users] AS [dbo]
GO
GRANT SELECT ON [tables] TO [users] AS [dbo]
GO
GRANT UPDATE ON [tables] TO [users] AS [dbo]
GO
/****** Object:  Table [tables_changes]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [tables_changes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[update_id] [int] NULL,
	[table_id] [int] NOT NULL,
	[database_id] [int] NOT NULL,
	[schema] [nvarchar](250) NULL,
	[name] [nvarchar](250) NOT NULL,
	[object_type] [nvarchar](100) NOT NULL,
	[subtype] [nvarchar](100) NULL,
	[definition] [nvarchar](max) NULL,
	[before_schema] [nvarchar](250) NULL,
	[before_name] [nvarchar](250) NULL,
	[before_object_type] [nvarchar](100) NULL,
	[before_subtype] [nvarchar](100) NULL,
	[before_definition] [nvarchar](max) NULL,
	[operation] [nvarchar](50) NOT NULL,
	[dbms_last_modification_date] [datetime] NULL,
	[valid_from] [datetime] NULL,
	[valid_to] [datetime] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[description] [nvarchar](max) NULL,
	[description_date] [datetime] NULL,
	[description_by] [nvarchar](1024) NULL,
 CONSTRAINT [PK_tables_changes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [tables_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [tables_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [tables_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [tables_changes] TO [users] AS [dbo]
GO
/****** Object:  Table [tables_modules]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [tables_modules](
	[table_module_id] [int] IDENTITY(1,1) NOT NULL,
	[table_id] [int] NOT NULL,
	[module_id] [int] NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_tables_modules] PRIMARY KEY CLUSTERED 
(
	[table_module_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [tables_modules] TO [users] AS [dbo]
GO
GRANT INSERT ON [tables_modules] TO [users] AS [dbo]
GO
GRANT SELECT ON [tables_modules] TO [users] AS [dbo]
GO
GRANT UPDATE ON [tables_modules] TO [users] AS [dbo]
GO
/****** Object:  Table [tables_relations]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [tables_relations](
	[table_relation_id] [int] IDENTITY(1,1) NOT NULL,
	[pk_table_id] [int] NOT NULL,
	[fk_table_id] [int] NOT NULL,
	[source] [nvarchar](50) NOT NULL,
	[name] [nvarchar](250) NOT NULL,
	[description] [nvarchar](max) NULL,
	[status] [char](1) NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[update_rule] [nvarchar](100) NULL,
	[delete_rule] [nvarchar](100) NULL,
	[disabled] [bit] NULL,
	[title] [nvarchar](250) NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[source_id] [int] NULL,
	[fk_type] [nvarchar](15) NOT NULL,
	[pk_type] [nvarchar](15) NOT NULL,
	[temp_sync_status] [bit] NOT NULL,
	[update_id] [int] NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
 CONSTRAINT [PK_tables_relations] PRIMARY KEY CLUSTERED 
(
	[table_relation_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [tables_relations] TO [users] AS [dbo]
GO
GRANT INSERT ON [tables_relations] TO [users] AS [dbo]
GO
GRANT SELECT ON [tables_relations] TO [users] AS [dbo]
GO
GRANT UPDATE ON [tables_relations] TO [users] AS [dbo]
GO
/****** Object:  Table [tables_relations_changes]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [tables_relations_changes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[update_id] [int] NULL,
	[table_relation_id] [int] NOT NULL,
	[database_id] [int] NOT NULL,
	[pk_table_id] [int] NOT NULL,
	[fk_table_id] [int] NOT NULL,
	[pk_table_schema] [nvarchar](250) NULL,
	[pk_table_name] [nvarchar](250) NULL,
	[fk_table_schema] [nvarchar](250) NULL,
	[fk_table_name] [nvarchar](250) NULL,
	[name] [nvarchar](250) NOT NULL,
	[update_rule] [nvarchar](100) NULL,
	[delete_rule] [nvarchar](100) NULL,
	[disabled] [bit] NULL,
	[fk_type] [nvarchar](15) NOT NULL,
	[pk_type] [nvarchar](15) NOT NULL,
	[before_pk_table_schema] [nvarchar](250) NULL,
	[before_fk_table_schema] [nvarchar](250) NULL,
	[before_pk_table_name] [nvarchar](250) NULL,
	[before_fk_table_name] [nvarchar](250) NULL,
	[before_name] [nvarchar](250) NULL,
	[before_update_rule] [nvarchar](100) NULL,
	[before_delete_rule] [nvarchar](100) NULL,
	[before_disabled] [bit] NULL,
	[before_fk_type] [nvarchar](15) NULL,
	[before_pk_type] [nvarchar](15) NULL,
	[operation] [nvarchar](50) NOT NULL,
	[valid_from] [datetime] NULL,
	[valid_to] [datetime] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[description] [nvarchar](max) NULL,
	[description_date] [datetime] NULL,
	[description_by] [nvarchar](1024) NULL,
 CONSTRAINT [PK_tables_relations_changes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [tables_relations_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [tables_relations_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [tables_relations_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [tables_relations_changes] TO [users] AS [dbo]
GO
/****** Object:  Table [tables_relations_columns]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [tables_relations_columns](
	[table_relation_column_id] [int] IDENTITY(1,1) NOT NULL,
	[table_relation_id] [int] NOT NULL,
	[column_fk_id] [int] NOT NULL,
	[column_pk_id] [int] NOT NULL,
	[ordinal_position] [int] NOT NULL,
	[status] [char](1) NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
	[temp_sync_status] [bit] NOT NULL,
	[update_id] [int] NULL,
 CONSTRAINT [PK_tables_relations_columns] PRIMARY KEY CLUSTERED 
(
	[table_relation_column_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [tables_relations_columns] TO [users] AS [dbo]
GO
GRANT INSERT ON [tables_relations_columns] TO [users] AS [dbo]
GO
GRANT SELECT ON [tables_relations_columns] TO [users] AS [dbo]
GO
GRANT UPDATE ON [tables_relations_columns] TO [users] AS [dbo]
GO
/****** Object:  Table [tables_relations_columns_changes]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [tables_relations_columns_changes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[update_id] [int] NULL,
	[table_relation_column_id] [int] NOT NULL,
	[table_relation_id] [int] NOT NULL,
	[column_fk_id] [int] NOT NULL,
	[column_pk_id] [int] NOT NULL,
	[column_fk_name] [nvarchar](250) NULL,
	[column_pk_name] [nvarchar](250) NULL,
	[ordinal_position] [int] NOT NULL,
	[before_column_fk_id] [int] NULL,
	[before_column_pk_id] [int] NULL,
	[before_column_fk_name] [nvarchar](250) NULL,
	[before_column_pk_name] [nvarchar](250) NULL,
	[before_ordinal_position] [int] NULL,
	[operation] [nvarchar](50) NOT NULL,
	[valid_from] [datetime] NULL,
	[valid_to] [datetime] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
 CONSTRAINT [PK_tables_relations_columns_changes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [tables_relations_columns_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [tables_relations_columns_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [tables_relations_columns_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [tables_relations_columns_changes] TO [users] AS [dbo]
GO
/****** Object:  Table [tables_stats]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [tables_stats](
	[table_stat_id] [int] IDENTITY(1,1) NOT NULL,
	[table_id] [int] NOT NULL,
	[stats_source] [varchar](10) NULL,
	[row_count] [bigint] NULL,
	[stats_datetime] [datetime] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_tables_stats] PRIMARY KEY CLUSTERED 
(
	[table_stat_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [tables_stats] TO [users] AS [dbo]
GO
GRANT INSERT ON [tables_stats] TO [users] AS [dbo]
GO
GRANT SELECT ON [tables_stats] TO [users] AS [dbo]
GO
GRANT UPDATE ON [tables_stats] TO [users] AS [dbo]
GO
/****** Object:  Table [triggers]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [triggers](
	[trigger_id] [int] IDENTITY(1,1) NOT NULL,
	[table_id] [int] NOT NULL,
	[name] [nvarchar](250) NOT NULL,
	[description] [nvarchar](max) NULL,
	[before] [bit] NOT NULL,
	[after] [bit] NOT NULL,
	[instead_of] [bit] NOT NULL,
	[on_insert] [bit] NOT NULL,
	[on_update] [bit] NOT NULL,
	[on_delete] [bit] NOT NULL,
	[disabled] [bit] NOT NULL,
	[definition] [nvarchar](max) NULL,
	[status] [char](1) NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[source_id] [int] NULL,
	[type] [nvarchar](100) NOT NULL,
	[source] [nvarchar](50) NOT NULL,
	[temp_sync_status] [bit] NOT NULL,
	[update_id] [int] NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
	[subtype] [nvarchar](100) NULL,
	[comments_count] [int] NOT NULL,
	[rating] [decimal](3, 2) NOT NULL,
	[rating_count] [int] NOT NULL,
	[open_warnings_count] [int] NOT NULL,
	[warnings_count] [int] NOT NULL,
	[open_todos_count] [int] NOT NULL,
	[todos_count] [int] NOT NULL,
 CONSTRAINT [PK_triggers] PRIMARY KEY CLUSTERED 
(
	[trigger_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [triggers] TO [users] AS [dbo]
GO
GRANT INSERT ON [triggers] TO [users] AS [dbo]
GO
GRANT SELECT ON [triggers] TO [users] AS [dbo]
GO
GRANT UPDATE ON [triggers] TO [users] AS [dbo]
GO
/****** Object:  Table [triggers_changes]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [triggers_changes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[update_id] [int] NULL,
	[trigger_id] [int] NOT NULL,
	[database_id] [int] NOT NULL,
	[table_id] [int] NOT NULL,
	[name] [nvarchar](250) NOT NULL,
	[before] [bit] NOT NULL,
	[after] [bit] NOT NULL,
	[instead_of] [bit] NOT NULL,
	[on_insert] [bit] NOT NULL,
	[on_update] [bit] NOT NULL,
	[on_delete] [bit] NOT NULL,
	[disabled] [bit] NOT NULL,
	[definition] [nvarchar](max) NULL,
	[before_before] [bit] NULL,
	[before_after] [bit] NULL,
	[before_instead_of] [bit] NULL,
	[before_on_insert] [bit] NULL,
	[before_on_update] [bit] NULL,
	[before_on_delete] [bit] NULL,
	[before_disabled] [bit] NULL,
	[before_definition] [nvarchar](max) NULL,
	[type] [nvarchar](100) NOT NULL,
	[operation] [nvarchar](50) NOT NULL,
	[valid_from] [datetime] NULL,
	[valid_to] [datetime] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[description] [nvarchar](max) NULL,
	[description_date] [datetime] NULL,
	[description_by] [nvarchar](1024) NULL,
 CONSTRAINT [PK_triggers_changes2] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [triggers_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [triggers_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [triggers_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [triggers_changes] TO [users] AS [dbo]
GO
/****** Object:  Table [unique_constraints]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [unique_constraints](
	[unique_constraint_id] [int] IDENTITY(1,1) NOT NULL,
	[table_id] [int] NOT NULL,
	[source] [nvarchar](50) NOT NULL,
	[name] [nvarchar](250) NOT NULL,
	[description] [nvarchar](max) NULL,
	[primary_key] [bit] NOT NULL,
	[status] [char](1) NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[disabled] [bit] NULL,
	[field1] [nvarchar](max) NULL,
	[field2] [nvarchar](max) NULL,
	[field3] [nvarchar](max) NULL,
	[field4] [nvarchar](max) NULL,
	[field5] [nvarchar](max) NULL,
	[field6] [nvarchar](max) NULL,
	[field7] [nvarchar](max) NULL,
	[field8] [nvarchar](max) NULL,
	[field9] [nvarchar](max) NULL,
	[field10] [nvarchar](max) NULL,
	[field11] [nvarchar](max) NULL,
	[field12] [nvarchar](max) NULL,
	[field13] [nvarchar](max) NULL,
	[field14] [nvarchar](max) NULL,
	[field15] [nvarchar](max) NULL,
	[field16] [nvarchar](max) NULL,
	[field17] [nvarchar](max) NULL,
	[field18] [nvarchar](max) NULL,
	[field19] [nvarchar](max) NULL,
	[field20] [nvarchar](max) NULL,
	[field21] [nvarchar](max) NULL,
	[field22] [nvarchar](max) NULL,
	[field23] [nvarchar](max) NULL,
	[field24] [nvarchar](max) NULL,
	[field25] [nvarchar](max) NULL,
	[field26] [nvarchar](max) NULL,
	[field27] [nvarchar](max) NULL,
	[field28] [nvarchar](max) NULL,
	[field29] [nvarchar](max) NULL,
	[field30] [nvarchar](max) NULL,
	[field31] [nvarchar](max) NULL,
	[field32] [nvarchar](max) NULL,
	[field33] [nvarchar](max) NULL,
	[field34] [nvarchar](max) NULL,
	[field35] [nvarchar](max) NULL,
	[field36] [nvarchar](max) NULL,
	[field37] [nvarchar](max) NULL,
	[field38] [nvarchar](max) NULL,
	[field39] [nvarchar](max) NULL,
	[field40] [nvarchar](max) NULL,
	[source_id] [int] NULL,
	[temp_sync_status] [bit] NOT NULL,
	[update_id] [int] NULL,
	[field41] [nvarchar](max) NULL,
	[field42] [nvarchar](max) NULL,
	[field43] [nvarchar](max) NULL,
	[field44] [nvarchar](max) NULL,
	[field45] [nvarchar](max) NULL,
	[field46] [nvarchar](max) NULL,
	[field47] [nvarchar](max) NULL,
	[field48] [nvarchar](max) NULL,
	[field49] [nvarchar](max) NULL,
	[field50] [nvarchar](max) NULL,
	[field51] [nvarchar](max) NULL,
	[field52] [nvarchar](max) NULL,
	[field53] [nvarchar](max) NULL,
	[field54] [nvarchar](max) NULL,
	[field55] [nvarchar](max) NULL,
	[field56] [nvarchar](max) NULL,
	[field57] [nvarchar](max) NULL,
	[field58] [nvarchar](max) NULL,
	[field59] [nvarchar](max) NULL,
	[field60] [nvarchar](max) NULL,
	[field61] [nvarchar](max) NULL,
	[field62] [nvarchar](max) NULL,
	[field63] [nvarchar](max) NULL,
	[field64] [nvarchar](max) NULL,
	[field65] [nvarchar](max) NULL,
	[field66] [nvarchar](max) NULL,
	[field67] [nvarchar](max) NULL,
	[field68] [nvarchar](max) NULL,
	[field69] [nvarchar](max) NULL,
	[field70] [nvarchar](max) NULL,
	[field71] [nvarchar](max) NULL,
	[field72] [nvarchar](max) NULL,
	[field73] [nvarchar](max) NULL,
	[field74] [nvarchar](max) NULL,
	[field75] [nvarchar](max) NULL,
	[field76] [nvarchar](max) NULL,
	[field77] [nvarchar](max) NULL,
	[field78] [nvarchar](max) NULL,
	[field79] [nvarchar](max) NULL,
	[field80] [nvarchar](max) NULL,
	[field81] [nvarchar](max) NULL,
	[field82] [nvarchar](max) NULL,
	[field83] [nvarchar](max) NULL,
	[field84] [nvarchar](max) NULL,
	[field85] [nvarchar](max) NULL,
	[field86] [nvarchar](max) NULL,
	[field87] [nvarchar](max) NULL,
	[field88] [nvarchar](max) NULL,
	[field89] [nvarchar](max) NULL,
	[field90] [nvarchar](max) NULL,
	[field91] [nvarchar](max) NULL,
	[field92] [nvarchar](max) NULL,
	[field93] [nvarchar](max) NULL,
	[field94] [nvarchar](max) NULL,
	[field95] [nvarchar](max) NULL,
	[field96] [nvarchar](max) NULL,
	[field97] [nvarchar](max) NULL,
	[field98] [nvarchar](max) NULL,
	[field99] [nvarchar](max) NULL,
	[field100] [nvarchar](max) NULL,
 CONSTRAINT [PK_unique_constraints] PRIMARY KEY CLUSTERED 
(
	[unique_constraint_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [unique_constraints] TO [users] AS [dbo]
GO
GRANT INSERT ON [unique_constraints] TO [users] AS [dbo]
GO
GRANT SELECT ON [unique_constraints] TO [users] AS [dbo]
GO
GRANT UPDATE ON [unique_constraints] TO [users] AS [dbo]
GO
/****** Object:  Table [unique_constraints_changes]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [unique_constraints_changes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[update_id] [int] NULL,
	[unique_constraint_id] [int] NOT NULL,
	[database_id] [int] NOT NULL,
	[table_id] [int] NOT NULL,
	[name] [nvarchar](250) NOT NULL,
	[primary_key] [bit] NOT NULL,
	[disabled] [bit] NULL,
	[before_name] [nvarchar](250) NULL,
	[before_primary_key] [bit] NULL,
	[before_disabled] [bit] NULL,
	[operation] [nvarchar](50) NOT NULL,
	[valid_from] [datetime] NULL,
	[valid_to] [datetime] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[description] [nvarchar](max) NULL,
	[description_date] [datetime] NULL,
	[description_by] [nvarchar](1024) NULL,
 CONSTRAINT [PK_unique_constraints_changes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [unique_constraints_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [unique_constraints_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [unique_constraints_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [unique_constraints_changes] TO [users] AS [dbo]
GO
/****** Object:  Table [unique_constraints_columns]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [unique_constraints_columns](
	[unique_constraint_column_id] [int] IDENTITY(1,1) NOT NULL,
	[unique_constraint_id] [int] NOT NULL,
	[column_id] [int] NOT NULL,
	[ordinal_position] [int] NOT NULL,
	[status] [char](1) NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
	[temp_sync_status] [bit] NOT NULL,
	[update_id] [int] NULL,
 CONSTRAINT [PK_unique_constraints_columns] PRIMARY KEY CLUSTERED 
(
	[unique_constraint_column_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [unique_constraints_columns] TO [users] AS [dbo]
GO
GRANT INSERT ON [unique_constraints_columns] TO [users] AS [dbo]
GO
GRANT SELECT ON [unique_constraints_columns] TO [users] AS [dbo]
GO
GRANT UPDATE ON [unique_constraints_columns] TO [users] AS [dbo]
GO
/****** Object:  Table [unique_constraints_columns_changes]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [unique_constraints_columns_changes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[update_id] [int] NULL,
	[unique_constraint_column_id] [int] NOT NULL,
	[unique_constraint_id] [int] NOT NULL,
	[column_id] [int] NOT NULL,
	[column_name] [nvarchar](250) NULL,
	[before_column_name] [nvarchar](250) NULL,
	[ordinal_position] [int] NOT NULL,
	[before_column_id] [int] NULL,
	[before_ordinal_position] [int] NULL,
	[operation] [nvarchar](50) NOT NULL,
	[valid_from] [datetime] NULL,
	[valid_to] [datetime] NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
 CONSTRAINT [PK_unique_constraints_columns_changes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [unique_constraints_columns_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [unique_constraints_columns_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [unique_constraints_columns_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [unique_constraints_columns_changes] TO [users] AS [dbo]
GO
/****** Object:  Table [user_connections]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [user_connections](
	[connection_id] [int] IDENTITY(1,1) NOT NULL,
	[license_id] [int] NOT NULL,
	[database_id] [int] NOT NULL,
	[database_name] [nvarchar](1024) NULL,
	[user] [nvarchar](1024) NULL,
	[password] [nvarchar](max) NULL,
	[windows_authentication] [bit] NULL,
	[different_schema] [bit] NULL,
	[connection_type] [nvarchar](100) NULL,
	[host] [nvarchar](1024) NULL,
	[port] [int] NULL,
	[service_name] [nvarchar](100) NULL,
	[network_alias] [nvarchar](100) NULL,
	[filter] [nvarchar](max) NULL,
	[multiple_schemas] [bit] NULL,
	[source_id] [int] NULL,
	[ssl_key_path] [nvarchar](500) NULL,
	[ssl_cert_path] [nvarchar](500) NULL,
	[ssl_ca_path] [nvarchar](500) NULL,
	[ssl_cipher] [nvarchar](max) NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[instance_identifier] [nvarchar](50) NULL,
	[oracle_sid] [nvarchar](50) NULL,
	[multihost] [nvarchar](max) NULL,
	[authentication_database] [nvarchar](250) NULL,
	[replica_set] [nvarchar](250) NULL,
	[ssl_type] [nvarchar](50) NULL,
	[connection_role] [nvarchar](100) NULL,
	[perspective_name] [nvarchar](250) NULL,
	[param1] [nvarchar](max) NULL,
	[param2] [nvarchar](max) NULL,
	[param3] [nvarchar](max) NULL,
	[param4] [nvarchar](max) NULL,
	[param5] [nvarchar](max) NULL,
	[param6] [nvarchar](max) NULL,
	[param7] [nvarchar](max) NULL,
	[param8] [nvarchar](max) NULL,
	[param9] [nvarchar](max) NULL,
	[param10] [nvarchar](max) NULL,
 CONSTRAINT [PK_user_connections] PRIMARY KEY CLUSTERED 
(
	[connection_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GRANT DELETE ON [user_connections] TO [users] AS [dbo]
GO
GRANT INSERT ON [user_connections] TO [users] AS [dbo]
GO
GRANT SELECT ON [user_connections] TO [users] AS [dbo]
GO
GRANT UPDATE ON [user_connections] TO [users] AS [dbo]
GO
/****** Object:  Table [user_groups]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [user_groups](
	[user_group_id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](1024) NOT NULL,
	[default] [bit] NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_user_groups] PRIMARY KEY CLUSTERED 
(
	[user_group_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [user_groups] TO [users] AS [dbo]
GO
GRANT INSERT ON [user_groups] TO [users] AS [dbo]
GO
GRANT SELECT ON [user_groups] TO [users] AS [dbo]
GO
GRANT UPDATE ON [user_groups] TO [users] AS [dbo]
GO
/****** Object:  Table [users_user_groups]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [users_user_groups](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[user_group_id] [int] NOT NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[source_id] [int] NULL,
 CONSTRAINT [PK_users_user_groups] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [users_user_groups] TO [users] AS [dbo]
GO
GRANT INSERT ON [users_user_groups] TO [users] AS [dbo]
GO
GRANT SELECT ON [users_user_groups] TO [users] AS [dbo]
GO
GRANT UPDATE ON [users_user_groups] TO [users] AS [dbo]
GO
/****** Object:  Table [version]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [version](
	[version_entry_id] [int] IDENTITY(1,1) NOT NULL,
	[version] [int] NOT NULL,
	[update] [int] NULL,
	[stable] [bit] NULL,
	[installation_date] [datetime] NULL,
	[installed_by] [nvarchar](100) NULL,
	[creation_date] [datetime] NOT NULL,
	[created_by] [nvarchar](1024) NULL,
	[last_modification_date] [datetime] NOT NULL,
	[modified_by] [nvarchar](1024) NULL,
	[release] [int] NULL,
 CONSTRAINT [PK_version] PRIMARY KEY CLUSTERED 
(
	[version_entry_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
GRANT DELETE ON [version] TO [users] AS [dbo]
GO
GRANT INSERT ON [version] TO [users] AS [dbo]
GO
GRANT SELECT ON [version] TO [users] AS [dbo]
GO
GRANT UPDATE ON [version] TO [users] AS [dbo]
GO
/****** Object:  View [data_objects_v]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object: View [data_objects_v] ******/
CREATE VIEW [data_objects_v] AS
	SELECT
		[object_id], 
		[o].[database_id], 
		[object_schema], 
		[object_name], 
		[object_type],
		[d].[name] AS [database_name],
		[d].[host] AS [database_host]
	FROM 
		(SELECT 
			[table_id] as [object_id], 
			[database_id], 
			[schema] AS [object_schema], 
			[name] AS [object_name], 
			[object_type]
		FROM [tables]
		UNION ALL 
		SELECT 
			[procedure_id] as [object_id], 
			[database_id], 
			[schema] AS [object_schema], 
			[name] AS [object_name], 
			[object_type]
		FROM [procedures]
		UNION ALL 
		SELECT 
			[t].[trigger_id] as [object_id], 
			[tab].[database_id], 
			[tab].[schema] AS [object_schema], 
			[t].name AS [object_name], 
			'TRIGGER' AS [object_type]
		FROM [triggers] AS t 
			INNER JOIN [tables] as [tab]
				on [t].[table_id] = [tab].[table_id]) [o]
	INNER JOIN [databases] [d]
		on [o].[database_id] = [d].[database_id]
GO
GRANT SELECT ON [data_objects_v] TO [admins] AS [dbo]
GO
GRANT SELECT ON [data_objects_v] TO [users] AS [dbo]
GO
/****** Object:  View [data_processes_v]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object: View [data_processes_v] ******/
CREATE VIEW [data_processes_v] AS
	SELECT 
		[o].[object_id] AS [processor_id],
		[o].[database_id], 
		[o].[object_schema] AS [processor_schema], 
		[o].[object_name] AS [processor_name],  
		[o].[object_type] AS [processor_type], 
		[p].[process_id], 
		[p].[name] AS [process_name], 
		[p].[source]
	FROM [data_processes] AS [p] 
		INNER JOIN [data_objects_v] AS [o]
			ON [p].[processor_id] = [o].[object_id] 
			AND [p].[processor_type] = [o].[object_type]
GO
GRANT SELECT ON [data_processes_v] TO [admins] AS [dbo]
GO
GRANT SELECT ON [data_processes_v] TO [users] AS [dbo]
GO
/****** Object:  View [data_flows_v]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object: View [data_flows_v] ******/
CREATE VIEW [data_flows_v] AS
	SELECT 
		[f].[flow_id],
		[p].[process_id],
		[p].[processor_id] [processor_id],
		[prc].[object_schema] AS [processor_schema],
		[prc].[object_name] AS [processor_name],
		[prc].[object_type] AS [processor_type],
		[p].[process_name],
		[f].[direction],
		[o].[object_id],
		[o].[object_schema],
		[o].[object_name],
		[o].[object_type],
		[f].[source] AS [flow_source],
		[p].[source] AS [processor_source]
	FROM [data_flows] [f]
	INNER JOIN [data_processes_v] [p]
		ON [f].[process_id] = [p].[process_id]
	INNER JOIN [data_objects_v] [o]
		ON [f].[object_id] = [o].[object_id]
		AND [f].[object_type] = [o].[object_type]
	INNER JOIN [data_objects_v] [prc]
		ON [p].[processor_id] = [prc].[object_id]
		AND [p].[processor_type] = [prc].[object_type]
GO
GRANT SELECT ON [data_flows_v] TO [admins] AS [dbo]
GO
GRANT SELECT ON [data_flows_v] TO [users] AS [dbo]
GO
SET IDENTITY_INSERT [version] ON 
GO
INSERT [version] ([version_entry_id], [version], [update], [stable], [installation_date], [installed_by], [creation_date], [created_by], [last_modification_date], [modified_by], [release]) VALUES (1, 10, 3, 1, getdate(), N'dataedo', getdate(), N'dataedo', getdate(), N'dataedo', 2)
GO
SET IDENTITY_INSERT [version] OFF
GO
INSERT [configuration] ([key], [value], [creation_date], [created_by]) VALUES (N'DATA_PROFILING', N'NO_SAVE', getdate(), N'dataedo')
GO
INSERT [configuration] ([key], [value], [creation_date], [created_by]) VALUES (N'SAVE_HISTORY_OF_CHANGES', N'ENABLED', getdate(), N'dataedo')
GO
