using System.Drawing;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ViewInfo;

namespace Dataedo.App.UserControls.RadioGroupWithImages;

public class RadioImageButtonPainter : SkinCheckObjectPainter
{
	public RadioImageButtonPainter(ISkinProvider provider)
		: base(provider)
	{
	}

	public override void DrawObject(ObjectInfoArgs e)
	{
		CheckObjectInfoArgs e2 = e as CheckObjectInfoArgs;
		DrawImage(e2);
		base.DrawObject(e);
	}

	protected virtual void DrawImage(CheckObjectInfoArgs e)
	{
		RadioGroupWithImages obj = (base.Provider as ControlUserLookAndFeel).OwnerControl as RadioGroupWithImages;
		RadioGroupItemViewInfo radioGroupItemViewInfo = e as RadioGroupItemViewInfo;
		Image itemImage = obj.GetItemImage(radioGroupItemViewInfo.Item);
		if (itemImage != null)
		{
			e.Cache.Paint.DrawImage(e.Graphics, itemImage, e.GlyphRect.Right + 2, e.GlyphRect.Top, new Rectangle(Point.Empty, itemImage.Size), enabled: true);
		}
	}

	public override void DrawCaption(ObjectInfoArgs e)
	{
		CheckObjectInfoArgs checkObjectInfoArgs = e as CheckObjectInfoArgs;
		e.Cache.Paint.DrawString(e.Cache, checkObjectInfoArgs.Caption, checkObjectInfoArgs.Appearance.Font, checkObjectInfoArgs.Appearance.GetForeBrush(checkObjectInfoArgs.Cache), new Rectangle(new Point(checkObjectInfoArgs.GlyphRect.Right + 6 + checkObjectInfoArgs.GlyphRect.Width, checkObjectInfoArgs.GlyphRect.Top), checkObjectInfoArgs.CaptionRect.Size), checkObjectInfoArgs.Appearance.GetStringFormat());
	}
}
