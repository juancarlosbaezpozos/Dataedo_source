using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.Aurora;
using Dataedo.App.Data.General;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class AuroraSupport : MySqlSupport, IDatabaseSupport, IDatabaseSupportShared
{
	public override Image TypeImage => Resources.aurora;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Aurora;

	public override SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		SharedDatabaseTypeEnum.DatabaseType? forkType = GetForkType(connection);
		SharedDatabaseTypeEnum.DatabaseType databaseType = SharedDatabaseTypeEnum.DatabaseType.Aurora;
		if (!forkType.HasValue || forkType.Value != databaseType)
		{
			return GeneralQueries.AskUserWhichConnectorUse(forkType, databaseType, connection, owner);
		}
		return SharedDatabaseTypeEnum.DatabaseType.Aurora;
	}

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Amazon Aurora MySQL";
	}

	public override SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeAurora(synchronizeParameters);
	}

	public override DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.Aurora;
	}
}
