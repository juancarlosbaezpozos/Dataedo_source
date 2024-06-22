using System.Drawing;
using Dataedo.App.Licences;
using Dataedo.App.Tools;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Import.CloudStorage;

public class CloudStorageTypeObject
{
	public CloudStorageTypeEnum.CloudStorageType Value { get; private set; }

	public string DisplayName => CloudStorageTypeEnum.GetDisplayName(Value);

	public Image Image => CloudStorageTypeEnum.GetImage(Value);

	public string URL
	{
		get
		{
			if (!Connectors.HasCloudStorageTypeConnector(Value))
			{
				return "<href=" + Links.ManageAccounts + ">(upgrade to connect)</href>";
			}
			return "";
		}
	}

	public SharedDatabaseTypeEnum.DatabaseType DatabaseType => CloudStorageTypeEnum.ToDatabaseType(Value);

	public CloudStorageTypeObject(CloudStorageTypeEnum.CloudStorageType cloudStorageType)
	{
		Value = cloudStorageType;
	}
}
