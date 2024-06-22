using Dataedo.Model.Data.Interfaces;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Tools;

public class BulkCopyWithBlockedIsSelectedCells<T> : BaseWithBlockedCellsBulkCopy, IBulkCopy where T : class, ISelectable
{
	public override bool IsCellBlocked(GridCell selectedCell, GridView grid)
	{
		if ((grid.GetRow(selectedCell.RowHandle) as T).IsSelected)
		{
			return selectedCell.Column.FieldName == "IsSelected";
		}
		return true;
	}
}
