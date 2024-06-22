using System.Collections.Generic;
using System.Linq;

namespace Dataedo.App.Tools.Export;

public class OtherFieldsSupport
{
	public List<OtherFieldModel> Fields { get; private set; }

	public OtherFieldsSupport()
	{
		Fields = new List<OtherFieldModel>
		{
			new OtherFieldModel(OtherFieldEnum.OtherField.Description, isSelected: true),
			new OtherFieldModel(OtherFieldEnum.OtherField.Title, isSelected: true),
			new OtherFieldModel(OtherFieldEnum.OtherField.DataType, isSelected: true),
			new OtherFieldModel(OtherFieldEnum.OtherField.Nullable, isSelected: true),
			new OtherFieldModel(OtherFieldEnum.OtherField.Identity, isSelected: true),
			new OtherFieldModel(OtherFieldEnum.OtherField.DefaultComputed, isSelected: true)
		};
	}

	public bool IsSelected(OtherFieldEnum.OtherField field)
	{
		return Fields.Any((OtherFieldModel x) => x.Field == field && x.IsSelected);
	}

	public void SetField(OtherFieldEnum.OtherField field, bool? value)
	{
		if (value.HasValue)
		{
			Fields.First((OtherFieldModel x) => x.Field == field).IsSelected = value.Value;
		}
	}

	public void SelectAll()
	{
		Fields?.ForEach(delegate(OtherFieldModel x)
		{
			x.IsSelected = true;
		});
	}

	public void UnselectAll()
	{
		Fields?.ForEach(delegate(OtherFieldModel x)
		{
			x.IsSelected = false;
		});
	}
}
