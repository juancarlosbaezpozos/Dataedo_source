using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Common;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.Base;

public abstract class RepositoryDocumentationCommandBase : RepositoryCommandWithDatabaseRowBase
{
	[XmlAnyElement(Name = "RepositoryDocumentationIdComment")]
	public XmlCommentObject RepositoryDocumentationIdComment { get; set; }

	[XmlElement]
	public int? RepositoryDocumentationId { get; set; }
}
