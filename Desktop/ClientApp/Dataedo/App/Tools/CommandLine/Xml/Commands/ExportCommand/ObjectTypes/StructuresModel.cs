using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.Base;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes;

public class StructuresModel : DependencyObjectModelBase
{
	[XmlElement]
	public ExportModelBase Relations { get; set; }

	[XmlElement]
	public ExportModelBase Keys { get; set; }

	[XmlElement]
	public ExportModelBase Schema { get; set; }

	public StructuresModel()
	{
		Relations = new ExportModelBase();
		Keys = new ExportModelBase();
		Schema = new ExportModelBase();
	}

	public StructuresModel(bool export)
		: base(export)
	{
	}
}
