using System.Globalization;
using System.IO;
using Dataedo.App.Import.DataLake.Model;

namespace Dataedo.App.Helpers.FileImport;

public abstract class ImportItem
{
	public string Path { get; protected set; }

	public string Name { get; protected set; }

	public string Location { get; protected set; }

	public bool IsStream { get; protected set; }

	public virtual bool DeleteFileAfterImport => false;

	public long Size { get; protected set; }

	public ImportItem(string path)
	{
		Path = path;
	}

	public virtual void CorrectObjectModelAfterImport(ObjectModel oM)
	{
		oM.ImportItem = this;
	}

	public abstract Stream CreateStream();

	public abstract FileInfo GetFile();

	public abstract FileInfo SaveTemporaryFile(DirectoryInfo directoryInfo);

	public abstract void DeleteTemporaryFiles();

	public abstract ImportItem FindDeltaLakeItem();

	protected static string GetTemporaryFilePath(DirectoryInfo directoryInfo, string tempFileName)
	{
		char[] invalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();
		foreach (char c in invalidFileNameChars)
		{
			tempFileName = tempFileName.Replace(c.ToString(CultureInfo.InvariantCulture), string.Empty);
		}
		string text = System.IO.Path.Combine(directoryInfo.FullName, tempFileName);
		try
		{
			new FileInfo(text);
			return text;
		}
		catch (IOException)
		{
			return System.IO.Path.Combine(directoryInfo.FullName, System.IO.Path.GetRandomFileName());
		}
	}
}
