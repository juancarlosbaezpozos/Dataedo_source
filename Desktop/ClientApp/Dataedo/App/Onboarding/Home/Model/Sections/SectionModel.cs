using System.Xml.Serialization;

namespace Dataedo.App.Onboarding.Home.Model.Sections;

[XmlInclude(typeof(NoUserDatabasesSection))]
[XmlInclude(typeof(DatabasesListSection))]
[XmlInclude(typeof(LinksToObjectsSection))]
[XmlInclude(typeof(LinksWithImageSection))]
[XmlInclude(typeof(LinksSection))]
public abstract class SectionModel
{
	[XmlElement]
	public string Title { get; set; }

	[XmlElement(ElementName = "Padding")]
	public PaddingModel Padding { get; set; } = new PaddingModel(0, 0, 0, 15);


	[XmlAttribute]
	public bool IfAnyUserDatabases { get; set; }

	[XmlAttribute]
	public bool IfNoUserDatabases { get; set; }

	public SectionModel()
	{
	}

	public SectionModel(string title)
	{
		Title = title;
		IfAnyUserDatabases = true;
		IfNoUserDatabases = true;
	}

	public SectionModel(string title, bool ifAnyUserDatabases, bool ifNoUserDatabases)
		: this(title)
	{
		Title = title;
		IfAnyUserDatabases = ifAnyUserDatabases;
		IfNoUserDatabases = ifNoUserDatabases;
	}

	public bool IsApplicable(bool isAnyDatabaseIsImportedByUser)
	{
		if (IfAnyUserDatabases != isAnyDatabaseIsImportedByUser)
		{
			return IfNoUserDatabases == !isAnyDatabaseIsImportedByUser;
		}
		return true;
	}
}
