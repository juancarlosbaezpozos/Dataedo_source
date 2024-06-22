using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.QueryTools;
using Dataedo.App.Enums;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Model.Data.Snowlfake;
using Dataedo.Shared.Enums;
using Snowflake.Data.Client;

namespace Dataedo.App.Data.Snowflake;

internal class SynchronizeSnowflake : SynchronizeDatabase
{
	private const string SHOW_PRIMARY_KEYS_QUERY_START = "SHOW PRIMARY KEYS";

	private const string SHOW_UNIQUE_KEYS_QUERY_START = "SHOW UNIQUE KEYS";

	private string DatabaseNameToQuery => synchronizeParameters.DatabaseName.Replace("'", "''");

	public SynchronizeSnowflake(SynchronizeParameters synchronizeParameters)
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
				using IDbCommand dbCommand = CommandsWithTimeout.Snowflake(query.Query, synchronizeParameters.DatabaseRow.Connection);
				using IDataReader dataReader = dbCommand.ExecuteReader();
				while (dataReader.Read())
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					ReadCount(dataReader as DbDataReader);
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
					using IDbCommand dbCommand = CommandsWithTimeout.Snowflake(query.Query, synchronizeParameters.DatabaseRow.Connection);
					using IDataReader dataReader = dbCommand.ExecuteReader();
					while (dataReader.Read())
					{
						if (synchronizeParameters.DbSynchLocker.IsCanceled)
						{
							return false;
						}
						AddDBObject(dataReader, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
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
			using IDbCommand dbCommand = CommandsWithTimeout.Snowflake(query, synchronizeParameters.DatabaseRow.Connection);
			using IDataReader dataReader = dbCommand.ExecuteReader();
			while (dataReader.Read())
			{
				AddRelation(dataReader, SharedDatabaseTypeEnum.DatabaseType.Snowflake);
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
			using IDbCommand dbCommand = CommandsWithTimeout.Snowflake(query, synchronizeParameters.DatabaseRow.Connection);
			using IDataReader dataReader = dbCommand.ExecuteReader();
			while (dataReader.Read())
			{
				AddColumn(dataReader);
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
			using IDbCommand dbCommand = CommandsWithTimeout.Snowflake(query, synchronizeParameters.DatabaseRow.Connection);
			using IDataReader dataReader = dbCommand.ExecuteReader();
			while (dataReader.Read())
			{
				if (query.StartsWith("SHOW PRIMARY KEYS"))
				{
					AddUniqueConstraint(new SnowflakeConstraint(dataReader as DbDataReader, isPk: true), SharedDatabaseTypeEnum.DatabaseType.Snowflake);
				}
				else if (query.StartsWith("SHOW UNIQUE KEYS"))
				{
					AddUniqueConstraint(new SnowflakeConstraint(dataReader as DbDataReader, isPk: false), SharedDatabaseTypeEnum.DatabaseType.Snowflake);
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

	public override bool GetParameters(string query, bool canReadParameters = true, Form owner = null)
	{
		if (!canReadParameters)
		{
			return true;
		}
		try
		{
			using IDbCommand dbCommand = CommandsWithTimeout.Snowflake(query, synchronizeParameters.DatabaseRow.Connection);
			using IDataReader dataReader = dbCommand.ExecuteReader();
			while (dataReader.Read())
			{
				AddParameter(dataReader);
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
			using IDbCommand dbCommand = CommandsWithTimeout.Snowflake(query, synchronizeParameters.DatabaseRow.Connection);
			using IDataReader dataReader = dbCommand.ExecuteReader();
			while (dataReader.Read())
			{
				AddDependency(dataReader);
			}
		}
		catch (SnowflakeDbException ex)
		{
			if (!string.IsNullOrEmpty(ex.Message) && (ex.Message.IndexOf("SNOWFLAKE", StringComparison.OrdinalIgnoreCase) != -1 || ex.Message.IndexOf("ACCOUNT_USAGE", StringComparison.OrdinalIgnoreCase) != -1 || ex.Message.IndexOf("OBJECT_DEPENDENCIES", StringComparison.OrdinalIgnoreCase) != -1))
			{
				return true;
			}
			throw;
		}
		catch (Exception ex2)
		{
			GeneralExceptionHandling.Handle(ex2, ex2.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Table, "t.table_schema", "t.table_name");
		yield return "select \r\n                        case when t.table_type = 'BASE TABLE' then 'TABLE' when t.table_type = 'EXTERNAL TABLE' then 'EXTERNAL_TABLE' END as \"object_type\",\r\n                        ROW_NUMBER() over (order by T.TABLE_NAME ASC) as \"count\"\r\n                    from information_schema.tables t\r\n                    where t.table_type in ('BASE TABLE', 'EXTERNAL TABLE') \r\n                        and t.table_schema <> 'INFORMATION_SCHEMA'\r\n                        and t.table_catalog='" + DatabaseNameToQuery + "' \r\n                        " + filterString;
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.View, "v.table_schema", "v.table_name");
		yield return "select 'view' as \"object_type\",\r\n                        count(1) as \"count\"\r\n                    from information_schema.tables v\r\n                    where v.table_type='VIEW'\r\n                        and v.table_schema <> 'INFORMATION_SCHEMA'\r\n                        and v.table_catalog='" + DatabaseNameToQuery + "' \r\n                        " + filterString;
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "p.procedure_schema", "p.procedure_name");
		yield return "select 'procedure' as \"object_type\",\r\n                        count(1) as \"count\"\r\n                    from information_schema.procedures p\r\n                    where p.procedure_catalog = '" + DatabaseNameToQuery + "'\r\n                        " + filterString;
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Function, "f.function_schema", "f.function_name");
		yield return "select 'function' as \"object_type\",\r\n                        count(1) as \"count\"\r\n                    from information_schema.functions f\r\n                    where f.function_catalog = '" + DatabaseNameToQuery + "'\r\n                        " + filterString;
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Table, "i.table_schema", "i.table_name");
		yield return "SELECT i.table_name AS \"name\"\r\n                    , i.table_schema AS \"schema\"\r\n                    , i.table_catalog AS \"database_name\"\r\n                    , case when i.table_type = 'BASE TABLE' then 'TABLE' when i.table_type = 'EXTERNAL TABLE' then 'EXTERNAL_TABLE' else NULL END AS \"type\"\r\n\t                --, null as \"language\" -- language for functions, null otherwise\r\n                    , CASE i.comment\r\n                        WHEN ''\r\n                            THEN NULL -- replace empty comment with null\r\n                        ELSE i.comment\r\n                        END AS \"description\"\r\n                    , NULL AS \"definition\"\r\n                    , TO_CHAR(i.created, 'YYYY-MM-DD HH24:MI:SS') AS \"create_date\"\r\n                    , CASE \r\n                        WHEN i.last_altered IS NULL\r\n                            THEN TO_CHAR(i.created, 'YYYY-MM-DD HH24:MI:SS')\r\n                        ELSE TO_CHAR(i.last_altered, 'YYYY-MM-DD HH24:MI:SS')\r\n                        END AS \"modify_date\"\r\n                    , NULL AS \"function_type\"\r\n                FROM information_schema.tables i\r\n                WHERE i.table_type in( 'BASE TABLE', 'EXTERNAL TABLE')\r\n                    AND i.table_catalog = '" + DatabaseNameToQuery + "'\r\n                    AND i.table_schema <> 'INFORMATION_SCHEMA'\r\n                    " + filterString;
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.View, "i.table_schema", "i.table_name");
		yield return "SELECT i.table_name AS \"name\"\r\n                    , i.table_schema AS \"schema\"\r\n                    , i.table_catalog AS \"database_name\"\r\n                    , 'VIEW' AS \"type\"\r\n\t                --, null as \"language\"\r\n                    , CASE i.comment\r\n                        WHEN ''\r\n                            THEN NULL -- replace empty comment with null\r\n                        ELSE i.comment\r\n                        END AS \"description\"\r\n                    , case when v.view_definition='' then NULL else v.view_definition end AS \"definition\"\r\n                    , TO_CHAR(i.created, 'YYYY-MM-DD HH24:MI:SS') AS \"create_date\"\r\n                    , CASE \r\n                        WHEN i.last_altered IS NULL\r\n                            THEN TO_CHAR(i.created, 'YYYY-MM-DD HH24:MI:SS')\r\n                        ELSE TO_CHAR(i.last_altered, 'YYYY-MM-DD HH24:MI:SS')\r\n                        END AS \"modify_date\"\r\n                    , NULL AS \"function_type\"\r\n                FROM information_schema.tables i\r\n                LEFT JOIN information_schema.VIEWS v\r\n                    ON v.table_schema = i.table_schema\r\n                        AND v.table_name = i.table_name\r\n                WHERE i.table_type = 'VIEW'\r\n                    AND i.table_catalog = '" + DatabaseNameToQuery + "'\r\n                    AND v.table_catalog = '" + DatabaseNameToQuery + "'\r\n                    AND i.table_schema <> 'INFORMATION_SCHEMA'\r\n                    " + filterString;
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "i.procedure_schema", "i.procedure_name");
		yield return "SELECT i.procedure_name AS \"name\"\r\n                   , i.procedure_schema AS \"schema\"\r\n                   , i.procedure_catalog AS \"database_name\"\r\n                   , 'PROCEDURE' AS \"type\"\r\n                   --, function_language as \"language\"\r\n                   , CASE i.comment\r\n                       WHEN ''\r\n                           THEN NULL --brak komentarza(opisu) pokazuje jako '', nie jako null, w sql serverze null przy braku komentarza(opisu)\r\n                       ELSE i.comment-- jesli null albo slowny komentarz\r\n                      END AS \"description\"\r\n                   , i.procedure_definition AS \"definition\"\r\n                   , --null dla tabel, dla pozostalych skrypt\r\n                   TO_CHAR(i.created, 'YYYY-MM-DD HH24:MI:SS') AS \"create_date\"\r\n                   , CASE\r\n                       WHEN i.last_altered IS NULL\r\n                           THEN TO_CHAR(i.created, 'YYYY-MM-DD HH24:MI:SS')\r\n                       ELSE TO_CHAR(i.last_altered, 'YYYY-MM-DD HH24:MI:SS')\r\n                       END AS \"modify_date\"\r\n                   , '......' AS \"function_type\"\r\n               FROM information_schema.procedures i\r\n               WHERE i.procedure_catalog = '" + DatabaseNameToQuery + "'\r\n                   AND i.procedure_schema <> 'INFORMATION_SCHEMA'\r\n                " + filterString;
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Function, "i.function_schema", "i.function_name");
		yield return "SELECT i.function_name AS \"name\"\r\n                    , i.function_schema AS \"schema\"\r\n                    , i.function_catalog AS \"database_name\"\r\n                    , 'FUNCTION' AS \"type\"\r\n                    --, function_language as \"language\"\r\n                    , CASE i.comment\r\n                        WHEN ''\r\n                            THEN NULL -- brak komentarza (opisu) pokazuje jako '', nie jako null, w sql serverze null przy braku komentarza (opisu)\r\n                        ELSE i.comment -- jesli null albo slowny komentarz\r\n                        END AS \"description\"\r\n                    , i.function_definition AS \"definition\"\r\n                    , --null dla tabel, dla pozostalych skrypt\r\n                    TO_CHAR(i.created, 'YYYY-MM-DD HH24:MI:SS') AS \"create_date\"\r\n                    , CASE\r\n                        WHEN i.last_altered IS NULL\r\n                            THEN TO_CHAR(i.created, 'YYYY-MM-DD HH24:MI:SS')\r\n                        ELSE TO_CHAR(i.last_altered, 'YYYY-MM-DD HH24:MI:SS')\r\n                        END AS \"modify_date\"\r\n                    , '......' AS \"function_type\"\r\n                FROM information_schema.functions i\r\n                WHERE i.function_catalog = '" + DatabaseNameToQuery + "'\r\n                    AND i.function_schema <> 'INFORMATION_SCHEMA'\r\n                    " + filterString;
	}

	public override IEnumerable<string> RelationsQuery()
	{
		foreach (ObjectRow item in synchronizeParameters.DatabaseRow.tableRows.Where((ObjectRow x) => x.SynchronizeState != SynchronizeStateEnum.SynchronizeState.Unknown && x.SynchronizeState != SynchronizeStateEnum.SynchronizeState.Deleted && x.SynchronizeState != SynchronizeStateEnum.SynchronizeState.Ignored && x.ObjectTypeValue == SharedObjectTypeEnum.ObjectType.Table))
		{
			if (!string.IsNullOrEmpty(item.DatabaseName) && !string.IsNullOrEmpty(item.Schema) && !string.IsNullOrEmpty(item.Name))
			{
				yield return "SHOW IMPORTED KEYS IN " + GetDatabaseDotSchemaDotTable(item) + ";";
			}
		}
	}

	private string GetDatabaseDotSchemaDotTable(ObjectRow objectRow)
	{
		return "\"" + objectRow.DatabaseName + "\".\"" + objectRow.Schema + "\".\"" + objectRow.Name + "\"";
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "c.table_schema", "c.table_name");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.View, "c.table_schema", "c.table_name");
		yield return "select \r\n                   c.table_schema as \"database_name\",\r\n                   c.table_name as \"table_name\",\r\n                   c.table_schema as \"table_schema\",\r\n                   c.column_name as \"name\",\r\n                   c.ordinal_position as \"position\",\r\n                   c.data_type as \"datatype\",\r\n                   case c.comment\r\n                        when '' then null \r\n                        else c.comment\r\n                   end as \"description\", \r\n                   null AS \"constraint_type\",\r\n                   case c.is_nullable\r\n                        when 'YES' then 1\r\n                        else 0\r\n                   end as \"nullable\", \r\n                   c.column_default as \"default_value\",\r\n                   case c.is_identity\r\n                        when 'YES' then 1\r\n                        else 0\r\n                   end as \"is_identity\",\r\n                   0 as \"is_computed\",\r\n                   null as \"computed_formula\",\r\n                   " + QueryDataTypes.GetQueryForDataLength(synchronizeParameters.DatabaseRow.Type) + " \r\n                   from information_schema.columns c\r\n                   left join information_schema.views v on c.table_schema = v.table_schema and c.table_name = v.table_name\r\n                   where c.table_catalog = '" + DatabaseNameToQuery + "'\r\n                        and ((v.table_schema is not null " + filterString2 + ")\r\n                            or (v.table_schema is null " + filterString + "))";
	}

	public override IEnumerable<string> TriggersQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		foreach (ObjectRow tableRow in synchronizeParameters.DatabaseRow.tableRows.Where((ObjectRow x) => x.SynchronizeState != SynchronizeStateEnum.SynchronizeState.Unknown && x.SynchronizeState != SynchronizeStateEnum.SynchronizeState.Deleted && x.SynchronizeState != SynchronizeStateEnum.SynchronizeState.Ignored && x.ObjectTypeValue == SharedObjectTypeEnum.ObjectType.Table))
		{
			if (!string.IsNullOrEmpty(tableRow.DatabaseName) && !string.IsNullOrEmpty(tableRow.Schema) && !string.IsNullOrEmpty(tableRow.Name))
			{
				yield return "SHOW PRIMARY KEYS IN " + GetDatabaseDotSchemaDotTable(tableRow) + ";";
				yield return "SHOW UNIQUE KEYS IN " + GetDatabaseDotSchemaDotTable(tableRow) + ";";
			}
		}
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		string functionFilterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Function, "ic.function_schema", "ic.function_name");
		string procedureFilterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Procedure, "ic.procedure_schema", "ic.procedure_name");
		yield return "SELECT\r\n                        ic.function_catalog as \"database_name\",\r\n                        ic.function_name as \"procedure_name\",\r\n                        ic.function_schema as \"procedure_schema\",\r\n                        '' as \"name\",\r\n                        0 as \"position\",\r\n                        'OUT' as \"parameter_mode\", \r\n                        ic.data_type as \"datatype\", \r\n                        case when ic.data_type = 'TEXT' then to_char(ic.character_maximum_length)\r\n                           when ic.data_type = 'NUMBER' then concat(concat(to_char(ic.numeric_precision), ', '), to_char(ic.numeric_scale))\r\n                           else null END as \"data_length\",\r\n                        null as \"description\"\r\n                        from information_schema.functions ic\r\n                        where ic.function_catalog = '" + DatabaseNameToQuery + "'\r\n                            " + functionFilterString;
		yield return "SELECT\r\n                        ic.procedure_catalog as \"database_name\",\r\n                        ic.procedure_name as \"procedure_name\",\r\n                        ic.procedure_schema as \"procedure_schema\",\r\n                        '' as \"name\",\r\n                        0 as \"position\",\r\n                        'OUT' as \"parameter_mode\", \r\n                        ic.data_type as \"datatype\", \r\n                        case when ic.data_type = 'TEXT' then to_char(ic.character_maximum_length)\r\n                           when ic.data_type = 'NUMBER' then concat(concat(to_char(ic.numeric_precision), ', '), to_char(ic.numeric_scale))\r\n                           else null END as \"data_length\",\r\n                        null as \"description\"\r\n                        from information_schema.procedures ic\r\n                        where ic.procedure_catalog = '" + DatabaseNameToQuery + "'\r\n                            " + procedureFilterString;
		yield return "SELECT\r\n                        ic.function_catalog as \"database_name\",\r\n                        ic.function_name as \"procedure_name\",\r\n                        ic.function_schema as \"procedure_schema\",\r\n                        'Arguments' as \"name\",\r\n                        1 as \"position\",\r\n                        'IN' as \"parameter_mode\", \r\n                        rtrim(ltrim(ic.argument_signature, '('),')') as \"datatype\", \r\n                        null as \"data_length\",\r\n                        null as \"description\"\r\n                        from information_schema.functions ic\r\n                        where ic.function_catalog = '" + DatabaseNameToQuery + "'\r\n\t\t\t\t\t\t\tand ic.argument_signature <> '()'\r\n                            " + functionFilterString;
		yield return "SELECT\r\n                        ic.procedure_catalog as \"database_name\",\r\n                        ic.procedure_name as \"procedure_name\",\r\n                        ic.procedure_schema as \"procedure_schema\",\r\n                        'Arguments' as \"name\",\r\n                        1 as \"position\",\r\n                        'IN' as \"parameter_mode\", \r\n                        rtrim(ltrim(ic.argument_signature, '('),')') as \"datatype\", \r\n                        null as \"data_length\",\r\n                        null as \"description\"\r\n                        from information_schema.procedures ic\r\n                        where ic.procedure_catalog = '" + DatabaseNameToQuery + "'\r\n\t\t\t\t\t\t\tand ic.argument_signature <> '()'\r\n                            " + procedureFilterString;
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		string text = synchronizeParameters.DatabaseRow.Host.ToUpper();
		string databaseNameToQuery = DatabaseNameToQuery;
		string filterStringForDependencies = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("REFERENCING_SCHEMA", "REFERENCING_OBJECT_NAME", "REFERENCING_OBJECT_DOMAIN", "REFERENCED_OBJECT_DOMAIN");
		string filterStringForDependencies2 = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("REFERENCED_SCHEMA", "REFERENCED_OBJECT_NAME", "REFERENCED_OBJECT_DOMAIN", "REFERENCING_OBJECT_DOMAIN");
		string text2 = FilterRulesCollection.CombineDependenciesFilter(filterStringForDependencies, filterStringForDependencies2);
		yield return "\r\n                            SELECT \r\n                                REFERENCING_OBJECT_DOMAIN AS \"referencing_type\",\r\n                                '" + text + "' AS \"referencing_server\",\r\n                                REFERENCING_DATABASE AS \"referencing_database_name\",\r\n                                REFERENCING_SCHEMA AS \"referencing_schema_name\",\r\n                                REFERENCING_OBJECT_NAME AS \"referencing_entity_name\",\r\n\r\n                                REFERENCED_OBJECT_DOMAIN AS \"referenced_type\",\r\n                                '" + text + "' AS \"referenced_server\",\r\n                                REFERENCED_DATABASE AS \"referenced_database_name\",\r\n                                REFERENCED_SCHEMA AS \"referenced_schema_name\",\r\n                                REFERENCED_OBJECT_NAME AS \"referenced_entity_name\",\r\n\r\n                                NULL as \"is_caller_dependent\",\r\n                                NULL as \"is_ambiguous\",\r\n                                NULL as \"dependency_type\"\r\n                            FROM \r\n                                SNOWFLAKE.ACCOUNT_USAGE.OBJECT_DEPENDENCIES\r\n                            WHERE\r\n                                REFERENCING_DATABASE = '" + databaseNameToQuery + "'\r\n                                " + text2;
	}
}
