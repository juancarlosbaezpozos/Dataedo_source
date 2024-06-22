using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.SqlServer;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Base;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Enums;
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
using Microsoft.Data.SqlClient;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class SqlServerSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public virtual bool CanImportDependencies => true;

	public virtual bool CanCreateImportCommand => true;

	public virtual bool CanExportExtendedPropertiesOrComments => true;

	public bool CanFilterBySchema => true;

	public bool CanGetDatabases => true;

	public bool CanImportToCustomFields => true;

	public string ChooseObjectPageDescriptionText => "Choose which objects' extended properties to export to the database.";

	public virtual string ExportToDatabaseButtonDescription => "Export descriptions/custom fields to database (extended properties)";

	public string EmptyScriptMessage => "The script is not available. This could be caused by lack of privileges at the time of import.";

	public bool HasExtendedPropertiesExport => true;

	public bool HasImportUsingCustomFields => true;

	public bool HasSslSettings => false;

	public bool IsSchemaType => true;

	public virtual PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.SqlServer;

	public virtual bool ShouldForceFullReimport => false;

	public bool SupportsReadingExternalDependencies => true;

	public virtual Image TypeImage => Resources.sqlserver;

	public LoadObjectTypeEnum TypeOfExportToDatabase => LoadObjectTypeEnum.ExtendedPropertiesObjects;

	public bool CanGenerateDDLScript => true;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.SqlServer;

	public virtual SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		SharedDatabaseTypeEnum.DatabaseType? forkType = GetForkType(connection);
		SharedDatabaseTypeEnum.DatabaseType databaseType = SharedDatabaseTypeEnum.DatabaseType.SqlServer;
		if (!forkType.HasValue || forkType.Value != databaseType)
		{
			return GeneralQueries.AskUserWhichConnectorUse(forkType, databaseType, connection, owner);
		}
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
		return SharedDatabaseTypeEnum.DatabaseType.SqlServer;
	}

	protected override string GetSupportedVersionsText()
	{
		return "2005 SP2 to 2019";
	}

	public void CloseConnection(object connection)
	{
		if (connection is SqlConnection sqlConnection && sqlConnection.State != 0)
		{
			sqlConnection.Close();
			sqlConnection.Dispose();
		}
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		ExtendedPropertiesExporter extendedPropertiesExporter = new ExtendedPropertiesExporter(exportWorker, connectionString, currentDatabaseId, chosenModules, chooseCustomFieldsUserControl.CustomFieldsSupport.DocumentationCustomFields.Where((DocumentationCustomFieldRow x) => x.IsSelected).ToList());
		extendedPropertiesExporter.GetTypeNames(chooseObjectTypesUserControl.GetIncludedDbObjects());
		extendedPropertiesExporter.ExportDescription(chooseCustomFieldsUserControl.ExtendedPropertiesExportDataSource.FirstOrDefault((IExtendedProperty x) => x.Title.Equals(chooseCustomFieldsUserControl.Description)).IsSelected, owner);
	}

	public virtual string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		SqlServerConnectionModeEnum.SqlServerConnectionMode connectionMode = SqlServerConnectionModeEnum.StringToTypeOrDefault(param1);
		return GetConnectionString(applicationName, database, host, login, password, port, authenticationType, connectionMode, withPooling, withDatabase);
	}

	public virtual List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		return GetDatabases(connectionString, splashScreenManager, owner);
	}

	public static List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, Form owner = null)
	{
		try
		{
			List<string> list = new List<string>();
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				using SqlCommand sqlCommand = CommandsWithTimeout.SqlServer("SELECT name\r\n                    FROM master.dbo.sysdatabases\r\n                    WHERE dbid > 4 -- to hide system databases\r\n                    ORDER BY name", sqlConnection);
				using SqlDataReader sqlDataReader = sqlCommand?.ExecuteReader();
				while (sqlDataReader != null && sqlDataReader.Read())
				{
					if (sqlDataReader[0] != null && !(sqlDataReader[0] is DBNull))
					{
						list.Add(sqlDataReader[0]?.ToString());
					}
				}
			}
			return list;
		}
		catch (SqlException ex)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			string title = "Error";
			MessageBoxIcon messageBoxIcon = MessageBoxIcon.Hand;
			if (ex.Number == 15871)
			{
				title = "Information";
				messageBoxIcon = MessageBoxIcon.Asterisk;
			}
			GeneralMessageBoxesHandling.Show(GetExceptionMessage(ex), title, MessageBoxButtons.OK, messageBoxIcon, null, 1, owner);
		}
		catch (Exception ex2)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			GeneralMessageBoxesHandling.Show("Unable to connect to server." + Environment.NewLine + Environment.NewLine + ex2.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		return null;
	}

	public string GetDbmsVersion(object connection)
	{
		return GetSQLServerDbmsVersion(connection);
	}

	public static string GetSQLServerDbmsVersion(object connection)
	{
		return GetDbmsVersion(connection, "Select @@version as version");
	}

	public static string GetDbmsVersion(object connection, string command)
	{
		using (SqlCommand sqlCommand = CommandsWithTimeout.SqlServer(command, connection))
		{
			using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
			if (sqlDataReader.Read())
			{
				return sqlDataReader["version"] as string;
			}
		}
		return null;
	}

	public virtual List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		List<string> list = new List<string>();
		using SqlCommand sqlCommand = new SqlCommand("SELECT distinct name FROM sys.extended_properties", connection as SqlConnection);
		using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
		while (sqlDataReader.Read())
		{
			list.Add(sqlDataReader["name"]?.ToString());
		}
		return list;
	}

	public IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		ExtendedPropertiesExporter extendedPropertiesExporter = new ExtendedPropertiesExporter(exportWorker, connectionString, currentDatabaseId, chosenModules, chooseCustomFieldsUserControl.CustomFieldsSupport.DocumentationCustomFields.Where((DocumentationCustomFieldRow x) => x.IsSelected).ToList());
		extendedPropertiesExporter.GetTypeNames(chooseObjectTypesUserControl.GetIncludedDbObjects());
		extendedPropertiesExporter.InitializeExportObjects(chooseCustomFieldsUserControl.ExtendedPropertiesExportDataSource.FirstOrDefault((IExtendedProperty x) => x.Title.Equals(chooseCustomFieldsUserControl.Description)).IsSelected);
		return extendedPropertiesExporter.DescriptionObjects.Select((DBDescription x) => x.Command);
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

	public virtual string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "SQL Server";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		return "CASE\r\n                    WHEN data_type = 'binary'\r\n                         OR data_type = 'char'\r\n                         OR data_type = 'nchar'\r\n                         OR data_type = 'varchar'\r\n                         OR data_type = 'nvarchar'\r\n                         OR data_type = 'varbinary'\r\n                    THEN CASE CHARACTER_MAXIMUM_LENGTH\r\n                             WHEN-1\r\n                             THEN 'MAX'\r\n                             ELSE CAST(CHARACTER_MAXIMUM_LENGTH AS varchar(4))\r\n                         END\r\n                    WHEN data_type = 'datetime2'\r\n                         OR data_type = 'datetimeoffset'\r\n                         OR data_type = 'time'\r\n                    THEN CAST(DATETIME_PRECISION AS varchar(4))\r\n                    WHEN data_type = 'decimal'\r\n                         OR data_type = 'numeric'\r\n                    THEN CAST(NUMERIC_PRECISION AS varchar(4)) + ', ' + CAST(NUMERIC_SCALE AS varchar(4))\r\n                END data_length ";
	}

	public DateTime? GetServerTime(object connection)
	{
		return GetServerTimeSQLServer(connection);
	}

	public static DateTime? GetServerTimeSQLServer(object connection)
	{
		using (SqlCommand sqlCommand = CommandsWithTimeout.SqlServer("SELECT CONVERT(VARCHAR(19), GETDATE(), 120) as date", connection))
		{
			using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
			if (sqlDataReader.Read())
			{
				return DateTime.ParseExact(sqlDataReader["date"] as string, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
			}
		}
		return null;
	}

	public virtual SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeDB(synchronizeParameters);
	}

	public virtual DatabaseType GetTypeForCommands(bool isFile)
	{
		if (!isFile)
		{
			return Dataedo.Data.Commands.Enums.DatabaseType.SqlServer;
		}
		return Dataedo.Data.Commands.Enums.DatabaseType.SqlServerCe;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		string[] array = GetVersionString(connection)?.Split('.');
		if (array != null && int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2) && int.TryParse(array[2], out var result3))
		{
			return new DatabaseVersionUpdate
			{
				Version = result,
				Update = result2,
				Build = result3
			};
		}
		return null;
	}

	public virtual ConnectionBase GetXmlConnectionModel()
	{
		return new SqlServerConnection();
	}

	public bool PrepareElements(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl, Form owner = null)
	{
		return true;
	}

	public void ProcessException(Exception ex, string name, string warehouse, Form owner = null)
	{
		if (ex is SqlException exception)
		{
			GeneralMessageBoxesHandling.Show(GetExceptionMessage(exception), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		else
		{
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
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
		return TryConnection(connectionStringBuilder);
	}

	public static ConnectionResult TryConnection(Func<string> connectionStringBuilder)
	{
		SqlConnection sqlConnection;
		try
		{
			sqlConnection = new SqlConnection(connectionStringBuilder());
			sqlConnection.Open();
		}
		catch (SqlException exception)
		{
			return new ConnectionResult(exception, GetExceptionMessage(exception));
		}
		catch (Exception ex)
		{
			return new ConnectionResult(ex, ex.Message);
		}
		return new ConnectionResult(null, null, sqlConnection);
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}

	protected SharedDatabaseTypeEnum.DatabaseType? GetForkType(object connection)
	{
		try
		{
			using SqlCommand sqlCommand = CommandsWithTimeout.SqlServer("SELECT @@version;", connection);
			using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
			if (sqlDataReader.Read())
			{
				string text = sqlDataReader.GetValue(0)?.ToString();
				if (text.Contains("Azure SQL Data Warehouse"))
				{
					return SharedDatabaseTypeEnum.DatabaseType.AzureSQLDataWarehouse;
				}
				if (text.Contains("Microsoft SQL Server"))
				{
					return SharedDatabaseTypeEnum.DatabaseType.SqlServer;
				}
			}
		}
		catch (Exception)
		{
		}
		try
		{
			using SqlCommand sqlCommand2 = CommandsWithTimeout.SqlServer("SELECT SERVERPROPERTY('Edition') AS edition;", connection);
			using SqlDataReader sqlDataReader2 = sqlCommand2.ExecuteReader();
			if (sqlDataReader2.Read())
			{
				string value = sqlDataReader2.GetValue(0)?.ToString();
				if ("SQL Azure".Equals(value))
				{
					return SharedDatabaseTypeEnum.DatabaseType.AzureSQLDatabase;
				}
			}
		}
		catch (Exception)
		{
		}
		return null;
	}

	public static string GetConnectionString(string applicationName, string database, string host, string login, string password, int? port, AuthenticationType.AuthenticationTypeEnum authenticationType, SqlServerConnectionModeEnum.SqlServerConnectionMode connectionMode, bool withPooling, bool withDatabase = true)
	{
		if (withDatabase)
		{
			ThrowNotValidExceptionIfNull(database, "database");
		}
		ThrowNotValidExceptionIfNull(host, "host");
		SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
		sqlConnectionStringBuilder.ApplicationName = applicationName;
		if (withDatabase)
		{
			sqlConnectionStringBuilder.InitialCatalog = database;
		}
		if (!port.HasValue || port == 1433)
		{
			sqlConnectionStringBuilder.DataSource = host;
		}
		else
		{
			sqlConnectionStringBuilder.DataSource = $"{host},{port}";
		}
		switch (authenticationType)
		{
		case AuthenticationType.AuthenticationTypeEnum.WindowsAuthentication:
			sqlConnectionStringBuilder.IntegratedSecurity = true;
			break;
		case AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryIntegrated:
			sqlConnectionStringBuilder.Authentication = SqlAuthenticationMethod.ActiveDirectoryIntegrated;
			break;
		case AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryPassword:
			ThrowNotValidExceptionIfNull(login, "login");
			ThrowNotValidExceptionIfNull(password, "password");
			sqlConnectionStringBuilder.Authentication = SqlAuthenticationMethod.ActiveDirectoryPassword;
			sqlConnectionStringBuilder.UserID = login;
			sqlConnectionStringBuilder.Password = password;
			break;
		case AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryInteractive:
			sqlConnectionStringBuilder.Authentication = SqlAuthenticationMethod.ActiveDirectoryInteractive;
			if (!string.IsNullOrEmpty(login))
			{
				sqlConnectionStringBuilder.UserID = login;
			}
			break;
		default:
			ThrowNotValidExceptionIfNull(login, "login");
			ThrowNotValidExceptionIfNull(password, "password");
			sqlConnectionStringBuilder.UserID = login;
			sqlConnectionStringBuilder.Password = password;
			break;
		}
		sqlConnectionStringBuilder.FailoverPartner = "";
		sqlConnectionStringBuilder.Pooling = withPooling;
		switch (connectionMode)
		{
		case SqlServerConnectionModeEnum.SqlServerConnectionMode.ForceEncryptionRequireTrustedCertificate:
			sqlConnectionStringBuilder.Encrypt = true;
			sqlConnectionStringBuilder.TrustServerCertificate = false;
			break;
		case SqlServerConnectionModeEnum.SqlServerConnectionMode.ForceEncryptionTrustServerCertificate:
			sqlConnectionStringBuilder.Encrypt = true;
			sqlConnectionStringBuilder.TrustServerCertificate = true;
			break;
		case SqlServerConnectionModeEnum.SqlServerConnectionMode.EncryptConnectionIfPossible:
			sqlConnectionStringBuilder.Encrypt = false;
			sqlConnectionStringBuilder.TrustServerCertificate = true;
			break;
		default:
			sqlConnectionStringBuilder.Encrypt = false;
			sqlConnectionStringBuilder.TrustServerCertificate = false;
			break;
		}
		return sqlConnectionStringBuilder?.ConnectionString;
	}

	public static string GetExceptionMessage(SqlException exception)
	{
		int? num = exception.Errors[0].Number;
		string message = exception.Errors[0].Message;
		if (message != null)
		{
			string text = "<href=" + Links.ConfigureSQLServerSSLCertificate + ">this guide</href>";
			if (message.Contains("The certificate chain was issued by an authority that is not trusted"))
			{
				return "Connection to the database could not be encrypted. The SSL certificate was not trusted." + Environment.NewLine + Environment.NewLine + "Check " + text + " for information on this issue and possible solutions.";
			}
			if (message.Contains("The target principal name is incorrect"))
			{
				return "Connection to the database could not be encrypted. The target principal name is incorrect." + Environment.NewLine + Environment.NewLine + "If you’re using an encrypted connection, make sure the certificate includes the same server name as the one used during the connection. Check " + text + " for information on this issue and possible solutions.";
			}
			if (!string.IsNullOrEmpty(exception.Procedure) && exception.Procedure.Equals("AcquireToken") && message.Contains("Invalid URI: The hostname could not be parsed."))
			{
				return "Unable to connect to the database." + Environment.NewLine + "Make sure a hostname is correct and authentication is possible with selected method.";
			}
		}
		switch (num)
		{
		case 53:
			return "Unable to connect to database." + Environment.NewLine + "Check server name and try again.";
		case 4060:
			return "Unable to connect to database." + Environment.NewLine + "Check database name and try again.";
		case 18452:
			return "Unable to connect to database." + Environment.NewLine + "Cannot connect using windows credentials.";
		case 229:
		case 18456:
			return "Unable to connect to database." + Environment.NewLine + "Check login and password and try again.";
		case 15871:
			return "Dataedo cannot retrieve a database list from the Azure Synapse Serverless." + Environment.NewLine + "Please type a database name manually.";
		default:
			return message;
		}
	}

	public static string GetVersionString(object connection)
	{
		using (SqlCommand sqlCommand = CommandsWithTimeout.SqlServer("SELECT SERVERPROPERTY('productversion') AS version", connection))
		{
			using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
			if (sqlDataReader.Read())
			{
				return sqlDataReader["version"] as string;
			}
		}
		return string.Empty;
	}

	protected static void ThrowNotValidExceptionIfNull<T>(T value, string name)
	{
		if (value == null)
		{
			throw new ArgumentException("Specified " + name + " is not valid.");
		}
	}
}
