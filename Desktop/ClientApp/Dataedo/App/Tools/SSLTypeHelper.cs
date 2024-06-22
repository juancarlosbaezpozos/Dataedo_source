using System;
using Dataedo.App.Classes.Synchronize;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools;

public class SSLTypeHelper
{
	public static string GetSelectedSSLType(SharedDatabaseTypeEnum.DatabaseType? databaseType, string type, SSLSettings settings)
	{
		switch (databaseType)
		{
		case SharedDatabaseTypeEnum.DatabaseType.MySQL8:
			if (settings != null)
			{
				return "YES";
			}
			return "NO";
		case SharedDatabaseTypeEnum.DatabaseType.MongoDB:
		case SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB:
			if (!Convert.ToBoolean(type))
			{
				return "NO";
			}
			return "YES";
		case SharedDatabaseTypeEnum.DatabaseType.Redshift:
			return type;
		default:
			return "NO";
		}
	}

	public static string GetSelectedSSLType(DatabaseRow databaseRow)
	{
		return GetSelectedSSLType(databaseRow.Type, databaseRow.SSLType, databaseRow.SSLSettings);
	}
}
