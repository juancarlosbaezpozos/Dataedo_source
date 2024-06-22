using System.Windows.Forms;

namespace Dataedo.App.UserControls;

public class ScrollableTableLayoutPanel : TableLayoutPanel
{
	public ScrollableTableLayoutPanel()
	{
		DoubleBuffered = true;
	}
}
