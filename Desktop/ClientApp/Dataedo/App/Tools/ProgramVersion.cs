using System;
using System.Configuration;
using System.Reflection;

namespace Dataedo.App.Tools;

public static class ProgramVersion
{
	private const string DevelopingVersion = "DevelopingVersion";

	public static readonly int Major = Assembly.GetExecutingAssembly().GetName().Version.Major;

	public static readonly int Minor = Assembly.GetExecutingAssembly().GetName().Version.Minor;

	public static readonly int Build = Assembly.GetExecutingAssembly().GetName().Version.Build;

	public static readonly string ReleaseType = "";

	public static readonly string ReleaseTypeForUrl = (string.IsNullOrWhiteSpace(ReleaseType) ? string.Empty : ("_" + ReleaseType.Replace(" ", string.Empty)));

	public static readonly string DisplayNameWithFullVersion = (IsDevelopingVersion ? "DEV " : string.Empty) + "Dataedo Desktop " + VersionWithBuild + ReleaseType;

	private static bool? isDevelopingVersion;

	public static bool IsDevelopingVersion
	{
		get
		{
			if (!isDevelopingVersion.HasValue)
			{
				isDevelopingVersion = false;
				if (ConfigurationManager.AppSettings["DevelopingVersion"] != null && bool.TryParse(ConfigurationManager.AppSettings["DevelopingVersion"], out var result))
				{
					isDevelopingVersion = result;
				}
			}
			return isDevelopingVersion.Value;
		}
	}

	public static string VersionWithBitVersion => $"{VersionWithBuild} ({(Environment.Is64BitProcess ? 64 : 32)} bit)";

	public static string VersionWithBuild =>
		$"{VersionWithUpdate}.{Build}{(IsDevelopingVersion ? ".dev" : string.Empty)}{ReleaseType}";

	public static string VersionWithBuildForUrl => $"{VersionWithUpdate}.{Build}{ReleaseTypeForUrl}";

	private static string VersionWithUpdate => $"{Major}.{Minor}";

	public static string Copyright
	{
		get
		{
			if (Assembly.GetEntryAssembly() != null)
			{
				var customAttributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), inherit: false);
				if (customAttributes != null && customAttributes.Length != 0)
				{
					return ((AssemblyCopyrightAttribute)customAttributes[0]).Copyright;
				}
			}
			return string.Empty;
		}
	}
}
