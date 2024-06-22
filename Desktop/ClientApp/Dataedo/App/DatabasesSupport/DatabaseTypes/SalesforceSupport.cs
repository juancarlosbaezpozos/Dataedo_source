using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.Salesforce;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Base;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Enums;
using Dataedo.App.Forms.Salesforce;
using Dataedo.App.Helpers.Extensions;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Data.Commands.Enums;
using Dataedo.SalesforceConnector;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using Salesforce.Common;
using Salesforce.Force;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class SalesforceSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => false;

	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => false;

	public bool CanFilterBySchema => false;

	public bool CanGetDatabases => false;

	public bool CanImportToCustomFields => false;

	public string ChooseObjectPageDescriptionText => "Choose which objects' comments to export to the database.";

	public string ExportToDatabaseButtonDescription => "Export comments to database";

	public string EmptyScriptMessage => "The script is not available. This could be caused by lack of privileges at the time of import.";

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool HasSslSettings => false;

	public bool IsSchemaType => false;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Salesforce;

	public bool ShouldForceFullReimport => false;

	public bool SupportsReadingExternalDependencies => false;

	public Image TypeImage => Resources.salesforce_16;

	public LoadObjectTypeEnum TypeOfExportToDatabase => LoadObjectTypeEnum.ExtendedPropertiesObjects;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Salesforce;

	public bool CanGenerateDDLScript => false;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.Salesforce;
	}

	public void CloseConnection(object connection)
	{
		SalesforceManager.DisconnectAsync();
	}

	public virtual void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotImplementedException();
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return GetConnectionString(password);
	}

	private string GetConnectionString(string password)
	{
		return password;
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		return null;
	}

	public string GetDbmsVersion(object connection)
	{
		return "1";
	}

	public List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		throw new NotSupportedException();
	}

	public IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		throw new NotSupportedException();
	}

	public FilterObjectTypeEnum.FilterObjectType[] GetFilterObjectTypes()
	{
		return new FilterObjectTypeEnum.FilterObjectType[1] { FilterObjectTypeEnum.FilterObjectType.Table };
	}

	public string GetFilterRuleConcatenation(string schemaFieldName, string nameFieldName)
	{
		throw new NotImplementedException();
	}

	public string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Salesforce";
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
		return new SynchronizeSalesforce(synchronizeParameters);
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.Salesforce;
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
		return new SalesforceConnection();
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
			object target = connectionStringBuilder.Target;
			DatabaseRow databaseRow = target as DatabaseRow;
			if (databaseRow != null)
			{
				if (databaseRow.Param3 == SalesforceConnectionTypeEnum.TypeToString(SalesforceConnectionTypeEnum.SalesforceConnectionType.Interactive))
				{
					SalesforceInteractiveSignInForm salesforceInteractiveSignInForm = new SalesforceInteractiveSignInForm(databaseRow.Param4 == "true");
					salesforceInteractiveSignInForm.ShowDialog();
					AuthenticationClient auth = salesforceInteractiveSignInForm.GetAuthenticationClient();
					if (auth == null)
					{
						return new ConnectionResult(null, "Authentication failed or access denied.", failedConnection: true);
					}
					if (!databaseRow.Id.HasValue)
					{
						databaseRow.Name = auth.InstanceUrl;
						databaseRow.Title = auth.InstanceUrl;
					}
					ForceClient result = Task.Run(() => TryConnectionAsync(auth)).Result;
					return new ConnectionResult(null, null, result);
				}
				ForceClient result2 = Task.Run(() => TryConnectionAsync(databaseRow.Host, databaseRow.User, databaseRow.Password, databaseRow.Param1, databaseRow.Param2)).Result;
				return new ConnectionResult(null, null, result2);
			}
		}
		catch (AggregateException ex)
		{
			return new ConnectionResult(ex, ex.GetInnerExceptionsMessages());
		}
		catch (Exception ex2)
		{
			return new ConnectionResult(ex2, ex2.Message);
		}
		return new ConnectionResult(null, null);
	}

	public async Task<ForceClient> TryConnectionAsync(string host, string username, string password, string consumerKey, string consumerSecretKey)
	{
		AuthenticationClient auth = new AuthenticationClient();
		SimpleAES simpleAES = new SimpleAES();
		await auth.UsernamePasswordAsync(simpleAES.DecryptString(consumerKey), simpleAES.DecryptString(consumerSecretKey), username, password);
		await auth.GetLatestVersionAsync();
		ForceClient client = new ForceClient(host, auth.AccessToken, auth.ApiVersion);
		await client.RecentAsync<object>(1);
		return client;
	}

	public async Task<ForceClient> TryConnectionAsync(AuthenticationClient auth)
	{
		await auth.GetLatestVersionAsync();
		ForceClient client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);
		await client.RecentAsync<object>(1);
		return client;
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}
}
