using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.PostgreSql;

namespace Dataedo.App.Data.AuroraPostgreSql;

internal class SynchronizeAuroraPostgreSql : SynchronizePostgreSql
{
	public SynchronizeAuroraPostgreSql(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}
}
