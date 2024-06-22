using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Dataedo.App.Tools.UpdateBar;

[XmlRoot("Versions")]
public class Versions
{
	[XmlElement("Version")]
	public List<UpdateVersion> VersionList { get; set; } = new List<UpdateVersion>();


	public static Versions LoadFromXml(XDocument xml)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(Versions));
		using XmlReader xmlReader = xml.Root.CreateReader();
		return (Versions)xmlSerializer.Deserialize(xmlReader);
	}
}
