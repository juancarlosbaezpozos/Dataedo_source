using System;
using System.Collections.Generic;
using Dataedo.App.Helpers.CloudStorage;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Nodes;

namespace Dataedo.App.Helpers.FileImport;

public class AzureStorageImportHelper
{
	public static IEnumerable<AzureStorageNode> FindParquetFiles(AzureStorageNode parentNode, int level)
	{
		if (parentNode == null || level <= 0 || parentNode.Name.Contains("_delta_log"))
		{
			yield break;
		}
		if (!parentNode.IsDirectoryLike && parentNode.FullName.EndsWith(".parquet", StringComparison.OrdinalIgnoreCase))
		{
			yield return parentNode;
			yield break;
		}
		foreach (FileLikeNode child in parentNode.Children)
		{
			foreach (AzureStorageNode item in FindParquetFiles(child as AzureStorageNode, level - 1))
			{
				yield return item;
			}
		}
	}
}
