using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Helpers.CloudStorage;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Connections;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Nodes;
using Dataedo.App.Helpers.FileImport;

namespace Dataedo.App.Import.CloudStorage;

public class AzureStorageCloudStorageResultObject : ICloudStorageResultObject
{
	public AzureStorageConnection AzureStorageConnection { get; set; }

	public List<FileLikeNode> CloudResult { get; }

	public string WarningMessage { get; set; }

	public AzureStorageCloudStorageResultObject(IEnumerable<FileLikeNode> cloudResult)
	{
		CloudResult = cloudResult.ToList();
	}

	public List<string> CloudStorageItems()
	{
		return CloudResult.Select((FileLikeNode x) => x.Name).ToList();
	}

	public List<ImportItem> GetStreamableImportItems()
	{
		List<ImportItem> list = new List<ImportItem>();
		foreach (FileLikeNode item in CloudResult)
		{
			if (!(item is AzureStorageBlobNode blobNode))
			{
				if (!(item is AzureStorageDataLakeNode dataLakeNode))
				{
					if (!(item is AzureStorageDynamicDataLakeNode dataLakeNode2))
					{
						if (!(item is AzureStorageDynamicBlobNode blobNode2))
						{
							throw new ArgumentException("Cannot recognize selected node type");
						}
						list.Add(new AzureStorageDynamicBlobImportItem(AzureStorageConnection, blobNode2));
					}
					else
					{
						list.Add(new AzureStorageDynamicDataLakeImportItem(AzureStorageConnection, dataLakeNode2));
					}
				}
				else
				{
					list.Add(new AzureStorageDataLakeImportItem(AzureStorageConnection, dataLakeNode));
				}
			}
			else
			{
				list.Add(new AzureStorageBlobImportItem(AzureStorageConnection, blobNode));
			}
		}
		return list;
	}
}
