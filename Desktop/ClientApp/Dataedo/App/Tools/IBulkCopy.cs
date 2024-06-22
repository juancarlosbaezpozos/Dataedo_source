using System.Windows.Forms;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Tools;

public interface IBulkCopy
{
	bool IsCopying { get; set; }

	void SetValueForSelectedColumns(GridView grid, string value, bool hasSingleValue, KeyEventArgs e);

	bool IsValueProper(string value, GridColumn cell);

	object GetProperValue(string value, GridColumn column);

	void SetValues(GridView grid, string value, int rowHandle, GridColumn column);

	bool ShowDialog(int changedCellsCount, string value);
}
