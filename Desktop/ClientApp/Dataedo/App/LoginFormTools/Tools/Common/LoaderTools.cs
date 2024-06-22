using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Tools.UI;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.LoginFormTools.Tools.Common;

internal static class LoaderTools
{
	public static OverlayWindowOptions GetOverlayWindowOptions(Control context)
	{
		Color backColor = SkinColors.TranslateColor(context.BackColor);
		if (context != null)
		{
			return GetOverlayWindowOptions(backColor);
		}
		return GetOverlayWindowOptions();
	}

	public static OverlayWindowOptions GetOverlayWindowOptions()
	{
		return GetOverlayWindowOptions(SkinsManager.CurrentSkin.ControlBackColor);
	}

	private static OverlayWindowOptions GetOverlayWindowOptions(Color backColor)
	{
		return new OverlayWindowOptions(backColor: backColor, opacity: 0.7, fadeIn: true, fadeOut: true);
	}
}
