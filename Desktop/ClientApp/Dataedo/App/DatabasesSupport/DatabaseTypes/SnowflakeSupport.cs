using System;
using System.Collections.Generic;
using System.Data;
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
using Dataedo.App.Data.Snowflake;
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
using Snowflake.Data.Client;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class SnowflakeSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => true;

	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => true;

	public bool CanFilterBySchema => true;

	public bool CanGetDatabases => true;

	public bool CanImportToCustomFields => false;

	public string ChooseObjectPageDescriptionText => "Choose which objects' comments to export to the database.";

	public string ExportToDatabaseButtonDescription => "Export comments to database";

	public string EmptyScriptMessage => "The script is not available. This could be caused by lack of privileges at the time of import.";

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool HasSslSettings => false;

	public bool IsSchemaType => true;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Snowflake;

	public bool ShouldForceFullReimport => false;

	public bool SupportsReadingExternalDependencies => false;

	public Image TypeImage => Resources.snowflake;

	public LoadObjectTypeEnum TypeOfExportToDatabase => LoadObjectTypeEnum.SnowflakeComments;

	public bool CanGenerateDDLScript => true;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Snowflake;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.Snowflake;
	}

	public void CloseConnection(object connection)
	{
		if (connection is SnowflakeDbConnection snowflakeDbConnection && snowflakeDbConnection.State != 0)
		{
			snowflakeDbConnection.Close();
			snowflakeDbConnection.Dispose();
		}
	}

	public virtual void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		SnowflakeCommentsExporter snowflakeCommentsExporter = new SnowflakeCommentsExporter(exportWorker, connectionString, currentDatabaseId, chosenModules, new SnowflakeCommentsExceptionHandler());
		snowflakeCommentsExporter.GetTypeNames(chooseObjectTypesUserControl.GetIncludedDbObjects());
		snowflakeCommentsExporter.ExportDescription(exportDescriptions: true, owner);
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return GetConnectionString(database, host, login, password, port, withDatabase, identifierName, SSLType, SSLKeyPath, connectionRole);
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		try
		{
			List<string> list = new List<string>();
			using (SnowflakeDbConnection snowflakeDbConnection = new SnowflakeDbConnection())
			{
				SnowflakeDbConnectionStringBuilder snowflakeDbConnectionStringBuilder = new SnowflakeDbConnectionStringBuilder
				{
					ConnectionString = connectionString
				};
				if (snowflakeDbConnectionStringBuilder.ContainsKey("db"))
				{
					snowflakeDbConnectionStringBuilder.Remove("db");
				}
				snowflakeDbConnection.ConnectionString = snowflakeDbConnectionStringBuilder.ConnectionString;
				snowflakeDbConnection.Open();
				using IDbCommand dbCommand = CommandsWithTimeout.Snowflake("SHOW DATABASES;", snowflakeDbConnection);
				using IDataReader dataReader = dbCommand?.ExecuteReader();
				while (dataReader != null && dataReader.Read())
				{
					int ordinal = dataReader.GetOrdinal("name");
					if (dataReader[ordinal] != null && !(dataReader[ordinal] is DBNull))
					{
						list.Add(dataReader[ordinal]?.ToString());
					}
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			GeneralMessageBoxesHandling.Show("Unable to connect to server." + Environment.NewLine + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		return null;
	}

	public string GetDbmsVersion(object connection)
	{
		using (IDbCommand dbCommand = CommandsWithTimeout.Snowflake("select 'Snowflake ' || CURRENT_VERSION();", connection))
		{
			using IDataReader dataReader = dbCommand.ExecuteReader();
			if (dataReader.Read())
			{
				return dataReader[0] as string;
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
		SnowflakeCommentsExporter snowflakeCommentsExporter = new SnowflakeCommentsExporter(exportWorker, connectionString, currentDatabaseId, chosenModules, new SnowflakeCommentsExceptionHandler());
		snowflakeCommentsExporter.GetTypeNames(chooseObjectTypesUserControl.GetIncludedDbObjects());
		snowflakeCommentsExporter.InitializeExportObjects();
		return snowflakeCommentsExporter.DescriptionObjects.Select((DBDescription x) => x.Command);
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

	public string GetFriendlyDisplayNameForImport(bool isLite)
	{
		if (!isLite)
		{
			return "Snowflake";
		}
		return "Snowflake (Pro)";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		return "case when c.data_type='TEXT' \r\n                    then to_char(c.character_maximum_length)\r\n                when c.data_type='NUMBER' \r\n                    then concat(concat(to_char(c.numeric_precision), ', '), to_char(c.numeric_scale))\r\n                else '' END as \"data_length\" ";
	}

	public DateTime? GetServerTime(object connection)
	{
		using (IDbCommand dbCommand = CommandsWithTimeout.Snowflake("SELECT TO_CHAR(current_timestamp, 'YYYY-MM-DD HH24:MI:SS')", connection))
		{
			using IDataReader dataReader = dbCommand.ExecuteReader();
			if (dataReader.Read())
			{
				return DateTime.ParseExact(dataReader[0] as string, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
			}
		}
		return null;
	}

	public SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeSnowflake(synchronizeParameters);
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.Snowflake;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		string versionString = GetVersionString(connection);
		Match match = new Regex("([0-9]+\\.[0-9]+)\\..+", RegexOptions.IgnoreCase).Match(versionString);
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
		return new SnowflakeConnection();
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
		return true;
	}

	public ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		SnowflakeDbConnection snowflakeDbConnection = new SnowflakeDbConnection();
		try
		{
			SnowflakeDbConnectionStringBuilder snowflakeDbConnectionStringBuilder = new SnowflakeDbConnectionStringBuilder
			{
				ConnectionString = connectionStringBuilder()
			};
			if (useOnlyRequiredFields)
			{
				if (snowflakeDbConnectionStringBuilder.ContainsKey("warehouse"))
				{
					snowflakeDbConnectionStringBuilder.Remove("warehouse");
				}
				if (snowflakeDbConnectionStringBuilder.ContainsKey("db"))
				{
					snowflakeDbConnectionStringBuilder.Remove("db");
				}
			}
			snowflakeDbConnection.ConnectionString = RebuildSnowflakeConnectionString(snowflakeDbConnectionStringBuilder);
			snowflakeDbConnection.Open();
			using IDbCommand dbCommand = CommandsWithTimeout.Snowflake("SHOW WAREHOUSES", snowflakeDbConnection);
			dbCommand.ExecuteReader();
		}
		catch (SnowflakeDbException ex)
		{
			if (ex.ErrorCode == 270008)
			{
				return new ConnectionResult(ex, "Please provide the private key or path to a private key.");
			}
			if (ex.ErrorCode == 270052)
			{
				return new ConnectionResult(ex, "Invalid key, pass phrase or wrong username was provided.");
			}
			return new ConnectionResult(ex, ex.Message);
		}
		catch (AggregateException ex2)
		{
			return new ConnectionResult(ex2, ex2.InnerException.Message);
		}
		catch (Exception ex3)
		{
			return new ConnectionResult(ex3, ex3.Message);
		}
		return new ConnectionResult(null, null, snowflakeDbConnection);
	}

	private string RebuildSnowflakeConnectionString(SnowflakeDbConnectionStringBuilder snowflakeDbConnectionString, bool shouldEqualSignBeDoubled = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string[] array = new string[3] { "warehouse", "db", "role" };
		foreach (string text in array)
		{
			if (snowflakeDbConnectionString.ContainsKey(text))
			{
				string text2 = snowflakeDbConnectionString[text]?.ToString();
				text2 = text2.Replace("\"", "\"\"");
				text2 = (shouldEqualSignBeDoubled ? text2.Replace("=", "==") : text2);
				stringBuilder.Append(text + "=\"" + text2 + "\";");
				snowflakeDbConnectionString.Remove(text);
			}
		}
		stringBuilder.Append(snowflakeDbConnectionString.ToString());
		return stringBuilder.ToString();
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}

	private string GetConnectionString(string database, string host, string login, string password, int? port, bool withDatabase = true, string warehouse = null, string sslType = null, string sslKeyPath = null, string connectionRole = null)
	{
		SnowflakeDbConnectionStringBuilder snowflakeDbConnectionStringBuilder = new SnowflakeDbConnectionStringBuilder();
		try
		{
			host = new Uri(host).Host;
		}
		catch (Exception)
		{
		}
		snowflakeDbConnectionStringBuilder["account"] = host.Split('.').FirstOrDefault();
		snowflakeDbConnectionStringBuilder["host"] = host;
		snowflakeDbConnectionStringBuilder["server"] = host;
		snowflakeDbConnectionStringBuilder["user"] = login;
		switch (SnowflakeAuthMethod.StringToType(sslType))
		{
		case SnowflakeAuthMethod.SnowflakeAuthMethodEnum.LoginPassword:
			snowflakeDbConnectionStringBuilder["authenticator"] = "snowflake";
			snowflakeDbConnectionStringBuilder["password"] = password;
			break;
		case SnowflakeAuthMethod.SnowflakeAuthMethodEnum.SSO_ExternalBrowser:
			snowflakeDbConnectionStringBuilder["authenticator"] = "externalbrowser";
			break;
		case SnowflakeAuthMethod.SnowflakeAuthMethodEnum.JWT_PrivateKey:
			snowflakeDbConnectionStringBuilder["authenticator"] = "snowflake_jwt";
			snowflakeDbConnectionStringBuilder["private_key_pwd"] = password;
			if (File.Exists(sslKeyPath))
			{
				snowflakeDbConnectionStringBuilder["private_key_file"] = sslKeyPath;
			}
			else
			{
				snowflakeDbConnectionStringBuilder["private_key"] = sslKeyPath;
			}
			break;
		}
		if (withDatabase && !string.IsNullOrEmpty(database))
		{
			snowflakeDbConnectionStringBuilder["db"] = database;
		}
		if (!string.IsNullOrEmpty(warehouse))
		{
			snowflakeDbConnectionStringBuilder["warehouse"] = warehouse;
		}
		if (!string.IsNullOrEmpty(connectionRole))
		{
			snowflakeDbConnectionStringBuilder["role"] = connectionRole;
		}
		return RebuildSnowflakeConnectionString(snowflakeDbConnectionStringBuilder, shouldEqualSignBeDoubled: true);
	}

	private string GetVersionString(object connection)
	{
		using (IDbCommand dbCommand = CommandsWithTimeout.Snowflake("select CURRENT_VERSION();", connection))
		{
			using IDataReader dataReader = dbCommand.ExecuteReader();
			if (dataReader.Read())
			{
				return dataReader[0] as string;
			}
		}
		return string.Empty;
	}
}
