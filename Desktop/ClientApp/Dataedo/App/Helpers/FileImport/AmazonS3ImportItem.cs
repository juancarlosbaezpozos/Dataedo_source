using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Amazon.S3.Model;
using Dataedo.App.Helpers.CloudStorage.AmazonS3;
using Dataedo.App.Helpers.Files;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.Import.Exceptions;

namespace Dataedo.App.Helpers.FileImport;

public class AmazonS3ImportItem : ImportItem
{
	private readonly AmazonS3Connection _amazonClient;

	private readonly S3Node _s3Node;

	private readonly S3Object _s3Object;

	private bool _deleteFileAfterImport;

	private readonly List<FileInfo> _temporaryFiles = new List<FileInfo>();

	public string BucketName { get; }

	public string Key { get; }

	public bool IsTemporaryFile { get; private set; }

	public override bool DeleteFileAfterImport => _deleteFileAfterImport;

	public AmazonS3ImportItem(AmazonS3Connection amazonClient, S3Node s3Node)
		: base(null)
	{
		_amazonClient = amazonClient;
		_s3Node = s3Node;
		_s3Object = s3Node.S3Object;
		_deleteFileAfterImport = false;
		base.IsStream = true;
		IsTemporaryFile = false;
		Key = _s3Object?.Key;
		BucketName = _s3Object?.BucketName;
		base.Size = _s3Object?.Size ?? 0;
		base.Name = Key?.Split('/')?.Last();
		base.Location = "s3://" + BucketName + "/" + Key;
	}

	public override Stream CreateStream()
	{
		return _amazonClient.GetObjectStream(_s3Object, 5120L);
	}

	public override void CorrectObjectModelAfterImport(ObjectModel oM)
	{
		base.CorrectObjectModelAfterImport(oM);
		string text2 = (oM.Name = (oM.OriginalName = base.Name));
		oM.Location = base.Location;
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

	private FileInfo SaveTemporaryFileImpl(DirectoryInfo directoryInfo)
	{
		string temporaryFilePath = ImportItem.GetTemporaryFilePath(directoryInfo, GetTemporaryFileName());
		_amazonClient.GetObject(_s3Object).WriteResponseStreamToFile(temporaryFilePath);
		return new FileInfo(temporaryFilePath);
	}

	private string GetTemporaryFileName()
	{
		return "temp_" + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + "_s3_" + _s3Object.Key.Replace('/', '_');
	}

	public override ImportItem FindDeltaLakeItem()
	{
		S3Node s3Node = (from x in FindParquetFiles()
			orderby x.LastModified descending
			select x).FirstOrDefault();
		if (s3Node == null)
		{
			throw new InvalidDataProvidedException("Selected item is not valid Delta Lake directory");
		}
		return new AmazonS3ImportItem(_amazonClient, s3Node);
	}

	private IEnumerable<S3Node> FindParquetFiles()
	{
		return FindParquetFiles(_s3Node, 2);
	}

	private static IEnumerable<S3Node> FindParquetFiles(S3Node parentNode, int level)
	{
		if (level <= 0 || parentNode.Name.Contains("_delta_log"))
		{
			yield break;
		}
		if (parentNode.S3Object != null && parentNode.S3Object.Key.EndsWith(".parquet", StringComparison.OrdinalIgnoreCase))
		{
			yield return parentNode;
			yield break;
		}
		foreach (S3Node node in parentNode.Nodes)
		{
			foreach (S3Node item in FindParquetFiles(node, level - 1))
			{
				yield return item;
			}
		}
	}
}
