using System.Drawing;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class CosmosDbTableApiSupport : CosmosDbSqlSupport, IDatabaseSupport, IDatabaseSupportShared
{
	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.CosmosDbTable;

	public override Image TypeImage => Resources.related_tables_16;

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Azure Cosmos DB - Table API";
	}
}
