using System;
using System.Text.RegularExpressions;

namespace Dataedo.App.Tools.CommandLine.Tools;

internal static class ParserHelper
{
	public static string ParseAll(string input)
	{
		input = ParseDate(input);
		input = ParseFolder(LastConnectionInfo.MyDocumentsPath, input);
		return input;
	}

	public static string ParseDate(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		string text = Regex.Match(input, "(\\{DateTime:)(.+)(\\})").Groups?[2]?.Value;
		if (text == null)
		{
			return input;
		}
		return Regex.Replace(input, "(\\{DateTime:.+\\})", DateTime.Now.ToString(text));
	}

	public static string ParseFolder(string folderPath, string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		return Regex.Replace(input, "(\\{MyDocuments\\})", folderPath);
	}
}
