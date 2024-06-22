using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand;

public class DocumentationsModel
{
	[XmlElement(ElementName = "Documentation", Type = typeof(DocumentationModelBase))]
	public List<DocumentationModelBase> Documentations { get; set; }

	public DocumentationsModel()
	{
		Documentations = new List<DocumentationModelBase>();
	}
}
