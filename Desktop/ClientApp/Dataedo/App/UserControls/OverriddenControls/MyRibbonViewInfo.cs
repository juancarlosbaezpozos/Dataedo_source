using DevExpress.XtraBars.Ribbon.ViewInfo;

namespace Dataedo.App.UserControls.OverriddenControls;

public class MyRibbonViewInfo : RibbonViewInfo
{
	public MyRibbonViewInfo(RibbonControlWithGroupIcons ribbonControl)
		: base(ribbonControl)
	{
	}

	protected override RibbonPanelViewInfo CreatePanelInfo()
	{
		return new MyRibbonPanelViewInfo(this);
	}
}
