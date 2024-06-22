using Dataedo.App.Import.CloudStorage;
using Dataedo.App.Import.DataLake;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Licences;

internal static class Connectors
{
	public static bool HasDatabaseTypeConnector(SharedDatabaseTypeEnum.DatabaseType? databaseType)
	{
		if (!databaseType.HasValue)
		{
			return false;
		}
		return StaticData.License.HasDatabaseTypeConnector(databaseType.Value);
	}

	public static bool HasDataLakeTypeConnector(DataLakeTypeEnum.DataLakeType? dataLakeType)
	{
		if (!dataLakeType.HasValue)
		{
			return false;
		}
		return StaticData.License.HasDataLakeTypeConnector(dataLakeType.Value);
	}

	public static bool HasCloudStorageTypeConnector(CloudStorageTypeEnum.CloudStorageType? cloudStorageType)
	{
		if (!cloudStorageType.HasValue)
		{
			return false;
		}
		if (StaticData.License == null)
		{
			return false;
		}
		return StaticData.License.HasCloudStorageTypeConnector(cloudStorageType.Value);
	}

	public static bool HasAllConnectors()
	{
		if (StaticData.License == null)
		{
			return false;
		}
		return StaticData.License.HasAllConnectors();
	}
}
