using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.SSIS;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Base;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using Dataedo.SSISConnector;
using Dataedo.SSISConnector.DtsxParser;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class SSISSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => false;

	public bool CanFilterBySchema => false;

	public bool CanGetDatabases => false;

	public bool CanImportToCustomFields => false;

	public string ChooseObjectPageDescriptionText => "Choose which objects' comments to export to the database.";

	public string ExportToDatabaseButtonDescription => "Export comments to database";

	public string EmptyScriptMessage => "The script is not available.";

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool CanImportDependencies => false;

	public bool HasSslSettings => false;

	public bool IsSchemaType => false;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.SSIS;

	public bool ShouldForceFullReimport => true;

	public bool SupportsReadingExternalDependencies => false;

	public Image TypeImage => Resources.sqlserver;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool CanGenerateDDLScript => false;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.SSIS;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.SSIS;
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotImplementedException();
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return string.Empty;
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		return new List<string>();
	}

	public string GetDbmsVersion(object connection)
	{
		if (connection is Package)
		{
			return ".DTSX package file";
		}
		if (connection is Project)
		{
			return ".ISPAC project deployment file";
		}
		return "SSIS";
	}

	public List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		throw new NotImplementedException();
	}

	public FilterObjectTypeEnum.FilterObjectType[] GetFilterObjectTypes()
	{
		return new FilterObjectTypeEnum.FilterObjectType[1] { FilterObjectTypeEnum.FilterObjectType.Procedure };
	}

	public string GetFilterRuleConcatenation(string schemaFieldName, string nameFieldName)
	{
		throw new NotImplementedException();
	}

	public string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "SQL Server Integration Services (SSIS) (beta)";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		throw new NotImplementedException();
	}

	public DateTime? GetServerTime(object connection)
	{
		return DateTime.UtcNow;
	}

	public SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeSSIS(synchronizeParameters);
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		throw new NotImplementedException();
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		return new DatabaseVersionUpdate
		{
			Build = 1,
			Update = 1,
			Version = 1
		};
	}

	public ConnectionBase GetXmlConnectionModel()
	{
		return new SSISConnection();
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
		try
		{
			if (connectionStringBuilder.Target is DatabaseRow databaseRow)
			{
				object obj = null;
				if (Enum.TryParse<SSISConnectionTypeEnum.SSISConnectionType>(databaseRow.Param1, out var result))
				{
					switch (result)
					{
					case SSISConnectionTypeEnum.SSISConnectionType.DTSX:
					{
						if (!File.Exists(databaseRow.Param2))
						{
							throw new FileNotFoundException("File not found.");
						}
						Package package = new Package();
						string dtsxString = File.ReadAllText(databaseRow.Param2);
						package.LoadFromDtsx(dtsxString);
						obj = package;
						break;
					}
					case SSISConnectionTypeEnum.SSISConnectionType.ISPAC:
					{
						if (!File.Exists(databaseRow.Param2))
						{
							throw new FileNotFoundException("File not found.");
						}
						Project project = new Project();
						project.LoadFromDeploymentFile(databaseRow.Param2);
						obj = project;
						break;
					}
					default:
						obj = null;
						break;
					}
					return new ConnectionResult(null, null, obj);
				}
				throw new ArgumentException("Unknown SSISConnectionType value: " + databaseRow.Param1);
			}
		}
		catch (InvalidDataException exception)
		{
			return new ConnectionResult(exception, "Wrong file format!");
		}
		catch (Exception ex)
		{
			return new ConnectionResult(ex, ex.Message);
		}
		return new ConnectionResult(null, null);
	}

	public void CloseConnection(object connection)
	{
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		return true;
	}
}
