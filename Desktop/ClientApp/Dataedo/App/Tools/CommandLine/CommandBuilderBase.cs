using Dataedo.App.Tools.CommandLine.Xml.Commands.Base;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine;

internal static class CommandBuilderBase
{
	public static string CommandsFileExtension = "dataedocmd";

	public static void SetRepositoryConnection(RepositoryCommandBase importCommand, DatabaseType repositoryType, LoginInfo loginInfo)
	{
		switch (repositoryType)
		{
		case DatabaseType.SqlServerCe:
			importCommand.RepositoryConnection = new SqlServerCeConnection();
			break;
		case DatabaseType.SqlServer:
			importCommand.RepositoryConnection = new SqlServerConnection();
			break;
		}
		importCommand.RepositoryConnection.SetConnection(loginInfo);
	}
}
