using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.Base;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes;

public class ViewsModel : DependencyObjectModelBase
{
	[XmlElement]
	public ExportModelBase Relations { get; set; }

	[XmlElement]
	public ExportModelBase Keys { get; set; }

	[XmlElement]
	public ExportModelWithScriptBase Triggers { get; set; }

	[XmlElement]
	public ExportModelBase Script { get; set; }

	public ViewsModel()
	{
		Relations = new ExportModelBase();
		Keys = new ExportModelBase();
		Triggers = new ExportModelWithScriptBase();
		Script = new ExportModelBase();
	}

	public ViewsModel(bool export)
		: base(export)
	{
	}
}
