using Dataedo.App.Tools.CustomFields;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools;

public class ColumnBulkHelper : BulkHelper
{
	public void SetTemplate(CustomFieldsSupport customFieldsSupport, SharedObjectTypeEnum.ObjectType objectType)
	{
		foreach (CustomFieldRowExtended visibleField in customFieldsSupport.GetVisibleFields(objectType))
		{
			base.Template = base.Template + "\t" + visibleField.Title;
		}
	}
}
