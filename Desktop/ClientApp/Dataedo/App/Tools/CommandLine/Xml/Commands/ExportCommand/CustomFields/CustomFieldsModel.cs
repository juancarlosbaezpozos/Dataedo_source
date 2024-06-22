using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.CustomFields;

public class CustomFieldsModel
{
	[XmlAttribute]
	public bool ExportNotSpecified { get; set; }

	[XmlElement(ElementName = "CustomField", Type = typeof(CustomFieldModelBase))]
	public List<CustomFieldModelBase> CustomFields { get; set; }

	public CustomFieldsModel()
	{
		CustomFields = new List<CustomFieldModelBase>();
	}
}
