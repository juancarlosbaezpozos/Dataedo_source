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
using Dataedo.App.Data.Oracle;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Base;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.ExtendedPropertiesExport;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using Devart.Data.Oracle;
using Devart.Data.Universal;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class OracleSupport : DatabaseSupportBase, IDatabaseSupport, IDatabaseSupportShared
{
	public bool CanImportDependencies => true;

	public bool CanCreateImportCommand => true;

	public bool CanExportExtendedPropertiesOrComments => true;

	public bool CanFilterBySchema => true;

	public bool CanGetDatabases => true;

	public bool CanImportToCustomFields => false;

	public string ChooseObjectPageDescriptionText => "Choose which objects' comments to export to the database.";

	public string ExportToDatabaseButtonDescription => "Export comments to database";

	public string EmptyScriptMessage => "The script is not available. This could be caused by lack of privileges at the time of import.";

	public bool HasExtendedPropertiesExport => false;

	public bool HasImportUsingCustomFields => false;

	public bool HasSslSettings => false;

	public bool IsSchemaType => false;

	public PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Oracle;

	public bool ShouldForceFullReimport => false;

	public bool SupportsReadingExternalDependencies => false;

	public Image TypeImage => Resources.oracle;

	public LoadObjectTypeEnum TypeOfExportToDatabase => LoadObjectTypeEnum.OracleComments;

	public bool CanGenerateDDLScript => true;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Oracle;

	public SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		if (version < base.VersionInfo.FirstSupportedVersion)
		{
			GeneralMessageBoxesHandling.Show(GetNotSupportedText(), "Unsupported version", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			return null;
		}
		if (version >= base.VersionInfo.FirstNotSupportedVersion)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(GetNotSupportedText(withQuestion: true), "Unsupported version", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 1, owner);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
			{
				return null;
			}
		}
		return SharedDatabaseTypeEnum.DatabaseType.Oracle;
	}

	protected override string GetSupportedVersionsText()
	{
		return "9i R1 to 12c R2";
	}

	public void CloseConnection(object connection)
	{
		if (connection is UniConnection uniConnection && uniConnection.State != 0)
		{
			uniConnection.Close();
			uniConnection.Dispose();
		}
	}

	public void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		CommentsExporter commentsExporter = new CommentsExporter(exportWorker, connectionString, currentDatabaseId, chosenModules, new OracleCommentsExceptionHandler());
		commentsExporter.GetTypeNames(chooseObjectTypesUserControl.GetIncludedDbObjects());
		commentsExporter.ExportDescription(exportDescriptions: true, owner);
	}

	public string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return "Provider=Oracle;" + GetConnectionString(host, login, password, port, identifierName, isDirectConnect, isServiceName, isUnicode);
	}

	public List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		try
		{
			List<string> list = new List<string>();
			using (UniConnection uniConnection = new UniConnection(connectionString))
			{
				uniConnection.Open();
				using UniCommand uniCommand = CommandsWithTimeout.Oracle("SELECT USERNAME AS name\r\n                    FROM SYS.ALL_USERS\r\n                    WHERE USERNAME NOT IN \r\n                        ('SYSTEM', 'XDB', 'SYS', 'TSMSYS', 'MDSYS', 'EXFSYS', 'WMSYS', 'ORDSYS', 'OUTLN', 'DBSNMP')\r\n                    ORDER BY USERNAME", uniConnection);
				using UniDataReader uniDataReader = uniCommand?.ExecuteReader();
				while (uniDataReader != null && uniDataReader.Read())
				{
					if (uniDataReader[0] != null && !(uniDataReader[0] is DBNull))
					{
						list.Add(uniDataReader[0]?.ToString());
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
		using (UniCommand uniCommand = CommandsWithTimeout.Oracle("SELECT banner as version FROM V$VERSION where banner like 'Oracle%'", connection))
		{
			using UniDataReader uniDataReader = uniCommand.ExecuteReader();
			if (uniDataReader.Read())
			{
				return uniDataReader["version"] as string;
			}
		}
		return null;
	}

	public List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		throw new NotSupportedException();
	}

	public IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		CommentsExporter commentsExporter = new CommentsExporter(exportWorker, connectionString, currentDatabaseId, chosenModules, new OracleCommentsExceptionHandler());
		commentsExporter.GetTypeNames(chooseObjectTypesUserControl.GetIncludedDbObjects());
		commentsExporter.InitializeExportObjects();
		return commentsExporter.DescriptionObjects.Select((DBDescription x) => x.Command);
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

	public virtual string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Oracle";
	}

	public string GetQueryForDataLength(string tableAlias)
	{
		return "CASE\r\n                WHEN " + tableAlias + "DATA_TYPE = 'VARCHAR2'\r\n                        OR " + tableAlias + "DATA_TYPE = 'NVARCHAR2'\r\n                        OR " + tableAlias + "DATA_TYPE = 'CHAR'\r\n                        OR " + tableAlias + "DATA_TYPE = 'NCHAR' THEN\r\n                  CASE\r\n                  WHEN " + tableAlias + "CHAR_LENGTH IS NOT NULL THEN\r\n                    TO_CHAR(" + tableAlias + "CHAR_LENGTH) ||\r\n                    (\r\n                      CASE " + tableAlias + "CHAR_USED\r\n                      WHEN 'B' THEN\r\n                        ' BYTE'\r\n                      WHEN 'C' THEN\r\n                        ' CHAR'\r\n                      ELSE\r\n                        ''\r\n                      END)\r\n                  END\r\n                WHEN " + tableAlias + "DATA_TYPE = 'NUMBER' THEN\r\n                  CASE\r\n                  WHEN " + tableAlias + "DATA_SCALE IS NOT NULL THEN\r\n                    (\r\n                      CASE\r\n                      WHEN " + tableAlias + "DATA_PRECISION IS NULL THEN\r\n                        '38'\r\n                      ELSE\r\n                        TO_CHAR(" + tableAlias + "DATA_PRECISION)\r\n                      END) || ', ' || TO_CHAR(" + tableAlias + "DATA_SCALE)\r\n                  END\r\n                END DATA_LENGTH ";
	}

	public DateTime? GetServerTime(object connection)
	{
		using (UniCommand uniCommand = CommandsWithTimeout.Oracle("SELECT TO_CHAR (SYSDATE, 'MM-DD-YYYY HH24:MI:SS') value FROM DUAL", connection))
		{
			using UniDataReader uniDataReader = uniCommand.ExecuteReader();
			if (uniDataReader.Read())
			{
				return DateTime.ParseExact(uniDataReader["value"] as string, "MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
			}
		}
		return null;
	}

	public SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeOracle(synchronizeParameters);
	}

	public DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.Oracle;
	}

	public DatabaseVersionUpdate GetVersion(object connection)
	{
		Regex regex;
		Match match;
		try
		{
			regex = new Regex("(\\d+)\\.(\\d+)\\.(\\d+)");
			string versionString = GetVersionString(connection);
			if (!string.IsNullOrWhiteSpace(versionString))
			{
				match = regex.Match(versionString);
				if (match.Success && (true & int.TryParse(match.Groups[0].Value, out var result) & int.TryParse(match.Groups[1].Value, out var result2) & int.TryParse(match.Groups[2].Value, out var result3)))
				{
					return new DatabaseVersionUpdate
					{
						Version = result,
						Update = result2,
						Build = result3
					};
				}
			}
		}
		catch
		{
		}
		string bannerVersionString = GetBannerVersionString(connection);
		regex = new Regex("Release (\\d+)\\.(\\d+)\\.(\\d+)", RegexOptions.IgnoreCase);
		try
		{
			match = regex.Match(bannerVersionString);
			if (match.Success && (true & int.TryParse(match.Groups[1].Value, out var result4) & int.TryParse(match.Groups[2].Value, out var result5) & int.TryParse(match.Groups[3].Value, out var result6)))
			{
				return new DatabaseVersionUpdate
				{
					Version = result4,
					Update = result5,
					Build = result6
				};
			}
		}
		catch
		{
		}
		regex = new Regex("Release ([0-9]+)", RegexOptions.IgnoreCase);
		match = regex.Match(bannerVersionString);
		if (match.Success)
		{
			bannerVersionString = match.Groups[1].Value;
			if (int.TryParse(bannerVersionString, out var result7))
			{
				return new DatabaseVersionUpdate
				{
					Version = result7
				};
			}
		}
		return null;
	}

	public ConnectionBase GetXmlConnectionModel()
	{
		return new OracleConnection();
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
		return GetVersion(connection).Version >= 11;
	}

	public bool ShouldSynchronizeParameters(object connection)
	{
		return true;
	}

	public ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		if ("sys".Equals(user?.ToLower()))
		{
			return new ConnectionResult(new Exception(), "Dataedo does not support connection as SYS user." + Environment.NewLine + "Please connect as a regular user.");
		}
		UniConnection uniConnection = null;
		try
		{
			uniConnection = new UniConnection(connectionStringBuilder());
			uniConnection.Open();
		}
		catch (Exception ex)
		{
			return new ConnectionResult(ex, GetExceptionMessage(ex));
		}
		return new ConnectionResult(null, null, uniConnection);
	}

	public bool ValidateDatabase(string connectionString, string name, ref string message)
	{
		throw new NotSupportedException();
	}

	private string GetConnectionString(string host, string login, string password, int? port, string serviceName, bool isDirectConnect, bool isServiceName, bool isUnicode = true)
	{
		if (!port.HasValue)
		{
			throw new ArgumentException("Specified port is not valid.");
		}
		string value = (isUnicode ? "Unicode=true;" : string.Empty);
		if (isDirectConnect)
		{
			OracleConnectionStringBuilder oracleConnectionStringBuilder = new OracleConnectionStringBuilder();
			oracleConnectionStringBuilder.Direct = true;
			oracleConnectionStringBuilder.Server = host;
			oracleConnectionStringBuilder.Port = port.Value;
			if (isServiceName)
			{
				oracleConnectionStringBuilder.ServiceName = serviceName;
			}
			else
			{
				oracleConnectionStringBuilder.Sid = serviceName;
			}
			oracleConnectionStringBuilder.UserId = login;
			oracleConnectionStringBuilder.Password = password;
			oracleConnectionStringBuilder.Pooling = false;
			oracleConnectionStringBuilder.Unicode = isUnicode;
			return oracleConnectionStringBuilder.ToString();
		}
		string text = (isServiceName ? ("SERVICE_NAME=" + serviceName) : ("SID=" + serviceName));
		string text2 = $"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port})))" + "(CONNECT_DATA=(" + text + ")));";
		OracleConnectionStringBuilder oracleConnectionStringBuilder2 = new OracleConnectionStringBuilder
		{
			UserId = login,
			Password = password,
			Pooling = false
		};
		if (!string.IsNullOrEmpty(value))
		{
			oracleConnectionStringBuilder2.Unicode = isUnicode;
		}
		return text2 + oracleConnectionStringBuilder2.ToString();
	}

	private string GetExceptionMessage(Exception ex)
	{
		OracleException ex2 = (ex as OracleException) ?? (ex?.InnerException as OracleException);
		if (ex2 != null)
		{
			return ex2.Code switch
			{
				12545 => "The target host does not exist!", 
				1017 => "Invalid username/password!", 
				1005 => "Password cannot be empty.", 
				28009 => "Dataedo does not support connection as SYS user." + Environment.NewLine + "Please connect as a regular user.", 
				12170 => "Connection timeout. Check your network settings or connection data.", 
				_ => "Unable to connect with server.", 
			};
		}
		if (ex.Message.Contains("CanNotObtainOracleClientFromRegistry") || ex.InnerException is BadImageFormatException)
		{
			string text;
			string text2;
			if (Environment.Is64BitProcess)
			{
				text = "64";
				text2 = "32";
			}
			else
			{
				text = "32";
				text2 = "64";
			}
			return "You are running Dataedo (" + text + " bit)." + Environment.NewLine + "Installed Oracle client is invalid for Dataedo (" + text + " bit)." + Environment.NewLine + Environment.NewLine + "Use direct connection (Connection type/Direct) or if you have " + text2 + " bit Oracle client run Dataedo (" + text2 + " bit)." + Environment.NewLine + Environment.NewLine + "Visit <href=" + Links.OracleConnectionRequirementsUrl + ">dataedo.com</href> for more information.";
		}
		if (ex is OracleException && ex?.InnerException is NullReferenceException)
		{
			return "Unable to connect with server. Check your network settings or connection data.";
		}
		return "Unable to connect to server." + Environment.NewLine + Environment.NewLine + ex.Message;
	}

	private string GetBannerVersionString(object connection)
	{
		using (UniCommand uniCommand = CommandsWithTimeout.Oracle("select banner from v$version where banner like 'Oracle%'", connection))
		{
			using UniDataReader uniDataReader = uniCommand.ExecuteReader();
			if (uniDataReader.Read())
			{
				return uniDataReader["banner"] as string;
			}
		}
		return string.Empty;
	}

	private string GetVersionString(object connection)
	{
		string text = string.Empty;
		try
		{
			using UniCommand uniCommand = CommandsWithTimeout.Oracle("SELECT VERSION_FULL FROM V$INSTANCE", connection);
			using UniDataReader uniDataReader = uniCommand.ExecuteReader();
			if (uniDataReader.Read())
			{
				text = uniDataReader["VERSION_FULL"] as string;
			}
		}
		catch (Exception)
		{
		}
		if (string.IsNullOrWhiteSpace(text))
		{
			try
			{
				using UniCommand uniCommand2 = CommandsWithTimeout.Oracle("SELECT VERSION_FULL FROM PRODUCT_COMPONENT_VERSION", connection);
				using UniDataReader uniDataReader2 = uniCommand2.ExecuteReader();
				if (!uniDataReader2.Read())
				{
					return text;
				}
				text = uniDataReader2["VERSION_FULL"] as string;
				return text;
			}
			catch (Exception)
			{
				return text;
			}
		}
		return text;
	}
}
