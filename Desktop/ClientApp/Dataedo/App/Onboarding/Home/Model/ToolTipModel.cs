using System.Xml.Serialization;

namespace Dataedo.App.Onboarding.Home.Model;

public class ToolTipModel
{
	[XmlElement]
	public string Title { get; set; }

	[XmlElement]
	public string Text { get; set; }

	public ToolTipModel()
	{
	}

	public ToolTipModel(string title, string text)
		: this()
	{
		Title = title;
		Text = text;
	}
}
