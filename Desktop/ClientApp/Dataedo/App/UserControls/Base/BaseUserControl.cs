using System.Windows.Forms;
using Dataedo.App.Tools.UI;
using DevExpress.LookAndFeel;

namespace Dataedo.App.UserControls.Base;

public class BaseUserControl : UserControl
{
	public BaseUserControl()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
		ApplyStyle();
		UserLookAndFeel.Default.StyleChanged += delegate
		{
			ApplyStyle();
		};
	}

	protected void ApplyStyle()
	{
		BackColor = SkinColors.ControlColorFromSystemColors;
	}
}
