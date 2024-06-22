using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class CosmosDbSupport : CosmosDbSqlSupport, IDatabaseSupport, IDatabaseSupportShared
{
	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.CosmosDB;

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Azure Cosmos DB";
	}
}
