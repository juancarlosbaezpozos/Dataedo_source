using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Azure;
using Azure.Identity;
using Azure.Storage;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Base;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Helpers.CloudStorage.AzureStorage;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Connections;
using Dataedo.App.Helpers.Extensions;
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

internal abstract class AzureStorageSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
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

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.AzureStorage;

	public bool ShouldForceFullReimport => false;

	public bool SupportsReadingExternalDependencies => false;

	public abstract Image TypeImage { get; }

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool CanGenerateDDLScript => false;

	public bool CanImportDependencies => false;

	public abstract SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null);

	public void CloseConnection(object connection)
	{
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
		return "VERSION";
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

	public abstract DatabaseType GetTypeForCommands(bool isFile);

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

	public ConnectionResult TryConnection(DatabaseRow databaseRow)
	{
		AzureStorageAuthentication.AzureStorageAuthenticationEnum? azureStorageAuthenticationEnum = AzureStorageAuthentication.FromString(databaseRow.Param1);
		RequestFailedException ex3 = default(RequestFailedException);
		try
		{
			return azureStorageAuthenticationEnum switch
			{
				AzureStorageAuthentication.AzureStorageAuthenticationEnum.AccessKey => TryConnectionAccessKey(databaseRow.Host, databaseRow.Password), 
				AzureStorageAuthentication.AzureStorageAuthenticationEnum.AzureADInteractive => TryConnectionAzureADInteractive(databaseRow.Host), 
				AzureStorageAuthentication.AzureStorageAuthenticationEnum.ConnectionString => TryConnectionAzureConnectionString(databaseRow.Password), 
				AzureStorageAuthentication.AzureStorageAuthenticationEnum.PublicContainer => TryConnectionAzurePublicContainer(databaseRow.Host, databaseRow.Param2), 
				AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureAccount => TryConnectionAzureSharedAccessSignatureAccount(databaseRow.Host, databaseRow.Password), 
				AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureDirectory => TryConnectionAzureSharedAccessSignatureDirectory(databaseRow.Host, databaseRow.Param2, databaseRow.Password), 
				AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureURL => TryConnectionAzureSharedAccessSignatureURL(databaseRow.Password), 
				_ => new ConnectionResult(null, "Unknown authentication method", failedConnection: true), 
			};
		}
		catch (RequestFailedException ex)
		{
			ParsedSASQuery parsedSASQuery = CheckSasExpiration(databaseRow, azureStorageAuthenticationEnum);
			if (parsedSASQuery != null && parsedSASQuery.ExpirationDate.HasValue && parsedSASQuery.ExpirationDate.Value < DateTime.Now)
			{
				return new ConnectionResult(ex, "Server failed to authenticate the request - SAS token expired.");
			}
			return new ConnectionResult(ex, ex.Message.GetFirstLine());
		}
		catch (AggregateException ex2) when (((Func<bool>)delegate
		{
			// Could not convert BlockContainer to single expression
			ex3 = ex2.InnerException as RequestFailedException;
			return ex3 != null;
		}).Invoke())
		{
			return new ConnectionResult(ex3, ex3.Message.GetFirstLine());
		}
		catch (Exception ex4)
		{
			return new ConnectionResult(ex4, ex4.Message);
		}
	}

	private static ParsedSASQuery CheckSasExpiration(DatabaseRow databaseRow, AzureStorageAuthentication.AzureStorageAuthenticationEnum? azureStorageAuth)
	{
		try
		{
			ParsedSASQuery result = null;
			switch (azureStorageAuth)
			{
			case AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureAccount:
			case AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureDirectory:
				result = new ParsedSASQuery(databaseRow.Password);
				break;
			case AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureURL:
				result = new ParsedSASQuery(new Uri(databaseRow.Password));
				break;
			}
			return result;
		}
		catch
		{
			return null;
		}
	}

	private static ConnectionResult TryConnectionAccessKey(string accountName, string accessKey)
	{
		return TryConnection(new AzureStorageBlobServiceConnection(new StorageSharedKeyCredential(accountName, accessKey), accountName));
	}

	private static ConnectionResult TryConnectionAzureADInteractive(string accountName)
	{
		return TryConnection(new AzureStorageBlobServiceConnection(new InteractiveBrowserCredential(), accountName));
	}

	private static ConnectionResult TryConnectionAzureConnectionString(string connectionString)
	{
		return TryConnection(new AzureStorageBlobServiceConnection(connectionString));
	}

	private static ConnectionResult TryConnectionAzurePublicContainer(string accountName, string containerName)
	{
		return TryConnection(new AzureStorageBlobContainerConnection(accountName, containerName));
	}

	private static ConnectionResult TryConnectionAzureSharedAccessSignatureAccount(string accountName, string sharedAccessSignature)
	{
		return TryConnection(new AzureStorageBlobServiceConnection(new AzureSasCredential(sharedAccessSignature), accountName));
	}

	private static ConnectionResult TryConnectionAzureSharedAccessSignatureDirectory(string accountName, string path, string sharedAccessSignature)
	{
		return TryConnection(AzureStorageConnection.CreateConnectionFromSASToken(sharedAccessSignature, accountName, path));
	}

	private static ConnectionResult TryConnectionAzureSharedAccessSignatureURL(string sasURL)
	{
		return TryConnection(AzureStorageConnection.CreateConnectionFromSASUri(new Uri(sasURL)));
	}

	private static ConnectionResult TryConnection(AzureStorageConnection connection)
	{
		if (!connection.TestConnection())
		{
			return new ConnectionResult(null, "Error", connection)
			{
				FailedConnection = true
			};
		}
		return new ConnectionResult(null, null, connection);
	}
}
