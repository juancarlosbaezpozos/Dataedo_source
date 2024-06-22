using Dataedo.App.Classes.Synchronize;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.HiveMetastore;

internal class SynchronizeHiveMetastoreMariaDB : SynchronizeHiveMetastoreMySQL
{
	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMariaDB;

	public SynchronizeHiveMetastoreMariaDB(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}
}
