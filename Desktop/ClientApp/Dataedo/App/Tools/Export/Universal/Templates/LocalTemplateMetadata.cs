using System.Xml.Serialization;

namespace Dataedo.App.Tools.Export.Universal.Templates;

[XmlRoot("Template")]
public class LocalTemplateMetadata
{
	[XmlElement("Name")]
	public string Name { get; set; }

	[XmlElement("Version")]
	public string Version { get; set; }

	[XmlElement("Description")]
	public string Description { get; set; }

	[XmlElement("Transformer")]
	public string Transformer { get; set; }

	[XmlElement("Customizer")]
	public string Customizer { get; set; }
}
