using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.DB2;
using Dataedo.App.Data.General;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Base;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.ExtendedPropertiesExport;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class Db2Support : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => true;

	public bool CanCreateImportCommand => true;

	public virtual bool CanExportExtendedPropertiesOrComments => true;

	public bool CanFilterBySchema => true;

	public bool CanGetDatabases => false;

	public bool CanImportToCustomFields => false;

	public virtual string ChooseObjectPageDescriptionText => "Choose which objects' comments to export to the database.";

	public virtual string ExportToDatabaseButtonDescription => "Export comments to database";

	public string EmptyScriptMessage => "The script is not available. Db2 only stores scripts for SQL based routines. Alternatively, this could be caused by lack of privileges at the time of import.";

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool HasSslSettings => false;

	public bool IsSchemaType => true;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Db2;

	public bool ShouldForceFullReimport => false;

	public bool SupportsReadingExternalDependencies => false;

	public virtual Image TypeImage => Resources.db2;

	public virtual LoadObjectTypeEnum TypeOfExportToDatabase => LoadObjectTypeEnum.OracleComments;

	public bool CanGenerateDDLScript => true;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Db2LUW;

	public virtual SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
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
		return SharedDatabaseTypeEnum.DatabaseType.Db2LUW;
	}

	public void CloseConnection(object connection)
	{
		if (connection is System.Data.Odbc.OdbcConnection odbcConnection && odbcConnection.State != 0)
		{
			odbcConnection.Close();
			odbcConnection.Dispose();
		}
	}

	public virtual void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		OdbcCommentsExporter odbcCommentsExporter = new OdbcCommentsExporter(exportWorker, connectionString, currentDatabaseId, chosenModules, "TABLE");
		odbcCommentsExporter.GetTypeNames(chooseObjectTypesUserControl.GetIncludedDbObjects());
		odbcCommentsExporter.ExportDescription(exportDescriptions: true, owner);
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return GetConnectionString(database, host, login, password, port, identifierName, withDatabase);
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		throw new NotSupportedException();
	}

	public string GetDbmsVersion(object connection)
	{
		using (OdbcCommand odbcCommand = CommandsWithTimeout.Odbc("SELECT (INSTALLED_PROD_FULLNAME || ' ' || PROD_RELEASE) AS version FROM SYSIBMADM.ENV_PROD_INFO", connection))
		{
			using OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();
			if (odbcDataReader.Read())
			{
				return odbcDataReader["version"] as string;
			}
		}
		return null;
	}

	public List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		throw new NotSupportedException();
	}

	public virtual IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		OdbcCommentsExporter odbcCommentsExporter = new OdbcCommentsExporter(exportWorker, connectionString, currentDatabaseId, chosenModules, "TABLE");
		odbcCommentsExporter.GetTypeNames(chooseObjectTypesUserControl.GetIncludedDbObjects());
		odbcCommentsExporter.InitializeExportObjects();
		return odbcCommentsExporter.DescriptionObjects.Select((DBDescription x) => x.Command);
	}

	public FilterObjectTypeEnum.FilterObjectType[] GetFilterObjectTypes()
	{
		return new FilterObjectTypeEnum.FilterObjectType[4]
		{
			FilterObjectTypeEnum.FilterObjectType.Table,
			FilterObjectTypeEnum.FilterObjectType.View,
			FilterObjectTypeEnum.FilterObjectType.Procedure,
			FilterObjectTypeEnum.FilterObjectType.Function
		};
	}

	public string GetFilterRuleConcatenation(string schemaFieldName, string nameFieldName)
	{
		return schemaFieldName + " || '.' || " + nameFieldName;
	}

	public virtual string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "IBM Db2 LUW";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		throw new NotSupportedException();
	}

	public DateTime? GetServerTime(object connection)
	{
		using (OdbcCommand odbcCommand = CommandsWithTimeout.Odbc("select VARCHAR_FORMAT(CURRENT TIMESTAMP, 'YYYY-MM-DD HH24:MI:SS') AS date FROM SYSIBM.SYSDUMMY1", connection))
		{
			using OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();
			if (odbcDataReader.Read())
			{
				return DateTime.ParseExact(odbcDataReader["date"] as string, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
			}
		}
		return null;
	}

	public virtual SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeDb2(synchronizeParameters);
	}

	public virtual DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.Db2LUW;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		string versionString = GetVersionString(connection);
		Match match = new Regex("([0-9]+\\.[0-9]+)", RegexOptions.IgnoreCase).Match(versionString);
		if (match.Success)
		{
			string[] array = match.Value.Split('.');
			if (int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2))
			{
				return new DatabaseVersionUpdate
				{
					Version = result,
					Update = result2
				};
			}
		}
		return null;
	}

	public ConnectionBase GetXmlConnectionModel()
	{
		return new Db2Connection();
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
		return true;
	}

	public bool ShouldSynchronizeParameters(object connection)
	{
		return true;
	}

	public ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		System.Data.Odbc.OdbcConnection odbcConnection = null;
		try
		{
			odbcConnection = new System.Data.Odbc.OdbcConnection(connectionStringBuilder());
			odbcConnection.Open();
		}
		catch (OdbcException ex)
		{
			return new ConnectionResult(ex, GetExceptionMessage(ex), ex.Errors[0]?.Message);
		}
		catch (Exception ex2)
		{
			return new ConnectionResult(ex2, ex2.Message);
		}
		return new ConnectionResult(null, null, odbcConnection);
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}

	private string GetConnectionString(string database, string host, string login, string password, int? port, string driverName, bool withDatabase = true, bool setUtf8Charset = true)
	{
		if (!port.HasValue)
		{
			throw new ArgumentException("Specified port is not valid.");
		}
		OdbcConnectionStringBuilder odbcConnectionStringBuilder = new OdbcConnectionStringBuilder();
		odbcConnectionStringBuilder.Driver = driverName;
		odbcConnectionStringBuilder["Hostname"] = host;
		odbcConnectionStringBuilder["Port"] = port;
		odbcConnectionStringBuilder["Uid"] = login;
		odbcConnectionStringBuilder["Pwd"] = password;
		if (withDatabase)
		{
			odbcConnectionStringBuilder["Database"] = database;
		}
		return odbcConnectionStringBuilder.ToString();
	}

	private string GetExceptionMessage(OdbcException ex)
	{
		string text = "https://www-01.ibm.com/support/docview.wss?rs=4020&uid=swg21385217";
		if (ex.Errors[0].SQLState == "IM002")
		{
			string text2 = (Environment.Is64BitProcess ? "32" : "64");
			return "Data source name not found and no default driver specified." + Environment.NewLine + "Install IBM DB2 ODBC Driver, see <href=" + text + ">IBM Support page</href> for more information." + Environment.NewLine + "If you have installed the driver, try running Dataedo in " + text2 + "-bit mode.";
		}
		return "Unable to connect to database.";
	}

	private string GetVersionString(object connection)
	{
		using (OdbcCommand odbcCommand = CommandsWithTimeout.Odbc("select prod_release from sysibmadm.env_prod_info", connection))
		{
			using OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();
			if (odbcDataReader.Read())
			{
				return odbcDataReader[0] as string;
			}
		}
		return string.Empty;
	}
}
