using System.Collections.Generic;
using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand;

namespace Dataedo.App.Tools.CommandLine.Xml;

public class ExportVersion1 : ExportBase
{
	[XmlAnyElement(Name = "RepositoryDocumentationIdComment")]
	public XmlCommentObject RepositoryDocumentationIdComment { get; set; }

	[XmlElement]
	public int? RepositoryDocumentationId { get; set; }

	[XmlAnyElement(Name = "ModulesComment")]
	public XmlCommentObject ModulesComment { get; set; } = new XmlCommentObject("List of modules to export");


	[XmlAnyElement(Name = "ModulesComment2")]
	public XmlCommentObject ModulesComment2 { get; set; } = new XmlCommentObject("Set 'ExportNotSpecified' attribute to 'true' to export all objects that are not specified in list.");


	[XmlElement]
	public ModulesModel Modules { get; set; }

	[XmlIgnore]
	public DatabaseRow RepositoryDatabaseRow { get; set; }

	[XmlIgnore]
	public override DatabaseRow FirstDatabaseRow => RepositoryDatabaseRow;

	public override IEnumerable<DocumentationModelBase> DataAsDocumentationModelBase
	{
		get
		{
			if (RepositoryDocumentationId.HasValue)
			{
				DocumentationModelBase documentationModelBase = new DocumentationModelBase(RepositoryDocumentationId.Value, export: true, RepositoryDatabaseRow.Title)
				{
					RepositoryDatabaseRow = FirstDatabaseRow,
					Modules = Modules
				};
				return new DocumentationModelBase[1] { documentationModelBase };
			}
			return new List<DocumentationModelBase>();
		}
	}
}
