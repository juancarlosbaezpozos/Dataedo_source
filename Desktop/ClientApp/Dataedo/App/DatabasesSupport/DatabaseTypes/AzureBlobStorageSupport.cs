using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Data.General;
using Dataedo.App.Properties;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class AzureBlobStorageSupport : AzureStorageSupport
{
	public override Image TypeImage => Resources.azure_blob_storage_16;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.AzureBlobStorage;

	public override SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.AzureBlobStorage;
	}

	public override DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.AzureBlobStorage;
	}
}
