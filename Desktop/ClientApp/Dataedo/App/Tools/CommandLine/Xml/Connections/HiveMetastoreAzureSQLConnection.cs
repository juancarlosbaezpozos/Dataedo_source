using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class HiveMetastoreAzureSQLConnection : HiveMetastoreSQLServerConnection
{
	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.HiveMetastoreAzureSQL;
	}
}
