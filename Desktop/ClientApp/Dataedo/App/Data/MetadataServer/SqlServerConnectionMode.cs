using System.Data;
using Dataedo.App.Enums;
using DevExpress.XtraEditors;

namespace Dataedo.App.Data.MetadataServer;

public static class SqlServerConnectionMode
{
	public static void SetSqlServerConnectionMode(LookUpEdit sqlServerConnectionModeLookUpEdit)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("id", typeof(string));
		dataTable.Columns.Add("name", typeof(string));
		dataTable.Rows.Add(SqlServerConnectionModeEnum.TypeToString(SqlServerConnectionModeEnum.SqlServerConnectionMode.ForceEncryptionRequireTrustedCertificate), SqlServerConnectionModeEnum.TypeToStringForDisplay(SqlServerConnectionModeEnum.SqlServerConnectionMode.ForceEncryptionRequireTrustedCertificate));
		dataTable.Rows.Add(SqlServerConnectionModeEnum.TypeToString(SqlServerConnectionModeEnum.SqlServerConnectionMode.ForceEncryptionTrustServerCertificate), SqlServerConnectionModeEnum.TypeToStringForDisplay(SqlServerConnectionModeEnum.SqlServerConnectionMode.ForceEncryptionTrustServerCertificate));
		dataTable.Rows.Add(SqlServerConnectionModeEnum.TypeToString(SqlServerConnectionModeEnum.SqlServerConnectionMode.EncryptConnectionIfPossible), SqlServerConnectionModeEnum.TypeToStringForDisplay(SqlServerConnectionModeEnum.SqlServerConnectionMode.EncryptConnectionIfPossible));
		sqlServerConnectionModeLookUpEdit.Properties.DataSource = dataTable;
		sqlServerConnectionModeLookUpEdit.Properties.DropDownRows = dataTable.Rows.Count;
	}
}
