using System.Xml.Serialization;

namespace Dataedo.App.Onboarding.Home.Model.Sections;

public class DatabasesListSection : SectionModel
{
	[XmlElement]
	public int? RowsCount { get; set; }

	public DatabasesListSection()
	{
	}

	public DatabasesListSection(string title, bool ifAnyUserDatabases, bool ifNoUserDatabases)
		: base(title, ifAnyUserDatabases, ifNoUserDatabases)
	{
	}
}
