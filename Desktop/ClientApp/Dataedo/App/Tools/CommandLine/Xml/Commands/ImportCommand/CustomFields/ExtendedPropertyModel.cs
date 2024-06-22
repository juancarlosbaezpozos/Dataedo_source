using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Common;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ImportCommand.CustomFields;

public class ExtendedPropertyModel
{
	[XmlAttribute]
	public bool IsEnabled { get; set; }

	[XmlAnyElement(Name = "DestinationCustomFieldIdComment")]
	public XmlCommentObject DestinationCustomFieldIdComment { get; set; }

	[XmlElement]
	public int DestinationCustomFieldId { get; set; }

	public int DestinationCustomFieldIdValue => DestinationCustomFieldId;

	[XmlElement]
	public string SourceExtendedProperty { get; set; }

	public ExtendedPropertyModel()
	{
	}

	public ExtendedPropertyModel(int id, bool isEnabled)
	{
		DestinationCustomFieldId = id;
		IsEnabled = isEnabled;
	}

	public ExtendedPropertyModel(int id, bool isEnabled, string destinationCustomFieldName, string sourceExtendedPropertyName)
	{
		DestinationCustomFieldId = id;
		IsEnabled = isEnabled;
		DestinationCustomFieldIdComment = new XmlCommentObject("Custom field: " + destinationCustomFieldName);
		SourceExtendedProperty = sourceExtendedPropertyName;
	}
}
