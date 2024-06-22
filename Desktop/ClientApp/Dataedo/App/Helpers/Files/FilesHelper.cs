using System.IO;
using Dataedo.ConfigurationFileHelperLibrary;

namespace Dataedo.App.Helpers.Files;

public static class FilesHelper
{
	public static string ToReadableSize(double bytes)
	{
		string[] array = new string[5] { "B", "KB", "MB", "GB", "TB" };
		int num = 0;
		while (bytes >= 1024.0 && num < array.Length - 1)
		{
			num++;
			bytes /= 1024.0;
		}
		return $"{bytes:0.##} {array[num]}";
	}

	public static DirectoryInfo GetDefaultTempDirectory()
	{
		return new DirectoryInfo(ConfigurationFileHelper.GetConfFolderPath()).CreateSubdirectory("temp");
	}
}
