using System;
using System.Collections.Generic;
using System.Linq;
using Azure;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using Dataedo.App.Helpers.Extensions;

namespace Dataedo.App.Helpers.CloudStorage.AzureStorage.Nodes;

public class AzureStorageDynamicDataLakeNode : AzureStorageNode
{
	private PathProperties _pathProperties;

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

	public DataLakeDirectoryClient DataLakeDirectoryClient { get; }

	public DataLakeFileClient DataLakeFileClient { get; }

	public PathProperties Properties
	{
		get
		{
			if (_pathProperties == null)
			{
				LoadPathProperties();
			}
			return _pathProperties;
		}
	}

	public override bool IsDirectoryLike => DataLakeDirectoryClient != null;

	public bool AreChildrenLoaded { get; private set; }

	public override DateTime? LastModified => (Properties?.LastModified)?.DateTime;

	public List<AzureStorageDynamicDataLakeNode> Nodes { get; private set; }

	public override long? Size
	{
		get
		{
			if (IsDirectoryLike)
			{
				return null;
			}
			return Properties?.ContentLength;
		}
	}

	public AzureStorageDynamicDataLakeNode(DataLakeDirectoryClient dataLakeDirectoryClient)
		: this()
	{
		DataLakeDirectoryClient = dataLakeDirectoryClient ?? throw new ArgumentNullException("dataLakeDirectoryClient");
		_fullName = dataLakeDirectoryClient.Path;
		_name = dataLakeDirectoryClient.Name;
	}

	public AzureStorageDynamicDataLakeNode(DataLakeFileClient fileClient)
		: this()
	{
		DataLakeFileClient = fileClient ?? throw new ArgumentNullException("fileClient");
		_fullName = (_name = fileClient.Name);
	}

	private void LoadPathProperties()
	{
		if (DataLakeFileClient != null)
		{
			_pathProperties = DataLakeFileClient.GetProperties().Value;
		}
	}

	private AzureStorageDynamicDataLakeNode()
	{
		Nodes = new List<AzureStorageDynamicDataLakeNode>();
	}

	public void LoadChildren()
	{
		List<AzureStorageDynamicDataLakeNode> list = new List<AzureStorageDynamicDataLakeNode>();
		if (DataLakeDirectoryClient != null)
		{
			try
			{
				foreach (PathItem item3 in DataLakeDirectoryClient.GetPaths().ToList())
				{
					if (item3.IsDirectory.GetValueOrDefault())
					{
						string subdirectoryName = item3.Name.DeletePrefixIfExists(DataLakeDirectoryClient.Path).DeletePrefixIfExists("/");
						AzureStorageDynamicDataLakeNode item = new AzureStorageDynamicDataLakeNode(DataLakeDirectoryClient.GetSubDirectoryClient(subdirectoryName));
						list.Add(item);
					}
					else
					{
						string fileName = item3.Name.DeletePrefixIfExists(DataLakeDirectoryClient.Path).DeletePrefixIfExists("/");
						AzureStorageDynamicDataLakeNode item2 = new AzureStorageDynamicDataLakeNode(DataLakeDirectoryClient.GetFileClient(fileName));
						list.Add(item2);
					}
				}
			}
			catch (RequestFailedException ex)
			{
				SetError(ex.Message.GetFirstLine());
			}
		}
		Nodes = list;
		AreChildrenLoaded = true;
	}
}
