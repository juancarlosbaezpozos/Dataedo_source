using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Azure;
using Azure.Storage.Files.DataLake;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Connections;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Nodes;
using Dataedo.App.Helpers.Extensions;
using Dataedo.App.Helpers.Files;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.Import.Exceptions;

namespace Dataedo.App.Helpers.FileImport;

public class AzureStorageDynamicDataLakeImportItem : ImportItem
{
	private readonly List<FileInfo> _temporaryFiles = new List<FileInfo>();

	private bool _deleteFileAfterImport;

	private readonly AzureStorageDynamicDataLakeNode _dataLakeNode;

	public AzureStorageConnection AzureStorageConnection { get; }

	public DataLakeDirectoryClient DataLakeDirectoryClient { get; }

	public DataLakeFileClient DataLakeFileClient { get; }

	public override bool DeleteFileAfterImport => _deleteFileAfterImport;

	public bool IsTemporaryFile { get; private set; }

	public AzureStorageDynamicDataLakeImportItem(AzureStorageConnection azureStorageConnection, AzureStorageDynamicDataLakeNode dataLakeNode)
		: base(null)
	{
		_dataLakeNode = dataLakeNode ?? throw new ArgumentNullException("dataLakeNode");
		AzureStorageConnection = azureStorageConnection ?? throw new ArgumentNullException("azureStorageConnection");
		if (_dataLakeNode.DataLakeDirectoryClient == null && _dataLakeNode.DataLakeFileClient == null)
		{
			throw new ArgumentException("Neither Data Lake Directory Client nor Data Lake File Client was supplied");
		}
		DataLakeDirectoryClient = dataLakeNode.DataLakeDirectoryClient;
		DataLakeFileClient = dataLakeNode.DataLakeFileClient;
		_deleteFileAfterImport = false;
		base.IsStream = true;
		IsTemporaryFile = false;
		base.Size = _dataLakeNode.Size.GetValueOrDefault();
		base.Name = _dataLakeNode.FullName.Split(new string[1] { "/" }, StringSplitOptions.RemoveEmptyEntries).Last();
		if (DataLakeFileClient != null)
		{
			base.Location = DataLakeFileClient.Uri?.GetLeftPart(UriPartial.Path);
		}
		else
		{
			base.Location = DataLakeDirectoryClient.Uri?.GetLeftPart(UriPartial.Path);
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
		try
		{
			return new AzureStorageStream(new DataLakeFileClient(GetFileUri()).OpenRead(0L));
		}
		catch (RequestFailedException ex)
		{
			throw new InvalidDataProvidedException(ex.Message.GetFirstLine(), ex);
		}
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
		return DataLakeFileClient.Uri;
	}

	private string GetTemporaryFileName()
	{
		return "temp_" + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + "_azure_storage_" + DataLakeFileClient.Name.Replace("/", "_");
	}

	private FileInfo SaveTemporaryFileImpl(DirectoryInfo directoryInfo)
	{
		if (DataLakeFileClient == null)
		{
			throw new InvalidOperationException("Data Lake File Client is not set");
		}
		string temporaryFilePath = ImportItem.GetTemporaryFilePath(directoryInfo, GetTemporaryFileName());
		DataLakeFileClient.ReadTo(temporaryFilePath);
		return new FileInfo(temporaryFilePath);
	}

	public override ImportItem FindDeltaLakeItem()
	{
		if ((from x in AzureStorageImportHelper.FindParquetFiles(_dataLakeNode, 2)
			orderby x.LastModified descending
			select x).FirstOrDefault() is AzureStorageDynamicDataLakeNode dataLakeNode)
		{
			return new AzureStorageDynamicDataLakeImportItem(AzureStorageConnection, dataLakeNode);
		}
		throw new InvalidDataProvidedException("Selected item is not valid Delta Lake directory");
	}
}
