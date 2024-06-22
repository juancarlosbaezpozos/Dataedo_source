using Dataedo.DataSources.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataProfiling.Tools;

public static class EnumToEnumChanger
{
	public static ProfilingDatabaseTypeEnum.ProfilingDatabaseType GetProfilingDatabaseTypeEnum(SharedDatabaseTypeEnum.DatabaseType? type)
	{
		switch (type)
		{
		case SharedDatabaseTypeEnum.DatabaseType.SqlServer:
		case SharedDatabaseTypeEnum.DatabaseType.AzureSQLDatabase:
			return ProfilingDatabaseTypeEnum.ProfilingDatabaseType.SqlServer;
		case SharedDatabaseTypeEnum.DatabaseType.AzureSQLDataWarehouse:
			return ProfilingDatabaseTypeEnum.ProfilingDatabaseType.AzureSqlDataWarehouse;
		case SharedDatabaseTypeEnum.DatabaseType.MySQL:
		case SharedDatabaseTypeEnum.DatabaseType.MariaDB:
		case SharedDatabaseTypeEnum.DatabaseType.Aurora:
		case SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL:
		case SharedDatabaseTypeEnum.DatabaseType.MySQL8:
		case SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL8:
			return ProfilingDatabaseTypeEnum.ProfilingDatabaseType.MySql;
		case SharedDatabaseTypeEnum.DatabaseType.Oracle:
			return ProfilingDatabaseTypeEnum.ProfilingDatabaseType.Oracle;
		case SharedDatabaseTypeEnum.DatabaseType.Snowflake:
			return ProfilingDatabaseTypeEnum.ProfilingDatabaseType.Snowflake;
		case SharedDatabaseTypeEnum.DatabaseType.PostgreSQL:
		case SharedDatabaseTypeEnum.DatabaseType.AuroraPostgreSQL:
			return ProfilingDatabaseTypeEnum.ProfilingDatabaseType.PostgreSql;
		case SharedDatabaseTypeEnum.DatabaseType.Redshift:
			return ProfilingDatabaseTypeEnum.ProfilingDatabaseType.Redshift;
		case SharedDatabaseTypeEnum.DatabaseType.Vertica:
			return ProfilingDatabaseTypeEnum.ProfilingDatabaseType.Vertica;
		case SharedDatabaseTypeEnum.DatabaseType.Db2LUW:
			return ProfilingDatabaseTypeEnum.ProfilingDatabaseType.Db2;
		case SharedDatabaseTypeEnum.DatabaseType.SapAse:
			return ProfilingDatabaseTypeEnum.ProfilingDatabaseType.SapAse;
		case SharedDatabaseTypeEnum.DatabaseType.SapHana:
			return ProfilingDatabaseTypeEnum.ProfilingDatabaseType.SapHana;
		default:
			return ProfilingDatabaseTypeEnum.ProfilingDatabaseType.SqlServer;
		}
	}

	public static bool IsDatabaseTypeNotSupportedForDataProfiling(SharedDatabaseTypeEnum.DatabaseType? type)
	{
		switch (type)
		{
		case SharedDatabaseTypeEnum.DatabaseType.SqlServer:
		case SharedDatabaseTypeEnum.DatabaseType.Oracle:
		case SharedDatabaseTypeEnum.DatabaseType.MySQL:
		case SharedDatabaseTypeEnum.DatabaseType.MariaDB:
		case SharedDatabaseTypeEnum.DatabaseType.AzureSQLDatabase:
		case SharedDatabaseTypeEnum.DatabaseType.AzureSQLDataWarehouse:
		case SharedDatabaseTypeEnum.DatabaseType.Aurora:
		case SharedDatabaseTypeEnum.DatabaseType.PostgreSQL:
		case SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL:
		case SharedDatabaseTypeEnum.DatabaseType.Snowflake:
		case SharedDatabaseTypeEnum.DatabaseType.Redshift:
		case SharedDatabaseTypeEnum.DatabaseType.AuroraPostgreSQL:
		case SharedDatabaseTypeEnum.DatabaseType.Db2LUW:
		case SharedDatabaseTypeEnum.DatabaseType.MySQL8:
		case SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL8:
		case SharedDatabaseTypeEnum.DatabaseType.Vertica:
		case SharedDatabaseTypeEnum.DatabaseType.SapAse:
		case SharedDatabaseTypeEnum.DatabaseType.SapHana:
			return false;
		default:
			return true;
		}
	}
}
