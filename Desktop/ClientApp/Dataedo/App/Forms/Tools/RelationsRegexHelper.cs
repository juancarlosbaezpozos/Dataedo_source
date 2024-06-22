using System.Text.RegularExpressions;

namespace Dataedo.App.Forms.Tools;

internal static class RelationsRegexHelper
{
	public static string GetRelationName(string fkTableName, string pkTableName)
	{
		Regex regex = new Regex("\\s|\\.");
		string text = regex.Replace(fkTableName, "_") ?? "";
		string text2 = regex.Replace(pkTableName, "_") ?? "";
		if (("fk_" + text2 + "_" + text).Length <= 250)
		{
			return "fk_" + text2 + "_" + text;
		}
		return ("fk_" + text2 + "_" + text).Substring(0, 249);
	}

	public static string GetPartialRelationName(string fkTableName)
	{
		Regex regex = new Regex("\\s|\\.");
		return "fk_" + regex.Replace(fkTableName, "_");
	}
}
