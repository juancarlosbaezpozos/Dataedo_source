using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Dataedo.App.Onboarding.Home.Model.Sections;

namespace Dataedo.App.Onboarding.Home.Model;

[XmlRoot("WelcomePage")]
public class WelcomePageModel
{
	public static readonly string VersionValue = "1.0";

	[XmlIgnore]
	public DateTime CreationTime { get; private set; } = DateTime.Now;


	[XmlAttribute]
	public string Version { get; set; }

	[XmlElement]
	public string Title { get; set; }

	[XmlArray]
	[XmlArrayItem(Type = typeof(NoUserDatabasesSection))]
	[XmlArrayItem(Type = typeof(DatabasesListSection))]
	[XmlArrayItem(Type = typeof(LinksToObjectsSection))]
	[XmlArrayItem(Type = typeof(LinksWithImageSection))]
	[XmlArrayItem(Type = typeof(LinksSection))]
	public List<SectionModel> Sections { get; set; }
}
