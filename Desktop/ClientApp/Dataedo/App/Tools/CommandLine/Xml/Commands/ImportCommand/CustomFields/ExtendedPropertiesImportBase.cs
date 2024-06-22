using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ImportCommand.CustomFields;

public class ExtendedPropertiesImportBase
{
	[XmlElement(ElementName = "ExtendedProperty", Type = typeof(ExtendedPropertyModel))]
	public List<ExtendedPropertyModel> ExtendedProperties { get; set; }

	public ExtendedPropertiesImportBase()
	{
		ExtendedProperties = new List<ExtendedPropertyModel>();
	}
}
