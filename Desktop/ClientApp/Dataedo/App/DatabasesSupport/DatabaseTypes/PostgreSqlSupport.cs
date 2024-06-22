using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.PostgreSql;
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
using Npgsql;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class PostgreSqlSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => true;

	public bool CanCreateImportCommand => true;

	public virtual bool CanExportExtendedPropertiesOrComments => true;

	public bool CanFilterBySchema => true;

	public virtual bool CanGetDatabases => true;

	public bool CanImportToCustomFields => false;

	public virtual string ChooseObjectPageDescriptionText => "Choose which objects' comments to export to the database.";

	public virtual string ExportToDatabaseButtonDescription => "Export comments to database";

	public string EmptyScriptMessage => "The script is not available. This could be caused by lack of privileges at the time of import.";

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool HasSslSettings => true;

	public bool IsSchemaType => true;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.PostgreSQL;

	public bool ShouldForceFullReimport => true;

	public bool SupportsReadingExternalDependencies => false;

	public virtual Image TypeImage => Resources.postgresql;

	public virtual LoadObjectTypeEnum TypeOfExportToDatabase => LoadObjectTypeEnum.PostgreComments;

	public bool CanGenerateDDLScript => true;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.PostgreSQL;

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
		return SharedDatabaseTypeEnum.DatabaseType.PostgreSQL;
	}

	public void CloseConnection(object connection)
	{
		if (connection is NpgsqlConnection npgsqlConnection && npgsqlConnection.State != 0)
		{
			npgsqlConnection.Close();
			npgsqlConnection.Dispose();
		}
	}

	public virtual void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		CommentsExporter commentsExporter = new CommentsExporter(exportWorker, connectionString, currentDatabaseId, chosenModules, new PostgreSqlCommentsExceptionHandler());
		commentsExporter.GetTypeNames(chooseObjectTypesUserControl.GetIncludedDbObjects());
		commentsExporter.ExportDescription(exportDescriptions: true, owner);
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return GetConnectionString(database, host, login, password, port, certPath, param1, withDatabase, SSLType);
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		return GetDatabases(connectionString, splashScreenManager, owner);
	}

	public static List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, Form owner = null)
	{
		try
		{
			List<string> list = new List<string>();
			using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(connectionString))
			{
				npgsqlConnection.Open();
				using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.PostgreSql("SELECT datname as name\r\n                    FROM pg_database\r\n                    WHERE datname NOT IN ('template0', 'template1');", npgsqlConnection);
				using NpgsqlDataReader npgsqlDataReader = npgsqlCommand?.ExecuteReader();
				while (npgsqlDataReader != null && npgsqlDataReader.Read())
				{
					if (npgsqlDataReader[0] != null && !(npgsqlDataReader[0] is DBNull))
					{
						list.Add(npgsqlDataReader[0]?.ToString());
					}
				}
			}
			return list;
		}
		catch (NpgsqlException ex)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			GeneralMessageBoxesHandling.Show(GetExceptionMessage(ex, string.Empty), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		catch (Exception ex2)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			GeneralMessageBoxesHandling.Show(GetSSLCertExceptionMessage(ex2), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		return null;
	}

	public string GetDbmsVersion(object connection)
	{
		return GetPostgreSQLDbmsVersion(connection);
	}

	public static string GetPostgreSQLDbmsVersion(object connection)
	{
		return GetDbmsVersion(connection, "SELECT version() as version");
	}

	public static string GetDbmsVersion(object connection, string command)
	{
		using (NpgsqlCommand npgsqlCommand = CommandsWithTimeout.PostgreSql(command, connection))
		{
			using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
			if (npgsqlDataReader.Read())
			{
				return npgsqlDataReader["version"] as string;
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
		CommentsExporter commentsExporter = new CommentsExporter(exportWorker, connectionString, currentDatabaseId, chosenModules, new PostgreSqlCommentsExceptionHandler());
		commentsExporter.GetTypeNames(chooseObjectTypesUserControl.GetIncludedDbObjects());
		commentsExporter.InitializeExportObjects();
		return commentsExporter.DescriptionObjects.Select((DBDescription x) => x.Command);
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
		return "PostgreSQL";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		return "'' as \"data_length\"";
	}

	public DateTime? GetServerTime(object connection)
	{
		return GetServerTimePostgreSQL(connection);
	}

	public static DateTime? GetServerTimePostgreSQL(object connection)
	{
		using (NpgsqlCommand npgsqlCommand = CommandsWithTimeout.PostgreSql("select to_char(current_timestamp, 'YYYY-MM-DD HH24:MI:SS') as date", connection))
		{
			using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
			if (npgsqlDataReader.Read())
			{
				return DateTime.ParseExact(npgsqlDataReader["date"] as string, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
			}
		}
		return null;
	}

	public virtual SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizePostgreSql(synchronizeParameters);
	}

	public virtual DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.PostgreSQL;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		return GetVersionPostgreSQL(connection);
	}

	public static DatabaseVersionUpdate GetVersionPostgreSQL(object connection)
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
		return new PostgreSQLConnection();
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
		return TryConnection(name, connectionStringBuilder);
	}

	public static ConnectionResult TryConnection(string name, Func<string> connectionStringBuilder)
	{
		NpgsqlConnection npgsqlConnection = null;
		try
		{
			npgsqlConnection = new NpgsqlConnection(connectionStringBuilder());
			npgsqlConnection.Open();
		}
		catch (NpgsqlException ex)
		{
			return new ConnectionResult(ex, GetExceptionMessage(ex, name));
		}
		catch (Exception ex2)
		{
			return new ConnectionResult(ex2, ex2.Message);
		}
		return new ConnectionResult(null, null, npgsqlConnection);
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}

	public static string GetConnectionString(string database, string host, string login, string password, int? port, string certPath, string encryptedCertPassword, bool withDatabase = true, string SSLType = "DISABLE")
	{
		if (!port.HasValue)
		{
			throw new ArgumentException("Specified port is not valid.");
		}
		NpgsqlConnectionStringBuilder npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder
		{
			Host = host,
			Port = port.Value
		};
		if (withDatabase)
		{
			npgsqlConnectionStringBuilder.Database = database;
		}
		npgsqlConnectionStringBuilder.Username = login;
		npgsqlConnectionStringBuilder.Password = password;
		npgsqlConnectionStringBuilder.Pooling = false;
		npgsqlConnectionStringBuilder.SslMode = SSLTypeEnum.StringToNpgsqlType(SSLType);
		if (npgsqlConnectionStringBuilder.SslMode == SslMode.Require || npgsqlConnectionStringBuilder.SslMode == SslMode.Prefer)
		{
			npgsqlConnectionStringBuilder.TrustServerCertificate = true;
		}
		return npgsqlConnectionStringBuilder.ToString();
	}

	private static string GetExceptionMessage(NpgsqlException ex, string name)
	{
		if (ex.Message.Equals("Exception while performing SSL handshake") && ex.InnerException != null)
		{
			return GetSSLCertExceptionMessage(ex.InnerException);
		}
		if (!(ex is PostgresException ex2))
		{
			return "Cannot connect to server." + Environment.NewLine + ex.Message;
		}
		string text = "<href=" + Links.PostgreSQLSSLErrors + ">Learn more...</href>";
		switch (ex2.SqlState)
		{
		case "3D000":
			return "Incorrect database was specified. Database '" + name + "' not exists or is not reachable.";
		case "28000":
		{
			string message = ex2.Message;
			if (message != null && message.Contains("SSL off"))
			{
				return "Server requires SSL connection." + Environment.NewLine + Environment.NewLine + text;
			}
			return ex.Message + Environment.NewLine + Environment.NewLine + text;
		}
		case "28P01":
			return "Incorrect username or password was specified.";
		default:
			return ex.Message;
		}
	}

	private static string GetSSLCertExceptionMessage(Exception ex)
	{
		string text = "<href=" + Links.PostgreSQLSSLErrors + ">Learn more...</href>";
		string empty = string.Empty;
		if (ex.Message.Contains("PEM certificates are only supported with .NET 5 and higher"))
		{
			empty = "Provided SSL certificate has an invalid format. Provide a PFX certificate.";
		}
		else if (ex.Message.Contains("The specified network password is not correct"))
		{
			empty = "Provided passphrase is incorrect.";
		}
		else if (ex.Message.Contains("A call to SSPI failed, see inner exception.") && ex.InnerException != null)
		{
			empty = ex.InnerException.Message;
		}
		else
		{
			StringBuilder stringBuilder = new StringBuilder();
			GetRecursiveExceptionMessage(ex, stringBuilder);
			empty = stringBuilder.ToString();
		}
		empty += Environment.NewLine;
		empty += Environment.NewLine;
		return empty + text;
	}

	private static void GetRecursiveExceptionMessage(Exception ex, StringBuilder builder)
	{
		if (!string.IsNullOrEmpty(ex.Message))
		{
			builder.Append(ex.Message + " ");
		}
		if (ex.InnerException != null)
		{
			GetRecursiveExceptionMessage(ex.InnerException, builder);
		}
	}

	private static string GetVersionString(object connection)
	{
		using (NpgsqlCommand npgsqlCommand = CommandsWithTimeout.PostgreSql("select version();", connection))
		{
			using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
			if (npgsqlDataReader.Read())
			{
				return npgsqlDataReader[0] as string;
			}
		}
		return string.Empty;
	}
}
