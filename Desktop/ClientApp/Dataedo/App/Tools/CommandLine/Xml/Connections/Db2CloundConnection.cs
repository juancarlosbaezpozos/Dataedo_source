using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class Db2CloundConnection : Db2Connection
{
	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.Db2Cloud;
	}
}
