using System.Drawing;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class ApacheImpalaMetastoreSupport : HiveMetastoreSQLServerSupport, IDatabaseSupport, IDatabaseSupportShared
{
	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.ApacheImpalaMetastore;

	public override Image TypeImage => Resources.impala;

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Apache Impala (with Hive Metastore)";
	}
}
