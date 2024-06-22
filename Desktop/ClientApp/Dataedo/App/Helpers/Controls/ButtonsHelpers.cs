using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;

namespace Dataedo.App.Helpers.Controls;

public class ButtonsHelpers
{
	public static void AddSuperTip(SimpleButton button, string text)
	{
		if (button.SuperTip == null)
		{
			button.SuperTip = new SuperToolTip();
		}
		if (button.SuperTip.Items.Count == 0)
		{
			button.SuperTip.Items.Add(text);
		}
	}

	public static void AddSuperTip(BarButtonItem button, string text)
	{
		if (button.SuperTip == null)
		{
			button.SuperTip = new SuperToolTip();
		}
		if (button.SuperTip.Items.Count == 0)
		{
			button.SuperTip.Items.Add(text);
		}
	}
}
