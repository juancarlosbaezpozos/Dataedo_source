using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Common;

namespace Dataedo.App.Tools.CommandLine.Xml;

public class LogFile
{
	[XmlElement]
	public string Path { get; set; } = string.Empty;


	[XmlAnyElement(Name = "PathAlternative")]
	public XmlCommentObject PathAlternative { get; set; }
}
