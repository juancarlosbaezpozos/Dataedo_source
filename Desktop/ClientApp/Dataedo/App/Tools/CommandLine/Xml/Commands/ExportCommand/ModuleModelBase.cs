using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.Base;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand;

public abstract class ModuleModelBase : ExportModelBase
{
	[XmlAnyElement(Name = "IdComment")]
	public XmlCommentObject IdComment { get; set; }

	public abstract int IdValue { get; }

	public ModuleModelBase()
	{
	}

	public ModuleModelBase(bool export)
	{
		base.Export = export;
	}

	public ModuleModelBase(bool export, string name)
	{
		base.Export = export;
		IdComment = new XmlCommentObject(name);
	}
}
