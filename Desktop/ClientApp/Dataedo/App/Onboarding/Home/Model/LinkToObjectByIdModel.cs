using System.Xml.Serialization;
using Dataedo.App.Onboarding.Home.Model.Interfaces;

namespace Dataedo.App.Onboarding.Home.Model;

public class LinkToObjectByIdModel : IToolTip
{
	[XmlElement]
	public string Text { get; set; }

	[XmlElement]
	public int DatabaseId { get; set; }

	[XmlElement]
	public string ObjectType { get; set; }

	[XmlElement]
	public int ObjectId { get; set; }

	[XmlElement(ElementName = "ToolTip")]
	public ToolTipModel ToolTip { get; set; }

	public LinkToObjectByIdModel()
	{
	}

	public LinkToObjectByIdModel(string text, int databaseId, string objectType, int objectId)
		: this()
	{
		Text = text;
		DatabaseId = databaseId;
		ObjectType = objectType;
		ObjectId = objectId;
	}

	public LinkToObjectByIdModel(string text, int databaseId, string objectType, int objectId, ToolTipModel toolTip)
		: this(text, databaseId, objectType, objectId)
	{
		ToolTip = toolTip;
	}
}
