using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Dataedo.App.Classes.CommonFunctionsForPanels;

public class DragRowsBase<T> : IDragRowsBase where T : class
{
	private GridHitInfo hitInfo;

	private readonly GridControl grid;

	protected readonly GridView gridView;

	public List<object> DraggedRows { get; set; }

	public DragRowsBase(GridView gridView)
	{
		this.gridView = gridView;
		grid = gridView.GridControl;
	}

	public void AddEvents()
	{
		grid.MouseDown += Grid_MouseDown;
		gridView.MouseMove += GridView_MouseMove;
		hitInfo = null;
	}

	protected virtual T GetData(int rowHandle)
	{
		return gridView.GetRow(rowHandle) as T;
	}

	private void Grid_MouseDown(object sender, MouseEventArgs e)
	{
		hitInfo = gridView.CalcHitInfo(new Point(e.X, e.Y));
	}

	private void GridView_MouseMove(object sender, MouseEventArgs e)
	{
		if (hitInfo == null || e.Button != MouseButtons.Left)
		{
			return;
		}
		Rectangle rectangle = new Rectangle(new Point(hitInfo.HitPoint.X - SystemInformation.DragSize.Width / 2, hitInfo.HitPoint.Y - SystemInformation.DragSize.Height / 2), SystemInformation.DragSize);
		if (!rectangle.Contains(new Point(e.X, e.Y)))
		{
			DraggedRows = new List<object>();
			int[] selectedRows = gridView.GetSelectedRows();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				DraggedRows.Add(GetData(selectedRows[i]));
			}
			T data = GetData(hitInfo.RowHandle);
			if (data != null)
			{
				int topRowIndex = gridView.TopRowIndex;
				int focusedRowHandle = gridView.FocusedRowHandle;
				grid.DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
				gridView.TopRowIndex = topRowIndex;
				gridView.FocusedRowHandle = focusedRowHandle;
			}
		}
	}
}
