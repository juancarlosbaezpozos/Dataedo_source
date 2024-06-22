using System;
using System.Collections.Generic;
using System.Linq;
using Azure;
using Azure.Core;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Nodes;

namespace Dataedo.App.Helpers.CloudStorage.AzureStorage.Connections;

public class AzureStorageBlobServiceConnection : AzureStorageConnection
{
	private readonly BlobServiceClient _blobServiceClient;

	public override Uri Uri => _blobServiceClient?.Uri;

	public AzureStorageBlobServiceConnection(StorageSharedKeyCredential credentials, string accountName)
	{
		if (credentials == null)
		{
			throw new ArgumentNullException("credentials");
		}
		if (string.IsNullOrEmpty(accountName))
		{
			throw new ArgumentException("Argument cannot be empty", "accountName");
		}
		string blobServiceUriString = AzureStorageConnection.GetBlobServiceUriString(accountName);
		_blobServiceClient = new BlobServiceClient(new Uri(blobServiceUriString), credentials);
	}

	public AzureStorageBlobServiceConnection(TokenCredential credentials, string accountName)
	{
		if (credentials == null)
		{
			throw new ArgumentNullException("credentials");
		}
		if (string.IsNullOrEmpty(accountName))
		{
			throw new ArgumentException("Argument cannot be empty", "accountName");
		}
		string blobServiceUriString = AzureStorageConnection.GetBlobServiceUriString(accountName);
		_blobServiceClient = new BlobServiceClient(new Uri(blobServiceUriString), credentials);
	}

	public AzureStorageBlobServiceConnection(AzureSasCredential credentials, string accountName)
	{
		if (credentials == null)
		{
			throw new ArgumentNullException("credentials");
		}
		if (string.IsNullOrEmpty(accountName))
		{
			throw new ArgumentException("Argument cannot be empty", "accountName");
		}
		string blobServiceUriString = AzureStorageConnection.GetBlobServiceUriString(accountName);
		_blobServiceClient = new BlobServiceClient(new Uri(blobServiceUriString), credentials);
	}

	public AzureStorageBlobServiceConnection(string connectionString)
	{
		if (string.IsNullOrEmpty(connectionString))
		{
			throw new ArgumentException("Argument cannot be empty", "connectionString");
		}
		_blobServiceClient = new BlobServiceClient(connectionString);
	}

	public AzureStorageBlobServiceConnection(BlobServiceClient blobServiceClient)
	{
		_blobServiceClient = blobServiceClient ?? throw new ArgumentNullException("blobServiceClient");
	}

	public override List<FileLikeNode> GetObjectsStructure(bool dynamicLoading)
	{
		if (dynamicLoading)
		{
			List<FileLikeNode> list = new List<FileLikeNode>();
			{
				foreach (BlobContainerItem item2 in _blobServiceClient.GetBlobContainers().ToList())
				{
					AzureStorageDynamicBlobNode item = new AzureStorageDynamicBlobNode(_blobServiceClient.GetBlobContainerClient(item2.Name));
					list.Add(item);
				}
				return list;
			}
		}
		List<FileLikeNode> list2 = new List<FileLikeNode>();
		foreach (BlobContainerItem item3 in _blobServiceClient.GetBlobContainers().ToList())
		{
			AzureStorageBlobNode azureStorageBlobNode = new AzureStorageBlobNode(item3);
			BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(item3.Name);
			try
			{
				foreach (AzureStorageBlobNode item4 in AzureStorageBlobContainerConnection.GetStructure(blobContainerClient.GetBlobsByHierarchy().ToList(), blobContainerClient))
				{
					azureStorageBlobNode.Nodes.Add(item4);
				}
			}
			catch (RequestFailedException ex) when (ex.Status == 403)
			{
				azureStorageBlobNode.SetError("NO ACCESS");
			}
			list2.Add(azureStorageBlobNode);
		}
		return list2;
	}

	public override bool TestConnection()
	{
		_blobServiceClient.GetBlobContainers().Take(1).ToList();
		return true;
	}
}
