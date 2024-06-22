using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.Redshift;
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

internal class RedshiftSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => true;

	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => true;

	public bool CanFilterBySchema => true;

	public bool CanGetDatabases => false;

	public bool CanImportToCustomFields => false;

	public string ChooseObjectPageDescriptionText => "Choose which objects' comments to export to the database.";

	public string ExportToDatabaseButtonDescription => "Export comments to database";

	public string EmptyScriptMessage => "The script is not available. This could be caused by lack of privileges at the time of import.";

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool HasSslSettings => false;

	public bool IsSchemaType => true;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Redshift;

	public bool ShouldForceFullReimport => true;

	public bool SupportsReadingExternalDependencies => false;

	public Image TypeImage => Resources.redshift;

	public LoadObjectTypeEnum TypeOfExportToDatabase => LoadObjectTypeEnum.OracleComments;

	public bool CanGenerateDDLScript => true;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Redshift;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.Redshift;
	}

	public void CloseConnection(object connection)
	{
		if (connection is NpgsqlConnection npgsqlConnection && npgsqlConnection.State != 0)
		{
			npgsqlConnection.Close();
			npgsqlConnection.Dispose();
		}
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		NpgsqlCommentsExporter npgsqlCommentsExporter = new NpgsqlCommentsExporter(exportWorker, connectionString, currentDatabaseId, chosenModules);
		npgsqlCommentsExporter.GetTypeNames(chooseObjectTypesUserControl.GetIncludedDbObjects());
		npgsqlCommentsExporter.ExportDescription(exportDescriptions: true, owner);
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return GetConnectionString(database, host, login, password, port, withDatabase, SSLType);
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		throw new NotSupportedException();
	}

	public string GetDbmsVersion(object connection)
	{
		using (NpgsqlCommand npgsqlCommand = CommandsWithTimeout.Redshift("select 'Redshift ' || VERSION() as version", connection))
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

	public IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		NpgsqlCommentsExporter npgsqlCommentsExporter = new NpgsqlCommentsExporter(exportWorker, connectionString, currentDatabaseId, chosenModules);
		npgsqlCommentsExporter.GetTypeNames(chooseObjectTypesUserControl.GetIncludedDbObjects());
		npgsqlCommentsExporter.InitializeExportObjects();
		return npgsqlCommentsExporter.DescriptionObjects.Select((DBDescription x) => x.Command);
	}

	public FilterObjectTypeEnum.FilterObjectType[] GetFilterObjectTypes()
	{
		return new FilterObjectTypeEnum.FilterObjectType[4]
		{
			FilterObjectTypeEnum.FilterObjectType.View,
			FilterObjectTypeEnum.FilterObjectType.Procedure,
			FilterObjectTypeEnum.FilterObjectType.Function,
			FilterObjectTypeEnum.FilterObjectType.Table
		};
	}

	public string GetFilterRuleConcatenation(string schemaFieldName, string nameFieldName)
	{
		return schemaFieldName + " + '.' + " + nameFieldName;
	}

	public string GetFriendlyDisplayNameForImport(bool isLite)
	{
		if (!isLite)
		{
			return "Amazon Redshift";
		}
		return "Amazon Redshift (Pro)";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		return "c.character_maximum_length as data_length";
	}

	public DateTime? GetServerTime(object connection)
	{
		using (NpgsqlCommand npgsqlCommand = CommandsWithTimeout.Redshift("select to_char(sysdate, 'YYYY-MM-DD HH24:MI:SS') as date", connection))
		{
			using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
			if (npgsqlDataReader.Read())
			{
				return DateTime.ParseExact(npgsqlDataReader["date"] as string, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
			}
		}
		return null;
	}

	public SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeRedshift(synchronizeParameters);
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.Redshift;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		string versionString = GetVersionString(connection);
		Match match = new Regex("Redshift ([0-9]+\\.[0-9]+)", RegexOptions.IgnoreCase).Match(versionString);
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
		return null;
	}

	public ConnectionBase GetXmlConnectionModel()
	{
		return new RedshiftConnection();
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

	private string GetConnectionString(string database, string host, string login, string password, int? port, bool withDatabase = true, string SSLType = "DISABLE")
	{
		NpgsqlConnectionStringBuilder npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder();
		npgsqlConnectionStringBuilder.Host = host;
		npgsqlConnectionStringBuilder.Port = port.Value;
		if (withDatabase)
		{
			npgsqlConnectionStringBuilder.Database = database;
		}
		npgsqlConnectionStringBuilder.Username = login;
		npgsqlConnectionStringBuilder.Password = password;
		npgsqlConnectionStringBuilder.Pooling = false;
		npgsqlConnectionStringBuilder.SslMode = SSLTypeEnum.StringToNpgsqlType(SSLType);
		return npgsqlConnectionStringBuilder.ToString();
	}

	private string GetExceptionMessage(NpgsqlException ex, string name)
	{
		if (!(ex is PostgresException ex2))
		{
			return "Cannot connect to server." + Environment.NewLine + ex.Message;
		}
		switch (ex2.SqlState)
		{
		case "3D000":
			return "Incorrect database was specified. Database '" + name + "' not exists or is not reachable.";
		case "28000":
		{
			string message = ex2.Message;
			if (message != null && message.Contains("SSL off"))
			{
				return "Server requires SSL connection. Please choose Require SSL Mode";
			}
			return "Incorrect username or password was specified.";
		}
		case "28P01":
			return "Incorrect password was specified.";
		default:
			return ex.Message;
		}
	}

	private string GetVersionString(object connection)
	{
		using (NpgsqlCommand npgsqlCommand = CommandsWithTimeout.Redshift("select VERSION()", connection))
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
