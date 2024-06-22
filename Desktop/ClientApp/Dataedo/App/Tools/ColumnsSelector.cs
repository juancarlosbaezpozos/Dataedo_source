using System.Collections.Generic;
using System.Linq;

namespace Dataedo.App.Tools;

public static class ColumnsSelector
{
	public static void SetColumnsSelection(ISelectableColumn selectedColumn, IEnumerable<ISelectableColumn> columns)
	{
		if (selectedColumn.Selected)
		{
			SelectParentColumns(selectedColumn, columns);
		}
		else
		{
			UnselectSubColumns(selectedColumn, columns);
		}
	}

	public static void SelectParentColumns(ISelectableColumn column, IEnumerable<ISelectableColumn> columns)
	{
		ISelectableColumn selectableColumn = columns.FirstOrDefault((ISelectableColumn x) => x.ColumnId == column.ParentId && x.ColumnId != column.ColumnId);
		if (selectableColumn != null)
		{
			selectableColumn.Selected = column.Selected;
			SelectParentColumns(selectableColumn, columns);
		}
	}

	public static void UnselectSubColumns(ISelectableColumn column, IEnumerable<ISelectableColumn> columns)
	{
		if (column == null)
		{
			return;
		}
		foreach (ISelectableColumn item in columns.Where((ISelectableColumn x) => x.ParentId == column.ColumnId && x.ColumnId != column.ColumnId))
		{
			item.Selected = column.Selected;
			UnselectSubColumns(item, columns);
		}
	}
}
