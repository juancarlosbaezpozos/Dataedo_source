using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.MariaDB;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class MariaDbSupport : MySqlSupport, IDatabaseSupport, IDatabaseSupportShared
{
	public override bool CanImportDependencies => false;

	public override Image TypeImage => Resources.mariadb;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.MariaDB;

	public override SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		SharedDatabaseTypeEnum.DatabaseType? forkType = GetForkType(connection);
		SharedDatabaseTypeEnum.DatabaseType databaseType = SharedDatabaseTypeEnum.DatabaseType.MariaDB;
		if (!forkType.HasValue || forkType.Value != databaseType)
		{
			return GeneralQueries.AskUserWhichConnectorUse(forkType, databaseType, connection, owner);
		}
		if (version >= base.VersionInfo.FirstNotSupportedVersion)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(GetNotSupportedText(withQuestion: true), "Unsupported version", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 1, owner);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
			{
				return null;
			}
		}
		return SharedDatabaseTypeEnum.DatabaseType.MariaDB;
	}

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "MariaDB";
	}

	public override SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeMariaDB(synchronizeParameters);
	}

	public override DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.MariaDB;
	}

	public override ConnectionBase GetXmlConnectionModel()
	{
		return new MariaDBConnection();
	}

	public override bool ShouldSynchronizeComputedFormula(object connection)
	{
		DatabaseVersionUpdate version = GetVersion(connection);
		if (version.Version <= 10)
		{
			if (version.Version == 10)
			{
				return version.Update >= 2;
			}
			return false;
		}
		return true;
	}

	protected override SharedDatabaseTypeEnum.DatabaseType? GetForkType(object connection)
	{
		SharedDatabaseTypeEnum.DatabaseType? forkType = base.GetForkType(connection);
		if (!forkType.HasValue)
		{
			string versionString = MySqlSupport.GetVersionString(connection);
			if (versionString != null && versionString.ToLower()?.Contains("mariadb") == true)
			{
				return SharedDatabaseTypeEnum.DatabaseType.MariaDB;
			}
		}
		return forkType;
	}
}
