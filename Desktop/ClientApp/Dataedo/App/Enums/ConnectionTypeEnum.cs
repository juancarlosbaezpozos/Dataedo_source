namespace Dataedo.App.Enums;

public static class ConnectionTypeEnum
{
	public enum ConnectionType
	{
		Direct = 1,
		OracleClient = 2
	}

	public static ConnectionType StringToType(string connectionTypeString)
	{
		if (!(connectionTypeString == "DIRECT"))
		{
			if (connectionTypeString == "ORACLE_CLIENT")
			{
				return ConnectionType.OracleClient;
			}
			return ConnectionType.Direct;
		}
		return ConnectionType.Direct;
	}

	public static string TypeToString(ConnectionType? connectionType)
	{
		return connectionType switch
		{
			ConnectionType.Direct => "DIRECT", 
			ConnectionType.OracleClient => "ORACLE_CLIENT", 
			_ => null, 
		};
	}

	public static string TypeToStringForDisplay(ConnectionType? connectionType)
	{
		return connectionType switch
		{
			ConnectionType.Direct => "Direct", 
			ConnectionType.OracleClient => "Oracle client", 
			_ => null, 
		};
	}
}
