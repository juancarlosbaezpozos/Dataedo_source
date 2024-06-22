using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Dataedo.App.Data.General;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.UserControls.ImportFilter;

[XmlRoot(ElementName = "Filter")]
public class FilterRulesCollection
{
	public bool IsTransparent
	{
		get
		{
			if (Rules == null || Rules.Count == 0)
			{
				return true;
			}
			return Rules.All((FilterRule x) => x.IsTransparent);
		}
	}

	public bool HasExcludedRules
	{
		get
		{
			if (Rules == null || Rules.Count == 0)
			{
				return false;
			}
			return Rules.Exists((FilterRule x) => x.RuleType == FilterRuleType.Exclude);
		}
	}

	[XmlElement(ElementName = "Rule")]
	public List<FilterRule> Rules { get; set; }

	public IEnumerable<FilterObjectTypeEnum.FilterObjectType> GetIncludedTypes(SharedDatabaseTypeEnum.DatabaseType? databaseType)
	{
		FilterObjectTypeEnum.FilterObjectType[] filterObjectTypes = GetFilterObjectTypes(databaseType);
		foreach (FilterObjectTypeEnum.FilterObjectType filterObjectType in filterObjectTypes)
		{
			if (!IsTypeExcluded(filterObjectType))
			{
				yield return filterObjectType;
			}
		}
	}

	private FilterObjectTypeEnum.FilterObjectType[] GetFilterObjectTypes(SharedDatabaseTypeEnum.DatabaseType? databaseType)
	{
		return DatabaseSupportFactory.GetDatabaseSupport(databaseType).GetFilterObjectTypes();
	}

	public bool IsTypeExcluded(FilterObjectTypeEnum.FilterObjectType type)
	{
		if (Rules == null || Rules.Count == 0)
		{
			return false;
		}
		bool num = Rules.Exists((FilterRule x) => x.RuleType == FilterRuleType.Exclude && (x.ObjectType == type || x.ObjectType == FilterObjectTypeEnum.FilterObjectType.Any) && x.HasEmptyValues);
		bool flag = !Rules.Exists((FilterRule x) => x.RuleType == FilterRuleType.Include && (x.ObjectType == type || x.ObjectType == FilterObjectTypeEnum.FilterObjectType.Any));
		return num || flag;
	}

	public static string CombineRelationsFilter(string first, string second, string joinWord = "AND")
	{
		if (!string.IsNullOrWhiteSpace(first) && !string.IsNullOrWhiteSpace(second))
		{
			return joinWord + " (" + first + " " + second + ")";
		}
		if (string.IsNullOrWhiteSpace(first) && !string.IsNullOrWhiteSpace(second))
		{
			return joinWord + " " + second;
		}
		if (!string.IsNullOrWhiteSpace(first) && string.IsNullOrWhiteSpace(second))
		{
			return joinWord + " " + first;
		}
		return string.Empty;
	}

	public static string CombineDependenciesFilter(string first, string second)
	{
		string result = string.Empty;
		bool flag = string.IsNullOrWhiteSpace(first);
		bool flag2 = string.IsNullOrWhiteSpace(second);
		if (flag && !flag2)
		{
			result = " AND " + second;
		}
		else if (!flag && flag2)
		{
			result = " AND " + first;
		}
		else if (!flag && !flag2)
		{
			result = " AND (" + first + " OR " + second + ")";
		}
		return result;
	}

	public FilterRulesCollection(IEnumerable<FilterRule> rules)
	{
		Rules = rules.ToList();
	}

	public FilterRulesCollection()
	{
		Rules = new List<FilterRule>();
	}

