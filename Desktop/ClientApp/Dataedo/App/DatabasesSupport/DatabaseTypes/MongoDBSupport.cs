using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.MongoDB;
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
using Dataedo.CustomMessageBox;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Misc;
using MongoDB.Driver.Core.Servers;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class MongoDBSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => true;

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

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.MongoDB;

	public bool ShouldForceFullReimport => true;

	public bool SupportsReadingExternalDependencies => false;

	public Image TypeImage => Resources.mongodb;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.MongoDB;

	public bool CanGenerateDDLScript => false;

	public static string GetConnectionStringWihoutPassword(string connectionString)
	{
		return new MongoUrlBuilder(connectionString)
		{
			Password = "-password-removed-"
		}.ToString();
	}

	public static string GetConnectionStringWithPassword(string connectionString, string password)
	{
		return new MongoUrlBuilder(connectionString)
		{
			Password = password
		}.ToString();
	}

	public static string GetHostFromConnectionString(string connectionString)
	{
		MongoUrlBuilder mongoUrlBuilder = new MongoUrlBuilder(connectionString);
		if (mongoUrlBuilder.Servers.Count() > 1)
		{
			return mongoUrlBuilder.Servers.FirstOrDefault().Host.ToString();
		}
		return mongoUrlBuilder.Server.Host.ToString();
	}

	public static string GetMultiHostFromConnectionString(string connectionString)
	{
		List<string> values = new MongoUrlBuilder(connectionString).Servers.Select((MongoServerAddress x) => x.ToString()).ToList();
		return string.Join(",", values);
	}

	public static int? GetPortFromConnectionString(string connectionString)
	{
		MongoUrlBuilder mongoUrlBuilder = new MongoUrlBuilder(connectionString);
		if (mongoUrlBuilder.Servers.Count() > 1)
		{
			return mongoUrlBuilder.Servers.FirstOrDefault().Port;
		}
		return mongoUrlBuilder.Server.Port;
	}

	public static string GetLoginFromConnectionString(string connectionString)
	{
		return new MongoUrlBuilder(connectionString).Username;
	}

	public static string GetPasswordFromConnectionString(string connectionString)
	{
		return new MongoUrlBuilder(connectionString).Password;
	}

	public static bool GetSrvFromConnectionString(string connectionString)
	{
		return new MongoUrlBuilder(connectionString).Scheme == ConnectionStringScheme.MongoDBPlusSrv;
	}

	public static bool IsInvalidDatabase(MongoCommandException mongoCommandException)
	{
		if (!mongoCommandException.Message.Contains("Max database name length is 38 bytes.."))
		{
			return mongoCommandException.CodeName == "InvalidNamespace";
		}
		return true;
	}

	public static bool PrepareAllMongoDbSchemas(DatabaseRow databaseRow, List<string> databases, Form owner = null)
	{
		if (databases == null)
		{
			return false;
		}
		IEnumerable<string> source = databases.Where((string x) => !string.IsNullOrEmpty(x));
		string text = DatabaseRow.PrepareSchemasList(source.ToList());
		if (StaticData.IsProjectFile && text.Length > 4000)
		{
			string text2 = "Too many databases to correctly import to a file repository." + Environment.NewLine + "Choose a smaller number of databases (empty database field means all databases on the host) to continue, or use server repository (" + Links.CreatingServerRepository + ") to import this many databases.";
			string message = "Too many databases to correctly import to a file repository." + Environment.NewLine + Environment.NewLine + "Choose a smaller number of databases (empty database field means all databases on the host) to continue, or use <href=" + Links.CreatingServerRepository + ">server repository</href> to import this many databases.";
			string messageSimple = text2;
			GeneralMessageBoxesHandling.Show(message, "Too many databases", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, owner, messageSimple);
			return false;
		}
		databaseRow.HasMultipleSchemas = source.Count() > 1;
		databaseRow.UseDifferentSchema = true;
		databaseRow.Name = text;
		return true;
	}

	public virtual SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		if (version < base.VersionInfo.FirstSupportedVersion || version >= base.VersionInfo.FirstNotSupportedVersion)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(GetNotSupportedText(withQuestion: true), "Unsupported version", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 1, owner);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
			{
				return null;
			}
		}
		return SharedDatabaseTypeEnum.DatabaseType.MongoDB;
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
		return GetConnectionString(host, login, password, isSrv, authenticationDatabase, replicaSet, multiHost, SSLType);
	}

	public string GetConnectionString(string host, string login, string password, bool isSrv, string database, string identifierName, string multiHost, string SSLType)
	{
		MongoUrlBuilder mongoUrlBuilder = new MongoUrlBuilder();
		if (isSrv)
		{
			mongoUrlBuilder.Scheme = ConnectionStringScheme.MongoDBPlusSrv;
			new List<MongoServerAddress>();
			List<MongoServerAddress> list = (List<MongoServerAddress>)(mongoUrlBuilder.Servers = (from x in multiHost.Split(',')
				select new MongoServerAddress(x)).ToList());
		}
		else
		{
			mongoUrlBuilder.Scheme = ConnectionStringScheme.MongoDB;
			new List<MongoServerAddress>();
			List<MongoServerAddress> list2 = (List<MongoServerAddress>)(mongoUrlBuilder.Servers = (from x in multiHost.Split(',')
				select new MongoServerAddress(x)).ToList());
		}
		if (!string.IsNullOrEmpty(login))
		{
			mongoUrlBuilder.Username = login;
			if (!string.IsNullOrEmpty(password))
			{
				mongoUrlBuilder.Password = password;
			}
		}
		if (!string.IsNullOrEmpty(database))
		{
			mongoUrlBuilder.DatabaseName = database;
		}
		if (!string.IsNullOrEmpty(identifierName))
		{
			mongoUrlBuilder.ReplicaSetName = identifierName;
		}
		mongoUrlBuilder.UseTls = Convert.ToBoolean(SSLType);
		return mongoUrlBuilder.ToString();
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		MongoClient mongoClient = null;
		try
		{
			mongoClient = CommandsWithTimeout.MongoDb(connectionString);
			string[] systemDatabases = new string[3] { "local", "admin", "config" };
			List<string> list = mongoClient.ListDatabaseNames().ToList();
			list.RemoveAll((string x) => systemDatabases.Contains(x));
			return list;
		}
		catch (Exception ex)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			GeneralMessageBoxesHandling.Show(GetExceptionMessage(ex, mongoClient), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		return null;
	}

	public IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		throw new NotSupportedException();
	}

	public FilterObjectTypeEnum.FilterObjectType[] GetFilterObjectTypes()
	{
		return new FilterObjectTypeEnum.FilterObjectType[1] { FilterObjectTypeEnum.FilterObjectType.Table };
	}

	public string GetFilterRuleConcatenation(string schemaFieldName, string nameFieldName)
	{
		throw new NotSupportedException();
	}

	public virtual string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "MongoDB";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		throw new NotSupportedException();
	}

	public DateTime? GetServerTime(object connection)
	{
		MongoClient mongoClient = CommandsWithTimeout.MongoDb(connection);
		IClientSessionHandle clientSessionHandle = null;
		try
		{
			using (clientSessionHandle = mongoClient.StartSession())
			{
				return clientSessionHandle.ServerSession.LastUsedAt ?? DateTime.UtcNow;
			}
		}
		finally
		{
			try
			{
				clientSessionHandle?.Dispose();
			}
			catch (Exception)
			{
			}
		}
	}

	public SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeMongoDB(synchronizeParameters);
	}

	public virtual DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.MongoDB;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		SemanticVersion semanticVersion = (from x in GetSemanticVersions(connection, primaryOnlyIfPossible: true)
			orderby x descending
			select x).FirstOrDefault();
		return new DatabaseVersionUpdate
		{
			Version = (semanticVersion?.Major ?? 0),
			Update = (semanticVersion?.Minor ?? 0),
			Build = (semanticVersion?.Patch ?? 0)
		};
	}

	public virtual ConnectionBase GetXmlConnectionModel()
	{
		return new MongoDBConnection();
	}

	public bool PrepareElements(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl, Form owner = null)
	{
		PrepareDatabasesData(databaseRow, ref elements, elementsButtonEdit, elementsCountLabelControl);
		List<string> databases = databaseRow.GetDatabases();
		for (int i = 0; i < databaseRow.Schemas.Count; i++)
		{
			string text = databaseRow.Schemas[i];
			if (databaseRow.Schemas.Count == 1 && string.IsNullOrEmpty(text))
			{
				continue;
			}
			string message = null;
			if (!ValidateDatabase(databaseRow.ConnectionString, text, ref message))
			{
				if (CustomMessageBoxForm.Show("Database \"" + text + "\" is invalid. " + Environment.NewLine + "Unable to continue with invalid names." + Environment.NewLine + Environment.NewLine + "Do you want to remove it from list?", "Database is invalid.", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, message, 1, owner) != DialogResult.Yes)
				{
					return false;
				}
				if (RemoveDatabaseFromList(databaseRow, ref elements, elementsButtonEdit, elementsCountLabelControl, text))
				{
					i--;
				}
			}
			else
			{
				if (databases.Contains(text))
				{
					continue;
				}
				switch (CustomMessageBoxForm.Show("Database \"" + text + "\" does not exist in " + databaseRow.Host + "." + Environment.NewLine + Environment.NewLine + "Do you want to remove it from list?", "Database does not exist.", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, owner))
				{
				case DialogResult.Yes:
					if (RemoveDatabaseFromList(databaseRow, ref elements, elementsButtonEdit, elementsCountLabelControl, text))
					{
						i--;
					}
					break;
				case DialogResult.None:
				case DialogResult.Cancel:
					return false;
				}
			}
		}
		if (string.IsNullOrEmpty(databaseRow.Name))
		{
			return PrepareAllMongoDbSchemas(databaseRow, databases, owner);
		}
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
		MongoClient mongoClient = null;
		try
		{
			mongoClient = CommandsWithTimeout.MongoDb(connectionStringBuilder());
			using (mongoClient.StartSession())
			{
				mongoClient.ListDatabaseNames();
			}
		}
		catch (Exception ex)
		{
			return new ConnectionResult(ex, GetExceptionMessage(ex, mongoClient));
		}
		return new ConnectionResult(null, null, mongoClient);
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		MongoClient mongoClient = CommandsWithTimeout.MongoDb(connectionString);
		try
		{
			mongoClient.GetDatabase(name).ListCollectionNames().FirstOrDefault();
		}
		catch (MongoCommandException ex)
		{
			if (IsInvalidDatabase(ex))
			{
				message = ex.Message;
				return false;
			}
			throw;
		}
		catch (ArgumentException ex2)
		{
			message = ex2.Message;
			return false;
		}
		return true;
	}

	public static string GetExceptionMessage(Exception ex, MongoClient client, out bool isHandled)
	{
		isHandled = true;
		if (ex.TargetSite.Name == "ThrowTimeoutException")
		{
			if (client?.Cluster?.Description?.Servers?.FirstOrDefault()?.HeartbeatException is MongoConnectionException ex2)
			{
				return ex2.Message + Environment.NewLine + Environment.NewLine + ex2?.InnerException?.Message + ".";
			}
			if (ex is TimeoutException)
			{
				return "A timeout occurred." + Environment.NewLine + Environment.NewLine + ex.Message;
			}
			return ex.Message;
		}
		if (ex is MongoConnectionException ex3)
		{
			return ex3.Message + Environment.NewLine + Environment.NewLine + ex3?.InnerException?.InnerException?.Message + ".";
		}
		if (ex is MongoAuthenticationException ex4)
		{
			return ex4.Message + Environment.NewLine + Environment.NewLine + ex4?.InnerException?.Message + ".";
		}
		if (ex is MongoConfigurationException ex5)
		{
			return ex5.Message ?? "";
		}
		if (ex is ArgumentException ex6)
		{
			return ex6.Message ?? "";
		}
		isHandled = false;
		return ex.ToString();
	}

	public static string GetExceptionMessage(Exception ex, out bool isHandled)
	{
		return GetExceptionMessage(ex, null, out isHandled);
	}

	private static string GetExceptionMessage(Exception ex, MongoClient client)
	{
		bool isHandled;
		return GetExceptionMessage(ex, client, out isHandled);
	}

	private IEnumerable<SemanticVersion> GetSemanticVersions(object connection, bool primaryOnlyIfPossible = false, Form owner = null)
	{
		MongoClient mongoClient = null;
		try
		{
			mongoClient = CommandsWithTimeout.MongoDb(connection);
			IEnumerable<SemanticVersion> enumerable = null;
			IClientSessionHandle clientSessionHandle = null;
			try
			{
				using (clientSessionHandle = mongoClient.StartSession())
				{
					enumerable = GetSemanticVersions(mongoClient, primaryOnlyIfPossible);
					if (enumerable != null)
					{
						return enumerable;
					}
				}
			}
			finally
			{
				try
				{
					clientSessionHandle?.Dispose();
				}
				catch (Exception)
				{
				}
			}
			mongoClient.ListDatabaseNames();
			enumerable = GetSemanticVersions(mongoClient, primaryOnlyIfPossible);
			if (enumerable == null)
			{
				enumerable = GetSemanticVersions(mongoClient, primaryOnlyIfPossible);
			}
			return enumerable;
		}
		catch (Exception ex2)
		{
			GeneralMessageBoxesHandling.Show(GetExceptionMessage(ex2, mongoClient), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		return null;
	}

	private IEnumerable<SemanticVersion> GetSemanticVersions(MongoClient client, bool primaryOnlyIfPossible = false)
	{
		List<SemanticVersion> list = new List<SemanticVersion>();
		SemanticVersion[] primaryVersions = null;
		SemanticVersion[] array = null;
		primaryVersions = client.Cluster.Description.Servers?.Where((ServerDescription x) => x.Type == ServerType.ReplicaSetPrimary)?.Select((ServerDescription x) => x.Version)?.GroupBy((SemanticVersion x) => x)?.Select((IGrouping<SemanticVersion, SemanticVersion> x) => x.Key)?.ToArray();
		if (primaryVersions != null && primaryVersions.Length != 0)
		{
			list.AddRange(primaryVersions);
		}
		if (!primaryOnlyIfPossible || list.Count == 0)
		{
			array = client.Cluster.Description.Servers?.Where((ServerDescription x) => x.Type != ServerType.ReplicaSetPrimary)?.Select((ServerDescription x) => x.Version)?.GroupBy((SemanticVersion x) => x)?.Select((IGrouping<SemanticVersion, SemanticVersion> x) => x.Key)?.Where((SemanticVersion x) => !primaryVersions.Contains(x))?.ToArray();
			if (array != null)
			{
				list.AddRange(array);
			}
		}
		return list;
	}

	private bool RemoveDatabaseFromList(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl, string database)
	{
		if (databaseRow.Schemas.Remove(database))
		{
			PrepareDatabasesData(databaseRow, ref elements, elementsButtonEdit, elementsCountLabelControl);
			return true;
		}
		return false;
	}

	private void PrepareDatabasesData(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl)
	{
		string name = DatabaseRow.PrepareSchemasList(databaseRow.Schemas);
		databaseRow.HasMultipleSchemas = databaseRow.Schemas.Count() > 1;
		databaseRow.UseDifferentSchema = true;
		databaseRow.Name = name;
		databaseRow.UseDifferentSchema = databaseRow.Schemas.Count() == 0 || (databaseRow.Schemas.Count() == 1 && string.IsNullOrEmpty(databaseRow.Schemas[0]));
		elements = new List<string>(databaseRow.Schemas ?? new List<string>());
		DatabaseSupportBase.SetElementsButtonEdit(elementsButtonEdit, elements);
		DatabaseSupportBase.SetElementsLabelControl(elementsCountLabelControl, elements);
	}

	public List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		throw new NotImplementedException();
	}

	public string GetDbmsVersion(object connection)
	{
		return string.Join(", ", GetSemanticVersions(connection))?.ToString() ?? null;
	}

	public void CloseConnection(object connection)
	{
	}
}
