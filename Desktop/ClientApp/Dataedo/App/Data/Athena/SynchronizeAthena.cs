using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Amazon.Athena;
using Amazon.Athena.Model;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.DataProcessing.Classes;
using Dataedo.Shared.Enums;
using Dataedo.SqlParser;
using Dataedo.SqlParser.Domain;

namespace Dataedo.App.Data.Athena;

internal class SynchronizeAthena : SynchronizeDatabase
{
	private const string DataType = "datatype";

	private const string Type = "type";

	private const string Path = "path";

	private const string Level = "level";

	private const string Name = "name";

	private const string Position = "position";

	private const string Struct = "struct";

	private const string Map = "map";

	private const string Array = "array";

	public SynchronizeAthena(SynchronizeParameters synchronizeParameters)
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
				AthenaConnectionWrapper athenaConnectionWrapper = CommandsWithTimeout.Athena(synchronizeParameters.DatabaseRow.Connection);
				AmazonAthenaClient amazonAthenaClient = athenaConnectionWrapper?.Client;
				if (amazonAthenaClient == null)
				{
					return false;
				}
				string queryExecutionId = SubmitAthenaQuery(amazonAthenaClient, athenaConnectionWrapper.Database, athenaConnectionWrapper.WorkGroup, athenaConnectionWrapper.DataCatalog, query.Query);
				WaitForQueryToComplete(amazonAthenaClient, queryExecutionId);
				GetQueryResultsRequest request = new GetQueryResultsRequest
				{
					QueryExecutionId = queryExecutionId
				};
				foreach (Dictionary<string, string> item in ProcessAthenaQuery(amazonAthenaClient.GetQueryResults(request).ResultSet))
				{
					SharedObjectTypeEnum.ObjectType value = SharedObjectTypeEnum.StringToType(PrepareValue.ToString(item["object_type"])).Value;
					ReadCount(value, Convert.ToInt32(item["count"]));
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

	private static List<Dictionary<string, string>> ProcessAthenaQuery(ResultSet rows)
	{
		List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
		foreach (Row item in rows.Rows.Skip(1))
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int i = 0; i < rows.ResultSetMetadata.ColumnInfo.Count; i++)
			{
				dictionary.Add(rows.ResultSetMetadata.ColumnInfo[i].Name, item.Data[i].VarCharValue);
			}
			list.Add(dictionary);
		}
		return list;
	}

	private static string SubmitAthenaQuery(AmazonAthenaClient athenaClient, string database, string workGroup, string dataCatalog, string query)
	{
		QueryExecutionContext queryExecutionContext = new QueryExecutionContext
		{
			Database = database,
			Catalog = dataCatalog
		};
		StartQueryExecutionRequest request = new StartQueryExecutionRequest
		{
			QueryString = query,
			QueryExecutionContext = queryExecutionContext,
			WorkGroup = workGroup
		};
		return athenaClient.StartQueryExecution(request).QueryExecutionId;
	}

	private void WaitForQueryToComplete(AmazonAthenaClient athenaClient, string queryExecutionId)
	{
		GetQueryExecutionRequest request = new GetQueryExecutionRequest
		{
			QueryExecutionId = queryExecutionId
		};
		GetQueryExecutionResponse getQueryExecutionResponse = null;
		bool flag = true;
		while (flag)
		{
			getQueryExecutionResponse = athenaClient.GetQueryExecution(request);
			QueryExecutionState state = getQueryExecutionResponse.QueryExecution.Status.State;
			if (state == QueryExecutionState.FAILED)
			{
				throw new Exception("Query Failed to run with Error Message: " + getQueryExecutionResponse.QueryExecution.Status.StateChangeReason);
			}
			if (state == QueryExecutionState.CANCELLED)
			{
				throw new Exception("Query was cancelled.");
			}
			if (state == QueryExecutionState.SUCCEEDED)
			{
				flag = false;
			}
			else
			{
				Thread.Sleep(TimeSpan.FromMilliseconds(1000.0));
			}
		}
	}

	public override bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (base.ObjectsCounter != null)
		{
			backgroundWorkerManager.ReportProgress("Retrieving database's objects");
			try
			{
				AthenaConnectionWrapper athenaConnectionWrapper = CommandsWithTimeout.Athena(synchronizeParameters.DatabaseRow.Connection);
				AmazonAthenaClient amazonAthenaClient = athenaConnectionWrapper?.Client;
				if (amazonAthenaClient == null)
				{
					return false;
				}
				string queryExecutionId = SubmitAthenaQuery(amazonAthenaClient, athenaConnectionWrapper.Database, athenaConnectionWrapper.WorkGroup, athenaConnectionWrapper.DataCatalog, query.Query);
				WaitForQueryToComplete(amazonAthenaClient, queryExecutionId);
				GetQueryResultsRequest request = new GetQueryResultsRequest
				{
					QueryExecutionId = queryExecutionId
				};
				foreach (Dictionary<string, string> item in ProcessAthenaQuery(amazonAthenaClient.GetQueryResults(request).ResultSet))
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					AddDBObject(item, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
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

	private List<Dictionary<string, string>> GenerateFlattenObject(Dictionary<string, string> column, List<AthenaParameter> parsingResults, List<Dictionary<string, string>> allColumns, string path = null, int level = 2, int listIterator = 0)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>(column);
		if (listIterator > parsingResults.Count - 1)
		{
			return allColumns;
		}
		string value = ((parsingResults[listIterator].DataType != null) ? parsingResults[listIterator].DataType : string.Empty);
		string name = parsingResults[listIterator].Name;
		string value2 = (listIterator + 1).ToString();
		dictionary["datatype"] = value;
		dictionary["name"] = name;
		dictionary["position"] = value2;
		dictionary = AddItemsToColumn(dictionary, SharedObjectSubtypeEnum.ObjectSubtype.Field, level.ToString(), path);
		allColumns.Add(dictionary);
		if (parsingResults[listIterator].NestedParameters.Count > 0)
		{
			dictionary["type"] = SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Column, SharedObjectSubtypeEnum.ObjectSubtype.Document);
			dictionary["datatype"] = "struct";
			return GenerateFlattenObject(column, parsingResults[listIterator].NestedParameters, allColumns, path + "." + parsingResults[listIterator].Name, level + 1);
		}
		return GenerateFlattenObject(column, parsingResults, allColumns, path, level, listIterator + 1);
	}

	private Dictionary<string, string> GenerateMapOrArrayColumn(Dictionary<string, string> column, List<AthenaParameter> parsingResults, string type)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>(column);
		string[] value = parsingResults[0].NestedParameters.Select((AthenaParameter el) => el.DataType).ToArray();
		dictionary["datatype"] = type + "(" + string.Join(",", value) + ")";
		return AddItemsToColumn(dictionary);
	}

	private Dictionary<string, string> AddItemsToColumn(Dictionary<string, string> column, SharedObjectSubtypeEnum.ObjectSubtype type = SharedObjectSubtypeEnum.ObjectSubtype.Document, string level = "1", string path = "")
	{
		column.Add("level", level);
		column.Add("path", path);
		column.Add("type", SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Column, type));
		return column;
	}

	public override bool GetColumns(string query, Form owner = null)
	{
		try
		{
			AthenaConnectionWrapper athenaConnectionWrapper = CommandsWithTimeout.Athena(synchronizeParameters.DatabaseRow.Connection);
			AmazonAthenaClient amazonAthenaClient = athenaConnectionWrapper?.Client;
			if (amazonAthenaClient == null)
			{
				return false;
			}
			string queryExecutionId = SubmitAthenaQuery(amazonAthenaClient, athenaConnectionWrapper.Database, athenaConnectionWrapper.WorkGroup, athenaConnectionWrapper.DataCatalog, query);
			WaitForQueryToComplete(amazonAthenaClient, queryExecutionId);
			GetQueryResultsRequest request = new GetQueryResultsRequest
			{
				QueryExecutionId = queryExecutionId
			};
			List<Dictionary<string, string>> list = ProcessAthenaQuery(amazonAthenaClient.GetQueryResults(request).ResultSet);
			List<Dictionary<string, string>> list2 = new List<Dictionary<string, string>>();
			foreach (Dictionary<string, string> item2 in list)
			{
				if (!item2["datatype"].Contains("row("))
				{
					Dictionary<string, string> column = new Dictionary<string, string>(item2);
					column = AddItemsToColumn(column, SharedObjectSubtypeEnum.ObjectSubtype.Field);
					list2.Add(column);
					continue;
				}
				List<AthenaParameter> list3 = new AthenaParser(item2["datatype"]).Run();
				if ((list3.Count == 1 && list3[0].DataType == "map") || list3[0].DataType == "array")
				{
					Dictionary<string, string> item = GenerateMapOrArrayColumn(item2, list3, list3[0].DataType);
					list2.Add(item);
					continue;
				}
				List<Dictionary<string, string>> collection = GenerateFlattenObject(item2, list3, new List<Dictionary<string, string>>(), item2["name"]);
				list2.AddRange(collection);
				Dictionary<string, string> dictionary = new Dictionary<string, string>(item2);
				dictionary["datatype"] = "struct";
				dictionary = AddItemsToColumn(dictionary);
				list2.Add(dictionary);
			}
			foreach (Dictionary<string, string> item3 in list2)
			{
				AddColumn(item3);
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetRelations(string query, Form owner = null)
	{
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
		return true;
	}

	public override bool GetDependencies(string query, Form owner = null)
	{
		return true;
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		yield return "\r\n                         select \r\n                            count(1) as count,\r\n                            'TABLE' as object_type\r\n                        from information_schema.tables t\r\n                        where t.table_schema <> 'information_schema' and t.table_schema = '" + synchronizeParameters.DatabaseName + "'\r\n                         and t.table_name not in (\r\n                            select v.table_name from information_schema.views v where table_schema = '" + synchronizeParameters.DatabaseName + "'\r\n                        )";
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		yield return "\r\n                        select \r\n                            count(1) as count,\r\n                            'VIEW' as object_type\r\n                        from information_schema.views v\r\n                        where v.table_schema = '" + synchronizeParameters.DatabaseName + "'";
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
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Table, "t.table_schema", "t.table_name");
		yield return "\r\n                         select \r\n                            t.table_name as name,\r\n                            t.table_schema as database_name,\r\n                            'TABLE' as type,\r\n                            null as description,\r\n                            null as definition,\r\n                            null as create_date,\r\n                            null as update_time,\r\n                            null as function_type\r\n                        from information_schema.tables t\r\n                        where t.table_schema <> 'information_schema' and \r\n                                        t.table_schema = '" + synchronizeParameters.DatabaseName + "'\r\n                         and t.table_name not in (\r\n                            select v.table_name from information_schema.views v\r\n                            where table_schema = '" + synchronizeParameters.DatabaseName + "')\r\n                        " + filterString;
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "v.tables_schema", "v.table_name");
		yield return "\r\n                        select \r\n                            '' as schema,\r\n                            v.table_schema as database_name,\r\n                            v.table_name as name ,\r\n                            v.view_definition as definition,\r\n                            'VIEW' as type,\r\n                            null as description,\r\n                            null as create_date,\r\n                            null as modify_date,\r\n                            null as function_type\r\n                        from information_schema.views v\r\n                        where v.table_schema = '" + synchronizeParameters.DatabaseName + "' " + filterString;
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
		yield return string.Empty;
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "c.table_schema", "c.table_name");
		yield return "\r\n                         select \r\n                            c.table_schema as database_name,\r\n                            c.table_name as table_name,\r\n                            '' as table_schema,\r\n                            c.column_name as name,\r\n                            c.ordinal_position as position,\r\n                            c.data_type as datatype,\r\n                            case c.comment\r\n                                when '' then null\r\n                                else c.comment\r\n                            end as description,\r\n                            case c.is_nullable\r\n                                when 'YES' then 1\r\n                                else 0\r\n                            end as nullable,\r\n                            c.column_default as default_value\r\n                        from information_schema.columns c\r\n                        where c.table_schema = '" + synchronizeParameters.DatabaseName + "' " + filterString;
	}

	public override IEnumerable<string> TriggersQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		yield return string.Empty;
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
