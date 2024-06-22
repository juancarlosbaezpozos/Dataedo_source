using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Data.General;
using Dataedo.App.Properties;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class AzureDataLakeStorageSupport : AzureStorageSupport
{
	public override Image TypeImage => Resources.azure_data_lake_storage_16;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.AzureDataLakeStorage;

	public override SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.AzureDataLakeStorage;
	}

	public override DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.AzureDataLakeStorage;
	}
}
