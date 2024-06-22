using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Windows.Forms;
using Cassandra;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.Astra;
using Dataedo.App.Data.General;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class AstraSupport : CassandraSupport, IDatabaseSupport, IDatabaseSupportShared
{
	private const string Username = "username";

	private const string Password = "password";

	private const string Host = "host";

	public override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Astra;

	public new virtual Image TypeImage => Resources.astradb_16;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Astra;

	public override SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.Astra;
	}

	public override string GetConnectionString(string applicationName, string database, string host, string login, string password, int? port, bool withDatabase)
	{
		return new DbConnectionStringBuilder
		{
			{ "username", login },
			{ "password", password },
			{ "host", host }
		}?.ConnectionString;
	}

	public override List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		try
		{
			List<string> list = new List<string>();
			DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder
			{
				ConnectionString = connectionString
			};
			using (Cluster cluster = Cluster.Builder().WithCloudSecureConnectionBundle((string)dbConnectionStringBuilder["host"]).WithCredentials((string)dbConnectionStringBuilder["username"], (string)dbConnectionStringBuilder["password"])
				.WithQueryTimeout(CommandsWithTimeout.Timeout * 1000)
				.Build())
			{
				using ISession session = cluster.Connect();
				using RowSet rowSet = session.Execute(new SimpleStatement("SELECT keyspace_name FROM system_schema.keyspaces;").SetReadTimeoutMillis(CommandsWithTimeout.Timeout * 1000));
				foreach (Row item in rowSet)
				{
					if (item[0] != null && !(item[0] is DBNull))
					{
						list.Add(item[0]?.ToString());
					}
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		return null;
	}

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return base.FriendlyDisplayName;
	}

	public override SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeAstra(synchronizeParameters);
	}

	public override DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.Astra;
	}

	public override ConnectionBase GetXmlConnectionModel()
	{
		return new AstraConnection();
	}

	public override ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder
		{
			ConnectionString = connectionStringBuilder()
		};
		ISession connection;
		try
		{
			connection = Cluster.Builder().WithCloudSecureConnectionBundle((string)dbConnectionStringBuilder["host"]).WithCredentials((string)dbConnectionStringBuilder["username"], (string)dbConnectionStringBuilder["password"])
				.WithQueryTimeout(CommandsWithTimeout.Timeout * 1000)
				.Build()
				.Connect();
		}
		catch (AuthenticationException exception)
		{
			return new ConnectionResult(exception, "Can not authorize. Verify Client key and Secret key.");
		}
		catch (NoHostAvailableException exception2)
		{
			return new ConnectionResult(exception2, "Can not connect to the host. Check if the host is online and available and verify the Client and Secret key.");
		}
		catch (Exception ex)
		{
			return new ConnectionResult(ex, ex.Message);
		}
		return new ConnectionResult(null, null, connection);
	}
}
