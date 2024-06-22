using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.PerconaMySQL;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class PerconaMySqlSupport : MySqlSupport, IDatabaseSupport, IDatabaseSupportShared
{
	public override Image TypeImage => Resources.percona;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL;

	public override SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		SharedDatabaseTypeEnum.DatabaseType? forkType = GetForkType(connection);
		SharedDatabaseTypeEnum.DatabaseType databaseType = SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL;
		if (!forkType.HasValue || forkType.Value != databaseType)
		{
			return GeneralQueries.AskUserWhichConnectorUse(forkType, databaseType, connection);
		}
		if (version < base.VersionInfo.FirstSupportedVersion)
		{
			GeneralMessageBoxesHandling.Show(GetNotSupportedText(), "Unsupported version", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			return null;
		}
		if (version >= base.VersionInfo.FirstNotSupportedVersion || (version.Version > 5 && version.Version < 8))
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(GetNotSupportedText(withQuestion: true), "Unsupported version", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 1, owner);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
			{
				return null;
			}
		}
		return SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL;
	}

	protected override string GetSupportedVersionsText()
	{
		return $"5.6 to 5.7 and from 8.0.11 before {base.VersionInfo.FirstNotSupportedVersion}";
	}

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Percona Server for MySQL";
	}

	public override SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizePerconaMySQL(synchronizeParameters);
	}

	public override DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.PerconaMySQL;
	}

	public List<string> GetExtendedProperties(string connectionString)
	{
		throw new NotSupportedException();
	}
}
