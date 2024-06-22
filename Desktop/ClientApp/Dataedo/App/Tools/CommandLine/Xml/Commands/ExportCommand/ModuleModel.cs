using System.Xml.Serialization;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand;

public class ModuleModel : ModuleModelBase
{
	[XmlElement]
	public int Id { get; set; }

	public override int IdValue => Id;

	public ModuleModel()
	{
	}

	public ModuleModel(int id, bool export)
		: base(export)
	{
		Id = id;
	}

	public ModuleModel(int id, bool export, string name)
		: base(export, name)
	{
		Id = id;
	}
}
