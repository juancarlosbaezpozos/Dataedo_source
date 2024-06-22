DECLARE @id AS UNIQUEIDENTIFIER = '17a13a89-9066-4bfc-a905-500b6a1b8492'
DECLARE @version as int = 10 
DECLARE @update as int = 2
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-1921'

IF NOT EXISTS (SELECT [id] FROM [database_update_log] WHERE [id] =  @id)
BEGIN

IF (SELECT COUNT(*) FROM [version] WHERE [version] = @version AND [update] = @update AND [release] = @release) = 0
     BEGIN
       INSERT INTO [version] ([version], [update], [release], [stable]) VALUES (@version, @update, @release, 0);
     END
  ELSE
     BEGIN
       UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 0 WHERE [version] = @version AND [update] = @update AND [release] = @release;
     END

	/*-----------------------------------------------------------------------------------------------------------------------------*/
	-- SQL script here. WITOUT key word 'GO'!

	/****** Object: Table [import_tables] ******/
	IF OBJECT_ID(N'import_tables', N'U') IS NULL
	BEGIN
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
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	END

	GRANT DELETE ON [import_tables] TO [users] AS [dbo]
	GRANT INSERT ON [import_tables] TO [users] AS [dbo]
	GRANT SELECT ON [import_tables] TO [users] AS [dbo]
	GRANT UPDATE ON [import_tables] TO [users] AS [dbo]

	IF OBJECT_ID('dbo.[DF_import_tables_object_type]') IS NULL
		ALTER TABLE [import_tables] ADD CONSTRAINT [DF_import_tables_object_type] DEFAULT (N'TABLE') FOR [object_type]
	IF OBJECT_ID('dbo.[DF_import_tables_object_subtype]') IS NULL
		ALTER TABLE [import_tables] ADD CONSTRAINT [DF_import_tables_object_subtype] DEFAULT (N'TABLE') FOR [object_subtype]
	IF OBJECT_ID('dbo.[DF_import_tables_creation_date]') IS NULL
		ALTER TABLE [import_tables] ADD CONSTRAINT [DF_import_tables_creation_date] DEFAULT (getdate()) FOR [creation_date]
	IF OBJECT_ID('dbo.[UK_import_tables]') IS NULL
		ALTER TABLE [import_tables] ADD CONSTRAINT [UK_import_tables] UNIQUE ([database_name], [table_schema], [table_name], [object_type])

	/****** Object: Table [import_columns] ******/
	IF OBJECT_ID(N'import_columns', N'U') IS NULL
	BEGIN
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
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	END

	GRANT DELETE ON [import_columns] TO [users] AS [dbo]
	GRANT INSERT ON [import_columns] TO [users] AS [dbo]
	GRANT SELECT ON [import_columns] TO [users] AS [dbo]
	GRANT UPDATE ON [import_columns] TO [users] AS [dbo]

	IF OBJECT_ID('dbo.[DF_import_columns_table_object_type]') IS NULL
		ALTER TABLE [import_columns] ADD CONSTRAINT [DF_import_columns_table_object_type] DEFAULT (N'TABLE') FOR [table_object_type]
	IF OBJECT_ID('dbo.[DF_import_columns_table_item_type]') IS NULL
		ALTER TABLE [import_columns] ADD CONSTRAINT [DF_import_columns_table_item_type] DEFAULT (N'COLUMN') FOR [item_type]
	IF OBJECT_ID('dbo.[DF_import_columns_table_column_level]') IS NULL
		ALTER TABLE [import_columns] ADD CONSTRAINT [DF_import_columns_table_column_level] DEFAULT (1) FOR [column_level]
	IF OBJECT_ID('dbo.[DF_import_columns_table_is_identity]') IS NULL
		ALTER TABLE [import_columns] ADD CONSTRAINT [DF_import_columns_table_is_identity] DEFAULT (0) FOR [is_identity]
	IF OBJECT_ID('dbo.[DF_import_columns_table_is_computed]') IS NULL
		ALTER TABLE [import_columns] ADD CONSTRAINT [DF_import_columns_table_is_computed] DEFAULT (0) FOR [is_computed]
	IF OBJECT_ID('dbo.[DF_import_columns_creation_date]') IS NULL
		ALTER TABLE [import_columns] ADD CONSTRAINT [DF_import_columns_creation_date] DEFAULT (getdate()) FOR [creation_date]
	IF OBJECT_ID('dbo.[UK_import_columns]') IS NULL
		ALTER TABLE [import_columns] ADD CONSTRAINT [UK_import_columns] UNIQUE ([database_name], [table_schema], [table_name], [table_object_type], [column_name], [column_path])

	/****** Object: Table [import_tables_keys_columns] ******/
	IF OBJECT_ID(N'import_tables_keys_columns', N'U') IS NULL
	BEGIN
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
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	END

	GRANT DELETE ON [import_tables_keys_columns] TO [users] AS [dbo]
	GRANT INSERT ON [import_tables_keys_columns] TO [users] AS [dbo]
	GRANT SELECT ON [import_tables_keys_columns] TO [users] AS [dbo]
	GRANT UPDATE ON [import_tables_keys_columns] TO [users] AS [dbo]

	IF OBJECT_ID('dbo.[DF_import_tables_keys_columns_table_object_type]') IS NULL
		ALTER TABLE [import_tables_keys_columns] ADD CONSTRAINT [DF_import_tables_keys_columns_table_object_type] DEFAULT (N'TABLE') FOR [table_object_type]
	IF OBJECT_ID('dbo.[DF_import_tables_keys_columns_key_type]') IS NULL
		ALTER TABLE [import_tables_keys_columns] ADD CONSTRAINT [DF_import_tables_keys_columns_key_type] DEFAULT (N'PK') FOR [key_type]
	IF OBJECT_ID('dbo.[DF_import_tables_keys_disabled]') IS NULL
		ALTER TABLE [import_tables_keys_columns] ADD CONSTRAINT [DF_import_tables_keys_disabled] DEFAULT (0) FOR [disabled]
	IF OBJECT_ID('dbo.[DF_import_tables_keys_column_order]') IS NULL
		ALTER TABLE [import_tables_keys_columns] ADD CONSTRAINT [DF_import_tables_keys_column_order] DEFAULT (1) FOR [column_order]
	IF OBJECT_ID('dbo.[DF_import_tables_keys_columns_creation_date]') IS NULL
		ALTER TABLE [import_tables_keys_columns] ADD CONSTRAINT [DF_import_tables_keys_columns_creation_date] DEFAULT (getdate()) FOR [creation_date]
	IF OBJECT_ID('dbo.[UK_import_tables_keys_columns]') IS NULL
		ALTER TABLE [import_tables_keys_columns] ADD CONSTRAINT [UK_import_tables_keys_columns] UNIQUE ([database_name], [table_schema], [table_name], [table_object_type], [key_name], [key_type], [column_name])

	/****** Object: Table [import_tables_foreign_keys_columns] ******/
	IF OBJECT_ID(N'import_tables_foreign_keys_columns', N'U') IS NULL
	BEGIN
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
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	END

	GRANT DELETE ON [import_tables_foreign_keys_columns] TO [users] AS [dbo]
	GRANT INSERT ON [import_tables_foreign_keys_columns] TO [users] AS [dbo]
	GRANT SELECT ON [import_tables_foreign_keys_columns] TO [users] AS [dbo]
	GRANT UPDATE ON [import_tables_foreign_keys_columns] TO [users] AS [dbo]

	IF OBJECT_ID('dbo.[DF_import_tables_foreign_keys_columns_foreign_table_object_type]') IS NULL
		ALTER TABLE [import_tables_foreign_keys_columns] ADD CONSTRAINT [DF_import_tables_foreign_keys_columns_foreign_table_object_type] DEFAULT (N'TABLE') FOR [foreign_table_object_type]
	IF OBJECT_ID('dbo.[DF_import_tables_foreign_keys_columns_primary_table_object_type]') IS NULL
		ALTER TABLE [import_tables_foreign_keys_columns] ADD CONSTRAINT [DF_import_tables_foreign_keys_columns_primary_table_object_type] DEFAULT (N'TABLE') FOR [primary_table_object_type]
	IF OBJECT_ID('dbo.[DF_import_tables_foreign_keys_columns_column_pair_order]') IS NULL
		ALTER TABLE [import_tables_foreign_keys_columns] ADD CONSTRAINT [DF_import_tables_foreign_keys_columns_column_pair_order] DEFAULT (1) FOR [column_pair_order]
	IF OBJECT_ID('dbo.[DF_import_tables_foreign_keys_columns_creation_date]') IS NULL
		ALTER TABLE [import_tables_foreign_keys_columns] ADD CONSTRAINT [DF_import_tables_foreign_keys_columns_creation_date] DEFAULT (getdate()) FOR [creation_date]
	IF OBJECT_ID('dbo.[UK_import_tables_foreign_keys_columns]') IS NULL
		ALTER TABLE [import_tables_foreign_keys_columns] ADD CONSTRAINT [UK_import_tables_foreign_keys_columns] 
		UNIQUE 
			([database_name], 
			[foreign_table_schema],
			[foreign_table_name], 
			[foreign_table_object_type],
			[primary_table_schema],
			[primary_table_name],
			[primary_table_object_type],
			[foreign_column_name], 
			[foreign_column_path], 
			[primary_column_name], 
			[primary_column_path])

	/****** Object: Table [import_procedures] ******/
	IF OBJECT_ID(N'import_procedures', N'U') IS NULL
	BEGIN
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
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	END

	GRANT DELETE ON [import_procedures] TO [users] AS [dbo]
	GRANT INSERT ON [import_procedures] TO [users] AS [dbo]
	GRANT SELECT ON [import_procedures] TO [users] AS [dbo]
	GRANT UPDATE ON [import_procedures] TO [users] AS [dbo]

	IF OBJECT_ID('dbo.[DF_import_procedures_object_type]') IS NULL
		ALTER TABLE [import_procedures] ADD CONSTRAINT [DF_import_procedures_object_type] DEFAULT (N'PROCEDURE') FOR [object_type]
	IF OBJECT_ID('dbo.[DF_import_procedures_object_subtype]') IS NULL
		ALTER TABLE [import_procedures] ADD CONSTRAINT [DF_import_procedures_object_subtype] DEFAULT (N'PROCEDURE') FOR [object_subtype]
	IF OBJECT_ID('dbo.[DF_import_procedures_creation_date]') IS NULL
		ALTER TABLE [import_procedures] ADD CONSTRAINT [DF_import_procedures_creation_date] DEFAULT (getdate()) FOR [creation_date]
	IF OBJECT_ID('dbo.[UK_import_procedures]') IS NULL
		ALTER TABLE [import_procedures] ADD CONSTRAINT [UK_import_procedures] UNIQUE ([database_name], [procedure_schema], [procedure_name], [object_type])

	/****** Object: Table [import_procedures_parameters] ******/
	IF OBJECT_ID(N'import_procedures_parameters', N'U') IS NULL
	BEGIN
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
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	END

	GRANT DELETE ON [import_procedures_parameters] TO [users] AS [dbo]
	GRANT INSERT ON [import_procedures_parameters] TO [users] AS [dbo]
	GRANT SELECT ON [import_procedures_parameters] TO [users] AS [dbo]
	GRANT UPDATE ON [import_procedures_parameters] TO [users] AS [dbo]

	IF OBJECT_ID('dbo.[DF_import_procedures_procedure_object_type]') IS NULL
		ALTER TABLE [import_procedures_parameters] ADD CONSTRAINT [DF_import_procedures_procedure_object_type] DEFAULT (N'PROCEDURE') FOR [procedure_object_type]
	IF OBJECT_ID('dbo.[DF_import_procedures_parameter_mode]') IS NULL
		ALTER TABLE [import_procedures_parameters] ADD CONSTRAINT [DF_import_procedures_parameter_mode] DEFAULT (N'IN') FOR [parameter_mode]
	IF OBJECT_ID('dbo.[DF_import_procedures_parameters_creation_date]') IS NULL
		ALTER TABLE [import_procedures_parameters] ADD CONSTRAINT [DF_import_procedures_parameters_creation_date] DEFAULT (getdate()) FOR [creation_date]
	IF OBJECT_ID('dbo.[UK_import_procedures_parameters]') IS NULL
		ALTER TABLE [import_procedures_parameters] ADD CONSTRAINT [UK_import_procedures_parameters] UNIQUE ([database_name], [procedure_schema], [procedure_name], [procedure_object_type], [parameter_name])

	/****** Object: Table [import_triggers] ******/
	IF OBJECT_ID(N'import_triggers', N'U') IS NULL
	BEGIN
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
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	END

	GRANT DELETE ON [import_triggers] TO [users] AS [dbo]
	GRANT INSERT ON [import_triggers] TO [users] AS [dbo]
	GRANT SELECT ON [import_triggers] TO [users] AS [dbo]
	GRANT UPDATE ON [import_triggers] TO [users] AS [dbo]

	IF OBJECT_ID('dbo.[DF_import_triggers_table_object_type]') IS NULL
		ALTER TABLE [import_triggers] ADD CONSTRAINT [DF_import_triggers_table_object_type] DEFAULT (N'TABLE') FOR [table_object_type]
	IF OBJECT_ID('dbo.[DF_import_triggers_trigger_type]') IS NULL
		ALTER TABLE [import_triggers] ADD CONSTRAINT [DF_import_triggers_trigger_type] DEFAULT (N'TRIGGER') FOR [trigger_type]
	IF OBJECT_ID('dbo.[DF_import_triggers_before]') IS NULL
		ALTER TABLE [import_triggers] ADD CONSTRAINT [DF_import_triggers_before] DEFAULT (0) FOR [before]
	IF OBJECT_ID('dbo.[DF_import_triggers_after]') IS NULL
		ALTER TABLE [import_triggers] ADD CONSTRAINT [DF_import_triggers_after] DEFAULT (0) FOR [after]
	IF OBJECT_ID('dbo.[DF_import_triggers_instead_of]') IS NULL
		ALTER TABLE [import_triggers] ADD CONSTRAINT [DF_import_triggers_instead_of] DEFAULT (0) FOR [instead_of]
	IF OBJECT_ID('dbo.[DF_import_triggers_on_insert]') IS NULL
		ALTER TABLE [import_triggers] ADD CONSTRAINT [DF_import_triggers_on_insert] DEFAULT (0) FOR [on_insert]
	IF OBJECT_ID('dbo.[DF_import_triggers_on_update]') IS NULL
		ALTER TABLE [import_triggers] ADD CONSTRAINT [DF_import_triggers_on_update] DEFAULT (0) FOR [on_update]
	IF OBJECT_ID('dbo.[DF_import_triggers_on_delete]') IS NULL
		ALTER TABLE [import_triggers] ADD CONSTRAINT [DF_import_triggers_on_delete] DEFAULT (0) FOR [on_delete]
	IF OBJECT_ID('dbo.[DF_import_triggers_disabled]') IS NULL
		ALTER TABLE [import_triggers] ADD CONSTRAINT [DF_import_triggers_disabled] DEFAULT (0) FOR [disabled]
	IF OBJECT_ID('dbo.[DF_import_triggers_parameters_creation_date]') IS NULL
		ALTER TABLE [import_triggers] ADD CONSTRAINT [DF_import_triggers_parameters_creation_date] DEFAULT (getdate()) FOR [creation_date]
	IF OBJECT_ID('dbo.[UK_import_triggers]') IS NULL
		ALTER TABLE [import_triggers] ADD CONSTRAINT [UK_import_triggers] UNIQUE ([database_name], [table_schema], [table_name], [table_object_type], [trigger_name])

	/****** Object: Table [import_errors] ******/
	IF OBJECT_ID(N'import_errors', N'U') IS NULL
	BEGIN
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
	END

	GRANT DELETE ON [import_errors] TO [users] AS [dbo]
	GRANT INSERT ON [import_errors] TO [users] AS [dbo]
	GRANT SELECT ON [import_errors] TO [users] AS [dbo]
	GRANT UPDATE ON [import_errors] TO [users] AS [dbo]

	IF OBJECT_ID('dbo.[DF_import_errors_creation_date]') IS NULL
		ALTER TABLE [import_errors] ADD CONSTRAINT [DF_import_errors_creation_date] DEFAULT (getdate()) FOR [creation_date]

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_database_name' 
		AND object_id = OBJECT_ID('dbo.[import_tables]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_database_name]
		ON [import_tables] ([database_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_table_schema' 
		AND object_id = OBJECT_ID('dbo.[import_tables]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_table_schema]
		ON [import_tables] ([table_schema] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_table_name' 
		AND object_id = OBJECT_ID('dbo.[import_tables]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_table_name]
		ON [import_tables] ([table_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_object_type' 
		AND object_id = OBJECT_ID('dbo.[import_tables]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_object_type]
		ON [import_tables] ([object_type] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_object_subtype' 
		AND object_id = OBJECT_ID('dbo.[import_tables]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_object_subtype]
		ON [import_tables] ([object_subtype] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_error_failed' 
		AND object_id = OBJECT_ID('dbo.[import_tables]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_error_failed]
		ON [import_tables] ([error_failed] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_procedures_database_name' 
		AND object_id = OBJECT_ID('dbo.[import_procedures]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_procedures_database_name]
		ON [import_procedures] ([database_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_procedures_procedure_schema' 
		AND object_id = OBJECT_ID('dbo.[import_procedures]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_procedures_procedure_schema]
		ON [import_procedures] ([procedure_schema] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_procedures_procedure_name' 
		AND object_id = OBJECT_ID('dbo.[import_procedures]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_procedures_procedure_name]
		ON [import_procedures] ([procedure_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_procedures_object_type' 
		AND object_id = OBJECT_ID('dbo.[import_procedures]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_procedures_object_type]
		ON [import_procedures] ([object_type] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_procedures_object_subtype' 
		AND object_id = OBJECT_ID('dbo.[import_procedures]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_procedures_object_subtype]
		ON [import_procedures] ([object_subtype] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_procedures_error_failed' 
		AND object_id = OBJECT_ID('dbo.[import_procedures]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_procedures_error_failed]
		ON [import_procedures] ([error_failed] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_foreign_keys_columns_database_name' 
		AND object_id = OBJECT_ID('dbo.[import_tables_foreign_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_database_name]
		ON [import_tables_foreign_keys_columns] ([database_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_foreign_keys_columns_foreign_table_schema' 
		AND object_id = OBJECT_ID('dbo.[import_tables_foreign_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_foreign_table_schema]
		ON [import_tables_foreign_keys_columns] ([foreign_table_schema] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_foreign_keys_columns_foreign_table_name' 
		AND object_id = OBJECT_ID('dbo.[import_tables_foreign_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_foreign_table_name]
		ON [import_tables_foreign_keys_columns] ([foreign_table_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_foreign_keys_columns_foreign_table_object_type' 
		AND object_id = OBJECT_ID('dbo.[import_tables_foreign_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_foreign_table_object_type]
		ON [import_tables_foreign_keys_columns] ([foreign_table_object_type] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_foreign_keys_columns_foreign_column_name' 
		AND object_id = OBJECT_ID('dbo.[import_tables_foreign_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_foreign_column_name]
		ON [import_tables_foreign_keys_columns] ([foreign_column_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_foreign_keys_columns_foreign_column_path' 
		AND object_id = OBJECT_ID('dbo.[import_tables_foreign_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_foreign_column_path]
		ON [import_tables_foreign_keys_columns] ([foreign_column_path] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_foreign_keys_columns_primary_table_schema' 
		AND object_id = OBJECT_ID('dbo.[import_tables_foreign_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_primary_table_schema]
		ON [import_tables_foreign_keys_columns] ([primary_table_schema] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_foreign_keys_columns_primary_table_name' 
		AND object_id = OBJECT_ID('dbo.[import_tables_foreign_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_primary_table_name]
		ON [import_tables_foreign_keys_columns] ([primary_table_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_foreign_keys_columns_primary_table_object_type' 
		AND object_id = OBJECT_ID('dbo.[import_tables_foreign_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_primary_table_object_type]
		ON [import_tables_foreign_keys_columns] ([primary_table_object_type] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_foreign_keys_columns_primary_column_name' 
		AND object_id = OBJECT_ID('dbo.[import_tables_foreign_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_primary_column_name]
		ON [import_tables_foreign_keys_columns] ([primary_column_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_foreign_keys_columns_primary_column_path' 
		AND object_id = OBJECT_ID('dbo.[import_tables_foreign_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_primary_column_path]
		ON [import_tables_foreign_keys_columns] ([primary_column_path] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_foreign_keys_columns_error_failed' 
		AND object_id = OBJECT_ID('dbo.[import_tables_foreign_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_error_failed]
		ON [import_tables_foreign_keys_columns] ([error_failed] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_columns_database_name' 
		AND object_id = OBJECT_ID('dbo.[import_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_columns_database_name]
		ON [import_columns] ([database_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_columns_table_schema' 
		AND object_id = OBJECT_ID('dbo.[import_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_columns_table_schema]
		ON [import_columns] ([table_schema] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_columns_table_name' 
		AND object_id = OBJECT_ID('dbo.[import_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_columns_table_name]
		ON [import_columns] ([table_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_columns_column_name' 
		AND object_id = OBJECT_ID('dbo.[import_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_columns_column_name]
		ON [import_columns] ([column_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_columns_column_path' 
		AND object_id = OBJECT_ID('dbo.[import_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_columns_column_path]
		ON [import_columns] ([column_path] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_columns_table_object_type' 
		AND object_id = OBJECT_ID('dbo.[import_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_columns_table_object_type]
		ON [import_columns] ([table_object_type] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_columns_column_level' 
		AND object_id = OBJECT_ID('dbo.[import_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_columns_column_level]
		ON [import_columns] ([column_level] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_columns_item_type' 
		AND object_id = OBJECT_ID('dbo.[import_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_columns_item_type]
		ON [import_columns] ([item_type] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_columns_error_failed' 
		AND object_id = OBJECT_ID('dbo.[import_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_columns_error_failed]
		ON [import_columns] ([error_failed] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_triggers_database_name' 
		AND object_id = OBJECT_ID('dbo.[import_triggers]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_triggers_database_name]
		ON [import_triggers] ([database_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_triggers_table_schema' 
		AND object_id = OBJECT_ID('dbo.[import_triggers]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_triggers_table_schema]
		ON [import_triggers] ([table_schema] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_triggers_table_name' 
		AND object_id = OBJECT_ID('dbo.[import_triggers]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_triggers_table_name]
		ON [import_triggers] ([table_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_triggers_table_object_type' 
		AND object_id = OBJECT_ID('dbo.[import_triggers]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_triggers_table_object_type]
		ON [import_triggers] ([table_object_type] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_triggers_trigger_type' 
		AND object_id = OBJECT_ID('dbo.[import_triggers]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_triggers_trigger_type]
		ON [import_triggers] ([trigger_type] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_triggers_error_failed' 
		AND object_id = OBJECT_ID('dbo.[import_triggers]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_triggers_error_failed]
		ON [import_triggers] ([error_failed] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_keys_columns_database_name' 
		AND object_id = OBJECT_ID('dbo.[import_tables_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_keys_columns_database_name]
		ON [import_tables_keys_columns] ([database_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_keys_columns_table_schema' 
		AND object_id = OBJECT_ID('dbo.[import_tables_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_keys_columns_table_schema]
		ON [import_tables_keys_columns] ([table_schema] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_keys_columns_table_name' 
		AND object_id = OBJECT_ID('dbo.[import_tables_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_keys_columns_table_name]
		ON [import_tables_keys_columns] ([table_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_keys_columns_table_object_type' 
		AND object_id = OBJECT_ID('dbo.[import_tables_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_keys_columns_table_object_type]
		ON [import_tables_keys_columns] ([table_object_type] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_keys_columns_column_name' 
		AND object_id = OBJECT_ID('dbo.[import_tables_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_keys_columns_column_name]
		ON [import_tables_keys_columns] ([column_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_keys_columns_column_path' 
		AND object_id = OBJECT_ID('dbo.[import_tables_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_keys_columns_column_path]
		ON [import_tables_keys_columns] ([column_path] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_keys_columns_key_type' 
		AND object_id = OBJECT_ID('dbo.[import_tables_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_keys_columns_key_type]
		ON [import_tables_keys_columns] ([key_type] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_tables_keys_columns_error_failed' 
		AND object_id = OBJECT_ID('dbo.[import_tables_keys_columns]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_tables_keys_columns_error_failed]
		ON [import_tables_keys_columns] ([error_failed] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_procedures_parameters_database_name' 
		AND object_id = OBJECT_ID('dbo.[import_procedures_parameters]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_procedures_parameters_database_name]
		ON [import_procedures_parameters] ([database_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_procedures_parameters_procedure_schema' 
		AND object_id = OBJECT_ID('dbo.[import_procedures_parameters]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_procedures_parameters_procedure_schema]
		ON [import_procedures_parameters] ([procedure_schema] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_procedures_parameters_procedure_name' 
		AND object_id = OBJECT_ID('dbo.[import_procedures_parameters]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_procedures_parameters_procedure_name]
		ON [import_procedures_parameters] ([procedure_name] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_procedures_parameters_procedure_object_type' 
		AND object_id = OBJECT_ID('dbo.[import_procedures_parameters]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_procedures_parameters_procedure_object_type]
		ON [import_procedures_parameters] ([procedure_object_type] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_procedures_parameters_parameter_mode' 
		AND object_id = OBJECT_ID('dbo.[import_procedures_parameters]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_procedures_parameters_parameter_mode]
		ON [import_procedures_parameters] ([parameter_mode] ASC)
	END

	IF NOT EXISTS(
		SELECT 1 FROM sys.indexes 
		WHERE name='IX_import_procedures_parameters_error_failed' 
		AND object_id = OBJECT_ID('dbo.[import_procedures_parameters]'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_import_procedures_parameters_error_failed]
		ON [import_procedures_parameters] ([error_failed] ASC)
	END

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO
