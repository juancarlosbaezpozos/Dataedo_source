using System.ComponentModel;
using Dataedo.App.Tools.UI;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.UserControls;

public class GridViewWithRowHighlightUserControl : GridView
{
	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public bool RowHighlightingIsEnabled { get; set; } = true;


	public GridViewWithRowHighlightUserControl()
	{
		ChangeSelectedCellsColor();
		AddEventToHighlightFocusedRow();
	}

	private void ChangeSelectedCellsColor()
	{
		base.Appearance.SelectedRow.BackColor = SkinsManager.CurrentSkin.GridSelectionBackColor;
		base.Appearance.SelectedRow.Options.UseBackColor = true;
	}

	private void AddEventToHighlightFocusedRow()
	{
		base.RowStyle += delegate(object s, RowStyleEventArgs e)
		{
			if (s is GridView gridView && RowHighlightingIsEnabled)
			{
				int[] selectedRows = gridView.GetSelectedRows();
				int num = gridView.FocusedRowHandle;
				if (selectedRows.Length != 0 && e.RowHandle == num)
				{
					e.Appearance.BackColor = SkinsManager.CurrentSkin.GridHighlightRowBackColor;
				}
			}
		};
	}
}
