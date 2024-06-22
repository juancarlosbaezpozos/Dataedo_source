using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Model.Extensions;
using Dataedo.Shared.Enums;
using Google.Cloud.BigQuery.V2;

namespace Dataedo.App.Data.GoogleBigQuery;

internal class SynchronizeGoogleBigQuery : SynchronizeDatabase
{
	public SynchronizeGoogleBigQuery(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Counting objects to import");
		if (!string.IsNullOrWhiteSpace(query.Query))
		{
			try
			{
				foreach (BigQueryRow item in CommandsWithTimeout.GoogleBigQuery(synchronizeParameters.DatabaseRow.Connection, query.Query))
				{
					int.TryParse(string.Format("{0}", item["count"]), out var result);
					ReadCount(SharedObjectTypeEnum.StringToType(query.ObjectType.GetAttributeDescription()).Value, result);
				}
			}
			catch (Exception ex)
			{
				GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
				return false;
			}
		}
		return true;
	}

	public override bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (base.ObjectsCounter != null)
		{
			try
			{
				backgroundWorkerManager.ReportProgress("Retrieving database's objects");
				if (!string.IsNullOrWhiteSpace(query.Query))
				{
					foreach (BigQueryRow item in CommandsWithTimeout.GoogleBigQuery(synchronizeParameters.DatabaseRow.Connection, query.Query))
					{
						DateTime value2;
						if (query.ObjectType == FilterObjectTypeEnum.FilterObjectType.Table || query.ObjectType == FilterObjectTypeEnum.FilterObjectType.View)
						{
							double value = Convert.ToDouble(CommandsWithTimeout.GoogleBigQuery(synchronizeParameters.DatabaseRow.Connection).GetTable(item.Field<string>("schema"), item.Field<string>("name")).Resource.LastModifiedTime);
							value2 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(value);
						}
						else
						{
							value2 = item.Field<DateTime>("modify_date");
						}
						AddDBObject(item, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, value2, owner);
					}
				}
			}
			catch (Exception exception)
			{
				GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
				return false;
			}
		}
		return true;
	}

	public override bool GetRelations(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetColumns(string query, Form owner = null)
	{
		try
		{
			foreach (BigQueryRow item in CommandsWithTimeout.GoogleBigQuery(synchronizeParameters.DatabaseRow.Connection, query))
			{
				AddColumn(item);
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetTriggers(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetParameters(string query, bool canReadParameters = true, Form owner = null)
	{
		if (!canReadParameters)
		{
			return true;
		}
		try
		{
			foreach (BigQueryRow item in CommandsWithTimeout.GoogleBigQuery(synchronizeParameters.DatabaseRow.Connection, query))
			{
				AddParameter(item);
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetDependencies(string query, Form owner = null)
	{
		return true;
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		string tableFilter = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM(t.TABLE_SCHEMA)", "TRIM(t.TABLE_NAME)");
		foreach (string schema in synchronizeParameters.DatabaseRow.Schemas)
		{
			yield return "SELECT 'table' as object_type ,count(1) as count\r\n                                    FROM `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.TABLES t\r\n                                    WHERE table_type = 'BASE TABLE'\r\n                                        " + tableFilter;
		}
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		string viewFilter = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "TRIM(t.TABLE_SCHEMA)", "TRIM(t.TABLE_NAME)");
		foreach (string schema in synchronizeParameters.DatabaseRow.Schemas)
		{
			yield return "SELECT 'view' as object_type ,count(1) as count\r\n                        FROM `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.VIEWS t\r\n                        WHERE 1=1\r\n                            " + viewFilter;
		}
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		string procedureFilter = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "TRIM(isr.specific_schema)", "TRIM(isr.specific_name)");
		foreach (string schema in synchronizeParameters.DatabaseRow.Schemas)
		{
			yield return "select 'procedure' as object_type ,count(1) as count \r\n                                FROM `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.ROUTINES isr             \r\n                                WHERE isr.ROUTINE_TYPE = 'PROCEDURE'                               \r\n                                " + procedureFilter;
		}
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		string functionFilter = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "TRIM(isr.specific_schema)", "TRIM(isr.specific_name)");
		foreach (string schema in synchronizeParameters.DatabaseRow.Schemas)
		{
			yield return "select 'function' as object_type ,count(1) as count \r\n                                FROM `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.ROUTINES isr             \r\n                                WHERE isr.ROUTINE_TYPE = 'FUNCTION'                               \r\n                                " + functionFilter;
		}
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		string tableFilter = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM(t.TABLE_SCHEMA)", "TRIM(t.TABLE_NAME)");
		foreach (string schema in synchronizeParameters.DatabaseRow.Schemas)
		{
			yield return "SELECT t.TABLE_NAME AS name,\r\n                            t.TABLE_SCHEMA AS schema,\r\n                            t.TABLE_CATALOG AS database_name,\r\n                                CASE \r\n                                    WHEN t.TABLE_TYPE = 'EXTERNAL' THEN 'EXTERNAL_TABLE'\r\n                                    ELSE  'TABLE'\r\n                                    END AS type,\r\n                            ts.OPTION_VALUE as description,\r\n                            null as definition,\r\n                            t.CREATION_TIME as create_date,\r\n                            t.TABLE_TYPE as table_type,\r\n                            null as modify_date,\r\n                            null as function_type,\r\n                            tsl.OPTION_VALUE as labels\r\n                        FROM `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.TABLES as t\r\n                        LEFT JOIN `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.TABLE_OPTIONS as ts\r\n                            ON ts.TABLE_NAME = t.TABLE_NAME\r\n                            AND ts.TABLE_SCHEMA = t.TABLE_SCHEMA\r\n                            AND ts.TABLE_CATALOG = t.TABLE_CATALOG\r\n                            AND ts.OPTION_NAME = 'description'\r\n                        LEFT JOIN `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.TABLE_OPTIONS as tsl\r\n                            ON tsl.TABLE_NAME = t.TABLE_NAME\r\n                            AND tsl.TABLE_SCHEMA = t.TABLE_SCHEMA\r\n                            AND tsl.TABLE_CATALOG = t.TABLE_CATALOG\r\n                            AND tsl.OPTION_NAME = 'labels'\r\n                        WHERE(t.TABLE_TYPE = 'BASE TABLE' OR t.TABLE_TYPE = 'EXTERNAL')\r\n                            " + tableFilter + "\r\n                        ";
		}
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		string viewFilter = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "TRIM(t.TABLE_SCHEMA)", "TRIM(t.TABLE_NAME)");
		foreach (string schema in synchronizeParameters.DatabaseRow.Schemas)
		{
			yield return "SELECT t.TABLE_NAME AS name,\r\n                            t.TABLE_SCHEMA AS schema,\r\n                            t.TABLE_CATALOG AS database_name,\r\n                            t.TABLE_TYPE AS type,\r\n                            ts.OPTION_VALUE as description,\r\n                            v.VIEW_DEFINITION as definition,\r\n                            t.CREATION_TIME as create_date,\r\n                            null as modify_date,\r\n                            null as function_type,\r\n                            tsl.OPTION_VALUE as labels\r\n                        FROM `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.TABLES as t\r\n                        LEFT JOIN  `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.TABLE_OPTIONS as ts\r\n                            ON ts.TABLE_NAME = t.TABLE_NAME\r\n                            AND ts.TABLE_SCHEMA = t.TABLE_SCHEMA\r\n                            AND ts.TABLE_CATALOG = t.TABLE_CATALOG\r\n                            AND ts.OPTION_NAME = 'description'\r\n                        LEFT JOIN `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.VIEWS as v \r\n                            ON t.TABLE_NAME = v.TABLE_NAME\r\n                            AND t.TABLE_SCHEMA = v.TABLE_SCHEMA\r\n                            AND t.TABLE_CATALOG = v.TABLE_CATALOG\r\n                        LEFT JOIN  `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.TABLE_OPTIONS as tsl\r\n                            ON tsl.TABLE_NAME = t.TABLE_NAME\r\n                            AND tsl.TABLE_SCHEMA = t.TABLE_SCHEMA\r\n                            AND tsl.TABLE_CATALOG = t.TABLE_CATALOG\r\n                            AND tsl.OPTION_NAME = 'labels'\r\n                        WHERE (t.TABLE_TYPE = 'VIEW' \r\n                          OR t.TABLE_TYPE = 'MATERIALIZED VIEW')\r\n                            " + viewFilter;
		}
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		string procedureFilter = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "TRIM(isr.specific_schema)", "TRIM(isr.specific_name)");
		foreach (string schema in synchronizeParameters.DatabaseRow.Schemas)
		{
			yield return "select \r\n                                isr.specific_name as `name`,\r\n                                isr.specific_schema as `schema`,\r\n                                isr.specific_catalog as `database_name`,\r\n                                'PROCEDURE' as `type`,\r\n                                isro.OPTION_VALUE as `description`,\r\n                                isr.routine_definition as `definition`,\r\n                                isr.created as `create_date`,\r\n                                isr.last_altered as `modify_date`,\r\n                                null as `function_type`,\r\n                                tsl.option_value as `labels`\r\n                                FROM `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.ROUTINES isr\r\n                            LEFT JOIN  `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.ROUTINE_OPTIONS as isro\r\n                                ON isro.specific_name = isr.specific_name\r\n                                AND isro.specific_schema = isr.specific_schema\r\n                                AND isro.specific_catalog = isr.specific_catalog\r\n                                AND isro.OPTION_NAME = 'description'                        \r\n                            LEFT JOIN  `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.TABLE_OPTIONS as tsl\r\n                                ON tsl.TABLE_NAME = isr.specific_name\r\n                                AND tsl.TABLE_SCHEMA = isr.specific_schema\r\n                                AND tsl.TABLE_CATALOG = isr.specific_catalog\r\n                                AND tsl.OPTION_NAME = 'labels'\r\n                            WHERE isr.ROUTINE_TYPE = 'PROCEDURE'\r\n                                " + procedureFilter;
		}
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		string functionFilter = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "TRIM(isr.specific_schema)", "TRIM(isr.specific_name)");
		foreach (string schema in synchronizeParameters.DatabaseRow.Schemas)
		{
			yield return "select \r\n                                isr.specific_name as `name`,\r\n                                isr.specific_schema as `schema`,\r\n                                isr.specific_catalog as `database_name`,\r\n                                'FUNCTION' as `type`,\r\n                                isro.OPTION_VALUE as `description`,\r\n                                isr.routine_definition as `definition`,\r\n                                isr.created as `create_date`,\r\n                                isr.last_altered as `modify_date`,\r\n                                null as `function_type`,\r\n                                tsl.option_value as `labels`\r\n                                FROM `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.ROUTINES isr\r\n                            LEFT JOIN  `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.ROUTINE_OPTIONS as isro\r\n                                ON isro.specific_name = isr.specific_name\r\n                                AND isro.specific_schema = isr.specific_schema\r\n                                AND isro.specific_catalog = isr.specific_catalog\r\n                                AND isro.OPTION_NAME = 'description'                        \r\n                            LEFT JOIN  `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.TABLE_OPTIONS as tsl\r\n                                ON tsl.TABLE_NAME = isr.specific_name\r\n                                AND tsl.TABLE_SCHEMA = isr.specific_schema\r\n                                AND tsl.TABLE_CATALOG = isr.specific_catalog\r\n                                AND tsl.OPTION_NAME = 'labels'\r\n                            WHERE isr.ROUTINE_TYPE = 'FUNCTION'\r\n                                " + functionFilter;
		}
	}

	public override IEnumerable<string> RelationsQuery()
	{
		yield return null;
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		string tableFilter = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM(t.TABLE_SCHEMA)", "TRIM(t.TABLE_NAME)");
		string viewFilter = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "TRIM(t.TABLE_SCHEMA)", "TRIM(t.TABLE_NAME)");
		foreach (string schema in synchronizeParameters.DatabaseRow.Schemas)
		{
			yield return "SELECT\r\n                                c.TABLE_CATALOG AS database_name,\r\n                                c.TABLE_NAME AS table_name,\r\n                                c.TABLE_SCHEMA AS table_schema, REGEXP_EXTRACT(cf.FIELD_PATH, \"[^.]*$\") AS name, cf.FIELD_PATH AS path, SUBSTR(REGEXP_EXTRACT(cf.FIELD_PATH, \"(.*[.])\"), 0, LENGTH(REGEXP_EXTRACT(cf.FIELD_PATH, \"(.*[.])\")) - 1) AS parent, ROW_NUMBER() OVER(PARTITION BY c.TABLE_CATALOG, c.TABLE_SCHEMA, c.TABLE_NAME, SUBSTR(REGEXP_EXTRACT(cf.FIELD_PATH, \"(.*[.])\"), 0, LENGTH(REGEXP_EXTRACT(cf.FIELD_PATH, \"(.*[.])\")) - 1)) AS position, CASE REGEXP_EXTRACT(cf.DATA_TYPE, \"^[a-zA-Z_.+-]+\") WHEN 'STRUCT' THEN 'RECORD'\r\n                                    WHEN 'ARRAY' THEN CASE STRPOS(REGEXP_EXTRACT(cf.DATA_TYPE, \"[<](.*)[>]\"),'<') WHEN 0 THEN cf.DATA_TYPE\r\n                                        ELSE 'RECORD REPEATED'\r\n                                        END\r\n                                    ELSE cf.DATA_TYPE\r\n                                    END AS datatype, CASE REGEXP_EXTRACT(cf.DATA_TYPE, \"^[a-zA-Z_.+-]+\") WHEN 'STRUCT' THEN 'OBJECT'\r\n                                WHEN 'ARRAY' THEN CASE STRPOS(REGEXP_EXTRACT(cf.DATA_TYPE, \"[<](.*)[>]\"), '<') WHEN 0 THEN 'ARRAY'\r\n                                    ELSE 'OBJECT_ARRAY'\r\n                                    END\r\n                                ELSE 'COLUMN'\r\n                                END AS type,\r\n                                cf.DESCRIPTION as description,\r\n                                '' as constraint_type,\r\n                                0 AS is_identity,\r\n                                CASE c.IS_NULLABLE\r\n                                WHEN 'YES' THEN 1\r\n                                ELSE 0\r\n                                END AS nullable,\r\n                                '' AS default_value,\r\n                                0 AS is_computed,\r\n                                null AS computed_formula,\r\n                                null AS data_length,\r\n                                LENGTH(REGEXP_REPLACE(cf.FIELD_PATH, '[^.]', ''))+1 AS level\r\n                            FROM `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.COLUMNS c\r\n                            LEFT JOIN  `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.COLUMN_FIELD_PATHS cf\r\n                            ON c.TABLE_SCHEMA = cf.TABLE_SCHEMA\r\n                                AND c.TABLE_NAME = cf.TABLE_NAME\r\n                                AND c.COLUMN_NAME = cf.COLUMN_NAME\r\n                            LEFT JOIN `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.TABLES t\r\n                                ON c.TABLE_SCHEMA = t.TABLE_SCHEMA\r\n                                AND c.TABLE_NAME = t.TABLE_NAME\r\n                            WHERE 1=1 \r\n                                AND( ((t.TABLE_TYPE = 'BASE TABLE' OR t.TABLE_TYPE = 'EXTERNAL') " + tableFilter + ")\r\n                                OR ((t.TABLE_TYPE = 'VIEW' OR t.TABLE_TYPE = 'MATERIALIZED VIEW') " + viewFilter + "))\r\n                                AND c.is_partitioning_column = 'NO'\r\n                                ";
		}
	}

	public override IEnumerable<string> TriggersQuery()
	{
		yield return null;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		yield return null;
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		string procedureFilterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Procedure, "isp.specific_schema", "isp.specific_name");
		string functionFilterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Function, "isp.specific_schema", "isp.specific_name");
		foreach (string schema in synchronizeParameters.DatabaseRow.Schemas)
		{
			yield return "select \r\n                                    isp.specific_name as `procedure_name`,\r\n                                    isp.specific_schema as `procedure_schema`,\r\n                                    isp.specific_catalog as `database_name`,\r\n                                    isp.parameter_name as `name`,\r\n                                    isp.ordinal_position as `position`,\r\n                                    isp.parameter_mode as `parameter_mode`,\r\n                                    isp.data_type as `datatype`,\r\n                                    null as `description`,\r\n                                    null as `data_length`,\r\n                                    null as `labels`,\r\n                                    is_result as `is_result`\r\n                                    from `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.PARAMETERS isp\r\n                                    join `" + synchronizeParameters.DatabaseRow.Host + "`." + schema + ".INFORMATION_SCHEMA.ROUTINES isr\r\n                                ON isp.specific_name = isr.specific_name\r\n                                    AND isp.specific_schema = isr.specific_schema\r\n                                    AND isp.specific_catalog = isr.specific_catalog\r\n                                WHERE 1 = 1            \r\n                                    and ((isr.routine_type = 'FUNCTION' " + functionFilterString + ") \r\n                                        or(isr.routine_type <> 'FUNCTION' " + procedureFilterString + "))";
		}
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		yield return null;
	}
}
