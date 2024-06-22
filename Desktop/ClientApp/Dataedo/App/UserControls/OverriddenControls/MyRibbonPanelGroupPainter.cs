using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Ribbon.Drawing;

namespace Dataedo.App.UserControls.OverriddenControls;

public class MyRibbonPanelGroupPainter : RibbonPanelGroupPainter
{
	public override void DrawBackground(ObjectInfoArgs e)
	{
		base.DrawBackground(e);
		MyRibbonPageGroupViewInfo myRibbonPageGroupViewInfo = (MyRibbonPageGroupViewInfo)((RibbonDrawInfo)e).ViewInfo;
		if (myRibbonPageGroupViewInfo.ImageData != null)
		{
			Image image = myRibbonPageGroupViewInfo.ImageData.Image;
			Size size = image.Size;
			GroupObjectInfoArgs drawInfo = myRibbonPageGroupViewInfo.GetDrawInfo();
			if (drawInfo.TextBounds.Right + size.Width < drawInfo.CaptionBounds.Width)
			{
				e.Cache.DrawImage(image, GetImageRectangle(drawInfo.TextBounds, size));
			}
		}
	}

	public override void DrawCaption(ObjectInfoArgs e, string caption, Font font, Brush brush, Rectangle bounds, StringFormat format)
	{
		base.DrawCaption(e, caption, font, brush, bounds, format);
	}

	public Rectangle GetImageRectangle(Rectangle textBounds, Size imageSize)
	{
		return new Rectangle(textBounds.Right, textBounds.Top, imageSize.Width, imageSize.Height);
	}
}
