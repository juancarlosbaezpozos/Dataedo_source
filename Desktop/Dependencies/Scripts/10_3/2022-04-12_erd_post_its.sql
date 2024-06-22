DECLARE @id AS UNIQUEIDENTIFIER = '0713d3b7-ef1d-46a1-a3a3-649d69b4109d'
DECLARE @version as int = 10 
DECLARE @update as int = 3
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-2919'

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
  
  IF OBJECT_ID(N'[erd_post_its]',N'U') IS NULL
  BEGIN
	/****** Object:  Table [dbo].[erd_post_its] ******/
	CREATE TABLE [dbo].[erd_post_its](
		[post_it_id] [int] IDENTITY(1,1) NOT NULL,
		[module_id] [int] NOT NULL,
		[pos_x] [int] NULL CONSTRAINT [DF_erd_post_its_pos_x] DEFAULT 0,
		[pos_y] [int] NULL CONSTRAINT [DF_erd_post_its_pos_y] DEFAULT 0,
		[pos_z] [int] NULL CONSTRAINT [DF_erd_post_its_pos_z] DEFAULT 100,
		[color] [char](7) NULL,
		[width] [int] NULL,
		[height] [int] NULL,
		[text] [nvarchar](1000) NULL,
		[text_position] [nvarchar](20) NULL,
		[creation_date] [datetime] NOT NULL CONSTRAINT [DF_erd_post_its_creation_date] DEFAULT getdate(),
		[created_by] [nvarchar](1024) NULL CONSTRAINT [DF_erd_post_its_created_by] DEFAULT suser_sname(),
		[last_modification_date] [datetime] NOT NULL CONSTRAINT [DF_erd_post_its_last_modification_date] DEFAULT getdate(),
		[modified_by] [nvarchar](1024) NULL CONSTRAINT [DF_erd_post_its_modified_by] DEFAULT suser_sname(),
		CONSTRAINT [PK_erd_post_its] PRIMARY KEY CLUSTERED ([post_it_id]));

		ALTER TABLE [dbo].[erd_post_its]  WITH CHECK ADD  CONSTRAINT [FK_erd_post_its_modules] FOREIGN KEY([module_id]) REFERENCES [dbo].[modules] ([module_id])
			ON DELETE CASCADE;
		ALTER TABLE [dbo].[erd_post_its] CHECK CONSTRAINT [FK_erd_post_its_modules];

		GRANT DELETE ON [erd_post_its] TO [users] AS [dbo]
		GRANT INSERT ON [erd_post_its] TO [users] AS [dbo]
		GRANT SELECT ON [erd_post_its] TO [users] AS [dbo]
		GRANT UPDATE ON [erd_post_its] TO [users] AS [dbo]

	END;

	IF NOT EXISTS
    (
    SELECT TOP 1 1
    FROM [sys].[triggers]
    WHERE [name] = N'trg_erd_post_its_descriptions_Modify'
    )
	BEGIN
		EXEC dbo.sp_executesql @statement = N'
			CREATE TRIGGER [trg_erd_post_its_descriptions_Modify]
			   ON  [erd_post_its]
			   AFTER INSERT,UPDATE
			AS 
			BEGIN
				 UPDATE [erd_post_its]
				 SET  
					last_modification_date = GETDATE()
				 WHERE post_it_id IN (SELECT DISTINCT post_it_id FROM Inserted)
			END
		'
	END;

	CREATE NONCLUSTERED INDEX [IX_erd_post_its_module_id] ON [erd_post_its] ([module_id]);


  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO