using System.Collections.Generic;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.PostgreSql;
using Dataedo.App.Data.QueryTools;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.HiveMetastore;

internal class SynchronizeHiveMetastorePostgreSQL : SynchronizePostgreSql
{
	private string HiveCatalogName => synchronizeParameters.DatabaseRow.Param3;

	private string HiveDatabaseName => synchronizeParameters.DatabaseRow.Param4;

	private FilterRulesCollection Filter => synchronizeParameters.DatabaseRow.Filter;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.HiveMetastorePostgreSQL;

	public SynchronizeHiveMetastorePostgreSQL(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		string filterString = Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, null, "\"t\".\"TBL_NAME\"");
		yield return "SELECT 'table' AS \"object_type\", count(1) AS \"count\"\r\n                    FROM \"TBLS\" AS \"t\"\r\n                    INNER JOIN \"DBS\" AS \"d\"\r\n                        ON \"t\".\"DB_ID\" = \"d\".\"DB_ID\"\r\n                    WHERE \"d\".\"NAME\" = '" + HiveDatabaseName + "'\r\n                        AND \"d\".\"CTLG_NAME\" = '" + HiveCatalogName + "'\r\n                        " + filterString + "\r\n                        AND \"t\".\"TBL_TYPE\" IN ('MANAGED_TABLE', 'EXTERNAL_TABLE', 'INDEXED_TABLE');";
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		string filterString = Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, null, "\"t\".\"TBL_NAME\"");
		yield return "SELECT 'view' AS \"object_type\", count(1) AS \"count\"\r\n                    FROM \"TBLS\" AS \"t\"\r\n                    INNER JOIN \"DBS\" AS \"d\"\r\n                        ON \"t\".\"DB_ID\" = \"d\".\"DB_ID\"\r\n                    WHERE \"d\".\"NAME\" = '" + HiveDatabaseName + "'\r\n                        AND \"d\".\"CTLG_NAME\" = '" + HiveCatalogName + "'\r\n                        " + filterString + "\r\n                        AND \"t\".\"TBL_TYPE\" IN ('VIRTUAL_VIEW', 'MATERIALIZED_VIEW');";
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		string filterString = Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, null, "\"t\".\"TBL_NAME\"");
		yield return "SELECT \"t\".\"TBL_NAME\" AS \"name\",\r\n                    '' AS \"schema\",\r\n                    \"d\".\"NAME\" AS \"database_name\",\r\n                    CASE\r\n                        WHEN \"t\".\"TBL_TYPE\" = 'MANAGED_TABLE' THEN 'TABLE'\r\n                        WHEN \"t\".\"TBL_TYPE\" = 'INDEX_TABLE' THEN 'TABLE'\r\n                        WHEN \"t\".\"TBL_TYPE\" = 'EXTERNAL_TABLE' THEN 'EXTERNAL_TABLE'\r\n                    END AS \"type\",\r\n                    NULL AS \"description\",\r\n                    NULL AS \"definition\",\r\n                    TO_TIMESTAMP(\"t\".\"CREATE_TIME\") AS \"create_date\",\r\n                    TO_TIMESTAMP(\r\n                        CASE \r\n                            WHEN \"t\".\"LAST_ACCESS_TIME\" = 0 THEN \"t\".\"CREATE_TIME\"\r\n                            ELSE \"t\".\"LAST_ACCESS_TIME\"\r\n                        END\r\n                    ) AS \"modify_date\",\r\n                    NULL AS \"function_type\"\r\n                FROM \"TBLS\" AS \"t\"\r\n                INNER JOIN \"DBS\" AS \"d\"\r\n                    ON \"t\".\"DB_ID\" = \"d\".\"DB_ID\"\r\n                WHERE \"d\".\"NAME\" = '" + HiveDatabaseName + "'\r\n                    AND \"d\".\"CTLG_NAME\" = '" + HiveCatalogName + "'\r\n                    " + filterString + "\r\n                    AND \"t\".\"TBL_TYPE\" IN ('MANAGED_TABLE', 'EXTERNAL_TABLE', 'INDEX_TABLE');";
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		string filterString = Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, null, "\"t\".\"TBL_NAME\"");
		yield return "SELECT \"t\".\"TBL_NAME\" AS \"name\",\r\n                    '' AS \"schema\",\r\n                    \"d\".\"NAME\" AS \"database_name\",\r\n                    CASE\r\n                        WHEN \"t\".\"TBL_TYPE\" = 'MATERIALIZED_VIEW' THEN \"t\".\"TBL_TYPE\"\r\n                        WHEN \"t\".\"TBL_TYPE\" = 'VIRTUAL_VIEW' THEN 'VIEW'\r\n                    END AS \"type\",\r\n                    NULL AS \"description\",\r\n                    \"t\".\"VIEW_ORIGINAL_TEXT\" AS \"definition\",\r\n                    TO_TIMESTAMP(\"t\".\"CREATE_TIME\") AS \"create_date\",\r\n                    TO_TIMESTAMP(\r\n                        CASE  \r\n                            WHEN \"t\".\"LAST_ACCESS_TIME\" = 0 THEN \"t\".\"CREATE_TIME\"\r\n                            ELSE \"t\".\"LAST_ACCESS_TIME\"\r\n                        END\r\n                    ) AS \"modify_date\",\r\n                    NULL AS \"function_type\"\r\n                FROM \"TBLS\" AS \"t\"\r\n                INNER JOIN \"DBS\" AS \"d\"\r\n                    ON \"t\".\"DB_ID\" = \"d\".\"DB_ID\"\r\n                WHERE \"d\".\"NAME\" = '" + HiveDatabaseName + "'\r\n                    AND \"d\".\"CTLG_NAME\" = '" + HiveCatalogName + "'\r\n                    " + filterString + "\r\n                    AND \"t\".\"TBL_TYPE\" IN ('MATERIALIZED_VIEW', 'VIRTUAL_VIEW');";
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> RelationsQuery()
	{
		string filterString = Filter.GetFilterString(null, null, FilterObjectTypeEnum.FilterObjectType.Table, null, "\"fk_table\".\"TBL_NAME\"");
		string filterString2 = Filter.GetFilterString("OR", null, FilterObjectTypeEnum.FilterObjectType.Table, null, "\"ref_table\".\"TBL_NAME\"");
		string text = FilterRulesCollection.CombineRelationsFilter(filterString, filterString2);
		yield return "\r\n                SELECT \r\n                    \"kc\".\"CONSTRAINT_NAME\" AS \"name\",\r\n                    \"fk_db\".\"NAME\" AS \"fk_table_database_name\",\r\n                    \"fk_table\".\"TBL_NAME\" AS \"fk_table_name\",\r\n                    '' AS \"fk_table_schema\",\r\n                    \"fk_column\".\"COLUMN_NAME\" AS \"fk_column\",\r\n                    \"ref_dbs\".\"NAME\" AS \"ref_table_database_name\",\r\n                    \"ref_table\".\"TBL_NAME\" AS \"ref_table_name\",\r\n                    '' AS \"ref_table_schema\",\r\n                    \"ref_column\".\"COLUMN_NAME\" AS \"ref_column\",\r\n                    \"kc\".\"POSITION\" AS \"ordinal_position\",\r\n                    NULL AS \"description\",\r\n                    \"kc\".\"UPDATE_RULE\" AS \"update_rule\",\r\n                    \"kc\".\"DELETE_RULE\" AS \"delete_rule\"\r\n                FROM \"KEY_CONSTRAINTS\" AS \"kc\"\r\n                INNER JOIN \"COLUMNS_V2\" AS \"fk_column\"\r\n                    ON \"kc\".\"PARENT_CD_ID\" = \"fk_column\".\"CD_ID\"\r\n                    AND \"kc\".\"PARENT_INTEGER_IDX\" = \"fk_column\".\"INTEGER_IDX\"\r\n                INNER JOIN \"TBLS\" AS \"fk_table\"\r\n                    ON \"kc\".\"PARENT_TBL_ID\" = \"fk_table\".\"TBL_ID\"\r\n                INNER JOIN \"DBS\" AS \"fk_db\"\r\n                    ON \"fk_table\".\"DB_ID\" = \"fk_db\".\"DB_ID\"\r\n                INNER JOIN \"COLUMNS_V2\" AS \"ref_column\"\r\n                    ON \"kc\".\"CHILD_CD_ID\" = \"ref_column\".\"CD_ID\"\r\n                    AND \"kc\".\"CHILD_INTEGER_IDX\" = \"ref_column\".\"INTEGER_IDX\"\r\n                INNER JOIN \"TBLS\" AS \"ref_table\"\r\n                    ON \"kc\".\"CHILD_TBL_ID\" = \"ref_table\".\"TBL_ID\"\r\n                INNER JOIN \"DBS\" AS \"ref_dbs\"\r\n                    ON \"ref_table\".\"DB_ID\" = \"ref_dbs\".\"DB_ID\"\r\n                WHERE \"CONSTRAINT_TYPE\" = 1\r\n                    " + text + ";";
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		string filterString = Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, null, "\"t\".\"TBL_NAME\"");
		string filterString2 = Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, null, "\"t\".\"TBL_NAME\"");
		yield return "\r\n                SELECT \r\n                    \"d\".\"NAME\" AS \"database_name\",\r\n                    \"t\".\"TBL_NAME\" AS \"table_name\",\r\n                    '' AS \"table_schema\",\r\n                    \"c\".\"COLUMN_NAME\" AS \"name\",\r\n                    \"c\".\"INTEGER_IDX\" AS \"position\",\r\n                    \"c\".\"TYPE_NAME\" AS \"datatype\",\r\n                    \"c\".\"COMMENT\" AS \"description\",\r\n                    CASE\r\n                        WHEN \"primary_kc\".\"CONSTRAINT_TYPE\" IS NOT NULL THEN 'P'\r\n                    END AS \"constraint_type\",\r\n                    CASE\r\n                        WHEN \"not_null_kc\".\"CONSTRAINT_TYPE\" IS NOT NULL THEN 0\r\n                        ELSE 1\r\n                    END AS \"nullable\",\r\n                    CASE\r\n                        WHEN \"default_kc\".\"CONSTRAINT_TYPE\" IS NOT NULL\r\n                        THEN \"default_kc\".\"DEFAULT_VALUE\"\r\n                    END AS \"default_value\",\r\n                    0 AS \"is_identity\",\r\n                    0 AS \"is_computed\",\r\n                    NULL AS \"computed_formula\",\r\n                   " + QueryDataTypes.GetQueryForDataLength(synchronizeParameters.DatabaseRow.Type) + " \r\n                FROM \"COLUMNS_V2\" AS \"c\"\r\n                INNER JOIN \"CDS\" AS \"cd\" \r\n                    ON \"c\".\"CD_ID\" = \"cd\".\"CD_ID\"\r\n                INNER JOIN \"SDS\" AS \"s\" \r\n                    ON \"cd\".\"CD_ID\" = \"s\".\"CD_ID\"\r\n                INNER JOIN \"TBLS\" AS \"t\"\r\n                    ON \"s\".\"SD_ID\" = \"t\".\"SD_ID\"\r\n                INNER JOIN \"DBS\" AS \"d\"\r\n                    ON \"t\".\"DB_ID\" = \"d\".\"DB_ID\"\r\n                LEFT JOIN \"KEY_CONSTRAINTS\" AS \"primary_kc\"\r\n                    ON \"c\".\"CD_ID\" = \"primary_kc\".\"PARENT_CD_ID\"\r\n                    AND \"c\".\"INTEGER_IDX\" = \"primary_kc\".\"PARENT_INTEGER_IDX\"\r\n                    AND \"t\".\"TBL_ID\" = \"primary_kc\".\"PARENT_TBL_ID\"\r\n                    AND \"primary_kc\".\"CONSTRAINT_TYPE\" = 0\r\n                LEFT JOIN \"KEY_CONSTRAINTS\" AS \"not_null_kc\"\r\n                    ON \"cd\".\"CD_ID\" = \"not_null_kc\".\"PARENT_CD_ID\"\r\n                    AND \"c\".\"INTEGER_IDX\" = \"not_null_kc\".\"PARENT_INTEGER_IDX\"\r\n                    AND \"t\".\"TBL_ID\" = \"not_null_kc\".\"PARENT_TBL_ID\"\r\n                \tAND \"not_null_kc\".\"CONSTRAINT_TYPE\" = 3\r\n                LEFT JOIN \"KEY_CONSTRAINTS\" AS \"default_kc\"\r\n                    ON \"cd\".\"CD_ID\" = \"default_kc\".\"PARENT_CD_ID\"\r\n                    AND \"c\".\"INTEGER_IDX\" = \"default_kc\".\"PARENT_INTEGER_IDX\"\r\n                    AND \"t\".\"TBL_ID\" = \"default_kc\".\"PARENT_TBL_ID\"\r\n                    AND \"default_kc\".\"CONSTRAINT_TYPE\" IN (4,5)\r\n                WHERE \"d\".\"NAME\" = '" + HiveDatabaseName + "'\r\n                    AND \"d\".\"CTLG_NAME\" = '" + HiveCatalogName + "'\r\n                    AND (\r\n                    (\"t\".\"TBL_TYPE\" IN ('MANAGED_TABLE', 'EXTERNAL_TABLE', 'INDEX_TABLE')\r\n                    " + filterString + ")\r\n                    OR\r\n                    (\"t\".\"TBL_TYPE\" IN ('MATERIALIZED_VIEW', 'VIRTUAL_VIEW')\r\n                    " + filterString2 + ")\r\n                        )";
	}

	public override IEnumerable<string> TriggersQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		string filterString = Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, null, "\"t\".\"TBL_NAME\"");
		yield return "\r\n                SELECT\r\n                    \"db\".\"NAME\" AS \"database_name\",\r\n                    \"t\".\"TBL_NAME\" AS \"table_name\",\r\n                    '' AS \"table_schema\",\r\n                    \"kc\".\"CONSTRAINT_NAME\" AS \"name\",\r\n                    CASE \r\n                        WHEN \"kc\".\"CONSTRAINT_TYPE\" = 0 THEN 'P'\r\n                        WHEN \"kc\".\"CONSTRAINT_TYPE\" = 2 THEN 'U'\r\n                    END AS \"type\",\r\n                    \"c\".\"COLUMN_NAME\" AS \"column_name\",\r\n                    \"c\".\"INTEGER_IDX\" AS \"column_ordinal\",\r\n                    \"c\".\"COMMENT\" AS \"description\",\r\n                    CASE\r\n                        WHEN \"kc\".\"CONSTRAINT_TYPE\" = 0 THEN 1\r\n                        WHEN \"kc\".\"CONSTRAINT_TYPE\" = 2 THEN 0\r\n                    END AS \"disabled\"\r\n                FROM \"KEY_CONSTRAINTS\" AS \"kc\"\r\n                INNER JOIN \"COLUMNS_V2\" AS \"c\"\r\n                    ON \"kc\".\"PARENT_CD_ID\" = \"c\".\"CD_ID\"\r\n                    AND \"kc\".\"PARENT_INTEGER_IDX\" = \"c\".\"INTEGER_IDX\"\r\n                INNER JOIN \"TBLS\" AS \"t\"\r\n                    ON \"kc\".\"PARENT_TBL_ID\" = \"t\".\"TBL_ID\"\r\n                INNER JOIN \"DBS\" AS \"db\"\r\n                    ON \"t\".\"DB_ID\" = \"db\".\"DB_ID\"\r\n                WHERE \"db\".\"NAME\" = '" + HiveDatabaseName + "'\r\n                    AND \"db\".\"CTLG_NAME\" = '" + HiveCatalogName + "'\r\n                    AND \"kc\".\"CONSTRAINT_TYPE\" IN (0, 2)\r\n                    " + filterString;
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		yield return string.Empty;
	}
}
