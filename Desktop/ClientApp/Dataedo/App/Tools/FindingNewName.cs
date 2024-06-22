using System.Collections.Generic;

namespace Dataedo.App.Tools;

public class FindingNewName
{
	public static string GetNewName(bool notExists, string defaultTitle, List<string> foundNumbers)
	{
		if (notExists)
		{
			return defaultTitle;
		}
		List<int> list = new List<int>();
		foreach (string foundNumber in foundNumbers)
		{
			int.TryParse(foundNumber, out var result);
			list.Add(result);
		}
		int num;
		if (list.Count != 0)
		{
			list.Add(1);
			list.Sort();
			num = list[list.Count - 1] + 1;
		}
		else
		{
			num = 2;
		}
		if (num < 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (i >= list.Count - 1 || list[i] + 1 < list[i + 1])
				{
					num = list[i] + 1;
					break;
				}
			}
		}
		if (num > 1)
		{
			return $"{defaultTitle} ({num})";
		}
		return defaultTitle;
	}
}
