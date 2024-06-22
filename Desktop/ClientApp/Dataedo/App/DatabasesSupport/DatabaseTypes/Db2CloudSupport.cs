using System.Windows.Forms;
using Dataedo.App.Data.General;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class Db2CloudSupport : Db2Support
{
	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Db2Cloud;

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "IBM Db2 Cloud";
	}

	public override SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
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
		return SharedDatabaseTypeEnum.DatabaseType.Db2Cloud;
	}
}
