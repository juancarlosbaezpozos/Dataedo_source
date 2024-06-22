using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Tools;

namespace Dataedo.App.Tools.CommandLine.Xml;

public class Settings
{
	[XmlElement]
	public LogFile LogFile { get; set; }

	public string LogFilePathParsed => ParserHelper.ParseAll(this?.LogFile?.Path);

	public Settings()
	{
		LogFile = new LogFile();
	}
}
