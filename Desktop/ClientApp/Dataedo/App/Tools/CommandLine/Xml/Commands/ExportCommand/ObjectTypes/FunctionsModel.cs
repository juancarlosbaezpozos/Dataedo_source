using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.Base;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes;

public class FunctionsModel : DependencyObjectModelBase
{
	[XmlElement]
	public ExportModelBase Script { get; set; }

	public FunctionsModel()
	{
		Script = new ExportModelBase();
	}

	public FunctionsModel(bool export)
		: base(export)
	{
	}
}
