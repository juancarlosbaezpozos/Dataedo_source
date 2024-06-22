using System.Xml.Serialization;
using Dataedo.App.Onboarding.Home.Model.Interfaces;

namespace Dataedo.App.Onboarding.Home.Model;

public class LinkModel : IToolTip
{
	[XmlElement]
	public string Text { get; set; }

	[XmlElement]
	public string Link { get; set; }

	[XmlElement(ElementName = "ToolTip")]
	public ToolTipModel ToolTip { get; set; }

	public LinkModel()
	{
	}

	public LinkModel(string text, string link)
		: this()
	{
		Text = text;
		Link = link;
	}

	public LinkModel(string text, string link, ToolTipModel toolTip)
		: this(text, link)
	{
		ToolTip = toolTip;
	}
}
