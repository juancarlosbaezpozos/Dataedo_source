using System.Drawing;
using System.Linq;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Svg;

namespace Dataedo.App.Tools.UI;

internal static class SkinColors
{
	private class SkinColorDefinitions
	{
		public static readonly SkinColorDefinition KeyPaint = new SkinColorDefinition("Key Paint", isInverse: true);

		public static readonly SkinColorDefinition Brush = new SkinColorDefinition("Brush", isInverse: true);

		public static readonly SkinColorDefinition BrushMajor = new SkinColorDefinition("Brush Major");

		public static readonly SkinColorDefinition BrushMinor = new SkinColorDefinition("Brush Minor");

		public static readonly SkinColorDefinition Paint = new SkinColorDefinition("Paint");

		public static readonly SkinColorDefinition PaintHigh = new SkinColorDefinition("Paint High");

		public static readonly SkinColorDefinition AccentPaint = new SkinColorDefinition("Accent Paint");

		public static readonly SkinColorDefinition AccentBrush = new SkinColorDefinition("Accent Brush");
	}

	private class SkinColorDefinition
	{
		public string ColorName { get; set; }

		public bool IsInverse { get; set; }

		public SkinColorDefinition(string colorName)
		{
			ColorName = colorName;
		}

		public SkinColorDefinition(string colorName, bool isInverse)
			: this(colorName)
		{
			IsInverse = isInverse;
		}
	}

	public static Color ControlColorFromSystemColorsStored;

	public static Color ControlForeColorFromSystemColorsStored;

	public static Color ControlBorderStored;

	public static Color GridViewBandBackColor = GetSkinColor(SkinColorDefinitions.BrushMinor);

	public static Color GridViewBandForeColor = GetSkinColor(SkinColorDefinitions.Brush);

	public static Color InputBackColor = GetSkinColor(SkinColorDefinitions.PaintHigh);

	public static Color KeyPaint = GetSkinColor(SkinColorDefinitions.KeyPaint);

	public static Color BrushMinor = GetSkinColor(SkinColorDefinitions.BrushMinor);

	public static Color Brush = GetSkinColor(SkinColorDefinitions.Brush);

	public static Color Paint = GetSkinColor(SkinColorDefinitions.Paint);

	public static Color PaintHigh = GetSkinColor(SkinColorDefinitions.PaintHigh);

	public static Color AccentPaint = GetSkinColor(SkinColorDefinitions.AccentPaint);

	public static Color AccentBrush = GetSkinColor(SkinColorDefinitions.AccentBrush);

	public static Color ControlColorFromSystemColors => CommonSkins.GetSkin(UserLookAndFeel.Default.ActiveLookAndFeel).TranslateColor(SystemColors.Control);

	public static Color ControlForeColorFromSystemColors => CommonSkins.GetSkin(UserLookAndFeel.Default.ActiveLookAndFeel).TranslateColor(SystemColors.ControlText);

	public static Color FocusColorFromSystemColors => CommonSkins.GetSkin(UserLookAndFeel.Default.ActiveLookAndFeel).TranslateColor(SystemColors.Highlight);

	public static Color TranslateColor(Color color)
	{
		return CommonSkins.GetSkin(UserLookAndFeel.Default.ActiveLookAndFeel).TranslateColor(color);
	}

	public static void SetStoredColors()
	{
		ControlColorFromSystemColorsStored = ControlColorFromSystemColors;
		ControlForeColorFromSystemColorsStored = ControlForeColorFromSystemColors;
		ControlBorderStored = SkinsManager.CurrentSkin.ControlBorderColor;
	}

	private static Color GetSkinColor(SkinColorDefinition skinColor)
	{
		Color? color = (CommonSkins.GetSkin(UserLookAndFeel.Default)?.SvgPalettes?[Skin.DefaultSkinPaletteName]?.CustomPalette)?.Colors?.FirstOrDefault((SvgColor x) => x.Name == skinColor.ColorName)?.Value;
		if (!color.HasValue)
		{
			return GetDefultColor(skinColor);
		}
		return color.Value;
	}

	private static Color GetDefultColor(SkinColorDefinition skinColor)
	{
		if (skinColor.IsInverse)
		{
			return Color.Black;
		}
		return Color.White;
	}
}
