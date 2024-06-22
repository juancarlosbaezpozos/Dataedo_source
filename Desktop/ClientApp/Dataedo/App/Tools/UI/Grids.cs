using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Dataedo.App.Tools.UI;

internal static class Grids
{
	public static GridHitInfo GetBeforePopupHitInfo(object sender)
	{
		return ((sender as PopupMenu).Activator as PopupMenuShowingEventArgs).HitInfo;
	}

	public static bool GetBeforePopupIsRowClicked(object sender)
	{
		GridHitInfo hitInfo = ((sender as PopupMenu).Activator as PopupMenuShowingEventArgs).HitInfo;
		if (!hitInfo.InRowCell)
		{
			return hitInfo.InRow;
		}
		return true;
	}

	public static bool GetBeforePopupShouldCancel(object sender)
	{
		return ((sender as PopupMenu).Activator as PopupMenuShowingEventArgs).HitInfo.InColumn;
	}

	public static void ShowPopupMenu(this PopupMenu popupMenu, object gridViewContext, Point location, bool inRowCellOnly = true)
	{
		GridView gridView = gridViewContext as GridView;
		if (gridView.CalcHitInfo(location).InRowCell || !inRowCellOnly)
		{
			popupMenu.ShowPopup(gridView.GridControl.PointToScreen(location));
		}
	}

	public static void ShowPopupMenu(this PopupMenu popupMenu, object gridViewContext, PopupMenuShowingEventArgs popupMenuShowingEventArgs, bool inRowCellOnly = true)
	{
		if (popupMenuShowingEventArgs.HitInfo.InRowCell || !inRowCellOnly)
		{
			popupMenu.ShowPopup(Control.MousePosition, popupMenuShowingEventArgs);
		}
	}

	public static void SetPopupHeight(GridLookUpEdit gridLookUpEdit, int? maxItemsCount)
	{
		int realViewHeight = gridLookUpEdit.Properties.View.GetRealViewHeight(maxItemsCount);
		gridLookUpEdit.Properties.PopupFormMinSize = new Size(gridLookUpEdit.Width, realViewHeight);
		gridLookUpEdit.Properties.PopupFormSize = new Size(gridLookUpEdit.Width, realViewHeight);
	}

	public static void BindSettingPopupHeightMethod(this GridLookUpEdit gridLookUpEdit, int? maxItemsCount)
	{
		gridLookUpEdit.BeforePopup += delegate
		{
			SetPopupHeight(gridLookUpEdit, maxItemsCount);
		};
	}

	public static void BindSettingPopupHeightMethod(this GridLookUpEdit gridLookUpEdit)
	{
		gridLookUpEdit.BindSettingPopupHeightMethod(null);
	}

	public static void BindSettingPopupHeightMethod(this RepositoryItemGridLookUpEdit repositoryItemGridLookUpEdit, int? maxItemsCount)
	{
		repositoryItemGridLookUpEdit.BeforePopup += delegate(object s, EventArgs e)
		{
			GridLookUpEdit obj = s as GridLookUpEdit;
			int realViewHeight = obj.Properties.View.GetRealViewHeight(maxItemsCount);
			obj.Properties.PopupFormMinSize = new Size(0, realViewHeight);
			obj.Properties.PopupFormSize = new Size(0, realViewHeight);
		};
	}

	public static void BindSettingPopupHeightMethod(this RepositoryItemGridLookUpEdit repositoryItemGridLookUpEdit)
	{
		repositoryItemGridLookUpEdit.BindSettingPopupHeightMethod(null);
	}

	public static int GetRealViewHeight(this GridView gridView, int? maxItemsCount)
	{
		GridViewInfo gridViewInfo = gridView.GetViewInfo() as GridViewInfo;
		int val = gridViewInfo.CalcRealViewHeight(new Rectangle(0, 0, int.MaxValue, int.MaxValue));
		int val2 = int.MaxValue;
		if (maxItemsCount.HasValue && maxItemsCount > 0 && gridViewInfo.RowsInfo?[0] != null)
		{
			val2 = gridViewInfo.RowsInfo[0].Bounds.Height * maxItemsCount.Value;
		}
		return Math.Min(val, val2);
	}

	public static int GetRealViewHeight(this GridView gridView)
	{
		return gridView.GetRealViewHeight(null);
	}

	public static void ShowEditiorOnClick(object sender, MouseEventArgs e, string columnPropertyName)
	{
		GridView gridView = sender as GridView;
		gridView.CloseEditor();
		GridHitInfo gridHitInfo = gridView?.CalcHitInfo(e.Location);
		if (gridHitInfo != null && gridHitInfo.InRowCell && gridHitInfo.Column.FieldName == columnPropertyName)
		{
			gridView.FocusedColumn = gridHitInfo.Column;
			gridView.FocusedRowHandle = gridHitInfo.RowHandle;
			gridView.ShowEditor();
		}
	}

	public static void SetYellowRowColor(object sender, RowCellCustomDrawEventArgs e, bool condition)
	{
		if (condition)
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridAccentGridRowBackColor;
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridAccentGridRowForeColor;
		}
		else
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridGridRowBackColor;
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridGridRowForeColor;
		}
	}

	public static void SetDisabledRowColor(object sender, RowCellCustomDrawEventArgs e, bool condition)
	{
		if (condition)
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridDisabledGridRowForeColor;
		}
		else
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridGridRowBackColor;
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridGridRowForeColor;
		}
	}

	public static void DrawCenteredString(object sender, RowCellCustomDrawEventArgs e, string text)
	{
		e.Handled = true;
		e.Appearance.DrawBackground(e.Cache, e.Bounds);
		GridView gridView = sender as GridView;
		SizeF sizeF = e.Cache.CalcTextSize(text, gridView.Appearance.Row.Font);
		int num = (int)Math.Round(sizeF.Width, 0);
		int height = (int)Math.Round(sizeF.Height, 0);
		GridViewInfo gridViewInfo = gridView.GetViewInfo() as GridViewInfo;
		int num2 = 0;
		num2 = ((gridViewInfo.ColumnsInfo.LastColumnInfo.Bounds.Right < gridViewInfo.Bounds.Width) ? ((gridViewInfo.ColumnsInfo.LastColumnInfo.Bounds.Right - gridViewInfo.ColumnsInfo.FirstColumnInfo.Bounds.Left) / 2 - num / 2) : (gridViewInfo.Bounds.Width / 2 - num / 2));
		Rectangle bounds = new Rectangle(num2, e.Bounds.Top, num, height);
		e.Appearance.Font = gridView.Appearance.Row.Font;
		e.Appearance.DrawString(e.Cache, text, bounds);
	}
}
