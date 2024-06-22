using System;
using System.Collections.Generic;
using System.Linq;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Nodes;

namespace Dataedo.App.Helpers.CloudStorage.AzureStorage.Connections;

public class AzureStorageBlobContainerConnection : AzureStorageConnection
{
	private readonly BlobContainerClient _blobContainerClient;

	private readonly List<string> _restOfPath;

	public override Uri Uri => _blobContainerClient.Uri;

	public AzureStorageBlobContainerConnection(Uri containerUri)
	{
		if (containerUri == null)
		{
			throw new ArgumentNullException("containerUri");
		}
		_blobContainerClient = new BlobContainerClient(containerUri);
	}

	public AzureStorageBlobContainerConnection(string accountName, string containerName)
	{
		if (string.IsNullOrEmpty(accountName))
		{
			throw new ArgumentException("Argument cannot be empty", "accountName");
		}
		if (string.IsNullOrEmpty(containerName))
		{
			throw new ArgumentException("Argument cannot be empty", "containerName");
		}
		BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(AzureStorageConnection.GetBlobServiceUriString(accountName)));
		_blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
	}

	public AzureStorageBlobContainerConnection(AzureSasCredential credentials, string accountName, string containerName, List<string> restOfPath)
	{
		if (credentials == null)
		{
			throw new ArgumentNullException("credentials");
		}
		if (string.IsNullOrEmpty(accountName))
		{
			throw new ArgumentException("Argument cannot be empty", "accountName");
		}
		if (string.IsNullOrEmpty(containerName))
		{
			throw new ArgumentException("Argument cannot be empty", "containerName");
		}
		BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(AzureStorageConnection.GetBlobServiceUriString(accountName)), credentials);
		_blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
		_restOfPath = restOfPath;
	}

	public static List<AzureStorageBlobNode> GetStructure(List<BlobHierarchyItem> azureObjects, BlobContainerClient containerClient)
	{
		List<AzureStorageBlobNode> list = new List<AzureStorageBlobNode>();
		foreach (BlobHierarchyItem azureObject in azureObjects)
		{
			string[] path = azureObject.Blob.Name.Split(new string[1] { "/" }, StringSplitOptions.RemoveEmptyEntries);
			if (path.Length == 0)
			{
				AzureStorageBlobNode azureStorageBlobNode = new AzureStorageBlobNode(azureObject, containerClient);
				azureStorageBlobNode.SetError("WRONG PATH");
				list.Add(azureStorageBlobNode);
				continue;
			}
			if (path.Length == 1)
			{
				AzureStorageBlobNode azureStorageBlobNode2 = list.Where((AzureStorageBlobNode x) => x.Name == path[0]).FirstOrDefault();
				if (azureStorageBlobNode2 != null && azureStorageBlobNode2.IsTemporary)
				{
					azureStorageBlobNode2.Set(azureObject, containerClient, isTemporary: false);
					continue;
				}
				azureStorageBlobNode2 = new AzureStorageBlobNode(azureObject, containerClient);
				list.Add(azureStorageBlobNode2);
				continue;
			}
			AzureStorageBlobNode azureStorageBlobNode3 = list.Where((AzureStorageBlobNode x) => x.Name == path[0]).FirstOrDefault();
			if (azureStorageBlobNode3 == null)
			{
				azureStorageBlobNode3 = new AzureStorageBlobNode(path[0], isTemporary: true);
				list.Add(azureStorageBlobNode3);
			}
			for (int i = 1; i < path.Length - 1; i++)
			{
				string item = path[i];
				AzureStorageBlobNode azureStorageBlobNode4 = azureStorageBlobNode3.Nodes.Where((AzureStorageBlobNode x) => x.Name == item).FirstOrDefault();
				if (azureStorageBlobNode4 == null)
				{
					azureStorageBlobNode4 = new AzureStorageBlobNode(item, isTemporary: true);
					azureStorageBlobNode3.Nodes.Add(azureStorageBlobNode4);
				}
				azureStorageBlobNode3 = azureStorageBlobNode4;
			}
			AzureStorageBlobNode azureStorageBlobNode5 = azureStorageBlobNode3.Nodes.Where((AzureStorageBlobNode x) => x.Name == path.Last()).FirstOrDefault();
			if (azureStorageBlobNode5 != null && azureStorageBlobNode5.IsTemporary)
			{
				azureStorageBlobNode5.Set(azureObject, containerClient, isTemporary: false);
				continue;
			}
			azureStorageBlobNode5 = new AzureStorageBlobNode(azureObject, containerClient);
			azureStorageBlobNode3.Nodes.Add(azureStorageBlobNode5);
		}
		return list;
	}

	public override List<FileLikeNode> GetObjectsStructure(bool dynamicLoading)
	{
		if (dynamicLoading)
		{
			AzureStorageDynamicBlobNode item = new AzureStorageDynamicBlobNode(_blobContainerClient);
			return new List<FileLikeNode> { item };
		}
		List<FileLikeNode> list = new List<FileLikeNode>();
		AzureStorageBlobNode azureStorageBlobNode = new AzureStorageBlobNode(_blobContainerClient.Name, isTemporary: false);
		try
		{
			foreach (AzureStorageBlobNode item2 in GetStructure(_blobContainerClient.GetBlobsByHierarchy(BlobTraits.None, BlobStates.None, null, GetPrefix()).ToList(), _blobContainerClient))
			{
				azureStorageBlobNode.Nodes.Add(item2);
			}
		}
		catch (RequestFailedException ex) when (ex.Status == 403)
		{
			azureStorageBlobNode.SetError("NO ACCESS");
		}
		list.Add(azureStorageBlobNode);
		return list;
	}

	public override bool TestConnection()
	{
		_blobContainerClient.GetBlobsByHierarchy().Take(1).ToList();
		return true;
	}

	private string GetPrefix()
	{
		string result = null;
		if (_restOfPath != null && _restOfPath.Any())
		{
			result = string.Join("/", _restOfPath);
		}
		return result;
	}
}