	public IEnumerable<FilterRule> GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType type)
	{
		return Rules.Where((FilterRule x) => x.ObjectType == FilterObjectTypeEnum.FilterObjectType.Any || x.ObjectType == type);
	}

	public string GetRulesXml()
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(FilterRulesCollection));
		string empty = string.Empty;
		using StringWriter stringWriter = new StringWriter();
		xmlSerializer.Serialize(stringWriter, this);
		return stringWriter.ToString();
	}

	public string GetDefaultFilter()
	{
		Rules.Clear();
		if (Rules.Count((FilterRule x) => x.RuleType == FilterRuleType.Include) == 0)
		{
			Rules.Add(new FilterRule(FilterRuleType.Include, FilterObjectTypeEnum.FilterObjectType.Any, string.Empty, string.Empty));
		}
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(FilterRulesCollection));
		string empty = string.Empty;
		using StringWriter stringWriter = new StringWriter();
		xmlSerializer.Serialize(stringWriter, this);
		return stringWriter.ToString();
	}

	public void SetRulesFromXml(string xml)
	{
		if (string.IsNullOrWhiteSpace(xml))
		{
			return;
		}
		try
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(FilterRulesCollection));
			using StringReader textReader = new StringReader(xml);
			FilterRulesCollection filterRulesCollection = (FilterRulesCollection)xmlSerializer.Deserialize(textReader);
			Rules = filterRulesCollection.Rules;
		}
		catch (Exception)
		{
		}
	}

	public string GetFilterString(FilterObjectTypeEnum.FilterObjectType objectType, string schemaFieldName, string fieldName)
	{
		return GetFilterString(null, null, objectType, schemaFieldName, fieldName);
	}

	public string GetFilterString(string joinWord, Restrictions restrictions, FilterObjectTypeEnum.FilterObjectType objectType, string schemaFieldName, string fieldName)
	{
		List<FilterFields> fields = new List<FilterFields>
		{
			new FilterFields(schemaFieldName, fieldName)
		};
		string filterString = GetFilterString(restrictions, objectType, fields);
		return MergeJoinWordWithFilter(joinWord, filterString);
	}

	public List<string> GetFilterTables()
	{
		List<string> list = new List<string>();
		foreach (FilterRule item in GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Table))
		{
			list.Add(item.PreparedName);
		}
		return list;
	}

	public string GetFilterString(string joinWord, Restrictions restrictions, FilterObjectTypeEnum.FilterObjectType objectType, string schemaFieldName1, string fieldName1, string schemaFieldName2, string fieldName2)
	{
		List<FilterFields> fields = new List<FilterFields>
		{
			new FilterFields(schemaFieldName1, fieldName1),
			new FilterFields(schemaFieldName2, fieldName2)
		};
		string filterString = GetFilterString(restrictions, objectType, fields);
		return MergeJoinWordWithFilter(joinWord, filterString);
	}

	private string MergeJoinWordWithFilter(string joinWord, string filter)
	{
		if (!string.IsNullOrWhiteSpace(joinWord) && !string.IsNullOrWhiteSpace(filter))
		{
			return " " + joinWord + " " + filter;
		}
		return filter;
	}

	public string GetFilterStringForDependencies(string schemaFieldName, string fieldName, string typeColumn, string parentTypeColumn = null)
	{
		List<FilterRule> rules = Rules;
		if (rules.Count() == 0 || rules.All((FilterRule x) => x.IsSemiTransparent))
		{
			return "(" + typeColumn + " IN('PROCEDURE', 'FUNCTION', 'TABLE', 'VIEW' )) OR\r\n                (" + typeColumn + " = 'TRIGGER' AND \r\n                " + parentTypeColumn + " IN('PROCEDURE', 'FUNCTION', 'TABLE', 'VIEW'))";
		}
		List<FilterFields> fieldNames = new List<FilterFields>
		{
			new FilterFields(schemaFieldName, fieldName)
		};
		List<FilterRule> source = rules.Where((FilterRule x) => x.RuleType == FilterRuleType.Include).ToList();
		List<FilterRule> rules2 = rules.Where((FilterRule x) => x.RuleType == FilterRuleType.Exclude).ToList();
		string text = " (";
		string rulesString = GetRulesString(source.Where((FilterRule x) => !x.IsSemiTransparent), fieldNames, FilterRuleType.Include, typeColumn, parentTypeColumn);
		if (!string.IsNullOrWhiteSpace(rulesString))
		{
			text += rulesString;
		}
		string rulesString2 = GetRulesString(rules2, fieldNames, FilterRuleType.Exclude, typeColumn, parentTypeColumn);
		if (!string.IsNullOrWhiteSpace(rulesString2))
		{
			if (!string.IsNullOrWhiteSpace(rulesString))
			{
				text += "AND";
			}
			text += rulesString2;
		}
		return text + ") ";
	}

	private string GetFilterString(Restrictions restrictions, FilterObjectTypeEnum.FilterObjectType objectType, List<FilterFields> fields)
	{
		if (restrictions != null && !restrictions.IsEmpty)
		{
			string restrictionsString = GetRestrictionsString(fields, restrictions);
			return " " + restrictionsString;
		}
		List<FilterRule> list = GetRulesByObjectType(objectType).ToList();
		list.Count();
		List<FilterRule> source = list.Where((FilterRule x) => x.RuleType == FilterRuleType.Include).ToList();
		List<FilterRule> rules = list.Where((FilterRule x) => x.RuleType == FilterRuleType.Exclude).ToList();
		if (source.Count() == 0)
		{
			list.Add(new FilterRule(FilterRuleType.Exclude, objectType, string.Empty, string.Empty));
			source = list.Where((FilterRule x) => x.RuleType == FilterRuleType.Include).ToList();
			rules = list.Where((FilterRule x) => x.RuleType == FilterRuleType.Exclude).ToList();
		}
		if (list.Count() == 0 || list.All((FilterRule x) => x.IsSemiTransparent))
		{
			return " ";
		}
		string text = " (";
		string rulesString = GetRulesString(source.Where((FilterRule x) => !x.IsSemiTransparent), fields, FilterRuleType.Include);
		if (!string.IsNullOrWhiteSpace(rulesString))
		{
			text += rulesString;
		}
		string rulesString2 = GetRulesString(rules, fields, FilterRuleType.Exclude);
		if (!string.IsNullOrWhiteSpace(rulesString2))
		{
			if (!string.IsNullOrWhiteSpace(rulesString))
			{
				text += "AND";
			}
			text += rulesString2;
		}
		return text + ") ";
	}

	private string GetRulesString(IEnumerable<FilterRule> rules, IEnumerable<FilterFields> fieldNames, FilterRuleType rulesType, string typeColumn = null, string parentTypeColumn = null)
	{
		string text = " ";
		int num = rules.Count();
		if (num == 0)
		{
			return text;
		}
		if (num > 1)
		{
			text += "(";
		}
		foreach (FilterRule rule in rules)
		{
			string text2 = string.Empty;
			if (rule.IsSemiTransparent)
			{
				continue;
			}
			if (rulesType == FilterRuleType.Exclude)
			{
				text2 += "NOT";
			}
			text2 += "(";
			foreach (FilterFields fieldName in fieldNames)
			{
				text2 = text2 + fieldName.GetFilterString(rule, typeColumn, parentTypeColumn) + " AND ";
			}
			text2 = text2.Remove(text2.Length - 5);
			switch (rulesType)
			{
			case FilterRuleType.Include:
				text2 += ") OR ";
				break;
			case FilterRuleType.Exclude:
				text2 += ") AND ";
				break;
			}
			text += text2;
		}
		switch (rulesType)
		{
		case FilterRuleType.Include:
			text = text.Remove(text.Length - 4);
			break;
		case FilterRuleType.Exclude:
			text = text.Remove(text.Length - 5);
			break;
		}
		if (num > 1)
		{
			text += ")";
		}
		return text + " ";
	}

	private string GetRestrictionsString(List<FilterFields> fields, Restrictions restrictions)
	{
		string text = " (";
		if (fields.Count == 0 || restrictions.IsEmpty)
		{
			return string.Empty;
		}
		IDatabaseSupport databaseSupport = DatabaseSupportFactory.GetDatabaseSupport(restrictions.DatabaseType);
		foreach (FilterFields field in fields)
		{
			string text2 = (string.IsNullOrWhiteSpace(field.SchemaFieldName) ? field.NameFieldName : databaseSupport.GetFilterRuleConcatenation(field.SchemaFieldName, field.NameFieldName));
			text = text + text2 + " " + restrictions.WhereClauseWord + " (" + restrictions.Restriction + ") " + restrictions.JoinWord + " ";
		}
		text = text.Remove(text.Length - (restrictions.JoinWord.Length + 2));
		return text + ") ";
	}
}
