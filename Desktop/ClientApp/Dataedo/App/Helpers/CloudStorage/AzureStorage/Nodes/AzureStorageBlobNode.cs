using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Dataedo.App.Helpers.CloudStorage.AzureStorage.Nodes;

public class AzureStorageBlobNode : AzureStorageNode
{
	public BlobContainerClient BlobContainerClient { get; private set; }

	public BlobHierarchyItem BlobHierarchyItem { get; private set; }

	public override IReadOnlyList<FileLikeNode> Children => Nodes;

	public override bool IsDirectoryLike
	{
		get
		{
			if (BlobHierarchyItem != null && !BlobHierarchyItem.IsPrefix)
			{
				return Nodes.Any();
			}
			return true;
		}
	}

	public bool IsTemporary { get; private set; }

	public override DateTime? LastModified => BlobHierarchyItem?.Blob?.Properties?.LastModified?.DateTime;

	public List<AzureStorageBlobNode> Nodes { get; private set; }

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

	public AzureStorageBlobNode(string name, bool isTemporary)
		: this()
	{
		_fullName = (_name = name);
		IsTemporary = isTemporary;
	}

	public AzureStorageBlobNode(BlobContainerItem blobContainer)
		: this()
	{
		if (blobContainer == null)
		{
			throw new ArgumentNullException("blobContainer");
		}
		_fullName = (_name = blobContainer.Name);
	}

	public AzureStorageBlobNode(BlobHierarchyItem blob, BlobContainerClient containerClient)
		: this()
	{
		BlobHierarchyItem = blob ?? throw new ArgumentNullException("blob");
		BlobContainerClient = containerClient ?? throw new ArgumentNullException("containerClient");
		_fullName = (blob.IsBlob ? blob.Blob.Name : blob.Prefix);
		_name = (blob.IsBlob ? blob.Blob.Name : blob.Prefix).Split(new string[1] { "/" }, StringSplitOptions.RemoveEmptyEntries).Last();
	}

	private AzureStorageBlobNode()
	{
		Nodes = new List<AzureStorageBlobNode>();
	}

	public void Set(BlobHierarchyItem blob, BlobContainerClient containerClient, bool isTemporary)
	{
		BlobHierarchyItem = blob;
		BlobContainerClient = containerClient;
		IsTemporary = isTemporary;
	}
}
