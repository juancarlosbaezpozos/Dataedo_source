using System.Drawing;
using Dataedo.App.Licences;
using Dataedo.App.Tools;

namespace Dataedo.App.Import.CloudStorage;

public class FileImportTypeObject
{
	public FileImportTypeEnum.FileImportType Value { get; private set; }

	public string DisplayName => FileImportTypeEnum.GetDisplayName(Value);

	public Image Image => FileImportTypeEnum.GetImage(Value);

	public string URL
	{
		get
		{
			CloudStorageTypeEnum.CloudStorageType? cloudStorageType = FileImportTypeEnum.ToCloudStorageType(Value);
			if (!cloudStorageType.HasValue)
			{
				return string.Empty;
			}
			if (!Connectors.HasCloudStorageTypeConnector(cloudStorageType))
			{
				return "<href=" + Links.ManageAccounts + ">(upgrade to connect)</href>";
			}
			return string.Empty;
		}
	}

	public FileImportTypeObject(FileImportTypeEnum.FileImportType fileImportType)
	{
		Value = fileImportType;
	}
}
