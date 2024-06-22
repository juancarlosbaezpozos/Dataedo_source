using System.Collections.Generic;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.SqlServer;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.Azure;

internal class SynchronizeAzureSQLDatabase : SynchronizeDB
{
	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.AzureSQLDatabase;

	public SynchronizeAzureSQLDatabase(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	protected override string GetTableTypesQuery()
	{
		return "CASE\r\n                        WHEN s.IS_EXTERNAL = 1 THEN 'EXTERNAL_TABLE'\r\n                        WHEN s.IS_NODE = 1 THEN 'GRAPH_NODE_TABLE'\r\n                        WHEN s.IS_EDGE = 1 THEN 'GRAPH_EDGE_TABLE'\r\n                        WHEN s.TEMPORAL_TYPE = 2 THEN 'SYSTEM_VERSIONED_TABLE'\r\n                        WHEN s.TEMPORAL_TYPE = 1 THEN 'HISTORY_TABLE'\r\n                        WHEN s.IS_FILETABLE = 1 THEN 'FILE_TABLE'\r\n                        ELSE 'TABLE'\r\n                    END AS [TYPE]";
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		synchronizeParameters.DatabaseRow.Name.ToUpper();
		string text = synchronizeParameters.DatabaseRow.Host.ToUpper();
		string filterStringForDependencies = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("a.referencing_schema_name", "a.referencing_entity_name", "a.referencing_type", "a.referenced_type");
		string filterStringForDependencies2 = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("a.referenced_schema_name", "a.referenced_entity_name", "a.referenced_type", "a.referencing_type");
		string text2 = FilterRulesCollection.CombineDependenciesFilter(filterStringForDependencies, filterStringForDependencies2);
		yield return "SELECT *\r\n                        FROM\r\n                        (SELECT DISTINCT\r\n                            CASE\r\n                                WHEN\r\n                                    o.type_desc IN ('SQL_STORED_PROCEDURE','CLR_STORED_PROCEDURE','EXTENDED_STORED_PROCEDURE','REPLICATION_FILTER_PROCEDURE')\r\n                                    THEN 'PROCEDURE'\r\n                                WHEN\r\n                                    o.type_desc IN ('SQL_SCALAR_FUNCTION','SQL_TABLE_VALUED_FUNCTION','CLR_SCALAR_FUNCTION','CLR_TABLE_VALUED_FUNCTION','AGGREGATE_FUNCTION','SQL_INLINE_TABLE_VALUED_FUNCTION')\r\n                                    THEN 'FUNCTION'\r\n                                WHEN\r\n                                    o.type_desc IN ('CLR_TRIGGER','SQL_TRIGGER')\r\n                                    THEN 'TRIGGER'\r\n                                WHEN\r\n                                    o.type_desc IN ('INTERNAL_TABLE','SYSTEM_TABLE','USER_TABLE')\r\n                                    THEN 'TABLE'\r\n                                WHEN\r\n                                    o.type_desc IN ('VIEW')\r\n                                    THEN 'VIEW'\r\n                                ELSE NULL\r\n                            END AS referencing_type,\r\n\t\t\t\t\t\t\t'" + text + "' as referencing_server,\r\n                            ISNULL(referencing_schemas.name, SCHEMA_NAME()) AS referencing_schema_name,\r\n                            DB_NAME() as referencing_database_name,\r\n                            OBJECT_NAME(sed.object_id) AS referencing_entity_name,\r\n                            '" + text + "' as referenced_server,\r\n                            DB_NAME() as referenced_database_name,\r\n                            ISNULL(OBJECT_SCHEMA_NAME ( sed.referenced_major_id ), SCHEMA_NAME()) as referenced_schema_name,\r\n                            CASE\r\n                                WHEN\r\n                                    ao.type_desc IN ('SQL_STORED_PROCEDURE','CLR_STORED_PROCEDURE','EXTENDED_STORED_PROCEDURE','REPLICATION_FILTER_PROCEDURE')\r\n                                    THEN 'PROCEDURE'\r\n                                WHEN\r\n                                    ao.type_desc IN ('SQL_SCALAR_FUNCTION','SQL_TABLE_VALUED_FUNCTION','CLR_SCALAR_FUNCTION','CLR_TABLE_VALUED_FUNCTION','AGGREGATE_FUNCTION','SQL_INLINE_TABLE_VALUED_FUNCTION')\r\n                                    THEN 'FUNCTION'\r\n                                WHEN\r\n                                    ao.type_desc IN ('CLR_TRIGGER','SQL_TRIGGER')\r\n                                    THEN 'TRIGGER'\r\n                                WHEN\r\n                                    ao.type_desc IN ('INTERNAL_TABLE','SYSTEM_TABLE','USER_TABLE')\r\n                                    THEN 'TABLE'\r\n                                WHEN\r\n                                    ao.type_desc IN ('VIEW')\r\n                                    THEN 'VIEW'\r\n\t\t                        ELSE\r\n\t\t\t                        NULL\r\n                            END AS referenced_type,\r\n                            OBJECT_NAME(sed.referenced_major_id) AS referenced_entity_name,\r\n                            CAST (0 AS CHAR(1)) as is_caller_dependent,\r\n                            CAST (0 AS CHAR(1)) as is_ambiguous,\r\n                            NULL as dependency_type\r\n                        FROM \r\n                            sys.sql_dependencies AS sed\r\n                            LEFT JOIN sys.objects AS o ON sed.object_id = o.object_id\r\n                            LEFT JOIN sys.all_objects ao ON object_name(sed.referenced_major_id) = ao.name\r\n                            LEFT JOIN sys.schemas referencing_schemas ON referencing_schemas.schema_id = o.schema_id\r\n                        ) a \r\n                        WHERE referencing_type IS NOT NULL\r\n                            AND referencing_entity_name IS NOT NULL\r\n                            AND referenced_entity_name IS NOT NULL" + text2;
	}
}
