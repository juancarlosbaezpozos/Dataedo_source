using System.Collections.Generic;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Tools;

public static class CellSelector
{
	public static void SelectCells(IEnumerable<GridCell> cells, GridView gridView)
	{
		foreach (GridCell cell in cells)
		{
			gridView.SelectCell(cell);
		}
	}

	public static void UnselectCells(IEnumerable<GridCell> cells, GridView gridView)
	{
		foreach (GridCell cell in cells)
		{
			gridView.UnselectCell(cell);
		}
	}

	public static void SelectEditableCells(IEnumerable<GridCell> cells, IEnumerable<GridCell> editableCells, GridView gridView)
	{
		UnselectCells(cells, gridView);
		SelectCells(editableCells, gridView);
	}
}
