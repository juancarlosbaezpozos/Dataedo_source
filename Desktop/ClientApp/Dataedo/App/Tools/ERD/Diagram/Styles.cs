using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Dataedo.App.Tools.ERD.Canvas;
using Dataedo.App.Tools.UI;

namespace Dataedo.App.Tools.ERD.Diagram;

internal static class Styles
{
	public static readonly int NodeHorizontalPadding;

	public static readonly int NodeVerticalPadding;

	public static readonly int PostItHorizontalPadding;

	public static readonly int PostItVerticalPadding;

	public static readonly int NodeIconSize;

	public static readonly int NodeTextSize;

	public static readonly int SvgNodeTextSize;

	public static readonly int SvgLinkTextSize;

	public static Font LinkFont;

	public static TextFormatFlags LinkFormatFlags;

	public static TextFormatFlags NodeFormatFlags;

	public static TextFormatFlags PostItFormatFlags;

	static Styles()
	{
		NodeHorizontalPadding = 5;
		NodeVerticalPadding = 7;
		PostItHorizontalPadding = 6;
		PostItVerticalPadding = 4;
		NodeIconSize = 12;
		NodeTextSize = 9;
		SvgNodeTextSize = 12;
		SvgLinkTextSize = 8;
		LinkFont = new Font("Arial", SvgLinkTextSize);
		LinkFormatFlags = TextFormatFlags.EndEllipsis | TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.TextBoxControl | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.NoPadding;
		NodeFormatFlags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.TextBoxControl | TextFormatFlags.VerticalCenter | TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.NoPadding;
		PostItFormatFlags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak | TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.NoPadding;
	}

	public static Color NodeBorderColor(RectangularObject node, CanvasObject.Output destination)
	{
		if (node.Selected)
		{
			if (destination != 0)
			{
				return SkinsManager.DefaultSkin.ErdSelectedNodeBorderColor;
			}
			return SkinsManager.CurrentSkin.ErdSelectedNodeBorderColor;
		}
		if (node.MouseOver)
		{
			if (destination != 0)
			{
				return SkinsManager.DefaultSkin.ErdSelectedNodeBorderColor;
			}
			return SkinsManager.CurrentSkin.ErdSelectedNodeBorderColor;
		}
		if (!SkinsManager.CurrentSkin.ErdUseNodeColorForHeaderBorder)
		{
			switch (destination)
			{
			case CanvasObject.Output.Image:
				break;
			default:
				return SkinsManager.DefaultSkin.ErdNodeBorderColor;
			case CanvasObject.Output.Control:
				return SkinsManager.CurrentSkin.ErdNodeBorderColor;
			}
		}
		if (destination != 0)
		{
			return SkinsManager.DefaultSkin.GetNodeColor(NodeColors.GetNodeColorString(node.Color));
		}
		return SkinsManager.CurrentSkin.GetNodeColor(NodeColors.GetNodeColorString(node.Color));
	}

	public static Color NodeBackgroundColor(RectangularObject node, CanvasObject.Output destination)
	{
		if (node.Selected)
		{
			return SkinsManager.CurrentSkin.ErdSelectedNodeColor;
		}
		if (destination != 0)
		{
			return SkinsManager.DefaultSkin.GetNodeColorLight(NodeColors.GetNodeColorString(node.Color));
		}
		return SkinsManager.CurrentSkin.GetNodeColorLight(NodeColors.GetNodeColorString(node.Color));
	}

	public static Pen LinkBorderPen(bool selected, bool userRelation, bool? hidden, CanvasObject.Output destination)
	{
		Pen pen = (selected ? ((destination == CanvasObject.Output.Control) ? new Pen(SkinsManager.CurrentSkin.ErdSelectedNodeBorderColor, 2f) : new Pen(SkinsManager.DefaultSkin.ErdSelectedNodeBorderColor, 2f)) : ((hidden != true) ? ((destination == CanvasObject.Output.Control) ? new Pen(SkinsManager.CurrentSkin.ErdLinkColor, 1f) : new Pen(SkinsManager.DefaultSkin.ErdLinkColor, 1f)) : ((destination == CanvasObject.Output.Control) ? new Pen(SkinsManager.CurrentSkin.ErdERDHiddenRelationColor, 1f) : new Pen(SkinsManager.DefaultSkin.ErdERDHiddenRelationColor, 1f))));
		if (userRelation)
		{
			float num = 6f / pen.Width;
			pen.DashStyle = DashStyle.Dash;
			pen.DashPattern = new float[2] { num, num };
		}
		return pen;
	}
}
