using System.Reflection;

namespace Dataedo.Api.AppSettings;

public static class ProductVersion
{
	public static readonly int Major = Assembly.GetExecutingAssembly().GetName().Version!.Major;

	public static readonly int Minor = Assembly.GetExecutingAssembly().GetName().Version!.Minor;

	public static readonly int Build = Assembly.GetExecutingAssembly().GetName().Version!.Build;

	public static readonly string Version = $"{Major}.{Minor}.{Build}";
}
