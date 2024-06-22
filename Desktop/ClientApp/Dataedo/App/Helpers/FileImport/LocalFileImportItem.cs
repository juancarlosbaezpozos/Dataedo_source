using System;
using System.IO;

namespace Dataedo.App.Helpers.FileImport;

public class LocalFileImportItem : ImportItem
{
	private readonly FileInfo _fileInfo;

	public LocalFileImportItem(string path)
		: base(path)
	{
		_fileInfo = new FileInfo(path);
		base.Size = _fileInfo.Length;
		base.Name = _fileInfo.Name;
		base.Location = _fileInfo.FullName;
	}

	public override Stream CreateStream()
	{
		return new StreamReader(base.Path).BaseStream;
	}

	public override void DeleteTemporaryFiles()
	{
		throw new NotImplementedException();
	}

	public override FileInfo GetFile()
	{
		return _fileInfo;
	}

	public override FileInfo SaveTemporaryFile(DirectoryInfo directoryInfo)
	{
		throw new NotImplementedException();
	}

	public override ImportItem FindDeltaLakeItem()
	{
		throw new NotImplementedException();
	}
}
