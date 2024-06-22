using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class HiveMetastoreOracleSupport : OracleSupport, IDatabaseSupport, IDatabaseSupportShared
{
	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreOracle;

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Hive Metastore - Oracle";
	}
}
