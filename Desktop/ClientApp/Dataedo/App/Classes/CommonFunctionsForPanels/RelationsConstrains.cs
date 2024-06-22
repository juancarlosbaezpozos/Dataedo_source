using System.Drawing;
using Dataedo.App.Classes.Synchronize;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Classes.CommonFunctionsForPanels;

public static class RelationsConstrains
{
	private static Color ColorOfModifiedRows = Color.FromArgb(150, 255, 255, 224);

	public static void SetModifiedObjectRowColor(GridView gridView, RowCellStyleEventArgs e)
	{
		if (gridView.GetRow(e.RowHandle) is RelationContraintRow relationContraintRow && relationContraintRow.RowState != 0)
		{
			e.Appearance.BackColor = ColorOfModifiedRows;
		}
	}

	public static void SetModifiedColumnRowColor(GridView gridView, RowCellStyleEventArgs e)
	{
		if (gridView.GetRow(e.RowHandle) is RelationConstraintColumnRow relationConstraintColumnRow && relationConstraintColumnRow.RowState != 0)
		{
			e.Appearance.BackColor = ColorOfModifiedRows;
		}
	}
}
