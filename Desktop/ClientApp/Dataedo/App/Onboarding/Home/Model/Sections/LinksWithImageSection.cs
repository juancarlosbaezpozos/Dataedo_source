using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

namespace Dataedo.App.Onboarding.Home.Model.Sections;

public class LinksWithImageSection : SectionModel
{
	[XmlElement]
	public Size TileSize { get; set; }

	[XmlElement]
	public Size ImageSize { get; set; }

	[XmlArray]
	[XmlArrayItem(Type = typeof(LinkWithImageModel), ElementName = "LinkWithImage")]
	public List<LinkWithImageModel> Links { get; set; }

	public LinksWithImageSection()
	{
	}

	public LinksWithImageSection(string title)
		: base(title)
	{
	}

	public LinksWithImageSection(string title, bool ifAnyUserDatabases, bool ifNoUserDatabases)
		: base(title, ifAnyUserDatabases, ifNoUserDatabases)
	{
	}
}
