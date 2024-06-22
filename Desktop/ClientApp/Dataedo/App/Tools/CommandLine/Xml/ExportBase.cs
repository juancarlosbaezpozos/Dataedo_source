using System.Collections.Generic;
using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.App.Tools.CommandLine.Tools;
using Dataedo.App.Tools.CommandLine.Xml.Commands.Base;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand;

namespace Dataedo.App.Tools.CommandLine.Xml;

[XmlInclude(typeof(ExportVersion1))]
[XmlInclude(typeof(ExportVersion2))]
public abstract class ExportBase : RepositoryCommandBase
{
	public enum FormatEnum
	{
		NotSet = 0,
		PDF = 1,
		HTML = 2,
		Excel = 3
	}

	[XmlAnyElement(Name = "OverwriteComment")]
	public XmlCommentObject OverwriteComment { get; set; } = new XmlCommentObject("Change to 'true' if you want the file to be overwritten if already exists.");


	[XmlElement]
	public bool Overwrite { get; set; }

	[XmlAnyElement(Name = "FormatComment")]
	public XmlCommentObject FormatComment { get; set; } = new XmlCommentObject("Output format (PDF/HTML/Excel)");


	[XmlElement]
	public FormatEnum Format { get; set; }

	[XmlElement]
	public string TemplatePath { get; set; }

	[XmlAnyElement(Name = "ObjectTypesComment")]
	public XmlCommentObject ObjectTypesComment { get; set; } = new XmlCommentObject("Object types to export");


	[XmlElement]
	public ObjectTypesModel ObjectTypes { get; set; }

	[XmlElement]
	public string OutputPath { get; set; } = string.Empty;


	[XmlIgnore]
	public string OutputPathParsed => ParserHelper.ParseAll(OutputPath);

	[XmlAnyElement(Name = "OutputPathAlternative")]
	public XmlCommentObject OutputPathAlternative { get; set; }

	[XmlIgnore]
	public abstract DatabaseRow FirstDatabaseRow { get; }

	public abstract IEnumerable<DocumentationModelBase> DataAsDocumentationModelBase { get; }
}
