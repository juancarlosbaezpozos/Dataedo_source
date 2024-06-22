using System.Drawing;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class ApacheHiveMetastoreSupport : HiveMetastoreSQLServerSupport, IDatabaseSupport, IDatabaseSupportShared
{
	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.ApacheHiveMetastore;

	public override Image TypeImage => Resources.hive_16;

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Apache Hive (with Hive Metastore)";
	}
}
