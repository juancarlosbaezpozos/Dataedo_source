using Dataedo.App.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Interfaces.Synchronize;

public interface IDatabaseRow
{
	string DbmsVersion { get; set; }

	ConnectionTypeEnum.ConnectionType ConnectionType { get; set; }

	SharedDatabaseTypeEnum.DatabaseType? Type { get; set; }

	string SSLType { get; set; }
}
