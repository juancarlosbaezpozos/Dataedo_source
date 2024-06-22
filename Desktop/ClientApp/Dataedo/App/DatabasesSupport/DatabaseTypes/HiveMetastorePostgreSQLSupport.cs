using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.HiveMetastore;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Base;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using Npgsql;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class HiveMetastorePostgreSQLSupport : DatabaseSupportBase, IHiveMetastoreSupport, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => false;

	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => false;

	public bool CanFilterBySchema => false;

	public bool CanGetDatabases => true;

	public bool CanImportToCustomFields => false;

	public string ChooseObjectPageDescriptionText => "Choose which objects' comments to export to the database.";

	public string ExportToDatabaseButtonDescription => "Export descriptions/custom fields to database (extended properties)";

	public string EmptyScriptMessage => string.Empty;

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool HasSslSettings => false;

	public bool IsSchemaType => false;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.HiveMetastorePostgreSQL;

	public bool ShouldForceFullReimport => true;

	public bool SupportsReadingExternalDependencies => false;

	public Image TypeImage => Resources.postgresql;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public bool CanGenerateDDLScript => false;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.HiveMetastorePostgreSQL;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.HiveMetastorePostgreSQL;
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
		throw new NotSupportedException();
	}

	public List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		throw new NotImplementedException();
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return PostgreSqlSupport.GetConnectionString(database, host, login, password, port, certPath, caPath, withDatabase, SSLType);
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		return PostgreSqlSupport.GetDatabases(connectionString, splashScreenManager, owner);
	}

	private string GetHiveDatabasesCmd(string databaseName)
	{
		return "\r\n                SELECT\r\n                    \"d\".\"NAME\" AS \"database_name\",\r\n                    \"d\".\"DESC\" AS \"database_desc\",\r\n                    \"c\".\"NAME\" AS \"catalog_name\",\r\n                    \"c\".\"DESC\" AS \"catalog_desc\"\r\n                FROM \"DBS\" AS \"d\"\r\n                LEFT JOIN \"CTLGS\" AS \"c\"\r\n                    ON \"d\".\"CTLG_NAME\" = \"c\".\"NAME\"\r\n                WHERE \r\n                    \"d\".\"NAME\" NOT IN ('information_schema', 'sys')\r\n                ORDER BY \"d\".\"NAME\"";
	}

	public List<HiveDatabaseInfo> GetHiveDatabases(string connectionString, SplashScreenManager splashScreenManager, string databaseName, Form owner = null)
	{
		try
		{
			List<HiveDatabaseInfo> list = new List<HiveDatabaseInfo>();
			using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(connectionString))
			{
				npgsqlConnection.Open();
				using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.PostgreSql(GetHiveDatabasesCmd(databaseName), npgsqlConnection);
				using NpgsqlDataReader npgsqlDataReader = npgsqlCommand?.ExecuteReader();
				while (npgsqlDataReader != null && npgsqlDataReader.Read())
				{
					list.Add(new HiveDatabaseInfo(npgsqlDataReader));
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
		return GetFullHiveNameVersion(connection);
	}

	public static string GetFullHiveNameVersion(object connection)
	{
		return GetHiveNameVersion(connection) + " (" + GetParentDBMSVersion(connection) + ")";
	}

	public static string GetParentDBMSVersion(object connection)
	{
		return PostgreSqlSupport.GetPostgreSQLDbmsVersion(connection)?.Trim();
	}

	public static string GetHiveNameVersion(object connection)
	{
		return PostgreSqlSupport.GetDbmsVersion(connection, "SELECT \"VERSION_COMMENT\" AS \"version\" FROM \"VERSION\"");
	}

	public static string GetHiveNumericVersion(object connection)
	{
		return PostgreSqlSupport.GetDbmsVersion(connection, "SELECT \"SCHEMA_VERSION\" AS \"version\" FROM \"VERSION\"");
	}

	public IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		throw new NotSupportedException();
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
		throw new NotSupportedException();
	}

	public string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Hive Metastore - PostgreSQL";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		return "NULL AS \"data_length\"";
	}

	public DateTime? GetServerTime(object connection)
	{
		return PostgreSqlSupport.GetServerTimePostgreSQL(connection);
	}

	public SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeHiveMetastorePostgreSQL(synchronizeParameters);
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.HiveMetastorePostgreSQL;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		return PostgreSqlSupport.GetVersionPostgreSQL(connection);
	}

	public ConnectionBase GetXmlConnectionModel()
	{
		return new HiveMetastorePostgreSQLConnection();
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
		return PostgreSqlSupport.TryConnection(name, connectionStringBuilder);
	}

	public ConnectionResult TryConnection(Func<string> connectionStringBuilder, string dbmsDatabaseName, string hiveCatalogName, string hiveDatabaseName, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		ConnectionResult connectionResult = TryConnection(connectionStringBuilder, dbmsDatabaseName, user, warehouse, useOnlyRequiredFields);
		if (connectionResult.IsSuccess && connectionResult.Connection != null && connectionResult.Connection is NpgsqlConnection npgsqlConnection && npgsqlConnection.State == ConnectionState.Open && !string.IsNullOrWhiteSpace(dbmsDatabaseName) && !string.IsNullOrWhiteSpace(hiveDatabaseName) && !string.IsNullOrWhiteSpace(hiveCatalogName))
		{
			if (!VerifyIfDBMSDatabaseIsHiveMetastore(dbmsDatabaseName, npgsqlConnection))
			{
				return new ConnectionResult(null, "The database <i>" + dbmsDatabaseName + "</i> is not an Hive Metastore 3.x database", failedConnection: true);
			}
			using (NpgsqlCommand npgsqlCommand = CommandsWithTimeout.PostgreSql("SELECT 1\r\n                    FROM \"CTLGS\"\r\n                    WHERE \"NAME\" = '" + hiveCatalogName + "'", npgsqlConnection))
			{
				if (npgsqlCommand?.ExecuteScalar() == null)
				{
					return new ConnectionResult(null, "The catalog <i>" + hiveCatalogName + "</i> does not exist in the <i>" + dbmsDatabaseName + "</i> Hive Metastore database", failedConnection: true);
				}
			}
			using NpgsqlCommand npgsqlCommand2 = CommandsWithTimeout.PostgreSql("SELECT 1\r\n                    FROM \"DBS\"\r\n                    WHERE\r\n                        \"NAME\" = '" + hiveDatabaseName + "'\r\n                        AND \"CTLG_NAME\" = '" + hiveCatalogName + "'", npgsqlConnection);
			if (npgsqlCommand2?.ExecuteScalar() == null)
			{
				return new ConnectionResult(null, "The database <i>" + hiveDatabaseName + "</i> does not exist in the <i>" + hiveCatalogName + "</i> catalog in <i>" + dbmsDatabaseName + "</i> Hive Metastore database", failedConnection: true);
			}
			return connectionResult;
		}
		return connectionResult;
	}

	public bool? VerifyIfDatabaseIsHiveMetastore(string connectionString, string databaseName, SplashScreenManager splashScreenManager, Form owner = null)
	{
		try
		{
			using NpgsqlConnection npgsqlConnection = new NpgsqlConnection(connectionString);
			npgsqlConnection.Open();
			return VerifyIfDBMSDatabaseIsHiveMetastore(databaseName, npgsqlConnection);
		}
		catch (Exception ex)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			GeneralMessageBoxesHandling.Show("Unable to connect to server." + Environment.NewLine + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			return null;
		}
	}

	private static bool VerifyIfDBMSDatabaseIsHiveMetastore(string dbmsDatabaseName, NpgsqlConnection conn)
	{
		try
		{
			string[] array = GetHiveNumericVersion(conn)?.Split('.');
			if (array[0] == "0" || array[0] == "1" || array[0] == "2")
			{
				TrackingRunner.Track(delegate
				{
					TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoConnectionBuilderEventSpecificDescription(new TrackingConnectionParameters(GetFullHiveNameVersion(conn), SharedDatabaseTypeEnum.DatabaseType.HiveMetastorePostgreSQL, "NO", string.Empty), new TrackingUserParameters(), new TrackingDataedoParameters(), "Selected Hive Metastore version is too low"), TrackingEventEnum.ImportConnectionFailed);
				});
				return false;
			}
		}
		catch (NpgsqlException ex)
		{
			if (ex?.Message == "relationship \"VERSION\" does not exist")
			{
				TrackNotHiveMetastore(conn);
				return false;
			}
			if (ex?.Message == "column \"SCHEMA_VERSION\" does not exist")
			{
				TrackNotHiveMetastore(conn);
				return false;
			}
			throw ex;
		}
		using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.PostgreSql("SELECT 1\r\n                        FROM \"information_schema\".\"tables\" AS \"tab\"\r\n                        WHERE \"tab\".\"table_catalog\" = '" + dbmsDatabaseName + "' \r\n                            AND \"tab\".\"table_type\" = 'BASE TABLE' \r\n                        \tAND \"tab\".\"table_name\" IN ('VERSION', 'TBLS', 'DBS', 'COLUMNS_V2', 'FUNCS', 'CTLGS')\r\n                        GROUP BY \"tab\".\"table_catalog\"\r\n                        HAVING COUNT(*) = 6", conn);
		bool num = npgsqlCommand?.ExecuteScalar() != null;
		if (!num)
		{
			TrackNotHiveMetastore(conn);
		}
		return num;
	}

	private static void TrackNotHiveMetastore(object conn)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoConnectionBuilderEventSpecificDescription(new TrackingConnectionParameters(GetParentDBMSVersion(conn), SharedDatabaseTypeEnum.DatabaseType.HiveMetastorePostgreSQL, "NO", string.Empty), new TrackingUserParameters(), new TrackingDataedoParameters(), "Selected database is not Hive Metastore 3.x"), TrackingEventEnum.ImportConnectionFailed);
		});
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}
}
