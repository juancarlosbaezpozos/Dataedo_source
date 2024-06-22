using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools;

public static class ConnectionTypeService
{
	public static string GetConnectionType(SharedDatabaseTypeEnum.DatabaseType? databaseType, ConnectionTypeEnum.ConnectionType connectionType, AuthenticationType.AuthenticationTypeEnum authenticationType, GeneralConnectionTypeEnum.GeneralConnectionType generalConnectionType)
	{
		switch (databaseType)
		{
		case SharedDatabaseTypeEnum.DatabaseType.SqlServer:
		case SharedDatabaseTypeEnum.DatabaseType.AzureSQLDatabase:
		case SharedDatabaseTypeEnum.DatabaseType.AzureSQLDataWarehouse:
		case SharedDatabaseTypeEnum.DatabaseType.SsasTabular:
		case SharedDatabaseTypeEnum.DatabaseType.PowerBiDataset:
		case SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreSQLServer:
		case SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreAzureSQL:
		case SharedDatabaseTypeEnum.DatabaseType.Tableau:
			return AuthenticationType.TypeToString(authenticationType);
		case SharedDatabaseTypeEnum.DatabaseType.Oracle:
			return ConnectionTypeEnum.TypeToString(connectionType);
		case SharedDatabaseTypeEnum.DatabaseType.MongoDB:
		case SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB:
			return GeneralConnectionTypeEnum.TypeToParamString(generalConnectionType);
		default:
			return string.Empty;
		}
	}

	public static string GetConnectionType(DatabaseRow databaseRow)
	{
		return GetConnectionType(databaseRow.Type, databaseRow.ConnectionType, databaseRow.SelectedAuthenticationType, databaseRow.GeneralConnectionType);
	}
}
