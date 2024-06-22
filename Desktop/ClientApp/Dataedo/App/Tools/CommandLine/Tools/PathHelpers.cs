using System.IO;

namespace Dataedo.App.Tools.CommandLine.Tools;

internal static class PathHelpers
{
	public static string GetRootedOrRelative(string basePath, string path)
	{
		if (!string.IsNullOrEmpty(path) && !Path.IsPathRooted(path))
		{
			return Path.Combine(Path.GetDirectoryName(basePath), path);
		}
		return path;
	}
}
