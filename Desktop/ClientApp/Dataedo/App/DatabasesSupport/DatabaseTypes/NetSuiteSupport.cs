using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.NetSuite;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Base;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using NetSuite.SuiteAnalyticsConnect;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class NetSuiteSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => false;

	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => false;

	public bool CanFilterBySchema => false;

	public bool CanGetDatabases => false;

	public bool CanImportToCustomFields => false;

	public string ChooseObjectPageDescriptionText => "Choose which objects' comments to export to the database.";

	public string ExportToDatabaseButtonDescription => "Export descriptions/custom fields to database (extended properties)";

	public string EmptyScriptMessage => string.Empty;

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool HasSslSettings => false;

	public bool IsSchemaType => false;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.NetSuite;

	public bool ShouldForceFullReimport => true;

	public bool SupportsReadingExternalDependencies => false;

	public Image TypeImage => Resources.net_suite_16;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public bool CanGenerateDDLScript => false;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.NetSuite;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.NetSuite;
	}

	public void CloseConnection(object connection)
	{
		if (connection is OpenAccessConnection openAccessConnection && openAccessConnection.State != 0)
		{
			openAccessConnection.Close();
		}
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotImplementedException();
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return GetConnectionString(host, port, login, password, param1, param2, param3);
	}

	private string GetConnectionString(string host, int? port, string login, string password, string serverDataSource, string accountId, string roleId)
	{
		if (!port.HasValue)
		{
			throw new ArgumentException("Specified port is not valid.");
		}
		return new OpenAccessConnectionStringBuilder
		{
			Host = host,
			Port = port.Value,
			ServerDataSource = serverDataSource,
			UserID = login,
			Password = password,
			Pooling = false,
			EncryptionMethod = "SSL",
			CustomProperties = "AccountID=" + accountId + ";RoleID=" + roleId
		}.ConnectionString;
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		throw new NotImplementedException();
	}

	public string GetDbmsVersion(object connection)
	{
		return "1";
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
		return new FilterObjectTypeEnum.FilterObjectType[1] { FilterObjectTypeEnum.FilterObjectType.Table };
	}

	public string GetFilterRuleConcatenation(string schemaFieldName, string nameFieldName)
	{
		throw new NotImplementedException();
	}

	public string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return base.FriendlyDisplayName;
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		throw new NotImplementedException();
	}

	public DateTime? GetServerTime(object connection)
	{
		using (OpenAccessCommand openAccessCommand = CommandsWithTimeout.NetSuiteCommand("SELECT TO_CHAR (SYSDATE, 'MM-DD-YYYY HH24:MI:SS') value FROM DUAL", connection))
		{
			using OpenAccessDataReader openAccessDataReader = openAccessCommand.ExecuteReader();
			if (openAccessDataReader.Read())
			{
				return DateTime.ParseExact(openAccessDataReader["value"] as string, "MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
			}
		}
		return null;
	}

	public SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeNetSuite(synchronizeParameters);
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.NetSuite;
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
		return new NetSuiteConnection();
	}

	public bool PrepareElements(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl, Form owner = null)
	{
		return true;
	}

	public void ProcessException(Exception ex, string name, string warehouse, Form owner = null)
	{
		throw new NotImplementedException();
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
		return TryConnection(connectionStringBuilder);
	}

	private ConnectionResult TryConnection(Func<string> connectionStringBuilder)
	{
		OpenAccessConnection openAccessConnection;
		try
		{
			openAccessConnection = new OpenAccessConnection(connectionStringBuilder());
			openAccessConnection.Open();
		}
		catch (OpenAccessException ex)
		{
			return new ConnectionResult(ex, GetOpenAccessExceptionMessage(ex));
		}
		catch (Exception ex2)
		{
			return new ConnectionResult(ex2, ex2.Message);
		}
		return new ConnectionResult(null, null, openAccessConnection);
	}

	private string GetOpenAccessExceptionMessage(OpenAccessException openAccessException)
	{
		if (openAccessException == null)
		{
			return string.Empty;
		}
		string text = openAccessException.Message;
		if (openAccessException.ErrorCode == 9535)
		{
			return "Connection timeout. Check your network settings or connection data.";
		}
		string text2 = "[OpenAccess SDK SQL Engine]";
		if (text.StartsWith(text2))
		{
			text = text.Substring(text2.Length);
		}
		string value = " Error ticket#";
		if (text.Contains(value))
		{
			text = text.Substring(0, text.LastIndexOf(value));
		}
		return text;
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotImplementedException();
	}
}
