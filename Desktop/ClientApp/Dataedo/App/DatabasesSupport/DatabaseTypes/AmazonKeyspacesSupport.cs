using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Cassandra;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.AmazonKeyspaces;
using Dataedo.App.Data.General;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class AmazonKeyspacesSupport : CassandraSupport, IDatabaseSupport, IDatabaseSupportShared
{
	public static readonly List<string> SystemDatabases = new List<string> { "system", "system_schema", "system_schema_mcs" };

	public override bool CanGenerateDDLScript => false;

	public override Image TypeImage => Resources.amazon_keyspaces_16;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.AmazonKeyspaces;

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		return "Amazon Keyspaces";
	}

	public override ConnectionBase GetXmlConnectionModel()
	{
		return new AmazonKeyspacesConnection();
	}

	public override SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		return SharedDatabaseTypeEnum.DatabaseType.AmazonKeyspaces;
	}

	public override SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		return new SynchronizeAmazonKeyspaces(synchronizeParameters);
	}

	public override List<string> GetDatabases(string connectionString, SplashScreenManager splashScreenManager, string name, string warehouse, Form owner = null)
	{
		try
		{
			List<string> list = new List<string>();
			using (Cluster cluster = Cluster.Builder().WithConnectionString(connectionString).WithSSL(new SSLOptions().SetCertificateCollection(GetAmazonCertificate()))
				.Build())
			{
				using ISession session = cluster.Connect();
				using (session.Execute(new SimpleStatement("SELECT keyspace_name FROM system_schema.keyspaces;").SetReadTimeoutMillis(CommandsWithTimeout.Timeout * 1000)))
				{
					list.AddRange(session.Cluster.Metadata.GetKeyspaces()?.Except(SystemDatabases));
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

	public override ConnectionResult TryConnection(Func<string> connectionStringBuilder, string name, string user, string warehouse, bool useOnlyRequiredFields = false)
	{
		ISession connection;
		try
		{
			connection = Cluster.Builder().WithConnectionString(connectionStringBuilder()).WithQueryTimeout(CommandsWithTimeout.Timeout * 1000)
				.WithSSL(new SSLOptions().SetCertificateCollection(GetAmazonCertificate()))
				.Build()
				.Connect();
		}
		catch (Exception ex)
		{
			return new ConnectionResult(ex, ex.Message);
		}
		return new ConnectionResult(null, null, connection);
	}

	private X509Certificate2Collection GetAmazonCertificate()
	{
		X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
		X509Certificate2 certificate = new X509Certificate2(Resources.Amazon_Keyspaces_SSL);
		x509Certificate2Collection.Add(certificate);
		return x509Certificate2Collection;
	}
}
