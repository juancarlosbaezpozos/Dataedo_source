-- Version
IF
  (
   SELECT COUNT(*)
     FROM [version]
     WHERE [version] = 8
           AND [update] = 0
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
        (8,
         0,
         0,
         0
        );
    END;
GO

-- Triggers table
IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[triggers]')
        AND name = 'subtype'
)
BEGIN
    ALTER TABLE [triggers]
    ADD [subtype] [nvarchar](100) NULL
END
GO

-- Databases table
ALTER TABLE [databases] ALTER COLUMN [name] NVARCHAR (MAX) NULL;
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[databases]')
        AND name = 'instance_identifier'
)
BEGIN
    ALTER TABLE [databases]
    ADD [instance_identifier] [nvarchar](50) NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[databases]')
        AND name = 'oracle_sid'
)
BEGIN
    ALTER TABLE [databases]
    ADD oracle_sid [nvarchar](50) NULL
END
GO

-- User connections table
IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[user_connections]')
        AND name = 'instance_identifier'
)
BEGIN
    ALTER TABLE [user_connections]
    ADD [instance_identifier] [nvarchar](50) NULL
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[user_connections]')
        AND name = 'oracle_sid'
)
BEGIN
    ALTER TABLE [user_connections]
    ADD oracle_sid [nvarchar](50) NULL
END
GO

-- Schema updates table
ALTER TABLE [schema_updates] ALTER COLUMN [connection_database_name] NVARCHAR (MAX) NULL;
GO

-- Version
UPDATE [version]
  SET
      [stable] = 1
  WHERE [version] = 8
        AND [update] = 0
        AND [release] = 0;
GO