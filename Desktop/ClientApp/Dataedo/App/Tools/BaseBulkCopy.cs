using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.DataProcessing.Synchronize;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Tools;

public class BaseBulkCopy : BulkCopy, IBulkCopy
{
	public virtual void SetValueForSelectedColumns(GridView grid, string value, bool hasSingleValue, KeyEventArgs e)
	{
		if (hasSingleValue)
		{
			GridCell[] selectedCells = grid.GetSelectedCells();
			IEnumerable<GridCell> enumerable = selectedCells.Where((GridCell x) => !IsCellBlocked(x, grid));
			int num = enumerable.Count();
			if (num == 0)
			{
				return;
			}
			CellSelector.SelectEditableCells(selectedCells, enumerable, grid);
			if (!ShowDialog(num, value))
			{
				CellSelector.SelectCells(selectedCells, grid);
				return;
			}
			base.SetValues(grid, value, enumerable, hasSingleValue);
		}
		else
		{
			grid.PasteFromClipboard();
		}
		base.IsCopying = false;
		e.SuppressKeyPress = true;
		e.Handled = true;
	}

	public virtual bool IsCellBlocked(GridCell selectedCell, GridView grid)
	{
		return ColumnsHelper.IsCellBlocked(grid.GetRow(selectedCell.RowHandle) as BaseRow, selectedCell.Column.FieldName);
	}
}
