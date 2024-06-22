using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.Classes.Synchronize;

public class HiveDatabaseRow : DatabaseRow
{
	public HiveDatabaseRow(SharedDatabaseTypeEnum.DatabaseType? databaseType, string dbmsDatabaseName, string hiveCatalogName, string hiveDbName, string title, string host, string user, string password, object port, int? id, string filter, string dbmsVersion, string serviceName = null, string connectionRole = null, SSLSettings sslSettings = null, List<string> schemas = null, string sslType = null)
		: base(databaseType, hiveCatalogName + "." + hiveDbName, title, host, user, password, port, id, filter, dbmsVersion, serviceName, connectionRole, sslSettings, schemas, sslType)
	{
		base.Param2 = dbmsDatabaseName ?? string.Empty;
		base.Param3 = hiveCatalogName;
		base.Param4 = hiveDbName;
	}

	public HiveDatabaseRow(SharedDatabaseTypeEnum.DatabaseType? databaseType, string dbmsDatabaseName, string hiveCatalogName, string hiveDbName, string title, string host, string user, string password, bool windows_authentication, int? id, string filter, string dbmsVersion, string port, AuthenticationType.AuthenticationTypeEnum? sqlAuthentication)
		: base(databaseType, hiveCatalogName + "." + hiveDbName, title, host, user, password, windows_authentication, id, filter, dbmsVersion, port, sqlAuthentication)
	{
		base.Param2 = dbmsDatabaseName ?? string.Empty;
		base.Param3 = hiveCatalogName;
		base.Param4 = hiveDbName;
	}

	public HiveDatabaseRow(SharedDatabaseTypeEnum.DatabaseType? databaseType, string dbmsDatabaseName, string hiveCatalogName, string hiveDbName, string title, string host, string user, string password, object port, int? id, string filter, string dbmsVersion, string sslType)
		: base(databaseType, hiveCatalogName + "." + hiveDbName, title, host, user, password, port, id, filter, dbmsVersion, sslType)
	{
		base.Param2 = dbmsDatabaseName ?? string.Empty;
		base.Param3 = hiveCatalogName;
		base.Param4 = hiveDbName;
	}

	public bool ValidateHiveDatabase(DXErrorProvider errorProvider, TextEdit databaseTextEdit, SplashScreenManager splashScreenManager = null, Form owner = null)
	{
		if ((string.IsNullOrEmpty(base.Host) || string.IsNullOrEmpty(base.Param2)) && string.IsNullOrEmpty(base.UserProvidedConnectionString))
		{
			return false;
		}
		try
		{
			bool? flag = (DatabaseSupportFactory.GetDatabaseSupport(base.Type) as IHiveMetastoreSupport).VerifyIfDatabaseIsHiveMetastore(base.ConnectionString, base.Param2, splashScreenManager, owner);
			if (flag == true)
			{
				errorProvider.SetError(databaseTextEdit, string.Empty);
				return true;
			}
			if (flag != false)
			{
				return false;
			}
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			errorProvider.SetError(databaseTextEdit, "The selected database is not an Hive Metastore database!");
			GeneralMessageBoxesHandling.Show("The selected database <i>" + base.Param2 + "</i> is not an Apache Hive Metastore 3.x database.<br>Please verify the selected value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		catch (Exception ex)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			GeneralMessageBoxesHandling.Show("Unable to connect to server." + Environment.NewLine + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		return false;
	}

	public List<HiveDatabaseInfo> GetHiveDatabases(SplashScreenManager splashScreenManager = null, Form owner = null)
	{
		if ((string.IsNullOrEmpty(base.Host) || string.IsNullOrEmpty(base.Param2)) && string.IsNullOrEmpty(base.UserProvidedConnectionString))
		{
			return null;
		}
		try
		{
			return (DatabaseSupportFactory.GetDatabaseSupport(base.Type) as IHiveMetastoreSupport).GetHiveDatabases(base.ConnectionString, splashScreenManager, base.Param2, owner);
		}
		catch (Exception ex)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			GeneralMessageBoxesHandling.Show("Unable to connect to server." + Environment.NewLine + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		return null;
	}
}
