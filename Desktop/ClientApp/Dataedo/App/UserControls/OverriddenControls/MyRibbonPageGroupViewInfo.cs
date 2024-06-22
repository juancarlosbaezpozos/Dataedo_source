using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.ViewInfo;

namespace Dataedo.App.UserControls.OverriddenControls;

public class MyRibbonPageGroupViewInfo : RibbonPageGroupViewInfo
{
	private string GroupName => base.PageGroup?.Name;

	public ImageData ImageData
	{
		get
		{
			RibbonControlWithGroupIcons obj = base.Ribbon as RibbonControlWithGroupIcons;
			if (obj == null || obj.GroupImages?.ContainsKey(GroupName) != true)
			{
				return null;
			}
			return (base.Ribbon as RibbonControlWithGroupIcons)?.GroupImages[GroupName];
		}
	}

	public MyRibbonPageGroupViewInfo(MyRibbonViewInfo viewInfo, RibbonPageGroup group)
		: base(viewInfo, group)
	{
	}

	protected override RibbonPanelGroupPainter CreatePanelGroupPainter()
	{
		return new MyRibbonPanelGroupPainter();
	}
}
