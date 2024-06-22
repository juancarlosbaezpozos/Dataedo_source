using System.Xml.Serialization;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.Base;

public class ExportModelBase
{
	[XmlAttribute]
	public bool Export { get; set; }

	public ExportModelBase()
	{
	}

	public ExportModelBase(bool export)
	{
		Export = export;
	}
}
