using System.Drawing;
using Dataedo.App.Tools.UI;
using DevExpress.Utils.Extensions;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.Forms.Helpers;

public class OverlayTextPainterEx : OverlayTextPainter
{
	private TextPosition _pos;

	public OverlayTextPainterEx(TextPosition pos, string text)
		: base(text)
	{
		_pos = pos;
		base.Font = GetFont(pos);
		SetColor();
	}

	public OverlayTextPainterEx(TextPosition pos)
	{
		_pos = pos;
		base.Font = GetFont(pos);
		SetColor();
	}

	private void SetColor()
	{
		base.Color = (SkinsManager.CurrentSkin.IsDarkTheme ? Color.FromArgb(175, Color.White) : Color.FromArgb(175, Color.Black));
	}

	private Font GetFont(TextPosition textPosition)
	{
		float emSize = 10f;
		switch (textPosition)
		{
		case TextPosition.Subtitle:
		case TextPosition.Percentage:
			emSize = 12f;
			break;
		case TextPosition.Title:
			emSize = 14f;
			break;
		}
		return new Font("Tahoma", emSize, FontStyle.Regular);
	}

	protected override Rectangle CalcTextBounds(OverlayLayeredWindowObjectInfoArgs drawArgs)
	{
		Size @this = CalcTextSize(drawArgs);
		int y = 0;
		switch (_pos)
		{
		case TextPosition.Title:
			y = drawArgs.ImageBounds.Top - @this.Height - 10;
			break;
		case TextPosition.Subtitle:
			y = drawArgs.ImageBounds.Bottom + 10;
			break;
		case TextPosition.Percentage:
			y = drawArgs.ImageBounds.Bottom + @this.Height + 20;
			break;
		}
		return @this.AlignWith(drawArgs.Bounds).WithY(y);
	}
}
