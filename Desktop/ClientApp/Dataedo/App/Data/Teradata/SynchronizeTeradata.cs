using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Model.Extensions;
using Dataedo.Shared.Enums;
using Teradata.Client.Provider;

namespace Dataedo.App.Data.Teradata;

internal class SynchronizeTeradata : SynchronizeDatabase
{
	private string columnsView;

	private string SchemasListForCondition => synchronizeParameters.DatabaseRow?.GetSchemasListForCondition((string x) => x.Replace("'", "''"));

	public SynchronizeTeradata(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
		columnsView = "DBC.ColumnsV";
		if (base.synchronizeParameters.DatabaseRow.Param1 == true.ToString(CultureInfo.InvariantCulture))
		{
			columnsView = "DBC.ColumnsqV";
		}
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Counting objects to import");
		if (!string.IsNullOrWhiteSpace(query.Query))
		{
			try
			{
				using TdCommand tdCommand = CommandsWithTimeout.Teradata(query.Query, synchronizeParameters.DatabaseRow.Connection);
				using TdDataReader tdDataReader = tdCommand.ExecuteReader();
				while (tdDataReader.Read())
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					ReadCount(tdDataReader);
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
			backgroundWorkerManager.ReportProgress("Retrieving database's objects");
			try
			{
				if (!string.IsNullOrWhiteSpace(query.Query))
				{
					using TdCommand tdCommand = CommandsWithTimeout.Teradata(query.Query, synchronizeParameters.DatabaseRow.Connection);
					using TdDataReader tdDataReader = tdCommand.ExecuteReader();
					while (tdDataReader.Read())
					{
						if (synchronizeParameters.DbSynchLocker.IsCanceled)
						{
							return false;
						}
						string text = tdDataReader.Field<string>("TYPE");
						if ("PROCEDURE".Equals(text) || "FUNCTION".Equals(text) || "VIEW".Equals(text))
						{
							string script = GetScript(text, tdDataReader.Field<string>("SCHEMA"), tdDataReader.Field<string>("NAME"));
							ObservableCollection<ObjectRow> tableRows = synchronizeParameters.DatabaseRow.tableRows;
							Form owner2 = owner;
							AddDBObject(tdDataReader, tableRows, backgroundWorkerManager, script, null, owner2);
						}
						else
						{
							ObservableCollection<ObjectRow> tableRows2 = synchronizeParameters.DatabaseRow.tableRows;
							Form owner2 = owner;
							AddDBObject(tdDataReader, tableRows2, backgroundWorkerManager, null, null, owner2);
						}
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

	private string GetScript(string type, string schema, string name)
	{
		string text = string.Empty;
		string columnName = ((!"PROCEDURE".Equals(type)) ? "Request Text" : "RequestText");
		try
		{
			using (TdCommand tdCommand = CommandsWithTimeout.Teradata("SHOW " + type + " " + schema + "." + name + ";", synchronizeParameters.DatabaseRow.Connection))
			{
				tdCommand.CommandType = CommandType.Text;
				using TdDataReader tdDataReader = tdCommand.ExecuteReader();
				while (tdDataReader.Read())
				{
					text += tdDataReader.Field<string>(columnName);
				}
			}
			return text;
		}
		catch
		{
			return null;
		}
	}

	public override bool GetRelations(string query, Form owner = null)
	{
		try
		{
			using TdCommand tdCommand = CommandsWithTimeout.Teradata(query, synchronizeParameters.DatabaseRow.Connection);
			using TdDataReader tdDataReader = tdCommand.ExecuteReader();
			while (tdDataReader.Read())
			{
				AddRelation(tdDataReader, SharedDatabaseTypeEnum.DatabaseType.Teradata);
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
			using TdCommand tdCommand = CommandsWithTimeout.Teradata(query, synchronizeParameters.DatabaseRow.Connection);
			using TdDataReader tdDataReader = tdCommand.ExecuteReader();
			while (tdDataReader.Read())
			{
				AddColumn(tdDataReader);
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
			using TdCommand tdCommand = CommandsWithTimeout.Teradata(query, synchronizeParameters.DatabaseRow.Connection);
			using TdDataReader tdDataReader = tdCommand.ExecuteReader();
			while (tdDataReader.Read())
			{
				AddTrigger(tdDataReader);
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
			using TdCommand tdCommand = CommandsWithTimeout.Teradata(query, synchronizeParameters.DatabaseRow.Connection);
			using TdDataReader tdDataReader = tdCommand.ExecuteReader();
			while (tdDataReader.Read())
			{
				AddUniqueConstraint(tdDataReader, SharedDatabaseTypeEnum.DatabaseType.Teradata);
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
			using TdCommand tdCommand = CommandsWithTimeout.Teradata(query, synchronizeParameters.DatabaseRow.Connection);
			using TdDataReader tdDataReader = tdCommand.ExecuteReader();
			while (tdDataReader.Read())
			{
				AddParameter(tdDataReader);
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
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "T.DATABASENAME", "T.TABLENAME");
		yield return "SELECT 'TABLE' AS \"OBJECT_TYPE\",\r\n                                COUNT(*) AS \"COUNT\"\r\n                            FROM DBC.TABLESV T\r\n                            WHERE T.TABLEKIND = 'T'\r\n                            AND T.DATABASENAME IN (" + SchemasListForCondition + ")\r\n                                " + filterString + "; ";
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "T.DATABASENAME", "T.TABLENAME");
		yield return "SELECT 'VIEW' AS \"OBJECT_TYPE\",\r\n                                COUNT(*) AS \"COUNT\"\r\n                            FROM DBC.TABLESV T\r\n                            WHERE T.TABLEKIND = 'V'\r\n                            AND T.DATABASENAME IN (" + SchemasListForCondition + ")\r\n                                " + filterString + "; ";
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "T.DATABASENAME", "T.TABLENAME");
		yield return "SELECT 'PROCEDURE' AS \"OBJECT_TYPE\",\r\n                                COUNT(*) AS \"COUNT\"\r\n                            FROM DBC.TABLESV T\r\n                            WHERE T.TABLEKIND = 'P'\r\n                            AND T.DATABASENAME IN (" + SchemasListForCondition + ")\r\n                                " + filterString + "; ";
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "T.DATABASENAME", "T.TABLENAME");
		yield return "SELECT 'FUNCTION' AS \"OBJECT_TYPE\",\r\n                                COUNT(*) AS \"COUNT\"\r\n                            FROM DBC.TABLESV T\r\n                            WHERE T.TABLEKIND = 'F'\r\n                            AND T.DATABASENAME IN (" + SchemasListForCondition + ")\r\n                                " + filterString + "; ";
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "T.DATABASENAME", "T.TABLENAME");
		yield return "SELECT T.TABLENAME AS \"NAME\",\r\n                                T.DATABASENAME AS \"SCHEMA\",\r\n                                DATABASE AS \"DATABASE_NAME\",\r\n                                'TABLE' AS \"TYPE\",\r\n                                T.COMMENTSTRING AS \"DESCRIPTION\",\r\n                                '' AS \"DEFINITION\",\r\n                                T.CREATETIMESTAMP AS \"CREATE_DATE\",\r\n                                T.LASTALTERTIMESTAMP AS \"MODIFY_DATE\",\r\n                                NULL AS \"FUNCTION_TYPE\"\r\n                            FROM DBC.TABLESV AS T\r\n                            WHERE T.TABLEKIND = 'T'\r\n                            AND T.DATABASENAME IN (" + SchemasListForCondition + ")\r\n                                " + filterString + ";";
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "T.DATABASENAME", "T.TABLENAME");
		yield return "SELECT T.TABLENAME AS \"NAME\",\r\n                                T.DATABASENAME AS \"SCHEMA\",\r\n                                DATABASE AS \"DATABASE_NAME\",\r\n                                'VIEW' AS \"TYPE\",\r\n                                T.COMMENTSTRING AS \"DESCRIPTION\",\r\n                                T.REQUESTTEXT AS \"DEFINITION\",\r\n                                T.CREATETIMESTAMP AS \"CREATE_DATE\",\r\n                                T.LASTALTERTIMESTAMP AS \"MODIFY_DATE\",\r\n                                NULL AS \"FUNCTION_TYPE\"\r\n                            FROM DBC.TABLESV AS T\r\n                            WHERE T.TABLEKIND = 'V'\r\n                            AND T.DATABASENAME IN (" + SchemasListForCondition + ")\r\n                                " + filterString + ";";
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "T.DATABASENAME", "T.TABLENAME");
		yield return "SELECT T.TABLENAME AS \"NAME\",\r\n                                T.DATABASENAME AS \"SCHEMA\",\r\n                                DATABASE AS \"DATABASE_NAME\",\r\n                                'PROCEDURE' AS \"TYPE\",\r\n                                T.COMMENTSTRING AS \"DESCRIPTION\",\r\n                                T.REQUESTTEXT AS \"DEFINITION\",\r\n                                T.CREATETIMESTAMP AS \"CREATE_DATE\",\r\n                                T.LASTALTERTIMESTAMP AS \"MODIFY_DATE\",\r\n                                NULL AS \"FUNCTION_TYPE\"\r\n                            FROM DBC.TABLESV AS T\r\n                            WHERE T.TABLEKIND = 'P'\r\n                            AND T.DATABASENAME IN (" + SchemasListForCondition + ")\r\n                                " + filterString + ";";
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "T.DATABASENAME", "T.TABLENAME");
		yield return "SELECT F.FUNCTIONNAME AS \"NAME\",\r\n                                T.DATABASENAME AS \"SCHEMA\",\r\n                                DATABASE AS \"DATABASE_NAME\",\r\n                                'FUNCTION' AS \"TYPE\",\r\n                                F.FUNCTIONTYPE AS \"FUNCTION_TYPE\",\r\n                                T.COMMENTSTRING AS \"DESCRIPTION\",\r\n                                T.REQUESTTEXT AS \"DEFINITION\",\r\n                                T.CREATETIMESTAMP AS \"CREATE_DATE\",\r\n                                T.LASTALTERTIMESTAMP AS \"MODIFY_DATE\"\r\n                            FROM DBC.FUNCTIONSV F\r\n                            INNER JOIN DBC.TABLESV T ON F.DATABASENAME = T.DATABASENAME\r\n                                AND F.FUNCTIONNAME = T.TABLENAME\r\n                                AND F.DATABASENAME IN (" + SchemasListForCondition + ")\r\n                                " + filterString + ";";
	}

	public override IEnumerable<string> RelationsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString(null, base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM (T.CHILDDB)", "TRIM (T.CHILDTABLE)");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("OR", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM (T.PARENTTABLE) ", "TRIM (T.PARENTDB)");
		string text = FilterRulesCollection.CombineRelationsFilter(filterString, filterString2);
		yield return "SELECT COALESCE(T.INDEXNAME,'') AS \"NAME\",\r\n                                DATABASE AS \"FK_TABLE_DATABASE_NAME\",\r\n                                TRIM (T.CHILDTABLE) AS \"FK_TABLE_NAME\",\r\n                                TRIM (T.CHILDDB) AS \"FK_TABLE_SCHEMA\",\r\n                                TRIM (T.CHILDKEYCOLUMN) AS \"FK_COLUMN\",\r\n                                DATABASE AS \"REF_TABLE_DATABASE_NAME\",\r\n                                TRIM (T.PARENTTABLE) AS \"REF_TABLE_NAME\",\r\n                                TRIM (T.PARENTDB) AS \"REF_TABLE_SCHEMA\",\r\n                                TRIM (T.PARENTKEYCOLUMN) AS \"REF_COLUMN\",\r\n                                0 AS \"ORDINAL_POSITION\",\r\n                                '' AS \"DESCRIPTION\",\r\n                                '' AS \"UPDATE_RULE\",\r\n                                '' AS \"DELETE_RULE\"\r\n                            FROM DBC.ALL_RI_CHILDREN AS T\r\n                            WHERE T.CHILDDB IN (" + SchemasListForCondition + ")\r\n                            " + text;
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "T.DATABASENAME", "T.TABLENAME");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.View, "T.DATABASENAME", "T.TABLENAME");
		yield return "SELECT C.    TABLENAME AS \"TABLE_NAME\",\r\n                            C.DATABASENAME AS \"TABLE_SCHEMA\",\r\n                            C.COLUMNNAME AS \"NAME\",\r\n                              DATABASE as \"DATABASE_NAME\",\r\n                            ROW_NUMBER() OVER (PARTITION BY C.DATABASENAME, C.TABLENAME ORDER BY C.COLUMNID)  AS \"POSITION\",\r\n                            CASE COLUMNTYPE\r\n                                WHEN 'BF'\r\n                                    THEN 'BYTE'\r\n                                WHEN 'BV'\r\n                                    THEN 'VARBYTE'\r\n                                WHEN 'CF'\r\n                                    THEN 'CHAR'\r\n                                WHEN 'CV'\r\n                                    THEN 'VARCHAR'\r\n                                WHEN 'D '\r\n                                    THEN 'DECIMAL'\r\n                                WHEN 'DA'\r\n                                    THEN 'DATE'\r\n                                WHEN 'F '\r\n                                    THEN 'FLOAT'\r\n                                WHEN 'I1'\r\n                                    THEN 'BYTEINT'\r\n                                WHEN 'I2'\r\n                                    THEN 'SMALLINT'\r\n                                WHEN 'I8'\r\n                                    THEN 'BIGINT'\r\n                                WHEN 'I '\r\n                                    THEN 'INTEGER'\r\n                                WHEN 'AT'\r\n                                    THEN 'TIME'\r\n                                WHEN 'TS'\r\n                                    THEN 'TIMESTAMP'\r\n                                WHEN 'TZ'\r\n                                    THEN 'TIME WITH TIME ZONE'\r\n                                WHEN 'SZ'\r\n                                    THEN 'TIMESTAMP WITH TIME ZONE'\r\n                                WHEN 'YR'\r\n                                    THEN 'INTERVAL YEAR'\r\n                                WHEN 'YM'\r\n                                    THEN 'INTERVAL YEAR TO MONTH'\r\n                                WHEN 'MO'\r\n                                    THEN 'INTERVAL MONTH'\r\n                                WHEN 'DY'\r\n                                    THEN 'INTERVAL DAY'\r\n                                WHEN 'DH'\r\n                                    THEN 'INTERVAL DAY TO HOUR'\r\n                                WHEN 'DM'\r\n                                    THEN 'INTERVAL DAY TO MINUTE'\r\n                                WHEN 'DS'\r\n                                    THEN 'INTERVAL DAY'\r\n                                WHEN 'HR'\r\n                                    THEN 'INTERVAL HOUR'\r\n                                WHEN 'HM'\r\n                                    THEN 'INTERVAL HOUR TO MINUTE'\r\n                                WHEN 'HS'\r\n                                    THEN 'INTERVAL HOUR'\r\n                                WHEN 'MI'\r\n                                    THEN 'INTERVAL MINUTE'\r\n                                WHEN 'MS'\r\n                                    THEN 'INTERVAL MINUTE TO SECOND'\r\n                                WHEN 'SC'\r\n                                    THEN 'INTERVAL SECOND'\r\n                                WHEN 'BO'\r\n                                    THEN 'BLOB'\r\n                                WHEN 'CO'\r\n                                    THEN 'CLOB'\r\n                                WHEN 'PD'\r\n                                    THEN 'PERIOD(DATE)'\r\n                                WHEN 'PM'\r\n                                    THEN 'PERIOD(TIMESTAMP) WITH TIME ZONE'\r\n                                WHEN 'PS'\r\n                                    THEN 'PERIOD(TIMESTAMP)'\r\n                                WHEN 'PT'\r\n                                    THEN 'PERIOD(TIME)'\r\n                                WHEN 'PZ'\r\n                                    THEN 'PERIOD(TIME) WITH TIME ZONE'\r\n                                WHEN 'UT'\r\n                                    THEN COALESCE(COLUMNUDTNAME, '<UNKNOWN> ' || COLUMNTYPE)\r\n                                WHEN '++'\r\n                                    THEN 'TD_ANYTYPE'\r\n                                WHEN 'N'\r\n                                    THEN 'NUMBER'\r\n                                WHEN 'A1'\r\n                                    THEN COALESCE('SYSUDTLIB.' || COLUMNUDTNAME, '<UNKNOWN> ' || COLUMNTYPE)\r\n                                WHEN 'AN'\r\n                                    THEN COALESCE('SYSUDTLIB.' || COLUMNUDTNAME, '<UNKNOWN> ' || COLUMNTYPE)\r\n                                ELSE '<UNKNOWN> ' || COLUMNTYPE\r\n                                END AS \"DATATYPE\",\r\n                            C.COMMENTSTRING AS \"DESCRIPTION\",\r\n                            NULL AS \"CONSTRAINT_TYPE\",\r\n                            CASE \r\n                                WHEN C.NULLABLE = 'Y'\r\n                                    THEN 1\r\n                                ELSE 0\r\n                                END AS \"NULLABLE\",\r\n                            C.DEFAULTVALUE AS \"DEFAULT_VALUE\",\r\n                            C.COLUMNLENGTH AS \"DATA_LENGTH\",\r\n                            NVL2(C.IDCOLTYPE, 1, 0) AS \"IS_IDENTITY\",\r\n                            0 AS \"IS_COMPUTED\",\r\n                            '' AS \"COMPUTED_FORMULA\"\r\n                        FROM " + columnsView + " C\r\n                        LEFT JOIN DBC.TABLESV AS T\r\n                            ON C.TABLENAME = T.TABLENAME AND C.DATABASENAME = T.DATABASENAME\r\n                        WHERE C.DATABASENAME IN (" + SchemasListForCondition + ")\r\n                            AND ((T.TABLEKIND = 'V' " + filterString2 + ") OR (T.TABLEKIND = 'T' " + filterString + "))\r\n                        ORDER BY C.DATABASENAME,\r\n                            C.TABLENAME,\r\n                            C.COLUMNID;";
	}

	public override IEnumerable<string> TriggersQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "T.DATABASENAME", "T.TABLENAME");
		yield return "SELECT TRIM(T.TRIGGERNAME) AS \"TRIGGER_NAME\",\r\n                            TRIM( T.DATABASENAME ) AS \"TABLE_SCHEMA\",\r\n                            TRIM( T.TABLENAME ) AS \"TABLE_NAME\",\r\n                            TRIM( DATABASE ) AS \"DATABASE_NAME\",\r\n                            CASE T.EVENT\r\n                                WHEN 'U'\r\n                                    THEN 1\r\n                                ELSE 0\r\n                                END AS ISUPDATE,\r\n                            CASE T.EVENT\r\n                                WHEN 'D'\r\n                                    THEN 1\r\n                                ELSE 0\r\n                                END AS ISDELETE,\r\n                            CASE T.EVENT\r\n                                WHEN 'I'\r\n                                    THEN 1\r\n                                ELSE 0\r\n                                END AS ISINSERT,\r\n                            CASE T.ACTIONTIME\r\n                                WHEN 'A'\r\n                                    THEN 1\r\n                                ELSE 0\r\n                                END AS ISAFTER,\r\n                            0 AS ISINSTEADOF,\r\n                            0 AS ISBEFORE,\r\n                            CASE T.ENABLEDFLAG\r\n                                WHEN 'Y'\r\n                                    THEN 0\r\n                                ELSE 1\r\n                                END AS \"DISABLED\",\r\n                            T.REQUESTTEXT AS \"DEFINITION\",\r\n                            T.TRIGGERCOMMENT AS \"DESCRIPTION\",\r\n                            '' AS \"TYPE\"\r\n                        FROM DBC.TRIGGERS T\r\n                        WHERE T.DATABASENAME IN (" + SchemasListForCondition + ")\r\n                        " + filterString + "\r\n                    ;";
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "T.DATABASENAME", "T.TABLENAME");
		yield return "SELECT DATABASE AS \"DATABASE_NAME\",\r\n                            T.TABLENAME AS \"TABLE_NAME\",\r\n                            T.DATABASENAME AS \"TABLE_SCHEMA\",\r\n                            COALESCE(T.INDEXNAME, '') AS \"NAME\",\r\n                            CASE T.INDEXTYPE\r\n                                WHEN 'K' THEN 'P'\r\n                            ELSE 'U'\r\n                            END AS \"TYPE\",                            \r\n                            T.COLUMNNAME AS \"COLUMN_NAME\",\r\n                            T.COLUMNPOSITION AS \"COLUMN_ORDINAL\",\r\n                            '' AS \"DESCRIPTION\",\r\n                            0 AS \"DISABLED\"\r\n                        FROM DBC.INDICESV T\r\n                        WHERE UNIQUEFLAG = 'Y'\r\n                            AND T.DATABASENAME IN (" + SchemasListForCondition + ")\r\n                            " + filterString;
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Procedure, "T.DATABASENAME", "T.TABLENAME");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Function, "T.DATABASENAME", "T.TABLENAME");
		yield return "SELECT DATABASE AS \"DATABASE_NAME\",\r\n                            C.TABLENAME AS \"PROCEDURE_NAME\",\r\n                            C.DATABASENAME AS \"PROCEDURE_SCHEMA\",\r\n                            C.COLUMNNAME AS \"NAME\",\r\n                            ROW_NUMBER() OVER (PARTITION BY C.DATABASENAME, C.TABLENAME ORDER BY C.COLUMNID)  AS \"POSITION\",\r\n                            CASE C.SPPARAMETERTYPE\r\n                                WHEN 'I'\r\n                                    THEN 'IN'\r\n                                WHEN 'O'\r\n                                    THEN 'OUT'\r\n                                ELSE 'INOUT'\r\n                                END AS \"PARAMETER_MODE\",\r\n                            CASE COLUMNTYPE\r\n                                WHEN 'BF'\r\n                                    THEN 'BYTE'\r\n                                WHEN 'BV'\r\n                                    THEN 'VARBYTE'\r\n                                WHEN 'CF'\r\n                                    THEN 'CHAR'\r\n                                WHEN 'CV'\r\n                                    THEN 'VARCHAR'\r\n                                WHEN 'D '\r\n                                    THEN 'DECIMAL'\r\n                                WHEN 'DA'\r\n                                    THEN 'DATE'\r\n                                WHEN 'F '\r\n                                    THEN 'FLOAT'\r\n                                WHEN 'I1'\r\n                                    THEN 'BYTEINT'\r\n                                WHEN 'I2'\r\n                                    THEN 'SMALLINT'\r\n                                WHEN 'I8'\r\n                                    THEN 'BIGINT'\r\n                                WHEN 'I '\r\n                                    THEN 'INTEGER'\r\n                                WHEN 'AT'\r\n                                    THEN 'TIME'\r\n                                WHEN 'TS'\r\n                                    THEN 'TIMESTAMP'\r\n                                WHEN 'TZ'\r\n                                    THEN 'TIME WITH TIME ZONE'\r\n                                WHEN 'SZ'\r\n                                    THEN 'TIMESTAMP WITH TIME ZONE'\r\n                                WHEN 'YR'\r\n                                    THEN 'INTERVAL YEAR'\r\n                                WHEN 'YM'\r\n                                    THEN 'INTERVAL YEAR TO MONTH'\r\n                                WHEN 'MO'\r\n                                    THEN 'INTERVAL MONTH'\r\n                                WHEN 'DY'\r\n                                    THEN 'INTERVAL DAY'\r\n                                WHEN 'DH'\r\n                                    THEN 'INTERVAL DAY TO HOUR'\r\n                                WHEN 'DM'\r\n                                    THEN 'INTERVAL DAY TO MINUTE'\r\n                                WHEN 'DS'\r\n                                    THEN 'INTERVAL DAY'\r\n                                WHEN 'HR'\r\n                                    THEN 'INTERVAL HOUR'\r\n                                WHEN 'HM'\r\n                                    THEN 'INTERVAL HOUR TO MINUTE'\r\n                                WHEN 'HS'\r\n                                    THEN 'INTERVAL HOUR'\r\n                                WHEN 'MI'\r\n                                    THEN 'INTERVAL MINUTE'\r\n                                WHEN 'MS'\r\n                                    THEN 'INTERVAL MINUTE TO SECOND'\r\n                                WHEN 'SC'\r\n                                    THEN 'INTERVAL SECOND'\r\n                                WHEN 'BO'\r\n                                    THEN 'BLOB'\r\n                                WHEN 'CO'\r\n                                    THEN 'CLOB'\r\n                                WHEN 'PD'\r\n                                    THEN 'PERIOD(DATE)'\r\n                                WHEN 'PM'\r\n                                    THEN 'PERIOD(TIMESTAMP) WITH TIME ZONE'\r\n                                WHEN 'PS'\r\n                                    THEN 'PERIOD(TIMESTAMP)'\r\n                                WHEN 'PT'\r\n                                    THEN 'PERIOD(TIME)'\r\n                                WHEN 'PZ'\r\n                                    THEN 'PERIOD(TIME) WITH TIME ZONE'\r\n                                WHEN 'UT'\r\n                                    THEN COALESCE(COLUMNUDTNAME, '<UNKNOWN> ' || COLUMNTYPE)\r\n                                WHEN '++'\r\n                                    THEN 'TD_ANYTYPE'\r\n                                WHEN 'N'\r\n                                    THEN 'NUMBER'\r\n                                WHEN 'A1'\r\n                                    THEN COALESCE('SYSUDTLIB.' || COLUMNUDTNAME, '<UNKNOWN> ' || COLUMNTYPE)\r\n                                WHEN 'AN'\r\n                                    THEN COALESCE('SYSUDTLIB.' || COLUMNUDTNAME, '<UNKNOWN> ' || COLUMNTYPE)\r\n                                ELSE '<UNKNOWN> ' || COLUMNTYPE\r\n                                END AS \"DATATYPE\",\r\n                            C.COLUMNLENGTH AS \"DATA_LENGTH\",\r\n                            '' AS \"DESCRIPTION\"\r\n                        FROM DBC.COLUMNSV AS C\r\n                        LEFT JOIN DBC.TABLESV AS T\r\n                            ON C.TABLENAME = T.TABLENAME AND C.DATABASENAME = T.DATABASENAME\r\n                        WHERE C.SPPARAMETERTYPE IN ('I', 'O', 'B')\r\n                            AND C.DATABASENAME IN (" + SchemasListForCondition + ")  \r\n                            AND ((T.TABLEKIND = 'F' " + filterString2 + " ) OR (T.TABLEKIND = 'P' " + filterString + " ))\r\n                        ORDER BY C.DATABASENAME,\r\n                            C.TABLENAME,\r\n                            C.COLUMNID;";
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		yield return string.Empty;
	}
}
