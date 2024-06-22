using System;

namespace Dataedo.App.Forms.Tools;

public static class MultilineStringHelper
{
	public static string GetShortenedDisplayValue(string value)
	{
		if (value.Contains("\t"))
		{
			return GetResult(value);
		}
		if (value.Contains(Environment.NewLine))
		{
			return GetMultilineResult(value);
		}
		if (value.Length > 60)
		{
			return GetSubstring(value);
		}
		return value;
	}

	private static string GetResult(string value)
	{
		string substring = GetSubstring(value, "\t");
		if (substring.Contains(Environment.NewLine))
		{
			return GetMultilineResult(substring);
		}
		if (substring.Length > 60)
		{
			return GetSubstring(substring);
		}
		return substring;
	}

	private static string GetMultilineResult(string value)
	{
		string substring = GetSubstring(value, "\r");
		if (substring.Length > 60)
		{
			return GetSubstring(substring);
		}
		return substring;
	}

	private static string GetSubstring(string result)
	{
		return result.Substring(0, 60) + "...";
	}

	private static string GetSubstring(string value, string character)
	{
		return value.Substring(0, value.IndexOf(character)) + "...";
	}
}
