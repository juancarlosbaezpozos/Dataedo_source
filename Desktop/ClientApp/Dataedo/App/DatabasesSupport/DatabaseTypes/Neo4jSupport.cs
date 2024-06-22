using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.Neo4j;
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
using Neo4j.Driver;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class Neo4jSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => false;

	public bool CanCreateImportCommand => false;

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

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Neo4j;

	public bool ShouldForceFullReimport => false;

	public bool SupportsReadingExternalDependencies => false;

	public Image TypeImage => Resources.neo4j;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public bool CanGenerateDDLScript => false;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Neo4j;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		if (version < base.VersionInfo.FirstSupportedVersion)
		{
			GeneralMessageBoxesHandling.Show(GetNotSupportedText(), "Unsupported version", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			return null;
		}
		if (version >= base.VersionInfo.FirstNotSupportedVersion)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(GetNotSupportedText(withQuestion: true), "Unsupported version", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 1, owner);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
			{
				return null;
			}
		}
		return SharedDatabaseTypeEnum.DatabaseType.Neo4j;
	}

	public void CloseConnection(object connection)
	{
		CommandsWithTimeout.Neo4j(connection)?.Dispose();
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotImplementedException();
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return GetConnectionString(database, host, login, password, withDatabase);
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		List<string> list = new List<string>();
		using (Neo4jSession connection = new Neo4jSession(connectionString))
		{
			if (new Version(GetDbmsVersion(connection)) < new Version("4.0.0"))
			{
				object obj = "You are trying to connect to a server that does not support multiple databases. Please upgrade to neo4j 4.0.0 or later in order to use this functionality";
				if (splashScreenManager != null)
				{
					CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
				}
				if (obj == null)
				{
					obj = "";
				}
				GeneralMessageBoxesHandling.Show((string)obj, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
				return null;
			}
		}
		using Neo4jSession neo4jSession = new Neo4jSession(connectionString);
		string query = "SHOW databases YIELD name return name";
		Action<SessionConfigBuilder> action = SessionConfigBuilder.ForDatabase("system");
		neo4jSession.InitConnection(action);
		foreach (IRecord item in neo4jSession.Run(query))
		{
			if (!(item["name"].ToString() == "system"))
			{
				list.Add(item["name"].ToString());
			}
		}
		return list;
	}

	public string GetDbmsVersion(object connection)
	{
		return Regex.Replace(CommandsWithTimeout.Neo4j(connection).Run("CALL dbms.components() yield versions unwind versions as version return version").Single()["version"].ToString(), "[^0-9.]", "");
	}

	public List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		throw new NotImplementedException();
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
		return "CONCAT(" + schemaFieldName + ", '.', " + nameFieldName + ")";
	}

	public string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Neo4j";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		throw new NotImplementedException();
	}

	public DateTime? GetServerTime(object connection)
	{
		return DateTime.ParseExact(CommandsWithTimeout.Neo4j(connection, "RETURN datetime()").First()["datetime()"].ToString().Replace('T', ' ').Split('.')[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
	}

	public SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeNeo4j(synchronizeParameters);
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.Neo4j;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		string dbmsVersion = GetDbmsVersion(connection);
		try
		{
			Version version = new Version(dbmsVersion);
			return new DatabaseVersionUpdate
			{
				Version = version.Major,
				Update = version.Minor,
				Build = ((version.Build >= 0) ? version.Build : 0)
			};
		}
		catch (Exception ex)
		{
			if (ex is ArgumentException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is FormatException || ex is OverflowException)
			{
				return null;
			}
			throw;
		}
	}

	public ConnectionBase GetXmlConnectionModel()
	{
		throw new NotSupportedException();
	}

	public bool PrepareElements(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl, Form owner = null)
	{
		return true;
	}

	public void ProcessException(Exception ex, string name, string warehouse, Form owner = null)
	{
		throw new NotSupportedException();
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
		Neo4jSession neo4jSession;
		try
		{
			neo4jSession = new Neo4jSession(connectionStringBuilder());
			neo4jSession.Run("MATCH () Return 1 Limit 1");
		}
		catch (AuthenticationException exception)
		{
			return new ConnectionResult(exception, "Wrong username or password");
		}
		catch (Neo4jException ex)
		{
			return new ConnectionResult(ex, ex.Message);
		}
		catch (Exception ex2)
		{
			return new ConnectionResult(ex2, ex2.Message);
		}
		return new ConnectionResult(null, null, neo4jSession);
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}

	private string GetConnectionString(string database, string host, string login, string password, bool withDatabase)
	{
		string text = "host=" + host.Trim() + ";login=" + login + ";password=" + password;
		if (withDatabase && !string.IsNullOrEmpty(database))
		{
			return text + ";database=" + database;
		}
		return text;
	}
}
