using System.Drawing;

namespace Dataedo.App.Tools.UI;

public static class ColorExtensions
{
	public static Color Invert(this Color color)
	{
		return Color.FromArgb(color.A, 255 - color.R, 255 - color.G, 255 - color.B);
	}

	public static Color? Invert(this Color? color)
	{
		if (!color.HasValue)
		{
			return null;
		}
		return color.Value.Invert();
	}
}
