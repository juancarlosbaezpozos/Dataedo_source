using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.Teradata;
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
using Dataedo.CustomMessageBox;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using Teradata.Client.Provider;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class TeradataSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => false;

	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => false;

	public bool CanFilterBySchema => true;

	public bool CanGetDatabases => true;

	public bool CanImportToCustomFields => false;

	public string ChooseObjectPageDescriptionText => "Choose which objects' comments to export to the database.";

	public string ExportToDatabaseButtonDescription => "Export comments to database";

	public string EmptyScriptMessage => "The script is not available.";

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool HasSslSettings => false;

	public bool IsSchemaType => true;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Teradata;

	public bool ShouldForceFullReimport => false;

	public bool SupportsReadingExternalDependencies => false;

	public Image TypeImage => Resources.teradata;

	public LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Teradata;

	public bool CanGenerateDDLScript => true;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		if (version < base.VersionInfo.FirstSupportedVersion || version >= base.VersionInfo.FirstNotSupportedVersion)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(GetNotSupportedText(withQuestion: true), "Unsupported version", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 1, owner);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
			{
				return null;
			}
		}
		return SharedDatabaseTypeEnum.DatabaseType.Teradata;
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotSupportedException();
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return GetConnectionString(database, port, host, login, password, withDatabase);
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		try
		{
			List<string> list = new List<string>();
			using (TdConnection tdConnection = new TdConnection(connectionString))
			{
				tdConnection.Open();
				using TdCommand tdCommand = CommandsWithTimeout.Teradata("SELECT DATABASENAME AS \"name\" FROM DBC.DATABASESV ORDER BY DATABASENAME;", tdConnection);
				using TdDataReader tdDataReader = tdCommand?.ExecuteReader();
				while (tdDataReader != null && tdDataReader.Read())
				{
					if (tdDataReader[0] != null && !(tdDataReader[0] is DBNull))
					{
						list.Add(tdDataReader[0]?.ToString());
					}
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
			GeneralMessageBoxesHandling.Show(GetExceptionMessage(ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		return null;
	}

	public string GetDbmsVersion(object connection)
	{
		using (TdCommand tdCommand = CommandsWithTimeout.Teradata("SELECT * FROM DBC.DBCInfoV WHERE InfoKey = 'VERSION';", connection))
		{
			using TdDataReader tdDataReader = tdCommand.ExecuteReader();
			if (tdDataReader.Read())
			{
				return tdDataReader["InfoData"] as string;
			}
		}
		return null;
	}

	public IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		throw new NotSupportedException();
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
		return "Teradata";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		throw new NotSupportedException();
	}

	public DateTime? GetServerTime(object connection)
	{
		using (TdCommand tdCommand = CommandsWithTimeout.Teradata("SELECT TO_CHAR(CURRENT_TIMESTAMP,'YYYY-MM-DD HH:MI:SS') AS \"date\";", connection))
		{
			using TdDataReader tdDataReader = tdCommand.ExecuteReader();
			if (tdDataReader.Read())
			{
				return DateTime.ParseExact(tdDataReader["date"] as string, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
			}
		}
		return null;
	}

	public SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeTeradata(synchronizeParameters);
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.Teradata;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		string versionString = GetVersionString(connection);
		Match match = new Regex("([0-9]+\\.[0-9]+)\\..+", RegexOptions.IgnoreCase).Match(versionString);
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
		return new TeradataConnection();
	}

	public bool PrepareElements(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl, Form owner = null)
	{
		PrepareDatabasesData(databaseRow, ref elements, elementsButtonEdit, elementsCountLabelControl);
		List<string> databases = databaseRow.GetDatabases();
		for (int i = 0; i < databaseRow.Schemas.Count; i++)
		{
			string text = databaseRow.Schemas[i];
			if (databaseRow.Schemas.Count == 1 && string.IsNullOrEmpty(text))
			{
				continue;
			}
			string message = null;
			if (!ValidateDatabase(databaseRow.ConnectionString, text, ref message))
			{
				if (CustomMessageBoxForm.Show("Database \"" + text + "\" is invalid. " + Environment.NewLine + "Unable to continue with invalid names." + Environment.NewLine + Environment.NewLine + "Do you want to remove it from list?", "Database is invalid.", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, message, 1, owner) != DialogResult.Yes)
				{
					return false;
				}
				if (RemoveDatabaseFromList(databaseRow, ref elements, elementsButtonEdit, elementsCountLabelControl, text))
				{
					i--;
				}
			}
			else
			{
				if (databases.Contains(text))
				{
					continue;
				}
				switch (CustomMessageBoxForm.Show("Database \"" + text + "\" does not exist in " + databaseRow.Host + "." + Environment.NewLine + Environment.NewLine + "Do you want to remove it from list?", "Database is does not exist.", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, owner))
				{
				case DialogResult.Yes:
					if (RemoveDatabaseFromList(databaseRow, ref elements, elementsButtonEdit, elementsCountLabelControl, text))
					{
						i--;
					}
					break;
				case DialogResult.None:
				case DialogResult.Cancel:
					return false;
				}
			}
		}
		if (databaseRow.Schemas.Count > 0)
		{
			return databaseRow.Schemas.Any((string x) => !string.IsNullOrEmpty(x));
		}
		return false;
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

	public ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		TdConnection tdConnection = null;
		try
		{
			tdConnection = new TdConnection(connectionStringBuilder());
			tdConnection.Open();
		}
		catch (Exception ex)
		{
			return new ConnectionResult(ex, GetExceptionMessage(ex));
		}
		return new ConnectionResult(null, null, tdConnection);
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		return true;
	}

	private string GetExceptionMessage(Exception ex)
	{
		if (ex is TdException ex2)
		{
			IEnumerable<TdError> source = from TdError x in ex2.Errors
				group x by x.Number into x
				select x.First();
			if (source.Any((TdError x) => x.Number == 115025 || x.Number == 10061 || x.Number == 100002))
			{
				return "Unable to connect with server." + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine + Environment.NewLine, source.Select((TdError x) => x.Message));
			}
		}
		return ex.Message;
	}

	private string GetConnectionString(string database, int? port, string host, string login, string password, bool withDatabase = true)
	{
		if (!port.HasValue)
		{
			throw new ArgumentException("Specified port is not valid.");
		}
		return new TdConnectionStringBuilder
		{
			DataSource = host,
			PortNumber = port.Value,
			UserId = login,
			Password = password,
			AuthenticationMechanism = "TD2"
		}.ToString();
	}

	private string GetVersionString(object connection)
	{
		using (TdCommand tdCommand = CommandsWithTimeout.Teradata("SELECT * FROM DBC.DBCInfoV WHERE InfoKey = 'VERSION';", connection))
		{
			using TdDataReader tdDataReader = tdCommand.ExecuteReader();
			if (tdDataReader.Read())
			{
				return tdDataReader["InfoData"] as string;
			}
		}
		return null;
	}

	private bool RemoveDatabaseFromList(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl, string database)
	{
		if (databaseRow.Schemas.Remove(database))
		{
			PrepareDatabasesData(databaseRow, ref elements, elementsButtonEdit, elementsCountLabelControl);
			return true;
		}
		return false;
	}

	private void PrepareDatabasesData(DatabaseRow databaseRow, ref IEnumerable<string> elements, ButtonEdit elementsButtonEdit, LabelControl elementsCountLabelControl)
	{
		string name = DatabaseRow.PrepareSchemasList(databaseRow.Schemas);
		databaseRow.HasMultipleSchemas = databaseRow.Schemas.Count() > 1;
		databaseRow.UseDifferentSchema = true;
		databaseRow.Name = name;
		elements = new List<string>(databaseRow.Schemas ?? new List<string>());
		DatabaseSupportBase.SetElementsButtonEdit(elementsButtonEdit, elements);
		DatabaseSupportBase.SetElementsLabelControl(elementsCountLabelControl, elements);
	}

	public List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		throw new NotSupportedException();
	}

	public void CloseConnection(object connection)
	{
		if (connection is TdConnection tdConnection && tdConnection.State != 0)
		{
			tdConnection.Close();
			tdConnection.Dispose();
		}
	}

	public bool CheckIfQVCIEnabled(Func<string> connectionStringBuilder)
	{
		try
		{
			using TdConnection tdConnection = new TdConnection(connectionStringBuilder());
			tdConnection.Open();
			using TdCommand tdCommand = CommandsWithTimeout.Teradata("SELECT 1 FROM dbc.columnsqv WHERE tableName='columnsqv' AND columnName='columnName';", tdConnection);
			tdCommand.ExecuteScalar();
			return true;
		}
		catch (TdException ex)
		{
			if (ex.Errors.Cast<TdError>().All((TdError err) => err.Number == 9719))
			{
				return false;
			}
			throw;
		}
	}
}
