INSERT INTO [version] ([version], [update], [stable]) VALUES (5, 1, 0)
GO

IF DATABASE_PRINCIPAL_ID('admins') IS NULL
BEGIN
    CREATE ROLE [admins]
    GRANT DELETE ON [licenses] TO [admins] AS [dbo]
    GRANT INSERT ON [licenses] TO [admins] AS [dbo]
    GRANT SELECT ON [licenses] TO [admins] AS [dbo]
    GRANT UPDATE ON [licenses] TO [admins] AS [dbo]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (
    SELECT *
    FROM sys.tables
    WHERE name = 'erd_nodes_columns'
	   AND [Type] = N'U'
)
BEGIN
    CREATE TABLE [erd_nodes_columns](
	    [node_column_id] [int] IDENTITY(1,1) NOT NULL,
	    [module_id] [int] NOT NULL,
	    [node_id] [int] NOT NULL,
	    [column_id] [int] NOT NULL,
	CONSTRAINT [PK_erd_nodes_columns] PRIMARY KEY CLUSTERED 
    (
	    [node_column_id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
    
    GRANT DELETE ON [erd_nodes_columns] TO [users] AS [dbo]
    
    GRANT INSERT ON [erd_nodes_columns] TO [users] AS [dbo]
    
    GRANT SELECT ON [erd_nodes_columns] TO [users] AS [dbo]
    
    GRANT UPDATE ON [erd_nodes_columns] TO [users] AS [dbo]
    
    ALTER TABLE [erd_nodes_columns]  WITH CHECK ADD  CONSTRAINT [FK_erd_nodes_columns_columns] FOREIGN KEY([column_id])
    REFERENCES [columns] ([column_id])
    
    ALTER TABLE [erd_nodes_columns] CHECK CONSTRAINT [FK_erd_nodes_columns_columns]
    
    ALTER TABLE [erd_nodes_columns]  WITH CHECK ADD  CONSTRAINT [FK_erd_nodes_columns_modules] FOREIGN KEY([module_id])
    REFERENCES [modules] ([module_id])
    
    ALTER TABLE [erd_nodes_columns] CHECK CONSTRAINT [FK_erd_nodes_columns_modules]
    
    ALTER TABLE [erd_nodes_columns]  WITH CHECK ADD  CONSTRAINT [FK_erd_nodes_columns_erd_nodes] FOREIGN KEY([node_id])
    REFERENCES [erd_nodes] ([node_id])
    
    ALTER TABLE [erd_nodes_columns] CHECK CONSTRAINT [FK_erd_nodes_columns_erd_nodes]
    
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[erd_links]')
	   AND name = 'hidden'
)
BEGIN
    ALTER TABLE [erd_links] ADD [hidden] BIT CONSTRAINT [DF_erd_links_hidden] DEFAULT (0) NOT NULL
END
GO
IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[erd_links]')
	   AND name = 'link_style'
)
BEGIN
    ALTER TABLE [erd_links] ADD [link_style] NVARCHAR(25) CONSTRAINT [DF_erd_links_link_style] DEFAULT ('STRAIGHT') NOT NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'erd_link_style'
)
BEGIN
    ALTER TABLE [dbo].[modules] ADD [erd_link_style] [nvarchar](25) CONSTRAINT [DF_Modules_erd_link_style]  DEFAULT (N'STRAIGHT') NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'erd_show_types'
)
BEGIN
    ALTER TABLE [dbo].[modules] ADD [erd_show_types] [bit] CONSTRAINT [DF_Modules_erd_show_types] DEFAULT (0) NULL;
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[licenses]')
	   AND name = 'user_name'
)
BEGIN
    ALTER TABLE [dbo].[licenses]
    DROP COLUMN [user_name]
END
GO

--columns
ALTER TABLE [columns]
ALTER COLUMN [description] nvarchar(max) NULL

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description2'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description2]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description3'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description3]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description4'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description4]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description5'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description5]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description6'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description6]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description7'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description7]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description8'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description8]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description9'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description9]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description10'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description10]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description11'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description11]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description12'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description12]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description13'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description13]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description14'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description14]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description15'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description15]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description16'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description16]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description17'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description17]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description18'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description18]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description19'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description19]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[columns]')
	   AND name = 'description20'
)
BEGIN
    ALTER TABLE [columns]
    DROP COLUMN [description20]
END
GO

--databases
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[databases]')
	   AND name = 'description_search'
)
BEGIN
    ALTER TABLE [databases]
    ALTER COLUMN [description_search] nvarchar(max) NULL
END
ELSE
BEGIN
    ALTER TABLE [databases]
    ADD [description_search] nvarchar(max) NULL
END
GO

ALTER TABLE [databases]
ALTER COLUMN [description] nvarchar(max) NULL
GO

--dependencies_descriptions
ALTER TABLE [dependencies_descriptions]
ALTER COLUMN [description] nvarchar(max) NULL
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description2'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description2]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description3'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description3]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description4'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description4]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description5'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description5]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description6'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description6]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description7'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description7]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description8'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description8]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description9'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description9]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description10'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description10]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description11'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description11]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description12'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description12]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description13'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description13]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description14'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description14]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description15'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description15]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description16'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description16]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description17'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description17]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description18'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description18]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description19'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description19]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dependencies_descriptions]')
	   AND name = 'description20'
)
BEGIN
    ALTER TABLE [dependencies_descriptions]
    DROP COLUMN [description20]
END
GO

--modules
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description_search'
)
BEGIN
    ALTER TABLE [modules]
    ALTER COLUMN [description_search] nvarchar(max) NULL
END
ELSE
BEGIN
    ALTER TABLE [modules]
    ADD [description_search] nvarchar(max) NULL
END
GO

ALTER TABLE [modules]
ALTER COLUMN [description] nvarchar(max) NULL
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description2'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description2]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description3'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description3]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description4'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description4]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description5'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description5]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description6'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description6]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description7'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description7]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description8'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description8]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description9'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description9]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description10'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description10]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description11'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description11]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description12'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description12]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description13'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description13]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description14'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description14]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description15'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description15]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description16'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description16]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description17'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description17]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description18'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description18]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description19'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description19]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[modules]')
	   AND name = 'description20'
)
BEGIN
    ALTER TABLE [modules]
    DROP COLUMN [description20]
END
GO

--parameters
ALTER TABLE [parameters]
ALTER COLUMN [description] nvarchar(max) NULL
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description2'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description2]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description3'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description3]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description4'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description4]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description5'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description5]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description6'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description6]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description7'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description7]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description8'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description8]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description9'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description9]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description10'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description10]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description11'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description11]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description12'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description12]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description13'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description13]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description14'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description14]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description15'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description15]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description16'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description16]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description17'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description17]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description18'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description18]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description19'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description19]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[parameters]')
	   AND name = 'description20'
)
BEGIN
    ALTER TABLE [parameters]
    DROP COLUMN [description20]
END
GO

--procedures
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description_search'
)
BEGIN
    ALTER TABLE [procedures]
    ALTER COLUMN [description_search] nvarchar(max) NULL
END
ELSE
BEGIN
    ALTER TABLE [procedures]
    ADD [description_search] nvarchar(max) NULL
END
GO

ALTER TABLE [procedures]
ALTER COLUMN [definition] nvarchar(max) NULL
GO

ALTER TABLE [procedures]
ALTER COLUMN [description] nvarchar(max) NULL
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description2'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description2]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description3'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description3]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description4'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description4]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description5'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description5]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description6'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description6]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description7'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description7]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description8'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description8]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description9'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description9]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description10'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description10]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description11'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description11]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description12'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description12]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description13'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description13]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description14'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description14]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description15'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description15]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description16'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description16]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description17'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description17]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description18'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description18]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description19'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description19]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[procedures]')
	   AND name = 'description20'
)
BEGIN
    ALTER TABLE [procedures]
    DROP COLUMN [description20]
END
GO

--tables
IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description_search'
)
BEGIN
    ALTER TABLE [tables]
    ALTER COLUMN [description_search] nvarchar(max) NULL
END
ELSE
BEGIN
    ALTER TABLE [tables]
    ADD [description_search] nvarchar(max) NULL
END
GO

ALTER TABLE [tables]
ALTER COLUMN [definition] nvarchar(max) NULL
GO

ALTER TABLE [tables]
ALTER COLUMN [description] nvarchar(max) NULL
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description2'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description2]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description3'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description3]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description4'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description4]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description5'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description5]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description6'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description6]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description7'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description7]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description8'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description8]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description9'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description9]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description10'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description10]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description11'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description11]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description12'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description12]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description13'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description13]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description14'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description14]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description15'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description15]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description16'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description16]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description17'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description17]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description18'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description18]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description19'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description19]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables]')
	   AND name = 'description20'
)
BEGIN
    ALTER TABLE [tables]
    DROP COLUMN [description20]
END
GO

--tables_relations
ALTER TABLE [tables_relations]
ALTER COLUMN [description] nvarchar(max) NULL
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description2'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description2]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description3'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description3]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description4'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description4]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description5'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description5]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description6'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description6]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description7'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description7]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description8'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description8]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description9'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description9]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description10'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description10]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description11'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description11]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description12'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description12]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description13'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description13]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description14'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description14]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description15'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description15]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description16'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description16]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description17'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description17]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description18'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description18]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description19'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description19]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[tables_relations]')
	   AND name = 'description20'
)
BEGIN
    ALTER TABLE [tables_relations]
    DROP COLUMN [description20]
END
GO

--triggers
ALTER TABLE [triggers]
ALTER COLUMN [description] nvarchar(max) NULL
GO

ALTER TABLE [triggers]
ALTER COLUMN [definition] nvarchar(max) NULL
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description2'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description2]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description3'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description3]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description4'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description4]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description5'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description5]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description6'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description6]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description7'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description7]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description8'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description8]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description9'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description9]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description10'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description10]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description11'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description11]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description12'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description12]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description13'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description13]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description14'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description14]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description15'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description15]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description16'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description16]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description17'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description17]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description18'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description18]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description19'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description19]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
	   AND name = 'description20'
)
BEGIN
    ALTER TABLE [triggers]
    DROP COLUMN [description20]
END
GO

--unique_constraints
ALTER TABLE [unique_constraints]
ALTER COLUMN [description] nvarchar(max) NULL
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description2'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description2]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description3'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description3]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description4'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description4]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description5'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description5]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description6'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description6]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description7'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description7]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description8'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description8]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description9'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description9]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description10'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description10]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description11'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description11]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description12'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description12]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description13'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description13]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description14'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description14]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description15'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description15]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description16'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description16]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description17'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description17]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description18'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description18]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description19'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description19]
END
GO

IF EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[unique_constraints]')
	   AND name = 'description20'
)
BEGIN
    ALTER TABLE [unique_constraints]
    DROP COLUMN [description20]
END
GO

UPDATE [version]
set [stable] = 1
where [version] = 5 and [update] = 1
GO