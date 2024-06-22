using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.Odbc;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Base;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Data.Commands.Enums;
using Dataedo.Data.Odbc;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using Microsoft.Win32;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class OdbcSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => true;

	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => false;

	public bool CanFilterBySchema => true;

	public bool CanGetDatabases => false;

	public bool CanImportToCustomFields => false;

	public string ChooseObjectPageDescriptionText
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public string ExportToDatabaseButtonDescription
	{
		get
		{
			string text = SharedDatabaseTypeEnum.TypeToStringForDisplay(base.SupportedDatabaseType);
			string text2 = (string.IsNullOrWhiteSpace(text) ? "this database type" : text);
			return "<color=192, 192, 192>Export comments to database (not available for " + text2 + ")</color>";
		}
	}

	public string EmptyScriptMessage => "The script is not available. This could be caused by lack of privileges at the time of import.";

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool HasSslSettings => false;

	public bool IsSchemaType => true;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Odbc;

	public bool ShouldForceFullReimport => true;

	public bool SupportsReadingExternalDependencies => false;

	public Image TypeImage => Resources.odbc;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public bool CanGenerateDDLScript => true;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Odbc;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.Odbc;
	}

	public void CloseConnection(object connection)
	{
		if (connection is System.Data.Odbc.OdbcConnection odbcConnection && odbcConnection.State != 0)
		{
			odbcConnection.Close();
			odbcConnection.Dispose();
		}
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotSupportedException();
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return GetConnectionString(database, host, login, password, port, withDatabase, identifierName);
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		throw new NotSupportedException();
	}

	public string GetDbmsVersion(object connection)
	{
		System.Data.Odbc.OdbcConnection obj = connection as System.Data.Odbc.OdbcConnection;
		string driver = obj.Driver;
		string text = TryGetODBCDriverName(driver);
		string text2 = string.Empty;
		if (!string.IsNullOrWhiteSpace(text))
		{
			text2 = text;
		}
		else if (!string.IsNullOrWhiteSpace(driver))
		{
			text2 = driver;
		}
		return obj.ServerVersion + ((!string.IsNullOrWhiteSpace(text2)) ? (" (" + text2 + ")") : string.Empty);
	}

	private static string TryGetODBCDriverName(string driverDLLName)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(driverDLLName))
			{
				return string.Empty;
			}
			using RegistryKey registryKey = Registry.LocalMachine;
			string text = "SOFTWARE\\ODBC\\ODBCINST.INI";
			using RegistryKey registryKey2 = registryKey.OpenSubKey(text + "\\ODBC Drivers");
			if (registryKey2 == null)
			{
				return string.Empty;
			}
			string[] valueNames = registryKey2.GetValueNames();
			foreach (string text2 in valueNames)
			{
				using RegistryKey registryKey3 = registryKey.OpenSubKey(text + "\\" + text2);
				if (registryKey2 == null)
				{
					continue;
				}
				string text3 = registryKey3.GetValue("Driver")?.ToString();
				if (!string.IsNullOrWhiteSpace(text3))
				{
					string text4 = Path.GetFileName(text3)?.ToLower();
					if (!string.IsNullOrWhiteSpace(text4) && text4 == driverDLLName?.ToLower())
					{
						return text2;
					}
				}
			}
		}
		catch
		{
		}
		return string.Empty;
	}

	public List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		throw new NotSupportedException();
	}

	public IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		throw new NotSupportedException();
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
		return schemaFieldName + " + '.' + " + nameFieldName;
	}

	public string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "ODBC (beta version)";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		throw new NotSupportedException();
	}

	public DateTime? GetServerTime(object connection)
	{
		return DateTime.Now;
	}

	public SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeOdbc(synchronizeParameters);
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.Odbc;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		string versionString = GetVersionString(connection);
		Match match = new Regex("([0-9]+\\.[0-9]+)", RegexOptions.IgnoreCase).Match(versionString);
		if (match.Success)
		{
			string[] array = match.Groups[1].Value.Split('.');
			if (int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2))
			{
				return new DatabaseVersionUpdate
				{
					Version = result,
					Update = result2
				};
			}
		}
		match = new Regex("([0-9]+)", RegexOptions.IgnoreCase).Match(versionString);
		if (match.Success && int.TryParse(match.Groups[1].Value, out var result3))
		{
			return new DatabaseVersionUpdate
			{
				Version = result3
			};
		}
		return null;
	}

	public ConnectionBase GetXmlConnectionModel()
	{
		return new Dataedo.App.Tools.CommandLine.Xml.Connections.OdbcConnection();
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
		string text = null;
		try
		{
			text = connectionStringBuilder();
			odbcConnection = new System.Data.Odbc.OdbcConnection(text);
			odbcConnection.Open();
		}
		catch (Exception ex)
		{
			return new ConnectionResult(ex, ex.Message);
		}
		return new ConnectionResult(null, null, odbcConnection)
		{
			NewConnectionString = text
		};
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}

	private string GetConnectionString(string database, string host, string login, string password, int? port, bool withDatabase = true, string connString = null)
	{
		return new OdbcConnectionStringBuilder(Dataedo.Data.Odbc.DataSources.CompleteConnectionString(password)).ToString();
	}

	private string GetVersionString(object connection)
	{
		return (connection as System.Data.Odbc.OdbcConnection).ServerVersion;
	}
}
