using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class PowerBiDatasetConnection : SsasTabularConnection
{
	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.PowerBiDataset;
	}
}
