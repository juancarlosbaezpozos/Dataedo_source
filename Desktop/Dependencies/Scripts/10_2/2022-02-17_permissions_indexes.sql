DECLARE @id AS UNIQUEIDENTIFIER = 'a3c181a6-6888-47a3-8806-f96ab7165435'
DECLARE @version as int = 10 
DECLARE @update as int = 2
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-3428'

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

      IF NOT EXISTS
        (
        SELECT TOP 1 1
        FROM [sys].[indexes]
        WHERE [name] = 'IX_licenses_name'
            AND [object_id] = OBJECT_ID(N'[licenses]')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_licenses_name ON [licenses] 
            (
                [name]
            )
        END;


     IF NOT EXISTS
        (
        SELECT TOP 1 1
        FROM [sys].[indexes]
        WHERE [name] = 'IX_user_groups_name'
            AND [object_id] = OBJECT_ID(N'[user_groups]')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_user_groups_name ON [user_groups] 
            (
                [name]
            )
        END;
    

     IF NOT EXISTS
        (
        SELECT TOP 1 1
        FROM [sys].[indexes]
        WHERE [name] = 'IX_users_user_groups_user_id'
            AND [object_id] = OBJECT_ID(N'[users_user_groups]')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_users_user_groups_user_id ON [users_user_groups] 
            (
                [user_id]
            )
        END;


    IF NOT EXISTS
        (
        SELECT TOP 1 1
        FROM [sys].[indexes]
        WHERE [name] = 'IX_users_user_groups_user_group_id'
            AND [object_id] = OBJECT_ID(N'[users_user_groups]')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_users_user_groups_user_group_id ON [users_user_groups] 
            (
                [user_group_id]
            )
        END;


    IF NOT EXISTS
        (
        SELECT TOP 1 1
        FROM [sys].[indexes]
        WHERE [name] = 'IX_roles_name'
            AND [object_id] = OBJECT_ID(N'[roles]')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_roles_name ON [roles] 
            (
                [name]
            )
        END;


    IF NOT EXISTS
        (
        SELECT TOP 1 1
        FROM [sys].[indexes]
        WHERE [name] = 'IX_role_actions_role_id'
            AND [object_id] = OBJECT_ID(N'[role_actions]')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_role_actions_role_id ON [role_actions] 
            (
                [role_id]
            )
        END;


    IF NOT EXISTS
        (
        SELECT TOP 1 1
        FROM [sys].[indexes]
        WHERE [name] = 'IX_role_actions_action_code'
            AND [object_id] = OBJECT_ID(N'[role_actions]')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_role_actions_action_code ON [role_actions] 
            (
                [action_code]
            )
        END;
    

    IF NOT EXISTS
            (
            SELECT TOP 1 1
            FROM [sys].[indexes]
            WHERE [name] = 'IX_permissions_user_id'
                AND [object_id] = OBJECT_ID(N'[permissions]')
            )
            BEGIN
                CREATE NONCLUSTERED INDEX IX_permissions_user_id ON [permissions] 
                (
                    [user_id]
                )
            END;


    IF NOT EXISTS
            (
            SELECT TOP 1 1
            FROM [sys].[indexes]
            WHERE [name] = 'IX_permissions_user_type_user_id'
                AND [object_id] = OBJECT_ID(N'[permissions]')
            )
            BEGIN
                CREATE NONCLUSTERED INDEX IX_permissions_user_type_user_id ON [permissions] 
                (
                    [user_type]
                    ,[user_id]
                )
            END;


    IF NOT EXISTS
        (
        SELECT TOP 1 1
        FROM [sys].[indexes]
        WHERE [name] = 'IX_permissions_user_group_id'
            AND [object_id] = OBJECT_ID(N'[permissions]')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_permissions_user_group_id ON [permissions] 
            (
                [user_group_id]
            )
        END;

    
    IF NOT EXISTS
        (
        SELECT TOP 1 1
        FROM [sys].[indexes]
        WHERE [name] = 'IX_permissions_object_type_database_id'
            AND [object_id] = OBJECT_ID(N'[permissions]')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_permissions_object_type_database_id ON [permissions] 
            (
                [object_type]
                ,[database_id]
            )
        END;


    IF NOT EXISTS
        (
        SELECT TOP 1 1
        FROM [sys].[indexes]
        WHERE [name] = 'IX_permissions_database_id'
            AND [object_id] = OBJECT_ID(N'[permissions]')
        )
        BEGIN
            CREATE NONCLUSTERED INDEX IX_permissions_database_id ON [permissions] 
            (
                [database_id]
            )
        END;

    
     IF NOT EXISTS
            (
            SELECT TOP 1 1
            FROM [sys].[indexes]
            WHERE [name] = 'IX_permissions_role_id'
                AND [object_id] = OBJECT_ID(N'[permissions]')
            )
            BEGIN
                CREATE NONCLUSTERED INDEX IX_permissions_role_id ON [permissions] 
                (
                    [role_id]
                )
            END;


  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO