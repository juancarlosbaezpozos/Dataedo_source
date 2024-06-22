using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;

namespace Dataedo.App.Tools.Helpers;

public static class GridColumnsHelper
{
	public static bool ShouldColumnBeGrayOut(GridColumn column)
	{
		if (column == null)
		{
			return false;
		}
		if (column.ColumnEdit is RepositoryItemProgressBar)
		{
			return true;
		}
		if (column.OptionsColumn.AllowEdit && column.OptionsColumn.AllowFocus)
		{
			return column.OptionsColumn.ReadOnly;
		}
		return true;
	}
}
