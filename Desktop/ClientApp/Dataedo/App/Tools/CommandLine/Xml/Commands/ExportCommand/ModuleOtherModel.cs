namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand;

public class ModuleOtherModel : ModuleModelBase
{
	public override int IdValue => -1;

	public ModuleOtherModel()
	{
	}

	public ModuleOtherModel(bool export)
		: base(export, "Other")
	{
	}
}
