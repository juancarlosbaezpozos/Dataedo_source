using System.Xml.Serialization;
using Dataedo.LicenseHelperLibrary.Repository;

namespace Dataedo.App.UserControls.ImportFilter;

[XmlRoot(ElementName = "r")]
public class FilterRule
{
	[XmlElement]
	public FilterRuleType RuleType { get; set; }

	[XmlElement]
	public FilterObjectTypeEnum.FilterObjectType ObjectType { get; set; }

	[XmlElement]
	public string Schema { get; set; }

	[XmlElement]
	public string Name { get; set; }

	public string PreparedSchema => LicenseHelper.EscapeInvalidCharacters(PrepareFilterValue(Schema));

	public string PreparedName => LicenseHelper.EscapeInvalidCharacters(PrepareFilterValue(Name));

	public bool IsTransparent
	{
		get
		{
			if (RuleType == FilterRuleType.Include && ObjectType == FilterObjectTypeEnum.FilterObjectType.Any)
			{
				return HasEmptyValues;
			}
			return false;
		}
	}

	public bool IsSemiTransparent
	{
		get
		{
			if (RuleType == FilterRuleType.Include)
			{
				return HasEmptyValues;
			}
			return false;
		}
	}

	public bool HasEmptyValues
	{
		get
		{
			if (string.IsNullOrWhiteSpace(Schema))
			{
				return string.IsNullOrWhiteSpace(Name);
			}
			return false;
		}
	}

	private string PrepareFilterValue(string value)
	{
		if (RuleType == FilterRuleType.Include || string.IsNullOrEmpty(Schema) || string.IsNullOrEmpty(Name))
		{
			if (!string.IsNullOrWhiteSpace(value))
			{
				return value.ToUpper();
			}
			return "%";
		}
		if (!string.IsNullOrWhiteSpace(value))
		{
			return value.ToUpper();
		}
		return string.Empty;
	}

	public FilterRule(FilterRuleType ruleType, FilterObjectTypeEnum.FilterObjectType type, string schema, string name)
	{
		RuleType = ruleType;
		ObjectType = type;
		Schema = schema;
		Name = name;
	}

	public FilterRule(FilterRowControl control)
		: this(control.RuleType, control.ObjectType, control.SchemaFilter, control.ValueFilter)
	{
	}

	public FilterRule()
	{
	}
}
