using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dataedo.App.Onboarding.Home.Model.Sections;

public class LinksToObjectsSection : SectionModel
{
	[XmlArray]
	[XmlArrayItem(Type = typeof(LinkToObjectByIdModel), ElementName = "LinkToObjectById")]
	public List<LinkToObjectByIdModel> Links { get; set; }

	public LinksToObjectsSection()
	{
	}

	public LinksToObjectsSection(string title)
		: base(title)
	{
	}

	public LinksToObjectsSection(string title, bool ifAnyUserDatabases, bool ifNoUserDatabases)
		: base(title, ifAnyUserDatabases, ifNoUserDatabases)
	{
	}
}
