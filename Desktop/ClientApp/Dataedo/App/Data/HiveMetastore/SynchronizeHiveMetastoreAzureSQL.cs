using Dataedo.App.Classes.Synchronize;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.HiveMetastore;

internal class SynchronizeHiveMetastoreAzureSQL : SynchronizeHiveMetastoreSQLServer
{
	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreAzureSQL;

	public SynchronizeHiveMetastoreAzureSQL(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}
}
