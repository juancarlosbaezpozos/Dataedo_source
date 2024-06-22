using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.QueryTools;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.LicenseHelperLibrary.Repository;
using Dataedo.Log.Execution;
using Dataedo.Shared.Enums;
using Dataedo.SqlParser.Domain;
using Dataedo.SqlParser.Parsers.PlSql;
using Devart.Data.Oracle;
using Devart.Data.Universal;

namespace Dataedo.App.Data.Oracle;

internal class SynchronizeOracle : SynchronizeDatabase
{
	private readonly PrivilegesHelper privilegesHelper;

	private PlSqlInterpreter interpreter;

	private string _viewsPrefix;

	private string procedureTypeDefinitionQuery;

	private string functionTypeDefinitionQuery;

	private string triggerTypeDefinitionQuery;

	public override IEnumerable<string> CountTablesQuery()
	{
		return new List<string> { string.Empty };
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		return new List<string> { string.Empty };
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		return new List<string> { string.Empty };
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		return new List<string> { string.Empty };
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		return new List<string> { string.Empty };
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		return new List<string> { string.Empty };
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		return new List<string> { string.Empty };
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		return new List<string> { string.Empty };
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "COL.OWNER", "COL.TABLE_NAME");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.View, "COL.OWNER", "COL.TABLE_NAME");
		if (synchronizeParameters.DatabaseRow.GetVersionUpdate().Version >= 12)
		{
			yield return "SELECT COL.OWNER AS DATABASE_NAME\r\n\t\t\t\t    , COL.TABLE_NAME AS TABLE_NAME\r\n\t\t\t\t    , COL.OWNER AS TABLE_SCHEMA\r\n\t\t\t\t    , COL.COLUMN_NAME AS NAME\r\n\t\t\t\t    , COL.COLUMN_ID AS POSITION\r\n\t\t\t\t    , COM.COMMENTS AS DESCRIPTION\r\n\t\t\t\t    , COL.DATA_TYPE AS DATATYPE\r\n\t\t\t\t    , NULL AS CONSTRAINT_TYPE\r\n\t\t\t\t    , CASE \r\n\t\t\t\t\t    WHEN COL.NULLABLE = 'Y'\r\n\t\t\t\t\t\t    THEN 1\r\n\t\t\t\t\t    ELSE 0\r\n\t\t\t\t\t    END AS NULLABLE\r\n\t\t\t\t    , CASE \r\n\t\t\t\t\t    WHEN COL.VIRTUAL_COLUMN = 'YES'\r\n\t\t\t\t\t\t    THEN NULL\r\n\t\t\t\t\t    ELSE COL.DATA_DEFAULT\r\n\t\t\t\t\t    END AS DEFAULT_VALUE\r\n\t\t\t\t    , 0 AS IS_IDENTITY\r\n\t\t\t\t    , CASE \r\n\t\t\t\t\t    WHEN COL.VIRTUAL_COLUMN = 'YES'\r\n\t\t\t\t\t\t    THEN 1\r\n\t\t\t\t\t    ELSE 0\r\n\t\t\t\t\t    END AS IS_COMPUTED\r\n\t\t\t\t    , CASE \r\n\t\t\t\t\t    WHEN COL.VIRTUAL_COLUMN = 'YES'\r\n\t\t\t\t\t\t    THEN COL.DATA_DEFAULT\r\n\t\t\t\t\t    ELSE NULL\r\n\t\t\t\t\t    END AS COMPUTED_FORMULA,\r\n\t\t\t\t\t\t\t    " + QueryDataTypes.GetQueryForDataLength(synchronizeParameters.DatabaseRow.Type, "COL.") + ", CASE WHEN OBJ.OBJECT_TYPE = 'MATERIALIZED VIEW' THEN 'VIEW' ELSE OBJ.OBJECT_TYPE END AS TABLE_TYPE\r\n\t\t\t\t    FROM " + privilegesHelper.GetName("TAB_COLS") + " COL\r\n\t\t\t\t    INNER JOIN " + privilegesHelper.GetName("OBJECTS") + "  OBJ\r\n\t\t\t\t\t    ON COL.OWNER = OBJ.OWNER\r\n\t\t\t\t\t    AND COL.TABLE_NAME = OBJ.OBJECT_NAME\r\n\t\t\t\t\t    AND OBJ.OBJECT_TYPE IN ('TABLE', 'VIEW', 'MATERIALIZED VIEW')\r\n                        AND COL.USER_GENERATED = 'YES'\r\n\t\t\t\t\t    AND COL.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n\t\t\t\t\t    AND ((OBJ.OBJECT_TYPE='TABLE' " + filterString + ") OR (OBJ.OBJECT_TYPE<>'TABLE' " + filterString2 + "))\r\n\t\t\t\t    LEFT JOIN " + privilegesHelper.GetName("COL_COMMENTS") + " COM\r\n\t\t\t\t\t    ON COL.TABLE_NAME = COM.TABLE_NAME\r\n\t\t\t\t\t    AND COL.COLUMN_NAME = COM.COLUMN_NAME\r\n\t\t\t\t\t    AND COL.OWNER = COM.OWNER";
		}
		else
		{
			yield return "SELECT COL.OWNER AS DATABASE_NAME\r\n                    , COL.TABLE_NAME AS TABLE_NAME\r\n                    , COL.OWNER AS TABLE_SCHEMA\r\n                    , COL.COLUMN_NAME AS NAME\r\n                    , COL.COLUMN_ID AS POSITION\r\n                    , COM.COMMENTS AS DESCRIPTION\r\n                    , COL.DATA_TYPE AS DATATYPE\r\n                    , NULL AS CONSTRAINT_TYPE\r\n                    , CASE \r\n                        WHEN COL.NULLABLE = 'Y'\r\n                            THEN 1\r\n                        ELSE 0\r\n                        END AS NULLABLE\r\n                    , CASE \r\n                        WHEN TAB_COLS.VIRTUAL_COLUMN = 'YES'\r\n                            THEN NULL\r\n                        ELSE COL.DATA_DEFAULT\r\n                        END AS DEFAULT_VALUE\r\n                    , 0 AS IS_IDENTITY\r\n                    , CASE \r\n                        WHEN TAB_COLS.VIRTUAL_COLUMN = 'YES'\r\n                            THEN 1\r\n                        ELSE 0\r\n                        END AS IS_COMPUTED\r\n                    , CASE \r\n                        WHEN TAB_COLS.VIRTUAL_COLUMN = 'YES'\r\n                            THEN COL.DATA_DEFAULT\r\n                        ELSE NULL\r\n                        END AS COMPUTED_FORMULA,\r\n                                " + QueryDataTypes.GetQueryForDataLength(synchronizeParameters.DatabaseRow.Type, "COL.") + ", CASE WHEN OBJ.OBJECT_TYPE = 'MATERIALIZED VIEW' THEN 'VIEW' ELSE OBJ.OBJECT_TYPE END AS TABLE_TYPE\r\n                            FROM " + privilegesHelper.GetName("TAB_COLUMNs") + " COL\r\n                    INNER JOIN " + privilegesHelper.GetName("COL_COMMENTS") + " COM\r\n                        ON COL.TABLE_NAME = COM.TABLE_NAME\r\n                            AND COL.COLUMN_NAME = COM.COLUMN_NAME\r\n                            AND COL.OWNER = COM.OWNER\r\n                    INNER JOIN " + privilegesHelper.GetName("TAB_COLS") + " TAB_COLS\r\n                        ON COL.TABLE_NAME = TAB_COLS.TABLE_NAME\r\n                            AND COL.COLUMN_NAME = TAB_COLS.COLUMN_NAME\r\n                            AND COL.OWNER = TAB_COLS.OWNER\r\n                            INNER JOIN " + privilegesHelper.GetName("OBJECTS") + " OBJ\r\n                        ON COL.OWNER = OBJ.OWNER\r\n                        AND COL.TABLE_NAME = OBJ.OBJECT_NAME\r\n                        AND OBJ.OBJECT_TYPE IN ('TABLE', 'VIEW', 'MATERIALIZED VIEW')\r\n                    WHERE COL.TABLE_NAME = COM.TABLE_NAME\r\n                        AND COL.COLUMN_NAME = COM.COLUMN_NAME\r\n                        AND COL.OWNER = COM.OWNER\r\n                        AND COL.TABLE_NAME = TAB_COLS.TABLE_NAME\r\n                        AND COL.COLUMN_NAME = TAB_COLS.COLUMN_NAME\r\n                        AND COL.OWNER = TAB_COLS.OWNER\r\n                        AND COL.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n                        AND ((OBJ.OBJECT_TYPE='TABLE' " + filterString + ") OR (OBJ.OBJECT_TYPE<>'TABLE' " + filterString2 + "))";
		}
	}

	public override List<ImportQuery> CountObjectsQuery()
	{
		List<ImportQuery> list = new List<ImportQuery>();
		List<FilterObjectTypeEnum.FilterObjectType> list2 = synchronizeParameters.DatabaseRow.Filter.GetIncludedTypes(synchronizeParameters.DatabaseRow.Type).ToList();
		if (list2.Contains(FilterObjectTypeEnum.FilterObjectType.Table))
		{
			string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "TAB.OWNER", "TAB.OBJECT_NAME");
			string query = "SELECT TAB.OBJECT_TYPE, COUNT(1) COUNT\r\n                    FROM " + privilegesHelper.GetName("OBJECTS") + " TAB\r\n                            ," + privilegesHelper.GetName("TAB_COMMENTS") + " COM\r\n                    WHERE TAB.OBJECT_NAME = COM.TABLE_NAME(+)\r\n                            AND TAB.OBJECT_TYPE = 'TABLE'\r\n                            AND TAB.OWNER = COM.OWNER (+)\r\n                            AND TAB.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n                        " + filterString + "\r\n                        GROUP BY TAB.OBJECT_TYPE";
			list.Add(new ImportQuery(FilterObjectTypeEnum.FilterObjectType.Table, query));
		}
		if (list2.Contains(FilterObjectTypeEnum.FilterObjectType.View))
		{
			string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "TAB.OWNER", "TAB.OBJECT_NAME");
			string query2 = "SELECT TAB.OBJECT_TYPE, COUNT(1) COUNT\r\n                    /*Should MVIEW be grouped together with VIEW?\r\n                                If so change above to: SELECT 'VIEW', COUNT(1) COUNT\r\n                                and remove GROUP BY TAB.OBJECT_TYPE*/\r\n                    FROM " + privilegesHelper.GetName("OBJECTS") + " TAB\r\n                            ," + privilegesHelper.GetName("TAB_COMMENTS") + " COM\r\n                    WHERE TAB.OBJECT_NAME = COM.TABLE_NAME(+)\r\n                            AND (TAB.OBJECT_TYPE = 'VIEW' OR TAB.OBJECT_TYPE = 'MATERIALIZED VIEW')\r\n                            AND TAB.OWNER = COM.OWNER (+)\r\n                            AND TAB.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n                        " + filterString2 + "\r\n                        GROUP BY TAB.OBJECT_TYPE";
			list.Add(new ImportQuery(FilterObjectTypeEnum.FilterObjectType.View, query2));
		}
		if (list2.Contains(FilterObjectTypeEnum.FilterObjectType.Procedure))
		{
			string filterString3 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "TAB.OWNER", "TAB.OBJECT_NAME");
			string query3 = "SELECT TAB.OBJECT_TYPE, COUNT(1) COUNT\r\n                    FROM " + privilegesHelper.GetName("OBJECTS") + " TAB\r\n                            ," + privilegesHelper.GetName("TAB_COMMENTS") + " COM\r\n                    WHERE TAB.OBJECT_NAME = COM.TABLE_NAME(+)\r\n                            AND TAB.OBJECT_TYPE = 'PROCEDURE'\r\n                            AND TAB.OWNER = COM.OWNER (+)\r\n                            AND TAB.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n                        " + filterString3 + "\r\n                        GROUP BY TAB.OBJECT_TYPE";
			list.Add(new ImportQuery(FilterObjectTypeEnum.FilterObjectType.Procedure, query3));
			string query4 = "SELECT TAB.OBJECT_TYPE, COUNT(1) COUNT\r\n                    FROM " + privilegesHelper.GetName("OBJECTS") + " TAB\r\n                            ," + privilegesHelper.GetName("TAB_COMMENTS") + " COM\r\n                    WHERE TAB.OBJECT_NAME = COM.TABLE_NAME(+)\r\n                            AND TAB.OBJECT_TYPE = 'PACKAGE'\r\n                            AND TAB.OWNER = COM.OWNER (+)\r\n                            AND TAB.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n                        " + filterString3 + "\r\n                        GROUP BY TAB.OBJECT_TYPE";
			list.Add(new ImportQuery(FilterObjectTypeEnum.FilterObjectType.Procedure, query4));
		}
		if (list2.Contains(FilterObjectTypeEnum.FilterObjectType.Function))
		{
			string filterString4 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "TAB.OWNER", "TAB.OBJECT_NAME");
			string query5 = "SELECT TAB.OBJECT_TYPE, COUNT(1) COUNT\r\n                    FROM " + privilegesHelper.GetName("OBJECTS") + " TAB\r\n                            ," + privilegesHelper.GetName("TAB_COMMENTS") + " COM\r\n                    WHERE TAB.OBJECT_NAME = COM.TABLE_NAME(+)\r\n                            AND TAB.OBJECT_TYPE = 'FUNCTION'\r\n                            AND TAB.OWNER = COM.OWNER (+)\r\n                            AND TAB.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n                        " + filterString4 + "\r\n                        GROUP BY TAB.OBJECT_TYPE";
			list.Add(new ImportQuery(FilterObjectTypeEnum.FilterObjectType.Function, query5));
		}
		return list;
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		string filterStringForDependencies = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("OWNER", "NAME", "TYPE", "REFERENCED_TYPE");
		string filterStringForDependencies2 = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("REFERENCED_OWNER", "REFERENCED_NAME", "REFERENCED_TYPE", "TYPE");
		string text = FilterRulesCollection.CombineDependenciesFilter(filterStringForDependencies, filterStringForDependencies2);
		yield return "SELECT DISTINCT\r\n                CASE TYPE\r\n                    WHEN 'MATERIALIZED VIEW' THEN 'VIEW'\r\n                    WHEN 'PACKAGE'  THEN 'PROCEDURE'\r\n                    WHEN 'PACKAGE BODY'  THEN 'PROCEDURE'\r\n                ELSE TYPE\r\n                END AS referencing_type,\r\n                '" + synchronizeParameters.DatabaseRow.Host.ToUpper() + "' as referencing_server,\r\n                OWNER AS referencing_schema_name,\r\n                OWNER as referencing_database_name,\r\n                NAME AS referencing_entity_name,\r\n                NVL(REFERENCED_LINK_NAME, '" + synchronizeParameters.DatabaseRow.Host.ToUpper() + "') as referenced_server,\r\n                REFERENCED_OWNER as referenced_database_name,\r\n                REFERENCED_OWNER as referenced_schema_name,\r\n                CASE REFERENCED_TYPE\r\n                    WHEN 'MATERIALIZED VIEW' THEN 'VIEW'\r\n                    WHEN 'PACKAGE'  THEN 'PROCEDURE'\r\n                    WHEN 'PACKAGE BODY'  THEN 'PROCEDURE'\r\n                ELSE REFERENCED_TYPE\r\n                END as referenced_type,\r\n                REFERENCED_NAME as referenced_entity_name,\r\n                0 as is_caller_dependent,\r\n                0 as is_ambiguous,\r\n                DEPENDENCY_TYPE as dependency_type\r\n            FROM \r\n                " + privilegesHelper.GetName("dependencies") + " \r\n\r\n                WHERE OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ") AND TYPE IN ('TABLE', 'VIEW', 'MATERIALIZED VIEW', 'FUNCTION', 'PROCEDURE', 'TRIGGER', 'PACKAGE', 'PACKAGE BODY')\r\n                    AND REFERENCED_TYPE IN ('TABLE', 'VIEW', 'MATERIALIZED VIEW', 'FUNCTION', 'PROCEDURE', 'TRIGGER', 'PACKAGE', 'PACKAGE BODY')\r\n                            AND NAME IS NOT NULL\r\n                            AND REFERENCED_NAME IS NOT NULL" + text;
	}

	public override List<ImportQuery> GetObjectsQuery()
	{
		List<ImportQuery> list = new List<ImportQuery>();
		List<FilterObjectTypeEnum.FilterObjectType> list2 = synchronizeParameters.DatabaseRow.Filter.GetIncludedTypes(synchronizeParameters.DatabaseRow.Type).ToList();
		if (list2.Contains(FilterObjectTypeEnum.FilterObjectType.Table))
		{
			string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "OBJ.OWNER", "OBJ.OBJECT_NAME");
			string query = "SELECT OBJ.OBJECT_NAME AS NAME\r\n                                   , OBJ.OWNER AS SCHEMA\r\n                                   , OBJ.OBJECT_TYPE AS TYPE\r\n                                   , NULL AS DEFINITION\r\n                                   , COM.COMMENTS AS DESCRIPTION\r\n                                   , OBJ.CREATED AS CREATE_DATE\r\n                                   , GREATEST(OBJ.LAST_DDL_TIME, NVL(IND.LAST_DDL_TIME, OBJ.LAST_DDL_TIME)) AS MODIFY_DATE\r\n                              FROM " + privilegesHelper.GetName("OBJECTS") + " OBJ\r\n                              LEFT JOIN " + privilegesHelper.GetName("TAB_COMMENTS") + " COM\r\n                                   ON OBJ.OBJECT_NAME = COM.TABLE_NAME\r\n                                        AND OBJ.OWNER = COM.OWNER\r\n                              LEFT JOIN (\r\n                                   SELECT I.OWNER\r\n                                        ,I.TABLE_NAME\r\n                                        , MAX(TABIND.LAST_DDL_TIME) LAST_DDL_TIME\r\n                                   FROM " + privilegesHelper.GetName("OBJECTS") + " TABIND\r\n                                   JOIN " + privilegesHelper.GetName("INDEXES") + " I\r\n                                        ON TABIND.OBJECT_NAME = I.INDEX_NAME\r\n                                             AND TABIND.OWNER = I.OWNER\r\n                                   WHERE TABIND.OBJECT_TYPE IN ('INDEX')\r\n                                        AND I.UNIQUENESS = 'UNIQUE'\r\n                                        AND TABIND.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n                                   GROUP BY I.OWNER\r\n                                        ,I.TABLE_NAME\r\n                                   ) IND\r\n                                   ON OBJ.OBJECT_NAME = IND.TABLE_NAME\r\n                                        AND OBJ.OWNER = IND.OWNER\r\n                              WHERE OBJ.OBJECT_TYPE IN ('TABLE')\r\n                                   AND OBJ.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n                                   AND (OBJ.OBJECT_TYPE = 'TABLE' " + filterString + ")\r\n                              ORDER BY OBJ.OBJECT_NAME";
			list.Add(new ImportQuery(FilterObjectTypeEnum.FilterObjectType.Table, query));
		}
		if (list2.Contains(FilterObjectTypeEnum.FilterObjectType.View))
		{
			bool num = CheckIfHasView(privilegesHelper.GetName("MVIEW_COMMENTS") ?? "");
			string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "OBJ.OWNER", "OBJ.OBJECT_NAME");
			string text = "";
			text = ((!num) ? ("SELECT OBJ.OBJECT_NAME AS NAME\r\n                                   , OBJ.OWNER AS SCHEMA\r\n                                   , OBJ.OBJECT_TYPE AS TYPE\r\n                            , CASE OBJ.OBJECT_TYPE \r\n                                             WHEN 'VIEW' THEN " + privilegesHelper.GetName("VIEWS") + ".TEXT\r\n                                             WHEN 'MATERIALIZED VIEW' THEN " + privilegesHelper.GetName("MVIEWS") + ".QUERY\r\n                                             END AS DEFINITION\r\n                                   , COM.COMMENTS AS DESCRIPTION\r\n                                   , OBJ.CREATED AS CREATE_DATE\r\n                                   , GREATEST(OBJ.LAST_DDL_TIME, NVL(IND.LAST_DDL_TIME, OBJ.LAST_DDL_TIME)) AS MODIFY_DATE\r\n                              FROM " + privilegesHelper.GetName("OBJECTS") + " OBJ\r\n                              LEFT JOIN " + privilegesHelper.GetName("TAB_COMMENTS") + " COM\r\n                                   ON OBJ.OBJECT_NAME = COM.TABLE_NAME\r\n                                        AND OBJ.OWNER = COM.OWNER\r\n                              LEFT JOIN (\r\n                                   SELECT I.OWNER\r\n                                        ,I.TABLE_NAME\r\n                                        , MAX(TABIND.LAST_DDL_TIME) LAST_DDL_TIME\r\n                                   FROM " + privilegesHelper.GetName("OBJECTS") + " TABIND\r\n                                   JOIN " + privilegesHelper.GetName("INDEXES") + " I\r\n                                        ON TABIND.OBJECT_NAME = I.INDEX_NAME\r\n                                             AND TABIND.OWNER = I.OWNER\r\n                                   WHERE TABIND.OBJECT_TYPE IN ('INDEX')\r\n                                        AND I.UNIQUENESS = 'UNIQUE'\r\n                                        AND TABIND.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n                                   GROUP BY I.OWNER\r\n                                        ,I.TABLE_NAME\r\n                                   ) IND\r\n                                   ON OBJ.OBJECT_NAME = IND.TABLE_NAME\r\n                                        AND OBJ.OWNER = IND.OWNER\r\n                              LEFT JOIN " + privilegesHelper.GetName("VIEWS") + "\r\n                                   ON OBJ.OBJECT_NAME = " + privilegesHelper.GetName("VIEWS") + ".VIEW_NAME\r\n                                        AND OBJ.OWNER = " + privilegesHelper.GetName("VIEWS") + ".OWNER\r\n                                AND OBJ.OBJECT_TYPE='VIEW'\r\n                        LEFT JOIN " + privilegesHelper.GetName("MVIEWS") + "\r\n                                   ON OBJ.OBJECT_NAME = " + privilegesHelper.GetName("MVIEWS") + ".MVIEW_NAME\r\n                                        AND OBJ.OWNER = " + privilegesHelper.GetName("MVIEWS") + ".OWNER\r\n                                AND OBJ.OBJECT_TYPE='MATERIALIZED VIEW'\r\n                              WHERE OBJ.OBJECT_TYPE IN ('VIEW', 'MATERIALIZED VIEW')\r\n                                   AND OBJ.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n                                   AND ((OBJ.OBJECT_TYPE = 'VIEW' OR OBJ.OBJECT_TYPE = 'MATERIALIZED VIEW') " + filterString2 + ")\r\n                              ORDER BY OBJ.OBJECT_NAME") : ("SELECT OBJ.OBJECT_NAME AS NAME\r\n                                   , OBJ.OWNER AS SCHEMA\r\n                                   , OBJ.OBJECT_TYPE AS TYPE\r\n                            , CASE OBJ.OBJECT_TYPE \r\n                                             WHEN 'VIEW' THEN " + privilegesHelper.GetName("VIEWS") + ".TEXT\r\n                                             WHEN 'MATERIALIZED VIEW' THEN " + privilegesHelper.GetName("MVIEWS") + ".QUERY\r\n                                             END AS DEFINITION\r\n                                   , CASE OBJ.OBJECT_TYPE \r\n                                             WHEN 'MATERIALIZED VIEW' THEN MVC.COMMENTS\r\n                                             ELSE COM.COMMENTS\r\n                                             END AS DESCRIPTION\r\n                                   , OBJ.CREATED AS CREATE_DATE\r\n                                   , GREATEST(OBJ.LAST_DDL_TIME, NVL(IND.LAST_DDL_TIME, OBJ.LAST_DDL_TIME)) AS MODIFY_DATE\r\n                              FROM " + privilegesHelper.GetName("OBJECTS") + " OBJ\r\n                              LEFT JOIN " + privilegesHelper.GetName("TAB_COMMENTS") + " COM\r\n                                   ON OBJ.OBJECT_NAME = COM.TABLE_NAME\r\n                                        AND OBJ.OWNER = COM.OWNER\r\n                              LEFT JOIN " + privilegesHelper.GetName("MVIEW_COMMENTS") + " MVC\r\n                                   ON OBJ.OBJECT_NAME = MVC.MVIEW_NAME\r\n                                        AND OBJ.OWNER = MVC.OWNER\r\n                              LEFT JOIN (\r\n                                   SELECT I.OWNER\r\n                                        ,I.TABLE_NAME\r\n                                        , MAX(TABIND.LAST_DDL_TIME) LAST_DDL_TIME\r\n                                   FROM " + privilegesHelper.GetName("OBJECTS") + " TABIND\r\n                                   JOIN " + privilegesHelper.GetName("INDEXES") + " I\r\n                                        ON TABIND.OBJECT_NAME = I.INDEX_NAME\r\n                                             AND TABIND.OWNER = I.OWNER\r\n                                   WHERE TABIND.OBJECT_TYPE IN ('INDEX')\r\n                                        AND I.UNIQUENESS = 'UNIQUE'\r\n                                        AND TABIND.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n                                   GROUP BY I.OWNER\r\n                                        ,I.TABLE_NAME\r\n                                   ) IND\r\n                                   ON OBJ.OBJECT_NAME = IND.TABLE_NAME\r\n                                        AND OBJ.OWNER = IND.OWNER\r\n                              LEFT JOIN " + privilegesHelper.GetName("VIEWS") + "\r\n                                   ON OBJ.OBJECT_NAME = " + privilegesHelper.GetName("VIEWS") + ".VIEW_NAME\r\n                                        AND OBJ.OWNER = " + privilegesHelper.GetName("VIEWS") + ".OWNER\r\n                                AND OBJ.OBJECT_TYPE='VIEW'\r\n                        LEFT JOIN " + privilegesHelper.GetName("MVIEWS") + "\r\n                                   ON OBJ.OBJECT_NAME = " + privilegesHelper.GetName("MVIEWS") + ".MVIEW_NAME\r\n                                        AND OBJ.OWNER = " + privilegesHelper.GetName("MVIEWS") + ".OWNER\r\n                                AND OBJ.OBJECT_TYPE='MATERIALIZED VIEW'\r\n                              WHERE OBJ.OBJECT_TYPE IN ('VIEW', 'MATERIALIZED VIEW')\r\n                                   AND OBJ.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n                                   AND ((OBJ.OBJECT_TYPE = 'VIEW' OR OBJ.OBJECT_TYPE = 'MATERIALIZED VIEW') " + filterString2 + ")\r\n                              ORDER BY OBJ.OBJECT_NAME"));
			list.Add(new ImportQuery(FilterObjectTypeEnum.FilterObjectType.View, text));
		}
		if (list2.Contains(FilterObjectTypeEnum.FilterObjectType.Procedure))
		{
			string filterString3 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "OBJ.OWNER", "OBJ.OBJECT_NAME");
			string query2 = "SELECT OBJ.OBJECT_NAME AS NAME\r\n                                   , OBJ.OWNER AS SCHEMA\r\n                                   , OBJ.OBJECT_TYPE AS TYPE\r\n                                   , NULL AS DEFINITION\r\n                                   , COM.COMMENTS AS DESCRIPTION\r\n                                   , OBJ.CREATED AS CREATE_DATE\r\n                                   , GREATEST(OBJ.LAST_DDL_TIME, NVL(IND.LAST_DDL_TIME, OBJ.LAST_DDL_TIME)) AS MODIFY_DATE\r\n                              FROM " + privilegesHelper.GetName("OBJECTS") + " OBJ\r\n                              LEFT JOIN " + privilegesHelper.GetName("TAB_COMMENTS") + " COM\r\n                                   ON OBJ.OBJECT_NAME = COM.TABLE_NAME\r\n                                        AND OBJ.OWNER = COM.OWNER\r\n                              LEFT JOIN (\r\n                                   SELECT I.OWNER\r\n                                        ,I.TABLE_NAME\r\n                                        , MAX(TABIND.LAST_DDL_TIME) LAST_DDL_TIME\r\n                                   FROM " + privilegesHelper.GetName("OBJECTS") + " TABIND\r\n                                   JOIN " + privilegesHelper.GetName("INDEXES") + " I\r\n                                        ON TABIND.OBJECT_NAME = I.INDEX_NAME\r\n                                             AND TABIND.OWNER = I.OWNER\r\n                                   WHERE TABIND.OBJECT_TYPE IN ('INDEX')\r\n                                        AND I.UNIQUENESS = 'UNIQUE'\r\n                                        AND TABIND.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n                                   GROUP BY I.OWNER\r\n                                        ,I.TABLE_NAME\r\n                                   ) IND\r\n                                   ON OBJ.OBJECT_NAME = IND.TABLE_NAME\r\n                                        AND OBJ.OWNER = IND.OWNER\r\n                              WHERE OBJ.OBJECT_TYPE IN ('PROCEDURE', 'PACKAGE')\r\n                                   AND OBJ.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n                                   AND ((OBJ.OBJECT_TYPE = 'PROCEDURE' OR OBJ.OBJECT_TYPE = 'PACKAGE') " + filterString3 + ")\r\n                              ORDER BY OBJ.OBJECT_NAME";
			list.Add(new ImportQuery(FilterObjectTypeEnum.FilterObjectType.Procedure, query2));
		}
		if (list2.Contains(FilterObjectTypeEnum.FilterObjectType.Function))
		{
			string filterString4 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "OBJ.OWNER", "OBJ.OBJECT_NAME");
			string query3 = "SELECT OBJ.OBJECT_NAME AS NAME\r\n                                   , OBJ.OWNER AS SCHEMA\r\n                                   , OBJ.OBJECT_TYPE AS TYPE\r\n                                   , NULL AS DEFINITION\r\n                                   , COM.COMMENTS AS DESCRIPTION\r\n                                   , OBJ.CREATED AS CREATE_DATE\r\n                                   , GREATEST(OBJ.LAST_DDL_TIME, NVL(IND.LAST_DDL_TIME, OBJ.LAST_DDL_TIME)) AS MODIFY_DATE\r\n                              FROM " + privilegesHelper.GetName("OBJECTS") + " OBJ\r\n                              LEFT JOIN " + privilegesHelper.GetName("TAB_COMMENTS") + " COM\r\n                                   ON OBJ.OBJECT_NAME = COM.TABLE_NAME\r\n                                        AND OBJ.OWNER = COM.OWNER\r\n                              LEFT JOIN (\r\n                                   SELECT I.OWNER\r\n                                        ,I.TABLE_NAME\r\n                                        , MAX(TABIND.LAST_DDL_TIME) LAST_DDL_TIME\r\n                                   FROM " + privilegesHelper.GetName("OBJECTS") + " TABIND\r\n                                   JOIN " + privilegesHelper.GetName("INDEXES") + " I\r\n                                        ON TABIND.OBJECT_NAME = I.INDEX_NAME\r\n                                             AND TABIND.OWNER = I.OWNER\r\n                                   WHERE TABIND.OBJECT_TYPE IN ('INDEX')\r\n                                        AND I.UNIQUENESS = 'UNIQUE'\r\n                                        AND TABIND.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n                                   GROUP BY I.OWNER\r\n                                        ,I.TABLE_NAME\r\n                                   ) IND\r\n                                   ON OBJ.OBJECT_NAME = IND.TABLE_NAME\r\n                                        AND OBJ.OWNER = IND.OWNER\r\n                              WHERE OBJ.OBJECT_TYPE IN ('FUNCTION')\r\n                                   AND OBJ.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")\r\n                                   AND (OBJ.OBJECT_TYPE = 'FUNCTION' " + filterString4 + ")\r\n                              ORDER BY OBJ.OBJECT_NAME";
			list.Add(new ImportQuery(FilterObjectTypeEnum.FilterObjectType.Function, query3));
		}
		return list;
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Procedure, "A.OWNER", "A.OBJECT_NAME");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Function, "A.OWNER", "A.OBJECT_NAME");
		yield return "SELECT A.OWNER AS DATABASE_NAME,\r\n                                        A.OBJECT_NAME AS PROCEDURE_NAME,\r\n                                        A.OWNER AS PROCEDURE_SCHEMA,\r\n                                        A.ARGUMENT_NAME AS NAME,\r\n                                        A.POSITION AS POSITION,\r\n                                        A.IN_OUT AS PARAMETER_MODE,\r\n                                        A.DATA_TYPE AS DATATYPE,\r\n                                        " + QueryDataTypes.GetQueryForDataLength(synchronizeParameters.DatabaseRow.Type) + "\r\n                                        ,NULL AS DESCRIPTION\r\n                                    FROM " + privilegesHelper.GetName("ARGUMENTS") + " A\r\n                                    INNER JOIN " + privilegesHelper.GetName("OBJECTS") + " O ON O.OBJECT_ID = A.OBJECT_ID\r\n                                    WHERE DATA_LEVEL = 0 \r\n                                       AND PACKAGE_NAME IS NULL  --Parameters that are not in package only.\r\n                                       AND ((O.OBJECT_TYPE='FUNCTION' " + filterString2 + ") OR (O.OBJECT_TYPE='PROCEDURE' " + filterString + "))\r\n                                       AND A.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")";
	}

	public override IEnumerable<string> RelationsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString(null, base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "C.OWNER", "C.TABLE_NAME");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("OR", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "B.OWNER", "B.TABLE_NAME");
		string text = FilterRulesCollection.CombineRelationsFilter(filterString, filterString2);
		yield return "SELECT\r\n                        A.CONSTRAINT_NAME AS NAME,\r\n                        B.OWNER AS FK_TABLE_DATABASE_NAME,\r\n                        B.TABLE_NAME AS FK_TABLE_NAME,\r\n                        " + synchronizeParameters.DatabaseRow.HasMultipleSchemasForCondition + " AS FK_DATABASE_MULTIPLE_SCHEMAS,\r\n                        B.OWNER AS FK_TABLE_SCHEMA,\r\n                        B.COLUMN_NAME AS FK_COLUMN,\r\n                        B.POSITION AS ORDINAL_POSITION,\r\n                        C.OWNER AS REF_TABLE_DATABASE_NAME,\r\n                        " + synchronizeParameters.DatabaseRow.HasMultipleSchemasForCondition + " AS REF_DATABASE_MULTIPLE_SCHEMAS,\r\n                        C.TABLE_NAME AS REF_TABLE_NAME,\r\n                        C.OWNER AS REF_TABLE_SCHEMA,\r\n                        C.COLUMN_NAME AS REF_COLUMN,\r\n                        NULL AS DESCRIPTION,\r\n                        NULL AS UPDATE_RULE,\r\n                        A.DELETE_RULE\r\n                    FROM\r\n                        " + privilegesHelper.GetName("CONS_COLUMNS") + " B,\r\n                        " + privilegesHelper.GetName("CONS_COLUMNS") + " C,\r\n                        " + privilegesHelper.GetName("CONSTRAINTS") + " A\r\n                    WHERE B.CONSTRAINT_NAME = A.CONSTRAINT_NAME\r\n                        AND A.OWNER           = B.OWNER\r\n                        AND B.POSITION        = C.POSITION\r\n                        AND C.CONSTRAINT_NAME = A.R_CONSTRAINT_NAME\r\n                        AND C.OWNER           = A.R_OWNER\r\n                        AND A.CONSTRAINT_TYPE = 'R'\r\n                        AND (B.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ") \r\n                            OR C.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")) \r\n                        " + text;
	}

	public override IEnumerable<string> TriggersQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "TR.OWNER", "TR.TABLE_NAME");
		yield return "SELECT\r\n                         'TR' AS TYPE,\r\n                        TR.TRIGGER_NAME AS TRIGGER_NAME,\r\n                        TR.OWNER AS TABLE_SCHEMA,\r\n                        TR.TABLE_NAME AS TABLE_NAME,\r\n                        TR.OWNER AS TABLE_SCHEMA,\r\n                        TR.OWNER AS DATABASE_NAME,\r\n                        CASE WHEN TR.TRIGGERING_EVENT LIKE '%UPDATE%' THEN 1 ELSE 0 END AS ISUPDATE,\r\n                        CASE WHEN TR.TRIGGERING_EVENT LIKE '%DELETE%' THEN 1 ELSE 0 END AS ISDELETE,\r\n                        CASE WHEN TR.TRIGGERING_EVENT LIKE '%INSERT%' THEN 1 ELSE 0 END AS ISINSERT,\r\n                        CASE WHEN TR.TRIGGER_TYPE IN ('BEFORE STATEMENT','BEFORE EACH ROW') THEN 1 ELSE 0 END AS ISBEFORE,\r\n                        CASE WHEN TR.TRIGGER_TYPE IN ('AFTER STATEMENT','AFTER EACH ROW') THEN 1 ELSE 0 END AS ISAFTER,\r\n                        CASE WHEN TR.TRIGGER_TYPE = 'INSTEAD OF' THEN 1 ELSE 0 END AS ISINSTEADOF,\r\n                        CASE TR.STATUS WHEN 'ENABLED' THEN 0 ELSE 1 END AS DISABLED,\r\n                        NULL AS DEFINITION,\r\n                        NULL AS DESCRIPTION\r\n                        FROM " + privilegesHelper.GetName("TRIGGERS") + " TR\r\n                        WHERE TR.TABLE_NAME IS NOT NULL\r\n                        AND TR.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")" + filterString;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "I.OWNER", "I.TABLE_NAME");
		string secondFilterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "CON.OWNER", "CON.TABLE_NAME");
		yield return "SELECT I.OWNER  AS DATABASE_NAME,\r\n                                        I.TABLE_NAME  AS TABLE_NAME,\r\n                                        I.OWNER AS TABLE_SCHEMA,\r\n                                        IC.DESCEND,\r\n                                        AIE.COLUMN_EXPRESSION  AS COLUMN_NAME_EXP,\r\n                                        IC.COLUMN_NAME  AS COLUMN_NAME,\r\n                                        NVL(CN.CONSTRAINT_NAME, I.INDEX_NAME)  AS NAME,\r\n                                        NVL(CN.CONSTRAINT_TYPE, 'U') AS TYPE,\r\n                                        IC.COLUMN_POSITION AS COLUMN_ORDINAL,\r\n                                        NULL AS DESCRIPTION,\r\n                                        0 AS DISABLED\r\n                                        FROM " + privilegesHelper.GetName("INDEXES") + " I\r\n                                        JOIN " + privilegesHelper.GetName("IND_COLUMNS") + " IC\r\n                                          ON I.INDEX_NAME = IC.INDEX_NAME\r\n                                          AND I.OWNER = IC.INDEX_OWNER\r\n                                          AND I.TABLE_NAME = IC.TABLE_NAME\r\n                                        LEFT JOIN " + privilegesHelper.GetName("CONSTRAINTS") + " CN  ON CN.INDEX_NAME = I.INDEX_NAME\r\n                                        LEFT JOIN " + privilegesHelper.GetName("IND_EXPRESSIONS") + " AIE\r\n                                          ON IC.INDEX_NAME = AIE.INDEX_NAME\r\n                                          AND IC.INDEX_OWNER = AIE.INDEX_OWNER\r\n                                          AND IC.COLUMN_POSITION = AIE.COLUMN_POSITION\r\n                                        WHERE (I.UNIQUENESS = 'UNIQUE' OR CN.STATUS IS NOT NULL)\r\n                                          AND I.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ") " + filterString;
		yield return "SELECT CON.OWNER  AS DATABASE_NAME,\r\n                                        CON.TABLE_NAME  AS TABLE_NAME,\r\n                                        CON.OWNER AS TABLE_SCHEMA,\r\n                                        NULL AS DESCEND,\r\n                                        NULL  AS COLUMN_NAME_EXP,\r\n                                        COL.COLUMN_NAME  AS COLUMN_NAME,\r\n                                        CON.CONSTRAINT_NAME AS NAME,\r\n                                        CON.CONSTRAINT_TYPE AS TYPE,\r\n                                        COL.POSITION AS COLUMN_ORDINAL,\r\n                                        NULL AS DESCRIPTION,\r\n                                        1 AS DISABLED\r\n                                        FROM " + privilegesHelper.GetName("CONSTRAINTS") + " CON\r\n                                        INNER JOIN " + privilegesHelper.GetName("CONS_COLUMNS") + " COL\r\n                                        ON  CON.CONSTRAINT_NAME = COL.CONSTRAINT_NAME\r\n                                        AND CON.OWNER = COL.OWNER\r\n                                        WHERE CON.STATUS = 'DISABLED' AND CON.CONSTRAINT_TYPE IN ('P','U')\r\n                                        AND CON.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ") " + secondFilterString;
	}

	private bool CheckIfHasView(string viewName)
	{
		return (CommandsWithTimeout.Oracle("SELECT COUNT(*) FROM ALL_VIEWS WHERE VIEW_NAME = '" + viewName + "'", synchronizeParameters.DatabaseRow.Connection).ExecuteScalar() as int?).GetValueOrDefault() > 0;
	}

	public SynchronizeOracle(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
		privilegesHelper = new PrivilegesHelper(synchronizeParameters.DatabaseRow.Connection);
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Counting objects to import");
		if (!string.IsNullOrWhiteSpace(query.Query))
		{
			try
			{
				using UniCommand uniCommand = CommandsWithTimeout.Oracle(query.Query, synchronizeParameters.DatabaseRow.Connection);
				using UniDataReader uniDataReader = uniCommand.ExecuteReader();
				while (uniDataReader.Read())
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					ReadCount(uniDataReader);
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
					using UniCommand uniCommand = CommandsWithTimeout.Oracle(query.Query, synchronizeParameters.DatabaseRow.Connection);
					using UniDataReader uniDataReader = uniCommand.ExecuteReader();
					while (uniDataReader.Read())
					{
						if (synchronizeParameters.DbSynchLocker.IsCanceled)
						{
							return false;
						}
						AddDBObject(uniDataReader, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
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

	public override bool GetProcedureDefinitions(BackgroundWorkerManager backgroundWorkerManager)
	{
		IEnumerable<ObjectRow> enumerable = synchronizeParameters.DatabaseRow.tableRows.Where((ObjectRow x) => x.Type == SharedObjectTypeEnum.ObjectType.Procedure || x.Type == SharedObjectTypeEnum.ObjectType.Function);
		if (enumerable.Any())
		{
			try
			{
				if (!GetDefinitionWithFormattingForAllProcedureObjects(new ObservableCollection<ObjectRow>(enumerable), backgroundWorkerManager))
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				OracleException obj = ex.InnerException as OracleException;
				if (obj == null || obj.Code != 1013)
				{
					foreach (ObjectRow item in enumerable.Where((ObjectRow x) => x.Definition != null))
					{
						item.Definition = null;
					}
				}
				return false;
			}
		}
		HandlePackages(backgroundWorkerManager);
		return true;
	}

	public override void PrepareQueriesForGettingProcedureDefinitions()
	{
		ExecutionLog.StartStopwatch();
		List<FilterObjectTypeEnum.FilterObjectType> list = synchronizeParameters.DatabaseRow.Filter.GetIncludedTypes(synchronizeParameters.DatabaseRow.Type).ToList();
		List<string> list2 = new List<string>();
		if (list.Contains(FilterObjectTypeEnum.FilterObjectType.Procedure))
		{
			string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "OWNER", "NAME");
			string item = "SELECT TEXT, NAME, TYPE, OWNER, LINE\r\n                                                       FROM " + privilegesHelper.GetName("SOURCE") + "\r\n                                                       WHERE\r\n                                                       OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ") " + filterString + "AND TYPE = 'PROCEDURE' AND OWNER = '{0}' AND NAME = '{1}' ";
			string item2 = "SELECT TEXT, NAME, TYPE, OWNER, LINE\r\n                                                     FROM " + privilegesHelper.GetName("SOURCE") + "\r\n                                                     where \r\n                                                     OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")" + filterString + "AND TYPE = 'PACKAGE'AND OWNER = '{0}' AND NAME = '{1}' ";
			string item3 = "SELECT TEXT, NAME, TYPE, OWNER, LINE\r\n                                                         FROM " + privilegesHelper.GetName("SOURCE") + "\r\n                                                         where \r\n                                                         OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")" + filterString + "AND TYPE = 'PACKAGE BODY' AND OWNER = '{0}' AND NAME = '{1}' ";
			list2.Add(item);
			list2.Add(item2);
			list2.Add(item3);
		}
		procedureTypeDefinitionQuery = string.Join(Environment.NewLine + "UNION ALL" + Environment.NewLine, list2) + "ORDER BY TYPE, NAME, OWNER, LINE";
		List<string> list3 = new List<string>();
		if (list.Contains(FilterObjectTypeEnum.FilterObjectType.Function))
		{
			string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "OWNER", "NAME");
			string item4 = "SELECT TEXT, NAME, TYPE, OWNER, LINE\r\n                                                      FROM " + privilegesHelper.GetName("SOURCE") + "\r\n                                                      WHERE\r\n                                                      OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ") " + filterString2 + "AND TYPE = 'FUNCTION'AND OWNER = '{0}' AND NAME = '{1}' ";
			list3.Add(item4);
		}
		functionTypeDefinitionQuery = string.Join(Environment.NewLine + "UNION ALL" + Environment.NewLine, list3) + "ORDER BY TYPE, NAME, OWNER, LINE";
	}

	public override void GetDefinitionForProcedureObject(ObjectRow row, BackgroundWorkerManager backgroundWorkerManager)
	{
		try
		{
			if (procedureTypeDefinitionQuery == null || functionTypeDefinitionQuery == null)
			{
				throw new InvalidOperationException("Queries for getting definitions are not set.");
			}
			string format = null;
			if (row.Type == SharedObjectTypeEnum.ObjectType.Procedure)
			{
				format = procedureTypeDefinitionQuery;
			}
			else if (row.Type == SharedObjectTypeEnum.ObjectType.Function)
			{
				format = functionTypeDefinitionQuery;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			using (UniCommand uniCommand = CommandsWithTimeout.Oracle(string.Format(format, LicenseHelper.EscapeInvalidCharacters(row.Schema), LicenseHelper.EscapeInvalidCharacters(row.Name)), synchronizeParameters.DatabaseRow.Connection))
			{
				using UniDataReader uniDataReader = uniCommand.ExecuteReader();
				while (uniDataReader.Read())
				{
					object obj = ((uniDataReader["TYPE"].Equals("PACKAGE") || uniDataReader["TYPE"].Equals("PACKAGE BODY")) ? "PROCEDURE" : uniDataReader["TYPE"]);
					if (!flag && uniDataReader["TYPE"].Equals("PACKAGE BODY"))
					{
						flag = true;
						stringBuilder.AppendLine();
					}
					if (row == null || !row.Name.Equals(uniDataReader["NAME"]) || !row.Schema.Equals(uniDataReader["OWNER"]) || !row.TypeAsString.Equals(obj))
					{
						if (row.Name.Equals(uniDataReader["NAME"]) && row.Schema.Equals(uniDataReader["OWNER"]) && row.TypeAsString.Equals(obj))
						{
							row.Definition = stringBuilder.ToString();
						}
						stringBuilder = new StringBuilder();
					}
					stringBuilder.Append(uniDataReader["TEXT"].ToString());
				}
			}
			if (row != null && row.Definition == null)
			{
				row.Definition = stringBuilder.ToString();
			}
		}
		catch (Exception)
		{
			throw;
		}
	}

	private bool GetDefinitionWithFormattingForAllProcedureObjects(ObservableCollection<ObjectRow> rows, BackgroundWorkerManager backgroundWorkerManager)
	{
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		try
		{
			ObjectRow objectRow = null;
			StringBuilder stringBuilder = new StringBuilder();
			backgroundWorkerManager.ReportProgress("Retrieving definitions");
			string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "OWNER", "NAME");
			string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "OWNER", "NAME");
			string commandText = "SELECT TEXT, NAME, TYPE, OWNER, LINE\r\n                                              FROM " + privilegesHelper.GetName("SOURCE") + "\r\n                                              WHERE\r\n                                              OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ") " + filterString + "AND TYPE = 'PROCEDURE' UNION ALL SELECT TEXT, NAME, TYPE, OWNER, LINE\r\n                                              FROM " + privilegesHelper.GetName("SOURCE") + "\r\n                                              WHERE\r\n                                              OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ") " + filterString2 + "AND TYPE = 'FUNCTION' UNION ALL SELECT TEXT, NAME, TYPE, OWNER, LINE\r\n                                            FROM " + privilegesHelper.GetName("SOURCE") + "\r\n                                            where \r\n                                            OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")" + filterString + "AND TYPE = 'PACKAGE' UNION ALL SELECT TEXT, NAME, TYPE, OWNER, LINE\r\n                                            FROM " + privilegesHelper.GetName("SOURCE") + "\r\n                                            where \r\n                                            OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ")" + filterString + "AND TYPE = 'PACKAGE BODY' ORDER BY TYPE, NAME, OWNER, LINE";
			bool flag = false;
			using (UniCommand uniCommand = CommandsWithTimeout.Oracle(commandText, synchronizeParameters.DatabaseRow.Connection))
			{
				UniDataReader dr = uniCommand.ExecuteReader();
				try
				{
					while (dr.Read())
					{
						object type = ((dr["TYPE"].Equals("PACKAGE") || dr["TYPE"].Equals("PACKAGE BODY")) ? "PROCEDURE" : dr["TYPE"]);
						if (!flag && dr["TYPE"].Equals("PACKAGE BODY"))
						{
							flag = true;
							stringBuilder.AppendLine();
						}
						if (objectRow == null || !objectRow.Name.Equals(dr["NAME"]) || !objectRow.Schema.Equals(dr["OWNER"]) || !objectRow.TypeAsString.Equals(type))
						{
							if (objectRow != null)
							{
								objectRow.Definition = stringBuilder.ToString();
							}
							stringBuilder = new StringBuilder();
							objectRow = rows.Where((ObjectRow x) => x.Name.Equals(dr["NAME"]) && x.Schema.Equals(dr["OWNER"]) && x.TypeAsString.Equals(type)).FirstOrDefault();
						}
						stringBuilder.Append(dr["TEXT"].ToString());
					}
				}
				finally
				{
					if (dr != null)
					{
						((IDisposable)dr).Dispose();
					}
				}
			}
			if (objectRow != null)
			{
				objectRow.Definition = stringBuilder.ToString();
				backgroundWorkerManager.ReportProgress(new List<string> { "Retrieving definitions" });
			}
		}
		finally
		{
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION GetDefinitionWithFormattingForAllProcedureObjects", null, null, 2, 1);
		}
		return true;
	}

	public override bool GetRelations(string query, Form owner = null)
	{
		try
		{
			using UniCommand uniCommand = CommandsWithTimeout.Oracle(query, synchronizeParameters.DatabaseRow.Connection);
			using UniDataReader uniDataReader = uniCommand.ExecuteReader();
			while (uniDataReader.Read())
			{
				AddRelation(uniDataReader, SharedDatabaseTypeEnum.DatabaseType.Oracle);
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetColumns(string query, Form owner = null)
	{
		try
		{
			using UniCommand uniCommand = CommandsWithTimeout.Oracle(query, synchronizeParameters.DatabaseRow.Connection);
			using UniDataReader uniDataReader = uniCommand.ExecuteReader();
			while (uniDataReader.Read())
			{
				AddColumn(uniDataReader, withCustomFields: false, withTableTypes: true);
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetParameters(string query, bool canReadParameters = true, Form owner = null)
	{
		try
		{
			using UniCommand uniCommand = CommandsWithTimeout.Oracle(query, synchronizeParameters.DatabaseRow.Connection);
			using UniDataReader uniDataReader = uniCommand.ExecuteReader();
			while (uniDataReader.Read())
			{
				AddParameter(uniDataReader);
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
		try
		{
			using (UniCommand uniCommand = CommandsWithTimeout.Oracle(query, synchronizeParameters.DatabaseRow.Connection))
			{
				using UniDataReader uniDataReader = uniCommand.ExecuteReader();
				while (uniDataReader.Read())
				{
					AddTrigger(uniDataReader);
				}
			}
			if (!GetTriggerDefinitions(owner))
			{
				return false;
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public bool GetTriggerDefinitions(Form owner = null)
	{
		if (base.TriggerRows.Any())
		{
			Stopwatch stopwatch = ExecutionLog.StartStopwatch();
			try
			{
				if (!GetDefinitionWithFormattingForAllTriggerObjects(base.TriggerRows, owner))
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				OracleException obj = ex.InnerException as OracleException;
				if (obj == null || obj.Code != 1013)
				{
					foreach (TriggerRow item in base.TriggerRows.Where((TriggerRow x) => x.Definition != null))
					{
						item.Definition = null;
					}
					PrepareQueriesForGettingTriggerDefinitions();
					foreach (TriggerRow triggerRow in base.TriggerRows)
					{
						Stopwatch stopwatch2 = ExecutionLog.StartStopwatch();
						try
						{
							GetDefinitionForTriggerObject(triggerRow);
						}
						finally
						{
							ExecutionLog.WriteExecutionLog(stopwatch2, "DATAEDO.APP SYNCHRONIZATION GetDefinitionForTriggerObject", null, null, 2, 1);
						}
					}
					return true;
				}
				return false;
			}
			finally
			{
				ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION GetTriggerDefinitions", null, null, 2, 1);
			}
		}
		return true;
	}

	public void PrepareQueriesForGettingTriggerDefinitions()
	{
		ExecutionLog.StartStopwatch();
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "TR.OWNER", "TR.TABLE_NAME");
		triggerTypeDefinitionQuery = "SELECT S.OWNER, S.NAME, S.TEXT, TR.TABLE_NAME, TR.OWNER AS TABLE_SCHEMA  FROM " + privilegesHelper.GetName("SOURCE") + " S INNER JOIN " + privilegesHelper.GetName("TRIGGERS") + " TR\r\n                                        ON S.OWNER = TR.OWNER\r\n                                        AND S.NAME = TR.TRIGGER_NAME AND S.TYPE = 'TRIGGER'\r\n                                        WHERE\r\n                                        S.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ") " + filterString + "AND NAME = '{0}' AND TABLE_NAME = '{1}' AND TR.OWNER = '{2}' ORDER BY OWNER, NAME, TO_NUMBER(LINE)";
	}

	public void GetDefinitionForTriggerObject(TriggerRow row)
	{
		try
		{
			if (triggerTypeDefinitionQuery == null)
			{
				throw new InvalidOperationException("Queries for getting trigger definitions are not set.");
			}
			string format = triggerTypeDefinitionQuery;
			StringBuilder stringBuilder = new StringBuilder();
			using (UniCommand uniCommand = CommandsWithTimeout.Oracle(string.Format(format, LicenseHelper.EscapeInvalidCharacters(row.Name), LicenseHelper.EscapeInvalidCharacters(row.TableName), LicenseHelper.EscapeInvalidCharacters(row.TableSchema)), synchronizeParameters.DatabaseRow.Connection))
			{
				using UniDataReader uniDataReader = uniCommand.ExecuteReader();
				while (uniDataReader.Read())
				{
					if (row == null || !row.Name.Equals(uniDataReader["NAME"]) || !row.TableName.Equals(uniDataReader["TABLE_NAME"]) || !row.TableSchema.Equals(uniDataReader["TABLE_SCHEMA"]))
					{
						if (row != null)
						{
							row.Definition = stringBuilder.ToString();
						}
						stringBuilder = new StringBuilder();
					}
					stringBuilder.Append(uniDataReader["TEXT"].ToString());
				}
			}
			if (row != null && row.Definition == null)
			{
				row.Definition = stringBuilder.ToString();
			}
		}
		catch (Exception)
		{
			throw;
		}
	}

	private bool GetDefinitionWithFormattingForAllTriggerObjects(ObservableCollection<TriggerRow> rows, Form owner = null)
	{
		try
		{
			TriggerRow triggerRow = null;
			StringBuilder stringBuilder = new StringBuilder();
			string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "TR.OWNER", "TR.TABLE_NAME");
			using (UniCommand uniCommand = CommandsWithTimeout.Oracle("SELECT S.OWNER, S.NAME, S.TEXT, TR.TABLE_NAME, TR.OWNER AS TABLE_SCHEMA  FROM " + privilegesHelper.GetName("SOURCE") + " S INNER JOIN " + privilegesHelper.GetName("TRIGGERS") + " TR\r\n                                              ON S.OWNER = TR.OWNER\r\n                                              AND S.NAME = TR.TRIGGER_NAME AND S.TYPE = 'TRIGGER'\r\n                                              WHERE\r\n                                              S.OWNER IN (" + synchronizeParameters.DatabaseRow.SchemasListForCondition + ") " + filterString + "ORDER BY OWNER, NAME, TO_NUMBER(LINE)", synchronizeParameters.DatabaseRow.Connection))
			{
				UniDataReader dr = uniCommand.ExecuteReader();
				try
				{
					while (dr.Read())
					{
						if (triggerRow == null || !triggerRow.Name.Equals(dr["NAME"]) || !triggerRow.TableName.Equals(dr["TABLE_NAME"]) || !triggerRow.TableSchema.Equals(dr["TABLE_SCHEMA"]))
						{
							if (triggerRow != null)
							{
								triggerRow.Definition = stringBuilder.ToString();
							}
							stringBuilder = new StringBuilder();
							triggerRow = rows.Where((TriggerRow x) => x.Name.Equals(dr["NAME"]) && x.TableName.Equals(dr["TABLE_NAME"]) && x.TableSchema.Equals(dr["TABLE_SCHEMA"])).FirstOrDefault();
						}
						stringBuilder.Append(dr["TEXT"].ToString());
					}
				}
				finally
				{
					if (dr != null)
					{
						((IDisposable)dr).Dispose();
					}
				}
			}
			if (triggerRow != null)
			{
				triggerRow.Definition = stringBuilder.ToString();
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		try
		{
			using UniCommand uniCommand = CommandsWithTimeout.Oracle(query, synchronizeParameters.DatabaseRow.Connection);
			using UniDataReader uniDataReader = uniCommand.ExecuteReader();
			while (uniDataReader.Read())
			{
				AddUniqueConstraint(uniDataReader, SharedDatabaseTypeEnum.DatabaseType.Oracle);
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
		try
		{
			using UniCommand uniCommand = CommandsWithTimeout.Oracle(query, synchronizeParameters.DatabaseRow.Connection);
			using UniDataReader uniDataReader = uniCommand.ExecuteReader();
			while (uniDataReader.Read())
			{
				AddDependency(uniDataReader);
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	private void HandlePackages(BackgroundWorkerManager backgroundWorkerManager)
	{
		List<ObjectRow> list = synchronizeParameters.DatabaseRow.tableRows.Where((ObjectRow t) => t.Type == SharedObjectTypeEnum.ObjectType.Procedure && (t.Definition.Contains("package") || (t.Definition.Contains("PACKAGE") && t.ToSynchronize))).ToList();
		int num = list.Count();
		for (int i = 0; i < num; i++)
		{
			try
			{
				AddProceduresFromPackage(list[i].Schema, list[i].Definition, backgroundWorkerManager);
				AddFunctionsFromPackage(list[i].Schema, list[i].Definition, backgroundWorkerManager);
			}
			catch (Exception)
			{
			}
		}
	}

	private void AddProceduresFromPackage(string packageSchema, string package, BackgroundWorkerManager backgroundWorkerManager)
	{
		ObjectRow objectRow = null;
		interpreter = new PlSqlInterpreter(package);
		foreach (ScriptData item in interpreter.Run())
		{
			objectRow = null;
			try
			{
				if (item.Procedure == null || (item.Type != ScriptType.CREATE_OR_ALTER_PROCEDURE && item.Type != ScriptType.CREATE_PROCEDURE))
				{
					continue;
				}
				Procedure procedure = item.Procedure;
				ObjectRow objectRow2 = new ObjectRow
				{
					Name = procedure.Name,
					Type = SharedObjectTypeEnum.ObjectType.Procedure,
					Schema = (packageSchema ?? string.Empty),
					Definition = procedure.Body,
					DatabaseName = synchronizeParameters.DatabaseName,
					DatabaseId = synchronizeParameters.DatabaseRow.Id,
					Subtype = SharedObjectSubtypeEnum.ObjectSubtype.OraclePackageProcedure,
					MultipleSchemasDatabase = true,
					ToSynchronize = true
				};
				objectRow = objectRow2;
				CheckForSameNamesOfObject(objectRow2);
				AddDBObject(objectRow2, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager);
				if (procedure.Parameters == null)
				{
					continue;
				}
				int num = 1;
				foreach (Parameter param in procedure.Parameters)
				{
					ParameterRow parameterRow = new ParameterRow
					{
						Name = param.Name,
						ProcedureName = objectRow2.Name,
						Mode = (ParameterRow.ModeEnum)Enum.Parse(typeof(ParameterRow.ModeEnum), param.Mode, ignoreCase: true),
						ParameterMode = param.Mode,
						DataType = param.DataType,
						ProcedureSchema = (packageSchema ?? string.Empty),
						DatabaseName = synchronizeParameters.DatabaseName,
						Position = num
					};
					if (!base.ParameterRows.Any((ParameterRow p) => p.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase) && p.DataType == parameterRow.DataType && p.ProcedureSchema.Equals(packageSchema, StringComparison.OrdinalIgnoreCase) && p.ProcedureName.Equals(procedure.Name, StringComparison.OrdinalIgnoreCase)))
					{
						AddParameter(parameterRow);
						num++;
					}
				}
			}
			catch
			{
				if (objectRow != null)
				{
					objectRow.ToSynchronize = false;
				}
			}
		}
	}

	private void AddFunctionsFromPackage(string packageSchema, string package, BackgroundWorkerManager backgroundWorkerManager)
	{
		ObjectRow objectRow = null;
		interpreter = new PlSqlInterpreter(package);
		foreach (ScriptData item in interpreter.Run())
		{
			objectRow = null;
			try
			{
				if (item.Function == null || (item.Type != ScriptType.CREATE_FUNCTION && item.Type != ScriptType.CREATE_OR_ALTER_FUNCTION))
				{
					continue;
				}
				Function function = item.Function;
				ObjectRow objectRow2 = new ObjectRow
				{
					Name = function.Name,
					Type = SharedObjectTypeEnum.ObjectType.Function,
					Schema = (packageSchema ?? string.Empty),
					Definition = function.Body,
					DatabaseName = synchronizeParameters.DatabaseName,
					DatabaseId = synchronizeParameters.DatabaseRow.Id,
					Subtype = SharedObjectSubtypeEnum.ObjectSubtype.OraclePackageFunction,
					MultipleSchemasDatabase = true,
					ToSynchronize = true
				};
				objectRow = objectRow2;
				CheckForSameNamesOfObject(objectRow2);
				AddDBObject(objectRow2, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager);
				if (function.Parameters == null)
				{
					continue;
				}
				int num = 1;
				foreach (Parameter param in function.Parameters)
				{
					ParameterRow parameterRow = new ParameterRow
					{
						Name = param.Name,
						ProcedureName = objectRow2.Name,
						DataType = param.DataType,
						ProcedureSchema = (packageSchema ?? string.Empty),
						DatabaseName = synchronizeParameters.DatabaseName,
						ParameterMode = param.Mode,
						Position = num
					};
					try
					{
						parameterRow.Mode = (ParameterRow.ModeEnum)Enum.Parse(typeof(ParameterRow.ModeEnum), param.Mode, ignoreCase: true);
					}
					catch (Exception)
					{
						parameterRow.Mode = ParameterRow.ModeEnum.Out;
					}
					if (!base.ParameterRows.Any((ParameterRow p) => p.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase) && p.DataType == parameterRow.DataType && p.ProcedureSchema.Equals(packageSchema, StringComparison.OrdinalIgnoreCase) && p.ProcedureName.Equals(function.Name, StringComparison.OrdinalIgnoreCase)))
					{
						AddParameter(parameterRow);
						num++;
					}
				}
			}
			catch
			{
				if (objectRow != null)
				{
					objectRow.ToSynchronize = false;
				}
			}
		}
	}

	private void CheckForSameNamesOfObject(ObjectRow row)
	{
		if (!synchronizeParameters.DatabaseRow.tableRows.Any((ObjectRow t) => t.Name.Equals(row.Name, StringComparison.OrdinalIgnoreCase)))
		{
			return;
		}
		IEnumerable<ObjectRow> enumerable = synchronizeParameters.DatabaseRow.tableRows.Where((ObjectRow t) => t.Name.Equals(row.Name, StringComparison.OrdinalIgnoreCase));
		int num = enumerable.Count() - 1;
		foreach (ObjectRow item in enumerable)
		{
			_ = item;
			num++;
		}
		string rowName = row.Name + num;
		bool flag = synchronizeParameters.DatabaseRow.tableRows.Any((ObjectRow t) => t.Name.Equals(rowName, StringComparison.OrdinalIgnoreCase));
		if (!flag)
		{
			row.Name += num;
			return;
		}
		while (flag)
		{
			num++;
			string name = row.Name + num;
			flag = synchronizeParameters.DatabaseRow.tableRows.Any((ObjectRow t) => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}
		row.Name += num;
	}
}
