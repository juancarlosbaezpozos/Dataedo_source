using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Amazon;
using Amazon.Athena;
using Amazon.Athena.Model;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.Athena;
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
using Devart.Common;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class AthenaSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	private const string AccessToken = "accesstoken";

	private const string SecretToken = "secrettoken";

	private const string AwsRegion = "awsregion";

	private const string Database = "database";

	private const string WorkGroup = "workgroup";

	private const string DataCatalog = "datacatalog";

	public bool CanImportDependencies => false;

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

	public virtual bool HasSslSettings => false;

	public bool CanGenerateDDLScript => false;

	public bool IsSchemaType => false;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Athena;

	public bool ShouldForceFullReimport => true;

	public bool SupportsReadingExternalDependencies => false;

	public virtual Image TypeImage => Resources.amazon_athena;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Athena;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(string connectionString, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.Athena;
	}

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.Athena;
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotSupportedException();
	}

	public virtual string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return new DbConnectionStringBuilder
		{
			{ "accesstoken", login },
			{ "secrettoken", password },
			{ "awsregion", host },
			{ "database", database },
			{ "workgroup", param1 },
			{ "datacatalog", param2 }
		}.ConnectionString;
	}

	private static string SubmitAthenaQuery(AmazonAthenaClient athenaClient, string database, string workgroup, string dataCatalog, string query)
	{
		QueryExecutionContext queryExecutionContext = new QueryExecutionContext
		{
			Database = database,
			Catalog = dataCatalog
		};
		StartQueryExecutionRequest request = new StartQueryExecutionRequest
		{
			QueryString = query,
			QueryExecutionContext = queryExecutionContext,
			WorkGroup = workgroup
		};
		return athenaClient.StartQueryExecution(request).QueryExecutionId;
	}

	private static void WaitForQueryToComplete(AmazonAthenaClient athenaClient, string queryExecutionId)
	{
		GetQueryExecutionRequest request = new GetQueryExecutionRequest
		{
			QueryExecutionId = queryExecutionId
		};
		GetQueryExecutionResponse getQueryExecutionResponse = null;
		bool flag = true;
		while (flag)
		{
			getQueryExecutionResponse = athenaClient.GetQueryExecution(request);
			QueryExecutionState state = getQueryExecutionResponse.QueryExecution.Status.State;
			if (state == QueryExecutionState.FAILED)
			{
				throw new Exception("Query Failed to run with Error Message: " + getQueryExecutionResponse.QueryExecution.Status.StateChangeReason);
			}
			if (state == QueryExecutionState.CANCELLED)
			{
				throw new Exception("Query was cancelled.");
			}
			if (state == QueryExecutionState.SUCCEEDED)
			{
				flag = false;
			}
			else
			{
				Thread.Sleep(TimeSpan.FromMilliseconds(1000.0));
			}
		}
	}

	public virtual List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder
		{
			ConnectionString = connectionString
		};
		try
		{
			AmazonAthenaConfig clientConfig = new AmazonAthenaConfig
			{
				RegionEndpoint = RegionEndpoint.GetBySystemName(dbConnectionStringBuilder["awsregion"].ToString())
			};
			AmazonAthenaClient amazonAthenaClient = new AmazonAthenaClient(dbConnectionStringBuilder["accesstoken"].ToString(), dbConnectionStringBuilder["secrettoken"].ToString(), clientConfig);
			ListDatabasesRequest request = new ListDatabasesRequest
			{
				CatalogName = dbConnectionStringBuilder["datacatalog"].ToString()
			};
			ListDatabasesResponse listDatabasesResponse = amazonAthenaClient.ListDatabases(request);
			List<string> list = new List<string>();
			foreach (Database database in listDatabasesResponse.DatabaseList)
			{
				list.Add(database.Name);
			}
			return list;
		}
		catch (Exception ex)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		return null;
	}

	public string GetDbmsVersion(object connection)
	{
		AthenaConnectionWrapper athenaConnectionWrapper = CommandsWithTimeout.Athena(connection as AthenaConnectionWrapper);
		AmazonAthenaClient client = athenaConnectionWrapper.Client;
		GetWorkGroupRequest request = new GetWorkGroupRequest
		{
			WorkGroup = athenaConnectionWrapper.WorkGroup
		};
		return client.GetWorkGroup(request).WorkGroup.Configuration.EngineVersion.EffectiveEngineVersion;
	}

	public List<string> GetWorkgroupNames(string connectionString, object connection, string text, Form owner)
	{
		try
		{
			if (!(connection is AthenaConnectionWrapper conn))
			{
				return null;
			}
			AmazonAthenaClient client = CommandsWithTimeout.Athena(conn).Client;
			ListWorkGroupsRequest request = new ListWorkGroupsRequest();
			ListWorkGroupsResponse listWorkGroupsResponse = client.ListWorkGroups(request);
			List<string> list = new List<string>();
			list.AddRange(listWorkGroupsResponse?.WorkGroups?.Select((WorkGroupSummary x) => x.Name));
			return list;
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show(ex?.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		return null;
	}

	public List<string> GetDataCatalogNames(string connectionString, object connection, string text, Form owner)
	{
		try
		{
			if (!(connection is AthenaConnectionWrapper conn))
			{
				return null;
			}
			AmazonAthenaClient client = CommandsWithTimeout.Athena(conn).Client;
			ListDataCatalogsRequest request = new ListDataCatalogsRequest();
			ListDataCatalogsResponse listDataCatalogsResponse = client.ListDataCatalogs(request);
			List<string> list = new List<string>();
			list.AddRange(listDataCatalogsResponse?.DataCatalogsSummary?.Select((DataCatalogSummary x) => x.CatalogName));
			return list;
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show(ex?.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		return null;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		string dbmsVersion = GetDbmsVersion(connection);
		Match match = new Regex("(\\b(\\d+\\.)?(\\d+\\.)?(\\*|\\d+)$)", RegexOptions.IgnoreCase).Match(dbmsVersion);
		if (!match.Success)
		{
			return null;
		}
		if (int.TryParse(match.Value.Split('.')[0], out var result))
		{
			return new DatabaseVersionUpdate
			{
				Version = result,
				Update = 0
			};
		}
		return null;
	}

	public List<string> GetExtendedProperties(string connectionString, string host = null, List<string> schemas = null)
	{
		throw new NotSupportedException();
	}

	public IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		throw new NotSupportedException();
	}

	public FilterObjectTypeEnum.FilterObjectType[] GetFilterObjectTypes()
	{
		return new FilterObjectTypeEnum.FilterObjectType[2]
		{
			FilterObjectTypeEnum.FilterObjectType.Table,
			FilterObjectTypeEnum.FilterObjectType.View
		};
	}

	public string GetFilterRuleConcatenation(string schemaFieldName, string nameFieldName)
	{
		return "CONCAT(" + schemaFieldName + ", '.', " + nameFieldName + ")";
	}

	public virtual string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Amazon Athena";
	}

	public virtual string GetQueryForDataLength(string tableAlias)
	{
		return "case\r\n                    when data_type in ('CHAR','VARCHAR') then (\r\n                        case \r\n                            when character_maximum_length =-1 then 'max'\r\n                            else cast(character_maximum_length as char(8))\r\n                        end)\r\n                    when data_type in \r\n                            ('TINYINT', 'SMALLINT', 'MEDIUMINT', 'INT', 'BIGINT', 'FLOAT', 'DOUBLE', 'DECIMAL', 'BIT')\r\n                        then concat(cast(numeric_precision as char(4)), \r\n                    case \r\n                        when numeric_scale is null then ''\r\n                        else concat(', ', cast(numeric_scale as char(4)))\r\n                    end)\r\n                    end as `data_length`";
	}

	public DateTime? GetServerTime(object connection)
	{
		AthenaConnectionWrapper athenaConnectionWrapper = connection as AthenaConnectionWrapper;
		AmazonAthenaClient obj = CommandsWithTimeout.Athena(athenaConnectionWrapper)?.Client;
		string query = "select date_format(now(), '%Y-%m-%d %H:%i:%s') as date";
		string queryExecutionId = SubmitAthenaQuery(obj, athenaConnectionWrapper.Database, athenaConnectionWrapper.WorkGroup, athenaConnectionWrapper.DataCatalog, query);
		WaitForQueryToComplete(obj, queryExecutionId);
		GetQueryResultsRequest request = new GetQueryResultsRequest
		{
			QueryExecutionId = queryExecutionId
		};
		return DateTime.ParseExact(Convert.ToDateTime(obj.GetQueryResults(request).ResultSet.Rows[1].Data[0].VarCharValue).ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
	}

	public virtual SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeAthena(synchronizeParameters);
	}

	public virtual DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.Athena;
	}

	public virtual ConnectionBase GetXmlConnectionModel()
	{
		return new AthenaConnection();
	}

	public bool PrepareElements(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl, Form owner = null)
	{
		return true;
	}

	public void ProcessException(Exception ex, string name, string warehouse, Form owner = null)
	{
		GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
	}

	protected void TestConnection(AthenaConnectionWrapper connection)
	{
		AmazonAthenaClient amazonAthenaClient = CommandsWithTimeout.Athena(connection)?.Client;
		string query = "select v.table_name from information_schema.views v where table_schema = '" + connection.Database + "' limit 1";
		if (connection.Database != null && connection.WorkGroup != null && connection.DataCatalog != null)
		{
			string queryExecutionId = SubmitAthenaQuery(amazonAthenaClient, connection.Database, connection.WorkGroup, connection.DataCatalog, query);
			WaitForQueryToComplete(amazonAthenaClient, queryExecutionId);
			GetQueryResultsRequest request = new GetQueryResultsRequest
			{
				QueryExecutionId = queryExecutionId
			};
			amazonAthenaClient.GetQueryResults(request);
		}
		ListEngineVersionsRequest request2 = new ListEngineVersionsRequest();
		amazonAthenaClient.ListEngineVersions(request2);
	}

	public ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder
		{
			ConnectionString = connectionStringBuilder()
		};
		try
		{
			AmazonAthenaConfig clientConfig = new AmazonAthenaConfig
			{
				RegionEndpoint = RegionEndpoint.GetBySystemName(dbConnectionStringBuilder["awsregion"].ToString())
			};
			AmazonAthenaClient client = new AmazonAthenaClient(dbConnectionStringBuilder["accesstoken"].ToString(), dbConnectionStringBuilder["secrettoken"].ToString(), clientConfig);
			AthenaConnectionWrapper athenaConnectionWrapper = new AthenaConnectionWrapper
			{
				Client = client
			};
			if (dbConnectionStringBuilder.ContainsKey("datacatalog"))
			{
				athenaConnectionWrapper.DataCatalog = dbConnectionStringBuilder["datacatalog"].ToString();
			}
			if (dbConnectionStringBuilder.ContainsKey("database"))
			{
				athenaConnectionWrapper.Database = dbConnectionStringBuilder["database"].ToString();
			}
			if (dbConnectionStringBuilder.ContainsKey("workgroup"))
			{
				athenaConnectionWrapper.WorkGroup = dbConnectionStringBuilder["workgroup"].ToString();
			}
			TestConnection(athenaConnectionWrapper);
			return new ConnectionResult(null, null, athenaConnectionWrapper);
		}
		catch (Exception ex)
		{
			return new ConnectionResult(ex, ex.Message);
		}
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}

	protected string GetVersionString(string connectionString)
	{
		return "Athena engine 2";
	}

	public bool ShouldSynchronizeComputedFormula(object connection)
	{
		return false;
	}

	public bool ShouldSynchronizeParameters(object connection)
	{
		return false;
	}

	public List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		throw new NotSupportedException();
	}

	public void CloseConnection(object connection)
	{
	}
}
