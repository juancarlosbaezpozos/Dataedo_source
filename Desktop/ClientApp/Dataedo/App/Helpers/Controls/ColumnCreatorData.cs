using System;
using DevExpress.XtraGrid;
using DevExpress.XtraTreeList.Columns;

namespace Dataedo.App.Helpers.Controls;

public class ColumnCreatorData<T>
{
	public string Caption { get; set; }

	public string FieldName { get; set; }

	public Func<T, object> GetValue { get; set; }

	public bool CustomSort { get; set; }

	public TreeListColumn CreateColumn(int index)
	{
		return new TreeListColumn
		{
			Caption = Caption,
			FieldName = FieldName,
			Name = FieldName + "TreeListColumn" + index,
			Visible = true,
			VisibleIndex = index,
			SortMode = (CustomSort ? ColumnSortMode.Custom : ColumnSortMode.Default)
		};
	}
}
