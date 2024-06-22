using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.Azure;
using Dataedo.App.Data.General;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class AzureSqlDatabaseSupport : SqlServerSupport, IDatabaseSupport, IDatabaseSupportShared
{
	public override bool CanCreateImportCommand => true;

	public override Image TypeImage => Resources.azure;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.AzureSQLDatabase;

	public override SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		SharedDatabaseTypeEnum.DatabaseType? forkType = GetForkType(connection);
		SharedDatabaseTypeEnum.DatabaseType databaseType = SharedDatabaseTypeEnum.DatabaseType.AzureSQLDatabase;
		if (!forkType.HasValue || forkType.Value != databaseType)
		{
			return GeneralQueries.AskUserWhichConnectorUse(forkType, databaseType, connection, owner);
		}
		if (version < base.VersionInfo.FirstSupportedVersion || version >= base.VersionInfo.FirstNotSupportedVersion)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(GetNotSupportedText(withQuestion: true), "Unsupported version", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 1, owner);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
			{
				return null;
			}
		}
		return SharedDatabaseTypeEnum.DatabaseType.AzureSQLDatabase;
	}

	protected override string GetSupportedVersionsText()
	{
		return $"from {base.VersionInfo.FirstSupportedVersion} " + $"and before {base.VersionInfo.FirstNotSupportedVersion}";
	}

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Azure SQL Database";
	}

	public override SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeAzureSQLDatabase(synchronizeParameters);
	}

	public override DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.AzureSQLDatabase;
	}

	public override ConnectionBase GetXmlConnectionModel()
	{
		return new AzureSQLDatabaseConnection();
	}
}
