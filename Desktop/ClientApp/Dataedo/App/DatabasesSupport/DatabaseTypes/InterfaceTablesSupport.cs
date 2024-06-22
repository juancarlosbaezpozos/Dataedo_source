using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.InterfaceTables;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class InterfaceTablesSupport : SqlServerSupport, IDatabaseSupport, IDatabaseSupportShared
{
	public override bool CanImportDependencies => false;

	public override bool ShouldForceFullReimport => true;

	public override Image TypeImage => Resources.odbc;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.InterfaceTables;

	public override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.InterfaceTables;

	public override SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.InterfaceTables;
	}

	public override string GetConnectionString(string applicationName, string database, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string defaultAuthdb = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
	{
		return StaticData.DataedoConnectionString;
	}

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Interface Tables";
	}

	public override SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeInterfaceTables(synchronizeParameters);
	}

	public override DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.InterfaceTables;
	}

	public override ConnectionBase GetXmlConnectionModel()
	{
		return new InterfaceTablesConnection();
	}

	public override List<string> GetExtendedProperties(object connection, string host = null, List<string> schemas = null)
	{
		List<string> list = new List<string>();
		for (int i = 1; i <= 100; i++)
		{
			list.Add($"field{i}");
		}
		return list;
	}
}
