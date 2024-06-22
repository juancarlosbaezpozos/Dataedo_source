using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.Dynamics365;
using Dataedo.App.Data.General;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class Dynamics365Support : DataverseSupport
{
	public override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Dataverse;

	public override Image TypeImage => Resources.dynamics;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Dynamics365;

	public override SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.Dynamics365;
	}

	public override SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeDynamics365(synchronizeParameters);
	}

	public override ConnectionBase GetXmlConnectionModel()
	{
		return new Dynamics365Connection();
	}

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Microsoft Dynamics 365 with Dataverse";
	}
}
