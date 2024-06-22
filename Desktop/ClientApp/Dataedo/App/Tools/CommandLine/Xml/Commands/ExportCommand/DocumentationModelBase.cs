using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.Base;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand;

public class DocumentationModelBase : ExportModelBase
{
	[XmlAnyElement(Name = "DocumentationIdComment")]
	public XmlCommentObject DocumentationIdComment { get; set; }

	[XmlElement]
	public int DocumentationId { get; set; }

	public int DocumentationIdValue => DocumentationId;

	[XmlAnyElement(Name = "ModulesComment")]
	public XmlCommentObject ModulesComment { get; set; } = new XmlCommentObject("List of modules to export");


	[XmlAnyElement(Name = "ModulesComment2")]
	public XmlCommentObject ModulesComment2 { get; set; } = new XmlCommentObject("Set 'ExportNotSpecified' attribute to 'true' to export all objects that are not specified in list.");


	[XmlElement]
	public ModulesModel Modules { get; set; }

	[XmlIgnore]
	public DatabaseRow RepositoryDatabaseRow { get; set; }

	[XmlIgnore]
	public IEnumerable<int> ModulesIds => Modules?.Modules?.Select((ModuleModelBase x) => x.IdValue) ?? new int[0];

	[XmlIgnore]
	public IEnumerable<int> SelectedModulesIds => (from x in Modules?.Modules?.Where((ModuleModelBase x) => x.Export)
		select x.IdValue) ?? new int[0];

	public DocumentationModelBase()
	{
	}

	public DocumentationModelBase(int id, bool export)
	{
		DocumentationId = id;
		base.Export = export;
	}

	public DocumentationModelBase(int id, bool export, string name)
	{
		DocumentationId = id;
		base.Export = export;
		DocumentationIdComment = new XmlCommentObject(name);
	}
}
