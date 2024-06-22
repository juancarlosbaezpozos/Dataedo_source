using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.CustomFields;

namespace Dataedo.App.Tools.CommandLine.Xml;

public class ExportVersion2 : ExportBase
{
	[XmlAnyElement(Name = "FieldsComment")]
	public XmlCommentObject FieldsComment { get; set; } = new XmlCommentObject("The fields to export. Applies only to HTML export.");


	[XmlAnyElement(Name = "ExportNotSpecifiedFieldComment")]
	public XmlCommentObject ExportNotSpecifiedFieldComment { get; set; } = new XmlCommentObject("Set 'ExportNotSpecified' attribute to 'true' to export all fields that are not specified in list.");


	[XmlElement]
	public FieldsModel Fields { get; set; }

	[XmlAnyElement(Name = "CustomFieldsComment")]
	public XmlCommentObject CustomFieldsComment { get; set; } = new XmlCommentObject("The custom fields to export.");


	[XmlAnyElement(Name = "ExportNotSpecifiedComment")]
	public XmlCommentObject ExportNotSpecifiedComment { get; set; } = new XmlCommentObject("Set 'ExportNotSpecified' attribute to 'true' to export all custom fields that are not specified in list.");


	[XmlElement]
	public CustomFieldsModel CustomFields { get; set; }

	[XmlAnyElement(Name = "DocumentTitleComment")]
	public XmlCommentObject DocumentTitleComment { get; set; } = new XmlCommentObject("The title of document");


	[XmlElement]
	public string DocumentTitle { get; set; }

	[XmlAnyElement(Name = "DocumentationsComment")]
	public XmlCommentObject DocumentationsComment { get; set; } = new XmlCommentObject("List of documentations to export");


	[XmlElement]
	public DocumentationsModel Documentations { get; set; }

	public IEnumerable<DocumentationModelBase> DocumentationsToExport => Documentations?.Documentations?.Where((DocumentationModelBase x) => x.Export) ?? new List<DocumentationModelBase>();

	[XmlIgnore]
	public override DatabaseRow FirstDatabaseRow => DocumentationsToExport.FirstOrDefault()?.RepositoryDatabaseRow;

	public override IEnumerable<DocumentationModelBase> DataAsDocumentationModelBase => DocumentationsToExport;
}
