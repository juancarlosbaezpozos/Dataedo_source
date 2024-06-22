using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.App.Tools.CommandLine.Xml.Connections;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.Base;

public abstract class RepositoryCommandBase : CommandBase
{
	[XmlAnyElement(Name = "RepositoryConnectionComment")]
	public XmlCommentObject RepositoryConnectionComment { get; set; } = new XmlCommentObject("Repository connection");


	[XmlElement(Type = typeof(SqlServerConnection), ElementName = "SqlServerRepository")]
	[XmlElement(Type = typeof(SqlServerCeConnection), ElementName = "SqlServerCeRepository")]
	public ConnectionBase RepositoryConnection { get; set; }
}
