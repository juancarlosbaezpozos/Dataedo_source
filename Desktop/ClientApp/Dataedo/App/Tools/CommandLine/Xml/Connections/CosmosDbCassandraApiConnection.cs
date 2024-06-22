using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class CosmosDbCassandraApiConnection : CassandraConnection
{
	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.CosmosDbCassandra;
	}
}
