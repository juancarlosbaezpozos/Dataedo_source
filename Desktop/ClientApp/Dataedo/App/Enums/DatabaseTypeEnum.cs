using Dataedo.App.DatabasesSupport;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Enums;

public class DatabaseTypeEnum : SharedDatabaseTypeEnum
{
	public static bool UsesOracleLibraries(DatabaseType databaseType)
	{
		return databaseType == DatabaseType.Oracle;
	}

	public static bool? IsSchemaType(DatabaseType? databaseType)
	{
		return DatabaseSupportFactory.GetDatabaseSupport(databaseType).IsSchemaType;
	}

	public new static string GetDefaultPort(DatabaseType? databaseType)
	{
		return DatabaseSupportFactory.GetDatabaseSupport(databaseType).DefaultConnectionPort;
	}

	public static Dataedo.Data.Commands.Enums.DatabaseType? ToTypeForDataCommands(DatabaseType? databaseType, bool isFile)
	{
		return DatabaseSupportFactory.GetDatabaseSupport(databaseType).GetTypeForCommands(isFile);
	}

	public new static DatabaseType? StringToType(string databaseTypeString)
	{
		return DatabaseSupportFactoryShared.GetDatabaseType(databaseTypeString);
	}

	public new static string TypeToString(DatabaseType? databaseType)
	{
		return DatabaseSupportFactory.GetDatabaseSupport(databaseType).TypeValue;
	}

	public new static string TypeToStringForDisplay(DatabaseType? databaseType)
	{
		return DatabaseSupportFactory.GetDatabaseSupport(databaseType).FriendlyDisplayName;
	}
}
