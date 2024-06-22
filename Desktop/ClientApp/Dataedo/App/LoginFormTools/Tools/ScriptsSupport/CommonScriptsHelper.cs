using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Dataedo.App.Tools.Exceptions;

namespace Dataedo.App.LoginFormTools.Tools.ScriptsSupport;

internal static class CommonScriptsHelper
{
	public static List<string> SplitScript(string path)
	{
		List<string> list = new List<string>();
		string[] array = File.ReadAllLines(path);
		int num = array.Length;
		for (int i = 0; i < num; i++)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (; i < num && !array[i].Equals("GO"); i++)
				{
					if (!string.IsNullOrWhiteSpace(array[i]))
					{
						stringBuilder.AppendLine(array[i]);
					}
				}
				string text = stringBuilder.ToString();
				if (!string.IsNullOrWhiteSpace(text))
				{
					list.Add(text);
				}
			}
			catch (Exception exception)
			{
				GeneralExceptionHandling.Handle(exception, "Unable to process SQL script.");
			}
		}
		return list;
	}
}
