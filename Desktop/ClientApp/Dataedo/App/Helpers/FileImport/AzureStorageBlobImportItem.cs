using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Connections;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Nodes;
using Dataedo.App.Helpers.Files;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.Import.Exceptions;

namespace Dataedo.App.Helpers.FileImport;

public class AzureStorageBlobImportItem : ImportItem
{
	private readonly List<FileInfo> _temporaryFiles = new List<FileInfo>();

	private bool _deleteFileAfterImport;

	private readonly AzureStorageBlobNode _blobNode;

	public AzureStorageConnection AzureStorageConnection { get; }

	public BlobContainerClient BlobContainerClient { get; }

	public BlobHierarchyItem BlobHierarchyItem { get; }

	public override bool DeleteFileAfterImport => _deleteFileAfterImport;

	public bool IsTemporaryFile { get; private set; }

	public AzureStorageBlobImportItem(AzureStorageConnection azureStorageConnection, AzureStorageBlobNode blobNode)
		: base(null)
	{
		_blobNode = blobNode ?? throw new ArgumentNullException("blobNode");
		AzureStorageConnection = azureStorageConnection ?? throw new ArgumentNullException("azureStorageConnection");
		BlobHierarchyItem = blobNode.BlobHierarchyItem;
		BlobContainerClient = blobNode.BlobContainerClient;
		_deleteFileAfterImport = false;
		base.IsStream = true;
		IsTemporaryFile = false;
		base.Size = blobNode.Size.GetValueOrDefault();
		base.Name = _blobNode.Name;
		string text = azureStorageConnection.Uri?.GetLeftPart(UriPartial.Authority) ?? string.Empty;
		if (!string.IsNullOrEmpty(text))
		{
			text += "/";
		}
		if (BlobContainerClient != null && BlobHierarchyItem != null)
		{
			base.Location = text + BlobContainerClient.Name + "/" + BlobHierarchyItem.Blob?.Name;
		}
		else
		{
			base.Location = text;
		}
	}

	public override void CorrectObjectModelAfterImport(ObjectModel oM)
	{
		base.CorrectObjectModelAfterImport(oM);
		string text2 = (oM.Name = (oM.OriginalName = base.Name));
		oM.Location = base.Location;
	}

	public override Stream CreateStream()
	{
		return new AzureStorageStream(BlobContainerClient.GetBlobClient(BlobHierarchyItem.Blob.Name).OpenRead(0L));
	}

	public override void DeleteTemporaryFiles()
	{
		foreach (FileInfo temporaryFile in _temporaryFiles)
		{
			temporaryFile.Delete();
		}
		_temporaryFiles.Clear();
		base.IsStream = true;
		IsTemporaryFile = false;
	}

	public override FileInfo GetFile()
	{
		if (_temporaryFiles.Any())
		{
			return _temporaryFiles.Last();
		}
		return SaveTemporaryFile(FilesHelper.GetDefaultTempDirectory());
	}

	public override FileInfo SaveTemporaryFile(DirectoryInfo directoryInfo)
	{
		FileInfo fileInfo = SaveTemporaryFileImpl(directoryInfo);
		_temporaryFiles.Add(fileInfo);
		base.IsStream = false;
		IsTemporaryFile = true;
		_deleteFileAfterImport = true;
		return fileInfo;
	}

	private string GetTemporaryFileName()
	{
		return "temp_" + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + "_azure_storage_" + BlobContainerClient.AccountName + "_" + BlobContainerClient.Name + "_" + BlobHierarchyItem.Blob.Name.Replace("/", "_");
	}

	private FileInfo SaveTemporaryFileImpl(DirectoryInfo directoryInfo)
	{
		string temporaryFilePath = ImportItem.GetTemporaryFilePath(directoryInfo, GetTemporaryFileName());
		BlobContainerClient.GetBlobClient(BlobHierarchyItem.Blob.Name).DownloadTo(temporaryFilePath);
		return new FileInfo(temporaryFilePath);
	}

	public override ImportItem FindDeltaLakeItem()
	{
		if ((from x in AzureStorageImportHelper.FindParquetFiles(_blobNode, 2)
			orderby x.LastModified descending
			select x).FirstOrDefault() is AzureStorageBlobNode blobNode)
		{
			return new AzureStorageBlobImportItem(AzureStorageConnection, blobNode);
		}
		throw new InvalidDataProvidedException("Selected item is not valid Delta Lake directory");
	}
}
