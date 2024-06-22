using System.Collections.Generic;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Shared.Enums;

namespace Dataedo.App.ImportDescriptions.Tools;

public class ColumnImportDataModel : ImportDataModel
{
	public int? ColumnId { get; set; }

	public string ColumnName { get; set; }

	public override int? IdProperty => ColumnId;

	public override void ClearData()
	{
		base.ClearData();
		ColumnId = null;
	}

	public void SetData(ColumnObject data, IEnumerable<CustomFieldRowExtended> selectedCustomFields, SharedObjectTypeEnum.ObjectType parentType)
	{
		base.TableId = data.TableId;
		ColumnId = data.ColumnId;
		base.TableObjectType = parentType;
		base.Title.CurrentValue = data.Title;
		base.Description.CurrentValue = data.Description;
		SetCustomFields(selectedCustomFields, data);
	}
}
