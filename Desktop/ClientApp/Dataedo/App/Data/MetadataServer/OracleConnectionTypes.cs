using System.Data;
using Dataedo.App.Enums;

namespace Dataedo.App.Data.MetadataServer;

internal class OracleConnectionTypes
{
	public static DataTable GetOracleConnectionTypesDataSource()
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("type", typeof(string));
		dataTable.Columns.Add("name", typeof(string));
		dataTable.Rows.Add(ConnectionTypeEnum.TypeToString(ConnectionTypeEnum.ConnectionType.Direct), ConnectionTypeEnum.TypeToStringForDisplay(ConnectionTypeEnum.ConnectionType.Direct));
		dataTable.Rows.Add(ConnectionTypeEnum.TypeToString(ConnectionTypeEnum.ConnectionType.OracleClient), ConnectionTypeEnum.TypeToStringForDisplay(ConnectionTypeEnum.ConnectionType.OracleClient));
		return dataTable;
	}
}
