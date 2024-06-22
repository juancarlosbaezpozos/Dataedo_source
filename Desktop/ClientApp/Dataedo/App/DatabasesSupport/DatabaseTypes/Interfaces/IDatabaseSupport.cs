using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
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

namespace Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;

public interface IDatabaseSupport : IDatabaseSupportShared
{
	IDatabaseSupportShared BasicData { get; }

	bool CanCreateImportCommand { get; }

	bool CanExportExtendedPropertiesOrComments { get; }

	bool CanFilterBySchema { get; }

	bool CanGetDatabases { get; }

	bool CanImportToCustomFields { get; }

	string ChooseObjectPageDescriptionText { get; }

	string ExportToDatabaseButtonDescription { get; }

	string EmptyScriptMessage { get; }

	bool HasExtendedPropertiesExport { get; }

	bool HasImportUsingCustomFields { get; }

	bool CanImportDependencies { get; }

	bool HasSslSettings { get; }

	bool IsSchemaType { get; }

	PanelTypeEnum.PanelType PanelType { get; }

	bool ShouldForceFullReimport { get; }

	bool SupportsReadingExternalDependencies { get; }

	Image TypeImage { get; }

	LoadObjectTypeEnum TypeOfExportToDatabase { get; }

	bool CanGenerateDDLScript { get; }

	SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null);

	void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null);

	string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null);

	List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null);

	string GetDbmsVersion(object connection);

	IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl);

	FilterObjectTypeEnum.FilterObjectType[] GetFilterObjectTypes();

	string GetFilterRuleConcatenation(string schemaFieldName, string nameFieldName);

	string GetFriendlyDisplayNameForImport(bool isLite);

	string GetQueryForDataLength(string tableAlias);

	DateTime? GetServerTime(object connection);

	SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters);

	DatabaseType GetTypeForCommands(bool isFile);

	DatabaseVersionUpdate GetVersion(object connection);

	ConnectionBase GetXmlConnectionModel();

	bool PrepareElements(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl, Form owner = null);

	void ProcessException(Exception ex, string name, string warehouse, Form owner = null);

	bool ShouldSynchronizeComputedFormula(object connection);

	bool ShouldSynchronizeParameters(object connection);

	ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false);

	bool ValidateDatabase(string connectionString, string name, ref string message);

	List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null);

	void CloseConnection(object connection);
}
