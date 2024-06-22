using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.IBMDb2BigQuery;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class Db2BigQuerySupport : Db2Support, IDatabaseSupport, IDatabaseSupportShared
{
	public override bool CanExportExtendedPropertiesOrComments => false;

	public override string ChooseObjectPageDescriptionText
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override string ExportToDatabaseButtonDescription
	{
		get
		{
			string text = SharedDatabaseTypeEnum.TypeToStringForDisplay(base.SupportedDatabaseType);
			string text2 = (string.IsNullOrWhiteSpace(text) ? "this database type" : text);
			return "<color=192, 192, 192>Export comments to database (not available for " + text2 + ")</color>";
		}
	}

	public override Image TypeImage => Resources.ibm_db2_big_sql;

	public override LoadObjectTypeEnum TypeOfExportToDatabase
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.IBMDb2BigQuery;

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
		return SharedDatabaseTypeEnum.DatabaseType.IBMDb2BigQuery;
	}

	public override void ExportExtendedPropertiesOrComments(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl, Form owner = null)
	{
		throw new NotSupportedException();
	}

	public override IEnumerable<string> GetExtendedPropertiesOrCommentsScriptLines(BackgroundProcessingWorker exportWorker, string connectionString, int currentDatabaseId, List<int> chosenModules, ChooseCustomFieldsUserControl chooseCustomFieldsUserControl, ChooseObjectTypesUserControl chooseObjectTypesUserControl)
	{
		throw new NotSupportedException();
	}

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "IBM Db2 Big SQL";
	}

	public override SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeIBMDb2BigQuery(synchronizeParameters);
	}

	public override DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.IBMDB2BigQuery;
	}
}
