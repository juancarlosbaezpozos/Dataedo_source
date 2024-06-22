using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Dataedo.App.Tools.UI;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Dataedo.App.Helpers.Controls;

public class GridViewHelpers
{
	public static void SetColumnBestWidth(GridColumn column)
	{
		column.OptionsColumn.FixedWidth = false;
		int bestWidth = column.GetBestWidth();
		column.OptionsColumn.FixedWidth = true;
		column.Width = bestWidth;
	}

	public static void DrawBackgroundForProgressBar(RowCellCustomDrawEventArgs e, Color color)
	{
		e.DefaultDraw();
		ProgressBarViewInfo progressBarViewInfo = (e?.Cell as GridCellInfo)?.ViewInfo as ProgressBarViewInfo;
		ProgressBarObjectInfoArgs progressBarObjectInfoArgs = progressBarViewInfo?.ProgressInfo;
		if (progressBarViewInfo != null && progressBarObjectInfoArgs != null && e != null)
		{
			Rectangle progressBounds = progressBarObjectInfoArgs.ProgressBounds;
			Padding progressPadding = progressBarObjectInfoArgs.ProgressPadding;
			e.Cache.FillRectangle(color, new Rectangle(progressBounds.Right, progressBounds.Top, progressBarViewInfo.ClientRect.Width - progressBounds.Width - progressPadding.Right - progressPadding.Left, progressBounds.Height));
		}
	}

	public static void GrayOutNoneditableColumns(GridView gridView, RowCellCustomDrawEventArgs e)
	{
		if (gridView != null && (!gridView.IsFocusedView || !gridView.IsRowSelected(e.RowHandle)))
		{
			GridColumn column = e.Column;
			if (column == null || column.OptionsColumn?.AllowEdit != true)
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.GridNonEditableColumnsBackColor;
			}
		}
	}

	public static int GetGridPerfectHeight(GridView gridView)
	{
		Type type = gridView.GetType();
		GridViewInfo gridViewInfo = gridView.GetViewInfo() as GridViewInfo;
		ScrollInfo scrollInfo = type.GetField("scrollInfo", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(gridView) as ScrollInfo;
		int num = gridViewInfo.CalcRealViewHeight(new Rectangle(0, 0, int.MaxValue, int.MaxValue));
		if (scrollInfo.HScrollVisible)
		{
			num += scrollInfo.HScrollRect.Height;
		}
		return num;
	}
}
