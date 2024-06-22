using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Scrolling;

namespace Dataedo.App.Onboarding.Home.Support;

public class ScrollHandler
{
	public static void HandleMouseWheel(MouseEventArgs e, GridControl gridControl, TablePanel tablePanel)
	{
		VCrkScrollBar vCrkScrollBar = gridControl?.Controls?.OfType<VCrkScrollBar>()?.FirstOrDefault();
		if (vCrkScrollBar == null || !vCrkScrollBar.Visible)
		{
			(e as DXMouseEventArgs).Handled = true;
			if (e.Delta > tablePanel.VerticalScroll.Value)
			{
				tablePanel.VerticalScroll.Value = 0;
			}
			else
			{
				tablePanel.VerticalScroll.Value -= e.Delta;
			}
		}
	}
}
