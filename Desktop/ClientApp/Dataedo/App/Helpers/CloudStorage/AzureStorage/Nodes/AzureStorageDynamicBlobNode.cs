using System;
using System.Collections.Generic;
using System.Linq;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Dataedo.App.Helpers.Extensions;

namespace Dataedo.App.Helpers.CloudStorage.AzureStorage.Nodes;

public class AzureStorageDynamicBlobNode : AzureStorageNode
{
	public override IReadOnlyList<FileLikeNode> Children
	{
		get
		{
			if (!AreChildrenLoaded)
			{
				LoadChildren();
			}
			return Nodes;
		}
	}

	public BlobContainerClient BlobContainerClient { get; private set; }

	public BlobHierarchyItem BlobHierarchyItem { get; }

	public override bool IsDirectoryLike
	{
		get
		{
			if (BlobHierarchyItem != null)
			{
				return BlobHierarchyItem.IsPrefix;
			}
			return true;
		}
	}

	public bool AreChildrenLoaded { get; private set; }

	public override DateTime? LastModified => BlobHierarchyItem?.Blob?.Properties?.LastModified?.DateTime;

	public List<AzureStorageDynamicBlobNode> Nodes { get; private set; }

	public override long? Size
	{
		get
		{
			if (IsDirectoryLike)
			{
				return null;
			}
			return BlobHierarchyItem?.Blob?.Properties?.ContentLength;
		}
	}

	public AzureStorageDynamicBlobNode(BlobContainerClient blobContainerClient)
		: this()
	{
		_fullName = (_name = blobContainerClient.Name);
		BlobContainerClient = blobContainerClient;
	}

	private AzureStorageDynamicBlobNode()
	{
		Nodes = new List<AzureStorageDynamicBlobNode>();
	}

	public AzureStorageDynamicBlobNode(BlobContainerClient blobContainerClient, BlobHierarchyItem blobHierarchyItem)
		: this()
	{
		_fullName = (blobHierarchyItem.IsBlob ? blobHierarchyItem.Blob.Name : blobHierarchyItem.Prefix);
		_name = (blobHierarchyItem.IsBlob ? blobHierarchyItem.Blob.Name : blobHierarchyItem.Prefix).Split(new string[1] { "/" }, StringSplitOptions.RemoveEmptyEntries).Last();
		BlobContainerClient = blobContainerClient;
		BlobHierarchyItem = blobHierarchyItem;
	}

	public void LoadChildren()
	{
		List<AzureStorageDynamicBlobNode> list = new List<AzureStorageDynamicBlobNode>();
		try
		{
			if (BlobContainerClient != null && BlobHierarchyItem == null)
			{
				foreach (BlobHierarchyItem item3 in BlobContainerClient.GetBlobsByHierarchy(BlobTraits.None, BlobStates.None, "/").ToList())
				{
					AzureStorageDynamicBlobNode item = new AzureStorageDynamicBlobNode(BlobContainerClient, item3);
					list.Add(item);
				}
			}
			else if (BlobContainerClient != null && BlobHierarchyItem != null && BlobHierarchyItem.IsPrefix)
			{
				foreach (BlobHierarchyItem item4 in BlobContainerClient.GetBlobsByHierarchy(BlobTraits.None, BlobStates.None, "/", BlobHierarchyItem.Prefix).ToList())
				{
					AzureStorageDynamicBlobNode item2 = new AzureStorageDynamicBlobNode(BlobContainerClient, item4);
					list.Add(item2);
				}
			}
		}
		catch (RequestFailedException ex)
		{
			SetError(ex.Message.GetFirstLine());
		}
		Nodes = list;
		AreChildrenLoaded = true;
	}
}
