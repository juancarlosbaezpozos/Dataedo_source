DECLARE @id AS UNIQUEIDENTIFIER = '1192113a-64b5-4735-ab1e-f95c5690c487'
DECLARE @version as int = 10
DECLARE @update as int = 2
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-2587'

IF OBJECT_ID(N'database_update_log', N'U') IS NULL
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

  CREATE TABLE [dbo].[database_update_log](
    [id] [uniqueidentifier] NOT NULL,
    [version_no] [varchar](10) NULL,
    [change_no] [varchar](25) NULL,
    [executed] [datetime] NULL,
    CONSTRAINT [PK_database_update_log] PRIMARY KEY CLUSTERED 
    (
      [id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

    GRANT DELETE ON [database_update_log] TO [admins] AS [dbo]
    GRANT INSERT ON [database_update_log] TO [admins] AS [dbo]
    GRANT SELECT ON [database_update_log] TO [admins] AS [dbo]
    GRANT UPDATE ON [database_update_log] TO [admins] AS [dbo]

    GRANT INSERT ON [database_update_log] TO [users] AS [dbo]
    GRANT SELECT ON [database_update_log] TO [users] AS [dbo]
    GRANT UPDATE ON [database_update_log] TO [users] AS [dbo]


  ALTER TABLE [dbo].[database_update_log] ADD CONSTRAINT [DF_database_update_log_executed] DEFAULT (getdate()) FOR [executed]

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);
  
  UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;
END
GO