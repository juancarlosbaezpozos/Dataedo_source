using System.Collections.Generic;
using Dataedo.App.Properties;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Support;

public static class ConnectionTypes
{
	public static IEnumerable<ConnectionTypeItem> GetConnectionTypes()
	{
		return new ConnectionTypeItem[11]
		{
			new ConnectionTypeItem("FILE_SYSTEM", "File System", Resources.file_open_32),
			new ConnectionTypeItem("FTP", "FTP", Resources.server_connect_32),
			new ConnectionTypeItem("SFTP", "SFTP", Resources.server_connect_32),
			new ConnectionTypeItem("HDFS", "HDFS", Resources.file_32),
			new ConnectionTypeItem("AWS_S3", "AWS S3", Resources.amazon_athena),
			new ConnectionTypeItem("AZURE_DATA_LAKE_STORAGE_GEN1", "Azure Data Lake Storage Gen1", Resources.azure),
			new ConnectionTypeItem("AZURE_DATA_LAKE_STORAGE_GEN2", "Azure Data Lake Storage Gen2", Resources.azure),
			new ConnectionTypeItem("AZURE_FILE_STORAGE", "Azure File Storage", Resources.file_32),
			new ConnectionTypeItem("AZURE_BLOB_STORAGE", "Azure Blob Storage", Resources.azure),
			new ConnectionTypeItem("GOOGLE_CLOUD_STORAGE", "Google Cloud Storage", Resources.google_big_query),
			new ConnectionTypeItem("HTTP", "HTTP", Resources.file_32)
		};
	}
}
