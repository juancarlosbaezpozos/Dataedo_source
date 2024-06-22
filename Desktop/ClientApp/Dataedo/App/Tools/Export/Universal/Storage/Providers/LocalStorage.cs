using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dataedo.App.Tools.Export.Universal.Storage.Providers;

internal class LocalStorage : IStorage, IReadStorage, IWriteStorage
{
	public readonly string Dir;

	public bool IsOpenable => true;

	public LocalStorage(string dir)
	{
		Dir = dir;
	}

	public IEnumerable<string> ListFiles(string searchDir = "")
	{
		string fullDir = Path.Combine(Dir, searchDir);
		return from path in Directory.EnumerateFiles(fullDir, "*.*", SearchOption.AllDirectories)
			select path.Substring(fullDir.Length + 1);
	}

	public string ReadTextFile(string path)
	{
		return File.ReadAllText(Path.Combine(Dir, path));
	}

	public byte[] ReadFile(string path)
	{
		return File.ReadAllBytes(Path.Combine(Dir, path));
	}

	public void SaveFile(string path, byte[] content)
	{
		string path2 = Path.Combine(Dir, path);
		Directory.CreateDirectory(Path.GetDirectoryName(path2));
		File.WriteAllBytes(path2, content);
	}

	public void SaveFile(string path, string content)
	{
		string path2 = Path.Combine(Dir, path);
		Directory.CreateDirectory(Path.GetDirectoryName(path2));
		File.WriteAllText(path2, content);
	}

	public void AppendFile(string path, string content)
	{
		string path2 = Path.Combine(Dir, path);
		Directory.CreateDirectory(Path.GetDirectoryName(path2));
		File.AppendAllText(path2, content);
	}

	public void DeleteFile(string path)
	{
		string path2 = Path.Combine(Dir, path);
		if (File.Exists(path2))
		{
			File.Delete(path2);
		}
	}

	public void Open()
	{
		Paths.OpenExplorerAndSelectFolder(Dir);
	}

	public void DeleteDir(string path)
	{
		string path2 = Path.Combine(Dir, path);
		if (Directory.Exists(path2))
		{
			Directory.Delete(path2, recursive: true);
		}
	}

	public void MoveFile(string oldPath, string newPath)
	{
		string sourceFileName = Path.Combine(Dir, oldPath);
		string destFileName = Path.Combine(Dir, newPath);
		File.Move(sourceFileName, destFileName);
	}

	public void MoveDir(string oldPath, string newPath)
	{
		string sourceDirName = Path.Combine(Dir, oldPath);
		string destDirName = Path.Combine(Dir, newPath);
		Directory.Move(sourceDirName, destDirName);
	}

	public bool HasFile(string path)
	{
		return File.Exists(Path.Combine(Dir, path));
	}
}
