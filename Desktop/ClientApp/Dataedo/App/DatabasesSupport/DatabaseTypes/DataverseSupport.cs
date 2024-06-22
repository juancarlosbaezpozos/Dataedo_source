using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.Dataverse;
using Dataedo.App.Data.General;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Base;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.Tools.Export;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class DataverseSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => true;

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

	public virtual PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Dataverse;

	public bool ShouldForceFullReimport => true;

	public bool SupportsReadingExternalDependencies => false;

	public virtual Image TypeImage => Resources.dataverse;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Dataverse;

	public virtual string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Microsoft Dataverse";
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.Dataverse;
	}

	public virtual SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeDataverse(synchronizeParameters);
	}

	private static void TestConnection(CrmServiceClient conn)
	{
		IOrganizationService organizationWebProxyClient = conn.OrganizationWebProxyClient;
		(organizationWebProxyClient ?? conn.OrganizationServiceProxy).Execute(new WhoAmIRequest());
	}

	public static ConnectionResult TryConnection(Func<string> connectionStringBuilder)
	{
		try
		{
			CrmServiceClient crmServiceClient = new CrmServiceClient(connectionStringBuilder());
			TestConnection(crmServiceClient);
			return new ConnectionResult(null, null, crmServiceClient);
		}
		catch (NullReferenceException exception)
		{
			return new ConnectionResult(exception, "Cannot connect to server. Please verify the provided info.");
		}
		catch (Exception ex)
		{
			return new ConnectionResult(ex, ex.Message);
		}
	}

	public virtual SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.Dataverse;
	}

	public ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		return TryConnection(connectionStringBuilder);
	}

	public static string GetConnectionString(string host, string login, string password, string appId, string redirectUri)
	{
		return new DbConnectionStringBuilder
		{
			{ "UserName", login },
			{ "Password", password },
			{ "Url", host },
			{ "AuthType", "OAuth" },
			{ "AppId", appId },
			{ "RedirectUri", redirectUri },
			{ "LoginPrompt", "Auto" }
		}?.ConnectionString;
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, Dataedo.Shared.Enums.AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return GetConnectionString(host, login, password, param3, param4);
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		return GetDatabases(connectionString, splashScreenManager, owner);
	}

	public static List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, Form owner = null)
	{
		return null;
	}

	public List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		return new List<string>();
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotSupportedException();
	}

	public string GetDbmsVersion(object connection)
	{
		return string.Empty;
	}

	public IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		throw new NotImplementedException();
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

	public string GetQueryForDataLength(string tableAlias)
	{
		throw new NotImplementedException();
	}

	public DateTime? GetServerTime(object connection)
	{
		return DateTime.Now;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		IOrganizationService obj = connection as IOrganizationService;
		RetrieveVersionRequest request = new RetrieveVersionRequest();
		int[] array = Array.ConvertAll(((RetrieveVersionResponse)obj.Execute(request)).Version.Split('.'), (string s) => int.Parse(s));
		return new DatabaseVersionUpdate
		{
			Build = array[0],
			Update = array[1],
			Version = array[2]
		};
	}

	public virtual ConnectionBase GetXmlConnectionModel()
	{
		return new DataverseConnection();
	}

	public bool PrepareElements(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl, Form owner = null)
	{
		return true;
	}

	public void ProcessException(Exception ex, string name, string warehouse, Form owner = null)
	{
		throw new NotImplementedException();
	}

	public bool ShouldSynchronizeComputedFormula(object connection)
	{
		return false;
	}

	public bool ShouldSynchronizeParameters(object connection)
	{
		return false;
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotImplementedException();
	}

	public void CloseConnection(object connection)
	{
	}
}
