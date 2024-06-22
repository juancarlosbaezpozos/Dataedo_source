using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Dataedo.App.Tools.UpdateBar;

[XmlType("Version")]
public class UpdateVersion
{
	[XmlIgnore]
	private string releaseType = "";

	[XmlElement("Major")]
	public int Major { get; set; }

	[XmlElement("Minor")]
	public int Minor { get; set; }

	[XmlElement("Build")]
	public int Build { get; set; }

	[XmlElement("ReleaseType")]
	public string ReleaseType
	{
		get
		{
			return releaseType;
		}
		set
		{
			releaseType = value;
		}
	}

	public static UpdateVersion LoadFromXElement(XElement xml)
	{
		if (xml == null)
		{
			return new UpdateVersion
			{
				Major = ProgramVersion.Major,
				Minor = ProgramVersion.Minor,
				Build = ProgramVersion.Build,
				ReleaseType = ProgramVersion.ReleaseType
			};
		}
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateVersion));
		using XmlReader xmlReader = xml.CreateReader();
		return (UpdateVersion)xmlSerializer.Deserialize(xmlReader);
	}
}
