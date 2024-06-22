using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class Dynamics365Connection : DataverseConnection
{
	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.Dynamics365;
	}
}
