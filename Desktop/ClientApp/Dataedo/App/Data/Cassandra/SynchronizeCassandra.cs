using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Cassandra;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Model.Data.Procedures.Parameters;
using Dataedo.Model.Extensions;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.Cassandra;

internal class SynchronizeCassandra : SynchronizeDatabase
{
	public SynchronizeCassandra(SynchronizeParameters synchronizeParameters)
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
				using RowSet rowSet = CommandsWithTimeout.Cassandra(synchronizeParameters.DatabaseRow.Connection).Execute(new SimpleStatement(query.Query));
				foreach (Row item in rowSet)
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					ReadCount(item);
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
				using RowSet rowSet = CommandsWithTimeout.Cassandra(synchronizeParameters.DatabaseRow.Connection).Execute(new SimpleStatement(query.Query));
				foreach (Row item in rowSet)
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					if (!FilterUserObjectsFilter(item.Field<string>("name"), query.ObjectType))
					{
						AddDBObject(item, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
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
			using RowSet rowSet = CommandsWithTimeout.Cassandra(synchronizeParameters.DatabaseRow.Connection).Execute(new SimpleStatement(query));
			foreach (Row item in rowSet)
			{
				if (!FilterUserObjectsFilter(item.Field<string>("table_name"), FilterObjectTypeEnum.FilterObjectType.Table) || !FilterUserObjectsFilter(item.Field<string>("table_name"), FilterObjectTypeEnum.FilterObjectType.View))
				{
					AddColumn(item);
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

	public override bool GetTriggers(string query, Form owner = null)
	{
		try
		{
			using RowSet rowSet = CommandsWithTimeout.Cassandra(synchronizeParameters.DatabaseRow.Connection).Execute(new SimpleStatement(query));
			foreach (Row item in rowSet)
			{
				if (!FilterUserObjectsFilter(item.Field<string>("table_name"), FilterObjectTypeEnum.FilterObjectType.Table) || !FilterUserObjectsFilter(item.Field<string>("table_name"), FilterObjectTypeEnum.FilterObjectType.View))
				{
					AddTrigger(item);
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

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		try
		{
			using RowSet rowSet = CommandsWithTimeout.Cassandra(synchronizeParameters.DatabaseRow.Connection).Execute(new SimpleStatement(query));
			foreach (Row item in rowSet)
			{
				if (!FilterUserObjectsFilter(item.Field<string>("table_name"), FilterObjectTypeEnum.FilterObjectType.Table) || !FilterUserObjectsFilter(item.Field<string>("table_name"), FilterObjectTypeEnum.FilterObjectType.View))
				{
					AddUniqueConstraint(item, SharedDatabaseTypeEnum.DatabaseType.Cassandra);
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
			using RowSet rowSet = CommandsWithTimeout.Cassandra(synchronizeParameters.DatabaseRow.Connection).Execute(new SimpleStatement(query));
			foreach (Row item in rowSet)
			{
				if (FilterUserObjectsFilter(item.Field<string>("procedure_name"), FilterObjectTypeEnum.FilterObjectType.Table) && FilterUserObjectsFilter(item.Field<string>("procedure_name"), FilterObjectTypeEnum.FilterObjectType.View))
				{
					continue;
				}
				if (item.Field<string>("parameter_mode") == "OUT")
				{
					AddParameter(item);
					continue;
				}
				string[] array = item["name"] as string[];
				string[] array2 = item["datatype"] as string[];
				for (int i = 0; i < array.Length; i++)
				{
					string datatype = array2[i];
					string name = array[i];
					ParameterSynchronizationObject parameterSynchronizationObject = new ParameterSynchronizationObject();
					parameterSynchronizationObject.DatabaseName = item.Field<string>("database_name");
					parameterSynchronizationObject.ProcedureName = item.Field<string>("procedure_name");
					parameterSynchronizationObject.ProcedureSchema = item.Field<string>("procedure_schema");
					parameterSynchronizationObject.Name = name;
					parameterSynchronizationObject.Position = i + 1;
					parameterSynchronizationObject.ParameterMode = item.Field<string>("parameter_mode");
					parameterSynchronizationObject.Datatype = datatype;
					parameterSynchronizationObject.DataLength = item.Field<string>("data_length");
					parameterSynchronizationObject.Description = item.Field<string>("description");
					AddParameter(parameterSynchronizationObject);
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
			using RowSet rowSet = CommandsWithTimeout.Cassandra(synchronizeParameters.DatabaseRow.Connection).Execute(new SimpleStatement(query));
			foreach (Row item in rowSet)
			{
				if (!FilterUserObjectsFilter(item.Field<string>("referenced_entity_name"), FilterObjectTypeEnum.FilterObjectType.Table) || !FilterUserObjectsFilter(item.Field<string>("referenced_entity_name"), FilterObjectTypeEnum.FilterObjectType.View))
				{
					AddDependency(item);
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

	protected bool FilterUserObjectsFilter(string name, FilterObjectTypeEnum.FilterObjectType objectType)
	{
		IEnumerable<FilterRule> source = synchronizeParameters.DatabaseRow.Filter.Rules.Where((FilterRule x) => (x.ObjectType == objectType || x.ObjectType == FilterObjectTypeEnum.FilterObjectType.Any) && x.RuleType == FilterRuleType.Include);
		if (synchronizeParameters.DatabaseRow.Filter.Rules.Where((FilterRule x) => (x.ObjectType == objectType || x.ObjectType == FilterObjectTypeEnum.FilterObjectType.Any) && x.RuleType == FilterRuleType.Exclude).Any((FilterRule x) => Like(name, x.PreparedName)))
		{
			return true;
		}
		if (source.Count() > 0)
		{
			if (!source.Any((FilterRule x) => Like(name, x.PreparedName)))
			{
				return true;
			}
			return false;
		}
		return false;
	}

	private bool Like(string toSearch, string toFind)
	{
		toSearch = toSearch.ToUpper();
		toFind = toFind.ToUpper();
		return new Regex("\\A" + new Regex("\\.|\\$|\\^|\\{|\\[|\\(|\\||\\)|\\*|\\+|\\?|\\\\").Replace(toFind, (Match ch) => "\\" + ch).Replace('_', '.').Replace("%", ".*") + "\\z", RegexOptions.Singleline).IsMatch(toSearch);
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		yield return "SELECT count(1) as \"count\",\r\n                                blobastext(textasblob('TABLE'))  AS \"object_type\"\r\n                            FROM SYSTEM_SCHEMA.tables\r\n                            WHERE " + GetKeyspaceCondition() + ";";
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		yield return "SELECT count(1) AS \"count\",\r\n                                blobastext(textasblob('VIEW')) AS \"object_type\"\r\n                            FROM SYSTEM_SCHEMA.VIEWS\r\n                            WHERE " + GetKeyspaceCondition() + ";";
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		yield return "SELECT count(1) AS \"count\",\r\n                                blobastext(textasblob('FUNCTION'))  AS \"object_type\"\r\n                            FROM SYSTEM_SCHEMA.functions\r\n                            WHERE " + GetKeyspaceCondition() + ";";
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		yield return "SELECT table_name AS \"name\",\r\n                                keyspace_name AS \"schema\",\r\n                                keyspace_name AS \"database_name\",\r\n                                blobastext(textasblob('TABLE')) AS \"type\",\r\n                                comment AS \"description\",\r\n                                textasblob(null) AS \"definition\",     \r\n                                textasblob(null) AS \"create_date\",\r\n                                textasblob(null) AS \"modify_date\",\r\n                                textasblob(null) AS \"function_type\"\r\n                            FROM SYSTEM_SCHEMA.tables\r\n            WHERE " + GetKeyspaceCondition() + ";";
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		yield return "SELECT view_name AS \"name\",\r\n                                keyspace_name AS \"schema\",\r\n                                keyspace_name AS \"database_name\",\r\n                                blobastext(textasblob('MATERIALIZED VIEW')) AS \"type\",\r\n                                comment AS \"description\",\r\n\r\n                                where_clause AS \"definition\",     \r\n                                textasblob(null) AS \"create_date\",\r\n                                textasblob(null) AS \"modify_date\",\r\n                                textasblob(null) AS \"function_type\"\r\n                            FROM SYSTEM_SCHEMA.VIEWS\r\n                            WHERE " + GetKeyspaceCondition() + ";";
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		yield return "SELECT function_name AS \"name\",\r\n                                keyspace_name AS \"schema\",\r\n                                keyspace_name AS \"database_name\",\r\n                                blobastext(textasblob('FUNCTION')) AS \"type\",\r\n                                textasblob(null) AS \"description\",\r\n                                body AS \"definition\",\r\n                                textasblob(null) AS \"create_date\",\r\n                                textasblob(null) AS \"modify_date\",\r\n                                textasblob(null) AS \"function_type\",\r\n                                language as \"language\"\r\n                            FROM system_schema.functions\r\n                            WHERE " + GetKeyspaceCondition() + ";";
	}

	public override IEnumerable<string> RelationsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		yield return "SELECT \r\n                                keyspace_name AS \"database_name\",\r\n                                table_name AS \"table_name\",\r\n                                keyspace_name AS \"table_schema\",\r\n                                column_name AS \"name\",\r\n                                position AS \"position\",\r\n                                type AS \"datatype\",\r\n                                textasblob(null) AS \"data_length\",\r\n                                textasblob(null) AS \"description\",\r\n                                kind AS \"constraint_type\",\r\n                                blobasint(intasblob(1)) AS \"nullable\",\r\n                                textasblob(null) AS \"default_value\",\r\n                                blobasint(intasblob(0)) AS \"is_identity\",\r\n                                blobasint(intasblob(0)) AS \"is_computed\",\r\n                                blobastext(textasblob('')) AS \"computed_formula\"\r\n                            FROM system_schema.columns\r\n                            WHERE " + GetKeyspaceCondition() + ";";
	}

	public override IEnumerable<string> TriggersQuery()
	{
		yield return "SELECT trigger_name AS \"trigger_name\",\r\n                                blobastext(textasblob('TR')) AS \"type\",\r\n                                keyspace_name AS \"table_schema\",\r\n                                table_name AS \"table_name\",\r\n                                keyspace_name AS \"database_name\",\r\n                                blobasint(intasblob(0)) AS \"isupdate\",\r\n                                blobasint(intasblob(0)) AS \"isdelete\",\r\n                                blobasint(intasblob(0)) AS \"isinsert\",\r\n                                blobasint(intasblob(1)) AS \"isbefore\",\r\n                                blobasint(intasblob(0)) AS \"isafter\",\r\n                                blobasint(intasblob(0)) AS \"isinsteadof\",\r\n                                blobasint(intasblob(0)) AS \"disabled\",\r\n                                options AS \"definition\",\r\n                                blobastext(textasblob('')) AS \"description\"\r\n                            FROM system_schema.triggers\r\n                            WHERE " + GetKeyspaceCondition() + ";";
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		yield return "SELECT table_name AS \"table_name\",\r\n                                keyspace_name AS \"database_name\",\r\n                                keyspace_name AS \"table_schema\",\r\n                                blobastext(textasblob('primary_key')) AS \"name\",\r\n                                column_name AS \"column_name\",\r\n                                blobastext(textasblob('P')) AS \"type\",\r\n                                position AS \"column_ordinal\",\r\n                                textasblob(null) AS \"description\",\r\n                                blobasint(intasblob(0)) AS \"disabled\"\r\n                            FROM system_schema.columns\r\n                            WHERE kind = 'partition_key' \r\n                                AND " + GetKeyspaceCondition() + " ALLOW FILTERING;";
		yield return "SELECT table_name AS \"table_name\",\r\n                                keyspace_name AS \"database_name\",\r\n                                keyspace_name AS \"table_schema\",\r\n                                blobastext(textasblob('primary_key')) AS \"name\",\r\n                                column_name AS \"column_name\",\r\n                                blobastext(textasblob('P')) AS \"type\",\r\n                                position AS \"column_ordinal\",\r\n                                blobastext(textasblob(null)) AS \"description\",\r\n                                blobasint(intasblob(0)) AS \"disabled\"\r\n                            FROM system_schema.columns\r\n                            WHERE kind = 'clustering' \r\n                                AND " + GetKeyspaceCondition() + " ALLOW FILTERING;";
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		yield return "SELECT keyspace_name AS \"database_name\",\r\n                                function_name AS \"procedure_name\",\r\n                                keyspace_name AS \"procedure_schema\",\r\n                                argument_names AS \"name\",\r\n                                argument_types AS \"datatype\",\r\n                                blobasint(intasblob(0)) AS \"position\",\r\n                                blobastext(textasblob('IN')) AS \"parameter_mode\",\r\n                                blobastext(textasblob(null)) AS \"description\",\r\n                                blobastext(textasblob(null)) AS \"data_length\"\r\n                            FROM system_schema.functions\r\n                            WHERE " + GetKeyspaceCondition() + ";";
		yield return "SELECT keyspace_name AS \"database_name\",\r\n                                function_name AS \"procedure_name\",\r\n                                keyspace_name AS \"procedure_schema\",\r\n                                blobastext(textasblob('returns')) AS \"name\",\r\n                                return_type AS \"datatype\",\r\n                                blobasint(intasblob(0)) AS \"position\",\r\n                                blobastext(textasblob('OUT')) AS \"parameter_mode\",\r\n                                blobastext(textasblob(null)) AS \"description\",\r\n                                blobastext(textasblob(null)) AS \"data_length\"\r\n                            FROM system_schema.functions\r\n                            WHERE " + GetKeyspaceCondition() + ";";
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		string text = synchronizeParameters.DatabaseRow.Host.ToUpper();
		yield return "SELECT  blobastext(textasblob('VIEW')) AS \"referencing_type\",\r\n                                    blobastext(textasblob('" + text + "')) AS \"referencing_server\",\r\n                                    keyspace_name AS \"referencing_schema_name\",\r\n                                    keyspace_name AS \"referencing_database_name\",\r\n                                    view_name AS \"referencing_entity_name\",\r\n                                    blobastext(textasblob('" + text + "')) AS \"referenced_server\",\r\n                                    keyspace_name AS \"referenced_database_name\",\r\n                                    keyspace_name AS \"referenced_schema_name\",\r\n                                    blobastext(textasblob('TABLE')) AS \"referenced_type\",\r\n                                    base_table_name AS \"referenced_entity_name\",\r\n                                    blobastext(textasblob('0')) AS \"is_caller_dependent\",\r\n                                    blobastext(textasblob('0')) AS \"is_ambiguous\",\r\n                                    textasblob(null) AS \"dependency_type\"\r\n                            FROM system_schema.VIEWS\r\n                            WHERE " + GetKeyspaceCondition() + ";";
	}

	protected virtual string GetKeyspaceCondition()
	{
		return "keyspace_name IN ('" + synchronizeParameters.DatabaseName + "')";
	}
}
