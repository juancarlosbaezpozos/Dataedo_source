namespace Dataedo.App.UserControls.ImportFilter;

public class FilterFields
{
	public string SchemaFieldName { get; set; }

	public string NameFieldName { get; set; }

	public FilterFields(string schema, string name)
	{
		SchemaFieldName = schema;
		NameFieldName = name;
	}

	public string GetFilterString(FilterRule rule, string typeColumn = null, string parentTypeColumn = null)
	{
		if (typeColumn == null)
		{
			return GetFilterString(rule);
		}
		return GetFilterStringForDependencies(rule, typeColumn, parentTypeColumn);
	}

	private string GetFilterString(FilterRule rule)
	{
		string text = PrepareSchemaFilter(rule);
		string text2 = PrepareNameFilter(rule);
		if (rule.RuleType == FilterRuleType.Exclude && rule.HasEmptyValues)
		{
			return "UPPER(" + NameFieldName + ") LIKE '%' ";
		}
		if (string.IsNullOrWhiteSpace(text) ^ string.IsNullOrWhiteSpace(text2))
		{
			return " " + text + " " + text2 + " ";
		}
		return " " + text + " AND " + text2 + " ";
	}

	private string PrepareSchemaFilter(FilterRule rule)
	{
		if (!string.IsNullOrWhiteSpace(SchemaFieldName) && (!string.IsNullOrWhiteSpace(rule.Schema) || string.IsNullOrWhiteSpace(rule.Name)))
		{
			return "UPPER(" + SchemaFieldName + ") LIKE '" + rule.PreparedSchema + "'";
		}
		return string.Empty;
	}

	private string PrepareNameFilter(FilterRule rule)
	{
		if (!string.IsNullOrWhiteSpace(NameFieldName) && (!string.IsNullOrWhiteSpace(rule.Name) || string.IsNullOrWhiteSpace(rule.Schema)))
		{
			return "UPPER(" + NameFieldName + ") LIKE '" + rule.PreparedName + "'";
		}
		return string.Empty;
	}

	private string GetFilterStringForDependencies(FilterRule rule, string typeColumn, string parentTypeColumn = null)
	{
		string text = "";
		string text2 = "";
		string text3 = "";
		string text4 = "";
		switch (rule.ObjectType)
		{
		case FilterObjectTypeEnum.FilterObjectType.Any:
			text = (text2 = (text3 = (text4 = rule.PreparedName)));
			break;
		case FilterObjectTypeEnum.FilterObjectType.Table:
			text3 = rule.PreparedName;
			break;
		case FilterObjectTypeEnum.FilterObjectType.View:
			text4 = rule.PreparedName;
			break;
		case FilterObjectTypeEnum.FilterObjectType.Procedure:
			text = rule.PreparedName;
			break;
		case FilterObjectTypeEnum.FilterObjectType.Function:
			text2 = rule.PreparedName;
			break;
		}
		string text5 = PrepareSchemaFilterForDependencies(rule.ObjectType, rule.RuleType, rule.PreparedSchema, typeColumn);
		string text6 = PrepareSchemaFilterForDependencies(rule.ObjectType, rule.RuleType, rule.PreparedSchema, parentTypeColumn);
		return "(" + typeColumn + " <> 'TRIGGER' AND \r\n                " + text5 + "UPPER(" + NameFieldName + ") LIKE\r\n                CASE " + typeColumn + "\r\n                    WHEN 'PROCEDURE' THEN '" + text + "'\r\n                    WHEN 'FUNCTION' THEN '" + text2 + "'\r\n                    WHEN 'TABLE' THEN '" + text3 + "'\r\n                    WHEN 'VIEW' THEN '" + text4 + "'\r\n                END) OR\r\n                (" + typeColumn + " = 'TRIGGER' AND \r\n                " + text6 + "UPPER(" + NameFieldName + ") LIKE\r\n                CASE " + parentTypeColumn + "\r\n                    WHEN 'PROCEDURE' THEN '" + text + "'\r\n                    WHEN 'FUNCTION' THEN '" + text2 + "'\r\n                    WHEN 'TABLE' THEN '" + text3 + "'\r\n                    WHEN 'VIEW' THEN '" + text4 + "'\r\n                END)";
	}

	private string PrepareSchemaFilterForDependencies(FilterObjectTypeEnum.FilterObjectType objectType, FilterRuleType ruleType, string filter, string typeColumn)
	{
		string text = "";
		string text2 = "";
		string text3 = "";
		string text4 = "";
		switch (objectType)
		{
		case FilterObjectTypeEnum.FilterObjectType.Any:
			text = (text2 = (text3 = (text4 = filter)));
			break;
		case FilterObjectTypeEnum.FilterObjectType.Table:
			text3 = filter;
			break;
		case FilterObjectTypeEnum.FilterObjectType.View:
			text4 = filter;
			break;
		case FilterObjectTypeEnum.FilterObjectType.Procedure:
			text = filter;
			break;
		case FilterObjectTypeEnum.FilterObjectType.Function:
			text2 = filter;
			break;
		}
		if (!string.IsNullOrWhiteSpace(SchemaFieldName))
		{
			return " UPPER(" + SchemaFieldName + ") LIKE\r\n                CASE " + typeColumn + "\r\n                    WHEN 'PROCEDURE' THEN '" + text + "'\r\n                    WHEN 'FUNCTION' THEN '" + text2 + "'\r\n                    WHEN 'TABLE' THEN '" + text3 + "'\r\n                    WHEN 'VIEW' THEN '" + text4 + "'\r\n                END AND ";
		}
		return string.Empty;
	}
}
