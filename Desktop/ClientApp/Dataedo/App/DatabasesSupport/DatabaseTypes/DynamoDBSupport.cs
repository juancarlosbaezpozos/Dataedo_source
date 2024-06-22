using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Amazon;
using Amazon.DynamoDBv2;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.DynamoDB;
using Dataedo.App.Data.General;
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
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class DynamoDBSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => false;

	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => false;

	public bool CanFilterBySchema => false;

	public bool CanGetDatabases => false;

	public bool CanImportToCustomFields => false;

	public string ChooseObjectPageDescriptionText => "Choose which objects' comments to export to the database.";

	public string ExportToDatabaseButtonDescription => "Export descriptions/custom fields to database (extended properties)";

	public string EmptyScriptMessage => "The script is not available.";

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool HasSslSettings => false;

	public bool IsSchemaType => false;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.DynamoDB;

	public bool ShouldForceFullReimport => true;

	public bool SupportsReadingExternalDependencies => false;

	public Image TypeImage => Resources.dynamodb_16;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public bool CanGenerateDDLScript => false;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.DynamoDB;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.DynamoDB;
	}

	public void CloseConnection(object connection)
	{
		if (connection is AmazonDynamoDBClient amazonDynamoDBClient)
		{
			amazonDynamoDBClient.Dispose();
		}
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotSupportedException();
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return string.Empty;
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		throw new NotSupportedException();
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
		throw new NotSupportedException();
	}

	public string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Amazon DynamoDB";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		throw new NotSupportedException();
	}

	public DateTime? GetServerTime(object connection)
	{
		return DateTime.UtcNow;
	}

	public SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeDynamoDB(synchronizeParameters);
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.DynamoDB;
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
		return new DynamoDBConnection();
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
				AmazonDynamoDBClient amazonDynamoDBClient = new AmazonDynamoDBClient(databaseRow.User, databaseRow.Password, RegionEndpoint.GetBySystemName(databaseRow.Host));
				amazonDynamoDBClient.ListTables(1);
				return new ConnectionResult(null, null, amazonDynamoDBClient);
			}
		}
		catch (AmazonDynamoDBException ex)
		{
			string message = ex.Message;
			if (ex.ErrorCode.Equals("InvalidSignatureException") || ex.ErrorCode.Equals("UnrecognizedClientException"))
			{
				message = "Invalid Access Key or Secret Key provided!";
			}
			return new ConnectionResult(ex, message);
		}
		catch (Exception ex2)
		{
			return new ConnectionResult(ex2, ex2.Message);
		}
		return new ConnectionResult(null, null);
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}
}
