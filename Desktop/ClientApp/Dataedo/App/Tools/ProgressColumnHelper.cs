using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraGrid.Views.Base;

namespace Dataedo.App.Tools;

public static class ProgressColumnHelper
{
	public static bool IsProgressColumnChanged(GridCell[] cells, string columnName)
	{
		return cells.Where((GridCell x) => x.Column.Caption.Equals(columnName)).Count() > 0;
	}

	public static bool IsProgressColumnChanged(IEnumerable<string> columnCaptions, string columnName)
	{
		return columnCaptions.Contains(columnName);
	}
}
