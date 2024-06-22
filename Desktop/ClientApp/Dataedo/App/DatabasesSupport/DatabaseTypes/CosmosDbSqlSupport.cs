using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.CosmosDB;
using Dataedo.App.Data.General;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Base;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using Microsoft.Azure.Cosmos;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class CosmosDbSqlSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	private const string DbmsVersion = "1";

	public bool CanImportDependencies => false;

	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => false;

	public bool CanFilterBySchema => false;

	public bool CanGetDatabases => true;

	public bool CanImportToCustomFields => false;

	public string ChooseObjectPageDescriptionText => "Choose which objects' comments to export to the database.";

	public string ExportToDatabaseButtonDescription => "Export descriptions/custom fields to database (extended properties)";

	public string EmptyScriptMessage => string.Empty;

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool HasSslSettings => false;

	public bool IsSchemaType => false;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.CosmosDBSqlAPI;

	public bool ShouldForceFullReimport => true;

	public bool SupportsReadingExternalDependencies => false;

	public virtual Image TypeImage => Resources.cosmos_db_16;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.CosmosDbSQL;

	public bool CanGenerateDDLScript => false;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.CosmosDbSQL;
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotSupportedException();
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string authenticationDatabase = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		if (!string.IsNullOrEmpty(userConnectionString))
		{
			return userConnectionString;
		}
		return GetConnectionString(host, password);
	}

	public string GetConnectionString(string host, string password)
	{
		return "AccountEndpoint=" + host + ";AccountKey=" + password;
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		try
		{
			return Task.Run(() => GetDatabasesAsync(connectionString)).Result;
		}
		catch (Exception ex)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			if (ex.InnerException is CosmosException ex2 && ex2.StatusCode == HttpStatusCode.Unauthorized)
			{
				GeneralMessageBoxesHandling.Show("Could not authorize access with the given credentials.\r\nPlease make sure that URI and Primary Key are correct.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			}
			else
			{
				GeneralMessageBoxesHandling.Show(ex.InnerException.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			}
		}
		return null;
	}

	private static async Task<List<string>> GetDatabasesAsync(string connectionString)
	{
		List<string> databases = new List<string>();
		CosmosClient cosmosClient = CommandsWithTimeout.CosmosDB(connectionString);
		using (FeedIterator<DatabaseProperties> iterator = cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>())
		{
			while (iterator.HasMoreResults)
			{
				foreach (DatabaseProperties item in await iterator.ReadNextAsync())
				{
					databases.Add(item.Id);
				}
			}
		}
		return databases;
	}

	public IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		throw new NotSupportedException();
	}

	public FilterObjectTypeEnum.FilterObjectType[] GetFilterObjectTypes()
	{
		return new FilterObjectTypeEnum.FilterObjectType[3]
		{
			FilterObjectTypeEnum.FilterObjectType.Table,
			FilterObjectTypeEnum.FilterObjectType.Procedure,
			FilterObjectTypeEnum.FilterObjectType.Function
		};
	}

	public string GetFilterRuleConcatenation(string schemaFieldName, string nameFieldName)
	{
		throw new NotSupportedException();
	}

	public virtual string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Azure Cosmos DB - SQL/Core API";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		throw new NotSupportedException();
	}

	public DateTime? GetServerTime(object connection)
	{
		return DateTime.UtcNow;
	}

	public SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeCosmosDB(synchronizeParameters);
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.CosmosDBSql;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		return new DatabaseVersionUpdate
		{
			Version = 1,
			Update = 1
		};
	}

	public ConnectionBase GetXmlConnectionModel()
	{
		return new CosmosDBSqlConnection();
	}

	public bool PrepareElements(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl, Form owner = null)
	{
		return true;
	}

	public void ProcessException(Exception ex, string name, string warehouse, Form owner = null)
	{
		GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
	}

	public bool ShouldSynchronizeComputedFormula(object connection)
	{
		return false;
	}

	public bool ShouldSynchronizeParameters(object connection)
	{
		return false;
	}

	public ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		CosmosClient client = null;
		try
		{
			client = CommandsWithTimeout.CosmosDB(connectionStringBuilder());
			_ = Task.Run(() => ReadAccountAsync(client)).Result;
			if (!string.IsNullOrEmpty(name))
			{
				List<string> databases = GetDatabases(connectionStringBuilder(), null, name, warehouse);
				if (databases == null || !databases.Contains(name))
				{
					return new ConnectionResult(new Exception(), "Could not find given database name.");
				}
			}
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			if (ex.InnerException is CosmosException ex2 && ex2.StatusCode == HttpStatusCode.Unauthorized)
			{
				message = "Could not authorize access with the given credentials.\r\nPlease make sure that URI and Primary Key are correct.";
			}
			return new ConnectionResult(ex, message);
		}
		return new ConnectionResult(null, null, client);
	}

	private async Task<bool> ReadAccountAsync(CosmosClient client)
	{
		if (await client.ReadAccountAsync() != null)
		{
			return true;
		}
		return false;
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		CosmosClient cosmosClient = CommandsWithTimeout.CosmosDB(connectionString);
		try
		{
			cosmosClient.GetDatabase(name);
		}
		catch (ArgumentException ex)
		{
			message = ex.Message;
			return false;
		}
		return true;
	}

	public List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		throw new NotImplementedException();
	}

	public string GetDbmsVersion(object connection)
	{
		return "1";
	}

	public void CloseConnection(object connection)
	{
	}
}
