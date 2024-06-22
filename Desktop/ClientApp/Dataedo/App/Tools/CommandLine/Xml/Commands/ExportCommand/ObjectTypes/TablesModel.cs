using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.Base;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes;

public class TablesModel : DependencyObjectModelBase
{
	[XmlElement]
	public ExportModelBase Relations { get; set; }

	[XmlElement]
	public ExportModelBase Keys { get; set; }

	[XmlElement]
	public ExportModelWithScriptBase Triggers { get; set; }

	public TablesModel()
	{
		Relations = new ExportModelBase();
		Keys = new ExportModelBase();
		Triggers = new ExportModelWithScriptBase();
	}

	public TablesModel(bool export)
		: base(export)
	{
	}
}
