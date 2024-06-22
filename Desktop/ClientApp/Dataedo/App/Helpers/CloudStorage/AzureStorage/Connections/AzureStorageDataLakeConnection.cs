using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Nodes;

namespace Dataedo.App.Helpers.CloudStorage.AzureStorage.Connections;

public class AzureStorageDataLakeConnection : AzureStorageConnection
{
	private readonly DataLakeDirectoryClient _dataLakeDirectoryClient;

	public override Uri Uri => _dataLakeDirectoryClient.Uri;

	public AzureStorageDataLakeConnection(DataLakeDirectoryClient dataLakeDirectoryClient)
	{
		_dataLakeDirectoryClient = dataLakeDirectoryClient ?? throw new ArgumentNullException("dataLakeDirectoryClient");
	}

	public override List<FileLikeNode> GetObjectsStructure(bool dynamicLoading)
	{
		if (dynamicLoading)
		{
			AzureStorageDynamicDataLakeNode item = new AzureStorageDynamicDataLakeNode(_dataLakeDirectoryClient);
			return new List<FileLikeNode> { item };
		}
		List<AzureStorageDataLakeNode> orphanedNodes = (from pathItem in _dataLakeDirectoryClient.GetPaths(recursive: true)
			select new AzureStorageDataLakeNode(pathItem, _dataLakeDirectoryClient)).ToList();
		AzureStorageDataLakeNode azureStorageDataLakeNode = new AzureStorageDataLakeNode(_dataLakeDirectoryClient);
		List<AzureStorageDataLakeNode> structure = GetStructure(orphanedNodes);
		azureStorageDataLakeNode.Nodes.AddRange(structure);
		return new List<FileLikeNode> { azureStorageDataLakeNode };
	}

	public override bool TestConnection()
	{
		_dataLakeDirectoryClient.GetPaths().Take(1).ToList();
		return true;
	}

	private List<AzureStorageDataLakeNode> GetStructure(List<AzureStorageDataLakeNode> orphanedNodes)
	{
		List<AzureStorageDataLakeNode> list = new List<AzureStorageDataLakeNode>();
		foreach (AzureStorageDataLakeNode orphanedNode in orphanedNodes)
		{
			string[] path = orphanedNode.FullName.Split(new string[1] { "/" }, StringSplitOptions.RemoveEmptyEntries);
			if (path.Length == 0)
			{
				orphanedNode.SetError("WRONG PATH");
				list.Add(orphanedNode);
				continue;
			}
			if (path.Length == 1)
			{
				list.Add(orphanedNode);
				continue;
			}
			AzureStorageDataLakeNode azureStorageDataLakeNode = list.Where((AzureStorageDataLakeNode x) => x.Name == path[0]).FirstOrDefault();
			if (azureStorageDataLakeNode == null)
			{
				azureStorageDataLakeNode = new AzureStorageDataLakeNode(path[0], isTemporary: true);
				list.Add(azureStorageDataLakeNode);
			}
			for (int i = 1; i < path.Length - 1; i++)
			{
				string item = path[i];
				AzureStorageDataLakeNode azureStorageDataLakeNode2 = azureStorageDataLakeNode.Nodes.Where((AzureStorageDataLakeNode x) => x.Name == item).FirstOrDefault();
				if (azureStorageDataLakeNode2 == null)
				{
					azureStorageDataLakeNode2 = new AzureStorageDataLakeNode(item, isTemporary: true);
					azureStorageDataLakeNode.Nodes.Add(azureStorageDataLakeNode2);
				}
				azureStorageDataLakeNode = azureStorageDataLakeNode2;
			}
			AzureStorageDataLakeNode azureStorageDataLakeNode3 = azureStorageDataLakeNode.Nodes.Where((AzureStorageDataLakeNode x) => x.Name == path.Last()).FirstOrDefault();
			if (azureStorageDataLakeNode3 != null && azureStorageDataLakeNode3.IsTemporary)
			{
				orphanedNode.StealChildren(azureStorageDataLakeNode3);
			}
			else
			{
				azureStorageDataLakeNode.Nodes.Add(orphanedNode);
			}
		}
		return list;
	}
}
