using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Enums;
using DevExpress.Data;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.UserControls.PanelControls.CommonHelpers;

internal static class Sorting
{
	public static void ApplyCustom(params GridView[] gridView)
	{
		foreach (GridView obj in gridView)
		{
			obj.CustomColumnSort += GridView_CustomColumnSort;
			foreach (GridColumn column in obj.Columns)
			{
				column.SortMode = ColumnSortMode.Custom;
			}
		}
	}

	private static void GridView_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		if ((e.RowObject1 as StatedObject).RowState == ManagingRowsEnum.ManagingRows.ForAdding && (e.RowObject2 as StatedObject).RowState == ManagingRowsEnum.ManagingRows.ForAdding)
		{
			return;
		}
		if ((e.RowObject1 as StatedObject).RowState == ManagingRowsEnum.ManagingRows.ForAdding)
		{
			if (e.SortOrder == ColumnSortOrder.Ascending)
			{
				e.Result = 1;
			}
			else
			{
				e.Result = -1;
			}
			e.Handled = true;
		}
		else if ((e.RowObject2 as StatedObject).RowState == ManagingRowsEnum.ManagingRows.ForAdding)
		{
			if (e.SortOrder == ColumnSortOrder.Ascending)
			{
				e.Result = -1;
			}
			else
			{
				e.Result = 1;
			}
			e.Handled = true;
		}
	}
}
