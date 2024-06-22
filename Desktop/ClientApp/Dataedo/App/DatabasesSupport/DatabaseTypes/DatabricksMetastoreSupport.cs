using System.Drawing;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class DatabricksMetastoreSupport : HiveMetastoreSQLServerSupport, IDatabaseSupport, IDatabaseSupportShared
{
	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.DatabricksMetastore;

	public override Image TypeImage => Resources.databricks;

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Databricks (with external Hive Metastore)";
	}
}
