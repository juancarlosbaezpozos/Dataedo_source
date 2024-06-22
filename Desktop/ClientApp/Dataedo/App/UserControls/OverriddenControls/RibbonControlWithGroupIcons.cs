using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.ViewInfo;

namespace Dataedo.App.UserControls.OverriddenControls;

public class RibbonControlWithGroupIcons : RibbonControl
{
	private readonly Dictionary<string, ImageData> groupImages;

	private bool toolTipInitialized;

	public Dictionary<string, ImageData> GroupImages => groupImages;

	public RibbonControlWithGroupIcons()
	{
		groupImages = new Dictionary<string, ImageData>();
	}

	private void ToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		if (e.SelectedControl != this)
		{
			return;
		}
		RibbonHitInfo ribbonHitInfo = CalcHitInfo(e.ControlMousePosition);
		if (ribbonHitInfo.HitTest != RibbonHitTest.PageGroupCaption || !(ribbonHitInfo.GetType().GetProperty("PageGroupInfo", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ribbonHitInfo, null) is MyRibbonPageGroupViewInfo myRibbonPageGroupViewInfo))
		{
			return;
		}
		ImageData imageData = myRibbonPageGroupViewInfo.ImageData;
		if (imageData != null)
		{
			Rectangle imageRectangle = ((MyRibbonPanelGroupPainter)myRibbonPageGroupViewInfo.GroupPainter).GetImageRectangle(myRibbonPageGroupViewInfo.GetDrawInfo().TextBounds, imageData.Image.Size);
			if (imageRectangle.Contains(e.ControlMousePosition))
			{
				e.Info = new ToolTipControlInfo(imageRectangle, imageData.ToolTip);
			}
		}
	}

	protected override RibbonViewInfo CreateViewInfo()
	{
		return new MyRibbonViewInfo(this);
	}

	public void AddImageToGroupCaption(string groupName, Image image, string tooltip)
	{
		if (string.IsNullOrWhiteSpace(groupName) || image == null || groupImages.ContainsKey(groupName))
		{
			return;
		}
		RibbonPageGroup groupByName = GetGroupByName(groupName);
		if (groupByName == null)
		{
			return;
		}
		groupByName.Text += " ";
		groupImages.Add(groupName, new ImageData(image, tooltip));
		if (!toolTipInitialized)
		{
			if (ToolTipController == null)
			{
				ToolTipController = ToolTipController.DefaultController;
			}
			ToolTipController.GetActiveObjectInfo += ToolTipController_GetActiveObjectInfo;
		}
	}
}
