using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Dataedo.App.Onboarding.Home.Controls;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;

namespace Dataedo.App.Onboarding.Home.Support;

internal class LayoutSupport
{
	public static void SetSizes(Control control, LayoutControlItem gridViewLayoutControlItem, GridView gridView, int itemsCount, int maxItemsCount, LayoutControlItem[] otherControls, bool databasesGridIsExpanded = false)
	{
		gridView.BestFitColumns();
		ScrollInfo scrollInfo = (ScrollInfo)typeof(GridView).GetField("scrollInfo", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(gridView);
		int limitedHeight = 20 * (((itemsCount >= maxItemsCount) ? maxItemsCount : itemsCount) + gridViewLayoutControlItem.Padding.Top + gridViewLayoutControlItem.Padding.Bottom);
		int num = 35;
		int maxHeight = 20 * (((itemsCount >= num) ? num : itemsCount) + gridViewLayoutControlItem.Padding.Top + gridViewLayoutControlItem.Padding.Bottom);
		SetHeightForDatabasesListUserControl(control, gridViewLayoutControlItem, databasesGridIsExpanded, limitedHeight, maxHeight);
		if (control is LinksUserControl || control is LinksToObjectsUserControl)
		{
			limitedHeight = 25 * ((itemsCount >= maxItemsCount) ? maxItemsCount : itemsCount) + gridViewLayoutControlItem.Padding.Top + gridViewLayoutControlItem.Padding.Bottom;
			control.Height = limitedHeight + control.Padding.Top + control.Padding.Bottom + otherControls.Where((LayoutControlItem x) => x.Visible).Sum((LayoutControlItem x) => x.Height) + (scrollInfo.HScrollVisible ? scrollInfo.HScrollSize : 0) + 20;
		}
		else
		{
			control.Height = gridViewLayoutControlItem.Height + control.Padding.Top + control.Padding.Bottom + otherControls.Where((LayoutControlItem x) => x.Visible).Sum((LayoutControlItem x) => x.Height) + (scrollInfo.HScrollVisible ? scrollInfo.HScrollSize : 0) + 20;
		}
	}

	private static void SetHeightForDatabasesListUserControl(Control control, LayoutControlItem gridViewLayoutControlItem, bool databasesGridIsExpanded, int limitedHeight, int maxHeight)
	{
		if (control is DatabasesListUserControl)
		{
			if (databasesGridIsExpanded)
			{
				Size size3 = (gridViewLayoutControlItem.MaxSize = (gridViewLayoutControlItem.MinSize = new Size(0, maxHeight)));
				gridViewLayoutControlItem.Height = maxHeight;
			}
			else
			{
				Size size3 = (gridViewLayoutControlItem.MaxSize = (gridViewLayoutControlItem.MinSize = new Size(0, limitedHeight)));
				gridViewLayoutControlItem.Height = limitedHeight;
			}
		}
	}
}
