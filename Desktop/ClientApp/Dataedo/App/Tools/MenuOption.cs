using System.Drawing;
using Dataedo.App.Enums;
using Dataedo.App.Tools.UI;
using DevExpress.Utils.Svg;

namespace Dataedo.App.Tools;

public class MenuOption
{
	private string caption;

	private string description;

	private Color subtitleColor => SkinsManager.CurrentSkin.ListSelectionForeColor;

	public LoginMenuOptionEnum Value { get; private set; }

	public string DisplayName => caption + "<br>" + $"<color={subtitleColor.R}, {subtitleColor.G}, {subtitleColor.B}>{description}</color>";

	public int ImageIndex => (int)Value;

	public Image Image { get; set; }

	public SvgImage SvgImage { get; set; }

	public bool IsHelpIconVisible { get; set; }

	public string HelpIconToolTipHeader { get; set; }

	public string HelpIconToolTipText { get; set; }

	public MenuOption(LoginMenuOptionEnum value, string caption, string description)
	{
		Value = value;
		this.caption = caption;
		this.description = description;
	}

	public MenuOption(LoginMenuOptionEnum value, string caption, string description, Image image)
	{
		Value = value;
		this.caption = caption;
		this.description = description;
		Image = image;
	}

	public MenuOption(LoginMenuOptionEnum value, string caption, string description, Image image, string helpIconToolTipText)
		: this(value, caption, description, image)
	{
		HelpIconToolTipText = helpIconToolTipText;
		if (!string.IsNullOrEmpty(HelpIconToolTipText))
		{
			IsHelpIconVisible = true;
		}
	}

	public MenuOption(LoginMenuOptionEnum value, string caption, string description, Image image, SvgImage svgImage)
	{
		Value = value;
		this.caption = caption;
		this.description = description;
		Image = image;
		SvgImage = svgImage;
	}
}
