using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.SsasTabular;
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
using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class SsasTabularSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared, IPerspectiveDatabase
{
	public bool CanImportDependencies => false;

	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => false;

	public bool CanFilterBySchema => true;

	public bool CanGetDatabases => true;

	public bool CanImportToCustomFields => true;

	public string ChooseObjectPageDescriptionText => "Choose which objects' comments to export to the database.";

	public string ExportToDatabaseButtonDescription => "Export comments to database";

	public string EmptyScriptMessage => "The script is not available.";

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool HasSslSettings => false;

	public bool IsSchemaType => true;

	public virtual PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.SsasTabular;

	public bool ShouldForceFullReimport => true;

	public bool SupportsReadingExternalDependencies => false;

	public virtual Image TypeImage => Resources.ssas_tabular_16;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.SsasTabular;

	public bool CanGenerateDDLScript => false;

	public virtual SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.SsasTabular;
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotImplementedException();
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return GetConnectionString(host, login, password, authenticationType);
	}

	private static string GetConnectionString(string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType)
	{
		SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder
		{
			DataSource = host,
			PersistSecurityInfo = true
		};
		switch (authenticationType)
		{
		case AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryIntegrated:
			return sqlConnectionStringBuilder.ToString();
		case AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryPassword:
			sqlConnectionStringBuilder.UserID = login;
			sqlConnectionStringBuilder.Password = password;
			return sqlConnectionStringBuilder.ToString() + "; Impersonation Level=Impersonate;";
		case AuthenticationType.AuthenticationTypeEnum.WindowsAuthentication:
			return sqlConnectionStringBuilder.ToString() + "; Integrated Security=SSPI;";
		case AuthenticationType.AuthenticationTypeEnum.StandardAuthentication:
			sqlConnectionStringBuilder.UserID = login;
			sqlConnectionStringBuilder.Password = password;
			break;
		default:
			sqlConnectionStringBuilder.UserID = login;
			sqlConnectionStringBuilder.Password = password;
			break;
		}
		return sqlConnectionStringBuilder.ToString();
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		try
		{
			using Microsoft.AnalysisServices.Server server = new Microsoft.AnalysisServices.Server();
			server.Connect(connectionString);
			List<string> list = new List<string>();
			list.AddRange(server.Databases.OfType<Microsoft.AnalysisServices.Database>()?.Select((Microsoft.AnalysisServices.Database d) => d.Name)?.ToList());
			return list;
		}
		catch (Exception ex)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			GeneralMessageBoxesHandling.Show("Unable to connect to server." + Environment.NewLine + Environment.NewLine + (ex?.InnerException?.Message ?? ex?.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			return null;
		}
	}

	public List<string> GetPerspectiveNames(string connectionString, object connection, string databaseName, Form owner)
	{
		try
		{
			Microsoft.AnalysisServices.Server server = CommandsWithTimeout.SSASTabular(connection);
			if (server != null && server.Connected)
			{
				return GetPerspectiveNamesList(server, databaseName, owner);
			}
			using (server = new Microsoft.AnalysisServices.Server())
			{
				server.Connect(connectionString);
				return GetPerspectiveNamesList(server, databaseName, owner);
			}
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to connect to server." + Environment.NewLine + Environment.NewLine + (ex?.InnerException?.Message ?? ex?.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			return null;
		}
	}

	private List<string> GetPerspectiveNamesList(Microsoft.AnalysisServices.Server server, string databaseName, Form owner)
	{
		List<string> list = new List<string>();
		try
		{
			Microsoft.AnalysisServices.Database database = server.Databases.FindByName(databaseName);
			if (database?.Model == null)
			{
				return list;
			}
			list.Add(database?.Model.Name);
			List<string> list2 = database?.Model.Perspectives?.Select((Microsoft.AnalysisServices.Tabular.Perspective d) => d.Name)?.ToList();
			if (list2 != null)
			{
				list.AddRange(list2);
				return list;
			}
			return list;
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show(ex?.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			return list;
		}
	}

	public string GetDbmsVersion(object connection)
	{
		return CommandsWithTimeout.SSASTabular(connection).Version;
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

	public virtual string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Analysis Services (SSAS/Azure) Tabular";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		throw new NotImplementedException();
	}

	public DateTime? GetServerTime(object connection)
	{
		return DateTime.Now;
	}

	public virtual SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeSsasTabular(synchronizeParameters);
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.SsasTabular;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		string[] array = CommandsWithTimeout.SSASTabular(connection).Version.Split('.');
		if (int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2))
		{
			return new DatabaseVersionUpdate
			{
				Version = result,
				Update = result2
			};
		}
		return null;
	}

	public virtual ConnectionBase GetXmlConnectionModel()
	{
		return new SsasTabularConnection();
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
		try
		{
			Microsoft.AnalysisServices.Server server = new Microsoft.AnalysisServices.Server();
			server.Connect(connectionStringBuilder());
			return CheckIfDatabaseExists(server, connectionStringBuilder);
		}
		catch (Exception ex)
		{
			return new ConnectionResult(ex, ex.Message);
		}
	}

	private ConnectionResult CheckIfDatabaseExists(Microsoft.AnalysisServices.Server server, Func<string> connectionStringBuilder)
	{
		if (connectionStringBuilder?.Target is DatabaseRow databaseRow && !string.IsNullOrEmpty(databaseRow.Name) && server.Connected && server.Databases.FindByName(databaseRow.Name) == null)
		{
			return new ConnectionResult(new Exception(), "Unable to connect to database.\r\nCheck database name and try again.");
		}
		return new ConnectionResult(null, null, server);
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotImplementedException();
	}

	public void CloseConnection(object connection)
	{
		Microsoft.AnalysisServices.Server obj = connection as Microsoft.AnalysisServices.Server;
		obj?.Disconnect();
		obj?.Dispose();
	}
}
