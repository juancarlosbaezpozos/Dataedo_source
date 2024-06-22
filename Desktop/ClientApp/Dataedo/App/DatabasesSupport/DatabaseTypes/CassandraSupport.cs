using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Cassandra;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.Cassandra;
using Dataedo.App.Data.General;
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

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class CassandraSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	protected const string GetKeyspaceListCommand = "SELECT keyspace_name FROM system_schema.keyspaces;";

	public bool CanImportDependencies => true;

	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => false;

	public bool CanFilterBySchema => false;

	public bool CanGetDatabases => true;

	public bool CanImportToCustomFields => false;

	public string ChooseObjectPageDescriptionText => "Choose which objects' comments to export to the database.";

	public string ExportToDatabaseButtonDescription => "Export comments to database";

	public string EmptyScriptMessage => "The script is not available";

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool HasSslSettings => false;

	public bool IsSchemaType => true;

	public virtual PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Cassandra;

	public bool ShouldForceFullReimport => true;

	public bool SupportsReadingExternalDependencies => false;

	public virtual Image TypeImage => Resources.apache_cassandra;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public virtual bool CanGenerateDDLScript => true;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Cassandra;

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
		return SharedDatabaseTypeEnum.DatabaseType.Cassandra;
	}

	public void CloseConnection(object connection)
	{
		ISession session = CommandsWithTimeout.Cassandra(connection);
		if (session != null)
		{
			ICluster cluster = session.Cluster;
			session.Dispose();
			if (cluster != null)
			{
				cluster.Shutdown();
				cluster.Dispose();
			}
		}
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotSupportedException();
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string authenticationDatabase = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return GetConnectionString(applicationName, database, host, login, password, port, withDatabase);
	}

	public virtual string GetConnectionString(string applicationName, string database, string host, string login, string password, int? port, bool withDatabase)
	{
		if (!port.HasValue)
		{
			throw new ArgumentException("Specified port is not valid.");
		}
		CassandraConnectionStringBuilder cassandraConnectionStringBuilder = new CassandraConnectionStringBuilder();
		cassandraConnectionStringBuilder.ContactPoints = new string[1] { host };
		cassandraConnectionStringBuilder.Port = port.Value;
		if (withDatabase)
		{
			cassandraConnectionStringBuilder.DefaultKeyspace = database;
		}
		cassandraConnectionStringBuilder.Username = login;
		cassandraConnectionStringBuilder.Password = password;
		return cassandraConnectionStringBuilder.ToString();
	}

	public virtual List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		try
		{
			List<string> list = new List<string>();
			using (Cluster cluster = Cluster.Builder().WithConnectionString(connectionString).Build())
			{
				using ISession session = cluster.Connect();
				using RowSet rowSet = session.Execute(new SimpleStatement("SELECT keyspace_name FROM system_schema.keyspaces;").SetReadTimeoutMillis(CommandsWithTimeout.Timeout * 1000));
				foreach (Row item in rowSet)
				{
					if (item[0] != null && !(item[0] is DBNull))
					{
						list.Add(item[0]?.ToString());
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
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		return null;
	}

	public string GetDbmsVersion(object connection)
	{
		string query = "SELECT release_version FROM system.local;";
		return (connection as ISession).Execute(new SimpleStatement(query).SetReadTimeoutMillis(CommandsWithTimeout.Timeout * 1000)).FirstOrDefault()["release_version"] as string;
	}

	public List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		throw new NotImplementedException();
	}

	public virtual IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		throw new NotImplementedException();
	}

	public FilterObjectTypeEnum.FilterObjectType[] GetFilterObjectTypes()
	{
		return new FilterObjectTypeEnum.FilterObjectType[3]
		{
			FilterObjectTypeEnum.FilterObjectType.Table,
			FilterObjectTypeEnum.FilterObjectType.View,
			FilterObjectTypeEnum.FilterObjectType.Function
		};
	}

	public string GetFilterRuleConcatenation(string schemaFieldName, string nameFieldName)
	{
		throw new NotImplementedException();
	}

	public virtual string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Cassandra";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		throw new NotImplementedException();
	}

	public DateTime? GetServerTime(object connection)
	{
		string query = "select CAST(dateof(now()) AS TIMESTAMP) as \"date\"\r\n                            from system.local;";
		return DateTime.ParseExact(Convert.ToDateTime((connection as ISession).Execute(new SimpleStatement(query).SetReadTimeoutMillis(CommandsWithTimeout.Timeout * 1000)).FirstOrDefault()["date"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
	}

	public virtual SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeCassandra(synchronizeParameters);
	}

	public virtual DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.Cassandra;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		string dbmsVersion = GetDbmsVersion(connection);
		Match match = new Regex("([0-9]+\\.[0-9]+)", RegexOptions.IgnoreCase).Match(dbmsVersion);
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

	public virtual ConnectionBase GetXmlConnectionModel()
	{
		return new CassandraConnection();
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

	public virtual ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		ISession connection;
		try
		{
			connection = Cluster.Builder().WithConnectionString(connectionStringBuilder()).WithQueryTimeout(CommandsWithTimeout.Timeout * 1000)
				.Build()
				.Connect();
		}
		catch (Exception ex)
		{
			return new ConnectionResult(ex, ex.Message);
		}
		return new ConnectionResult(null, null, connection);
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}
}
