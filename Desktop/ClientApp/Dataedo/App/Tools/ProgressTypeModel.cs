using Dataedo.App.Enums;
using Dataedo.DataProcessing.CustomFields;

namespace Dataedo.App.Tools;

public class ProgressTypeModel
{
	public int? CustomFieldId { get; set; }

	public ProgressTypeEnum Type { get; set; }

	public string ColumnName { get; }

	public string FieldName { get; set; }

	public CustomFieldRowExtended CustomField { get; set; }

	public ProgressTypeModel(ProgressTypeEnum type, string columnName)
	{
		Type = type;
		ColumnName = columnName;
	}
}
