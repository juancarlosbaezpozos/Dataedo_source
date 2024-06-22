using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools;

public class ClassificatorCustomFieldItem
{
	public string CustomFieldTitle { get; }

	public int? CustomFieldId { get; set; }

	public string ClassificatorCustomFieldTitle { get; set; }

	public string CustomFieldIdFieldName { get; set; }

	public string CustomFieldValueFieldName { get; set; }

	public string CustomFieldDefinition { get; set; }

	public CustomFieldTypeEnum.CustomFieldType? CustomFieldType { get; set; }

	public ClassificatorCustomFieldItem()
	{
	}

	public ClassificatorCustomFieldItem(string title, int? id, string classificatorCustomFieldTitle, CustomFieldTypeEnum.CustomFieldType? customFieldType, string definition)
	{
		CustomFieldTitle = title.Trim();
		CustomFieldId = id;
		ClassificatorCustomFieldTitle = classificatorCustomFieldTitle.Trim();
		CustomFieldType = customFieldType;
		CustomFieldDefinition = definition;
	}
}
