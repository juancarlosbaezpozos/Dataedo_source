using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.SapHana;
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
using Sap.Data.Hana;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class SapHanaSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => false;

	public bool CanFilterBySchema => true;

	public bool CanGetDatabases => true;

	public bool CanImportToCustomFields => false;

	public string ChooseObjectPageDescriptionText => "Export comments to database";

	public string ExportToDatabaseButtonDescription => "Export comments to database";

	public string EmptyScriptMessage => "The script is not available.";

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool HasSslSettings => false;

	public bool IsSchemaType => true;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.SapHana;

	public bool ShouldForceFullReimport => true;

	public bool SupportsReadingExternalDependencies => false;

	public Image TypeImage => Resources.sap;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool CanGenerateDDLScript => true;

	public bool CanImportDependencies => false;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.SapHana;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.SapHana;
	}

	public void CloseConnection(object connection)
	{
		if (connection is HanaConnection hanaConnection && hanaConnection.State != 0)
		{
			hanaConnection.Close();
			hanaConnection.Dispose();
		}
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotImplementedException();
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return GetConnectionString(host, login, password, port);
	}

	public string GetConnectionString(string host, string login, string password, int? port)
	{
		if (!port.HasValue)
		{
			throw new ArgumentException("Specified port is not valid.");
		}
		return new DbConnectionStringBuilder
		{
			{
				"Server",
				$"{host}:{port}"
			},
			{ "UID", login },
			{ "PWD", password },
			{ "encrypt", true },
			{ "sslValidateCertificate", true }
		}?.ConnectionString;
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		return null;
	}

	public string GetDbmsVersion(object connection)
	{
		using (HanaCommand hanaCommand = CommandsWithTimeout.SapHana("select version from sys.m_database;", connection))
		{
			using HanaDataReader hanaDataReader = hanaCommand.ExecuteReader();
			if (hanaDataReader.Read())
			{
				return hanaDataReader["version"] as string;
			}
		}
		return null;
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
		return "SAP HANA";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		throw new NotImplementedException();
	}

	public DateTime? GetServerTime(object connection)
	{
		using (HanaCommand hanaCommand = CommandsWithTimeout.SapHana("select current_timestamp from dummy;", connection))
		{
			using HanaDataReader hanaDataReader = hanaCommand.ExecuteReader();
			if (hanaDataReader.Read())
			{
				return hanaDataReader["current_timestamp"] as DateTime?;
			}
		}
		return null;
	}

	public SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeSapHana(synchronizeParameters);
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.SapHana;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		string dbmsVersion = GetDbmsVersion(connection);
		Match match = new Regex("([0-9]+\\.[0-9]+)", RegexOptions.IgnoreCase).Match(dbmsVersion);
		if (match.Success)
		{
			string[] array = match.Value.Split('.');
			if (int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2))
			{
				return new DatabaseVersionUpdate
				{
					Version = result,
					Update = result2
				};
			}
		}
		return null;
	}

	public ConnectionBase GetXmlConnectionModel()
	{
		return new SapHanaConnection();
	}

	public bool PrepareElements(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl, Form owner = null)
	{
		return true;
	}

	public void ProcessException(Exception ex, string name, string warehouse, Form owner = null)
	{
		GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
	}

	public bool ShouldSynchronizeComputedFormula(object connection)
	{
		return false;
	}

	public bool ShouldSynchronizeParameters(object connection)
	{
		return true;
	}

	public ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		HanaConnection hanaConnection = null;
		try
		{
			hanaConnection = new HanaConnection(connectionStringBuilder());
			hanaConnection.Open();
		}
		catch (HanaException ex)
		{
			if (ex.Errors.Cast<HanaError>().Any((HanaError x) => x.Message.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) != -1 || x.Message.IndexOf("connection failed", StringComparison.OrdinalIgnoreCase) != -1))
			{
				string message = "Connection failed. Make sure provided host and port values are correct.";
				return new ConnectionResult(ex, message);
			}
			return new ConnectionResult(ex, ex.Message);
		}
		catch (Exception ex2)
		{
			return new ConnectionResult(ex2, ex2.Message);
		}
		return new ConnectionResult(null, null, hanaConnection);
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}
}
