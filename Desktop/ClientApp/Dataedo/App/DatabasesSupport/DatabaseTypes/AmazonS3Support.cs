using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using Amazon;
using Amazon.Runtime;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Base;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Helpers.CloudStorage.AmazonS3;
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

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class AmazonS3Support : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => false;

	public bool CanCreateImportCommand => false;

	public bool CanExportExtendedPropertiesOrComments => false;

	public bool CanFilterBySchema => false;

	public bool CanGetDatabases => false;

	public bool CanImportToCustomFields => false;

	public string ChooseObjectPageDescriptionText
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public string ExportToDatabaseButtonDescription
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public string EmptyScriptMessage => string.Empty;

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool HasSslSettings => false;

	public bool IsSchemaType => false;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.AmazonS3;

	public bool ShouldForceFullReimport => false;

	public bool SupportsReadingExternalDependencies => false;

	public Image TypeImage => Resources.amazon_s3;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool CanGenerateDDLScript => false;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.AmazonS3;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.AmazonS3;
	}

	public void CloseConnection(object connection)
	{
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
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
		return "VERSION";
	}

	public List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		return new List<string>();
	}

	public IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		return new List<string>();
	}

	public FilterObjectTypeEnum.FilterObjectType[] GetFilterObjectTypes()
	{
		throw new NotImplementedException();
	}

	public string GetFilterRuleConcatenation(string schemaFieldName, string nameFieldName)
	{
		throw new NotImplementedException();
	}

	public string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return base.FriendlyDisplayName;
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		return string.Empty;
	}

	public DateTime? GetServerTime(object connection)
	{
		return DateTime.UtcNow;
	}

	public SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		throw new NotImplementedException();
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.AmazonS3;
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
		throw new NotSupportedException();
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
		throw new NotImplementedException();
	}

	public bool ShouldSynchronizeParameters(object connection)
	{
		return true;
	}

	public ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		return new ConnectionResult(null, "OK");
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotImplementedException();
	}

	public ConnectionResult TryConnection(string arnString, string accessKey, string secretKey)
	{
		AWSCredentials credentials = ((!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey)) ? ((AWSCredentials)new BasicAWSCredentials(accessKey, secretKey)) : ((AWSCredentials)new AnonymousAWSCredentials()));
		RegionEndpoint uSEast = RegionEndpoint.USEast1;
		AmazonS3Connection amazonS3Connection = new AmazonS3Connection(credentials, uSEast);
		try
		{
			Arn arn = Arn.Parse(arnString);
			HttpStatusCode httpStatusCode = amazonS3Connection.TestArn(arn);
			if (httpStatusCode != HttpStatusCode.OK)
			{
				return new ConnectionResult(null, "Error: " + httpStatusCode, amazonS3Connection);
			}
			return new ConnectionResult(null, null, amazonS3Connection);
		}
		catch (Exception ex)
		{
			return new ConnectionResult(ex, ex.Message);
		}
	}
}
