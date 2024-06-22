using System;
using System.Drawing;
using System.Linq;
using Dataedo.App.Properties;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Import.CloudStorage;

public class CloudStorageTypeEnum
{
	public enum CloudStorageType
	{
		AmazonS3 = 0,
		AzureBlobStorage = 1,
		AzureDataLakeStorage = 2
	}

	public const string AMAZON_S3_STRING = "AMAZON_S3";

	public const string AZURE_BLOB_STORAGE_STRING = "AZURE_BLOB_STORAGE";

	public const string AZURE_DATA_LAKE_STORAGE_STRING = "AZURE_DATA_LAKE_STORAGE";

	public static string GetDisplayName(CloudStorageType cloudStorageType)
	{
		return cloudStorageType switch
		{
			CloudStorageType.AmazonS3 => "Amazon S3", 
			CloudStorageType.AzureBlobStorage => "Azure Blob Storage", 
			CloudStorageType.AzureDataLakeStorage => "Azure Data Lake Storage (ADLS)", 
			_ => throw new ArgumentException("cloudStorageType"), 
		};
	}

	public static Image GetImage(CloudStorageType cloudStorageType)
	{
		return cloudStorageType switch
		{
			CloudStorageType.AmazonS3 => Resources.amazon_s3, 
			CloudStorageType.AzureBlobStorage => Resources.azure_blob_storage_16, 
			CloudStorageType.AzureDataLakeStorage => Resources.azure_data_lake_storage_16, 
			_ => throw new ArgumentException("cloudStorageType"), 
		};
	}

	public static CloudStorageType[] GetCloudStorageTypes()
	{
		return (CloudStorageType[])Enum.GetValues(typeof(CloudStorageType));
	}

	public static CloudStorageTypeObject[] GetCloudStorageTypeObjects()
	{
		return (from x in GetCloudStorageTypes()
			select new CloudStorageTypeObject(x) into x
			orderby x.DisplayName
			select x).ToArray();
	}

	public static string GetConnectorLicenseString(CloudStorageType value)
	{
		return value switch
		{
			CloudStorageType.AmazonS3 => "CONNECTOR_AWS_S3", 
			CloudStorageType.AzureBlobStorage => "CONNECTOR_AZURE_BLOB_STORAGE", 
			CloudStorageType.AzureDataLakeStorage => "CONNECTOR_AZURE_DATA_LAKE_STORAGE", 
			_ => throw new ArgumentException($"Provided connector type ({value}) is not supported."), 
		};
	}

	public static string TypeToString(CloudStorageType cloudStorageType)
	{
		return cloudStorageType switch
		{
			CloudStorageType.AmazonS3 => "AMAZON_S3", 
			CloudStorageType.AzureBlobStorage => "AZURE_BLOB_STORAGE", 
			CloudStorageType.AzureDataLakeStorage => "AZURE_DATA_LAKE_STORAGE", 
			_ => throw new ArgumentException("cloudStorageType"), 
		};
	}

	public static CloudStorageType StringToType(string cloudStorageType)
	{
		return cloudStorageType switch
		{
			"AMAZON_S3" => CloudStorageType.AmazonS3, 
			"AZURE_BLOB_STORAGE" => CloudStorageType.AzureBlobStorage, 
			"AZURE_DATA_LAKE_STORAGE" => CloudStorageType.AzureDataLakeStorage, 
			_ => throw new ArgumentException("cloudStorageType"), 
		};
	}

	public static SharedDatabaseTypeEnum.DatabaseType ToDatabaseType(CloudStorageType cloudStorageType)
	{
		return cloudStorageType switch
		{
			CloudStorageType.AmazonS3 => SharedDatabaseTypeEnum.DatabaseType.AmazonS3, 
			CloudStorageType.AzureBlobStorage => SharedDatabaseTypeEnum.DatabaseType.AzureBlobStorage, 
			CloudStorageType.AzureDataLakeStorage => SharedDatabaseTypeEnum.DatabaseType.AzureDataLakeStorage, 
			_ => throw new ArgumentException("cloudStorageType"), 
		};
	}

	public static CloudStorageType? FromDatabaseType(SharedDatabaseTypeEnum.DatabaseType? databaseType)
	{
		return databaseType switch
		{
			SharedDatabaseTypeEnum.DatabaseType.AmazonS3 => CloudStorageType.AmazonS3, 
			SharedDatabaseTypeEnum.DatabaseType.AzureBlobStorage => CloudStorageType.AzureBlobStorage, 
			SharedDatabaseTypeEnum.DatabaseType.AzureDataLakeStorage => CloudStorageType.AzureDataLakeStorage, 
			_ => null, 
		};
	}
}
