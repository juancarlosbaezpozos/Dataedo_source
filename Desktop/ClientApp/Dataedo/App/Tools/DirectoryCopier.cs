using System.IO;

namespace Dataedo.App.Tools;

internal static class DirectoryCopier
{
	public static void CopyDirectory(string source, string destination, bool overwrite = false)
	{
		Directory.CreateDirectory(destination);
		DirectoryInfo directoryInfo = new DirectoryInfo(source);
		FileInfo[] files = directoryInfo.GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			fileInfo.CopyTo(Path.Combine(destination, fileInfo.Name), overwrite);
		}
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		foreach (DirectoryInfo directoryInfo2 in directories)
		{
			CopyDirectory(directoryInfo2.FullName, Path.Combine(destination, directoryInfo2.Name), overwrite);
		}
	}
}
