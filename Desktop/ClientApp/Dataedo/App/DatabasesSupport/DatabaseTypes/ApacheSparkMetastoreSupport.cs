using System.Drawing;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class ApacheSparkMetastoreSupport : HiveMetastoreSQLServerSupport, IDatabaseSupport, IDatabaseSupportShared
{
	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.ApacheSparkMetastore;

	public override Image TypeImage => Resources.spark;

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Apache Spark (with Hive Metastore)";
	}
}
