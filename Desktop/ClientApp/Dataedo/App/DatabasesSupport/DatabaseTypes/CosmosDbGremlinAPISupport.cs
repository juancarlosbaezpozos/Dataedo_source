using System.Drawing;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class CosmosDbGremlinAPISupport : CosmosDbSqlSupport, IDatabaseSupport, IDatabaseSupportShared
{
	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.CosmosDbGremlin;

	public override Image TypeImage => Resources.gremlin_16;

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Azure Cosmos DB - Gremlin API";
	}
}
