using System.Xml.Serialization;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.Base;

public class ExportModelWithScriptBase : ExportModelBase
{
	[XmlElement]
	public ExportModelBase Script { get; set; }

	public ExportModelWithScriptBase()
	{
		Script = new ExportModelBase();
	}

	public ExportModelWithScriptBase(bool export)
		: base(export)
	{
		Script = new ExportModelBase();
	}
}
