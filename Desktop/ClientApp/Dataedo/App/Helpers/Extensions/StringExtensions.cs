using System;
using System.Collections.Generic;
using System.Linq;

namespace Dataedo.App.Helpers.Extensions;

public static class StringExtensions
{
	public static string TrimEnd(this string source, string value)
	{
		if (!source.EndsWith(value))
		{
			return source;
		}
		return source.Remove(source.LastIndexOf(value));
	}

	public static string DeletePrefixIfExists(this string source, string prefix)
	{
		if (source == null)
		{
			return source;
		}
		if (source.StartsWith(prefix))
		{
			return source.Substring(prefix.Length);
		}
		return source;
	}

	public static string GetFirstLine(this string source)
	{
		if (source == null)
		{
			return source;
		}
		return source.Split('\r', '\n').FirstOrDefault();
	}

	public static IEnumerable<string> Split(this string str, int chunkSize)
	{
		if (string.IsNullOrEmpty(str) || str.Length <= chunkSize || chunkSize <= 0)
		{
			yield return str;
			yield break;
		}
		for (int i = 0; i < str.Length; i += chunkSize)
		{
			yield return str.Substring(i, Math.Min(chunkSize, str.Length - i));
		}
	}
}
