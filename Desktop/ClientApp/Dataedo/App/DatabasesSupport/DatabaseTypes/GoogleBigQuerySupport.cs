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
using Dataedo.App.Data.GoogleBigQuery;
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
using Dataedo.Model.Extensions;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using Google.Apis.Auth.OAuth2;
using Google.Apis.CloudResourceManager.v1;
using Google.Apis.CloudResourceManager.v1.Data;
using Google.Apis.Services;
using Google.Cloud.BigQuery.V2;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class GoogleBigQuerySupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => false;

	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => false;

	public bool CanFilterBySchema => true;

	public bool CanGetDatabases => true;

	public bool CanImportToCustomFields => true;

	public string ChooseObjectPageDescriptionText => "Choose which objects' comments to export to the database.";

	public string ExportToDatabaseButtonDescription => "Export comments to database";

	public string EmptyScriptMessage => "Script not available";

	public bool HasExtendedPropertiesExport => true;

	public bool HasImportUsingCustomFields => true;

	public bool HasSslSettings => false;

	public bool IsSchemaType => false;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.GoogleBigQuery;

	public bool ShouldForceFullReimport => false;

	public bool SupportsReadingExternalDependencies => false;

	public Image TypeImage => Resources.google_big_query;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.GoogleBigQuery;

	public bool CanGenerateDDLScript => true;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version)
	{
		return SharedDatabaseTypeEnum.DatabaseType.GoogleBigQuery;
	}

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.GoogleBigQuery;
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotSupportedException();
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return login.Replace("\"", string.Empty);
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		try
		{
			List<string> list = new List<string>();
			GoogleCredential credential = GoogleCredential.FromFile(connectionString);
			using (BigQueryClient bigQueryClient = BigQueryClient.Create(name, credential))
			{
				foreach (BigQueryDataset item in bigQueryClient.ListDatasets())
				{
					list.Add(item.Reference.DatasetId);
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
		return "1";
	}

	public IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		throw new NotImplementedException();
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
		return "Google BigQuery";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		throw new NotImplementedException();
	}

	public DateTime? GetServerTime(object connection)
	{
		throw new NotSupportedException();
	}

	public DateTime? GetServerTime(object connection, string project)
	{
		string command = "SELECT FORMAT_TIMESTAMP(\"%F %T\",CURRENT_TIMESTAMP()) as date;";
		BigQueryResults source = CommandsWithTimeout.GoogleBigQuery(connection, command);
		if (source.Count() > 0)
		{
			return DateTime.ParseExact(source.FirstOrDefault()["date"] as string, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
		}
		return null;
	}

	public SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeGoogleBigQuery(synchronizeParameters);
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.GoogleBigQuery;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		return new DatabaseVersionUpdate
		{
			Version = 1,
			Update = 1
		};
	}

	public ConnectionBase GetXmlConnectionModel()
	{
		return new GoogleBigQueryConnection();
	}

	public bool PrepareElements(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl, Form owner = null)
	{
		List<string> schemas = databaseRow.Schemas;
		List<string> databases = GetDatabases(databaseRow.GetConnectionString(), null, databaseRow.Host, null);
		if (databases == null)
		{
			return false;
		}
		foreach (string item in schemas)
		{
			if (!databases.Contains(item.Trim()))
			{
				GeneralMessageBoxesHandling.Show("Provided dataset does not exist in the database", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
				return false;
			}
		}
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

	public List<string> GetProjectsId(string credentialPath, SplashScreenManager splashScreenManager, Form owner = null)
	{
		try
		{
			GoogleCredential httpClientInitializer = GoogleCredential.FromFile(credentialPath);
			ListProjectsResponse listProjectsResponse = new ProjectsResource.ListRequest(new CloudResourceManagerService(new BaseClientService.Initializer
			{
				HttpClientInitializer = httpClientInitializer
			})).Execute();
			if (listProjectsResponse.Projects != null)
			{
				return listProjectsResponse.Projects.Select((Project x) => x.ProjectId).ToList();
			}
			return new List<string>();
		}
		catch (Exception ex)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			ProcessException(ex, null, null, owner);
			return new List<string>();
		}
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}

	public ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		BigQueryClient connection;
		try
		{
			GoogleCredential credential = GoogleCredential.FromFile(connectionStringBuilder());
			connection = BigQueryClient.Create(name, credential);
		}
		catch (Exception ex)
		{
			return new ConnectionResult(ex, ex.Message);
		}
		return new ConnectionResult(null, null, connection);
	}

	public List<string> GetExtendedProperties(object connection, string host, List<string> schemas)
	{
		List<string> list = new List<string>();
		foreach (string schema in schemas)
		{
			string command = "SELECT tsl.option_value\r\n                        FROM `" + host + "`.`" + schema.Trim() + "`.INFORMATION_SCHEMA.TABLE_OPTIONS as tsl\r\n                        WHERE option_name = 'labels'";
			foreach (BigQueryRow item in CommandsWithTimeout.GoogleBigQuery(connection, command))
			{
				string text = item.Field<string>("option_value");
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				string text2 = text.Replace("STRUCT", "").Replace("[", "").Replace("]", "")
					.Replace(" ", "");
				foreach (KeyValuePair<string, string> item2 in (from value in text2.Substring(0, text2.LastIndexOf(")")).Replace("),", ")").Split(')')
					select value.Replace("\"", "").Replace("(", "").Replace(")", "") into value
					select value.Split(',')).ToDictionary((string[] pair) => pair[0], (string[] pair) => pair[1]))
				{
					if (!list.Contains(item2.Key))
					{
						list.Add(item2.Key);
					}
				}
			}
		}
		return list;
	}

	public bool ValidateCredentialPath(string path, Form owner = null)
	{
		try
		{
			GoogleCredential.FromFile(path);
			return true;
		}
		catch (Exception ex)
		{
			ProcessException(ex, null, null, owner);
			return false;
		}
	}

	public void CloseConnection(object connection)
	{
		CommandsWithTimeout.GoogleBigQuery(connection)?.Dispose();
	}
}
