using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.LoginFormTools.Tools.Repository;

internal interface IRepositoryConnection
{
	DatabaseType RepositoryType { get; }
}
