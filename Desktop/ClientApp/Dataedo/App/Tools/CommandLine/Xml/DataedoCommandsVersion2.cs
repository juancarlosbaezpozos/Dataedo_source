using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Xml.Commands.Base;

namespace Dataedo.App.Tools.CommandLine.Xml;

[XmlRoot(ElementName = "DataedoCommands")]
public class DataedoCommandsVersion2 : DataedoCommandsBase
{
	public static readonly string VersionValue = "2.0";

	[XmlAttribute]
	public override string Version { get; set; } = VersionValue;


	[XmlArray]
	[XmlArrayItem(Type = typeof(Import))]
	[XmlArrayItem(Type = typeof(ExportVersion2), ElementName = "Export")]
	public override List<CommandBase> Commands { get; set; }

	public static XmlSerializer GetSerializer()
	{
		return new XmlSerializer(typeof(DataedoCommandsVersion2));
	}

	public override XmlSerializer GetCurrentSerializer()
	{
		return GetSerializer();
	}

	public static DataedoCommandsVersion2 DeserializeXml(Stream stream)
	{
		return GetSerializer().Deserialize(stream) as DataedoCommandsVersion2;
	}
}
