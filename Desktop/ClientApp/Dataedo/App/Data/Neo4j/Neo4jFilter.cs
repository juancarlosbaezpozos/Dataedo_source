using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.UserControls.ImportFilter;

namespace Dataedo.App.Data.Neo4j;

internal class Neo4jFilter
{
	private SynchronizeParameters synchronizeParameters;

	public Neo4jFilter(SynchronizeParameters synchronizeParameters)
	{
		this.synchronizeParameters = synchronizeParameters;
	}

	public string GetFilterStringForObjectType(string objectName, FilterObjectTypeEnum.FilterObjectType objectType, string prefix = null)
	{
		string includeFilterString = GetIncludeFilterString(objectType);
		string excludeFilterString = GetExcludeFilterString(objectType);
		if (string.IsNullOrEmpty(includeFilterString) && string.IsNullOrEmpty(excludeFilterString))
		{
			return string.Empty;
		}
		if (!string.IsNullOrEmpty(includeFilterString) && !string.IsNullOrEmpty(excludeFilterString))
		{
			return prefix + " " + objectName + " =~ '" + includeFilterString + "'\r\n                          AND " + objectName + " =~ '" + excludeFilterString + "'";
		}
		return prefix + " " + objectName + " =~ '" + includeFilterString + excludeFilterString + "'";
	}

	private string GetExcludeFilterString(FilterObjectTypeEnum.FilterObjectType objectType)
	{
		List<string> list = (from x in synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(objectType)
			where x.RuleType == FilterRuleType.Exclude
			select x.Name).ToList();
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item in list)
		{
			if (!string.IsNullOrEmpty(item))
			{
				stringBuilder.Append("(^(?!" + ConvertSQLRuleToNeo4j(item) + "$).*)");
			}
		}
		return stringBuilder.ToString();
	}

	private string GetIncludeFilterString(FilterObjectTypeEnum.FilterObjectType objectType)
	{
		List<string> list = (from x in synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(objectType)
			where x.RuleType == FilterRuleType.Include
			select x.Name).ToList();
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		foreach (string item in list)
		{
			if (!string.IsNullOrEmpty(item))
			{
				if (!flag)
				{
					stringBuilder.Append("||");
				}
				else
				{
					flag = false;
				}
				stringBuilder.Append("(\\\\b" + ConvertSQLRuleToNeo4j(item) + "\\\\b)");
			}
		}
		return stringBuilder.ToString();
	}

	private string ConvertSQLRuleToNeo4j(string rule)
	{
		return rule.Replace("%", ".*").Replace("_", ".");
	}
}
