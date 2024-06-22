using System;
using System.Collections.Generic;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport;

[Serializable]
public class SerializableConnectorVersion
{
	public string databaseType;

	public ConnectorVersionInfo versionInfo;

	public SerializableConnectorVersion()
	{
	}

	public SerializableConnectorVersion(KeyValuePair<SharedDatabaseTypeEnum.DatabaseType, ConnectorVersionInfo> keyValuePair)
	{
		databaseType = keyValuePair.Key.ToString();
		versionInfo = keyValuePair.Value;
	}
}
