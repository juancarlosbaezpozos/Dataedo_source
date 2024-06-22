using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.MenuTree;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.Tools.DragDrop;

internal static class SourceAndTargetHelpers
{
	public static void RetrieveTargetFromGridAndDraggedFromTree<Target>(object sender, DragEventArgs e, out Target target, out DBTreeNode dragged) where Target : class
	{
		GridControl obj = sender as GridControl;
		Point pt = obj.PointToClient(new Point(e.X, e.Y));
		BaseView defaultView = obj.DefaultView;
		target = null;
		dragged = null;
		if (defaultView == null)
		{
			e.Effect = DragDropEffects.None;
			return;
		}
		if (!(e.Data.GetData(typeof(TreeListNode)) is TreeListNode treeListNode))
		{
			e.Effect = DragDropEffects.None;
			return;
		}
		dragged = treeListNode.TreeList.GetDataRecordByNode(treeListNode) as DBTreeNode;
		GridHitInfo gridHitInfo = defaultView.CalcHitInfo(pt) as GridHitInfo;
		if (!gridHitInfo.InRow || gridHitInfo.RowHandle == -2147483646)
		{
			e.Effect = DragDropEffects.None;
		}
		else
		{
			target = defaultView.GetRow(gridHitInfo.RowHandle) as Target;
		}
	}
}
