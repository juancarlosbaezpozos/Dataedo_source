using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.General;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Model.Data.Procedures.Parameters;
using Dataedo.Model.Extensions;
using Dataedo.Shared.Enums;
using Vertica.Data.VerticaClient;

namespace Dataedo.App.Data.Vertica;

internal class SynchronizeVertica : SynchronizeDatabase
{
	public SynchronizeVertica(SynchronizeParameters synchronizeParameters)
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
				using VerticaCommand verticaCommand = CommandsWithTimeout.Vertica(query.Query, synchronizeParameters.DatabaseRow.Connection);
				using VerticaDataReader verticaDataReader = verticaCommand.ExecuteReader();
				while (verticaDataReader.Read())
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					ReadCount(verticaDataReader);
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
					using VerticaCommand verticaCommand = CommandsWithTimeout.Vertica(query.Query, synchronizeParameters.DatabaseRow.Connection);
					using VerticaDataReader verticaDataReader = verticaCommand.ExecuteReader();
					while (verticaDataReader.Read())
					{
						if (synchronizeParameters.DbSynchLocker.IsCanceled)
						{
							return false;
						}
						string text = string.Empty;
						if (verticaDataReader.Field<string>("type") == "PROJECTION")
						{
							using VerticaCommand verticaCommand2 = CommandsWithTimeout.Vertica("SELECT TO_CHAR(EXPORT_OBJECTS('','" + verticaDataReader.Field<string>("schema") + "." + verticaDataReader.Field<string>("name") + "','false')) AS 'SCRIPT'", synchronizeParameters.DatabaseRow.Connection);
							using VerticaDataReader verticaDataReader2 = verticaCommand2.ExecuteReader();
							while (verticaDataReader2.Read())
							{
								text += verticaDataReader2.Field<string>("SCRIPT");
							}
						}
						AddDBObject(verticaDataReader, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, text, null, owner);
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
			using VerticaCommand verticaCommand = CommandsWithTimeout.Vertica(query, synchronizeParameters.DatabaseRow.Connection);
			using VerticaDataReader verticaDataReader = verticaCommand.ExecuteReader();
			while (verticaDataReader.Read())
			{
				AddRelation(verticaDataReader, SharedDatabaseTypeEnum.DatabaseType.Vertica);
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
			using VerticaCommand verticaCommand = CommandsWithTimeout.Vertica(query, synchronizeParameters.DatabaseRow.Connection);
			using VerticaDataReader verticaDataReader = verticaCommand.ExecuteReader();
			while (verticaDataReader.Read())
			{
				AddColumn(verticaDataReader);
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
		try
		{
			using VerticaCommand verticaCommand = CommandsWithTimeout.Vertica(query, synchronizeParameters.DatabaseRow.Connection);
			using VerticaDataReader verticaDataReader = verticaCommand.ExecuteReader();
			while (verticaDataReader.Read())
			{
				AddUniqueConstraint(verticaDataReader, SharedDatabaseTypeEnum.DatabaseType.Vertica);
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
			using VerticaCommand verticaCommand = CommandsWithTimeout.Vertica(query, synchronizeParameters.DatabaseRow.Connection);
			using VerticaDataReader verticaDataReader = verticaCommand.ExecuteReader();
			while (verticaDataReader.Read())
			{
				if (verticaDataReader.HasColumn("argument_types"))
				{
					string databaseName = verticaDataReader.Field<string>("database_name");
					string schemaName = verticaDataReader.Field<string>("procedure_schema");
					string procName = verticaDataReader.Field<string>("procedure_name");
					if (base.ParameterRows.Where((ParameterRow p) => p.DatabaseName == databaseName && p.ProcedureSchema == schemaName && p.ProcedureName == procName).Count() != 0)
					{
						continue;
					}
					int num = 0;
					string text = verticaDataReader["argument_types"] as string;
					if (text.Length != 0)
					{
						List<string> list = text.Split(',').ToList();
						num += list.Count;
						for (int i = 0; i < list.Count; i++)
						{
							ParameterSynchronizationObject parameterSynchronizationObject = new ParameterSynchronizationObject();
							parameterSynchronizationObject.DatabaseName = verticaDataReader.Field<string>("database_name");
							parameterSynchronizationObject.ProcedureName = verticaDataReader.Field<string>("procedure_name");
							parameterSynchronizationObject.ProcedureSchema = verticaDataReader.Field<string>("procedure_schema");
							parameterSynchronizationObject.Name = "$" + (i + 1);
							parameterSynchronizationObject.Position = i + 1;
							parameterSynchronizationObject.ParameterMode = "IN";
							parameterSynchronizationObject.Datatype = list[i].Trim();
							parameterSynchronizationObject.DataLength = verticaDataReader.Field<string>("data_length");
							parameterSynchronizationObject.Description = verticaDataReader.Field<string>("description");
							AddParameter(parameterSynchronizationObject);
						}
					}
					string text2 = verticaDataReader["returned_types"] as string;
					if (text2.Length != 0)
					{
						List<string> list2 = text2.Split(',').ToList();
						for (int j = 0; j < list2.Count; j++)
						{
							ParameterSynchronizationObject parameterSynchronizationObject2 = new ParameterSynchronizationObject();
							parameterSynchronizationObject2.DatabaseName = verticaDataReader.Field<string>("database_name");
							parameterSynchronizationObject2.ProcedureName = verticaDataReader.Field<string>("procedure_name");
							parameterSynchronizationObject2.ProcedureSchema = verticaDataReader.Field<string>("procedure_schema");
							parameterSynchronizationObject2.Name = "$" + (num + j + 1);
							parameterSynchronizationObject2.Position = num + j + 1;
							parameterSynchronizationObject2.ParameterMode = "OUT";
							parameterSynchronizationObject2.Datatype = list2[j].Trim();
							parameterSynchronizationObject2.DataLength = verticaDataReader.Field<string>("data_length");
							parameterSynchronizationObject2.Description = verticaDataReader.Field<string>("description");
							AddParameter(parameterSynchronizationObject2);
						}
					}
				}
				else
				{
					AddParameter(verticaDataReader);
				}
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
			using VerticaCommand verticaCommand = CommandsWithTimeout.Vertica(query, synchronizeParameters.DatabaseRow.Connection);
			using VerticaDataReader verticaDataReader = verticaCommand.ExecuteReader();
			while (verticaDataReader.Read())
			{
				AddDependency(verticaDataReader);
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
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("WHERE", null, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM(T.TABLE_SCHEMA)", "TRIM(T.TABLE_NAME)");
		yield return "SELECT 'TABLE' AS 'object_type',\r\n                                COUNT(*) AS 'count'\r\n                            FROM V_CATALOG.TABLES T\r\n                            " + filterString + ";";
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("WHERE", null, FilterObjectTypeEnum.FilterObjectType.View, "TRIM(V.TABLE_SCHEMA)", "TRIM(V.TABLE_NAME)");
		string projectionFilter = synchronizeParameters.DatabaseRow.Filter.GetFilterString("WHERE", null, FilterObjectTypeEnum.FilterObjectType.View, "TRIM(V.PROJECTION_SCHEMA)", "TRIM(V.PROJECTION_NAME)");
		yield return "SELECT \r\n                            'VIEW' AS 'object_type',\r\n                            COUNT(*) AS 'count'\r\n                            FROM V_CATALOG.VIEWS V\r\n                            " + filterString + ";";
		yield return "SELECT 'VIEW' AS 'object_type',\r\n                            COUNT(*) AS 'count'\r\n                            FROM V_CATALOG.PROJECTIONS V\r\n                            " + projectionFilter + "; ";
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("WHERE", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "TRIM(P.SCHEMA_NAME)", "TRIM(P.PROCEDURE_NAME)");
		yield return "SELECT 'PROCEDURE' AS 'object_type'\r\n                                ,COUNT(*) AS 'count'\r\n                            FROM V_CATALOG.USER_PROCEDURES P\r\n                            " + filterString + "; ";
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("WHERE", null, FilterObjectTypeEnum.FilterObjectType.Function, "TRIM(F.SCHEMA_NAME)", "TRIM(F.FUNCTION_NAME)");
		yield return "SELECT 'FUNCTION' AS 'object_type'\r\n                                ,COUNT(*) AS 'count'\r\n                            FROM V_CATALOG.USER_FUNCTIONS F\r\n                            " + filterString + "; ";
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM(T.TABLE_SCHEMA)", "TRIM(T.TABLE_NAME)");
		yield return "SELECT T.TABLE_NAME AS 'name'\r\n                                ,T.TABLE_SCHEMA AS 'schema'\r\n                                ,DBNAME() AS 'database_name'\r\n                                ,'TABLE' AS 'type'\r\n                                ,C.COMMENT AS 'description'\r\n                                ,T.TABLE_DEFINITION AS 'definition'\r\n                                ,TO_CHAR(T.CREATE_TIME ,'YYYY-MM-DD HH24:MI:SS') AS 'create_date'\r\n                                ,NULL AS 'modify_date'\r\n                                ,NULL AS 'function_type'\r\n                            FROM V_CATALOG.TABLES AS T\r\n                            LEFT JOIN V_CATALOG.COMMENTS AS C ON T.TABLE_ID = C.OBJECT_ID\r\n                                AND C.OBJECT_TYPE = 'TABLE'\r\n                            WHERE T.IS_SYSTEM_TABLE = 'FALSE'\r\n                                " + filterString + ";";
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "TRIM(V.TABLE_SCHEMA)", "TRIM(V.TABLE_NAME)");
		string projectionFilter = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "TRIM(V.PROJECTION_SCHEMA)", "TRIM(V.PROJECTION_NAME)");
		yield return "SELECT V.TABLE_NAME AS 'name'\r\n                                ,V.TABLE_SCHEMA AS 'schema'\r\n                                ,DBNAME() AS 'database_name'\r\n                                ,'VIEW' AS 'type'\r\n                                ,C.COMMENT AS 'description'\r\n                                ,V.VIEW_DEFINITION AS 'definition'\r\n                                ,TO_CHAR(V.CREATE_TIME ,'YYYY-MM-DD HH24:MI:SS') AS 'create_date'\r\n                                ,NULL AS 'modify_date'\r\n                                ,NULL AS 'function_type'\r\n                            FROM V_CATALOG.VIEWS V\r\n                            LEFT JOIN V_CATALOG.COMMENTS AS C ON V.TABLE_ID = C.OBJECT_ID\r\n                                AND C.OBJECT_TYPE = 'VIEW'\r\n                            WHERE V.IS_SYSTEM_VIEW = 'FALSE'\r\n                                " + filterString + ";";
		yield return "SELECT V.PROJECTION_NAME AS 'name'\r\n                                ,V.PROJECTION_SCHEMA AS 'schema'\r\n                                ,DBNAME() AS 'database_name'\r\n                                ,'PROJECTION' AS 'type'\r\n                                ,C.COMMENT AS 'description'\r\n                                ,'' AS 'definition'\r\n                                ,NULL AS 'create_date'\r\n                                ,NULL AS 'modify_date'\r\n                                ,NULL AS 'function_type'\r\n                            FROM V_CATALOG.PROJECTIONS V\r\n                            LEFT JOIN V_CATALOG.COMMENTS AS C ON V.PROJECTION_ID = C.OBJECT_ID\r\n                                AND C.OBJECT_TYPE = 'PROJECTION'\r\n                            WHERE V.CREATE_TYPE != 'SYSTEM TABLE'\r\n                                " + projectionFilter + ";";
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("WHERE", null, FilterObjectTypeEnum.FilterObjectType.Function, "TRIM(F.SCHEMA_NAME)", "TRIM(F.FUNCTION_NAME)");
		yield return "SELECT F.FUNCTION_NAME AS 'name'\r\n                                ,F.SCHEMA_NAME AS 'schema'\r\n                                ,DBNAME() AS 'database_name'\r\n                                ,CASE WHEN F.PROCEDURE_TYPE = 'Stored Procedure' THEN 'PROCEDURE'\r\n                                    ELSE 'FUNCTION' END AS 'type'\r\n                                ,MAX(F.COMMENT) AS 'description'\r\n                                ,MAX(F.FUNCTION_DEFINITION) AS 'definition'\r\n                                ,NULL AS 'create_date'\r\n                                ,NULL AS 'modify_date'\r\n                                ,NULL AS 'function_type'\r\n                            FROM V_CATALOG.USER_FUNCTIONS AS F\r\n                            " + filterString + "                                \r\n                            GROUP BY F.FUNCTION_NAME, F.SCHEMA_NAME, F.PROCEDURE_TYPE\r\n                                ;";
	}

	public override IEnumerable<string> RelationsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString(null, null, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM(F.TABLE_SCHEMA)", "TRIM(F.TABLE_NAME)");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("OR", null, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM(F.REFERENCE_TABLE_SCHEMA)", "TRIM(F.REFERENCE_TABLE_NAME)");
		string text = FilterRulesCollection.CombineRelationsFilter(filterString, filterString2);
		yield return "SELECT F.CONSTRAINT_NAME AS 'NAME'\r\n                                ,DBNAME() AS 'FK_TABLE_DATABASE_NAME'\r\n                                ,F.TABLE_NAME AS 'FK_TABLE_NAME'\r\n                                ,F.TABLE_SCHEMA AS 'FK_TABLE_SCHEMA'\r\n                                ,F.COLUMN_NAME AS 'FK_COLUMN'\r\n                                ,DBNAME() AS 'REF_TABLE_DATABASE_NAME'\r\n                                ,F.REFERENCE_TABLE_NAME AS 'REF_TABLE_NAME'\r\n                                ,F.REFERENCE_TABLE_SCHEMA AS 'REF_TABLE_SCHEMA'\r\n                                ,F.REFERENCE_COLUMN_NAME AS 'REF_COLUMN'\r\n                                ,F.ORDINAL_POSITION AS 'ORDINAL_POSITION'\r\n                                ,'' AS 'DESCRIPTION'\r\n                                ,'' AS 'UPDATE_RULE'\r\n                                ,'' AS 'DELETE_RULE'\r\n                            FROM V_CATALOG.FOREIGN_KEYS F\r\n                            WHERE 1 = 1\r\n                                " + text + ";";
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM(C.TABLE_SCHEMA)", "TRIM(C.TABLE_NAME)");
		string viewFilterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("WHERE", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.View, "TRIM(C.TABLE_SCHEMA)", "TRIM(C.TABLE_SCHEMA)");
		string projectionFilterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("WHERE", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.View, "TRIM(C.TABLE_SCHEMA)", "TRIM(C.PROJECTION_NAME)");
		yield return "SELECT DBNAME() AS 'database_name'\r\n                                ,C.TABLE_NAME AS 'table_name'\r\n                                ,C.TABLE_SCHEMA AS 'table_schema'\r\n                                ,C.COLUMN_NAME AS 'name'\r\n                                ,C.ORDINAL_POSITION AS 'position'\r\n                                ,SPLIT_PART(C.DATA_TYPE, '(', 1) AS 'datatype'\r\n                                ,NULL AS 'data_length'\r\n                                ,NULL AS 'constraint_type'\r\n                                ,NULL AS 'description'\r\n                                ,CASE CS.CONSTRAINT_TYPE\r\n                                    WHEN 'P'\r\n                                        THEN 'P'\r\n                                    ELSE NULL\r\n                                    END AS 'CONSTRAINT_TYPE'\r\n                                ,CASE \r\n                                    WHEN C.IS_NULLABLE = 'TRUE'\r\n                                        THEN 1\r\n                                    ELSE 0\r\n                                    END AS 'nullable'\r\n                                ,C.COLUMN_DEFAULT AS 'default_value'\r\n                                ,CASE C.IS_IDENTITY\r\n                                    WHEN 'TRUE'\r\n                                        THEN 1\r\n                                    ELSE 0\r\n                                    END AS 'is_identity'\r\n                                ,0 AS 'is_computed'\r\n                                ,NULL AS 'computed_formula'\r\n                            FROM V_CATALOG.COLUMNS AS C\r\n                            LEFT JOIN V_CATALOG.CONSTRAINT_COLUMNS AS CS ON C.COLUMN_NAME = CS.COLUMN_NAME\r\n                                AND CS.TABLE_NAME = CS.TABLE_NAME\r\n                                AND CS.CONSTRAINT_TYPE = 'P'\r\n                            WHERE C.IS_SYSTEM_TABLE = 'FALSE'\r\n                                " + filterString;
		yield return "\r\n                            SELECT DBNAME() AS 'database_name'\r\n                                ,C.TABLE_NAME AS 'table_name'\r\n                                ,C.TABLE_SCHEMA AS 'table_schema'\r\n                                ,C.COLUMN_NAME AS 'name'\r\n                                ,C.ORDINAL_POSITION AS 'position'\r\n                                ,SPLIT_PART(C.DATA_TYPE, '(', 1) AS 'datatype'\r\n                                ,NULL AS 'data_length'\r\n                                ,NULL AS 'description'\r\n                                ,NULL AS 'constraint_type'\r\n                                ,1 AS 'nullable'\r\n                                ,NULL AS 'default_value'\r\n                                ,0 AS 'is_identity'\r\n                                ,0 AS 'is_computed'\r\n                                ,NULL AS 'computed_formula'\r\n                            FROM V_CATALOG.VIEW_COLUMNS AS C\r\n                                " + viewFilterString + ";";
		yield return "SELECT DBNAME() AS 'database_name'\r\n                                ,C.PROJECTION_NAME AS 'table_name'\r\n                                ,P.PROJECTION_SCHEMA AS 'table_schema'\r\n                                ,C.PROJECTION_COLUMN_NAME AS 'name'\r\n                                ,C.COLUMN_POSITION + 1 AS 'position'\r\n                                ,SPLIT_PART(C.DATA_TYPE, '(', 1) AS 'datatype'\r\n                                ,NULL AS 'data_length'\r\n                                ,COM.COMMENT AS 'description'\r\n                                ,NULL AS 'constraint_type'\r\n                                ,1 AS 'nullable'\r\n                                ,NULL AS 'default_value'\r\n                                ,0 AS 'is_identity'\r\n                                ,0 AS 'is_computed'\r\n                                ,CASE \r\n                                    WHEN C.IS_EXPRESSION = 'TRUE'\r\n                                        THEN 1\r\n                                    WHEN C.IS_AGGREGATE = 'TRUE'\r\n                                        THEN 1\r\n                                    ELSE 0\r\n                                    END AS 'IS_COMPUTED'\r\n                                ,CASE \r\n                                    WHEN C.IS_EXPRESSION = 'TRUE'\r\n                                        THEN C.COLUMN_EXPRESSION\r\n                                    WHEN C.IS_AGGREGATE = 'TRUE'\r\n                                        THEN C.COLUMN_EXPRESSION\r\n                                    ELSE NULL\r\n                                    END AS 'computed_formula'\r\n                            FROM V_CATALOG.PROJECTION_COLUMNS AS C\r\n                            LEFT JOIN V_CATALOG.COMMENTS AS COM ON C.COLUMN_ID = COM.OBJECT_ID\r\n                                AND COM.OBJECT_TYPE = 'COLUMN'\r\n                           \tLEFT JOIN V_CATALOG.PROJECTIONS P ON P.PROJECTION_ID = C.PROJECTION_ID\r\n                            " + projectionFilterString + ";";
	}

	public override IEnumerable<string> TriggersQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "TRIM(CS.TABLE_SCHEMA)", "TRIM(CS.TABLE_NAME)");
		yield return "SELECT DBNAME() AS 'database_name'\r\n                                ,CS.TABLE_NAME AS 'table_name'\r\n                                ,CS.TABLE_SCHEMA AS 'table_schema'\r\n                                ,CS.CONSTRAINT_NAME AS 'name'\r\n                                ,CASE UPPER(CS.CONSTRAINT_TYPE)\r\n                                    WHEN 'P'\r\n                                        THEN 'P'\r\n                                    ELSE 'U'\r\n                                    END AS 'type'\r\n                                ,CS.COLUMN_NAME AS 'column_name'\r\n                                ,C.ORDINAL_POSITION AS 'column_ordinal'\r\n                                ,COM.COMMENT AS 'description'\r\n                                ,CASE CS.IS_ENABLED\r\n                                    WHEN TRUE\r\n                                        THEN 0\r\n                                    ELSE 1\r\n                                    END AS 'disabled'\r\n                            FROM V_CATALOG.CONSTRAINT_COLUMNS AS CS\r\n                            INNER JOIN V_CATALOG.COLUMNS AS C ON C.COLUMN_NAME = CS.COLUMN_NAME\r\n                                AND C.TABLE_NAME = CS.TABLE_NAME\r\n                                AND C.TABLE_SCHEMA = CS.TABLE_SCHEMA\r\n                            LEFT JOIN V_CATALOG.COMMENTS AS COM ON CS.CONSTRAINT_ID = COM.OBJECT_ID\r\n                                AND UPPER(COM.OBJECT_TYPE) = 'CONSTRAINT'\r\n                            WHERE C.IS_SYSTEM_TABLE = FALSE\r\n                                AND UPPER(CS.CONSTRAINT_TYPE) IN ('P','U')\r\n                                " + filterString + ";";
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		string functionFilter = synchronizeParameters.DatabaseRow.Filter.GetFilterString("WHERE", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Function, "TRIM(UF.SCHEMA_NAME)", "TRIM(UF.FUNCTION_NAME)");
		yield return "SELECT DISTINCT DBNAME() AS 'database_name'\r\n                                ,UF.FUNCTION_NAME AS 'procedure_name'\r\n                                ,UF.SCHEMA_NAME AS 'procedure_schema'\r\n                                ,UF.PARAMETER_NAME AS 'name'\r\n                                ,0 AS 'position'\r\n                                ,'IN' AS 'parameter_mode'\r\n                                ,SPLIT_PART(UF.DATA_TYPE, '(', 1) AS 'datatype'\r\n                                ,UF.DATA_TYPE_LENGTH AS 'data_length'\r\n                                ,'' AS 'description'\r\n                            FROM v_catalog.USER_FUNCTION_PARAMETERS UF\r\n                            " + functionFilter + ";";
		yield return "SELECT DBNAME() AS 'database_name'\r\n                                ,UF.FUNCTION_NAME AS 'procedure_name'\r\n                                ,UF.SCHEMA_NAME AS 'procedure_schema'\r\n                                ,'' AS 'name'\r\n                                ,'' AS 'position'\r\n                                ,'' AS 'parameter_mode'\r\n                                ,UF.FUNCTION_ARGUMENT_TYPE AS 'argument_types' \r\n                                ,UF.FUNCTION_RETURN_TYPE AS 'returned_types' \r\n                                ,'' AS 'datatype'\r\n                                ,NULL AS 'data_length'\r\n                                ,'' AS 'description'\r\n                            FROM V_CATALOG.USER_FUNCTIONS AS UF\r\n                            " + functionFilter + ";";
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		DatabaseVersionUpdate versionUpdate = synchronizeParameters.DatabaseRow.GetVersionUpdate();
		string text = synchronizeParameters.DatabaseRow.Host.ToUpper();
		string filterStringForDependencies = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("AL.SCHEMA_NAME", "AL.TABLE_NAME", "AL.TABLE_TYPE", "FAL.TABLE_TYPE");
		string filterStringForDependencies2 = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("FAL.SCHEMA_NAME", "FAL.TABLE_NAME", "FAL.TABLE_TYPE", "AL.TABLE_TYPE");
		string text2 = FilterRulesCollection.CombineDependenciesFilter(filterStringForDependencies, filterStringForDependencies2);
		if (versionUpdate.Version >= 10 || (versionUpdate.Version == 9 && versionUpdate.Update >= 2))
		{
			yield return "SELECT AL.TABLE_TYPE AS 'referencing_type'\r\n                                ,'" + text + "' AS 'referencing_server'\r\n                                ,AL.SCHEMA_NAME AS 'referencing_schema_name'\r\n                                ,DBNAME() AS 'referencing_database_name'\r\n                                ,AL.TABLE_NAME AS 'referencing_entity_name'\r\n                                ,'" + text + "' AS 'referenced_server'\r\n                                ,DBNAME() AS 'referenced_database_name'\r\n                                ,FAL.SCHEMA_NAME AS 'referenced_schema_name'\r\n                                ,FAL.TABLE_TYPE AS 'referenced_type'\r\n                                ,FAL.TABLE_NAME AS 'referenced_entity_name'\r\n                                ,'' AS 'is_caller_dependent'\r\n                                ,'' AS 'is_ambiguous'\r\n                                ,NULL AS 'dependency_type'\r\n                            FROM V_CATALOG.VIEW_TABLES VT\r\n                            INNER JOIN V_CATALOG.ALL_TABLES AS AL ON VT.TABLE_ID = AL.TABLE_ID\r\n                                AND VT.TABLE_SCHEMA = AL.SCHEMA_NAME\r\n                            INNER JOIN V_CATALOG.ALL_TABLES FAL ON VT.REFERENCE_TABLE_ID = FAL.TABLE_ID\r\n                                AND VT.TABLE_SCHEMA = FAL.SCHEMA_NAME\r\n                            WHERE 1 = 1\r\n                            AND AL.TABLE_NAME IS NOT NULL\r\n                            AND FAL.TABLE_NAME IS NOT NULL\r\n                                " + text2 + ";";
		}
	}
}
