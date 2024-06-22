using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Tools;

public abstract class BaseWithBlockedCellsBulkCopy : BaseBulkCopy, IBulkCopy
{
	public override void SetValueForSelectedColumns(GridView grid, string value, bool hasSingleValue, KeyEventArgs e)
	{
		if (hasSingleValue)
		{
			GridCell[] selectedCells;
			IEnumerable<GridCell> enumerable = from x in GetEditableCells(grid, value, hasSingleValue, out selectedCells)
				where !IsCellBlocked(x, grid)
				select x;
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
			CellSelector.SelectCells(selectedCells, grid);
			base.SetValues(grid, value, enumerable, hasSingleValue);
		}
		else
		{
			grid.PasteFromClipboard();
		}
		e.SuppressKeyPress = true;
		e.Handled = true;
		base.IsCopying = false;
	}

	public abstract override bool IsCellBlocked(GridCell selectedCell, GridView grid);

	public IEnumerable<GridCell> GetEditableCells(GridView grid, string value, bool isDeleting, out GridCell[] selectedCells)
	{
		IEnumerable<GridCell> enumerable = new List<GridCell>();
		selectedCells = grid.GetSelectedCells();
		enumerable = ((!isDeleting) ? selectedCells.Where((GridCell x) => !x.Column.ReadOnly && x.Column.OptionsColumn.AllowEdit) : selectedCells.Where((GridCell x) => !x.Column.ReadOnly && x.Column.OptionsColumn.AllowEdit));
		return enumerable.Where((GridCell x) => IsValueProper(value, x));
	}
}
