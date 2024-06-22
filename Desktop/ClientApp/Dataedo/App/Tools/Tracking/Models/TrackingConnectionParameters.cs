using Dataedo.App.Enums;
using Dataedo.App.Interfaces.Synchronize;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.Tracking.Models;

public class TrackingConnectionParameters
{
	public string Connector { get; set; }

	public string DBVersion { get; set; }

	public string ConnectionType { get; set; }

	public string SSLMode { get; set; }

	public TrackingConnectionParameters(IDatabaseRow databaseRow, SharedDatabaseTypeEnum.DatabaseType? databaseType, string sslMode, string connectionType)
	{
		Connector = DatabaseTypeEnum.TypeToString(databaseType);
		DBVersion = databaseRow.DbmsVersion;
		ConnectionType = connectionType;
		SSLMode = sslMode;
	}

	public TrackingConnectionParameters(string dbmsVersion, SharedDatabaseTypeEnum.DatabaseType? databaseType, string sslMode, string connectionType)
	{
		Connector = DatabaseTypeEnum.TypeToString(databaseType);
		DBVersion = dbmsVersion;
		ConnectionType = connectionType;
		SSLMode = sslMode;
	}
}
