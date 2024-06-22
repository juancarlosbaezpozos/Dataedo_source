using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;

namespace Dataedo.App.Helpers.CloudStorage.AzureStorage.Nodes;

public class AzureStorageDataLakeNode : AzureStorageNode
{
	public override IReadOnlyList<FileLikeNode> Children => Nodes;

	public DataLakeDirectoryClient DataLakeDirectoryClient { get; }

	public override bool IsDirectoryLike
	{
		get
		{
			if (!(PathItem?.IsDirectory).GetValueOrDefault())
			{
				if (PathItem == null)
				{
					return DataLakeDirectoryClient != null;
				}
				return false;
			}
			return true;
		}
	}

	public bool IsTemporary { get; }

	public override DateTime? LastModified => (PathItem?.LastModified)?.DateTime;

	public List<AzureStorageDataLakeNode> Nodes { get; private set; }

	public PathItem PathItem { get; }

	public override long? Size
	{
		get
		{
			if (IsDirectoryLike)
			{
				return null;
			}
			return PathItem?.ContentLength;
		}
	}

	public AzureStorageDataLakeNode(string name, bool isTemporary)
		: this()
	{
		_fullName = (_name = name);
		IsTemporary = isTemporary;
	}

	public AzureStorageDataLakeNode(PathItem pathItem, DataLakeDirectoryClient dataLakeDirectoryClient)
		: this()
	{
		PathItem = pathItem ?? throw new ArgumentNullException("pathItem");
		DataLakeDirectoryClient = dataLakeDirectoryClient ?? throw new ArgumentNullException("dataLakeDirectoryClient");
		_fullName = pathItem.Name;
		_name = pathItem.Name.Split(new string[1] { "/" }, StringSplitOptions.RemoveEmptyEntries).Last();
	}

	public AzureStorageDataLakeNode(DataLakeDirectoryClient dataLakeDirectoryClient)
		: this()
	{
		DataLakeDirectoryClient = dataLakeDirectoryClient ?? throw new ArgumentNullException("dataLakeDirectoryClient");
		_fullName = (_name = dataLakeDirectoryClient.Name);
	}

	private AzureStorageDataLakeNode()
	{
		Nodes = new List<AzureStorageDataLakeNode>();
	}

	public void StealChildren(AzureStorageDataLakeNode node)
	{
		Nodes = node.Nodes;
		node.Nodes = new List<AzureStorageDataLakeNode>();
	}
}
