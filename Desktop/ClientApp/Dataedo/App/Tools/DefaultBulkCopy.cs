using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Tools;

public class DefaultBulkCopy : BulkCopy, IBulkCopy
{
	public void SetValueForSelectedColumns(GridView grid, string value, bool hasSingleValue, KeyEventArgs e)
	{
		if (!hasSingleValue)
		{
			grid.PasteFromClipboard();
		}
		else
		{
			GridCell[] selectedCells;
			IEnumerable<GridCell> editableCells = GetEditableCells(grid, value, hasSingleValue, out selectedCells);
			int num = editableCells.Count();
			if (num == 0)
			{
				return;
			}
			CellSelector.SelectEditableCells(selectedCells, editableCells, grid);
			if (!ShowDialog(num, value))
			{
				CellSelector.SelectCells(selectedCells, grid);
				return;
			}
			base.SetValues(grid, value, editableCells, hasSingleValue);
		}
		e.SuppressKeyPress = true;
		e.Handled = true;
		base.IsCopying = false;
	}

	public IEnumerable<GridCell> GetEditableCells(GridView grid, string value, bool isDeleting, out GridCell[] selectedCells)
	{
		IEnumerable<GridCell> enumerable = new List<GridCell>();
		selectedCells = grid.GetSelectedCells();
		enumerable = ((!isDeleting) ? selectedCells.Where((GridCell x) => !x.Column.ReadOnly && x.Column.OptionsColumn.AllowEdit) : selectedCells.Where((GridCell x) => !x.Column.ReadOnly && x.Column.OptionsColumn.AllowEdit));
		return enumerable.Where((GridCell x) => IsValueProper(value, x));
	}
}
