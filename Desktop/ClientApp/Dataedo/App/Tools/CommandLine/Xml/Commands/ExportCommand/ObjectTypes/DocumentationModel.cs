using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.Base;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes;

public class DocumentationModel : ExportModelBase
{
	[XmlElement]
	public ExportModelBase DatabaseName { get; set; }

	[XmlElement]
	public ExportModelBase HostName { get; set; }

	public DocumentationModel()
	{
		DatabaseName = new ExportModelBase();
		HostName = new ExportModelBase();
	}

	public DocumentationModel(bool export)
		: base(export)
	{
	}
}
