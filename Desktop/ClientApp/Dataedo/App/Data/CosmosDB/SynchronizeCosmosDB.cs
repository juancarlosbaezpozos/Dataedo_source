using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Model.Data.MongoDB;
using Dataedo.Model.Data.Object;
using Dataedo.Model.Data.Tables.Triggers;
using Dataedo.Model.Data.UniqueConstraints;
using Dataedo.Shared.Enums;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Dataedo.App.Data.CosmosDB;

internal class SynchronizeCosmosDB : SynchronizeDatabase
{
	private int documentLimitToGet = 50;

	public SynchronizeCosmosDB(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		return true;
	}

	public override bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (base.ObjectsCounter != null)
		{
			try
			{
				backgroundWorkerManager.ReportProgress("Retrieving database's objects");
				switch (query.ObjectType)
				{
				case FilterObjectTypeEnum.FilterObjectType.Table:
					return GetTables(query, backgroundWorkerManager, owner);
				case FilterObjectTypeEnum.FilterObjectType.Procedure:
					return GetProcedures(query, backgroundWorkerManager, owner);
				case FilterObjectTypeEnum.FilterObjectType.Function:
					return GetFunctions(query, backgroundWorkerManager, owner);
				case FilterObjectTypeEnum.FilterObjectType.View:
					break;
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

	private bool GetTables(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner)
	{
		CosmosClient cosmosClient = CommandsWithTimeout.CosmosDB(synchronizeParameters.DatabaseRow.Connection);
		if (cosmosClient == null)
		{
			return false;
		}
		IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Table);
		Database database = GetDatabase(cosmosClient, synchronizeParameters.DatabaseRow.Name);
		if (database == null)
		{
			return false;
		}
		foreach (string containerName in GetContainerNames(database))
		{
			List<ObjectRow> source = new List<ObjectRow>(synchronizeParameters.DatabaseRow.tableRows);
			if (!IsObjectFiltered(containerName, rulesByObjectType) && !source.Any((ObjectRow x) => x.Name == containerName && x.Type == SharedObjectTypeEnum.ObjectType.Table && !x.ToSynchronize))
			{
				if (database.GetContainer(containerName) == null)
				{
					return false;
				}
				BsonTable dr = new BsonTable(containerName, synchronizeParameters.DatabaseRow.Name);
				AddDBObject(dr, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
			}
		}
		return true;
	}

	private bool GetProcedures(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner)
	{
		CosmosClient cosmosClient = CommandsWithTimeout.CosmosDB(synchronizeParameters.DatabaseRow.Connection);
		if (cosmosClient == null)
		{
			return false;
		}
		IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Procedure);
		Database database = GetDatabase(cosmosClient, synchronizeParameters.DatabaseRow.Name);
		if (database == null)
		{
			return false;
		}
		foreach (string containerName in GetContainerNames(database))
		{
			List<ObjectRow> source = new List<ObjectRow>(synchronizeParameters.DatabaseRow.tableRows);
			if (IsObjectFiltered(containerName, rulesByObjectType) || source.Any((ObjectRow x) => x.Name == containerName && x.Type == SharedObjectTypeEnum.ObjectType.Procedure && !x.ToSynchronize))
			{
				continue;
			}
			Container container = database.GetContainer(containerName);
			foreach (StoredProcedureProperties item in Task.Run(() => GetStoredProcedureProperties(container, query.Query)).Result)
			{
				ObjectSynchronizationForCosmosDBObject dr = new ObjectSynchronizationForCosmosDBObject(item.Id, synchronizeParameters.DatabaseRow.Name, synchronizeParameters.DatabaseRow.Name, SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Procedure), null, item.Body, null, item.LastModified, null, "SQL");
				AddDBObject(dr, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
			}
		}
		return true;
	}

	private async Task<List<StoredProcedureProperties>> GetStoredProcedureProperties(Container container, string query)
	{
		List<StoredProcedureProperties> storedProcedureProperties = new List<StoredProcedureProperties>();
		using (FeedIterator<StoredProcedureProperties> feedIterator = container.Scripts.GetStoredProcedureQueryIterator<StoredProcedureProperties>(query))
		{
			while (feedIterator.HasMoreResults)
			{
				foreach (StoredProcedureProperties item in await feedIterator.ReadNextAsync())
				{
					storedProcedureProperties.Add(item);
				}
			}
		}
		return storedProcedureProperties;
	}

	private bool GetFunctions(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner)
	{
		CosmosClient cosmosClient = CommandsWithTimeout.CosmosDB(synchronizeParameters.DatabaseRow.Connection);
		if (cosmosClient == null)
		{
			return false;
		}
		IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Function);
		Database database = GetDatabase(cosmosClient, synchronizeParameters.DatabaseRow.Name);
		if (database == null)
		{
			return false;
		}
		foreach (string containerName in GetContainerNames(database))
		{
			List<ObjectRow> source = new List<ObjectRow>(synchronizeParameters.DatabaseRow.tableRows);
			if (IsObjectFiltered(containerName, rulesByObjectType) || source.Any((ObjectRow x) => x.Name == containerName && x.Type == SharedObjectTypeEnum.ObjectType.Function && !x.ToSynchronize))
			{
				continue;
			}
			Container container = database.GetContainer(containerName);
			foreach (UserDefinedFunctionProperties item in Task.Run(() => GetUserDefinedFunctionsProperties(container, query.Query)).Result)
			{
				ObjectSynchronizationForCosmosDBObject dr = new ObjectSynchronizationForCosmosDBObject(item.Id, synchronizeParameters.DatabaseRow.Name, synchronizeParameters.DatabaseRow.Name, SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Function), null, item.Body, null, null, SharedObjectTypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Function), "SQL");
				AddDBObject(dr, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
			}
		}
		return true;
	}

