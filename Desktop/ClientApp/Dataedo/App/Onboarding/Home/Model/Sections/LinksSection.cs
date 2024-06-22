using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dataedo.App.Onboarding.Home.Model.Sections;

public class LinksSection : SectionModel
{
	[XmlArray]
	[XmlArrayItem(Type = typeof(LinkModel), ElementName = "Link")]
	public List<LinkModel> Links { get; set; }

	public LinksSection()
	{
	}

	public LinksSection(string title)
		: base(title)
	{
	}

	public LinksSection(string title, bool ifAnyUserDatabases, bool ifNoUserDatabases)
		: base(title, ifAnyUserDatabases, ifNoUserDatabases)
	{
	}
}
