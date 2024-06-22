using System.Xml.Serialization;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.Base;

public class DependencyObjectModelBase : ExportModelBase
{
	[XmlElement]
	public ExportModelBase Dependencies { get; set; }

	public DependencyObjectModelBase()
	{
		Dependencies = new ExportModelBase();
	}

	public DependencyObjectModelBase(bool export)
		: base(export)
	{
	}
}
