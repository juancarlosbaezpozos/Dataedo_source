using Dataedo.App.Tools.UI;
using DevExpress.XtraTab;

namespace Dataedo.App.UserControls;

public class CustomXtraTabPage : XtraTabPage
{
	public CustomXtraTabPage()
	{
		base.Appearance.PageClient.BackColor = SkinColors.ControlColorFromSystemColors;
		base.Appearance.PageClient.Options.UseBackColor = true;
		base.Appearance.PageClient.ForeColor = SkinColors.ControlForeColorFromSystemColors;
		base.Appearance.PageClient.Options.UseForeColor = true;
	}
}
