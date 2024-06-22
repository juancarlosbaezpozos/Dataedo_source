using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.MySQL;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Base;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Enums;
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
using MySqlConnector;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class MySqlSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	private static readonly SharedDatabaseTypeEnum.DatabaseType[] MySQLLikeTypes = new SharedDatabaseTypeEnum.DatabaseType[8]
	{
		SharedDatabaseTypeEnum.DatabaseType.MySQL,
		SharedDatabaseTypeEnum.DatabaseType.MariaDB,
		SharedDatabaseTypeEnum.DatabaseType.Aurora,
		SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL,
		SharedDatabaseTypeEnum.DatabaseType.MySQL8,
		SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL8,
		SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMariaDB,
		SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMySQL
	};

	public virtual bool CanImportDependencies => true;

	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => false;

	public bool CanFilterBySchema => false;

	public bool CanGetDatabases => true;

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

	public virtual bool HasSslSettings => true;

	public bool IsSchemaType => false;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.MySQL;

	public bool ShouldForceFullReimport => true;

	public bool SupportsReadingExternalDependencies => false;

	public virtual Image TypeImage => Resources.mysql;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public bool CanGenerateDDLScript => true;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.MySQL;

	public virtual SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		SharedDatabaseTypeEnum.DatabaseType? forkType = GetForkType(connection);
		SharedDatabaseTypeEnum.DatabaseType databaseType = SharedDatabaseTypeEnum.DatabaseType.MySQL;
		if (!forkType.HasValue || forkType.Value != databaseType)
		{
			return GeneralQueries.AskUserWhichConnectorUse(forkType, databaseType, connection, owner);
		}
		if (version < base.VersionInfo.FirstSupportedVersion)
		{
			GeneralMessageBoxesHandling.Show(GetNotSupportedText(), "Unsupported version", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			return null;
		}
		if (version >= base.VersionInfo.FirstNotSupportedVersion || (version.Version > 5 && version.Version < 8))
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(GetNotSupportedText(withQuestion: true), "Unsupported version", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 1, owner);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
			{
				return null;
			}
		}
		return SharedDatabaseTypeEnum.DatabaseType.MySQL;
	}

	protected override string GetSupportedVersionsText()
	{
		return $"5.1 to 5.7 and from 8.0.0 before {base.VersionInfo.FirstNotSupportedVersion}";
	}

	public void CloseConnection(object connection)
	{
		if (connection is MySqlConnection mySqlConnection && mySqlConnection.State != 0)
		{
			mySqlConnection.Close();
			mySqlConnection.Dispose();
		}
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotSupportedException();
	}

	public virtual string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string sslType = null, string sslKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return GetConnectionString(database, host, login, password, port, sslType, withDatabase, setUtf8Charset: true, keyPath, certPath, caPath, cipher, useStandardConnectionString);
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
			Exception ex = CheckSslConfiguration(connectionString);
			if (ex != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
				GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
				return null;
			}
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				mySqlConnection.Open();
				using MySqlCommand mySqlCommand = CommandsWithTimeout.MySQL("SELECT SCHEMA_NAME as name\r\n                        FROM INFORMATION_SCHEMA.SCHEMATA\r\n                        WHERE SCHEMA_NAME NOT IN \r\n                            ('information_schema', 'mysql', 'performance_schema') -- to hide system databases\r\n                        ORDER BY SCHEMA_NAME", mySqlConnection);
				using MySqlDataReader mySqlDataReader = mySqlCommand?.ExecuteReader();
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					if (mySqlDataReader[0] != null && !(mySqlDataReader[0] is DBNull))
					{
						list.Add(mySqlDataReader[0]?.ToString());
					}
				}
			}
			return list;
		}
		catch (MySqlException ex2)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			GeneralMessageBoxesHandling.Show(GetExceptionMessage(ex2), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		catch (Exception ex3)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			GeneralMessageBoxesHandling.Show("Unable to connect to server." + Environment.NewLine + Environment.NewLine + ex3.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		return null;
	}

	public string GetDbmsVersion(object connection)
	{
		return GetMySQLDbmsVersion(connection);
	}

	public static string GetMySQLDbmsVersion(object connection)
	{
		return GetDbmsVersion(connection, "select concat(concat(@@version_comment, ' '),@@version) as version");
	}

	public static string GetDbmsVersion(object connection, string command)
	{
		try
		{
			using MySqlCommand mySqlCommand = CommandsWithTimeout.MySQL(command, connection);
			using MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
			if (mySqlDataReader.Read())
			{
				return mySqlDataReader["version"] as string;
			}
		}
		catch (Exception ex)
		{
			if (!ex.Message.Contains("version_comment"))
			{
				throw;
			}
		}
		return null;
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
		return "CONCAT(" + schemaFieldName + ", '.', " + nameFieldName + ")";
	}

	public virtual string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "MySQL";
	}

	public virtual string GetQueryForDataLength(string tableAlias)
	{
		return "case\r\n                    when upper(data_type) in ('CHAR','VARCHAR') then (\r\n                        case \r\n                            when character_maximum_length =-1 then 'max'\r\n                            else cast(character_maximum_length as char(8))\r\n                        end)\r\n                    when upper(data_type) in \r\n                            ('TINYINT', 'SMALLINT', 'MEDIUMINT', 'INT', 'BIGINT', 'FLOAT', 'DOUBLE', 'DECIMAL', 'BIT') \r\n                        then concat(cast(numeric_precision as char(4)), \r\n                    case \r\n                        when numeric_scale is null then ''\r\n                        else concat(', ', cast(numeric_scale as char(4)))\r\n                    end)\r\n                    end as `data_length`";
	}

	public DateTime? GetServerTime(object connection)
	{
		return GetServerTimeMySQL(connection);
	}

	public static DateTime? GetServerTimeMySQL(object connection)
	{
		using (MySqlCommand mySqlCommand = CommandsWithTimeout.MySQL("select date_format(now(), '%Y-%m-%d %H:%i:%s') as date", connection))
		{
			using MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
			if (mySqlDataReader.Read())
			{
				return DateTime.ParseExact(mySqlDataReader["date"] as string, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
			}
		}
		return null;
	}

	public virtual SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeMySQL(synchronizeParameters);
	}

	public virtual DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.MySql;
	}

	public virtual DatabaseVersionUpdate GetVersion(object connection)
	{
		return GetVersionMySQL(connection);
	}

	public static DatabaseVersionUpdate GetVersionMySQL(object connection)
	{
		string versionString = GetVersionString(connection);
		Match match = new Regex("([0-9]+\\.[0-9]+\\.[0-9]+)", RegexOptions.IgnoreCase).Match(versionString);
		if (match.Success)
		{
			string[] array = match.Value.Split('.');
			if (int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2) && int.TryParse(array[2], out var result3))
			{
				return new DatabaseVersionUpdate
				{
					Version = result,
					Update = result2,
					Build = result3
				};
			}
		}
		return null;
	}

	public virtual ConnectionBase GetXmlConnectionModel()
	{
		return new MySQLConnection();
	}

	public bool PrepareElements(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl, Form owner = null)
	{
		return true;
	}

	public void ProcessException(Exception ex, string name, string warehouse, Form owner = null)
	{
		if (ex is MySqlException ex2)
		{
			GeneralMessageBoxesHandling.Show(GetExceptionMessage(ex2), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		else
		{
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
	}

	public virtual bool ShouldSynchronizeComputedFormula(object connection)
	{
		DatabaseVersionUpdate version = GetVersion(connection);
		if (version.Version != 8 && version.Version <= 5)
		{
			if (version.Version == 5)
			{
				return version.Update >= 7;
			}
			return false;
		}
		return true;
	}

	public virtual bool ShouldSynchronizeParameters(object connection)
	{
		DatabaseVersionUpdate version = GetVersion(connection);
		if (version.Version <= 5)
		{
			if (version.Version == 5)
			{
				return version.Update >= 5;
			}
			return false;
		}
		return true;
	}

	public ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		return TryConnection(connectionStringBuilder);
	}

	public static ConnectionResult TryConnection(Func<string> connectionStringBuilder)
	{
		MySqlConnection mySqlConnection = null;
		try
		{
			Exception ex = CheckSslConfiguration(connectionStringBuilder());
			if (ex != null)
			{
				return new ConnectionResult(ex, ex.Message);
			}
			mySqlConnection = new MySqlConnection(connectionStringBuilder());
			mySqlConnection.Open();
		}
		catch (MySqlException ex2)
		{
			return new ConnectionResult(ex2, GetExceptionMessage(ex2));
		}
		catch (Exception ex3)
		{
			return new ConnectionResult(ex3, ex3.Message);
		}
		return new ConnectionResult(null, null, mySqlConnection);
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}

	public static string GetExceptionMessage(MySqlException ex)
	{
		switch (ex.ErrorCode)
		{
		case MySqlErrorCode.None:
		case MySqlErrorCode.UnableToConnectToHost:
		case MySqlErrorCode.HandshakeError:
			return "Cannot connect to server." + Environment.NewLine + ex.Message;
		case MySqlErrorCode.DatabaseAccessDenied:
		case MySqlErrorCode.AccessDenied:
			return "Access denied. Make sure username and password are correct and SSL is configured.";
		default:
			return ex.Message;
		}
	}

	protected virtual SharedDatabaseTypeEnum.DatabaseType? GetForkType(object connection)
	{
		try
		{
			using (MySqlCommand mySqlCommand = CommandsWithTimeout.MySQL("SELECT @@aurora_version;", connection))
			{
				using (mySqlCommand.ExecuteReader())
				{
				}
			}
			return SharedDatabaseTypeEnum.DatabaseType.Aurora;
		}
		catch (Exception)
		{
		}
		try
		{
			using MySqlCommand mySqlCommand2 = CommandsWithTimeout.MySQL("select concat(@@version, @@version_comment) as Value", connection);
			using MySqlDataReader mySqlDataReader2 = mySqlCommand2.ExecuteReader();
			if (mySqlDataReader2.Read() && (mySqlDataReader2["Value"]?.ToString()?.ToLower()).Contains("percona"))
			{
				return SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL;
			}
		}
		catch (Exception)
		{
		}
		try
		{
			using MySqlCommand mySqlCommand3 = CommandsWithTimeout.MySQL("SHOW VARIABLES LIKE 'version_comment';", connection);
			using MySqlDataReader mySqlDataReader3 = mySqlCommand3.ExecuteReader();
			if (mySqlDataReader3.Read())
			{
				string text = mySqlDataReader3["Value"]?.ToString()?.ToLower();
				if (text.Contains("mysql community server - gpl"))
				{
					return SharedDatabaseTypeEnum.DatabaseType.MySQL;
				}
				SharedDatabaseTypeEnum.DatabaseType[] mySQLLikeTypes = MySQLLikeTypes;
				foreach (SharedDatabaseTypeEnum.DatabaseType? databaseType in mySQLLikeTypes)
				{
					string mySQLForkLowerName = GetMySQLForkLowerName(databaseType);
					if (text.Contains(mySQLForkLowerName))
					{
						return databaseType;
					}
				}
			}
		}
		catch (Exception)
		{
		}
		return null;
	}

	public static string GetVersionString(object connection)
	{
		using (MySqlCommand mySqlCommand = CommandsWithTimeout.MySQL("select version();", connection))
		{
			using MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
			if (mySqlDataReader.Read())
			{
				return mySqlDataReader[0] as string;
			}
		}
		return string.Empty;
	}

	private string GetMySQLForkLowerName(SharedDatabaseTypeEnum.DatabaseType? databaseType)
	{
		return databaseType switch
		{
			SharedDatabaseTypeEnum.DatabaseType.MySQL => "mysql", 
			SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL => "percona", 
			SharedDatabaseTypeEnum.DatabaseType.MariaDB => "mariadb", 
			SharedDatabaseTypeEnum.DatabaseType.Aurora => "aurora", 
			SharedDatabaseTypeEnum.DatabaseType.MySQL8 => "mysql8", 
			SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL8 => "percona8", 
			SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMariaDB => "hivemetastoremariadb", 
			SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMySQL => "hivemetastoremysql", 
			_ => null, 
		};
	}

	public static string GetConnectionString(string database, string host, string login, string password, int? port, string sslType, bool withDatabase = true, bool setUtf8Charset = true, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false)
	{
		if (!port.HasValue)
		{
			throw new ArgumentException("Specified port is not valid.");
		}
		string characterSet = (setUtf8Charset ? "utf8" : string.Empty);
		string tlsCipherSuites = (string.IsNullOrEmpty(cipher) ? string.Empty : cipher);
		string text = (string.IsNullOrEmpty(keyPath) ? string.Empty : keyPath);
		string text2 = (string.IsNullOrEmpty(certPath) ? string.Empty : certPath);
		string text3 = (string.IsNullOrEmpty(caPath) ? string.Empty : caPath);
		MySqlConnectionStringBuilder mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder
		{
			UserID = login,
			Password = password,
			Server = host,
			Pooling = false,
			CharacterSet = characterSet,
			TlsCipherSuites = tlsCipherSuites,
			SslMode = GetMySqlSslMode(sslType)
		};
		if (!string.IsNullOrEmpty(text))
		{
			mySqlConnectionStringBuilder.SslKey = text;
		}
		if (!string.IsNullOrEmpty(text2))
		{
			mySqlConnectionStringBuilder.SslCert = text2;
		}
		if (!string.IsNullOrEmpty(text3))
		{
			mySqlConnectionStringBuilder.SslCa = text3;
		}
		if (port.HasValue)
		{
			mySqlConnectionStringBuilder.Port = (uint)port.Value;
		}
		if (withDatabase)
		{
			mySqlConnectionStringBuilder.Database = database;
		}
		return mySqlConnectionStringBuilder.ToString();
	}

	private static MySqlSslMode GetMySqlSslMode(string sslType)
	{
		return SSLTypeEnum.StringToType(sslType) switch
		{
			SSLTypeEnum.SSLType.Prefer => MySqlSslMode.Preferred, 
			SSLTypeEnum.SSLType.Require => MySqlSslMode.Required, 
			SSLTypeEnum.SSLType.VerifyCA => MySqlSslMode.VerifyCA, 
			SSLTypeEnum.SSLType.VerifyFull => MySqlSslMode.VerifyFull, 
			SSLTypeEnum.SSLType.Disable => MySqlSslMode.None, 
			_ => MySqlSslMode.Preferred, 
		};
	}

	public static bool IsDatabaseTypeMySqlFork(SharedDatabaseTypeEnum.DatabaseType? databaseType)
	{
		if (!databaseType.HasValue)
		{
			return false;
		}
		return MySQLLikeTypes.Any((SharedDatabaseTypeEnum.DatabaseType d) => d == databaseType);
	}

	public static Exception CheckSslConfiguration(string connectionString)
	{
		try
		{
			MySqlConnectionStringBuilder mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrWhiteSpace(mySqlConnectionStringBuilder.SslKey) && !File.Exists(mySqlConnectionStringBuilder.SslKey))
			{
				stringBuilder.AppendLine("Client Private Key file does not exist.");
			}
			if (!string.IsNullOrWhiteSpace(mySqlConnectionStringBuilder.SslCert) && !File.Exists(mySqlConnectionStringBuilder.SslCert))
			{
				stringBuilder.AppendLine("Client Certificate file does not exist.");
			}
			if (!string.IsNullOrWhiteSpace(mySqlConnectionStringBuilder.SslCa) && !File.Exists(mySqlConnectionStringBuilder.SslCa))
			{
				stringBuilder.AppendLine("CA Certificate file does not exist.");
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Insert(0, Environment.NewLine);
				stringBuilder.Insert(0, Environment.NewLine);
				stringBuilder.Insert(0, "Please check the SSL configuration:");
				return new Exception(stringBuilder.ToString());
			}
		}
		catch (Exception result)
		{
			return result;
		}
		return null;
	}
}
