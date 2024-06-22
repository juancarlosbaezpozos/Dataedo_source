using System.Windows.Forms;
using Dataedo.App.Onboarding.Home.Model.Interfaces;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;

namespace Dataedo.App.Onboarding.Home.Support;

internal class ActionsSupport
{
	public static void GetActiveObjectInfo<T>(GridControl gridControl, GridView gridView, GridColumn gridColumn, ToolTipControllerGetActiveObjectInfoEventArgs e) where T : IToolTip
	{
		if (e.SelectedControl != gridControl)
		{
			return;
		}
		GridHitInfo gridHitInfo = gridView.CalcHitInfo(e.ControlMousePosition);
		if (gridHitInfo.HitTest == GridHitTest.RowCell && gridHitInfo.Column == gridColumn)
		{
			IToolTip toolTip = gridView.GetRow(gridHitInfo.RowHandle) as IToolTip;
			if (toolTip?.ToolTip != null)
			{
				e.Info = new ToolTipControlInfo($"{gridHitInfo.RowHandle}-{gridHitInfo.Column}", toolTip.ToolTip.Text, toolTip.ToolTip.Title);
			}
		}
	}

	public static T GetRowDataUnderCursor<T>(GridView gridView) where T : class
	{
		GridHitInfo gridHitInfo = gridView.CalcHitInfo(gridView.GridControl.PointToClient(Control.MousePosition));
		if (gridHitInfo.InRowCell)
		{
			int rowHandle = gridHitInfo.RowHandle;
			return gridView.GetRow(rowHandle) as T;
		}
		return null;
	}

	public static T GetRowDataUnderCursor<T>(TileView gridView) where T : class
	{
		TileViewHitInfo tileViewHitInfo = gridView.CalcHitInfo(gridView.GridControl.PointToClient(Control.MousePosition));
		if (tileViewHitInfo.InItem)
		{
			int rowHandle = tileViewHitInfo.RowHandle;
			return gridView.GetRow(rowHandle) as T;
		}
		return null;
	}
}
