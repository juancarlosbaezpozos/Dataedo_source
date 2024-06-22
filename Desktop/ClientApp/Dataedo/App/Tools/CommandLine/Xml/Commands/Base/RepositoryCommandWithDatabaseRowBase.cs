using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.Base;

public abstract class RepositoryCommandWithDatabaseRowBase : RepositoryCommandBase
{
	[XmlIgnore]
	public DatabaseRow RepositoryDatabaseRow { get; set; }
}
