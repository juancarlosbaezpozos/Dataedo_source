using Dataedo.App.Enums;

namespace Dataedo.App.LoginFormTools.UserControls.Subcontrols.Interfaces;

public interface IConnectionData
{
	string AuthenticationTypeString { get; }

	string Database { get; }

	string Login { get; }

	string Password { get; }

	int? Port { get; }

	string ServerName { get; }

	SqlServerConnectionModeEnum.SqlServerConnectionMode? SqlServerConnectionMode { get; }
}
