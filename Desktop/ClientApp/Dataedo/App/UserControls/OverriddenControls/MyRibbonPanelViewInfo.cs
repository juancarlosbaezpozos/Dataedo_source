using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.ViewInfo;

namespace Dataedo.App.UserControls.OverriddenControls;

public class MyRibbonPanelViewInfo : RibbonPanelViewInfo
{
	public MyRibbonPanelViewInfo(MyRibbonViewInfo viewInfo)
		: base(viewInfo)
	{
	}

	protected override RibbonPageGroupViewInfo CreateGroupViewInfo(RibbonPageGroup group)
	{
		return new MyRibbonPageGroupViewInfo(base.ViewInfo as MyRibbonViewInfo, group);
	}
}
