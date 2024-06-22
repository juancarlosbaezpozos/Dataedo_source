using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand;

public class ModulesModel
{
	[XmlAttribute]
	public bool ExportNotSpecified { get; set; }

	[XmlElement(ElementName = "Module", Type = typeof(ModuleModel))]
	[XmlElement(ElementName = "Other", Type = typeof(ModuleOtherModel))]
	public List<ModuleModelBase> Modules { get; set; }

	public ModulesModel()
	{
		Modules = new List<ModuleModelBase>();
	}
}
