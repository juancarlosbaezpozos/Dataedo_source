using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.General;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.DB2;

internal class SynchronizeDb2 : SynchronizeDatabase
{
	public SynchronizeDb2(SynchronizeParameters synchronizeParameters)
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
				OdbcDataReader odbcDataReader = CommandsWithTimeout.Odbc(query.Query, synchronizeParameters.DatabaseRow.Connection).ExecuteReader();
				while (odbcDataReader.Read())
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					ReadCount(odbcDataReader);
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
					OdbcDataReader odbcDataReader = CommandsWithTimeout.Odbc(query.Query, synchronizeParameters.DatabaseRow.Connection).ExecuteReader();
					while (odbcDataReader.Read())
					{
						if (synchronizeParameters.DbSynchLocker.IsCanceled)
						{
							return false;
						}
						AddDBObject(odbcDataReader, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
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
		try
		{
			OdbcDataReader odbcDataReader = CommandsWithTimeout.Odbc(query, synchronizeParameters.DatabaseRow.Connection).ExecuteReader();
			while (odbcDataReader.Read())
			{
				AddRelation(odbcDataReader, SharedDatabaseTypeEnum.DatabaseType.Db2LUW);
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
			OdbcDataReader odbcDataReader = CommandsWithTimeout.Odbc(query, synchronizeParameters.DatabaseRow.Connection).ExecuteReader();
			while (odbcDataReader.Read())
			{
				AddColumn(odbcDataReader);
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
			OdbcDataReader odbcDataReader = CommandsWithTimeout.Odbc(query, synchronizeParameters.DatabaseRow.Connection).ExecuteReader();
			while (odbcDataReader.Read())
			{
				AddTrigger(odbcDataReader);
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
			OdbcDataReader odbcDataReader = CommandsWithTimeout.Odbc(query, synchronizeParameters.DatabaseRow.Connection).ExecuteReader();
			while (odbcDataReader.Read())
			{
				AddUniqueConstraint(odbcDataReader, SharedDatabaseTypeEnum.DatabaseType.Db2LUW);
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
		if (!canReadParameters)
		{
			return true;
		}
		try
		{
			OdbcDataReader odbcDataReader = CommandsWithTimeout.Odbc(query, synchronizeParameters.DatabaseRow.Connection).ExecuteReader();
			while (odbcDataReader.Read())
			{
				AddParameter(odbcDataReader);
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
			using OdbcCommand odbcCommand = CommandsWithTimeout.Odbc(query, synchronizeParameters.DatabaseRow.Connection);
			using OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();
			while (odbcDataReader.Read())
			{
				AddDependency(odbcDataReader);
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM(TAB.TABSCHEMA)", "TRIM(TAB.TABNAME)");
		yield return "SELECT \r\n                        'TABLE' AS OBJECT_TYPE,\r\n                        COUNT(1) AS COUNT\r\n                    FROM SYSCAT.TABLES TAB\r\n                    WHERE TAB.TYPE IN('T','S','U') -- TABLE\r\n                        AND TAB.TABSCHEMA NOT LIKE 'SYS%'  \r\n                        " + filterString;
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "TRIM(TAB.TABSCHEMA)", "TRIM(TAB.TABNAME)");
		yield return "SELECT\r\n                        'VIEW' AS OBJECT_TYPE,\r\n                        COUNT(1) AS COUNT\r\n                    FROM  SYSCAT.TABLES TAB\r\n                    WHERE TAB.TYPE IN('V','W') -- VIEW\r\n                        AND TAB.TABSCHEMA NOT LIKE 'SYS%'   \r\n                        " + filterString;
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "TRIM(R.ROUTINESCHEMA)", "TRIM(R.ROUTINENAME)");
		yield return "SELECT\r\n                        'PROCEDURE' AS OBJECT_TYPE,\r\n                        COUNT(1) AS COUNT\r\n                    FROM SYSIBM.SYSROUTINES R\r\n                    WHERE R.ROUTINETYPE = 'P' \r\n                        AND R.ROUTINESCHEMA NOT LIKE 'SYS%' \r\n                        " + filterString;
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "TRIM(R.ROUTINESCHEMA)", "TRIM(R.ROUTINENAME)");
		yield return "SELECT\r\n                        'FUNCTION' AS OBJECT_TYPE,\r\n                        COUNT(1) AS COUNT\r\n                    FROM SYSIBM.SYSROUTINES R\r\n                    WHERE R.ROUTINETYPE in ('F', 'M')\r\n                        AND ROUTINESCHEMA NOT LIKE 'SYS%' \r\n                        " + filterString;
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM(TAB.TABSCHEMA)", "TRIM(TAB.TABNAME)");
		DatabaseVersionUpdate versionUpdate = synchronizeParameters.DatabaseRow.GetVersionUpdate();
		if (versionUpdate.Version < 10 || (versionUpdate.Version == 10 && versionUpdate.Update < 5))
		{
			yield return "SELECT \r\n                    TRIM(TAB.TABNAME) AS NAME,\t\r\n                    TRIM(TAB.TABSCHEMA)  AS SCHEMA,\r\n                    TRIM(CURRENT_SERVER) AS DATABASE_NAME,\r\n                    'TABLE' AS TYPE,\r\n                    CAST(TAB.REMARKS AS VARCHAR(32000)) AS DESCRIPTION,\r\n                    CAST(NULL AS VARCHAR(32000)) AS DEFINITION,\r\n                    TAB.CREATE_TIME AS CREATE_DATE,\r\n                    TAB.ALTER_TIME AS MODIFY_DATE,\r\n                    NULL AS FUNCTION_TYPE\r\n            --   NULL AS LANGUAGE\r\n                FROM SYSCAT.TABLES TAB\r\n                WHERE TAB.TYPE IN('T','S','U') -- TABLE \r\n                        AND TAB.TABSCHEMA NOT LIKE 'SYS%'  \r\n                    " + filterString;
		}
		else
		{
			yield return "SELECT \r\n\t\t\t\t     TRIM(TAB.TABNAME) AS NAME,\t\r\n\t\t\t\t     TRIM(TAB.TABSCHEMA)  AS SCHEMA,\r\n\t\t\t\t     TRIM(CURRENT_SERVER) AS DATABASE_NAME,\r\n\t\t\t\t     'TABLE' AS TYPE,\r\n\t\t\t\t     CAST(TAB.REMARKS AS VARCHAR(32000)) AS DESCRIPTION,\r\n\t\t\t\t     CAST(NULL AS VARCHAR(32000)) AS DEFINITION,\r\n\t\t\t\t     TAB.CREATE_TIME AS CREATE_DATE,\r\n\t\t\t\t     MAX(TAB.ALTER_TIME, COALESCE(T.ALTER_TIME,TAB.ALTER_TIME)) AS MODIFY_DATE,\r\n\t\t\t\t     NULL AS FUNCTION_TYPE\r\n\t\t\t\t     --   NULL AS LANGUAGE\r\n\t\t\t\t    FROM SYSCAT.TABLES TAB\r\n\t\t\t\t    LEFT JOIN SYSCAT.TRIGGERS T\r\n\t\t\t\t     ON TRIM(TAB.TABSCHEMA) = TRIM(T.TABSCHEMA)\r\n\t\t\t\t     AND TRIM(TAB.TABNAME) = TRIM(T.TABNAME)\r\n\t\t\t\t     AND T.ALTER_TIME = (\r\n\t\t\t\t\t    SELECT MAX(ALTER_TIME)\r\n\t\t\t\t\t    FROM SYSCAT.TRIGGERS \r\n\t\t\t\t\t    WHERE TABNAME = T.TABNAME\r\n\t\t\t\t\t    AND TABSCHEMA = T.TABSCHEMA\r\n\t\t\t\t     )\r\n\t\t\t\t    WHERE TAB.TYPE IN('T','S','U') -- TABLE \r\n\t\t\t\t     AND TAB.TABSCHEMA NOT LIKE 'SYS%'\r\n                            " + filterString;
		}
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "TRIM(TAB.TABSCHEMA)", "TRIM(TAB.TABNAME)");
		DatabaseVersionUpdate versionUpdate = synchronizeParameters.DatabaseRow.GetVersionUpdate();
		if (versionUpdate.Version < 10 || (versionUpdate.Version == 10 && versionUpdate.Update < 5))
		{
			yield return "SELECT \r\n                         TRIM(TAB.TABNAME) AS NAME,\t\r\n                         TRIM(TAB.TABSCHEMA)  AS SCHEMA,\r\n                         TRIM(CURRENT_SERVER) AS DATABASE_NAME,\r\n                         'VIEW' AS TYPE,\r\n                         CAST(TAB.REMARKS AS VARCHAR(32000)) AS DESCRIPTION,\r\n                         CAST(V.TEXT AS VARCHAR(32000)) AS DEFINITION,\r\n                         TAB.CREATE_TIME AS CREATE_DATE,\r\n                         TAB.ALTER_TIME AS MODIFY_DATE,\r\n                         NULL AS FUNCTION_TYPE\r\n                       --   NULL AS LANGUAGE\r\n                    FROM SYSCAT.TABLES TAB\r\n                    INNER JOIN SYSCAT.VIEWS V ON V.VIEWSCHEMA = TAB.TABSCHEMA \r\n                          AND V.VIEWNAME = TAB.TABNAME \r\n                    WHERE TAB.TYPE IN('V','W') -- VIEW \r\n                         AND TAB.TABSCHEMA NOT LIKE 'SYS%'   \r\n                        " + filterString;
		}
		else
		{
			yield return "SELECT \r\n\t\t\t\t\t\t\t TRIM(TAB.TABNAME) AS NAME,\t\r\n\t\t\t\t\t\t\t TRIM(TAB.TABSCHEMA)  AS SCHEMA,\r\n\t\t\t\t\t\t\t TRIM(CURRENT_SERVER) AS DATABASE_NAME,\r\n\t\t\t\t\t\t\t 'VIEW' AS TYPE,\r\n\t\t\t\t\t\t\t CAST(TAB.REMARKS AS VARCHAR(32000)) AS DESCRIPTION,\r\n\t\t\t\t\t\t\t CAST(V.TEXT AS VARCHAR(32000)) AS DEFINITION,\r\n\t\t\t\t\t\t\t TAB.CREATE_TIME AS CREATE_DATE,\r\n\t\t\t\t\t\t\t MAX(TAB.ALTER_TIME, COALESCE(T.ALTER_TIME,TAB.ALTER_TIME)) AS MODIFY_DATE,\r\n\t\t\t\t\t\t\t NULL AS FUNCTION_TYPE\r\n\t\t\t\t\t\t   --   NULL AS LANGUAGE\r\n\t\t\t\t\t\tFROM SYSCAT.TABLES TAB\r\n\t\t\t\t\t\tINNER JOIN SYSCAT.VIEWS V ON V.VIEWSCHEMA = TAB.TABSCHEMA \r\n\t\t\t\t\t\t\tAND V.VIEWNAME = TAB.TABNAME \r\n\t\t\t\t\t\tLEFT JOIN SYSCAT.TRIGGERS T\r\n\t\t\t\t\t\t ON TRIM(TAB.TABSCHEMA) = TRIM(T.TABSCHEMA)\r\n\t\t\t\t\t\t AND TRIM(TAB.TABNAME) = TRIM(T.TABNAME)\r\n\t\t\t\t\t\t AND T.ALTER_TIME = (\r\n\t\t\t\t\t\t\tSELECT MAX(ALTER_TIME)\r\n\t\t\t\t\t\t\tFROM SYSCAT.TRIGGERS \r\n\t\t\t\t\t\t\tWHERE TABNAME = T.TABNAME\r\n\t\t\t\t\t\t\tAND TABSCHEMA = T.TABSCHEMA\r\n\t\t\t\t\t\t)\r\n\t\t\t\t\t\tWHERE TAB.TYPE IN('V','W') -- VIEW \r\n\t\t\t\t\t\t\tAND TAB.TABSCHEMA NOT LIKE 'SYS%'   \r\n                        " + filterString;
		}
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "TRIM(R.ROUTINESCHEMA)", "TRIM(R.ROUTINENAME)");
		yield return "SELECT\r\n                         CASE WHEN il>1 THEN\r\n                         TRIM(R.ROUTINENAME) || ' [' || TRIM(R.SPECIFICNAME) || ']' \r\n                         ELSE TRIM(R.ROUTINENAME)\r\n                         END AS NAME,\r\n                         TRIM(R.ROUTINESCHEMA) AS SCHEMA,\r\n                         TRIM(CURRENT_SERVER) AS DATABASE_NAME,\r\n                         'PROCEDURE' AS TYPE,\r\n                         CAST(R.REMARKS AS VARCHAR(32000)) AS DESCRIPTION,\r\n                         CAST(R.TEXT AS VARCHAR(32000)) AS DEFINITION,\r\n                         R.CREATE_TIME AS CREATE_DATE,\r\n                         R.ALTER_TIME AS MODIFY_DATE,\r\n                         NULL AS FUNCTION_TYPE\r\n                       --    LANGUAGE\r\n                    FROM SYSCAT.ROUTINES R\r\n                         LEFT JOIN \r\n                    (\r\n                        SELECT ROUTINESCHEMA, ROUTINENAME , count(*) as il\r\n                        FROM\r\n                        SYSCAT.ROUTINES \r\n                        WHERE\r\n                             ROUTINETYPE = 'P' \r\n                             AND ROUTINESCHEMA NOT LIKE 'SYS%' \r\n                        GROUP BY ROUTINESCHEMA, ROUTINENAME\r\n                        having count(*)>1\r\n                    ) R2 on R2.ROUTINESCHEMA = R.ROUTINESCHEMA AND R2.ROUTINENAME = R.ROUTINENAME\r\n                    WHERE R.ROUTINETYPE = 'P' \r\n                        AND R.ROUTINESCHEMA NOT LIKE 'SYS%'   \r\n                        " + filterString;
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "TRIM(R.ROUTINESCHEMA)", "TRIM(R.ROUTINENAME)");
		yield return "SELECT\r\n                         CASE WHEN il>1 THEN\r\n                         TRIM(R.ROUTINENAME) || ' [' || TRIM(R.SPECIFICNAME) || ']' \r\n                         ELSE TRIM(R.ROUTINENAME)\r\n                         END AS NAME,\r\n                         TRIM(R.ROUTINESCHEMA) AS SCHEMA,\r\n                         TRIM(CURRENT_SERVER) AS DATABASE_NAME,\r\n                         'FUNCTION' AS TYPE,\r\n                         CAST(R.REMARKS AS VARCHAR(32000)) AS DESCRIPTION,\t\r\n                         CAST(R.TEXT AS VARCHAR(32000)) AS DEFINITION,\r\n                         R.CREATE_TIME AS CREATE_DATE,\r\n                         R.ALTER_TIME AS MODIFY_DATE,\r\n                         CASE R.FUNCTIONTYPE\r\n                              WHEN 'C' THEN 'COLUMN OR AGGREGATE'\r\n                              WHEN 'R' THEN 'ROW'\r\n                              WHEN 'S' THEN 'SCALAR' \r\n                              WHEN 'T' THEN 'TABLE' \r\n                         END AS FUNCTION_TYPE\r\n                          --    LANGUAGE\r\n                    FROM SYSCAT.ROUTINES R\r\n                    LEFT JOIN \r\n                    (\r\n                    SELECT ROUTINESCHEMA, ROUTINENAME , count(*) as il\r\n                    FROM\r\n                    SYSCAT.ROUTINES \r\n                    WHERE\r\n                         ROUTINETYPE IN ('F','M') \r\n                         AND ROUTINESCHEMA NOT LIKE 'SYS%' \r\n                    GROUP BY ROUTINESCHEMA, ROUTINENAME\r\n                    having count(*)>1\r\n                    ) R2\r\n                     on R2.ROUTINESCHEMA = R.ROUTINESCHEMA AND R2.ROUTINENAME = R.ROUTINENAME                   \r\n                    WHERE R.ROUTINETYPE in ('F', 'M')\r\n                         AND R.ROUTINESCHEMA NOT LIKE 'SYS%'    \r\n                        " + filterString;
	}

	public override IEnumerable<string> RelationsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString(null, base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM(REF.TABSCHEMA)", "TRIM(REF.TABNAME)");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("OR", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM(REF.REFTABSCHEMA)", "TRIM(REF.REFTABNAME)");
		string text = FilterRulesCollection.CombineRelationsFilter(filterString, filterString2);
		yield return "SELECT \r\n                         TRIM(REF.CONSTNAME) AS NAME,\r\n                         TRIM(CURRENT_SERVER) AS FK_TABLE_DATABASE_NAME,\r\n                         TRIM(REF.TABNAME) AS FK_TABLE_NAME,\r\n                         TRIM(REF.TABSCHEMA) AS FK_TABLE_SCHEMA,\r\n                         TRIM(KEY.COLNAME)  AS FK_COLUMN,\r\n                         TRIM(CURRENT_SERVER) AS REF_TABLE_DATABASE_NAME,\r\n                         TRIM(REF.REFTABNAME) AS REF_TABLE_NAME,\r\n                         TRIM(REF.REFTABSCHEMA) AS REF_TABLE_SCHEMA,\r\n                         TRIM(KEYPK.COLNAME) AS REF_COLUMN,\r\n                         KEY.COLSEQ  AS ORDINAL_POSITION,\r\n                         NULL AS DESCRIPTION,\r\n                         REF.UPDATERULE AS UPDATE_RULE,\r\n                         REF.DELETERULE AS DELETE_RULE\t\r\n                    FROM SYSCAT.REFERENCES REF\r\n                    INNER JOIN SYSCAT.KEYCOLUSE KEY ON KEY.TABSCHEMA = REF.TABSCHEMA \r\n                          AND KEY.TABNAME = REF.TABNAME \r\n                          AND KEY.CONSTNAME = REF.CONSTNAME\r\n                    INNER JOIN SYSCAT.KEYCOLUSE KEYPK ON KEYPK.TABSCHEMA = REF.REFTABSCHEMA \r\n                          AND KEYPK.TABNAME = REF.REFTABNAME \r\n                          AND KEYPK.CONSTNAME = REF.REFKEYNAME \r\n                          AND KEYPK.COLSEQ=KEY.COLSEQ\r\n                          " + text;
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM(COL.TABSCHEMA)", "TRIM(COL.TABNAME)");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.View, "TRIM(COL.TABSCHEMA)", "TRIM(COL.TABNAME)");
		yield return "SELECT \r\n                         TRIM(CURRENT_SERVER) AS DATABASE_NAME,\r\n                         TRIM(COL.TABNAME) AS TABLE_NAME,\r\n                         TRIM(COL.TABSCHEMA) AS TABLE_SCHEMA,\r\n                         TRIM(COL.COLNAME) AS NAME,\r\n                         COL.COLNO + 1 AS POSITION,\r\n                         COL.TYPENAME AS DATATYPE,\r\n                        COL.LENGTH AS DATA_LENGTH,\r\n                        CAST(COL.REMARKS AS VARCHAR(32000)) AS DESCRIPTION,\t\r\n                         PK.TYPE AS CONSTRAINT_TYPE,\r\n                         CASE WHEN  COL.NULLS = 'Y' THEN 1 ELSE 0 END AS NULLABLE,\r\n                         CAST(DEFAULT AS VARCHAR(32000)) AS DEFAULT_VALUE,\r\n                         CASE WHEN COL.IDENTITY ='Y' THEN 1 ELSE 0 END AS IS_IDENTITY,\r\n                         CASE WHEN COL.GENERATED ='' THEN 0 ELSE 1 END AS  IS_COMPUTED,\r\n                         CAST(COL.TEXT AS VARCHAR(32000)) AS COMPUTED_FORMULA \r\n                    FROM SYSCAT.COLUMNS  COL     \r\n                    LEFT JOIN (SELECT 'P' as TYPE,\r\n                                   KEY.TABSCHEMA,\r\n                                   KEY.TABNAME,\r\n                                   KEY.COLNAME\r\n                              FROM SYSCAT.KEYCOLUSE KEY\r\n                              INNER JOIN  SYSCAT.TABCONST CON\r\n                                   ON CON.TABSCHEMA = KEY.TABSCHEMA\r\n                                   AND CON.TABNAME = KEY.TABNAME\r\n                                   AND CON.CONSTNAME = KEY.CONSTNAME \r\n                              WHERE CON.TYPE='P') PK\r\n                         ON PK.TABSCHEMA = COL.TABSCHEMA \r\n                              AND PK.TABNAME = COL.TABNAME \r\n                              AND PK.COLNAME = COL.COLNAME\r\n                         INNER JOIN SYSCAT.TABLES TABS\r\n                              ON TABS.TABSCHEMA = COL.TABSCHEMA\r\n                                   AND TABS.TABNAME = COL.TABNAME  \r\n                    WHERE COL.TABSCHEMA NOT LIKE 'SYS%'\r\n                         AND ((TABS.TYPE IN('T','S','U') " + filterString + ")\r\n                              OR (TABS.TYPE IN('V','W') " + filterString2 + "))";
	}

	public override IEnumerable<string> TriggersQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM(T.TABSCHEMA)", "TRIM(T.TABNAME)");
		DatabaseVersionUpdate versionUpdate = synchronizeParameters.DatabaseRow.GetVersionUpdate();
		if (versionUpdate.Version > 10 || (versionUpdate.Version == 10 && versionUpdate.Update >= 1))
		{
			yield return " SELECT\r\n                     'TR' AS TYPE,\r\n                   TRIM(T.TRIGNAME) AS TRIGGER_NAME,\r\n                   TRIM(T.TABSCHEMA) AS TABLE_SCHEMA,\r\n                   TRIM(T.TABNAME) AS TABLE_NAME,\r\n                   TRIM(CURRENT_SERVER) AS DATABASE_NAME,\r\n                   CASE T.EVENTUPDATE\r\n                        WHEN 'Y' THEN 1\r\n                        ELSE 0\r\n                   END AS     ISUPDATE,\r\n                   CASE T.EVENTDELETE\r\n                        WHEN 'Y' THEN 1\r\n                        ELSE 0\r\n                   END AS     ISDELETE,   \r\n                   CASE T.EVENTINSERT\r\n                        WHEN 'Y' THEN 1\r\n                        ELSE 0\r\n                   END AS     ISINSERT,\r\n                   CASE T.TRIGTIME\r\n                        WHEN 'B' THEN 1\r\n                        ELSE 0\r\n                   END AS     ISBEFORE,\r\n                   CASE T.TRIGTIME\r\n                        WHEN 'A' THEN 1\r\n                        ELSE 0\r\n                   END AS     ISAFTER,\r\n                   CASE T.TRIGTIME\r\n                        WHEN 'I' THEN 1\r\n                        ELSE 0\r\n                   END AS     ISINSTEADOF,\r\n                   0 AS  DISABLED,\r\n                   CAST(T.TEXT AS VARCHAR(32000)) AS DEFINITION,\r\n                   CAST(T.REMARKS AS VARCHAR(32000)) AS DESCRIPTION\r\n                FROM SYSCAT.TRIGGERS T\r\n                WHERE T.TABSCHEMA NOT LIKE 'SYS%'\r\n                " + filterString;
		}
		else
		{
			yield return " SELECT\r\n                     'TR' AS TYPE,\r\n                   TRIM(T.TRIGNAME) AS TRIGGER_NAME,\r\n                   TRIM(T.TABSCHEMA) AS TABLE_SCHEMA,\r\n                   TRIM(T.TABNAME) AS TABLE_NAME,\r\n                   TRIM(CURRENT_SERVER) AS DATABASE_NAME,\r\n                   CASE T.TRIGEVENT \r\n                      WHEN 'U' THEN 1 \r\n                      ELSE 0\r\n                   END AS \tISUPDATE,\r\n                   CASE T.TRIGEVENT \r\n                      WHEN 'D' THEN 1 \r\n                      ELSE 0\r\n                   END AS \tISDELETE,   \r\n                   CASE T.TRIGEVENT \r\n                      WHEN 'I' THEN 1 \r\n                      ELSE 0\r\n                   END AS \tISINSERT,\r\n                   CASE T.TRIGTIME \r\n                      WHEN 'B' THEN 1 \r\n                      ELSE 0\r\n                   END AS \tISBEFORE,\r\n                   CASE T.TRIGTIME \r\n                      WHEN 'A' THEN 1 \r\n                      ELSE 0\r\n                   END AS \tISAFTER,\r\n                   CASE T.TRIGTIME \r\n                      WHEN 'I' THEN 1 \r\n                      ELSE 0\r\n                   END AS \tISINSTEADOF,\r\n                   0 AS  DISABLED,\r\n                   CAST(T.TEXT AS VARCHAR(32000)) AS DEFINITION,\r\n                   CAST(T.REMARKS AS VARCHAR(32000)) AS DESCRIPTION\r\n                FROM SYSCAT.TRIGGERS T\r\n                WHERE T.TABSCHEMA NOT LIKE 'SYS%'\r\n                    " + filterString;
		}
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM(CON.TABSCHEMA)", "TRIM(CON.TABNAME)");
		yield return "SELECT \t\r\n                 TRIM(CURRENT_SERVER) AS DATABASE_NAME,\r\n                 TRIM(CON.TABNAME) AS TABLE_NAME,\r\n                 TRIM(CON.TABSCHEMA) AS TABLE_SCHEMA,\r\n                 TRIM(CON.CONSTNAME) AS NAME,\r\n                 CON.TYPE AS TYPE,\r\n                 TRIM(KEY.COLNAME) AS COLUMN_NAME,\r\n                 KEY.COLSEQ AS COLUMN_ORDINAL,\r\n                 CAST(CON.REMARKS AS VARCHAR(32000)) AS DESCRIPTION,\r\n               CASE CON.ENFORCED \r\n                  WHEN 'N' THEN 1\r\n                  WHEN 'Y' THEN 0\r\n                  END AS DISABLED\r\n            FROM SYSCAT.TABCONST CON\r\n            INNER JOIN SYSCAT.KEYCOLUSE KEY ON CON.TABSCHEMA = KEY.TABSCHEMA \r\n                  AND CON.TABNAME = KEY.TABNAME\r\n                  AND CON.CONSTNAME = KEY.CONSTNAME \r\n            WHERE CON.TYPE IN ('P','U')\r\n                  AND CON.TABSCHEMA NOT LIKE 'SYS%'\r\n                  " + filterString;
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Function, "TRIM(R.ROUTINESCHEMA)", "TRIM(R.ROUTINENAME)");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Procedure, "TRIM(R.ROUTINESCHEMA)", "TRIM(R.ROUTINENAME)");
		yield return "SELECT                       \r\n                           TRIM(CURRENT_SERVER) AS DATABASE_NAME,\r\n                       CASE WHEN il>1 THEN\r\n                         TRIM(R.ROUTINENAME) || ' [' || TRIM(R.SPECIFICNAME) || ']' \r\n                         ELSE TRIM(R.ROUTINENAME)\r\n                         END AS PROCEDURE_NAME,\r\n                      TRIM(R.ROUTINESCHEMA) AS PROCEDURE_SCHEMA,\r\n                      CASE WHEN PAR.PARMNAME IS NOT NULL\r\n                        THEN TRIM(PAR.PARMNAME)\r\n                        ELSE CONCAT(CONCAT('Unnamed_', PAR.ROWTYPE),  CAST(PAR.ORDINAL AS VARCHAR(32000)))\r\n                      END AS NAME,\r\n                      PAR.ORDINAL AS POSITION,\r\n                      CASE\r\n                      WHEN PAR.ROWTYPE IN ('B') THEN 'INOUT' \r\n                      WHEN PAR.ROWTYPE IN ('P','S') THEN 'IN'\r\n                      WHEN PAR.ROWTYPE IN ('O','C','R') THEN 'OUT'\r\n                      END\tAS PARAMETER_MODE,\r\n                      PAR.TYPENAME AS DATATYPE, \r\n                      CASE WHEN PAR.TYPENAME = 'DECIMAL' THEN CONCAT(CONCAT(LENGTH,','),SCALE)\r\n                      ELSE CAST(PAR.LENGTH AS VARCHAR(4))  \r\n                      END AS DATA_LENGTH,\r\n                      CAST(PAR.REMARKS AS VARCHAR(32000)) AS DESCRIPTION\r\n                      FROM \r\n                      SYSCAT.ROUTINES R\r\n                      INNER JOIN SYSCAT.ROUTINEPARMS PAR ON PAR.ROUTINESCHEMA = R.ROUTINESCHEMA \r\n                            AND PAR.SPECIFICNAME = R.SPECIFICNAME --AND PAR.ROUTINENAME=R.ROUTINENAME\r\n                      LEFT JOIN \r\n                         (\r\n                         SELECT RCOUNT.ROUTINESCHEMA, RCOUNT.ROUTINENAME , count(*) as il\r\n                         FROM\r\n                         SYSCAT.ROUTINES RCOUNT\r\n                         WHERE\r\n                              RCOUNT.ROUTINETYPE IN ('F','P','M')\r\n                              AND RCOUNT.ROUTINESCHEMA NOT LIKE 'SYS%' \r\n                         GROUP BY RCOUNT.ROUTINESCHEMA, RCOUNT.ROUTINENAME\r\n                         having count(*)>1\r\n                         ) R2\r\n                          on R2.ROUTINESCHEMA = R.ROUTINESCHEMA AND R2.ROUTINENAME = R.ROUTINENAME \r\n                       WHERE R.ROUTINESCHEMA NOT LIKE 'SYS%' \r\n                            AND ((R.ROUTINETYPE IN ('F', 'M') " + filterString + ")\r\n                              OR (R.ROUTINETYPE ='P' " + filterString2 + "))";
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		string host = synchronizeParameters.DatabaseRow.Host.ToUpper();
		string filterStringForDependencies = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("TRIM(DEP.TABSCHEMA)", "TRIM(DEP.TABNAME)", "(CASE \r\n                    WHEN DEP.DTYPE IN('S', 'T') THEN 'TABLE'\r\n                    WHEN DEP.DTYPE IN('V', 'W') THEN 'VIEW'\r\n                END)", "(CASE WHEN DEP.BTYPE IN ('S','T','G','U') THEN 'TABLE'\r\n                      WHEN DEP.BTYPE IN ('V','W') THEN 'VIEW'\r\n                    WHEN R.ROUTINETYPE IN ('F', 'M') THEN 'FUNCTION'\r\n                      WHEN R.ROUTINETYPE IN ('P') THEN 'PROCEDURE' \r\n                    ELSE NULL\r\n                END)");
		string filterStringForDependencies2 = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("TRIM(DEP.BSCHEMA)", "TRIM(DEP.BNAME)", "(CASE WHEN DEP.BTYPE IN ('S','T','G','U') THEN 'TABLE'\r\n                      WHEN DEP.BTYPE IN ('V','W') THEN 'VIEW'\r\n                      WHEN R.ROUTINETYPE IN ('F', 'M') THEN 'FUNCTION'\r\n                      WHEN R.ROUTINETYPE IN ('P') THEN 'PROCEDURE' \r\n                    ELSE NULL\r\n                END)", "(CASE \r\n                    WHEN DEP.DTYPE IN('S', 'T') THEN 'TABLE'\r\n                    WHEN DEP.DTYPE IN('V', 'W') THEN 'VIEW'\r\n                END)");
		string text = FilterRulesCollection.CombineDependenciesFilter(filterStringForDependencies, filterStringForDependencies2);
		string filterStringForDependencies3 = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("TRIM(DEP.DSCHEMA)", "TRIM(DEP.DNAME)", "(CASE \r\n                      WHEN DEP.DTYPE IN ('B') THEN 'TRIGGER'\r\n                    WHEN R.routinetype IN ('F', 'M') THEN 'FUNCTION'\r\n                      WHEN R.routinetype IN ('P') THEN 'PROCEDURE' \r\n                 END)", "(CASE WHEN DEP.BTYPE IN ('T','G','M') THEN 'TABLE'\r\n                      WHEN DEP.BTYPE IN ('V') THEN 'VIEW'\r\n                      WHEN DEP.BTYPE IN ('O') THEN 'PROCEDURE' \r\n                      WHEN DEP.BTYPE IN ('F') THEN 'FUNCTION' \r\n                     ELSE NULL\r\n                END)");
		string filterStringForDependencies4 = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("TRIM(DEP.BSCHEMA)", "TRIM(DEP.BNAME)", "(CASE WHEN DEP.BTYPE IN ('T','G','M') THEN 'TABLE'\r\n                      WHEN DEP.BTYPE IN ('V') THEN 'VIEW'\r\n                      WHEN DEP.BTYPE IN ('O') THEN 'PROCEDURE' \r\n                      WHEN DEP.BTYPE IN ('F') THEN 'FUNCTION' \r\n                     ELSE NULL\r\n                END)", "(CASE \r\n                      WHEN DEP.DTYPE IN ('B') THEN 'TRIGGER'\r\n                    WHEN R.routinetype IN ('F', 'M') THEN 'FUNCTION'\r\n                      WHEN R.routinetype IN ('P') THEN 'PROCEDURE' \r\n                 END)");
		string altFilterString = FilterRulesCollection.CombineDependenciesFilter(filterStringForDependencies3, filterStringForDependencies4);
		yield return "SELECT \r\n                              '" + host + "' as REFERENCING_SERVER,\r\n                         CASE \r\n                              WHEN DEP.DTYPE IN ('S','T') THEN 'TABLE'\r\n                              WHEN DEP.DTYPE IN ('V','W') THEN 'VIEW'\r\n                         END AS REFERENCING_TYPE,\r\n                         TRIM(DEP.TABSCHEMA) AS REFERENCING_SCHEMA_NAME,\r\n                         TRIM(CURRENT_SERVER) AS REFERENCING_DATABASE_NAME,\r\n                         TRIM(DEP.TABNAME) AS REFERENCING_ENTITY_NAME,\r\n                        '" + host + "' as REFERENCED_SERVER,\r\n                         TRIM(CURRENT_SERVER) AS REFERENCED_DATABASE_NAME,\r\n                         TRIM(DEP.BSCHEMA) AS REFERENCED_SCHEMA_NAME,\r\n                         CASE WHEN DEP.BTYPE IN ('S','T','G','U') THEN 'TABLE'\r\n                               WHEN DEP.BTYPE IN ('V','W') THEN 'VIEW'\r\n                             WHEN R.ROUTINETYPE IN ('F', 'M') THEN 'FUNCTION'\r\n                               WHEN R.ROUTINETYPE IN ('P') THEN 'PROCEDURE' \r\n                       ELSE NULL END AS REFERENCED_TYPE,\r\n                         TRIM(DEP.BNAME) AS REFERENCED_ENTITY_NAME,\r\n                         NULL AS IS_CALLER_DEPENDENT, \r\n                         NULL AS IS_AMBIGUOUS, \r\n                         NULL AS DEPENDENCY_TYPE  \r\n                    FROM SYSCAT.TABDEP DEP \r\n                    LEFT JOIN SYSCAT.ROUTINES R ON R.SPECIFICNAME = DEP.BNAME \r\n                          AND R.ROUTINESCHEMA = DEP.BSCHEMA\r\n                    WHERE DEP.TABSCHEMA NOT LIKE 'SYS%'\r\n                            AND TRIM(DEP.TABNAME) IS NOT NULL\r\n                            AND TRIM(DEP.BNAME) IS NOT NULL\r\n                          " + text;
		yield return "SELECT \r\n                              '" + host + "' as REFERENCING_SERVER,\r\n                         CASE \r\n                              WHEN DEP.DTYPE IN ('B') THEN 'TRIGGER'\r\n                             WHEN R.routinetype IN ('F', 'M') THEN 'FUNCTION'\r\n                               WHEN R.routinetype IN ('P') THEN 'PROCEDURE' \r\n                         END AS REFERENCING_TYPE,\r\n                         TRIM(DEP.DSCHEMA) AS  REFERENCING_SCHEMA_NAME,\r\n                         TRIM(CURRENT_SERVER) AS REFERENCING_DATABASE_NAME,\r\n                              CASE \r\n                            WHEN R.routinetype IN ('F','P', 'M') and il>1 THEN\r\n                              TRIM(R.ROUTINENAME) || ' [' || TRIM(R.SPECIFICNAME) || ']'\r\n                            WHEN R.routinetype IN ('F','P', 'M') and il is null THEN\r\n                              TRIM(R.ROUTINENAME) \r\n                         ELSE TRIM(DEP.DNAME) END as REFERENCING_ENTITY_NAME,\r\n                        '" + host + "' as REFERENCED_SERVER,\r\n                         TRIM(CURRENT_SERVER) AS REFERENCED_DATABASE_NAME,\r\n                         TRIM(DEP.BSCHEMA) AS REFERENCED_SCHEMA_NAME,\r\n                         CASE WHEN DEP.BTYPE IN ('T','G','M') THEN 'TABLE'\r\n                               WHEN DEP.BTYPE IN ('V') THEN 'VIEW'\r\n                               WHEN DEP.BTYPE IN ('O') THEN 'PROCEDURE' \r\n                               WHEN DEP.BTYPE IN ('F') THEN 'FUNCTION' \r\n                         ELSE NULL END AS REFERENCED_TYPE,\r\n                         CASE \r\n                                   --  K = Package \r\n                                   WHEN DEP.BTYPE IN ('F','O') and il>1 THEN\r\n                              TRIM(R3.ROUTINENAME) || ' [' || TRIM(R3.SPECIFICNAME) || ']'\r\n                              WHEN DEP.BTYPE IN ('F','O') and il is null THEN\r\n                              TRIM(R3.ROUTINENAME) \r\n                         ELSE TRIM(DEP.BNAME)\r\n                         END AS REFERENCED_ENTITY_NAME,\r\n                         NULL AS IS_CALLER_DEPENDENT, \r\n                         NULL AS IS_AMBIGUOUS,  \r\n                         NULL AS DEPENDENCY_TYPE  \r\n                    FROM  SYSIBM.SYSDEPENDENCIES DEP\r\n                    LEFT JOIN SYSCAT.ROUTINES R ON R.SPECIFICNAME = DEP.DNAME AND R.ROUTINESCHEMA = DEP.DSCHEMA\r\n                    LEFT JOIN SYSCAT.ROUTINES R3 ON R3.SPECIFICNAME = DEP.BNAME AND R3.ROUTINESCHEMA = DEP.BSCHEMA\r\n                    LEFT JOIN \r\n                         (\r\n                         SELECT ROUTINESCHEMA, ROUTINENAME , count(*) as il\r\n                         FROM\r\n                         SYSCAT.ROUTINES \r\n                         WHERE\r\n                            ROUTINETYPE IN ('F','P','M')\r\n                              AND ROUTINESCHEMA NOT LIKE 'SYS%' \r\n                         GROUP BY ROUTINESCHEMA, ROUTINENAME\r\n                         having count(*)>1\r\n                         ) R2\r\n                          on R2.ROUTINESCHEMA = R.ROUTINESCHEMA AND R2.ROUTINENAME = R.ROUTINENAME \r\n                    WHERE DEP.DTYPE IN('F','B','O') \r\n                          AND DEP.DSCHEMA NOT LIKE 'SYS%'\r\n                            AND                            \r\n                                CASE \r\n                                    WHEN R.routinetype IN ('F','P', 'M') and il>1 THEN\r\n                                    TRIM(R.ROUTINENAME) || ' [' || TRIM(R.SPECIFICNAME) || ']'\r\n                                    WHEN R.routinetype IN ('F','P', 'M') and il is null THEN\r\n                                    TRIM(R.ROUTINENAME) \r\n                                    ELSE TRIM(DEP.DNAME)\r\n                                    END IS NOT NULL\r\n                            AND \r\n                                CASE \r\n                                   --  K = Package \r\n                                    WHEN DEP.BTYPE IN ('F','O') and il>1 THEN\r\n                                    TRIM(R3.ROUTINENAME) || ' [' || TRIM(R3.SPECIFICNAME) || ']'\r\n                                    WHEN DEP.BTYPE IN ('F','O') and il is null THEN\r\n                                    TRIM(R3.ROUTINENAME) \r\n                                    ELSE TRIM(DEP.BNAME)\r\n                                    END IS NOT NULL\r\n                          " + altFilterString;
	}
}
