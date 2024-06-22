using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Cassandra;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.CosmosDbCassandraAPI;
using Dataedo.App.Data.General;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class CosmosDbCassandraApiSupport : CassandraSupport, IDatabaseSupport, IDatabaseSupportShared
{
	public static readonly List<string> SystemDatabases = new List<string> { "system_auth", "system", "system_schema", "system_distributed", "system_traces" };

	public override bool CanGenerateDDLScript => false;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra;

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Azure Cosmos DB - Cassandra API";
	}

	public override SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra;
	}

	public override SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeCosmosDbCassandraAPI(synchronizeParameters);
	}

	public override DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.CosmosDbCassandra;
	}

	public override ConnectionBase GetXmlConnectionModel()
	{
		return new CosmosDbCassandraApiConnection();
	}

	public override List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		try
		{
			List<string> list = new List<string>();
			CassandraConnectionStringBuilder builder = new CassandraConnectionStringBuilder(connectionString);
			SSLOptions sSLOptions = new SSLOptions(SslProtocols.Tls12, checkCertificateRevocation: true, ValidateServerCertificate);
			sSLOptions.SetHostNameResolver((IPAddress ipAddress) => builder.ContactPoints.FirstOrDefault());
			using (Cluster cluster = Cluster.Builder().WithConnectionString(connectionString).WithSSL(sSLOptions)
				.Build())
			{
				using ISession session = cluster.Connect();
				list.AddRange(session.Cluster.Metadata.GetKeyspaces()?.Except(SystemDatabases));
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

	public override ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		ISession connection;
		try
		{
			CassandraConnectionStringBuilder builder = new CassandraConnectionStringBuilder(connectionStringBuilder());
			SSLOptions sSLOptions = new SSLOptions(SslProtocols.Tls12, checkCertificateRevocation: true, ValidateServerCertificate);
			sSLOptions.SetHostNameResolver((IPAddress ipAddress) => builder.ContactPoints.FirstOrDefault());
			connection = Cluster.Builder().WithConnectionString(connectionStringBuilder()).WithQueryTimeout(CommandsWithTimeout.Timeout * 1000)
				.WithSSL(sSLOptions)
				.Build()
				.Connect();
		}
		catch (Exception ex)
		{
			return new ConnectionResult(ex, ex.Message);
		}
		return new ConnectionResult(null, null, connection);
	}

	public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
		if (sslPolicyErrors == SslPolicyErrors.None)
		{
			return true;
		}
		return false;
	}
}
