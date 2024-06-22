using Dataedo.App.DatabasesSupport;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.QueryTools;

public class QueryDataTypes
{
	public static string GetQueryForDataLength(SharedDatabaseTypeEnum.DatabaseType? databaseType, string tableAlias = null)
	{
		return DatabaseSupportFactory.GetDatabaseSupport(databaseType).GetQueryForDataLength(tableAlias);
	}
}
