using System.Drawing;
using Dataedo.App.Tools.UI;

namespace Dataedo.App.Tools;

public static class NodeColors
{
	public const string NodeBlueString = "Blue";

	public const string NodeGreenString = "Green";

	public const string NodeRedString = "Red";

	public const string NodeYellowString = "Yellow";

	public const string NodePurpleString = "Purple";

	public const string NodeOrangeString = "Orange";

	public const string NodeCyanString = "Cyan";

	public const string NodeLimeString = "Lime";

	public const string NodeDefaultString = "Default";

	public static string GetNodeColorString(Color? color)
	{
		if (SkinsManager.DefaultSkin.ErdNodeBlue.Equals(color))
		{
			return "Blue";
		}
		if (SkinsManager.DefaultSkin.ErdNodeGreen.Equals(color))
		{
			return "Green";
		}
		if (SkinsManager.DefaultSkin.ErdNodeRed.Equals(color))
		{
			return "Red";
		}
		if (SkinsManager.DefaultSkin.ErdNodeYellow.Equals(color))
		{
			return "Yellow";
		}
		if (SkinsManager.DefaultSkin.ErdNodePurple.Equals(color))
		{
			return "Purple";
		}
		if (SkinsManager.DefaultSkin.ErdNodeOrange.Equals(color))
		{
			return "Orange";
		}
		if (SkinsManager.DefaultSkin.ErdNodeCyan.Equals(color))
		{
			return "Cyan";
		}
		if (SkinsManager.DefaultSkin.ErdNodeLime.Equals(color))
		{
			return "Lime";
		}
		return "Default";
	}

	public static Color GetNodeColor(string color)
	{
		return color switch
		{
			"Blue" => SkinsManager.DefaultSkin.ErdNodeBlue, 
			"Green" => SkinsManager.DefaultSkin.ErdNodeGreen, 
			"Red" => SkinsManager.DefaultSkin.ErdNodeRed, 
			"Yellow" => SkinsManager.DefaultSkin.ErdNodeYellow, 
			"Purple" => SkinsManager.DefaultSkin.ErdNodePurple, 
			"Orange" => SkinsManager.DefaultSkin.ErdNodeOrange, 
			"Cyan" => SkinsManager.DefaultSkin.ErdNodeCyan, 
			"Lime" => SkinsManager.DefaultSkin.ErdNodeLime, 
			_ => SkinsManager.DefaultSkin.ErdNodeDefault, 
		};
	}

	public static Color GetColorFromHtml(string color)
	{
		try
		{
			GetNodeColorString(ColorTranslator.FromHtml(color));
			return ColorTranslator.FromHtml(color);
		}
		catch
		{
			return SkinsManager.CurrentSkin.ErdNodeDefault;
		}
	}
}
