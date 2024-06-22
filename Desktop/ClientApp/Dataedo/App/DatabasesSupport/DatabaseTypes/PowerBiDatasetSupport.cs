using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.PowerBiDataset;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class PowerBiDatasetSupport : SsasTabularSupport, IPerspectiveDatabase
{
	public override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.PowerBiDataset;

	public override Image TypeImage => Resources.power_bi_dataset_16;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.PowerBiDataset;

	public override SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.PowerBiDataset;
	}

	public override SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizePowerBiDataset(synchronizeParameters);
	}

	public override ConnectionBase GetXmlConnectionModel()
	{
		return new PowerBiDatasetConnection();
	}

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Power BI Premium Workspace/Dataset";
	}
}
