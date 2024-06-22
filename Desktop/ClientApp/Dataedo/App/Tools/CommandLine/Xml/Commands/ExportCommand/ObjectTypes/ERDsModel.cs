using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.Base;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes;

public class ERDsModel : ExportModelBase
{
	public ERDsModel()
		: base(export: true)
	{
	}

	public ERDsModel(bool export)
		: base(export)
	{
	}
}
