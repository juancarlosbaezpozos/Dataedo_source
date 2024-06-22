using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
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

internal class ManualDatabaseSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => false;

	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => false;

	public bool CanFilterBySchema => true;

	public bool CanGetDatabases => false;

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

	public string EmptyScriptMessage => string.Empty;

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public bool HasSslSettings => false;

	public bool IsSchemaType => true;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.None;

	public bool ShouldForceFullReimport
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public bool SupportsReadingExternalDependencies => false;

	public Image TypeImage => Resources.server_16;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Manual;

	public bool CanGenerateDDLScript => true;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.Manual;
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotSupportedException();
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		throw new NotSupportedException();
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		throw new NotSupportedException();
	}

	public string GetDbmsVersion(object connection)
	{
		throw new NotSupportedException();
	}

	public IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		throw new NotSupportedException();
	}

	public FilterObjectTypeEnum.FilterObjectType[] GetFilterObjectTypes()
	{
		throw new NotSupportedException();
	}

	public string GetFilterRuleConcatenation(string schemaFieldName, string nameFieldName)
	{
		throw new NotSupportedException();
	}

	public string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Manual";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		throw new NotSupportedException();
	}

	public DateTime? GetServerTime(object connection)
	{
		return DateTime.Now;
	}

	public SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		throw new NotSupportedException();
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		throw new NotSupportedException();
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		throw new NotSupportedException();
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
		GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
	}

	public bool ShouldSynchronizeComputedFormula(object connection)
	{
		throw new NotSupportedException();
	}

	public bool ShouldSynchronizeParameters(object connection)
	{
		throw new NotSupportedException();
	}

	public ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		return new ConnectionResult(null, "Wrong database's type", failedConnection: true);
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}

	public List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		throw new NotSupportedException();
	}

	public void CloseConnection(object connection)
	{
	}
}
