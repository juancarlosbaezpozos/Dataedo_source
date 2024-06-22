using System;
using System.Drawing;
using System.Linq;
using Dataedo.App.Properties;

namespace Dataedo.App.Import.CloudStorage;

public class FileImportTypeEnum
{
	public enum FileImportType
	{
		LocalFile = 0,
		AmazonS3 = 1,
		AzureBlobStorage = 2,
		AzureDataLakeStorage = 3
	}

	public static string GetDisplayName(FileImportType fileImportType)
	{
		return fileImportType switch
		{
			FileImportType.LocalFile => "Local File", 
			FileImportType.AmazonS3 => "Amazon S3", 
			FileImportType.AzureBlobStorage => "Azure Blob Storage", 
			FileImportType.AzureDataLakeStorage => "Azure Data Lake Storage (ADLS)", 
			_ => throw new ArgumentException("fileImportType"), 
		};
	}

	public static CloudStorageTypeEnum.CloudStorageType? ToCloudStorageType(FileImportType fileImportType)
	{
		return fileImportType switch
		{
			FileImportType.AmazonS3 => CloudStorageTypeEnum.CloudStorageType.AmazonS3, 
			FileImportType.AzureBlobStorage => CloudStorageTypeEnum.CloudStorageType.AzureBlobStorage, 
			FileImportType.AzureDataLakeStorage => CloudStorageTypeEnum.CloudStorageType.AzureDataLakeStorage, 
			FileImportType.LocalFile => null, 
			_ => throw new ArgumentException("fileImportType"), 
		};
	}

	public static FileImportType[] GetFileImportTypes()
	{
		return (FileImportType[])Enum.GetValues(typeof(FileImportType));
	}

	public static FileImportTypeObject[] GetFileImportTypeObjects()
	{
		return (from x in GetFileImportTypes()
			select new FileImportTypeObject(x) into x
			orderby x.DisplayName
			select x).ToArray();
	}

	public static Image GetImage(FileImportType cloudStorageType)
	{
		return cloudStorageType switch
		{
			FileImportType.LocalFile => Resources.directory_16, 
			FileImportType.AmazonS3 => Resources.amazon_s3, 
			FileImportType.AzureBlobStorage => Resources.azure_blob_storage_16, 
			FileImportType.AzureDataLakeStorage => Resources.azure_data_lake_storage_16, 
			_ => throw new ArgumentException("cloudStorageType"), 
		};
	}
}
