using System.Collections.Generic;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.SqlServer;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.Azure;

internal class SynchronizeAzureSQLDataWarehouse : SynchronizeDB
{
	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.AzureSQLDataWarehouse;

	public SynchronizeAzureSQLDataWarehouse(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	public override bool GetRelations(string query, Form owner = null)
	{
		return true;
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
		yield return "SELECT *\r\n                    FROM (\r\n                        SELECT DISTINCT CASE \r\n                                WHEN o.type_desc IN (\r\n                                        'SQL_STORED_PROCEDURE'\r\n                                        , 'CLR_STORED_PROCEDURE'\r\n                                        , 'EXTENDED_STORED_PROCEDURE'\r\n                                        , 'REPLICATION_FILTER_PROCEDURE'\r\n                                        )\r\n                                    THEN 'PROCEDURE'\r\n                                WHEN o.type_desc IN (\r\n                                        'SQL_SCALAR_FUNCTION'\r\n                                        , 'SQL_TABLE_VALUED_FUNCTION'\r\n                                        , 'CLR_SCALAR_FUNCTION'\r\n                                        , 'CLR_TABLE_VALUED_FUNCTION'\r\n                                        , 'AGGREGATE_FUNCTION'\r\n                                        , 'SQL_INLINE_TABLE_VALUED_FUNCTION'\r\n                                        )\r\n                                    THEN 'FUNCTION'\r\n                                WHEN o.type_desc IN (\r\n                                        'CLR_TRIGGER'\r\n                                        , 'SQL_TRIGGER'\r\n                                        )\r\n                                    THEN 'TRIGGER'\r\n                                WHEN o.type_desc IN (\r\n                                        'INTERNAL_TABLE'\r\n                                        , 'SYSTEM_TABLE'\r\n                                        , 'USER_TABLE'\r\n                                        )\r\n                                    THEN 'TABLE'\r\n                                WHEN o.type_desc IN ('VIEW')\r\n                                    THEN 'VIEW'\r\n                                ELSE NULL\r\n                                END AS referencing_type\r\n                            , '" + text + "' AS referencing_server\r\n                            , ISNULL(referencing_schemas.NAME, SCHEMA_NAME()) AS referencing_schema_name\r\n                            , DB_NAME() AS referencing_database_name\r\n                            , OBJECT_NAME(sed.referencing_id) AS referencing_entity_name\r\n                            , '" + text + "' AS referenced_server\r\n                            , ISNULL(sed.referenced_database_name, DB_NAME()) AS referenced_database_name\r\n                            , ISNULL(sed.referenced_schema_name, SCHEMA_NAME()) AS referenced_schema_name\r\n                            , CASE \r\n                                WHEN ao.type_desc IN (\r\n                                        'SQL_STORED_PROCEDURE'\r\n                                        , 'CLR_STORED_PROCEDURE'\r\n                                        , 'EXTENDED_STORED_PROCEDURE'\r\n                                        , 'REPLICATION_FILTER_PROCEDURE'\r\n                                        )\r\n                                    THEN 'PROCEDURE'\r\n                                WHEN ao.type_desc IN (\r\n                                        'SQL_SCALAR_FUNCTION'\r\n                                        , 'SQL_TABLE_VALUED_FUNCTION'\r\n                                        , 'CLR_SCALAR_FUNCTION'\r\n                                        , 'CLR_TABLE_VALUED_FUNCTION'\r\n                                        , 'AGGREGATE_FUNCTION'\r\n                                        , 'SQL_INLINE_TABLE_VALUED_FUNCTION'\r\n                                        )\r\n                                    THEN 'FUNCTION'\r\n                                WHEN ao.type_desc IN (\r\n                                        'CLR_TRIGGER'\r\n                                        , 'SQL_TRIGGER'\r\n                                        )\r\n                                    THEN 'TRIGGER'\r\n                                WHEN ao.type_desc IN (\r\n                                        'INTERNAL_TABLE'\r\n                                        , 'SYSTEM_TABLE'\r\n                                        , 'USER_TABLE'\r\n                                        )\r\n                                    THEN 'TABLE'\r\n                                WHEN ao.type_desc IN ('VIEW')\r\n                                    THEN 'VIEW'\r\n                                ELSE NULL\r\n                                END AS referenced_type\r\n                            , sed.referenced_entity_name\r\n                            , CAST(sed.is_caller_dependent AS CHAR(1)) AS is_caller_dependent\r\n                            , CAST(sed.is_ambiguous AS CHAR(1)) AS is_ambiguous\r\n                            , NULL AS dependency_type\r\n                        FROM sys.sql_expression_dependencies AS sed\r\n                        LEFT JOIN sys.objects AS o\r\n                            ON sed.referencing_id = o.object_id\r\n                        LEFT JOIN sys.all_objects ao\r\n                            ON sed.referenced_entity_name = ao.NAME\r\n                                AND ISNULL(sed.referenced_database_name, DB_NAME()) = DB_NAME()\r\n                        LEFT JOIN sys.schemas referencing_schemas\r\n                            ON referencing_schemas.schema_id = o.schema_id\r\n                        ) a\r\n                        WHERE referencing_type IS NOT NULL\r\n                            AND referencing_entity_name IS NOT NULL\r\n                            AND referenced_entity_name IS NOT NULL" + text2;
	}
}
