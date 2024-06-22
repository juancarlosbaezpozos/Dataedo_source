-- Version
IF
  (
   SELECT COUNT(*)
     FROM [version]
     WHERE [version] = 7
           AND [update] = 2
           AND [release] = 2
  ) = 0
    BEGIN
        INSERT INTO [version]
        ([version],
         [update],
         [release],
         [stable]
        )
        VALUES
        (7,
         2,
         2,
         0
        );
    END;
GO

GRANT DELETE ON [columns] TO [users] AS [dbo]
GO
GRANT INSERT ON [columns] TO [users] AS [dbo]
GO
GRANT SELECT ON [columns] TO [users] AS [dbo]
GO
GRANT UPDATE ON [columns] TO [users] AS [dbo]
GO
GRANT DELETE ON [custom_fields] TO [users] AS [dbo]
GO
GRANT INSERT ON [custom_fields] TO [users] AS [dbo]
GO
GRANT SELECT ON [custom_fields] TO [users] AS [dbo]
GO
GRANT UPDATE ON [custom_fields] TO [users] AS [dbo]
GO
GRANT DELETE ON [custom_fields_values] TO [users] AS [dbo]
GO
GRANT INSERT ON [custom_fields_values] TO [users] AS [dbo]
GO
GRANT SELECT ON [custom_fields_values] TO [users] AS [dbo]
GO
GRANT UPDATE ON [custom_fields_values] TO [users] AS [dbo]
GO
GRANT DELETE ON [databases] TO [users] AS [dbo]
GO
GRANT INSERT ON [databases] TO [users] AS [dbo]
GO
GRANT SELECT ON [databases] TO [users] AS [dbo]
GO
GRANT UPDATE ON [databases] TO [users] AS [dbo]
GO
GRANT DELETE ON [dependencies] TO [users] AS [dbo]
GO
GRANT INSERT ON [dependencies] TO [users] AS [dbo]
GO
GRANT SELECT ON [dependencies] TO [users] AS [dbo]
GO
GRANT UPDATE ON [dependencies] TO [users] AS [dbo]
GO
GRANT DELETE ON [dependencies_descriptions] TO [users] AS [dbo]
GO
GRANT INSERT ON [dependencies_descriptions] TO [users] AS [dbo]
GO
GRANT SELECT ON [dependencies_descriptions] TO [users] AS [dbo]
GO
GRANT UPDATE ON [dependencies_descriptions] TO [users] AS [dbo]
GO
GRANT DELETE ON [documentation_custom_fields] TO [users] AS [dbo]
GO
GRANT INSERT ON [documentation_custom_fields] TO [users] AS [dbo]
GO
GRANT SELECT ON [documentation_custom_fields] TO [users] AS [dbo]
GO
GRANT UPDATE ON [documentation_custom_fields] TO [users] AS [dbo]
GO
GRANT DELETE ON [erd_links] TO [users] AS [dbo]
GO
GRANT INSERT ON [erd_links] TO [users] AS [dbo]
GO
GRANT SELECT ON [erd_links] TO [users] AS [dbo]
GO
GRANT UPDATE ON [erd_links] TO [users] AS [dbo]
GO
GRANT DELETE ON [erd_nodes] TO [users] AS [dbo]
GO
GRANT INSERT ON [erd_nodes] TO [users] AS [dbo]
GO
GRANT SELECT ON [erd_nodes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [erd_nodes] TO [users] AS [dbo]
GO
GRANT DELETE ON [erd_nodes_columns] TO [users] AS [dbo]
GO
GRANT INSERT ON [erd_nodes_columns] TO [users] AS [dbo]
GO
GRANT SELECT ON [erd_nodes_columns] TO [users] AS [dbo]
GO
GRANT UPDATE ON [erd_nodes_columns] TO [users] AS [dbo]
GO
GRANT DELETE ON [guid] TO [users] AS [dbo]
GO
GRANT INSERT ON [guid] TO [users] AS [dbo]
GO
GRANT SELECT ON [guid] TO [users] AS [dbo]
GO
GRANT UPDATE ON [guid] TO [users] AS [dbo]
GO
GRANT DELETE ON [ignored_objects] TO [users] AS [dbo]
GO
GRANT INSERT ON [ignored_objects] TO [users] AS [dbo]
GO
GRANT SELECT ON [ignored_objects] TO [users] AS [dbo]
GO
GRANT UPDATE ON [ignored_objects] TO [users] AS [dbo]
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
GRANT DELETE ON [modules] TO [users] AS [dbo]
GO
GRANT INSERT ON [modules] TO [users] AS [dbo]
GO
GRANT SELECT ON [modules] TO [users] AS [dbo]
GO
GRANT UPDATE ON [modules] TO [users] AS [dbo]
GO
GRANT DELETE ON [parameters] TO [users] AS [dbo]
GO
GRANT INSERT ON [parameters] TO [users] AS [dbo]
GO
GRANT SELECT ON [parameters] TO [users] AS [dbo]
GO
GRANT UPDATE ON [parameters] TO [users] AS [dbo]
GO
GRANT DELETE ON [procedures] TO [users] AS [dbo]
GO
GRANT INSERT ON [procedures] TO [users] AS [dbo]
GO
GRANT SELECT ON [procedures] TO [users] AS [dbo]
GO
GRANT UPDATE ON [procedures] TO [users] AS [dbo]
GO
GRANT DELETE ON [procedures_modules] TO [users] AS [dbo]
GO
GRANT INSERT ON [procedures_modules] TO [users] AS [dbo]
GO
GRANT SELECT ON [procedures_modules] TO [users] AS [dbo]
GO
GRANT UPDATE ON [procedures_modules] TO [users] AS [dbo]
GO
GRANT DELETE ON [tables] TO [users] AS [dbo]
GO
GRANT INSERT ON [tables] TO [users] AS [dbo]
GO
GRANT SELECT ON [tables] TO [users] AS [dbo]
GO
GRANT UPDATE ON [tables] TO [users] AS [dbo]
GO
GRANT DELETE ON [tables_modules] TO [users] AS [dbo]
GO
GRANT INSERT ON [tables_modules] TO [users] AS [dbo]
GO
GRANT SELECT ON [tables_modules] TO [users] AS [dbo]
GO
GRANT UPDATE ON [tables_modules] TO [users] AS [dbo]
GO
GRANT DELETE ON [tables_relations] TO [users] AS [dbo]
GO
GRANT INSERT ON [tables_relations] TO [users] AS [dbo]
GO
GRANT SELECT ON [tables_relations] TO [users] AS [dbo]
GO
GRANT UPDATE ON [tables_relations] TO [users] AS [dbo]
GO
GRANT DELETE ON [tables_relations_columns] TO [users] AS [dbo]
GO
GRANT INSERT ON [tables_relations_columns] TO [users] AS [dbo]
GO
GRANT SELECT ON [tables_relations_columns] TO [users] AS [dbo]
GO
GRANT UPDATE ON [tables_relations_columns] TO [users] AS [dbo]
GO
GRANT DELETE ON [triggers] TO [users] AS [dbo]
GO
GRANT INSERT ON [triggers] TO [users] AS [dbo]
GO
GRANT SELECT ON [triggers] TO [users] AS [dbo]
GO
GRANT UPDATE ON [triggers] TO [users] AS [dbo]
GO
GRANT DELETE ON [unique_constraints] TO [users] AS [dbo]
GO
GRANT INSERT ON [unique_constraints] TO [users] AS [dbo]
GO
GRANT SELECT ON [unique_constraints] TO [users] AS [dbo]
GO
GRANT UPDATE ON [unique_constraints] TO [users] AS [dbo]
GO
GRANT DELETE ON [unique_constraints_columns] TO [users] AS [dbo]
GO
GRANT INSERT ON [unique_constraints_columns] TO [users] AS [dbo]
GO
GRANT SELECT ON [unique_constraints_columns] TO [users] AS [dbo]
GO
GRANT UPDATE ON [unique_constraints_columns] TO [users] AS [dbo]
GO
GRANT DELETE ON [version] TO [users] AS [dbo]
GO
GRANT INSERT ON [version] TO [users] AS [dbo]
GO
GRANT SELECT ON [version] TO [users] AS [dbo]
GO
GRANT UPDATE ON [version] TO [users] AS [dbo]
GO
GRANT DELETE ON [columns_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [columns_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [columns_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [columns_changes] TO [users] AS [dbo]
GO
GRANT DELETE ON [parameters_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [parameters_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [parameters_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [parameters_changes] TO [users] AS [dbo]
GO
GRANT DELETE ON [procedures_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [procedures_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [procedures_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [procedures_changes] TO [users] AS [dbo]
GO
GRANT DELETE ON [schema_updates] TO [users] AS [dbo]
GO
GRANT INSERT ON [schema_updates] TO [users] AS [dbo]
GO
GRANT SELECT ON [schema_updates] TO [users] AS [dbo]
GO
GRANT UPDATE ON [schema_updates] TO [users] AS [dbo]
GO
GRANT DELETE ON [tables_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [tables_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [tables_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [tables_changes] TO [users] AS [dbo]
GO
GRANT DELETE ON [tables_relations_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [tables_relations_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [tables_relations_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [tables_relations_changes] TO [users] AS [dbo]
GO
GRANT DELETE ON [tables_relations_columns_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [tables_relations_columns_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [tables_relations_columns_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [tables_relations_columns_changes] TO [users] AS [dbo]
GO
GRANT DELETE ON [triggers_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [triggers_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [triggers_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [triggers_changes] TO [users] AS [dbo]
GO
GRANT DELETE ON [unique_constraints_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [unique_constraints_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [unique_constraints_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [unique_constraints_changes] TO [users] AS [dbo]
GO
GRANT DELETE ON [unique_constraints_columns_changes] TO [users] AS [dbo]
GO
GRANT INSERT ON [unique_constraints_columns_changes] TO [users] AS [dbo]
GO
GRANT SELECT ON [unique_constraints_columns_changes] TO [users] AS [dbo]
GO
GRANT UPDATE ON [unique_constraints_columns_changes] TO [users] AS [dbo]
GO

-- Version
UPDATE [version]
  SET
      [stable] = 1
  WHERE [version] = 7
        AND [update] = 2
        AND [release] = 2;
GO