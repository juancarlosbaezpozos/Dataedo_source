using Dataedo.App.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.LoginFormTools.UserControls.Subcontrols.Interfaces;

public interface IRecentProjectData
{
	AuthenticationType.AuthenticationTypeEnum AuthenticationType { get; }

	string Database { get; }

	string Login { get; }

	string Password { get; }

	int? Port { get; }

	bool SavePassword { get; }

	string ServerName { get; }

	SqlServerConnectionModeEnum.SqlServerConnectionMode? SqlServerConnectionMode { get; }
}