	private async Task<List<UserDefinedFunctionProperties>> GetUserDefinedFunctionsProperties(Container container, string query)
	{
		List<UserDefinedFunctionProperties> userDefinedFunctionProperties = new List<UserDefinedFunctionProperties>();
		using (FeedIterator<UserDefinedFunctionProperties> feedIterator = container.Scripts.GetUserDefinedFunctionQueryIterator<UserDefinedFunctionProperties>(query))
		{
			while (feedIterator.HasMoreResults)
			{
				foreach (UserDefinedFunctionProperties item in await feedIterator.ReadNextAsync())
				{
					userDefinedFunctionProperties.Add(item);
				}
			}
		}
		return userDefinedFunctionProperties;
	}

	public override bool GetColumns(string query, Form owner = null)
	{
		try
		{
			CosmosClient cosmosClient = CommandsWithTimeout.CosmosDB(synchronizeParameters.DatabaseRow.Connection);
			if (cosmosClient == null)
			{
				return false;
			}
			IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Table);
			Database database = GetDatabase(cosmosClient, synchronizeParameters.DatabaseRow.Name);
			if (database == null)
			{
				return false;
			}
			foreach (string containerName in GetContainerNames(database))
			{
				List<ObjectRow> source = new List<ObjectRow>(synchronizeParameters.DatabaseRow.tableRows);
				if (IsObjectFiltered(containerName, rulesByObjectType) || source.Any((ObjectRow x) => x.Name == containerName && !x.ToSynchronize))
				{
					continue;
				}
				Container container = database.GetContainer(containerName);
				if (container == null)
				{
					continue;
				}
				BsonTable bsonTable = new BsonTable(containerName, synchronizeParameters.DatabaseRow.Name);
				foreach (string item in Task.Run(() => GetDocuments(container, query)).Result)
				{
					bsonTable.AddDocument(BsonDocument.Parse(item));
				}
				foreach (BsonColumn column in bsonTable.Columns)
				{
					if (!base.ColumnRows.Any((ColumnRow c) => c.Name == column.Name && c.TableName == column.TableName))
					{
						AddColumn(column);
					}
				}
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	private async Task<List<string>> GetDocuments(Container container, string query)
	{
		List<string> itemsJSONs = new List<string>();
		using (FeedIterator<object> feedIterator = container.GetItemQueryIterator<object>(string.Format(query, documentLimitToGet)))
		{
			while (feedIterator.HasMoreResults)
			{
				foreach (dynamic item in await feedIterator.ReadNextAsync())
				{
					itemsJSONs.Add(JsonConvert.SerializeObject(item));
				}
			}
		}
		return itemsJSONs;
	}

	public override bool GetTriggers(string query, Form owner = null)
	{
		try
		{
			CosmosClient cosmosClient = CommandsWithTimeout.CosmosDB(synchronizeParameters.DatabaseRow.Connection);
			if (cosmosClient == null)
			{
				return false;
			}
			IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Table);
			Database database = GetDatabase(cosmosClient, synchronizeParameters.DatabaseRow.Name);
			if (database == null)
			{
				return false;
			}
			foreach (string containerName in GetContainerNames(database))
			{
				List<ObjectRow> source = new List<ObjectRow>(synchronizeParameters.DatabaseRow.tableRows);
				if (IsObjectFiltered(containerName, rulesByObjectType) || source.Any((ObjectRow x) => x.Name == containerName && !x.ToSynchronize))
				{
					continue;
				}
				Container container = database.GetContainer(containerName);
				if (container == null)
				{
					continue;
				}
				foreach (TriggerProperties item in Task.Run(() => GetTriggerProperties(container, query)).Result)
				{
					TriggerSynchronizationObject dr = new TriggerSynchronizationObject(item.Id, synchronizeParameters.DatabaseRow.Name, containerName, synchronizeParameters.DatabaseRow.Name, item.TriggerOperation == TriggerOperation.Update, item.TriggerOperation == TriggerOperation.Delete, item.TriggerOperation == TriggerOperation.Create, item.TriggerType == TriggerType.Pre, item.TriggerType == TriggerType.Post, isInsteadOf: false, disabled: false, item.Body, null, SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Trigger));
					AddTrigger(dr);
				}
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	private async Task<List<TriggerProperties>> GetTriggerProperties(Container container, string query)
	{
		List<TriggerProperties> triggerProperties = new List<TriggerProperties>();
		using (FeedIterator<TriggerProperties> feedIterator = container.Scripts.GetTriggerQueryIterator<TriggerProperties>(query))
		{
			while (feedIterator.HasMoreResults)
			{
				foreach (TriggerProperties item in await feedIterator.ReadNextAsync())
				{
					triggerProperties.Add(item);
				}
			}
		}
		return triggerProperties;
	}

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		try
		{
			CosmosClient cosmosClient = CommandsWithTimeout.CosmosDB(synchronizeParameters.DatabaseRow.Connection);
			if (cosmosClient == null)
			{
				return false;
			}
			IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Table);
			Database database = GetDatabase(cosmosClient, synchronizeParameters.DatabaseRow.Name);
			if (database == null)
			{
				return false;
			}
			foreach (string containerName in GetContainerNames(database))
			{
				List<ObjectRow> source = new List<ObjectRow>(synchronizeParameters.DatabaseRow.tableRows);
				if (IsObjectFiltered(containerName, rulesByObjectType) || source.Any((ObjectRow x) => x.Name == containerName && !x.ToSynchronize))
				{
					continue;
				}
				Container container = database.GetContainer(containerName);
				if (container == null)
				{
					continue;
				}
				Collection<UniqueKey> uniqueKeys = container.ReadContainerAsync().Result.Resource.UniqueKeyPolicy.UniqueKeys;
				int num = 0;
				foreach (UniqueKey item in uniqueKeys)
				{
					string text = item.Paths.FirstOrDefault() ?? "/";
					string text2 = text.Substring(text.LastIndexOf("/") + 1);
					if (!string.IsNullOrEmpty(text2))
					{
						UniqueConstraintBaseObject dr = new UniqueConstraintBaseObject(text, synchronizeParameters.DatabaseRow.Name, synchronizeParameters.DatabaseRow.Name, containerName, text2, num, null, "U");
						AddUniqueConstraint(dr, SharedDatabaseTypeEnum.DatabaseType.CosmosDbSQL);
						num++;
					}
				}
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetRelations(string query, Form owner = null)
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

	private Database GetDatabase(CosmosClient client, string databaseName)
	{
		try
		{
			return client.GetDatabase(databaseName);
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, synchronizeParameters.Owner);
		}
		return null;
	}

	private List<string> GetContainerNames(Database database)
	{
		return Task.Run(() => GetContainerNamesAsync(database)).Result;
	}

	private static async Task<List<string>> GetContainerNamesAsync(Database database)
	{
		List<string> containersList = new List<string>();
		using (FeedIterator<ContainerProperties> iterator = database.GetContainerQueryIterator<ContainerProperties>())
		{
			FeedResponse<ContainerProperties> feedResponse = await iterator.ReadNextAsync().ConfigureAwait(continueOnCapturedContext: false);
			if (feedResponse == null)
			{
				return containersList;
			}
			foreach (ContainerProperties item in feedResponse)
			{
				containersList.Add(item.Id);
			}
		}
		return containersList;
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		return new List<string> { "SELECT * FROM c ORDER BY c._ts OFFSET 0 LIMIT {0}", "SELECT * FROM c ORDER BY c._ts DESC OFFSET 0 LIMIT {0}" };
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		yield return "SELECT * FROM u";
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		yield return "SELECT * FROM s";
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> RelationsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> TriggersQuery()
	{
		yield return "SELECT * FROM t";
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		yield return string.Empty;
	}
}
