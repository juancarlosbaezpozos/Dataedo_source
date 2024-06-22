using System.Xml.Serialization;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.Base;

[XmlInclude(typeof(ExportVersion1))]
[XmlInclude(typeof(ExportVersion2))]
public abstract class CommandBase
{
	[XmlAttribute]
	public bool IsEnabled { get; set; }
}
