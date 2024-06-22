using System.Xml.Serialization;

namespace Dataedo.App.Drivers.ODBC.Repositories;

[XmlRoot("driver")]
public class DriverMetaFileXml
{
	[XmlElement("name")]
	public string Name { get; set; }

	[XmlElement("version")]
	public string Version { get; set; }
}
