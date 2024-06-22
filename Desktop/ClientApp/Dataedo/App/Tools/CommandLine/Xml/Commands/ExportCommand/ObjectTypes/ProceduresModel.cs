using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.Base;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes;

public class ProceduresModel : DependencyObjectModelBase
{
	[XmlElement]
	public ExportModelBase Script { get; set; }

	public ProceduresModel()
	{
		Script = new ExportModelBase();
	}

	public ProceduresModel(bool export)
		: base(export)
	{
	}
}
