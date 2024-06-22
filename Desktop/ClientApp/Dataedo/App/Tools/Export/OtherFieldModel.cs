using Dataedo.Model.Data.OtherObjects;

namespace Dataedo.App.Tools.Export;

public class OtherFieldModel : SelectWithTitleObject
{
	public OtherFieldEnum.OtherField Field { get; set; }

	public OtherFieldModel(OtherFieldEnum.OtherField field, bool isSelected)
		: base(OtherFieldEnum.ToStringForDisplay(field), isSelected)
	{
		Field = field;
	}
}
