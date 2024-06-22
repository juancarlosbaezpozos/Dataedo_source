using System.Xml.Serialization;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ImportCommand.Connections;

public class DatabaseModel
{
	[XmlText]
	public string Name { get; set; }

	[XmlAttribute]
	public bool IsMultiple { get; set; }

	public DatabaseModel()
	{
	}

	public DatabaseModel(string name, bool isMultiple)
	{
		Name = name;
		IsMultiple = isMultiple;
	}
}
