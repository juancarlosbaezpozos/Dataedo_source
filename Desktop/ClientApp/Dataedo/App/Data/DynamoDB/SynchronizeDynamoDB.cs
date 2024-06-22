using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Model.Data.DynamoDB;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.DynamoDB;

internal class SynchronizeDynamoDB : SynchronizeDatabase
{
	private List<DynamoDBTable> tables;

	public List<string> TableNames
	{
		get
		{
			if (synchronizeParameters.DatabaseRow.Tag is List<string> result)
			{
				return result;
			}
			if (synchronizeParameters.DatabaseRow.Connection is AmazonDynamoDBClient client)
			{
				synchronizeParameters.DatabaseRow.Tag = GetAmazonTableNames(client);
			}
			else
			{
				synchronizeParameters.DatabaseRow.Tag = new List<string>();
			}
			return (List<string>)synchronizeParameters.DatabaseRow.Tag;
		}
		private set
		{
			synchronizeParameters.DatabaseRow.Tag = value;
		}
	}

	public SynchronizeDynamoDB(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
		tables = new List<DynamoDBTable>();
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		try
		{
			ReadCount(SharedObjectTypeEnum.ObjectType.Table, TableNames.Count);
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
			return false;
		}
	}

	public override bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		try
		{
			backgroundWorkerManager.ReportProgress("Retrieving database's objects");
			IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Table);
			AmazonDynamoDBClient amazonDynamoDBClient = synchronizeParameters.DatabaseRow.Connection as AmazonDynamoDBClient;
			foreach (string tableName in TableNames)
			{
				if (IsObjectFiltered(tableName, rulesByObjectType))
				{
					continue;
				}
				try
				{
					DescribeTableRequest request = new DescribeTableRequest
					{
						TableName = tableName
					};
					TableDescription table = amazonDynamoDBClient.DescribeTable(request).Table;
					DynamoDBTable dr = new DynamoDBTable(synchronizeParameters.DatabaseRow.Name, table);
					AddDBObject(dr, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
				}
				catch (AmazonDynamoDBException ex)
				{
					if (ex is ResourceNotFoundException || ex is ResourceInUseException)
					{
						continue;
					}
					throw;
				}
			}
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
			return false;
		}
	}

	public override bool GetColumns(string query, Form owner = null)
	{
		try
		{
			IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Table);
			AmazonDynamoDBClient amazonDynamoDBClient = synchronizeParameters.DatabaseRow.Connection as AmazonDynamoDBClient;
			int num = 0;
			foreach (string item in synchronizeParameters.DatabaseRow.tableRows.Select((ObjectRow x) => x.Name).ToList())
			{
				if (IsObjectFiltered(item, rulesByObjectType))
				{
					continue;
				}
				DescribeTableRequest request = new DescribeTableRequest
				{
					TableName = item
				};
				TableDescription table = amazonDynamoDBClient.DescribeTable(request).Table;
				Table table2 = Table.LoadTable(amazonDynamoDBClient, item);
				DynamoDBTable dynamoDBTable = new DynamoDBTable(synchronizeParameters.DatabaseRow.Name, table, table2);
				Search search = table2.Scan(new ScanFilter());
				new List<Document>();
				do
				{
					foreach (Document item2 in search.GetNextSet())
					{
						dynamoDBTable.AddDocument(item2);
						num++;
					}
				}
				while (!search.IsDone && num < int.Parse(synchronizeParameters.DatabaseRow.Param1));
				tables.Add(dynamoDBTable);
				dynamoDBTable.SynchronizeColumnsListWithKeys();
				foreach (DynamoDBColumn column in dynamoDBTable.Columns)
				{
					AddColumn(column);
				}
			}
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
			return false;
		}
	}

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		try
		{
			foreach (DynamoDBTable table in tables)
			{
				if (table.Columns.Count == 0)
				{
					continue;
				}
				StringBuilder stringBuilder = new StringBuilder();
				List<string> list = new List<string>();
				foreach (KeySchemaElement tableKey in table.TableKeys)
				{
					if (tableKey.KeyType == KeyType.HASH)
					{
						stringBuilder.Append("Partition Key, ");
						list.Add(tableKey.AttributeName);
					}
					else if (tableKey.KeyType == KeyType.RANGE)
					{
						stringBuilder.Append("Sort Key, ");
						list.Add(tableKey.AttributeName);
					}
				}
				string constraintName = stringBuilder.Remove(stringBuilder.Length - 2, 2).ToString();
				int num = 1;
				foreach (string item in list)
				{
					DynamoDBConstraint dr = new DynamoDBConstraint(constraintName, synchronizeParameters.DatabaseRow.Name, table.Name, item, num);
					num++;
					AddUniqueConstraint(dr, SharedDatabaseTypeEnum.DatabaseType.DynamoDB);
				}
			}
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
			return false;
		}
	}

	private List<string> GetAmazonTableNames(AmazonDynamoDBClient client)
	{
		List<string> list = new List<string>();
		string text = null;
		do
		{
			ListTablesRequest request = new ListTablesRequest
			{
				ExclusiveStartTableName = text
			};
			ListTablesResponse listTablesResponse = client.ListTables(request);
			list.AddRange(listTablesResponse.TableNames);
		}
		while (!string.IsNullOrEmpty(text));
		return list;
	}

	public override bool GetDependencies(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetParameters(string query, bool canReadParameters = true, Form owner = null)
	{
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

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> RelationsQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> TriggersQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		return new List<string> { "" };
	}
}
