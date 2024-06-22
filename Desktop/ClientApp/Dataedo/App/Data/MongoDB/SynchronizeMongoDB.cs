using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Model.Data.MongoDB;
using Dataedo.Shared.Enums;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace Dataedo.App.Data.MongoDB;

internal class SynchronizeMongoDB : SynchronizeDatabase
{
	private int documentLimitToGet = 50;

	private string unauthorizedColectionMessage = "Dataedo couldn't access this collection's data. To see more details, reimport this collection with a user that has access to it granted.";

	public SynchronizeMongoDB(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Counting objects to import");
		try
		{
			int num = 0;
			IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Table);
			MongoClient client = synchronizeParameters.DatabaseRow.Connection as MongoClient;
			List<string> importedDatabasesNames = GetImportedDatabasesNames();
			List<string> databases = synchronizeParameters.DatabaseRow.GetDatabases(null, forceStandardConnection: false, synchronizeParameters.Owner);
			foreach (string item in importedDatabasesNames)
			{
				IMongoDatabase database = GetDatabase(client, item);
				if (database == null)
				{
					synchronizeParameters.Log?.Write("Database \"" + item + "\" is invalid.");
					continue;
				}
				if (!databases.Contains(item))
				{
					synchronizeParameters.Log?.Write("Database \"" + item + "\" does not exist in " + synchronizeParameters.DatabaseRow.Host + ".");
					continue;
				}
				foreach (string userCollectionName in GetUserCollectionNames(database))
				{
					if (!IsCollectionFiltered(userCollectionName, rulesByObjectType))
					{
						num++;
					}
				}
			}
			ReadCount(SharedObjectTypeEnum.ObjectType.Table, num);
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, synchronizeParameters.Owner);
			return false;
		}
		return true;
	}

	public override bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (base.ObjectsCounter != null)
		{
			try
			{
				IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Table);
				backgroundWorkerManager.ReportProgress("Retrieving database's objects");
				MongoClient client = synchronizeParameters.DatabaseRow.Connection as MongoClient;
				foreach (string importedDatabasesName in GetImportedDatabasesNames())
				{
					IMongoDatabase database = GetDatabase(client, importedDatabasesName);
					if (database == null)
					{
						continue;
					}
					ListCollectionNamesOptions listCollectionNamesOptions = new ListCollectionNamesOptions();
					listCollectionNamesOptions.Filter = new BsonDocument("type", "collection");
					foreach (string userCollectionName in GetUserCollectionNames(database, listCollectionNamesOptions))
					{
						if (!IsCollectionFiltered(userCollectionName, rulesByObjectType))
						{
							database.GetCollection<BsonDocument>(userCollectionName);
							BsonTable dr = new BsonTable(userCollectionName, importedDatabasesName);
							ObservableCollection<ObjectRow> tableRows = synchronizeParameters.DatabaseRow.tableRows;
							Form owner2 = owner;
							AddDBObject(dr, tableRows, backgroundWorkerManager, null, null, owner2);
						}
					}
					ListCollectionNamesOptions listCollectionNamesOptions2 = new ListCollectionNamesOptions();
					listCollectionNamesOptions2.Filter = new BsonDocument("type", "view");
					foreach (string collectionName in GetUserCollectionNames(database, listCollectionNamesOptions2))
					{
						if (IsCollectionFiltered(collectionName, rulesByObjectType))
						{
							continue;
						}
						database.GetCollection<BsonDocument>(collectionName);
						IMongoDatabase database2 = GetDatabase(client, importedDatabasesName);
						if (database2 != null)
						{
							BsonDocument obj = database2.ListCollections().ToList().Find((BsonDocument x) => x["name"] == collectionName);
							obj.ToBsonDocument().GetValue("options").ToBsonDocument();
							string viewPipeline = obj.ToJson(new JsonWriterSettings
							{
								Indent = true
							});
							BsonTable dr2 = new BsonTable(collectionName, importedDatabasesName, SharedObjectTypeEnum.ObjectType.View, viewPipeline);
							ObservableCollection<ObjectRow> tableRows2 = synchronizeParameters.DatabaseRow.tableRows;
							Form owner2 = owner;
							AddDBObject(dr2, tableRows2, backgroundWorkerManager, null, null, owner2);
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

	public override bool GetRelations(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetColumns(string query, Form owner = null)
	{
		try
		{
			IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Table);
			MongoClient client = synchronizeParameters.DatabaseRow.Connection as MongoClient;
			foreach (string databaseName in GetImportedDatabasesNames())
			{
				IMongoDatabase database = GetDatabase(client, databaseName);
				if (database == null)
				{
					continue;
				}
				foreach (string collectionName in GetUserCollectionNames(database))
				{
					List<ObjectRow> source = new List<ObjectRow>(synchronizeParameters.DatabaseRow.tableRows);
					if (IsCollectionFiltered(collectionName, rulesByObjectType) || source.Any((ObjectRow x) => x.Name == collectionName && x.Schema == databaseName && !x.ToSynchronize))
					{
						continue;
					}
					IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);
					BsonTable bsonTable = new BsonTable(collectionName, databaseName);
					try
					{
						foreach (BsonDocument item in collection.Find((BsonDocument _) => true).Limit(documentLimitToGet).ToList())
						{
							bsonTable.AddDocument(item);
						}
						BsonSchema bsonSchema = new BsonSchema(client, databaseName, collectionName);
						if (bsonSchema.Schema != null)
						{
							bsonTable.AddSchema(bsonSchema);
						}
						foreach (BsonColumn column in bsonTable.Columns)
						{
							AddColumn(column);
						}
					}
					catch (MongoCommandException ex)
					{
						if (ex.HResult == -2146233088)
						{
							source.Where((ObjectRow x) => x.Name == collectionName && x.Schema == databaseName).FirstOrDefault().Description = unauthorizedColectionMessage;
							continue;
						}
						throw ex;
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

	public override bool GetTriggers(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		try
		{
			IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Table);
			MongoClient client = synchronizeParameters.DatabaseRow.Connection as MongoClient;
			foreach (string databaseName in GetImportedDatabasesNames())
			{
				IMongoDatabase database = GetDatabase(client, databaseName);
				if (database == null)
				{
					continue;
				}
				foreach (string collectionName in GetUserCollectionNames(database))
				{
					List<ObjectRow> source = new List<ObjectRow>(synchronizeParameters.DatabaseRow.tableRows);
					if (IsCollectionFiltered(collectionName, rulesByObjectType) || source.Any((ObjectRow x) => x.Name == collectionName && x.Schema == databaseName && !x.ToSynchronize))
					{
						continue;
					}
					IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);
					BsonTable bsonTable = new BsonTable(collectionName, databaseName);
					try
					{
						foreach (BsonDocument item in collection.Find((BsonDocument _) => true).Limit(documentLimitToGet).ToList())
						{
							bsonTable.AddDocument(item);
						}
						foreach (BsonColumn column in bsonTable.Columns)
						{
							if (column.Name == "_id")
							{
								MongoDBConstraint dr = new MongoDBConstraint("_id", synchronizeParameters.DatabaseRow.Name, databaseName, collectionName, "_id", column.Position);
								AddUniqueConstraint(dr, SharedDatabaseTypeEnum.DatabaseType.MongoDB);
							}
						}
					}
					catch (MongoCommandException ex)
					{
						if (ex.HResult == -2146233088)
						{
							source.Where((ObjectRow x) => x.Name == collectionName && x.Schema == databaseName).FirstOrDefault().Description = unauthorizedColectionMessage;
							continue;
						}
						throw ex;
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

	public override bool GetParameters(string query, bool canReadParameters = true, Form owner = null)
	{
		return true;
	}

	public override bool GetDependencies(string query, Form owner = null)
	{
		try
		{
			IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Table);
			MongoClient client = synchronizeParameters.DatabaseRow.Connection as MongoClient;
			foreach (string importedDatabasesName in GetImportedDatabasesNames())
			{
				IMongoDatabase database = GetDatabase(client, importedDatabasesName);
				if (database == null)
				{
					continue;
				}
				ListCollectionNamesOptions listCollectionNamesOptions = new ListCollectionNamesOptions();
				listCollectionNamesOptions.Filter = new BsonDocument("type", "view");
				foreach (string collectionName in GetUserCollectionNames(database, listCollectionNamesOptions))
				{
					if (!IsCollectionFiltered(collectionName, rulesByObjectType))
					{
						database.GetCollection<BsonDocument>(collectionName);
						string referencedEntityName = database.ListCollections().ToList().Find((BsonDocument x) => x["name"] == collectionName)
							.GetValue("options")
							.ToBsonDocument()
							.GetValue("viewOn")
							.ToString();
						MongoDBDependency mongoDBDependency = new MongoDBDependency();
						mongoDBDependency.ReferencingServer = synchronizeParameters.DatabaseRow.Host;
						mongoDBDependency.ReferencingSchemaName = importedDatabasesName;
						mongoDBDependency.ReferencingDatabaseName = synchronizeParameters.DatabaseRow.Host;
						mongoDBDependency.ReferencingEntityName = collectionName;
						mongoDBDependency.ReferencedServer = synchronizeParameters.DatabaseRow.Host;
						mongoDBDependency.ReferencedDatabaseName = synchronizeParameters.DatabaseRow.Host;
						mongoDBDependency.ReferencedSchemaName = importedDatabasesName;
						mongoDBDependency.ReferencedEntityName = referencedEntityName;
						mongoDBDependency.IsCallerDependent = null;
						mongoDBDependency.IsAmbiguous = null;
						mongoDBDependency.DependencyType = null;
						AddDependency(mongoDBDependency);
					}
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

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		yield return string.Empty;
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
		yield return string.Empty;
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		yield return string.Empty;
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
		yield return string.Empty;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		yield return string.Empty;
	}

	private bool Like(string toSearch, string toFind)
	{
		return new Regex("\\A" + new Regex("\\.|\\$|\\^|\\{|\\[|\\(|\\||\\)|\\*|\\+|\\?|\\\\").Replace(toFind, (Match ch) => "\\" + ch).Replace('_', '.').Replace("%", ".*") + "\\z", RegexOptions.Singleline).IsMatch(toSearch);
	}

	private bool IsCollectionFiltered(string collectionName, IEnumerable<FilterRule> filterRules)
	{
		foreach (FilterRule item in filterRules.Where((FilterRule rule) => rule.RuleType == FilterRuleType.Exclude))
		{
			if (Like(collectionName.ToUpper(), item.PreparedName))
			{
				return true;
			}
		}
		foreach (FilterRule item2 in filterRules.Where((FilterRule rule) => rule.RuleType == FilterRuleType.Include))
		{
			if (Like(collectionName.ToUpper(), item2.PreparedName))
			{
				return false;
			}
		}
		return true;
	}

	private IMongoDatabase GetDatabase(MongoClient client, string databaseName)
	{
		try
		{
			client.GetDatabase(databaseName).ListCollectionNames().FirstOrDefault();
			return client.GetDatabase(databaseName);
		}
		catch (MongoCommandException mongoCommandException)
		{
			if (!MongoDBSupport.IsInvalidDatabase(mongoCommandException))
			{
				throw;
			}
		}
		catch (ArgumentException ex)
		{
			if (ex.ParamName != "databaseName")
			{
				throw;
			}
		}
		catch (Exception ex2)
		{
			bool isHandled;
			string exceptionMessage = MongoDBSupport.GetExceptionMessage(ex2, out isHandled);
			if (!isHandled)
			{
				throw;
			}
			GeneralMessageBoxesHandling.Show(exceptionMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, synchronizeParameters.Owner);
		}
		return null;
	}

	private List<string> GetImportedDatabasesNames()
	{
		List<string> list = new List<string>();
		IEnumerable<string> enumerable = synchronizeParameters.DatabaseRow.Schemas.Where((string x) => !string.IsNullOrEmpty(x));
		if (enumerable.Count() > 0)
		{
			list.AddRange(enumerable);
		}
		else
		{
			synchronizeParameters.DatabaseRow.Schemas.AddRange(list);
		}
		string[] systemDatabases = new string[3] { "local", "admin", "config" };
		list.RemoveAll((string x) => systemDatabases.Contains(x));
		return list;
	}

	private List<string> GetUserCollectionNames(IMongoDatabase database, ListCollectionNamesOptions filter = null)
	{
		List<string> list = database.ListCollectionNames(filter).ToList();
		list.RemoveAll((string x) => x.StartsWith("system."));
		return list;
	}
}
