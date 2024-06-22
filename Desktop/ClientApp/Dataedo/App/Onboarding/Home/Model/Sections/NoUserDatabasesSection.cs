using System.Xml.Serialization;

namespace Dataedo.App.Onboarding.Home.Model.Sections;

public class NoUserDatabasesSection : SectionModel
{
	[XmlElement]
	public string Text { get; set; }

	public NoUserDatabasesSection()
	{
	}

	public NoUserDatabasesSection(string title, bool ifAnyUserDatabases, bool ifNoUserDatabases)
		: base(title, ifAnyUserDatabases, ifNoUserDatabases)
	{
	}
}
