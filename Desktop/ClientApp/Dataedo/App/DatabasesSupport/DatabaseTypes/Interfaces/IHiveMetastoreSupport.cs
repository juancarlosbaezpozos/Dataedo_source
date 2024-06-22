using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;

internal interface IHiveMetastoreSupport : IDatabaseSupport, IDatabaseSupportShared
{
	List<HiveDatabaseInfo> GetHiveDatabases(string connectionString, SplashScreenManager splashScreenManager, string databaseName, Form owner = null);

	bool? VerifyIfDatabaseIsHiveMetastore(string connectionString, string databaseName, SplashScreenManager splashScreenManager, Form owner = null);

	ConnectionResult TryConnection(Func<string> connectionStringBuilder, string dbmsDatabaseName, string hiveCatalogName, string hiveDatabaseName, string user, string warehouse, bool useOnlyRequiredFields = false);
}
