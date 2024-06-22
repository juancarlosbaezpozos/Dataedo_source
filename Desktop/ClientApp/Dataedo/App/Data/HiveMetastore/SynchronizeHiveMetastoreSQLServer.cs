using System.Collections.Generic;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.QueryTools;
using Dataedo.App.Data.SqlServer;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.HiveMetastore;

internal class SynchronizeHiveMetastoreSQLServer : SynchronizeDB
{
	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreSQLServer;

	private FilterRulesCollection Filter => synchronizeParameters.DatabaseRow.Filter;

	private string HiveCatalogName => synchronizeParameters.DatabaseRow.Param3;

	private string HiveDatabaseName => synchronizeParameters.DatabaseRow.Param4;

	protected override bool WithCustomFields => false;

	public override IEnumerable<string> CountTablesQuery()
	{
		string filterString = Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, null, "t.TBL_NAME");
		yield return "SELECT 'table' as 'object_type', count(1) as 'count'\r\n                    FROM TBLS t\r\n                    INNER JOIN DBS d\r\n                        ON t.DB_ID = d.DB_ID\r\n                    WHERE d.NAME = '" + HiveDatabaseName + "'\r\n                        AND d.CTLG_NAME = '" + HiveCatalogName + "'\r\n                        " + filterString + "\r\n                        AND t.TBL_TYPE in ('MANAGED_TABLE', 'EXTERNAL_TABLE', 'INDEXED_TABLE');";
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		string filterString = Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, null, "t.TBL_NAME");
		yield return "SELECT 'view' as 'object_type', count(1) as 'count'\r\n                    FROM TBLS t\r\n                    INNER JOIN DBS d\r\n                        ON t.DB_ID = d.DB_ID\r\n                    WHERE d.NAME = '" + HiveDatabaseName + "'\r\n                        AND d.CTLG_NAME = '" + HiveCatalogName + "'\r\n                        " + filterString + "\r\n                        AND t.TBL_TYPE in ('VIRTUAL_VIEW', 'MATERIALIZED_VIEW');";
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
		string filterString = Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, null, "t.TBL_NAME");
		yield return "SELECT t.TBL_NAME as 'name',\r\n                    '' as 'schema',\r\n                    d.NAME as 'database_name',\r\n                    CASE\r\n                        WHEN t.TBL_TYPE = 'MANAGED_TABLE' then 'TABLE'\r\n                        WHEN t.TBL_TYPE = 'INDEX_TABLE' then 'TABLE'\r\n                        WHEN t.TBL_TYPE = 'EXTERNAL_TABLE' then 'EXTERNAL_TABLE'\r\n                    END AS 'TYPE',\r\n                    null as 'description',\r\n                    null as 'definition',\r\n                    DATEADD(S, t.CREATE_TIME, '1970-01-01') AS 'create_date',\r\n                    DATEADD(S,\r\n                        CASE  \r\n                            WHEN t.LAST_ACCESS_TIME = 0 THEN t.CREATE_TIME\r\n                            ELSE t.LAST_ACCESS_TIME\r\n                        END,\r\n                        '1970-01-01') AS 'modify_date',\r\n                    null as 'function_type'\r\n                FROM TBLS t\r\n                INNER JOIN DBS d\r\n                    ON t.DB_ID = d.DB_ID\r\n                WHERE d.NAME = '" + HiveDatabaseName + "'\r\n                    AND d.CTLG_NAME = '" + HiveCatalogName + "'\r\n                    " + filterString + "\r\n                    AND t.TBL_TYPE IN ('MANAGED_TABLE', 'EXTERNAL_TABLE', 'INDEX_TABLE');";
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		string filterString = Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, null, "t.TBL_NAME");
		yield return "SELECT t.TBL_NAME as 'name',\r\n                    '' as 'schema',\r\n                    d.NAME as 'database_name',\r\n                    CASE\r\n                        WHEN t.TBL_TYPE = 'MATERIALIZED_VIEW' THEN t.TBL_TYPE\r\n                        WHEN t.TBL_TYPE = 'VIRTUAL_VIEW' then 'VIEW'\r\n                    END AS 'TYPE',\r\n                    null as 'description',\r\n                    t.VIEW_ORIGINAL_TEXT as 'definition',\r\n                    DATEADD(S, t.CREATE_TIME, '1970-01-01') AS 'create_date',\r\n                    DATEADD(S,\r\n                        CASE  \r\n                            WHEN t.LAST_ACCESS_TIME = 0 THEN t.CREATE_TIME\r\n                            ELSE t.LAST_ACCESS_TIME\r\n                        END,\r\n                        '1970-01-01') AS 'modify_date',\r\n                    null as 'function_type'\r\n                FROM TBLS t\r\n                INNER JOIN DBS d\r\n                    ON t.DB_ID = d.DB_ID\r\n                WHERE d.NAME = '" + HiveDatabaseName + "'\r\n                    AND d.CTLG_NAME = '" + HiveCatalogName + "'\r\n                    " + filterString + "\r\n                    AND t.TBL_TYPE IN ('MATERIALIZED_VIEW', 'VIRTUAL_VIEW');";
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
		string filterString = Filter.GetFilterString(null, null, FilterObjectTypeEnum.FilterObjectType.Table, null, "fk_table.TBL_NAME");
		string filterString2 = Filter.GetFilterString("OR", null, FilterObjectTypeEnum.FilterObjectType.Table, null, "ref_table.TBL_NAME");
		string text = FilterRulesCollection.CombineRelationsFilter(filterString, filterString2);
		yield return "\r\n                SELECT \r\n                    kc.CONSTRAINT_NAME as 'name',\r\n                    fk_db.NAME as 'fk_table_database_name',\r\n                    fk_table.TBL_NAME as 'fk_table_name',\r\n                    '' as 'fk_table_schema',\r\n                    fk_column.COLUMN_NAME as 'fk_column',\r\n                    ref_dbs.NAME as 'ref_table_database_name',\r\n                    ref_table.TBL_NAME as 'ref_table_name',\r\n                    '' as 'ref_table_schema',\r\n                    ref_column.COLUMN_NAME as 'ref_column',\r\n                    kc.POSITION as 'ordinal_position',\r\n                    NULL as 'description',\r\n                    kc.UPDATE_RULE as 'update_rule',\r\n                    kc.DELETE_RULE as 'delete_rule'\r\n                FROM KEY_CONSTRAINTS kc\r\n                INNER JOIN COLUMNS_V2 fk_column ON\r\n                \tkc.PARENT_CD_ID = fk_column.CD_ID\r\n                    AND kc.PARENT_INTEGER_IDX = fk_column.INTEGER_IDX\r\n                INNER JOIN TBLS fk_table ON\r\n                \tkc.PARENT_TBL_ID = fk_table.TBL_ID\r\n                INNER JOIN DBS fk_db ON\r\n                \tfk_table.DB_ID = fk_db.DB_ID\r\n                INNER JOIN COLUMNS_V2 ref_column ON\r\n                \tkc.CHILD_CD_ID = ref_column.CD_ID\r\n                    AND kc.CHILD_INTEGER_IDX = ref_column.INTEGER_IDX\r\n                INNER JOIN TBLS ref_table ON\r\n                \tkc.CHILD_TBL_ID = ref_table.TBL_ID\r\n                INNER JOIN DBS ref_dbs ON\r\n                \tref_table.DB_ID = ref_dbs.DB_ID\r\n                WHERE CONSTRAINT_TYPE = 1\r\n                    " + text + ";";
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		string filterString = Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, null, "t.TBL_NAME");
		string filterString2 = Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, null, "t.TBL_NAME");
		yield return "\r\n                SELECT \r\n                    d.NAME as 'database_name',\r\n                    t.TBL_NAME as 'table_name',\r\n                    '' as 'table_schema',\r\n                    c.COLUMN_NAME as 'name',\r\n                    c.INTEGER_IDX as 'position',\r\n                    c.TYPE_NAME as 'datatype',\r\n                    c.COMMENT as 'description',\r\n                    CASE\r\n                        WHEN primary_kc.CONSTRAINT_TYPE IS NOT NULL THEN 'P'\r\n                    END AS 'constraint_type',\r\n                    CASE\r\n                        WHEN not_null_kc.CONSTRAINT_TYPE IS NOT NULL THEN 0\r\n                        ELSE 1\r\n                    END AS 'nullable',\r\n                    CASE\r\n                        WHEN default_kc.CONSTRAINT_TYPE IS NOT NULL THEN default_kc.DEFAULT_VALUE\r\n                    END AS 'default_value',\r\n                    0 AS 'is_identity',\r\n                    0 AS 'is_computed',\r\n                    NULL AS 'computed_formula',\r\n                   " + QueryDataTypes.GetQueryForDataLength(synchronizeParameters.DatabaseRow.Type) + " \r\n                FROM COLUMNS_V2 c\r\n                INNER JOIN CDS cd\r\n                    ON c.CD_ID = cd.CD_ID\r\n                INNER JOIN SDS s \r\n                    ON cd.CD_ID = s.CD_ID\r\n                INNER JOIN TBLS t\r\n                    ON s.SD_ID = t.SD_ID\r\n                INNER JOIN DBS d\r\n                    ON t.DB_ID = d.DB_ID\r\n                LEFT JOIN KEY_CONSTRAINTS primary_kc\r\n                    ON  c.CD_ID = primary_kc.PARENT_CD_ID\r\n                    AND c.INTEGER_IDX = primary_kc.PARENT_INTEGER_IDX\r\n                    AND t.TBL_ID = primary_kc.PARENT_TBL_ID\r\n                    AND primary_kc.CONSTRAINT_TYPE = 0\r\n                LEFT JOIN KEY_CONSTRAINTS not_null_kc\r\n                    ON cd.CD_ID = not_null_kc.PARENT_CD_ID\r\n                    AND c.INTEGER_IDX = not_null_kc.PARENT_INTEGER_IDX\r\n                    AND t.TBL_ID = not_null_kc.PARENT_TBL_ID\r\n                \tAND not_null_kc.CONSTRAINT_TYPE = 3\r\n                LEFT JOIN KEY_CONSTRAINTS default_kc\r\n                    ON cd.CD_ID = default_kc.PARENT_CD_ID\r\n                    AND c.INTEGER_IDX = default_kc.PARENT_INTEGER_IDX\r\n                    AND t.TBL_ID = default_kc.PARENT_TBL_ID\r\n                    AND default_kc.CONSTRAINT_TYPE in (4,5)\r\n                WHERE d.NAME = '" + HiveDatabaseName + "'\r\n                    AND d.CTLG_NAME = '" + HiveCatalogName + "'\r\n                    AND (\r\n                    (t.TBL_TYPE IN ('MANAGED_TABLE', 'EXTERNAL_TABLE', 'INDEX_TABLE')\r\n                    " + filterString + ")\r\n                    OR\r\n                    (t.TBL_TYPE IN ('MATERIALIZED_VIEW', 'VIRTUAL_VIEW')\r\n                    " + filterString2 + ")\r\n                        )";
	}

	public override IEnumerable<string> TriggersQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		string filterString = Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, null, "t.TBL_NAME");
		yield return "\r\n                SELECT\r\n                    db.NAME AS 'database_name',\r\n                    t.TBL_NAME AS 'table_name',\r\n                    '' AS 'table_schema',\r\n                    kc.CONSTRAINT_NAME AS 'name',\r\n                    CASE \r\n                        WHEN kc.CONSTRAINT_TYPE = 0 THEN 'P'\r\n                        WHEN kc.CONSTRAINT_TYPE = 2 THEN 'U'\r\n                    END AS 'type',\r\n                    c.COLUMN_NAME as 'column_name',\r\n                    c.INTEGER_IDX as 'column_ordinal',\r\n                    c.COMMENT as 'description',\r\n                    CASE\r\n                        WHEN kc.CONSTRAINT_TYPE = 0 THEN 1\r\n                        WHEN kc.CONSTRAINT_TYPE = 2 THEN 0\r\n                    END AS 'disabled'\r\n                FROM KEY_CONSTRAINTS kc\r\n                INNER JOIN COLUMNS_V2 c\r\n                    ON  kc.PARENT_CD_ID = c.CD_ID\r\n                    AND kc.PARENT_INTEGER_IDX = c.INTEGER_IDX\r\n                INNER JOIN TBLS t\r\n                    ON kc.PARENT_TBL_ID = t.TBL_ID\r\n                INNER JOIN DBS db\r\n                    on t.DB_ID = db.DB_ID\r\n                WHERE db.NAME = '" + HiveDatabaseName + "'\r\n                    AND db.CTLG_NAME = '" + HiveCatalogName + "'\r\n                    " + filterString + "\r\n                    AND kc.CONSTRAINT_TYPE in (0, 2)";
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		yield return string.Empty;
	}

	public SynchronizeHiveMetastoreSQLServer(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}
}
