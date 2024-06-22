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
using Dataedo.App.Enums;
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
using Microsoft.Data.SqlClient;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class HiveMetastoreSQLServerSupport : DatabaseSupportBase, IHiveMetastoreSupport, IDatabaseSupport, IDatabaseSupportShared
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

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.HiveMetastoreSQLServer;

	public bool ShouldForceFullReimport => true;

	public bool SupportsReadingExternalDependencies => false;

	public virtual Image TypeImage => Resources.sqlserver;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public bool CanGenerateDDLScript => false;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreSQLServer;

	public virtual SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreSQLServer;
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
		throw new NotSupportedException();
	}

	public List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		throw new NotImplementedException();
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		SqlServerConnectionModeEnum.SqlServerConnectionMode connectionMode = SqlServerConnectionModeEnum.StringToTypeOrDefault(param1);
		return SqlServerSupport.GetConnectionString(applicationName, database, host, login, password, port, authenticationType, connectionMode, withPooling, withDatabase);
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		return SqlServerSupport.GetDatabases(connectionString, splashScreenManager, owner);
	}

	private string GetHiveDatabasesCmd(string databaseName)
	{
		return "\r\n                SELECT\r\n                    d.[NAME] AS 'database_name',\r\n                    d.[DESC] AS 'database_desc',\r\n                    c.[NAME] AS 'catalog_name',\r\n                    c.[DESC] AS 'catalog_desc'\r\n                FROM [" + databaseName + "].dbo.DBS d\r\n                LEFT JOIN [" + databaseName + "].dbo.CTLGS c\r\n                    on d.CTLG_NAME = c.NAME\r\n                WHERE \r\n                    d.NAME NOT IN ('information_schema', 'sys')\r\n                ORDER BY d.NAME";
	}

	public List<HiveDatabaseInfo> GetHiveDatabases(string connectionString, SplashScreenManager splashScreenManager, string databaseName, Form owner = null)
	{
		try
		{
			List<HiveDatabaseInfo> list = new List<HiveDatabaseInfo>();
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				using SqlCommand sqlCommand = CommandsWithTimeout.SqlServer(GetHiveDatabasesCmd(databaseName), sqlConnection);
				using SqlDataReader sqlDataReader = sqlCommand?.ExecuteReader();
				while (sqlDataReader != null && sqlDataReader.Read())
				{
					list.Add(new HiveDatabaseInfo(sqlDataReader));
				}
			}
			return list;
		}
		catch (SqlException exception)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			GeneralMessageBoxesHandling.Show(SqlServerSupport.GetExceptionMessage(exception), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
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
		return GetFullHiveNameVersion(connection);
	}

	public static string GetFullHiveNameVersion(object connection)
	{
		return GetHiveNameVersion(connection) + " (" + GetParentDBMSVersion(connection) + ")";
	}

	public static string GetParentDBMSVersion(object connection)
	{
		return SqlServerSupport.GetSQLServerDbmsVersion(connection)?.Trim();
	}

	public static string GetHiveNameVersion(object connection)
	{
		return SqlServerSupport.GetDbmsVersion(connection, "SELECT VERSION_COMMENT as version FROM dbo.VERSION");
	}

	public static string GetHiveNumericVersion(object connection)
	{
		return SqlServerSupport.GetDbmsVersion(connection, "SELECT SCHEMA_VERSION as version FROM VERSION");
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

	public virtual string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Hive Metastore - SQL Server";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		return "null as 'data_length'";
	}

	public DateTime? GetServerTime(object connection)
	{
		return SqlServerSupport.GetServerTimeSQLServer(connection);
	}

	public virtual SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeHiveMetastoreSQLServer(synchronizeParameters);
	}

	public virtual DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.HiveMetastoreSQLServer;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		string[] array = SqlServerSupport.GetVersionString(connection)?.Split('.');
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
		return new HiveMetastoreSQLServerConnection();
	}

	public bool PrepareElements(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl, Form owner = null)
	{
		return true;
	}

	public void ProcessException(Exception ex, string name, string warehouse, Form owner = null)
	{
		if (ex is SqlException exception)
		{
			GeneralMessageBoxesHandling.Show(SqlServerSupport.GetExceptionMessage(exception), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		else
		{
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
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
		return SqlServerSupport.TryConnection(connectionStringBuilder);
	}

	public ConnectionResult TryConnection(Func<string> connectionStringBuilder, string dbmsDatabaseName, string hiveCatalogName, string hiveDatabaseName, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		ConnectionResult connectionResult = TryConnection(connectionStringBuilder, dbmsDatabaseName, user, warehouse, useOnlyRequiredFields);
		if (connectionResult.IsSuccess && connectionResult.Connection != null && connectionResult.Connection is SqlConnection sqlConnection && sqlConnection.State == ConnectionState.Open && !string.IsNullOrWhiteSpace(dbmsDatabaseName) && !string.IsNullOrWhiteSpace(hiveDatabaseName) && !string.IsNullOrWhiteSpace(hiveCatalogName))
		{
			if (!VerifyIfDBMSDatabaseIsHiveMetastore(dbmsDatabaseName, sqlConnection))
			{
				return new ConnectionResult(null, "The database <i>" + dbmsDatabaseName + "</i> is not a Hive Metastore 3.x database", failedConnection: true);
			}
			using (SqlCommand sqlCommand = CommandsWithTimeout.SqlServer("SELECT 1\r\n                    FROM " + dbmsDatabaseName + ".dbo.CTLGS\r\n                    WHERE NAME = '" + hiveCatalogName + "'", sqlConnection))
			{
				if (sqlCommand?.ExecuteScalar() == null)
				{
					return new ConnectionResult(null, "The catalog <i>" + hiveCatalogName + "</i> does not exist in the <i>" + dbmsDatabaseName + "</i> Hive Metastore database", failedConnection: true);
				}
			}
			using SqlCommand sqlCommand2 = CommandsWithTimeout.SqlServer("SELECT 1\r\n                    FROM " + dbmsDatabaseName + ".dbo.DBS\r\n                    WHERE\r\n                        NAME = '" + hiveDatabaseName + "'\r\n                        AND CTLG_NAME = '" + hiveCatalogName + "'", sqlConnection);
			if (sqlCommand2?.ExecuteScalar() == null)
			{
				return new ConnectionResult(null, "The database <i>" + hiveDatabaseName + "</i> does not exist in the <i>" + hiveCatalogName + "</i> catalog in <i>" + dbmsDatabaseName + "</i> Hive Metastore database", failedConnection: true);
			}
			return connectionResult;
		}
		return connectionResult;
	}

	private static bool VerifyIfDBMSDatabaseIsHiveMetastore(string databaseName, SqlConnection conn)
	{
		try
		{
			string[] array = GetHiveNumericVersion(conn)?.Split('.');
			if (array[0] == "0" || array[0] == "1" || array[0] == "2")
			{
				TrackingRunner.Track(delegate
				{
					TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoConnectionBuilderEventSpecificDescription(new TrackingConnectionParameters(GetFullHiveNameVersion(conn), SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreSQLServer, "NO", string.Empty), new TrackingUserParameters(), new TrackingDataedoParameters(), "Selected Hive Metastore version is too low"), TrackingEventEnum.ImportConnectionFailed);
				});
				return false;
			}
		}
		catch (SqlException ex)
		{
			if (ex.Number == 208)
			{
				TrackNotHiveMetastore(conn);
				return false;
			}
			if (ex.Number == 207)
			{
				TrackNotHiveMetastore(conn);
				return false;
			}
			throw ex;
		}
		using SqlCommand sqlCommand = CommandsWithTimeout.SqlServer("SELECT 1\r\n                FROM [" + databaseName + "].INFORMATION_SCHEMA.tables tab\r\n                WHERE tab.table_type='BASE TABLE' \r\n                \tAND tab.table_name in ('VERSION', 'TBLS', 'DBS', 'COLUMNS_V2', 'FUNCS', 'CTLGS')\r\n                GROUP BY TABLE_CATALOG\r\n                HAVING COUNT(*) = 6", conn);
		bool num = sqlCommand?.ExecuteScalar() != null;
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
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoConnectionBuilderEventSpecificDescription(new TrackingConnectionParameters(GetParentDBMSVersion(conn), SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreSQLServer, "NO", string.Empty), new TrackingUserParameters(), new TrackingDataedoParameters(), "Selected database is not Hive Metastore 3.x"), TrackingEventEnum.ImportConnectionFailed);
		});
	}

	public bool? VerifyIfDatabaseIsHiveMetastore(string connectionString, string databaseName, SplashScreenManager splashScreenManager, Form owner = null)
	{
		try
		{
			using SqlConnection sqlConnection = new SqlConnection(connectionString);
			sqlConnection.Open();
			return VerifyIfDBMSDatabaseIsHiveMetastore(databaseName, sqlConnection);
		}
		catch (SqlException exception)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			GeneralMessageBoxesHandling.Show(SqlServerSupport.GetExceptionMessage(exception), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			return null;
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

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}
}
