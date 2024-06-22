using System.Text.RegularExpressions;

namespace Dataedo.App.Tools.Export.XmlExportTools.Tools;

public static class XmlStringExtensions
{
	private static Regex XmlValidStringRegex;

	static XmlStringExtensions()
	{
		XmlValidStringRegex = new Regex("([\0-\b]|[\v-\f]|[\u000e-\u001f])", RegexOptions.Compiled);
	}

	public static string ToXmlValidString(this string value)
	{
		if (value == null)
		{
			return null;
		}
		return XmlValidStringRegex.Replace(value, string.Empty);
	}
}
