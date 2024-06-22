using System.Xml.Serialization;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.CustomFields;

public class FieldsModel
{
	[XmlAttribute]
	public bool ExportNotSpecified { get; set; }

	[XmlElement]
	public bool? Description { get; set; }

	[XmlElement]
	public bool? Title { get; set; }

	[XmlElement]
	public bool? DataType { get; set; }

	[XmlElement]
	public bool? Nullable { get; set; }

	[XmlElement]
	public bool? Identity { get; set; }

	[XmlElement]
	public bool? DefaultComputed { get; set; }
}
