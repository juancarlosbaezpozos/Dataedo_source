using System;
using System.IO;

namespace Dataedo.App.Helpers.FileImport;

public class LocalFolderImportItem : ImportItem
{
	private readonly DirectoryInfo _directoryInfo;

	public LocalFolderImportItem(string path)
		: base(path)
	{
		_directoryInfo = new DirectoryInfo(path);
		base.Name = _directoryInfo.Name;
		base.Location = _directoryInfo.FullName;
	}

	public override Stream CreateStream()
	{
		return new StreamReader(base.Path).BaseStream;
	}

	public override void DeleteTemporaryFiles()
	{
		throw new NotImplementedException();
	}

	public override ImportItem FindDeltaLakeItem()
	{
		throw new NotImplementedException();
	}

	public override FileInfo GetFile()
	{
		throw new NotImplementedException();
	}

	public override FileInfo SaveTemporaryFile(DirectoryInfo directoryInfo)
	{
		throw new NotImplementedException();
	}
}
