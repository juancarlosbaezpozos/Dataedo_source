using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Connections;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Nodes;
using Dataedo.App.Helpers.Files;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.Import.Exceptions;

namespace Dataedo.App.Helpers.FileImport;

public class AzureStorageDataLakeImportItem : ImportItem
{
	private readonly List<FileInfo> _temporaryFiles = new List<FileInfo>();

	private bool _deleteFileAfterImport;

	private readonly AzureStorageDataLakeNode _dataLakeNode;

	public AzureStorageConnection AzureStorageConnection { get; }

	public DataLakeDirectoryClient DataLakeDirectoryClient { get; }

	public override bool DeleteFileAfterImport => _deleteFileAfterImport;

	public bool IsTemporaryFile { get; private set; }

	public PathItem PathItem { get; }

	public AzureStorageDataLakeImportItem(AzureStorageConnection azureStorageConnection, AzureStorageDataLakeNode dataLakeNode)
		: base(null)
	{
		_dataLakeNode = dataLakeNode;
		AzureStorageConnection = azureStorageConnection;
		PathItem = dataLakeNode.PathItem;
		DataLakeDirectoryClient = dataLakeNode.DataLakeDirectoryClient;
		_deleteFileAfterImport = false;
		base.IsStream = true;
		IsTemporaryFile = false;
		base.Size = PathItem.ContentLength.GetValueOrDefault();
		base.Name = PathItem.Name.Split(new string[1] { "/" }, StringSplitOptions.RemoveEmptyEntries).Last();
		string text = azureStorageConnection.Uri?.GetLeftPart(UriPartial.Authority) ?? string.Empty;
		if (!string.IsNullOrEmpty(text))
		{
			text += "/";
		}
		base.Location = text + DataLakeDirectoryClient.FileSystemName + "/" + PathItem.Name;
	}

	public override void CorrectObjectModelAfterImport(ObjectModel oM)
	{
		base.CorrectObjectModelAfterImport(oM);
		string text2 = (oM.Name = (oM.OriginalName = base.Name));
		oM.Location = base.Location;
	}

	public override Stream CreateStream()
	{
		return new AzureStorageStream(new DataLakeFileClient(GetFileUri()).OpenRead(0L));
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

	private Uri GetFileUri()
	{
		return new DataLakeUriBuilder(DataLakeDirectoryClient.Uri)
		{
			DirectoryOrFilePath = PathItem.Name
		}.ToUri();
	}

	private string GetTemporaryFileName()
	{
		return "temp_" + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + "_azure_storage_" + DataLakeDirectoryClient.AccountName + "_" + DataLakeDirectoryClient.FileSystemName + "_" + PathItem.Name.Replace("/", "_");
	}

	private FileInfo SaveTemporaryFileImpl(DirectoryInfo directoryInfo)
	{
		string temporaryFilePath = ImportItem.GetTemporaryFilePath(directoryInfo, GetTemporaryFileName());
		new DataLakeFileClient(GetFileUri()).ReadTo(temporaryFilePath);
		return new FileInfo(temporaryFilePath);
	}

	public override ImportItem FindDeltaLakeItem()
	{
		if ((from x in AzureStorageImportHelper.FindParquetFiles(_dataLakeNode, 2)
			orderby x.LastModified descending
			select x).FirstOrDefault() is AzureStorageDataLakeNode dataLakeNode)
		{
			return new AzureStorageDataLakeImportItem(AzureStorageConnection, dataLakeNode);
		}
		throw new InvalidDataProvidedException("Selected item is not valid Delta Lake directory");
	}
}
