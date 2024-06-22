DECLARE @id AS UNIQUEIDENTIFIER = 'CDFFD7E2-0D34-4CF3-B0D0-CEBBE0B7697D'
DECLARE @version as int = 10 
DECLARE @update as int = 3
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-4788'

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
  
  IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'NVARCHAR') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'NVARCHAR', N'STRING', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'REFERENCE') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'REFERENCE', N'STRING', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'NAME: NVARCHAR') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'NAME: NVARCHAR', N'STRING', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'PICKLIST') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'PICKLIST', N'STRING', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'INT64') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'INT64', N'NUMERIC', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'UNIQUEIDENTIFIER') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'UNIQUEIDENTIFIER', N'OTHER', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'TEXTAREA') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'TEXTAREA', N'STRING', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'UNSIGNED SMALLINT') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'UNSIGNED SMALLINT', N'NUMERIC', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'TIMESTAMP WITHOUT TIME ZONE') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'TIMESTAMP WITHOUT TIME ZONE', N'DATE_TIME', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'INT32') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'INT32', N'NUMERIC', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'KEYWORD') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'KEYWORD', N'STRING', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'XML') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'XML', N'OTHER', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'UNSIGNED TINYINT') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'UNSIGNED TINYINT', N'NUMERIC', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'FLAG: BIT') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'FLAG: BIT', N'NUMERIC', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'URL') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'URL', N'STRING', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'DOCUMENT') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'DOCUMENT', N'STRING', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'PHONE: NVARCHAR') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'PHONE: NVARCHAR', N'STRING', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'HALF_FLOAT') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'HALF_FLOAT', N'NUMERIC', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'FLOAT64') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'FLOAT64', N'NUMERIC', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'PHONE') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'PHONE', N'STRING', getdate(), N'dataedo', getdate(), N'dataedo')
END
IF NOT EXISTS (SELECT [datatype] FROM [datatypes] WHERE [datatype] = N'EMAIL') BEGIN
INSERT INTO [datatypes] ([datatype], [global_datatype], [creation_date], [created_by], [last_modification_date], [modified_by]) VALUES (N'EMAIL', N'STRING', getdate(), N'dataedo', getdate(), N'dataedo')
END

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO