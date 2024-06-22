DECLARE @id AS UNIQUEIDENTIFIER = 'e89ff38f-b36c-4fc2-a076-d38f054b97da'
DECLARE @version as int = 10
DECLARE @update as int = 2
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-197'

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
  
  if OBJECT_ID(N'changes_history',N'U') IS NULL
  BEGIN
	  CREATE TABLE [dbo].[changes_history](
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
						
	  ALTER TABLE [dbo].[changes_history] ADD  CONSTRAINT [DF_changes_history_datetime]  DEFAULT (getdate()) FOR [datetime]
	  ALTER TABLE [dbo].[changes_history] ADD  CONSTRAINT [DF_changes_history_creation_date]  DEFAULT (getdate()) FOR [creation_date]

	  GRANT DELETE ON [changes_history] TO [admins] AS [dbo]
	  GRANT INSERT ON [changes_history] TO [admins] AS [dbo]
	  GRANT SELECT ON [changes_history] TO [admins] AS [dbo]
	  GRANT UPDATE ON [changes_history] TO [admins] AS [dbo]
	  
	  GRANT INSERT ON [changes_history] TO [users] AS [dbo]
	  GRANT SELECT ON [changes_history] TO [users] AS [dbo]
	  GRANT UPDATE ON [changes_history] TO [users] AS [dbo]
	
END;

IF OBJECT_ID (N'configuration',N'U') IS NOT NULL
BEGIN
	IF 1 != (SELECT COUNT(1) FROM [configuration] WHERE [key] = N'SAVE_HISTORY_OF_CHANGES')
	BEGIN
		INSERT [configuration] ([key], [value], [creation_date], [created_by]) VALUES (N'SAVE_HISTORY_OF_CHANGES', N'ENABLED', getdate(), N'dataedo')
	END
END

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO
