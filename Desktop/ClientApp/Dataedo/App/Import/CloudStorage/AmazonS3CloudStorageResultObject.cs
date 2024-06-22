using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Helpers.CloudStorage.AmazonS3;
using Dataedo.App.Helpers.FileImport;

namespace Dataedo.App.Import.CloudStorage;

public class AmazonS3CloudStorageResultObject : ICloudStorageResultObject
{
	public List<S3Node> CloudResult { get; set; }

	public AmazonS3Connection AmazonClient { get; set; }

	public string WarningMessage { get; set; }

	public List<string> CloudStorageItems()
	{
		return CloudResult.Select((S3Node x) => x.FullPath).ToList();
	}

	public List<ImportItem> GetStreamableImportItems()
	{
		List<ImportItem> list = new List<ImportItem>();
		foreach (S3Node item2 in CloudResult)
		{
			AmazonS3ImportItem item = new AmazonS3ImportItem(AmazonClient, item2);
			list.Add(item);
		}
		return list;
	}
}
